using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class IdleState : BaseState {
        private Character character;

        public IdleState(Character character) : base(character.gameObject) {
            this.character = character;
        }

        public override Type Tick() {
            bool actionInput = character.CaptureActionInput();

            if (actionInput) {
                return typeof(ActionState);
            }

            bool moving = character.HandleMoveForward();
            if (character.IsGrounded) {
                float jumpInput = character.CurrentJumpInput;
                if (jumpInput != 0) {
                    character.Jump();
                    return typeof(JumpingState);
                }

                if (moving) return typeof(MovingState);
            } else {
                return typeof(FallingState);
            }

            character.animator.SetFloat("MoveSpeed", 0);
            return null;
        }

        public override void OnEnter() {
            character.animator.SetFloat("MoveSpeed", 0);
            character.animator.SetBool("Grounded", true);
        }
    }
}