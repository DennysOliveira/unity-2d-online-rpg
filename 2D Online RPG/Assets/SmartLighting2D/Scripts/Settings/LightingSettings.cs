using UnityEngine;
using UnityEngine.Events;
using LightSettings;

namespace LightingSettings {

	[System.Serializable]
	public class QualitySettings {
		public static string[] LightingSourceTextureSizeArray = new string[]{"Custom", "2048", "1024", "512", "256", "128",  "PixelPerfect"};

		public bool HDR = true;
	
		public LightingSourceTextureSize lightTextureSize = LightingSourceTextureSize.px2048;
		public LightingSourceTextureSize lightEffectTextureSize = LightingSourceTextureSize.px2048;
		
		public FilterMode lightFilterMode = FilterMode.Bilinear;
		public UpdateMethod updateMethod = UpdateMethod.LateUpdate;
		public CoreAxis coreAxis = CoreAxis.XY;

		public Projection projection = Projection.Orthographic;
	}

	[System.Serializable]
	public class Layers {
		public LayersList lightLayers = new LayersList();
		public LayersList nightLayers = new LayersList();
		public LayersList dayLayers = new LayersList();

		public Layers() {
			lightLayers.names[0] = "Default";

			nightLayers.names[0] = "Default";

			dayLayers.names[0] = "Default";
		}
	}

	[System.Serializable]
	public class LayersList {
		public string[] names = new string[1];

		public string[] GetNames() {
			string[] layers = new string[names.Length];

			for(int i = 0; i < names.Length; i++) {
				layers[i] = names[i];
			}

			return(layers);
		}
	}

	[System.Serializable]
	public class FogOfWar {
		public FilterMode filterMode = FilterMode.Bilinear;
		
		public bool useOnlyInPlay = false;

		public FogOfWarSorting sorting = FogOfWarSorting.None;

		[Range(0, 1)]
		public float resolution = 1;

		public SortingLayer sortingLayer = new SortingLayer();
	}

	[System.Serializable]
	public class DayLightingSettings {

		[Range(0, 1)]
		public float alpha = 1;

		[Range(0, 360)]
		public float direction = 270;

		[Range(0, 10)]
		public float height = 1;

		public Softness softness = new Softness();

		public BumpMap bumpMap = new BumpMap();

		[System.Serializable]
		public class Softness {
			public bool enable = true;
			public float intensity = 0.5f;
		}

		// Is this only bumpmap settings?
		[System.Serializable]
		public class BumpMap {
			[Range(0, 5)]
			public float height = 1;

			[Range(0, 5)]
			public float strength = 1;
		}
	}

	[System.Serializable]
	public class SortingLayer {
		[SerializeField]
		private string name = "Default";
		public string Name {
			get {

				if (name.Length < 1) {
					name = "Default";
				}

				return(name);
			} 

			set => name = value;
		}

		public int Order = 0;

		public void ApplyToMeshRenderer(MeshRenderer meshRenderer) {
			if (meshRenderer == null) {
				return;
			}
			
			if (meshRenderer.sortingLayerName != Name) {
				meshRenderer.sortingLayerName = Name;
			}

			if (meshRenderer.sortingOrder != Order) {
				meshRenderer.sortingOrder = Order;
			}
		}
	}

	[System.Serializable]
	public class EditorView {
		public EditorDrawGizmos drawGizmos = EditorDrawGizmos.Selected;
		public EditorGizmosBounds drawGizmosBounds = EditorGizmosBounds.None; 
		
		public int sceneViewLayer = 0;

		public int gameViewLayer = 0;

		public int fowSceneViewLayer = 0;
		public int fowGameViewLayer = 0;
	}

	[System.Serializable]
	public class Chunks {
		public bool enabled = false;

		public int chunkSize = 10;
	}

	[System.Serializable]
	public class MeshMode {
		public bool enable = false;

		[Range(0, 1)]
		public float alpha = 0.5f;

		public MeshModeShader shader = MeshModeShader.Additive;
		public Material[] materials = new Material[1];

		public LightingSettings.SortingLayer sortingLayer = new LightingSettings.SortingLayer();
	}

	
	[System.Serializable]
	public class BumpMapMode {
		public NormalMapType type = NormalMapType.PixelToLight;
		
		public NormalMapTextureType textureType = NormalMapTextureType.Texture;
		
		public Texture texture;
		public Sprite sprite;

		public bool invertX = false;
		public bool invertY = false;

		[Range(0, 1)]
		public float depth = 1;

		public SpriteRenderer spriteRenderer;

		public void SetSpriteRenderer(SpriteRenderer spriteRenderer) {
			this.spriteRenderer = spriteRenderer;
		}

		public Texture GetBumpTexture() {
			switch(textureType) {
				case NormalMapTextureType.Sprite:
					if (sprite == null) {
						return(null);
					}

					return(sprite.texture);

				case NormalMapTextureType.Texture:
					return(texture);

				case NormalMapTextureType.SecondaryTexture:
					MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
					spriteRenderer.GetPropertyBlock(matBlock);
					Texture secondaryTexture = matBlock.GetTexture("_SecondaryTex");

					Debug.Log("done" + secondaryTexture);
					return(null);
			}
			
			return(null);
		}

		public Material SelectMaterial(Material pixel, Material direction) {
			Material material = pixel;

			if (type == NormalMapType.ObjectToLight) {
				material = direction;
			}

			return(material);
		}
	}

	[System.Serializable]
	public class DayNormalMapMode {
		public NormalMapTextureType textureType = NormalMapTextureType.Texture;
		
		public Texture texture;
		public Sprite sprite;

		public Texture GetBumpTexture() {
			switch(textureType) {
				case NormalMapTextureType.Sprite:
					if (sprite == null) {
						return(null);
					}

					return(sprite.texture);

				case NormalMapTextureType.Texture:
					return(texture);
			}
			
			return(null);
		}
	}

	[System.Serializable]
	public class GlowMode {
		public bool enable = false;

		[Range(1, 10)]
		public int glowSize = 1;

		[Range(1, 10)]
		public int glowIterations = 1;
	}

		public enum MeshModeShader {
		Additive, 
		Alpha, 
		FogOfWar, 
		Custom
	}

	public enum FogOfWarSorting {
		None, 
		ZAxisLower, 
		YAxisLower, 
		SortingOrder, 
		SortingOrderAndLayer
	}

	public enum EditorDrawGizmos {
		Disabled, 
		Selected, 
		Always
	}

	public enum EditorGizmosBounds {
		None, 
		Rectangle
	}

	public enum ManagerInternal {
		HideInHierarchy,
		ShowInHierarchy
	}
	public enum ManagerInstance {
		Static,
		DontDestroyOnLoad,
		Dynamic
	}

	public enum UpdateMethod {
		LateUpdate,
		OnPreCull,
		OnRenderObject,
	}
	
}