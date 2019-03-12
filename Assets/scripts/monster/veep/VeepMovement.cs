using AI;
using UnityEngine;

namespace monster.veep
{ 
    public class VeepMovement : BaseMovement
    {
        [SerializeField] private Rigidbody2D target;
        [SerializeField] private float attackDistance;
        protected override void Awake()
        {
            base.Awake();
            movementBehaviour = GetComponent<MovementBehaviour>();
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        protected override void FixedUpdate()
        {
            nextMovement = movementBehaviour.NextLocation(target.position, body.position, maxSpeed);

            base.FixedUpdate();
            
            TryAttack();
        }

        private void TryAttack()
        {
            if (Vector2.Distance(target.position, body.position) <= attackDistance)
            {
                animator.SetTrigger("attack1");
            }
        }

        public override void FlipSpriteIfNeeded()
        {
            if (! nextMovement.Equals(Vector2.zero)) {
                spriteRenderer.flipX = nextMovement.x < 0;
            }
        }
    }
}
