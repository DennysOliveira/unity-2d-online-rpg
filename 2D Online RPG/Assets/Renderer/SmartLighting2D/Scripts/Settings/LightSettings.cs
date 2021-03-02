using UnityEngine;
using UnityEngine.Events;
using LightSettings;

namespace LightSettings {
		
	// Light 2D
	public class LightEvent : UnityEvent <Light2D> {}

	public enum MaskEffect {Lit, Unlit, Isometric}

	public enum LightLayerType {ShadowAndMask, ShadowOnly, MaskOnly}

	public enum LightLayerSorting {None, SortingLayerAndOrder, DistanceToLight, YDistanceToLight, YAxisLower, YAxisHigher, ZAxisLower, ZAxisHigher, Isometric};
	public enum LightLayerSortingIgnore {None, IgnoreAbove};

	public enum LightLayerShadowEffect {Default, Projected, SoftObjects, SoftVertex, SpriteProjection};
	public enum LightLayerMaskEffect {AlwaysLit, AboveLit, NeverLit};

	public enum LayerSorting {None, ZAxisLower, ZAxisHigher, YAxisLower, YAxisHigher};
	public enum LayerType {ShadowsAndMask, ShadowsOnly, MaskOnly}

	public enum NormalMapTextureType {
		Texture,
		Sprite,
		SecondaryTexture
	}

	public enum NormalMapType {
		PixelToLight,
		ObjectToLight
	}

	public enum LightEventState {
		OnCollision, 
		OnCollisionEnter, 
		OnCollisionExit, 
		None
	}

}