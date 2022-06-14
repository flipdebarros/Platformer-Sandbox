using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[CustomEditor(typeof(JumpCurve))]
public class JumpCurveCustomInspector : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(GUILayout.Button("Edit Curve"))
			ActivateCurveEditor();
	}

	private static void ActivateCurveEditor() {
		ToolManager.SetActiveTool<CurveEditorTool>();
	}

}
