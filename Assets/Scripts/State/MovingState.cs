using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class MovingState : BaseState {
        private ControlledBody controller;
        public MovingState(ControlledBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {
            Vector2 horizontalInputs = controller.CaptureHorizontalInput();
            float jumpInput = controller.CaptureJumpInput();

            controller.anim.SetFloat("MoveSpeed", horizontalInputs.y);

            bool moving = controller.HandleMoveForward();

            if (jumpInput != 0 && controller.IsGrounded()) {
                controller.Jump();
                return typeof(FallingState);
            }
            if (moving && !controller.IsGrounded()) return typeof(FallingState);
            if (moving) return null;
            return typeof(IdleState);

        }

        public override void OnEnter() {
            controller.anim.SetBool("Grounded", true);
        }
    }
}