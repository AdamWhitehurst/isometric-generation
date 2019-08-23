using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace State {
    public class StateMachine : MonoBehaviour {
        private Dictionary<Type, BaseState> availableStates;
        [SerializeField]
        public BaseState currentState;

        [SerializeField] private string StateName;



        // public event Action<BaseState> OnStateChanged;

        public void SetStates(Dictionary<Type, BaseState> states) {
            availableStates = states;
            currentState = availableStates.Values.First();
        }

        private void FixedUpdate() {
            Type nextState = currentState.Tick();

            if (nextState != null && nextState != currentState.GetType()) {
                SwitchToNextState(nextState);
            }
        }

        private void SwitchToNextState(Type nextState) {
            currentState = availableStates[nextState];
            StateName = currentState.ToString();
            // OnStateChanged?.Invoke(CurrentState);
            currentState.OnEnter();
        }
    }
}