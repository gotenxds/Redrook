// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This is a saver that saves the Dialogue System's save data 
    /// to the Pixel Crushers Common Library Save System.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DialogueSystemSaver : Saver
    {

        public override string RecordData()
        {
            return PersistentDataManager.GetSaveData();
        }

        public override void ApplyData(string data)
        {
            PersistentDataManager.ApplySaveData(data);
        }

        public override void OnBeforeSceneChange()
        {
            PersistentDataManager.LevelWillBeUnloaded();
        }

    }

}
