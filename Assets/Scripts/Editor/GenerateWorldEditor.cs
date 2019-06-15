using DefaultNamespace;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GridSaver
{
    [CustomEditor(typeof(GenerateWorldMap))]
    public class GenerateWorldEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("GeNeRaTe WoRLd"))
            {
                ((GenerateWorldMap) target).Generate();
            }
        }
    }
}