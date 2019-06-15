using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GridSaver
{
    [CustomEditor(typeof(Saver))]
    public class SaverEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("Save"))
            {
                var saver = (Saver) target;
                saver.Save();
                EditorSceneManager.SaveScene(saver.gameObject.scene);
            }
        }
    }
}