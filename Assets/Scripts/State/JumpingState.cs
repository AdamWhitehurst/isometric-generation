using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class JumpingState : BaseState {
        private Character character;

        public JumpingState(Character character) : base(character.gameObject) {
            this.character = character;
        }

        public override Type Tick() {
            return typeof(FallingState);
        }

        public override void OnEnter() {
            character.animator.SetFloat("MoveSpeed", 1f);
            character.animator.SetBool("Grounded", false);
            character.animator.SetTrigger("Jump");
        }
    }
}