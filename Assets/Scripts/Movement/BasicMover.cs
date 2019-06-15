using UnityEngine;

namespace Movement
{
    public class BasicMover: Mover
    {
        protected override void FlipSpriteAsNeeded(Vector2 steering)
        {
            sprite.flipX = steering.x > 0;
        }
    }
}