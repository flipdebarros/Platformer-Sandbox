using System;
using UnityEngine;

[ExecuteAlways]
public class JumpCurve : MonoBehaviour {

	[SerializeField] private Vector2 offset;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float jumpReach;
	[SerializeField] [Range(0f, 1f)] private float apexBias;
	[SerializeField] private float airSpeed;
	
	private Vector2 _apexPoint;
	private Vector2 _groundPoint;
	private Vector2 _controlPoint1;
	private Vector2 _controlPoint2;
	public Curve riseCurve {get; private set;}
	public Curve fallCurve {get; private set;}
	
	public float AirSpeed => airSpeed;
	public float Bias => apexBias;
	public float Height => jumpHeight;
	public float Reach => jumpReach;
	public Vector2 StartPoint => (Vector2) transform.position + offset;
	public Vector2 PeakPoint => _apexPoint + StartPoint;
	public Vector2 ReachPoint => _groundPoint + StartPoint;
	public float FlightTime => AirSpeed == 0f ? 0f : jumpReach / AirSpeed;
	public float RiseTime => FlightTime * apexBias;
	public float FallTime => FlightTime * (1f - apexBias);

	public float InitYVelocity => riseCurve.EvaluateDenormalizedDerivative(0f, RiseTime).y;
	public float gRise => riseCurve.GetDenormalizedSecondDerivative(RiseTime).y;
	public float gFall  => fallCurve.GetDenormalizedSecondDerivative(FallTime).y;

	private void OnEnable() {
		ValidateCurve();
	}

	public void OnValidate() {
		ValidateCurve();
	}

	public void ValidateCurve() {
		if(airSpeed == 0f) Debug.LogError("Jump Curve: Air Speed should not be 0.");
		
		jumpHeight = Mathf.Clamp(jumpHeight,0.0000001f, Mathf.Infinity);
		jumpReach = Mathf.Clamp(jumpReach, 0.0000001f, Mathf.Infinity);
		apexBias = Mathf.Clamp(apexBias, 0.0000001f, 0.9999999f);
		
		_apexPoint = new Vector2(jumpReach * apexBias, jumpHeight);
		_groundPoint = Vector3.right * jumpReach;

		_controlPoint1 = _apexPoint - new Vector2(_apexPoint.x / 2f, 0);
		_controlPoint2 = _apexPoint +  new Vector2((_groundPoint.x - _apexPoint.x) / 2f, 0);
		
		riseCurve = new Curve(StartPoint, _apexPoint + StartPoint, _controlPoint1 + StartPoint);
		fallCurve = new Curve(_apexPoint + StartPoint, _groundPoint + StartPoint, _controlPoint2 + StartPoint);
	}

	public void UpdateApexPoint(float bias, float height) {
		apexBias = (bias - StartPoint.x) / jumpReach;
		jumpHeight = height - StartPoint.y;
	}
	
	public void UpdateGroundPoint(Vector2 ground) {
		_groundPoint = ground - StartPoint;
		jumpReach = _groundPoint.x;
	}

	public void UpdatePosition(Vector2 pos) {
		transform.position = pos - offset;
	}
}
