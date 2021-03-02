using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightTilemapOcclusion2D))]
public class LightingTilemap2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightTilemapOcclusion2D script = target as LightTilemapOcclusion2D;

        script.tilemapType = (LightTilemapOcclusion2D.MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

        script.onlyColliders = EditorGUILayout.Toggle("Only Colliders", script.onlyColliders);

		GUISortingLayer.Draw(script.sortingLayer);
        
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
