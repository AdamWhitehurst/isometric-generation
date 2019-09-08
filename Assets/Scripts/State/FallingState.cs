using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class FallingState : BaseState {
        private Character character;


        public FallingState(Character character) : base(character.gameObject) {
            this.character = character;
        }

        public override Type Tick() {
            var grounded = character.IsGrounded;
            if (!grounded) {
                character.HandleMoveForward();
                return null;
            }
            return typeof(IdleState);
        }

        public override void OnEnter() {
            character.animator.SetBool("Grounded", false);
            character.animator.SetFloat("MoveSpeed", 0);
        }
    }
}