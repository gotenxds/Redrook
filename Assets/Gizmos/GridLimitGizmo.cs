using UnityEngine;

namespace Gizmos
{
    public class GridLimitGizmo : MonoBehaviour
    {
        [SerializeField] private Color32 color = new Color32(255, 0, 220, 255);
        
        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = color;
            UnityEngine.Gizmos.DrawWireCube(transform.position, new Vector3(100, 100, 1));
        }
    }
}
    