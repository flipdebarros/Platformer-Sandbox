using UnityEngine;

[RequireComponent(typeof(Brain),
	typeof(Rigidbody2D),
	typeof(JumpCurve))]
public class Actor : MonoBehaviour {
	[SerializeField] private float groundSpeed;
	[SerializeField] private float terminalVelocity;
	
	private JumpCurve _jump;
	
	public float GroundSpeed => groundSpeed;
	public JumpCurve Jump => _jump ??= GetComponent<JumpCurve>();
	public float MinJumpHeight => _jump.Height;
	public bool FacingLeft { get; set; }

	private Rigidbody2D _rigidbody2D;

	[Header("Ground Detection")]
	[SerializeField] 
	private Rect groundContact;
	[SerializeField]
	private LayerMask groundLayer;
	public bool TouchingGround { get; private set; }
	
	[Header("Wall Detection")]
	[SerializeField] 
	private Rect wallContact;
	[SerializeField]
	private LayerMask wallLayer;
	public bool AgainstWall { get; private set; }

	private void OnEnable() {
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		CheckWall();
		CheckGround();
		ClampVelocity();
	}

	private void ClampVelocity() {
		VelocityY = Mathf.Clamp(_rigidbody2D.velocity.y, -terminalVelocity, terminalVelocity);
	}

	private void CheckGround() { 
		var box = Centralize(groundContact);
		var hit = Physics2D.BoxCast(box.position, box.size, 0f, Vector2.down, 1f/32f, groundLayer);
		
		var goingUp = VelocityY > 0f;
		var hitFromAbove = hit.normal.y > 0f;
		
		TouchingGround = hit && hitFromAbove && !goingUp;
	}

	private void CheckWall() {
		var box = Centralize(wallContact);
		var hit = Physics2D.BoxCast(box.position, box.size, 0f, Vector2.zero, Mathf.Infinity, wallLayer);
		AgainstWall = hit;
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

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.blue;
		
		var box = Centralize(groundContact);
		Gizmos.DrawWireCube(box.position, box.size);

		Gizmos.color = Color.red;
		
		box = Centralize(wallContact);
		Gizmos.DrawWireCube(box.position, box.size);

		Gizmos.color = Color.white;
	}

	private Rect Centralize(Rect rect) {
		var box = rect;
		box.position = Vector2.Scale(box.position, new Vector2(FacingLeft ? -1f : 1f, 1f)) + (Vector2) transform.position;
		return box;
	}
}
