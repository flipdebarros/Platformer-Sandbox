using UnityEngine;

public class ActorAnimation {

	private readonly Animator _animator;

	private static readonly int Grounded = Animator.StringToHash("Grounded");
	private static readonly int XSpeed = Animator.StringToHash("xSpeed");
	private static readonly int YSpeed = Animator.StringToHash("ySpeed");
	private static readonly int Jump = Animator.StringToHash("Jump");

	public ActorAnimation(Animator animator) {
		_animator = animator;
	}

	public void Update(Actor actor, Brain brain) {
		_animator.SetFloat(XSpeed, DeadZone(brain.moveAxis));
		_animator.SetFloat(YSpeed, DeadZone(actor.VelocityY));
		_animator.SetBool(Grounded, brain.Grounded);
	}

	private const float deadZone = 0.1f;
	private static float DeadZone(float value) => Mathf.Abs(value) < deadZone ? 0f : 1f;
	private static float DeadZoneSign(float value) => Mathf.Abs(value) < deadZone ? 0f : value > 0f ? 1f : -1f;

	public void HandleInput(InputType input, Brain brain) {
		switch (input) {
			case InputType.JumpStarted when brain.Grounded: 
			case InputType.Fall: break;
			case InputType.Move: break;
			case InputType.Grounded: break;
			case InputType.LeaveGround: _animator.SetTrigger(Jump); break;
			case InputType.JumpEnded: break;
			default: break;
		}
	}

}
