using UnityEngine;

public enum InputType {
	Move, 
	JumpStarted, JumpEnded,  
	Fall,
	Grounded, LeaveGround
}

[CreateAssetMenu(menuName = "Behaviour")]
public class BehaviourMachine : ScriptableObject {

	public bool enabled { get; private set; }
	
	private BehaviourState _initialState;
	private BehaviourState _currentState;
	private BehaviourState CurrentState {
		get => _currentState;
		set {
			if(value == null) return;
			_currentState?.OnExit(_actor, _brain);
			_currentState = value;
			_currentState.OnEnter(_actor, _brain);
		}
	}
	
	private Actor _actor;
	private Brain _brain;
	private ActorAnimation _animation;

	public void Enable(Actor actor, Brain brain, Animator animator) {
		_actor = actor;
		_brain = brain;
		_animation = new ActorAnimation(animator);

		_initialState = new MovementState();
		CurrentState = _initialState;
		
		enabled = true;
	}

	public void Disable() {
		enabled = false;
	}

	public void UpdateState() {
		if(!enabled) return;
		CurrentState.OnUpdate(_actor, _brain);
		_animation.Update(_actor, _brain);
	}
	
	public void HandleInput(InputType input) {
		if (!enabled) return;
		_animation.HandleInput(input, _brain);
		CurrentState = CurrentState.OnHandleInput(_actor, _brain, input);
	}
}