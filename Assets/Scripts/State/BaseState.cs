using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State {
    [Serializable]
    public abstract class BaseState {
        protected GameObject gameObject;
        protected Transform transform;
        public BaseState(GameObject gameObject) {
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
        }

        public abstract Type Tick();

        public abstract void OnEnter();
    }
}