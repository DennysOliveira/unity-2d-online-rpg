using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;
using LightSettings;

public class ProfileEditor {
	
	public static void Draw() {
		EditorGUI.BeginChangeCheck ();

		LightingSettings.Profile profile = Lighting2D.Profile;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.ObjectField("Current Profile", profile, typeof(LightingSettings.Profile), true);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		if (profile == null) {
			EditorGUILayout.HelpBox("Lighting2D Settings Profile Not Found!", MessageType.Error);

			return;
		}

		// Common Settings

		CommonSettings(profile.bufferPresets.list[0]);

		EditorGUILayout.Space();

		// Sorting Layer

		SortingLayer(profile.bufferPresets.list[0].sortingLayer);

		EditorGUILayout.Space();

		// Quality Settings

		QualitySettings.Draw(profile);

		EditorGUILayout.Space();

		// Layers

		Layers.Draw(profile);
		
		EditorGUILayout.Space();

		// Day Lighting

		DayLighting.Draw(profile);
		
		EditorGUILayout.Space();
		
		// Fog of War

		FogOfWar.Draw(profile);

		EditorGUILayout.Space();

		// Buffer Presets

		BufferPresets.Draw(profile.bufferPresets);

		EditorGUILayout.Space();

		// Light Presets

		LightPresets.Draw(profile.lightPresets);

		EditorGUILayout.Space();

		// Event Presets

		EventPresets.Draw(profile.eventPresets);

		EditorGUILayout.Space();

		// Disable

		profile.disable = EditorGUILayout.Toggle("Disable", profile.disable);

		EditorGUI.EndChangeCheck ();

		if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {
				Light2D.ForceUpdateAll();
				
				LightingManager2D.ForceUpdate();

				foreach(OnRenderMode onRender in OnRenderMode.List) {
					BufferPreset bufferPreset = onRender.mainBuffer.GetBufferPreset();
					bufferPreset.sortingLayer.ApplyToMeshRenderer(onRender.meshRenderer);
				}

				EditorUtility.SetDirty(profile);
			}
		}
	}

	public class LightPresets {
		public static void Draw(LightPresetList lightPresetList) {
			bool foldout = GUIFoldoutHeader.Begin( "Light Presets (" + lightPresetList.list.Length + ")", lightPresetList);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

			int bufferCount = EditorGUILayout.IntSlider ("Count", lightPresetList.list.Length, 1, 4);

			if (bufferCount !=lightPresetList.list.Length) {
				int oldCount = lightPresetList.list.Length;

				System.Array.Resize(ref lightPresetList.list, bufferCount);

				for(int i = oldCount; i < bufferCount; i++) {
					lightPresetList.list[i] = new LightPreset(i);
				}
			}

			for(int i = 0; i < lightPresetList.list.Length; i++) {
				bool fold = GUIFoldout.Draw( "Preset " + (i + 1), lightPresetList.list[i]);

				if (fold == false) {
					continue;
				}

				EditorGUI.indentLevel++;

				lightPresetList.list[i].name = EditorGUILayout.TextField ("Name", lightPresetList.list[i].name);

				EditorGUILayout.Space();
				
				DrawLightLayers(lightPresetList.list[i].layerSetting);

				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class EventPresets {
		public static void Draw(EventPresetList lightPresetList) {
			bool foldout = GUIFoldoutHeader.Begin( "Light Event Presets (" + (lightPresetList.list.Length - 1) + ")", lightPresetList);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

			int bufferCount = EditorGUILayout.IntSlider ("Count", lightPresetList.list.Length - 1, 1, 4) + 1;

			if (bufferCount !=lightPresetList.list.Length) {
				int oldCount = lightPresetList.list.Length;

				System.Array.Resize(ref lightPresetList.list, bufferCount);

				for(int i = oldCount; i < bufferCount; i++) {
					lightPresetList.list[i] = new EventPreset(i);
				}
			}

			for(int i = 1; i < lightPresetList.list.Length; i++) {
				bool fold = GUIFoldout.Draw( "Preset " + (i), lightPresetList.list[i]);

				if (fold == false) {
					continue;
				}

				EditorGUI.indentLevel++;

				lightPresetList.list[i].name = EditorGUILayout.TextField ("Name", lightPresetList.list[i].name);

				EditorGUILayout.Space();
				
				DrawEventLayers(lightPresetList.list[i].layerSetting);

				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	static public void DrawEventLayers(EventPresetLayers presetLayers) {
		LayerEventSetting[] layerSetting = presetLayers.Get();

		int layerCount = layerSetting.Length;

		layerCount = EditorGUILayout.IntSlider("Layer Count", layerCount, 1, 4);

		EditorGUILayout.Space();

		if (layerCount != layerSetting.Length) {
			int oldCount = layerSetting.Length;

			System.Array.Resize(ref layerSetting, layerCount);

			for(int i = oldCount; i < layerCount; i++) {
				
				if (layerSetting[i] == null) {
					layerSetting[i] = new LayerEventSetting();
					layerSetting[i].layerID = i;
				}
				
			}

			presetLayers.SetArray(layerSetting);
		}

		for(int i = 0; i < layerSetting.Length; i++) {
			LayerEventSetting layer = layerSetting[i];

			layer.layerID = EditorGUILayout.Popup(" ", layer.layerID, Lighting2D.Profile.layers.lightLayers.GetNames());
		}

	}

	static public void DrawLightLayers(LightPresetLayers presetLayers) {
		LayerSetting[] layerSetting = presetLayers.Get();

		int layerCount = layerSetting.Length;

		layerCount = EditorGUILayout.IntSlider("Layer Count", layerCount, 1, 4);

		EditorGUILayout.Space();

		if (layerCount != layerSetting.Length) {
			int oldCount = layerSetting.Length;

			System.Array.Resize(ref layerSetting, layerCount);

			for(int i = oldCount; i < layerCount; i++) {
				
				if (layerSetting[i] == null) {
					layerSetting[i] = new LayerSetting();
					layerSetting[i].layerID = i;
				}
				
			}

			presetLayers.SetArray(layerSetting);
		}

		for(int i = 0; i < layerSetting.Length; i++) {
			LayerSetting layer = layerSetting[i];

			bool foldout = GUIFoldout.Draw( "Layer " + (i + 1), layer);

			if (foldout) {
				EditorGUI.indentLevel++;
			
				layer.layerID = EditorGUILayout.Popup("Layer (Light)", layer.layerID, Lighting2D.Profile.layers.lightLayers.GetNames());
				
				layer.type = (LightLayerType)EditorGUILayout.EnumPopup("Type", layer.type);

				bool shadowEnabled = layer.type != LightLayerType.MaskOnly;
				bool maskEnabled = layer.type != LightLayerType.ShadowOnly;
				
				EditorGUILayout.Space();

				layer.sorting = (LightLayerSorting)EditorGUILayout.EnumPopup("Sorting", layer.sorting);
				
				EditorGUI.BeginDisabledGroup(layer.sorting == LightLayerSorting.None);
				
				layer.sortingIgnore = (LightLayerSortingIgnore)EditorGUILayout.EnumPopup("Sorting Ignore", layer.sortingIgnore);
				
				EditorGUI.EndDisabledGroup();

				EditorGUILayout.Space();

				EditorGUI.BeginDisabledGroup(shadowEnabled == false);

				layer.shadowEffect = (LightLayerShadowEffect)EditorGUILayout.EnumPopup("Shadow Effect", layer.shadowEffect);

				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(shadowEnabled == false || layer.shadowEffect != LightLayerShadowEffect.Projected);

				layer.shadowEffectLayer = EditorGUILayout.Popup("Effect Layer (Light)", layer.shadowEffectLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

				EditorGUI.EndDisabledGroup();

				EditorGUILayout.Space();

				EditorGUI.BeginDisabledGroup(maskEnabled == false);

				layer.maskEffect = (LightLayerMaskEffect)EditorGUILayout.EnumPopup("Mask Effect", layer.maskEffect);

				EditorGUI.EndDisabledGroup();

				bool maskEffectLit = (layer.maskEffect == LightLayerMaskEffect.AboveLit);
		
				EditorGUI.BeginDisabledGroup(maskEnabled == false || maskEffectLit == false);
			
				layer.maskEffectDistance = EditorGUILayout.FloatField("Mask Effect Distance", layer.maskEffectDistance);

				if (layer.maskEffectDistance < 0) {
					layer.maskEffectDistance = 0;
				}
		
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.Space();

				EditorGUI.indentLevel--;
			}

			
			EditorGUILayout.Space();
		}

	}

	public class BufferPresets {
		public static void Draw(BufferPresetList bufferList) {
			bool foldout = GUIFoldoutHeader.Begin( "Buffer Presets (" + bufferList.list.Length + ")", bufferList);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

			int bufferCount = EditorGUILayout.IntSlider ("Count", bufferList.list.Length, 1, 4);

			if (bufferCount != bufferList.list.Length) {
				int oldCount = bufferList.list.Length;

				System.Array.Resize(ref bufferList.list, bufferCount);

				for(int i = oldCount; i < bufferCount; i++) {
					bufferList.list[i] = new BufferPreset(i);
				}
			}

			for(int i = 0; i < bufferList.list.Length; i++) {
				bool fold = GUIFoldout.Draw( "Preset " + (i + 1), bufferList.list[i]);

				if (fold == false) {
					continue;
				}

				EditorGUI.indentLevel++;

				bufferList.list[i].name = EditorGUILayout.TextField ("Name", bufferList.list[i].name);

				EditorGUILayout.Space();

				CommonSettings(bufferList.list[i]);

				EditorGUILayout.Space();

				if (Lighting2D.ProjectSettings.renderingMode == RenderingMode.OnRender) {
					SortingLayer(bufferList.list[i].sortingLayer);	
				}
				
				EditorGUILayout.Space();

				LayerSettings.DrawList(bufferList.list[i].dayLayers, "Day Layers (" + bufferList.list[i].dayLayers.list.Length + ")", Lighting2D.Profile.layers.dayLayers, true);

				EditorGUILayout.Space();
				
				LayerSettings.DrawList(bufferList.list[i].nightLayers, "Night Layers (" + bufferList.list[i].nightLayers.list.Length  + ")", Lighting2D.Profile.layers.nightLayers, false);

				EditorGUILayout.Space();

				EditorGUI.indentLevel--;
			}
		
			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class Layers {

        public static void Draw(LightingSettings.Profile profile) {
            bool foldout = GUIFoldoutHeader.Begin("Layers", profile.layers);
    
            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                DrawList(profile.layers.lightLayers, "Light Layers", "Light Layer");

                EditorGUILayout.Space();

                DrawList(profile.layers.nightLayers, "Night Layers", "Night Layer");

                EditorGUILayout.Space();

                DrawList(profile.layers.dayLayers, "Day Layers", "Day Layer");

            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }

        public static void DrawList(LightingSettings.LayersList layerList, string name, string singular) {
            bool foldout = GUIFoldout.Draw(name, layerList);

            if (foldout == false) {
                return;
            }
            
            EditorGUI.indentLevel++;

            int lightLayerCount = EditorGUILayout.IntSlider ("Count", layerList.names.Length, 1, 10);

            if (lightLayerCount != layerList.names.Length) {
                int oldCount = layerList.names.Length;

                System.Array.Resize(ref layerList.names, lightLayerCount);

                for(int i = oldCount; i < lightLayerCount; i++) {
                    layerList.names[i] = singular + " " + (i);
                }

            }

            for(int i = 0; i < lightLayerCount; i++) {
                layerList.names[i] = EditorGUILayout.TextField(" ", layerList.names[i]);
            }

            EditorGUI.indentLevel--;
        }
    }

	public class QualitySettings {

		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Quality", profile.qualitySettings);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}
	
			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.qualitySettings.projection = (Projection)EditorGUILayout.EnumPopup("Projection", profile.qualitySettings.projection);
				        
				profile.qualitySettings.coreAxis = (CoreAxis)EditorGUILayout.EnumPopup("Core Axis", profile.qualitySettings.coreAxis);

				profile.qualitySettings.updateMethod = (LightingSettings.UpdateMethod)EditorGUILayout.EnumPopup("Update Method", profile.qualitySettings.updateMethod);

               	profile.qualitySettings.lightTextureSize = (LightingSourceTextureSize)EditorGUILayout.Popup("Light Resolution", (int)profile.qualitySettings.lightTextureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);
				
 				profile.qualitySettings.lightEffectTextureSize = (LightingSourceTextureSize)EditorGUILayout.Popup("Light Effect Resolution", (int)profile.qualitySettings.lightEffectTextureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);

				profile.qualitySettings.lightFilterMode = (FilterMode)EditorGUILayout.EnumPopup("Light Filter Mode", profile.qualitySettings.lightFilterMode);
				
				profile.qualitySettings.HDR = EditorGUILayout.Toggle("Light HDR", profile.qualitySettings.HDR);

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class FogOfWar {
		// Fog Of War Preset????
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Fog of War (RT)", profile.fogOfWar); 

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.fogOfWar.useOnlyInPlay = EditorGUILayout.Toggle("Use Only in Play", profile.fogOfWar.useOnlyInPlay);

				profile.fogOfWar.sorting = (FogOfWarSorting) EditorGUILayout.EnumPopup("Sorting", profile.fogOfWar.sorting);

				profile.fogOfWar.filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", profile.fogOfWar.filterMode);

				SortingLayer(profile.fogOfWar.sortingLayer);			

			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}
	}

	public class DayLighting {
		public static void Draw(LightingSettings.Profile profile) {
			bool foldout = GUIFoldoutHeader.Begin( "Day Lighting", profile.dayLightingSettings);

			if (foldout == false) {
				GUIFoldoutHeader.End();
				return;
			}

			EditorGUI.indentLevel++;

				EditorGUILayout.Space();

				profile.dayLightingSettings.alpha = EditorGUILayout.Slider("Alpha", profile.dayLightingSettings.alpha, 0, 1);

				profile.dayLightingSettings.direction = EditorGUILayout.Slider("Direction", profile.dayLightingSettings.direction, 0 , 360);

				profile.dayLightingSettings.height = EditorGUILayout.Slider("Height", profile.dayLightingSettings.height, 0.1f, 10);

				SunPenumbra.Draw(profile);

				NormalMap.Draw(profile);
		
			EditorGUI.indentLevel--;

			GUIFoldoutHeader.End();
		}

		public class NormalMap {
			public static void Draw(LightingSettings.Profile profile) {
				bool foldout = GUIFoldout.Draw( "Normal Map", profile.dayLightingSettings.bumpMap);

				if (foldout == false) {
					return;
				}

				EditorGUI.indentLevel++;

					profile.dayLightingSettings.bumpMap.height = EditorGUILayout.Slider("Height", profile.dayLightingSettings.bumpMap.height, 0, 5);
					profile.dayLightingSettings.bumpMap.strength = EditorGUILayout.Slider("Strength", profile.dayLightingSettings.bumpMap.strength, 0, 5);
					
				EditorGUI.indentLevel--;
			}
		}

		public class SunPenumbra {
			public static void Draw(LightingSettings.Profile profile) {
				bool foldout = GUIFoldout.Draw( "Softness", profile.dayLightingSettings.softness);

				if (foldout == false) {
					return;
				}
		
				EditorGUI.indentLevel++;	

					profile.dayLightingSettings.softness.enable = EditorGUILayout.Toggle("Enable", profile.dayLightingSettings.softness.enable);
					profile.dayLightingSettings.softness.intensity = EditorGUILayout.FloatField("Intensity", profile.dayLightingSettings.softness.intensity);

					if (profile.dayLightingSettings.softness.intensity < 0) {
						profile.dayLightingSettings.softness.intensity = 0;
					}		
				EditorGUI.indentLevel--;
			}	
		}
	}

	static void CommonSettings(BufferPreset bufferPreset) {
		bufferPreset.darknessColor = EditorGUILayout.ColorField("Darkness Color", bufferPreset.darknessColor);
		bufferPreset.darknessColor.a = EditorGUILayout.Slider("Darkness Alpha", bufferPreset.darknessColor.a, 0, 1);
	
		bufferPreset.lightingResolution = EditorGUILayout.Slider("Resolution", bufferPreset.lightingResolution, 0.25f, 1.0f);
	}

	static void SortingLayer(LightingSettings.SortingLayer sortingLayer) {
		EditorGUI.BeginDisabledGroup(Lighting2D.ProjectSettings.renderingMode != RenderingMode.OnRender);

		GUISortingLayer.Draw(sortingLayer);

		EditorGUI.EndDisabledGroup();
	}

	public class LayerSettings {

		public static void DrawList(PresetLayers bufferLayers, string name, LayersList layerList, bool drawType) {
			bool foldout = GUIFoldout.Draw(name, bufferLayers);

			if (foldout == false) {
				return;
			}

			EditorGUI.indentLevel++;

			LightingLayerSetting[] layerSettings = bufferLayers.Get();
		
			int layerCount = EditorGUILayout.IntSlider ("Count", layerSettings.Length, 0, 10);

			EditorGUILayout.Space();
			
			if (layerCount != layerSettings.Length) {
				int oldCount = layerSettings.Length;

				System.Array.Resize(ref layerSettings, layerCount);

				for(int i = oldCount; i < layerCount; i++) {
					
					if (layerSettings[i] == null) {
						layerSettings[i] = new LightingLayerSetting();
						layerSettings[i].layer = i;
					}
					
				}

				bufferLayers.SetArray(layerSettings);
			}

			for(int i = 0; i < layerSettings.Length; i++) {
				layerSettings[i].layer = EditorGUILayout.Popup("Layer", layerSettings[i].layer, layerList.GetNames());

				if (drawType) {
					layerSettings[i].type = (LayerType)EditorGUILayout.EnumPopup("Type", layerSettings[i].type);
				}
				
				layerSettings[i].sorting = (LayerSorting)EditorGUILayout.EnumPopup("Sorting", layerSettings[i].sorting);
				

				EditorGUILayout.Space();
			}

			EditorGUI.indentLevel--;
		}
	}
}