// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: "LookAt(target, [, subject[, duration[, allAxes]]])", which rotates the 
    /// subject to face the target.
    /// 
    /// Arguments:
    /// -# Target to look at. Can be speaker, listener, or the name of a game object. Default: listener.
    /// -# (Optional) The subject; can be speaker, listener, or the name of a game object. Default: speaker.
    /// -# (Optional) Duration in seconds.
    /// -# (Optional) allAxes to rotate on all axes, otherwise stays upright.
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandLookAt : SequencerCommand
    {

        private const float SmoothMoveCutoff = 0.05f;

        private Transform target;
        private Transform subject;
        private float duration;
        float startTime;
        float endTime;
        Quaternion originalRotation;
        Quaternion targetRotation;
        Vector3 targetPosition;

        public void Start()
        {
            // Get the values of the parameters:
            target = GetSubject(0, sequencer.listener);
            subject = GetSubject(1);
            duration = GetParameterAsFloat(2, 0);
            bool yAxisOnly = (string.Compare(GetParameter(3), "allAxes", System.StringComparison.OrdinalIgnoreCase) != 0);

            if (DialogueDebug.logInfo) Debug.Log(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: Sequencer: LookAt({1}, {2}, {3})", new System.Object[] { DialogueDebug.Prefix, target, subject, duration }));
            if ((target == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: Target '{1}' wasn't found.", new System.Object[] { DialogueDebug.Prefix, GetParameter(0) }));
            if ((subject == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: Subject '{1}' wasn't found.", new System.Object[] { DialogueDebug.Prefix, GetParameter(1) }));

            // Set up the move:
            if ((subject != null) && (target != null) && (subject != target))
            {

                // If duration is above the cutoff, smoothly rotate toward target:
                if (duration > SmoothMoveCutoff)
                {
                    startTime = DialogueTime.time;
                    endTime = startTime + duration;
                    originalRotation = subject.rotation;
                    targetPosition = (yAxisOnly)
                        ? new Vector3(target.position.x, subject.position.y, target.position.z)
                        : target.position;
                    targetRotation = Quaternion.LookRotation(targetPosition - subject.position, Vector3.up);
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }


        public void Update()
        {
            // Keep smoothing for the specified duration:
            if (DialogueTime.time < endTime)
            {
                float elapsed = (DialogueTime.time - startTime) / duration;
                subject.rotation = Quaternion.Lerp(originalRotation, targetRotation, elapsed);
            }
            else
            {
                Stop();
            }
        }

        public void OnDestroy()
        {
            // Final rotation:
            if (subject != null && target != null)
            {
                subject.LookAt(targetPosition);
            }

        }

    }

}
