using System;
using UnityEngine;

[RequireComponent(typeof(Brain))]
public class Actor : MonoBehaviour {
	[SerializeField] private float groundSpeed;
	[SerializeField] private float airSpeed;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float minJumpHeight;
	[SerializeField] private float jumpReach;
	[SerializeField] [Range(0f, 1f)] private float jumpPeak = 0.5f;
	[SerializeField] private float terminalVelocity;
	
	public float GroundSpeed => groundSpeed;
	public float AirSpeed => airSpeed;
	public float MinJumpHeight => minJumpHeight;
	public float JumpHeight => jumpHeight;
	public float JumpReach => jumpReach;
	public float JumpPeak => jumpPeak;

	private Rigidbody2D _rigidbody2D;

	[Header("Ground Detection")]
	[SerializeField] 
	private Rect groundContact;
	[SerializeField]
	private LayerMask groundLayer;
	public bool TouchingGround { get; private set; }

	private void OnEnable() {
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		CheckGround();
		ClampVelocity();
	}

	private void ClampVelocity() {
		VelocityY = Mathf.Clamp(_rigidbody2D.velocity.y, -terminalVelocity, terminalVelocity);
	}

	private void CheckGround() { 
		var box = groundContact;
		box.position += (Vector2) transform.position - groundContact.size / 2f;
		var hit = Physics2D.BoxCast(box.position, box.size, 0f, Vector2.zero, Mathf.Infinity, groundLayer);

		var goingUp = VelocityY > 0f;
		var hitFromAbove = hit.normal.y > 0f;

		TouchingGround = hit && hitFromAbove && !goingUp;
	}
	
	#region Rigidbody Acessors

	private const float deadZone = 0.1f;
	private void SetVelocity(float x, float y) => _rigidbody2D.velocity = new Vector2(x, y);
	public float VelocityX {
		get => Mathf.Abs(_rigidbody2D.velocity.x) < deadZone ? 0f : _rigidbody2D.velocity.x;
		set => SetVelocity(value, _rigidbody2D.velocity.y);
	}
	public float VelocityY {
		get => Mathf.Abs(_rigidbody2D.velocity.y) < deadZone ? 0f : _rigidbody2D.velocity.y;
		set => SetVelocity(_rigidbody2D.velocity.x, value);
	}

	public float Gravity {
		get => _rigidbody2D.gravityScale * Physics2D.gravity.y;
		set => _rigidbody2D.gravityScale = value / Physics2D.gravity.y;
	}

	#endregion
}
