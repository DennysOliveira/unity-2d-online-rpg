using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightingSettings {

	[System.Serializable]
	public class EventPresetList {
		public EventPreset[] list = new EventPreset[2];

		public string[] GetBufferLayers() {
			string[] layers = new string[list.Length];

			for(int i = 0; i < list.Length; i++) {
				if (i == 0) {
					layers[i] = "Disabled";
				} else if (list[i].name.Length > 0) {
					layers[i] = list[i].name;
				} else {
					layers[i] = "Preset (Id: " + (i) + ")";
				}
				
			}

			return(layers);
		}

		public EventPreset[] Get() {
			for(int i = 0; i < list.Length; i++) {
				if (list[i] == null) {
					list[i] = new EventPreset(i);
				}
			}
			return(list);
		}
	}

	[System.Serializable]
	public class EventPreset {
		public string name = "Default";

		public EventPresetLayers layerSetting = new EventPresetLayers();

		public EventPreset (int id) {
			if (id == 0) {
				name = "Disabled";
			} else {
				name = "Preset (Id: " + id + ")";
			}
			
		}
	}

	[System.Serializable]
	public class EventPresetLayers {
		public LayerEventSetting[] list = new LayerEventSetting[1];

		public void SetArray(LayerEventSetting[] array) {
			list = array;
		}

		public LayerEventSetting[] Get() {
			for(int i = 0; i < list.Length; i++) {
				if (list[i] == null) {
					list[i] = new LayerEventSetting();
				}
			}
	
			return(list);
		}
	}
}

[System.Serializable]
public class LayerEventSetting {
	public int layerID = 0;

	public int GetLayerID() {
		int layer = (int)layerID;

		if (layer < 0) {
			return(-1);
		}

		return(layer);
	}
}