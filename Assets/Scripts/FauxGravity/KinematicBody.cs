using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;
using Worlds;

namespace FauxGravity {
    #region Required Components 
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(StateMachine))]
    #endregion
    public class KinematicBody : Body {
        public Animator anim { get; private set; }
        public StateMachine stateMachine { get; private set; }

        public FloatReference inputDelayMS;

        public FloatReference jumpSpeedModifier;
        public FloatReference moveSpeed;

        public FloatReference rotateSpeed;
        public FloatReference jumpForce;
        public FloatReference maxCastDistance;
        public FloatReference horizontalRayDistanceModifier;
        public FloatReference sphereCastRadius;

        private float turn;

        private Planet planet;

        // private float curInputDelay = 0f;


        // private void Awake() {
        //     InitializeStateMachine();
        // }

        // private void InitializeStateMachine() {
        //     // var states = new Dictionary<Type, BaseState>() {
        //     //     {typeof(FallingState), new FallingState(this)},
        //     //     {typeof(IdleState), new IdleState(this)},
        //     //     {typeof(MovingState), new MovingState(this)},
        //     //     {typeof(JumpingState), new JumpingState(this)},
        //     //     {typeof(ActionState), new ActionState(this)},
        //     // };
        //     // stateMachine = GetComponent<StateMachine>();
        //     // stateMachine.SetStates(states);
        // }
        // void Start() {
        //     rb = GetComponent<Rigidbody>();
        //     rb.isKinematic = true;
        //     anim = GetComponent<Animator>();
        //     col = GetComponent<CapsuleCollider>();
        // }

        // void Update() {
        //     curInputDelay += Time.deltaTime * 1000;
        // }

        // void OnCollisionEnter(Collision other) {
        //     Debug.Log(other.gameObject.name);
        // }


        // /// <summary>
        // /// Returns horizontal input based on keys pressed:
        // /// Horizontal: A and D, or arrow-left and arrow-right
        // /// Vertical: W and S, or arrow-up and arrow-down
        // /// </summary>
        // public Vector2 CaptureHorizontalInput() {
        //     return new Vector2(
        //         Input.GetAxisRaw("Horizontal"),
        //         Input.GetAxisRaw("Vertical")
        //     );
        // }
        // /// <summary>
        // /// Returns jump input value as float:
        // /// Input.GetAxisRaw("Jump")
        // /// </summary>
        // public float CaptureJumpInput() {
        //     return Input.GetAxis("Jump");
        // }

        // /// <summary>
        // /// Returns whether actionInput has been pressed
        // /// </summary>
        // public bool CaptureActionInput() {
        //     if (curInputDelay >= inputDelayMS && Input.GetKeyDown(KeyCode.C)) {
        //         curInputDelay = 0;
        //         return true;
        //     }
        //     return false;
        // }

        // public void HandleAction() {
        //     RaycastHit hit;
        //     if (Physics.Raycast(col.bounds.center, Input.GetKey(KeyCode.LeftShift) ? -transform.up : transform.forward, out hit, 1f, LayerMask.GetMask("World"))) {
        //         var modifiable = hit.transform.parent.GetComponent<IModifiable>();
        //         Debug.DrawRay(col.bounds.center, transform.forward, Color.cyan, 0.01f);
        //         if (modifiable != null) {
        //             Shaper.ReplaceBlockAtRaycast(modifiable.planet, hit, null);
        //         }
        //     }
        // }
        // /// <summary>
        // /// Handle forward and backward movement of body
        // /// <para>Returns Whether a movement occurred</para>
        // /// </summary>
        // public bool HandleMoveForward() {
        //     Vector2 horizontalInputs = CaptureHorizontalInput();

        //     if (horizontalInputs.x != 0) {
        //         Spin(horizontalInputs.x);
        //     }

        //     if (horizontalInputs.y != 0) {
        //         Vector3 moveForce = horizontalInputs.y * Vector3.forward * moveSpeed;
        //         if (horizontalInputs.y > 0 && CanMoveForward()) transform.Translate(moveForce * Time.fixedDeltaTime);
        //         if (horizontalInputs.y < 0 && CanMoveBackward()) transform.Translate(moveForce * Time.fixedDeltaTime);
        //         return true;
        //     }

        //     return false;
        // }

        // /// <summary>
        // /// Rotates player toward angle
        // /// </summary>
        // public void Spin(float direction) {
        //     turn = Mathf.Lerp(turn, direction, Time.deltaTime * 10);
        //     transform.Rotate(0, turn * rotateSpeed * Time.deltaTime, 0);
        // }

        // public void Jump() {
        //     if (CanMoveUpward()) {
        //         StartCoroutine(JumpRoutine());
        //     }
        // }

        // public IEnumerator JumpRoutine() {
        //     float curJumpHeight = 0;
        //     while (CanMoveUpward() && curJumpHeight <= jumpForce) {
        //         transform.Translate(Vector3.up * jumpSpeedModifier);
        //         curJumpHeight += 1 * jumpSpeedModifier;
        //         yield return null;
        //     }
        // }

        // public override void AddGravityForce(Vector3 gravity) {
        //     transform.Translate(-Vector3.up * gravity.magnitude * Time.fixedDeltaTime);

        //     // Debug.DrawRay(transform.position, -transform.up, Color.green, 0.01f);
        // }

        // // public override bool IsGrounded() {
        // //     RaycastHit hit;
        // //     if (Physics.SphereCast(col.bounds.center, sphereCastRadius, -transform.up, out hit, maxCastDistance, LayerMask.GetMask("World"))) {
        // //         return true;
        // //     }

        // //     return false;
        // // }


        // public bool CanMoveForward() {
        //     RaycastHit hit;
        //     if (Physics.SphereCast(col.bounds.center, sphereCastRadius, transform.forward, out hit, maxCastDistance * horizontalRayDistanceModifier, LayerMask.GetMask("World"))) {
        //         Debug.DrawRay(col.bounds.center, transform.forward, Color.blue, 0.001f);
        //         return false;
        //     }
        //     return true;
        // }

        // public bool CanMoveBackward() {
        //     RaycastHit hit;
        //     if (Physics.SphereCast(col.bounds.center, sphereCastRadius, -transform.forward, out hit, maxCastDistance * horizontalRayDistanceModifier, LayerMask.GetMask("World"))) {
        //         Debug.DrawRay(col.bounds.center, -transform.forward, Color.green, 0.001f);
        //         return false;
        //     }
        //     return true;
        // }

        // public bool CanMoveUpward() {
        //     RaycastHit hit;
        //     if (Physics.SphereCast(col.bounds.center, sphereCastRadius, transform.up, out hit, maxCastDistance, LayerMask.GetMask("World"))) {
        //         Debug.DrawRay(col.bounds.center, transform.up, Color.cyan, 0.001f);
        //         return false;
        //     }
        //     return true;
        // }
        // public bool CanMoveDownward() {
        //     RaycastHit hit;
        //     if (Physics.SphereCast(col.bounds.center, sphereCastRadius, -transform.up, out hit, maxCastDistance, LayerMask.GetMask("World"))) {
        //         Debug.DrawRay(col.bounds.center, transform.up, Color.magenta, 0.001f);
        //         return false;
        //     }
        //     return true;

        // }
    }
}