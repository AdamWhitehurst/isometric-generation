using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class JumpingState : BaseState {
        private KinematicBody controller;

        public JumpingState(KinematicBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {
            return typeof(FallingState);
        }

        public override void OnEnter() {
            controller.anim.SetFloat("MoveSpeed", 1f);
            controller.anim.SetBool("Grounded", false);
            controller.anim.SetTrigger("Jump");
        }
    }
}