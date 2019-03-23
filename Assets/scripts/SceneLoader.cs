using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNameNspace
{
    [ExecuteInEditMode]
    public class SceneLoader : MonoBehaviour
    {
        private const string Path = "Assets/Scenes/landscape/";
        private const string Suffix = ".unity";
        private static Dictionary<string, Scene> currentScenes = new Dictionary<string, Scene>();
        private static readonly List<(short, short)> Directions = new List<(short, short)>
        {
            (1, 0), (1, 1), (1, -1),
            (-1, 0), (-1, 1), (-1, -1),
            (0, 0), (0, 1), (0, -1)
        };

        private void OnDrawGizmos()
        {
            var cameraPosOptional = GetCameraPosition();

            if (!cameraPosOptional.HasValue) return;
            var cameraPos = cameraPosOptional.GetValueOrDefault();

            ShowDebug(cameraPos);
            LoadScenes(cameraPos);
        }

        private static void LoadScenes(Vector3 cameraPos)
        {
            var x = Math.Round(cameraPos.x / 100);
            var y = Math.Round(cameraPos.y / 100);

            var newScenes = new Dictionary<string, Scene>();
            Directions
                .Select(direction => CreateScenePath(direction, x, y)).ToList()
                .ForEach(path =>
                {
                    if (currentScenes.ContainsKey(path))
                    {
                        newScenes.Add(path, currentScenes[path]);
                        currentScenes.Remove(path);
                    }
                    else
                    {
                        newScenes.Add(path, EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
                    }
                });

          
            currentScenes.Values.ToList().ForEach(scene =>
            {
                    if (scene.isDirty)
                    {
                        EditorSceneManager.SaveScene(scene);    
                    }
                    
                    EditorSceneManager.CloseScene(scene, true);
            });

            currentScenes = newScenes;
        }

        private static string CreateScenePath((short, short) direction, double x, double y)
        {
            return $"{Path}x{x + direction.Item1}y{y + direction.Item2}z0{Suffix}";
        }

        private static void ShowDebug(Vector3 cameraPos)
        {
            Debug.Log(cameraPos);
            Gizmos.DrawCube(new Vector3(cameraPos.x, cameraPos.y), new Vector3(10, 10, 10));
        }

        private static Vector3? GetCameraPosition()
        {
            var current = Camera.current;
            
            return current != null ? current.transform.position : (Vector3?) null;
        }
    }
}