public class MovementState : BehaviourState {
	
	public override void OnUpdate(Actor actor, Brain brain) {
		var speed = actor.GroundSpeed;
		actor.VelocityX = speed * brain.moveAxis;
	}
	
	public override BehaviourState OnHandleInput(Actor actor, Brain brain, InputType input) => input switch {
		InputType.JumpStarted when brain.Grounded => new JumpState(),
		InputType.Fall => new FallState(),
		_ => null
	};
}
