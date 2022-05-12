using UnityEngine;

public class JumpState : BehaviourState {

	private float _timeToPeak;
	private float _initYVelocity;
	private float _g;
	private float _startJumpTime;
	
	public override void OnEnter(Actor actor, Brain brain) {
		_timeToPeak = actor.JumpReach * actor.JumpPeak / actor.GroundSpeed;
		var timeOfFlight = _timeToPeak * 2f;
		_g = -8f * actor.JumpHeight / (timeOfFlight * timeOfFlight);
		_initYVelocity = 4f * actor.JumpHeight / timeOfFlight;
		
		actor.VelocityY = _initYVelocity;
		actor.Gravity = _g;
		_startJumpTime = Time.time;

		brain.LeaveGround();
	}

	public override void OnUpdate(Actor actor, Brain brain) {
		var speed = actor.AirSpeed * brain.moveAxis;
		actor.VelocityX = speed;
	}

	private void TerminateJump(Actor actor) {
		var terminationHeight = actor.JumpHeight - actor.MinJumpHeight;
		var terminalVelocity = _initYVelocity * _initYVelocity;
		terminalVelocity += 2 * _g * terminationHeight;
		terminalVelocity = Mathf.Sqrt(terminalVelocity);
		
		var terminationTime = _timeToPeak - 2f * terminationHeight / (_initYVelocity + terminalVelocity);

		var elapsed = Time.time - _startJumpTime;
		if(elapsed > terminationTime || actor.VelocityY <= terminalVelocity) return;

		actor.VelocityY = terminalVelocity;
	}
	
	public override BehaviourState OnHandleInput(Actor actor, Brain brain, InputType input) {
		if (input is not InputType.JumpEnded)
			return input switch {
				InputType.Grounded when brain.Grounded => new MovementState(),
				InputType.Fall => new FallState(),
				_ => null
			};
		
		TerminateJump(actor);
		return null;

	}
}
