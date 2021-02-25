using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

namespace LightingSettings {
    
	[System.Serializable]
	public class BufferPresetList {
		public BufferPreset[] list = new BufferPreset[1];

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
	}

	[System.Serializable]
	public class BufferPreset {
		public string name = "Default";

		public SortingLayer sortingLayer = new SortingLayer();

		public Color darknessColor = new Color(0, 0, 0, 1);
		public float lightingResolution = 1f;

		public PresetLayers dayLayers = new PresetLayers();
		public PresetLayers nightLayers = new PresetLayers();

		public BufferPreset (int id) {
			if (id == 0) {
				name = "Default";
			} else {
				name = "Preset (Id: " + (id + 1) + ")";
			}

			sortingLayer.Order = 1;
		}
	}
	
	[System.Serializable]
	public class PresetLayers {
		public LightingLayerSetting[] list = new LightingLayerSetting[1];

		public void SetArray(LightingLayerSetting[] array) {
			list = array;
		}

		public LightingLayerSetting[] Get() {
			for(int i = 0; i < list.Length; i++) {
				if (list[i] == null) {
					list[i] = new LightingLayerSetting();
				}
			}
	
			return(list);
		}
	}

	[System.Serializable]
	public class LightingLayerSetting {
		public int layer = 0;
		public LayerType type =LayerType.ShadowsAndMask;
		public LayerSorting sorting = LayerSorting.None;

		public int GetLayerID() {
			int layerId = (int)layer;

			if (layerId < 0) {
				return(-1);
			}

			return(layerId);
		}
	}
}