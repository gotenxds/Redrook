using UnityEngine;

namespace AI
{
    public class Arriving : MovementBehaviour
    {
        [SerializeField] private float satisfactionRadius;
        [SerializeField] private float timeToTarget;

        public override Vector2 NextLocation(Vector2 target, Vector2 position, float maxSpeed)
        {
            var velocity = (target - position) / timeToTarget;
            
            if (velocity.magnitude < satisfactionRadius) return Vector2.zero;

            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            return velocity;
        }
    }
}