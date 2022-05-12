public class FallState : BehaviourState {

	public override void OnEnter(Actor actor, Brain brain) {
		var timeOfFlight = actor.JumpReach * (1f - actor.JumpPeak) * 2f / actor.AirSpeed;
		var g = -8f * actor.JumpHeight / (timeOfFlight * timeOfFlight);
		actor.Gravity = g;
	}

	public override void OnUpdate(Actor actor, Brain brain) {
		var speed = actor.AirSpeed * brain.moveAxis;
		actor.VelocityX = speed;
	}
	
	public override BehaviourState OnHandleInput(Actor actor, Brain brain, InputType input) => input switch {
		InputType.Grounded => new MovementState(),
		_ => null
	};
}
