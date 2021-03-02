using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

namespace Rendering.Light {
	
	public static class Main {
		private static Pass pass = new Pass();

		public static void Draw(Light2D light) {
			ShadowEngine.Prepare(light);

            LayerSetting[] layerSettings = light.GetLayerSettings();

			if (layerSettings == null) {
				return;
			}

			if (layerSettings.Length < 1) {
				return;
			}

			for (int layerID = 0; layerID < layerSettings.Length; layerID++) {
				LayerSetting layerSetting = layerSettings[layerID];

				if (layerSetting == null) {
					continue;
				}

				if (pass.Setup(light, layerSetting) == false) {
					continue;
				}

				ShadowEngine.SetPass(light, layerSetting);

				if (layerSetting.sorting == LightLayerSorting.None) {
					NoSort.Draw(pass);
					
				} else {
					pass.sortPass.SortObjects();

					Sorted.Draw(pass);
				}
			}
	
			LightSource.Main.Draw(light);
		}

		
		public static void DrawCollisions(Light2D light) {
			ShadowEngine.Prepare(light);

            LayerSetting[] layerSettings = light.GetLayerSettings();

			if (layerSettings == null) {
				return;
			}

			if (layerSettings.Length < 1) {
				return;
			}

			for (int layerID = 0; layerID < layerSettings.Length; layerID++) {
				LayerSetting layerSetting = layerSettings[layerID];

				if (layerSetting == null) {
					continue;
				}

				if (pass.Setup(light, layerSetting) == false) {
					continue;
				}

				ShadowEngine.SetPass(light, layerSetting);

				NoSort.Shadows.Draw(pass);
			}
	
			// LightSource.Main.Draw(light);
		}
	}
}