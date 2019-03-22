using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace player
{
    public class SceneLoader : MonoBehaviour
    {
        private Rigidbody2D body;
        private readonly float loadRadius = Configs.CellSize / 3; 
        private readonly float forgetRadius = Configs.CellSize / 2; 
        

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void LateUpdate()
        {
            LoadScenes();
        }

        private void LoadScenes()
        {
            var position = body.position;

            var scenePosition = SceneUtils.GetScenePosition(gameObject.scene.name);

            Configs.Directions
                .Select(direction => new Vector3(position.x + direction.Item1, position.y + direction.Item2))
                .Where(pos => Vector2.Distance(pos, position) < loadRadius)
                .Select(sceneName => SceneManager.LoadSceneAsync(SceneUtils.CreateScenePath(sceneName)));
        }
    }
}