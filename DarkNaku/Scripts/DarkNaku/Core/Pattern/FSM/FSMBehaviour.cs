using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

namespace DarkNaku {
	public abstract class FSMState<S, M> 
			where S : struct, System.IConvertible, System.IComparable
			where M : FSMBehaviour<S, M> {
		protected M Machine { get; private set; }

		public object Param { get; protected set; }

		public abstract S State { get; }

		public virtual void OnInitialize() { }
		public virtual void OnEnter(object param) { }
		public virtual void OnLeave() { }
		public virtual S NextState() { return State; }
		public virtual void Update() { }

		public void Initialize(M machine) { 
			Assert.IsNotNull(machine);
			Machine = machine;
			OnInitialize();
		}

		public void ClearParam() {
			Param = null;
		}
	}

	public class FSMBehaviour<S, M> : MonoBehaviour 
			where S : struct, System.IConvertible, System.IComparable 
			where M : FSMBehaviour<S, M> {
		public class StateChangeEvent : UnityEvent<S, S> { }

		private Dictionary<S, FSMState<S, M>> _states = new Dictionary<S, FSMState<S, M>>();

		public StateChangeEvent _onTransition = null;
		public StateChangeEvent OnTransition {
			get {
				if (_onTransition == null) _onTransition = new StateChangeEvent();
				return _onTransition;
			}
		}

		private S _state;
		public S State { 
			get { return _state; }
			protected set {
				if (value.CompareTo(_state) == 0) return;
				S prevState = _state;
				_states[_state].OnLeave();
				_state = value;
				_states[_state].OnEnter(_states[prevState].Param);
				OnTransition.Invoke(prevState, _state);
				_states[prevState].ClearParam();
			}
		}

		private bool _isFirstExecute = true;

		protected void AddStates(params FSMState<S, M>[] states) {
			Assert.IsNotNull(states, "[FSM] AddStates : Parameter can be not null.");

			for (int i = 0; i < states.Length; i++) {
				AddState(states[i]);
			}
		}

		private void AddState(FSMState<S, M> state) {
			Assert.IsFalse(_states.ContainsKey(state.State), 
					string.Format("[FSM] AddState : {0} has already been added.", state.State));
			state.Initialize(this as M);
			if (_states.Count == 0) _state = state.State;
			_states.Add(state.State, state);
		}

		protected void RemoveState(S state) {
			Assert.IsTrue(_states.ContainsKey(state), 
					string.Format("[FSM] RemoveState : {0} is not on the list of states.", state));
			_states.Remove(state);
		}

		protected void Execute() {
			if (_isFirstExecute) {
				_states[State].OnEnter(null);
				_isFirstExecute = false;
			}

			State = _states[State].NextState();
			_states[State].Update();
		}
	}
}
