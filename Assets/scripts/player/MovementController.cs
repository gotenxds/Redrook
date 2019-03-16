using System;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace player
{
    public class MovementController : CharacterMovement
    {
        [SerializeField] private Controls controls;
        [SerializeField] private float rollingSpeed;
        [SerializeField] private float rollCooldown;
        
        private Vector2 velocity;
        private Vector2 rollingVelocity;
        private int framesInIdle;
        private float lastHorizontal;
        private float lastVertical;
        private bool isIdle;
        private float nextRoll;
        private bool isRolling;
        private int framesInRoll;
        
        private void OnEnable()
        {
            controls.Player.Enable();
        }

        private void OnDisable()
        {
            controls.Player.Disable();
        }

        protected override void Awake()
        {
            base.Awake();

            controls.Player.Move.performed += MoveEvent;
            controls.Player.Move.cancelled += MoveEvent;
            controls.Player.Roll.performed += TryRoll;
            
            velocity = new Vector2(maxSpeed, maxSpeed);
            rollingVelocity = new Vector2(rollingSpeed, rollingSpeed);
        }

        protected override void FixedUpdate()
        {
            if (!isIdle)
            {
                spriteRenderer.flipX = lastHorizontal > 0;
            }
            
            if (isRolling && !IsRollingAnimationRunning())
            {
                if (framesInRoll > 0)
                {
                    OnRollEnd();
                }

                framesInRoll++;
            }
        }

        private void MoveEvent(InputAction.CallbackContext ctx)
        {
            if (isRolling) return;
            
            isIdle = ctx.cancelled;
            Debug.Log("Move" + isIdle);
            var axis = ctx.ReadValue<Vector2>();
            lastHorizontal = (float) Math.Round(axis.x);
            lastVertical = (float) Math.Round(axis.y);
                
            SetAnimatorDirections();
            Move();
        }

        private void Move()
        {
            base.Move(isRolling ? rollingVelocity : velocity, new Vector2(lastHorizontal, lastVertical));
        }

        private void SetAnimatorDirections()
        {
            animator.SetFloat("horizontal", IsIdle() ? 0 : lastHorizontal);
            animator.SetFloat("vertical", IsIdle() ? 0 : lastVertical);

            if (IsIdle()) return;
        
            animator.SetFloat("lastHorizontal", lastHorizontal);
            animator.SetFloat("lastVertical", lastVertical);
        }

        private void TryRoll(InputAction.CallbackContext ctx)
        {
            var rollCooldownOver = Time.time > nextRoll;

            if(isRolling || isIdle || !rollCooldownOver) return;
            
            controls.Player.Move.Disable();
            isRolling = true;
            framesInRoll = 0;
            nextRoll = Time.time + rollCooldown;
            animator.SetTrigger("roll");
            Move();
            
        }

        private bool IsIdle()
        {
            return isIdle && !isRolling;
        }

        private bool IsRollingAnimationRunning()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag("roll");
        }
        
        private void OnRollEnd()
        {
            controls.Player.Move.Enable();
            isRolling = false;
            Move(Vector2.zero);
            Debug.Log("EndRoll");
        }
    }
}
