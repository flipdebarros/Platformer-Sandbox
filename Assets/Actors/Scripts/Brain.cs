using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor), 
	typeof(SpriteRenderer), 
	typeof(Animator))]
public class Brain : MonoBehaviour {

	[SerializeField] private BehaviourMachine behaviour;
	
	[Header("Ground Detection")]
	[SerializeField] private float coyoteTime;
	[SerializeField] private float bufferExpirationTime = 0.1f;

	private bool _inCoyoteTime;
	private bool _isFalling;
	private bool _grounded;
	public bool Grounded {
		get => _grounded;
		private set{
			if (_grounded != value)
				behaviour.HandleInput(value ? InputType.Grounded : InputType.LeaveGround);
			_grounded = value;
		}
	}

	private Actor _actor;
	private PlayerActions _input;

	public float moveAxis { get; private set; }

	private void OnEnable() {
		_actor = GetComponent<Actor>();
		var sprite = GetComponent<SpriteRenderer>();
		var animator = GetComponent<Animator>();
		
		_input = new PlayerActions();
		_input.Enable();
		InitInputs();
		
		behaviour.Enable(_actor, this, animator, sprite);
	}
	private void OnDisable() {
		behaviour.Disable();
		
		DestroyInputs();
		_input.Disable();
	}
	private void Update() {
		moveAxis = _input.Gameplay.Move.ReadValue<float>();
		_actor.FacingLeft = moveAxis == 0f ? _actor.FacingLeft : moveAxis < 0f;
		
		CheckGround();
		CheckWall();
		CheckFalling();

		behaviour.UpdateState();
	}
	private void CheckFalling() {
		var wasFalling = _isFalling;
		_isFalling = _actor.VelocityY < 0f && !Grounded;
		if(!wasFalling && _isFalling)
			behaviour.HandleInput(InputType.Fall);
	}
	
	private void CheckWall() {
		if(_actor.AgainstWall && Grounded) 
			moveAxis = Mathf.Clamp(moveAxis, _actor.FacingLeft ? 0f : -1f, _actor.FacingLeft ? 1f : 0f);
	}

	#region Ground Checking
	private void CheckGround() {
		//Reach Ground from falling
		if (_actor.TouchingGround) {
			Grounded = true;
			_inCoyoteTime = false;
			return;
		}
		
		if(Grounded && !_inCoyoteTime)
			StartCoroutine(CoyoteTime());
	}
	public void LeaveGround() {
		_inCoyoteTime = false;
		Grounded = false;
	}
	private IEnumerator CoyoteTime() {
		_inCoyoteTime = true;
		yield return new WaitForSeconds(coyoteTime);
		if(!_inCoyoteTime) yield break;
		Grounded = false;
		_inCoyoteTime = false;
	}
	#endregion

	#region Input
	private void InitInputs() {
		_input.Gameplay.Move.performed += OnMove;
		_input.Gameplay.Jump.performed += OnJump;
	}
	private void DestroyInputs() {
		_input.Gameplay.Move.performed -= OnMove;
		_input.Gameplay.Jump.performed -= OnJump;
	}
	
	private void OnMove(InputAction.CallbackContext context) => behaviour.HandleInput(InputType.Move);
	private void OnJump(InputAction.CallbackContext context) {
		var pressed = context.ReadValueAsButton();

		if (!Grounded && pressed) StartCoroutine(Buffer(InputType.JumpStarted, Time.time));
		else behaviour.HandleInput(pressed ? InputType.JumpStarted : InputType.JumpEnded);
	}

	private IEnumerator Buffer(InputType input, float time) {
		while (!Grounded) yield return null;
		if(Time.time - time >= bufferExpirationTime) yield break; 
		behaviour.HandleInput(input);
	}

	#endregion
	
}
