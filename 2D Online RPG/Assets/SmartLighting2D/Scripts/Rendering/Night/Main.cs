using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;
using LightSettings;

namespace Rendering.Night {
	
	public static class Main {

		static Pass pass = new Pass();

		public static void Draw(Camera camera, BufferPreset bufferPreset) {
			DarknessColor(camera, bufferPreset);

			LightingLayerSetting[] layerSettings = bufferPreset.nightLayers.Get();
			
			if (layerSettings == null) {
				return;
			}

			if (layerSettings.Length < 1) {
				return;
			}

			for(int i = 0; i < layerSettings.Length; i++) {
				LightingLayerSetting nightLayer = layerSettings[i];

				if (pass.Setup(nightLayer, camera) == false) {
					continue;
				}

				if (nightLayer.sorting == LayerSorting.None) {

					NoSort.Draw(pass);

				} else {

					pass.SortObjects();

					Sorted.Draw(pass);
				}
			}
		}

		private static void DarknessColor(Camera camera, BufferPreset bufferPreset) {
			Color color = bufferPreset.darknessColor;

			if (color.a > 0) {
				Material material = Lighting2D.materials.GetAlphaBlend();		
				material.SetColor ("_TintColor", color);
				material.mainTexture = null;

				float cameraRotation = -LightingPosition.GetCameraRotation(camera);

				Universal.Texture.Draw(material, Vector2.zero, LightingRender2D.GetSize(camera), cameraRotation, 0);
			}
		}		
	}
}