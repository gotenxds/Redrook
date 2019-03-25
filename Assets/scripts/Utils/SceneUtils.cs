using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneUtils
    {
        private const string Path = "Assets/Scenes/landscape/";
        private const string Suffix = ".unity";
        private static readonly Regex reg = new Regex(@"x(\d*)y(\d*)z(\d*)");
        public static Vector3Int GetScenePosition(string sceneName)
        {
            var groups = reg.Matches(sceneName)[0].Groups;
            
            return new Vector3Int(
                int.Parse(groups[1].Value) * Configs.CellSize,
                int.Parse(groups[2].Value) * Configs.CellSize,
                int.Parse(groups[3].Value) * Configs.CellSize
            );
        }
        
        public static Vector3Int GetScenePositionInt(Scene scene)
        {
            return Vector3Int.RoundToInt(scene.GetRootGameObjects()[0].transform.position);
        }
        
        public static Vector3 GetScenePosition(Scene scene)
        {
            return Vector3Int.RoundToInt(scene.GetRootGameObjects()[0].transform.position);
        }
        
        public static string CreateScenePath(Vector3 postion)
        {
            return $"{Path}x{postion.x}y{postion.y}z${postion.z}{Suffix}";
        }
    }
}