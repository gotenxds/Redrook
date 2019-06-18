using UnityEditor;
using UnityEngine;

namespace GridSaver
{
    [CustomEditor(typeof(Loader))]
    public class LoaderEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}