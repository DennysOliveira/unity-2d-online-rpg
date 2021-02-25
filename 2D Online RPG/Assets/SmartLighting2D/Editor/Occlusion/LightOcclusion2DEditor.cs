using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightOcclusion2D))]
public class LightOcclusion2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightOcclusion2D script = target as LightOcclusion2D;

        script.shape.shadowType = (LightOcclusion2D.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.shape.shadowType);

		script.occlusionType = (LightOcclusion2D.OcclusionType)EditorGUILayout.EnumPopup("Occlusion Type", script.occlusionType);

		script.occlusionSize = EditorGUILayout.FloatField("Size", script.occlusionSize);
		
        if (GUILayout.Button("Update")) {
			script.Initialize();
		}

		if (GUI.changed) {
			script.Initialize();

			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
    }
}
