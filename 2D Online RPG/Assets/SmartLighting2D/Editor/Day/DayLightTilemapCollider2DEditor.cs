using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using LightTilemapCollider;

#if UNITY_2017_4_OR_NEWER

[CustomEditor(typeof(DayLightTilemapCollider2D))]
public class DayLightTilemapCollider2DEditor : Editor {
    

    override public void OnInspectorGUI() {
        DayLightTilemapCollider2D script = target as DayLightTilemapCollider2D;

		script.tilemapType = (MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

        EditorGUILayout.Space();

        switch(script.tilemapType) {
			case MapType.UnityRectangle:
				script.rectangle.shadowType = (LightTilemapCollider.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.rectangle.shadowType);
				
				EditorGUI.BeginDisabledGroup(script.rectangle.shadowType == LightTilemapCollider.ShadowType.None);

				script.shadowLayer = EditorGUILayout.Popup("Shadow Layer (Day)", script.shadowLayer, Lighting2D.Profile.layers.lightLayers.GetNames());
				
				switch(script.rectangle.shadowType) {
					case LightTilemapCollider.ShadowType.Grid:
					case LightTilemapCollider.ShadowType.SpritePhysicsShape:
						script.shadowTileType = (ShadowTileType)EditorGUILayout.EnumPopup("Shadow Tile Type", script.shadowTileType);
					break;
				}

				EditorGUI.EndDisabledGroup();

				EditorGUILayout.Space();

				script.rectangle.maskType = (LightTilemapCollider.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.rectangle.maskType);
				
				EditorGUI.BeginDisabledGroup(script.rectangle.maskType == LightTilemapCollider.MaskType.None);

				//if (script.rectangle.maskType == LightTilemapCollider.Rectangle.MaskType.BumpedSprite) {
				//	GUIBumpMapMode.Draw(script.bumpMapMode);
				//}

				EditorGUI.EndDisabledGroup();

			break;

			case MapType.UnityIsometric:
			break;


			case MapType.UnityHexagon:
			break;

			case MapType.SuperTilemapEditor:
				script.superTilemapEditor.shadowTypeSTE = (SuperTilemapEditorSupport.TilemapCollider2D.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.superTilemapEditor.shadowTypeSTE);
			
				script.shadowLayer = EditorGUILayout.Popup("Shadow Layer (Day)", script.shadowLayer, Lighting2D.Profile.layers.lightLayers.GetNames());
				
				EditorGUILayout.Space();

				script.superTilemapEditor.maskTypeSTE = (SuperTilemapEditorSupport.TilemapCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.superTilemapEditor.maskTypeSTE);
				
				EditorGUI.BeginDisabledGroup(script.superTilemapEditor.maskTypeSTE == SuperTilemapEditorSupport.TilemapCollider2D.MaskType.None);
				
				script.maskLayer = EditorGUILayout.Popup("Mask Layer (Day)", script.maskLayer, Lighting2D.Profile.layers.lightLayers.GetNames());
				
				if (script.superTilemapEditor.maskTypeSTE == SuperTilemapEditorSupport.TilemapCollider2D.MaskType.BumpedSprite) {
					GUIBumpMapMode.Draw(serializedObject, script);
				}

				
				EditorGUI.EndDisabledGroup();
			break;
		}

		EditorGUILayout.Space();

        UpdateCollisions(script);

		if (GUI.changed) {
			script.Initialize();
			
            if (EditorApplication.isPlaying == false) {
				
				
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
    }

    static void UpdateCollisions(DayLightTilemapCollider2D script) {
		if (GUILayout.Button("Update")) {
			// PhysicsShapeManager.Clear();
			
			script.Initialize();

			//LightingSource2D.ForceUpdateAll();
			LightingManager2D.ForceUpdate();
		}
	}
}

#endif