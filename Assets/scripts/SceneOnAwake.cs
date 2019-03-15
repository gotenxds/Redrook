using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class SceneOnAwake : MonoBehaviour
    {
        private const int CellSize = 100;
        private readonly Regex reg = new Regex(@"x(\d*)y(\d*)z(\d*)");
       
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
            var groups = reg.Matches(sceneName)[0].Groups;

            transform.position = new Vector3(float.Parse(groups[1].Value) * CellSize, float.Parse(groups[2].Value)  * CellSize);
        }
    }
}