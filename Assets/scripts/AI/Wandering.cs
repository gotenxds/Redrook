using UnityEngine;

namespace AI
{
    public class Wandering : Seek
    {
        public override Vector2 NextLocation(Vector2 target, Vector2 position, float maxSpeed)
        {
            return base.NextLocation(target * 10 *(Random.value - Random.value), position, maxSpeed);
        }
    }
}