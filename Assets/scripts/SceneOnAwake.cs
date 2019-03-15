using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class SceneOnAwake : MonoBehaviour
    {
        private const int CellSize = 100;

        private void Awake()
        {
            setPosition();
        }

        private void OnDrawGizmos()
        {
            setPosition();
        }

        private void setPosition()
        {
            var sceneName = gameObject.scene.name;
            var yIndex = sceneName.IndexOf("y", StringComparison.Ordinal);
            var zIndex = sceneName.IndexOf("z", StringComparison.Ordinal);
            var xValue = float.Parse(sceneName.Substring(1, yIndex - 1));
            var yValue = float.Parse(sceneName.Substring(yIndex + 1, sceneName.Length - zIndex - 1));
            
            transform.position = new Vector3(xValue * CellSize, yValue * CellSize);
        }
    }
}