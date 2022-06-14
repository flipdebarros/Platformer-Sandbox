using UnityEngine;

[System.Serializable]
public class Curve {
	public Vector2 startPoint  = Vector2.zero;
	public Vector2 endPoint = Vector2.zero;
	public Vector2 controlPoint = Vector2.zero;

	public Vector2 this[float t] => Evaluate(t);

	public Curve() { }
	public Curve(Vector2 start, Vector2 end, Vector2 control) {
		startPoint = start;
		endPoint = end;
		controlPoint = control;
	}
	
	public Vector2 Evaluate(float t) {
		var u = 1f - t;
		var a = u * u;
		var b = 2f * u * t;
		var c = t * t;
		return a * startPoint + b * controlPoint + c * endPoint;
	}
	
	public Vector2 EvaluateDerivative(float t) {
		var p = controlPoint - startPoint;
		var q = endPoint - controlPoint;
		var u = 1f - t;
		return 2 * (u * p + t * q);
	}

	public Vector2 EvaluateDenormalizedDerivative(float t, float T) => EvaluateDerivative(t) / T; 
	public Vector2 GetDenormalizedSecondDerivative(float T) => 2f * (startPoint - 2f * controlPoint + endPoint) / (T * T);
}