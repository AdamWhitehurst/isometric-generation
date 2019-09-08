using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;
using Worlds;

namespace FauxGravity {
    #region Required Components 
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(StateMachine))]
    #endregion
    public class Character : Body {

        #region Fields 
        public Animator animator { get; protected set; }
        public StateMachine stateMachine { get; protected set; }
        public Quaternion TargetRotation => _targetRotation;
        public Vector2 CurrentHorizontalInputs() => new Vector2(_turnInput, _forwardInput);

        public float CurrentJumpInput => _jumpInput;
        [SerializeField] private FloatReference _inputDelayMS = new FloatReference();
        [SerializeField] private FloatReference _groundMoveSpeed = new FloatReference();
        [SerializeField] private FloatReference _airMoveSpeed = new FloatReference();
        [SerializeField] private FloatReference _jumpForce = new FloatReference();

        [SerializeField] private FloatReference _rotateSpeed = new FloatReference();


        [SerializeField] private float _curInputDelay = 0f;
        [SerializeField] private Quaternion _targetRotation = new Quaternion();
        [SerializeField] private float _forwardInput, _turnInput, _jumpInput;

        #endregion
        private void Awake() {
            InitializeStateMachine();
        }
        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
        }

        void Update() {
            CaptureInputs();
        }


        private void InitializeStateMachine() {
            var states = new Dictionary<Type, BaseState>() {
                {typeof(FallingState), new FallingState(this)},
                {typeof(IdleState), new IdleState(this)},
                {typeof(MovingState), new MovingState(this)},
                {typeof(JumpingState), new JumpingState(this)},
                {typeof(ActionState), new ActionState(this)},
            };
            stateMachine = GetComponent<StateMachine>();
            stateMachine.SetStates(states);
        }

        private void CaptureInputs() {
            _curInputDelay += Time.deltaTime * 1000;
            _forwardInput = Input.GetAxisRaw("Vertical");
            _turnInput = Input.GetAxisRaw("Horizontal");
            _jumpInput = Input.GetAxis("Jump");
        }

        public bool CaptureActionInput() {
            if (_curInputDelay >= _inputDelayMS && Input.GetKeyDown(KeyCode.C)) {
                _curInputDelay = 0;
                return true;
            }
            return false;
        }

        public void HandleAction() {
            RaycastHit hit;
            Debug.DrawRay(bodyCollider.bounds.center, transform.forward, Color.cyan, 0.1f);
            if (Physics.Raycast(bodyCollider.bounds.center, transform.forward, out hit, 0.5f, LayerMask.GetMask("World"))) {
                var modifiable = hit.transform.GetComponent<IModifiable>();
                if (modifiable != null) {
                    Shaper.ReplaceBlockAtRaycast(modifiable.planet, hit, null);
                }
            }
        }

        public bool HandleMoveForward() {
            if (_turnInput != 0) {
                Turn();
            }

            if (_forwardInput != 0) {
                if (IsGrounded) rigidBody.AddForce(transform.forward * _forwardInput * _groundMoveSpeed, ForceMode.Force);
                else rigidBody.AddForce(transform.forward * _forwardInput * _airMoveSpeed, ForceMode.Force);
                return true;
            }

            return false;
        }

        public void Turn() {
            rigidBody.rotation *= Quaternion.AngleAxis(_rotateSpeed * _turnInput * Time.deltaTime, Vector3.up);
        }

        public void Jump() {
            rigidBody.AddRelativeForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}