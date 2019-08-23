using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class IdleState : BaseState {
        private ControlledBody controller;

        public IdleState(ControlledBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {
            bool moving = controller.HandleMoveForward();
            if (controller.IsGrounded()) {
                float jumpInput = controller.CaptureJumpInput();
                if (jumpInput != 0) {
                    controller.Jump();
                    return typeof(JumpingState);
                }

                if (moving) return typeof(MovingState);
            } else {
                return typeof(FallingState);
            }

            controller.anim.SetFloat("MoveSpeed", 0);
            return null;
        }

        public override void OnEnter() {
            controller.anim.SetFloat("MoveSpeed", 0);
            controller.anim.SetBool("Grounded", true);
        }
    }
}