using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class SceneOnAwake : MonoBehaviour
    {
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

            transform.position = SceneUtils.GetScenePosition(sceneName);
        }
    }
}