using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor), 
	typeof(SpriteRenderer), 
	typeof(Animator))]
public class Brain : MonoBehaviour {

	[SerializeField] private Behaviour behaviour;
	
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
	
	private Rigidbody2D _rigidbody2D;
	private Actor _actor;
	private SpriteRenderer _sprite;
	private PlayerActions _input;

	public float moveAxis { get; private set; }

	private Dictionary<InputType, System.Action<InputAction.CallbackContext>> _handleInput;

	private void OnEnable() {
		_actor = GetComponent<Actor>();
		_sprite = GetComponent<SpriteRenderer>();
		var animator = GetComponent<Animator>();
		
		_input = new PlayerActions();
		_input.Enable();
		InitInputs();
		
		behaviour.Enable(_actor, this, animator);
	}
	private void OnDisable() {
		behaviour.Disable();
		
		DestroyInputs();
		_input.Disable();
	}
	private void Update() {
		moveAxis = _input.Gameplay.Move.ReadValue<float>();
		_sprite.flipX = moveAxis == 0 ? _sprite.flipX : moveAxis < 0f;
		
		CheckGround();
		CheckFalling();
		
		behaviour.UpdateState();
	}
	private void CheckFalling() {
		var wasFalling = _isFalling;
		_isFalling = _actor.VelocityY < 0f && !Grounded;
		if(!wasFalling && _isFalling)
			behaviour.HandleInput(InputType.Fall);
	}

	#region Ground Checking
	private void CheckGround() {
		//Reach Ground from falling
		if (_actor.TouchingGround) {
			if(!Grounded) FlushBuffer();
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
		_handleInput = new Dictionary<InputType, System.Action<InputAction.CallbackContext>>();
		foreach (InputType type in System.Enum.GetValues(typeof(InputType))) {
			_handleInput.Add(type, _ => behaviour.HandleInput(type));
		}

		_input.Gameplay.Move.performed += _handleInput[InputType.Move];
		_input.Gameplay.Jump.started += _handleInput[InputType.JumpStarted];
		_input.Gameplay.Jump.performed += _handleInput[InputType.JumpEnded];
	}
	private void DestroyInputs() {
		_input.Gameplay.Move.performed -= _handleInput[InputType.Move];
		_input.Gameplay.Jump.started -= _handleInput[InputType.JumpStarted];
		_input.Gameplay.Jump.performed -= _handleInput[InputType.JumpEnded];

		_handleInput = null;
	}

	private struct BufferedInput {
		public InputType type;
		public float bufferTime;
	}
	private Queue<BufferedInput> _buffer;
	public void BufferUntilGrounded(InputType input) {
		_buffer ??= new Queue<BufferedInput>();
		
		_buffer.Enqueue(new BufferedInput {type = input, bufferTime = Time.time });
	}

	private void FlushBuffer() {
		if(_buffer == null) return;
		
		while (_buffer.Count > 0) {
			var input = _buffer.Dequeue();
			var elapsed = Time.time - input.bufferTime;
			if(elapsed <= bufferExpirationTime)
				behaviour.HandleInput(input.type, false);
		}
	}
	

	#endregion
	
}
