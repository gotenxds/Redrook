using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class CharacterMover : BasicMover
    {
        [SerializeField]
        private Vector2 speed = new Vector2(100, 100);
        
        private Controls controls;

        [SerializeField]
        private float rollCooldown = 5;
        private float lastRoll;
        private static readonly int IsRolling = Animator.StringToHash("isRolling");

        private void OnEnable()
        {
            controls.Player.Enable();
        }

        protected override void Awake()
        {
            base.Awake();
            controls = new Controls();
            controls.Player.Move.performed += OnMovePerformed;
            controls.Player.Move.canceled += OnMoveCancelled;
            controls.Player.Roll.performed += OnRollPerformed;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var steering = context.ReadValue<Vector2>();

            Move(speed, Vector2.ClampMagnitude(steering, 1f));
        }
        
        private void OnMoveCancelled(InputAction.CallbackContext context)
        {
            Idle();
        }
        
        private void OnRollPerformed(InputAction.CallbackContext context)
        {
            LockMovement();
            controls.Player.Roll.Disable();
            animator.SetBool(IsRolling, true);
        }     
        
        public void OnRollFinished()
        {
            StartCoroutine(RollFinished());
        }
        
        private IEnumerator RollFinished()
        {
            UnLockMovement();
            animator.SetBool(IsRolling, false);
            
            yield return new WaitForSeconds(rollCooldown);
            controls.Player.Roll.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
    }
}