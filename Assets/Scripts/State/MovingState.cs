using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class MovingState : BaseState {
        private Character character;
        public MovingState(Character character) : base(character.gameObject) {
            this.character = character;
        }

        public override Type Tick() {

            bool actionInput = character.CaptureActionInput();

            if (actionInput) {
                return typeof(ActionState);
            }

            Vector2 horizontalInputs = character.CurrentHorizontalInputs();
            float jumpInput = character.CurrentJumpInput;

            character.animator.SetFloat("MoveSpeed", horizontalInputs.y);

            var moving = character.HandleMoveForward();
            var grounded = character.IsGrounded;
            if (jumpInput != 0 && grounded) {
                character.Jump();
                return typeof(FallingState);
            }
            if (moving && !grounded) return typeof(FallingState);
            if (moving) return null;
            return typeof(IdleState);

        }

        public override void OnEnter() {
            character.animator.SetBool("Grounded", true);
        }
    }
}