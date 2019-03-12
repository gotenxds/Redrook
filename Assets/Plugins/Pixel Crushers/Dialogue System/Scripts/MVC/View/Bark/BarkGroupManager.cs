// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This singleton GameObject manages bark groups specified by BarkGroupMember.
    /// This is a GameObject to make it easy to inspect in the editor.
    /// </summary>
    [AddComponentMenu("")] // Added automatically at runtime.
    public class BarkGroupManager : MonoBehaviour
    {

        private static bool s_applicationIsQuitting = false;
        private static BarkGroupManager s_instance = null;
        public static BarkGroupManager instance
        {
            get
            {
                if (s_applicationIsQuitting) return null;
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<BarkGroupManager>();
                    if (s_instance == null)
                    {
                        s_instance = new GameObject("Bark Group Manager").AddComponent<BarkGroupManager>();
                        DontDestroyOnLoad(s_instance.gameObject);
                    }
                }
                return s_instance;
            }
        }

        public Dictionary<string, HashSet<BarkGroupMember>> groups = new Dictionary<string, HashSet<BarkGroupMember>>();

        private void OnApplicationQuit()
        {
            s_applicationIsQuitting = true;
        }

        public void AddToGroup(string groupId, BarkGroupMember member)
        {
            if (string.IsNullOrEmpty(groupId) || member == null) return;
            if (!groups.ContainsKey(groupId)) groups.Add(groupId, new HashSet<BarkGroupMember>());
            groups[groupId].Add(member);
        }

        public void RemoveFromGroup(string groupId, BarkGroupMember member)
        {
            if (string.IsNullOrEmpty(groupId) || member == null) return;
            if (!groups.ContainsKey(groupId)) return;
            if (!groups[groupId].Contains(member)) return;
            groups[groupId].Remove(member);
            if (groups[groupId].Count == 0) groups.Remove(groupId);
        }

        public void MutexBark(string groupId, BarkGroupMember member)
        {
            if (string.IsNullOrEmpty(groupId) || member == null) return;
            if (!groups.ContainsKey(groupId)) return;
            foreach (var other in groups[groupId])
            {
                if (other == member) continue;
                other.CancelBark();
            }
        }

    }
}
