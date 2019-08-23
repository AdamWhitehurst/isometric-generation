using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;
namespace FauxGravity {
    #region Required Components 
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(StateMachine))]
    #endregion
    public class ControlledBody : Body {
        public Animator anim { get; private set; }
        public StateMachine stateMachine { get; private set; }

        private float turn;

        private Planet planet;

        private void Awake() {
            InitializeStateMachine();
        }

        private void InitializeStateMachine() {
            var states = new Dictionary<Type, BaseState>() {
                {typeof(FallingState), new FallingState(this)},
                {typeof(IdleState), new IdleState(this)},
                {typeof(MovingState), new MovingState(this)},
                {typeof(JumpingState), new JumpingState(this)},
            };
            stateMachine = GetComponent<StateMachine>();
            stateMachine.SetStates(states);
        }
        void Start() {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            col = GetComponent<CapsuleCollider>();
        }
        /// <summary>
        /// Returns horizontal input based on keys pressed:
        /// Horizontal: A and D, or arrow-left and arrow-right
        /// Vertical: W and S, or arrow-up and arrow-down
        /// </summary>
        public Vector2 CaptureHorizontalInput() {
            return new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }
        /// <summary>
        /// Returns jump input value as float:
        /// Input.GetAxisRaw("Jump")
        /// </summary>
        public float CaptureJumpInput() {
            return Input.GetAxis("Jump");
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            var targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

            // Debug.DrawRay(transform.position, targetUp, Color.red, 0.01f);
        }
        /// <summary>
        /// Handle forward and backward movement of body
        /// <para>Returns Whether a movement occurred</para>
        /// </summary>
        public bool HandleMoveForward() {
            Vector2 horizontalInputs = CaptureHorizontalInput();

            if (horizontalInputs.x != 0) {
                Spin(horizontalInputs.x);
            }

            if (horizontalInputs.y != 0) {
                Vector3 moveForce = horizontalInputs.y * transform.forward * Settings.CharacterSpeed;
                rb.AddForce(moveForce, ForceMode.Acceleration);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates player toward angle
        /// </summary>
        public void Spin(float direction) {
            turn = Mathf.Lerp(turn, direction, Time.deltaTime * 10);
            transform.Rotate(0, turn * Settings.CharacterRotationSpeed * Time.deltaTime, 0);
        }

        public void Jump() {
            rb.AddRelativeForce(Vector3.up * Settings.CharacterJumpHeight, ForceMode.Impulse);
        }
    }
}