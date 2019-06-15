using System;
using UnityEngine;

namespace Movement
{
    public abstract class Mover : MonoBehaviour
    {
        protected Rigidbody2D rigidBody;
        protected SpriteRenderer sprite;
        protected Animator animator;

        private bool movementLocked = false;
        private Vector2 lastVelocity;
        private Vector2 lastSteering;
        private Vector2 steeredVelocity;

        private static readonly int Vertical = Animator.StringToHash("vertical");
        private static readonly int Horizontal = Animator.StringToHash("horizontal");
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        
        protected virtual void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            rigidBody.velocity = steeredVelocity * Time.deltaTime;
        }

        protected void Move(Vector2 velocity, Vector2 linearSteering)
        {
            if (linearSteering.x != 0 && linearSteering.y != 0)
            {
                //linearSteering /= 2;
            }
            lastVelocity = velocity;
            lastSteering = linearSteering;
            
            if (movementLocked)
            {                
                return;
            }

            if (velocity.Equals(Vector2.zero))
            {
                Idle();
                return;
            }

            animator.SetBool(IsIdle, false);
            steeredVelocity = velocity * linearSteering;

            FlipSpriteAsNeeded(linearSteering);
            animator.SetFloat(Vertical, linearSteering.y);
            animator.SetFloat(Horizontal, linearSteering.x);
        }

        protected void Idle()
        {
            lastVelocity = Vector2.zero;

            if (movementLocked) return;

            animator.SetBool(IsIdle, true);
            steeredVelocity = Vector2.zero;
        }

        protected void LockMovement()
        {
            movementLocked = true;
        }
        
        protected void UnLockMovement()
        {
            movementLocked = false;
            Move(lastVelocity, lastSteering);
        }

        protected abstract void FlipSpriteAsNeeded(Vector2 steering);
    }
}