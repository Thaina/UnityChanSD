using UnityEngine;
using UnityEditor;

using System.Linq;
using System.Collections.Generic;

using UnityChan;

[CustomEditor(typeof(CameraController))]
public class TransformInspector : Editor
{
	static AnimationCurve curve;
	public override void OnInspectorGUI()
	{
		if(AnimationUtility.onCurveWasModified == null)
		{
			AnimationUtility.onCurveWasModified	= (clip,binding,type) => {
				if(type == AnimationUtility.CurveModifiedType.CurveDeleted)
					return;

				curve	= AnimationUtility.GetEditorCurve(clip,binding);
			};
		}

		var controller = target as CameraController;

		DrawDefaultInspector();

		GUILayout.Label(curve == null ? "null" : curve.keys.Length.ToString());
		if(curve != null)
		{
			GUILayout.BeginVertical();
			foreach(var key in curve.keys)
				GUILayout.Label(key.time + " : " + key.value + " : " + key.inTangent + " : " + key.outTangent + " : " + key.tangentMode);
			GUILayout.EndVertical();
		}
	}

	private Vector3 FixIfNaN(Vector3 v)
	{
		if(float.IsNaN(v.x))
			v.x = 0;
		if(float.IsNaN(v.y))
			v.y = 0;
		if(float.IsNaN(v.z))
			v.z = 0;

		return v;
	}
}
