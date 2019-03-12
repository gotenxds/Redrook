using UnityEngine;

namespace AI
{
    public class Seek : MovementBehaviour
    {
        public override Vector2 NextLocation(Vector2 target, Vector2 position, float maxSpeed)
        {
            return (target - position).normalized * maxSpeed;
        }
    }
}
