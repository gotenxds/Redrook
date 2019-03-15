using System.Linq;
using GitHub.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class GenerateScences
    {
        private const string path = "Assets/Scenes/landscape/";

        [MenuItem("Tools/David/Generate Scenes")]
        static void Init()
        {
            var prefab = Resources.Load<GameObject>("prefabs/Root");
                            
            var amount = Enumerable.Range(0, 100).ToList();
        
            foreach (var row in amount)
            {
                foreach (var cell in amount)
                {
                    var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    
                    PrefabUtility.InstantiatePrefab(prefab, newScene);
                    
                    EditorSceneManager.SaveScene(newScene, $"{path}/x{row}y{cell}z0.unity");
                }
            }
        }
    }
}