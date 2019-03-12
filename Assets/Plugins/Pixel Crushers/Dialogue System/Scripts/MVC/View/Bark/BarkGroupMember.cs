// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// A member of a bark group. Barks are mutually exclusive within a bark group.
    /// When one member barks, the other members hide their active barks.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class BarkGroupMember : MonoBehaviour
    {

        /// <summary>
        /// Member of this group. Can be a Lua expression.
        /// </summary>
        [Tooltip("Member of this group. Can be a Lua expression.")]
        public string groupId;

        /// <summary>
        /// Evaluate Group Id before every bark. Useful if Id is a Lua expression that can change value.
        /// </summary>
        [Tooltip("Evaluate Group Id before every bark. Useful if Id is a Lua expression that can change value.")]
        public bool evaluateIdEveryBark = true;

        /// <summary>
        /// When another group member forces this member's bark to hide, delay this many seconds before hiding.
        /// </summary>
        [Tooltip("When another group member forces this member's bark to hide, delay this many seconds before hiding.")]
        public float forcedHideDelay = 0;

        private string m_currentIdValue = string.Empty;
        private IBarkUI m_barkUI = null;
        private bool m_ignoreOnDisable = false;

        private IBarkUI barkUI
        {
            get
            {
                if (m_barkUI == null) m_barkUI = GetComponentInChildren(typeof(IBarkUI)) as IBarkUI;
                return m_barkUI;
            }
        }

        private void OnApplicationQuit()
        {
            m_ignoreOnDisable = true;
        }

        private void OnLevelWillBeUnloaded()
        {
            m_ignoreOnDisable = true;
        }

        private void OnDisable()
        {
            if (m_ignoreOnDisable || BarkGroupManager.instance == null) return;
            BarkGroupManager.instance.RemoveFromGroup(m_currentIdValue, this);
        }

        private void OnBarkStart(Transform listener)
        {
            if (string.IsNullOrEmpty(m_currentIdValue) || evaluateIdEveryBark)
            {
                UpdateMembership();
            }
            BarkGroupManager.instance.MutexBark(m_currentIdValue, this);
        }

        public void UpdateMembership()
        {
            var newIdValue = Lua.Run("return " + groupId, DialogueDebug.logInfo, false).asString;
            if (string.Equals(newIdValue, "nil")) newIdValue = groupId;
            if (newIdValue != m_currentIdValue)
            {
                BarkGroupManager.instance.RemoveFromGroup(m_currentIdValue, this);
                BarkGroupManager.instance.AddToGroup(newIdValue, this);
                m_currentIdValue = newIdValue;
            }
        }

        public void CancelBark()
        {
            if (barkUI == null || !barkUI.isPlaying) return;
            CancelInvoke("HideBarkNow");
            Invoke("HideBarkNow", forcedHideDelay);
        }

        private void HideBarkNow()
        {
            if (barkUI == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Didn't find a bark UI on " + name, this);
            }
            else if (barkUI.isPlaying)
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Hiding bark on " + name, this);
                barkUI.Hide();
            }
        }

    }
}