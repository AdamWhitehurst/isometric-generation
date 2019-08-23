using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class FallingState : BaseState {
        private KinematicBody controller;


        public FallingState(KinematicBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {
            var grounded = controller.IsGrounded();
            if (!grounded) {
                controller.HandleMoveForward();
                return null;
            }
            return typeof(IdleState);
        }

        public override void OnEnter() {
            controller.anim.SetBool("Grounded", false);
            controller.anim.SetFloat("MoveSpeed", 0);
        }
    }
}