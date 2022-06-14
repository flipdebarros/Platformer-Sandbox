using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Curve Editor", typeof(JumpCurve))]
public class CurveEditorTool : EditorTool {

	private const float dotSize = 0.05f;
	private const float sliderSize = 0.03f;
	private const float sliderDist = 0.3f;
	private const int n = 35;
	private const float snap = 0.5f;
	
	public override void OnToolGUI(EditorWindow window) {
		if (window is not SceneView sceneView)
			return;

		foreach (var obj in targets) {
			if(obj is not JumpCurve jump) continue;
			
			EditorGUI.BeginChangeCheck();
			var pos = Handles.DoPositionHandle(jump.StartPoint, Quaternion.identity);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(jump.transform, "Change Position");
				jump.UpdatePosition(pos);
			}
			
			EditorGUI.BeginChangeCheck();
			Handles.color = Color.green;
			Vector3 apex = jump.PeakPoint;
			apex = Handles.FreeMoveHandle(apex , Quaternion.identity,
				HandleUtility.GetHandleSize(Vector3.zero) * dotSize, Vector3.zero, Handles.DotHandleCap);
			var bias = Handles.FreeMoveHandle(apex + Vector3.right * sliderDist , Quaternion.identity,
				HandleUtility.GetHandleSize(Vector3.zero) * sliderSize, Vector3.right * 0.1f * jump.Reach, Handles.DotHandleCap).x - sliderDist;
			var height = Handles.FreeMoveHandle(apex + Vector3.up * sliderDist , Quaternion.identity,
				HandleUtility.GetHandleSize(Vector3.zero) * sliderSize, Vector3.up * snap, Handles.DotHandleCap).y - sliderDist;
			Handles.DrawAAPolyLine(apex, apex + Vector3.right * sliderDist);
			Handles.DrawAAPolyLine(apex, apex + Vector3.up * sliderDist);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(target, "Modify Jump Curve");
				jump.UpdateApexPoint(bias, height);
			}
			Handles.Label(apex + new Vector3(0.4f, .2f), jump.Bias.ToString("0.00"));
			Handles.Label(apex + new Vector3(0.1f, .4f) , jump.Height.ToString("0.00"));
			
			Vector3 ground = jump.ReachPoint;
			EditorGUI.BeginChangeCheck();
			ground.x = Handles.FreeMoveHandle(ground, Quaternion.identity,
				HandleUtility.GetHandleSize(Vector3.zero) * dotSize, Vector3.right * snap, Handles.DotHandleCap).x;
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(target, "Modify Jump Curve");
				jump.UpdateGroundPoint(ground);
			}
			Handles.Label(jump.ReachPoint + new Vector2(0.2f, .2f), jump.Reach.ToString("0.00"));
			
			jump.ValidateCurve();
			DrawCurve(jump.riseCurve);
			DrawCurve(jump.fallCurve);
			Handles.color = Color.white;
		}

		base.OnToolGUI(window);
	}

	private void DrawCurve(Curve curve) {
		var points = new Vector3[n + 1];
		for (var i = 0; i < n + 1; i++) {
			var t = i * 1f / n;
			points[i] = curve[t];
		}

		Handles.DrawAAPolyLine(points);
	}

}
