public class FallState : BehaviourState {

	public override void OnEnter(Actor actor, Brain brain) {
		var g = actor.Jump.gFall;
		actor.Gravity = g;
	}

	public override void OnUpdate(Actor actor, Brain brain) {
		var speed = actor.Jump.AirSpeed * brain.moveAxis;
		actor.VelocityX = speed;
	}
	
	public override BehaviourState OnHandleInput(Actor actor, Brain brain, InputType input) => input switch {
		InputType.Grounded => new MovementState(),
		_ => null
	};
}
