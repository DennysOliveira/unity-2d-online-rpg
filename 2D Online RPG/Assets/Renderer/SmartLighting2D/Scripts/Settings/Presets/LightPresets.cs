using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightingSettings {

	[System.Serializable]
	public class LightPresetList {
		public LightPreset[] list = new LightPreset[1];

		public string[] GetBufferLayers() {
			string[] layers = new string[list.Length];

			for(int i = 0; i < list.Length; i++) {
				if (list[i].name.Length > 0) {
					layers[i] = list[i].name;
				} else {
					layers[i] = "Preset (Id: " + (i + 1) + ")";
				}
				
			}

			return(layers);
		}

		public LightPreset[] Get() {
			for(int i = 0; i < list.Length; i++) {
				if (list[i] == null) {
					list[i] = new LightPreset(i);
				}
			}
			return(list);
		}
	}

	[System.Serializable]
	public class LightPreset {
		public string name = "Default";

		public LightPresetLayers layerSetting = new LightPresetLayers();

		public LightPreset (int id) {
			if (id == 0) {
				name = "Default";
			} else {
				name = "Preset (Id: " + (id + 1) + ")";
			}
			
		}
	}

	[System.Serializable]
	public class LightPresetLayers {
		public LayerSetting[] list = new LayerSetting[1];

		public void SetArray(LayerSetting[] array) {
			list = array;
		}

		public LayerSetting[] Get() {
			for(int i = 0; i < list.Length; i++) {
				if (list[i] == null) {
					list[i] = new LayerSetting();
				}
			}
	
			return(list);
		}
	}
}