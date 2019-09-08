using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class ActionState : BaseState {
        private Character character;

        public ActionState(Character character) : base(character.gameObject) {
            this.character = character;
        }

        public override Type Tick() {

            return typeof(IdleState);
        }

        public override void OnEnter() {
            // character.anim.SetTrigger("Wave");
            character.HandleAction();
        }
    }
}