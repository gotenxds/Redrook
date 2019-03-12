using UnityEngine;

namespace AI
{
    public class Flee : Seek
    {
        public override Vector2 NextLocation(Vector2 target, Vector2 position, float maxSpeed)
        {
            return base.NextLocation(position, target, maxSpeed);
        }
    }
}
