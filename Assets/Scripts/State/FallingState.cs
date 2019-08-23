using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class FallingState : BaseState {
        private ControlledBody controller;


        public FallingState(ControlledBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {
            if (controller.IsGrounded()) {
                return typeof(IdleState);
            }
            controller.HandleMoveForward();
            return null;
        }

        public override void OnEnter() {
            controller.anim.SetBool("Grounded", false);
            controller.anim.SetFloat("MoveSpeed", 0);
        }
    }
}