using UnityEngine;

namespace AI
{
    [DisallowMultipleComponent]
    public abstract class MovementBehaviour : MonoBehaviour
    {
        public abstract Vector2 NextLocation(Vector2 target, Vector2 position, float maxSpeed);
    }
}
