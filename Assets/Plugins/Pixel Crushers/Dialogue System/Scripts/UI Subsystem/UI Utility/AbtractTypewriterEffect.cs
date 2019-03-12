// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This is an abstract base typewriter class. It's the ancestor of 
    /// UnityUITypewriterEffect and TextMeshProTypewriterEffect.
    /// </summary>
    public abstract class AbstractTypewriterEffect : MonoBehaviour
    {

        /// <summary>
        /// Set `true` to type right to left.
        /// </summary>
        [Tooltip("Tick for right-to-left text such as Arabic.")]
        public bool rightToLeft = false;

        /// <summary>
        /// How fast to "type."
        /// </summary>
        [Tooltip("How fast to type. This is separate from Dialogue Manager > Subtitle Settings > Chars Per Second.")]
        public float charactersPerSecond = 50;

        /// <summary>
        /// The audio clip to play with each character.
        /// </summary>
        [Tooltip("Optional audio clip to play with each character.")]
        public AudioClip audioClip = null;

        /// <summary>
        /// If specified, randomly use these clips or the main Audio Clip.
        /// </summary>
        [Tooltip("If specified, randomly use these clips or the main Audio Clip.")]
        public AudioClip[] alternateAudioClips = new AudioClip[0];

        /// <summary>
        /// The audio source through which to play the clip. If unassigned, will look for an
        /// audio source on this GameObject.
        /// </summary>
        [Tooltip("Optional audio source through which to play the clip.")]
        public AudioSource audioSource = null;

        /// <summary>
        /// If audio clip is still playing from previous character, stop and restart it when typing next character.
        /// </summary>
        [Tooltip("If audio clip is still playing from previous character, stop and restart it when typing next character.")]
        public bool interruptAudioClip = false;

        /// <summary>
        /// Don't play audio on these characters.
        /// </summary>
        [Tooltip("Don't play audio on these characters.")]
        public string silentCharacters = string.Empty;

        /// <summary>
        /// Duration to pause on when text contains '\\.'
        /// </summary>
        [Tooltip("Duration to pause on when text contains '\\.'")]
        public float fullPauseDuration = 1f;

        /// <summary>
        /// Duration to pause when text contains '\\,'
        /// </summary>
        [Tooltip("Duration to pause when text contains '\\,'")]
        public float quarterPauseDuration = 0.25f;

        /// <summary>
        /// Ensures this GameObject has only one typewriter effect.
        /// </summary>
        [Tooltip("Ensure this GameObject has only one typewriter effect.")]
        public bool removeDuplicateTypewriterEffects = true;

        /// <summary>
        /// Play using the current text content whenever component is enabled.
        /// </summary>
        [Tooltip("Play using the current text content whenever component is enabled.")]
        public bool playOnEnable = true;

        /// <summary>
        /// Wait one frame to allow layout elements to setup first.
        /// </summary>
        [Tooltip("Wait one frame to allow layout elements to setup first.")]
        public bool waitOneFrameBeforeStarting = false;

        /// <summary>
        /// Stop typing when the conversation ends.
        /// </summary>
        [Tooltip("Stop typing when the conversation ends.")]
        public bool stopOnConversationEnd = false;

        public abstract bool isPlaying { get; }

        /// <summary>
        /// Returns the typewriter's charactersPerSecond.
        /// </summary>
        public virtual float GetSpeed()
        {
            return charactersPerSecond;
        }

        /// <summary>
        /// Sets the typewriter's charactersPerSecond. Takes effect the next time the typewriter is used.
        /// </summary>
        public virtual void SetSpeed(float charactersPerSecond)
        {
            this.charactersPerSecond = charactersPerSecond;
        }

        public abstract void Awake();

        public abstract void Start();

        public virtual void OnEnable()
        {
            if (stopOnConversationEnd && DialogueManager.hasInstance)
            {
                DialogueManager.instance.conversationEnded -= StopOnConversationEnd;
                DialogueManager.instance.conversationEnded += StopOnConversationEnd;
            }
        }

        public virtual void OnDisable()
        {
            if (stopOnConversationEnd && DialogueManager.hasInstance)
            {
                DialogueManager.instance.conversationEnded -= StopOnConversationEnd;
            }
        }

        public virtual void StopOnConversationEnd(Transform actor)
        {
            if (isPlaying) Stop();
        }

        public abstract void Stop();

        public abstract void StartTyping(string text, int fromIndex = 0);

        public abstract void StopTyping();
        
        public static string StripRPGMakerCodes(string s) // Moved to UITools, but kept for compatibility with third party code.
        {
            return UITools.StripRPGMakerCodes(s);
        }

    }

}
