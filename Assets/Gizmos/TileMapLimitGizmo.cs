using UnityEngine;

namespace DefaultNamespace
{
    public class TileMapLimitGizmo : MonoBehaviour
    {
        [SerializeField] private Color32 color = new Color32(255, 0, 220, 255);
        
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, new Vector3(100, 100, 1));
        }
    }
}