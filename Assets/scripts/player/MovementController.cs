using UnityEngine;

namespace player
{
    public class MovementController : CharacterMovement
    {
        [SerializeField] private float rollingSpeed;
        [SerializeField] private float rollCooldown;
        
        private Vector2 velocity;
        private Vector2 rollingVelocity;
        private int framesInIdle;
        private int lastHorizontal;
        private float nextRoll;

        protected override void Awake()
        {
            base.Awake();
//            controls.Enable();
//
//            controls.Player.Move.performed += ctx =>
//            {
//                //Debug.Log(ctx.ReadValue<Vector2>());
//            };
            velocity = new Vector2(maxSpeed, maxSpeed);
            rollingVelocity = new Vector2(rollingSpeed, rollingSpeed);
        }

        protected override void FixedUpdate()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            FlipSpriteIfNeeded();

            TryIdle(horizontal, vertical);
            TryRoll();
        
            SetAnimatorDirections(horizontal, vertical);
        
            Move();
        }

        public void Move()
        {
            if (IsRollingAnimationRunning())
            {
                base.Move(rollingVelocity, new Vector2(animator.GetFloat("lastHorizontal"), animator.GetFloat("lastVertical")));
            }
            else
            {
                base.Move(velocity, new Vector2(animator.GetFloat("horizontal"), animator.GetFloat("vertical")));
            }
        }

        private void TryIdle(float horizontal, float vertical)
        {
            framesInIdle = !IsRollingAnimationRunning() && (int)horizontal == 0 && (int) vertical == 0 ? framesInIdle + 1 : 0;
        }

        private void SetAnimatorDirections(float horizontal, float vertical)
        {
            animator.SetFloat("horizontal", horizontal);
            animator.SetFloat("vertical", vertical);

            if (IsIdle() || IsRollingAnimationRunning()) return;
        
            animator.SetFloat("lastHorizontal", horizontal);
            animator.SetFloat("lastVertical", vertical);
        }

        private void TryRoll()
        {
            var pressedRoll = Input.GetAxisRaw("Roll") > 0;
            var rollCooldownOver = Time.time > nextRoll;
        
            if (pressedRoll && rollCooldownOver && !IsIdle() && !IsRollingAnimationRunning())
            {
                animator.SetTrigger("roll");
                nextRoll = Time.time + rollCooldown;
            }
        }

        private bool IsIdle()
        {
            return framesInIdle > 0;
        }

        public override void FlipSpriteIfNeeded()
        {
            float horizontal;

            if (IsIdle() || IsRollingAnimationRunning())
            {
                horizontal = animator.GetFloat("lastHorizontal");
            }
            else
            {
                horizontal = animator.GetFloat("horizontal");
            }
        
            spriteRenderer.flipX = horizontal > 0;
        }

        private bool IsRollingAnimationRunning()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag("roll");
        }
    }
}
