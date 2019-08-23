using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FauxGravity;

namespace State {
    [Serializable]
    public class ActionState : BaseState {
        private KinematicBody controller;

        public ActionState(KinematicBody controller) : base(controller.gameObject) {
            this.controller = controller;
        }

        public override Type Tick() {

            return typeof(IdleState);
        }

        public override void OnEnter() {
            controller.anim.SetTrigger("Wave");
            controller.HandleAction();
        }
    }
}