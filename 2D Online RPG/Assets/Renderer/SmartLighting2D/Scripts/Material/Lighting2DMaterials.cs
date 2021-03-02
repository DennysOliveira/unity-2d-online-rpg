using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Lighting2DMaterials {
	private Sprite penumbraSprite;
	private Sprite atlasPenumbraSprite;

	private Sprite whiteSprite;
	private Sprite blackSprite;

	private Sprite atlasSpriteMaskTexture;
	private Sprite atlasBlackMaskSprite;

	private LightingMaterial occlusionEdge = null;
	private LightingMaterial occlusionBlur = null;
	private LightingMaterial shadowBlur = null;
	private LightingMaterial additive = null;
	private LightingMaterial light = null;
	private LightingMaterial multiplyHDR = null;
	private LightingMaterial alphablend = null;

	private LightingMaterial spriteProjection = null;

	private LightingMaterial mask = null;
	private LightingMaterial roomMask = null;

	private LightingMaterial softShadow = null;

	private LightingMaterial spriteShadow = null;
	
	private LightingMaterial roomMultiply = null;


	private LightingMaterial normalPixelToLightSprite = null;
	private LightingMaterial normalObjectToLightSprite = null;

	private LightingMaterial bumpedDaySprite = null;

	private LightingMaterial atlasMaterial = null;

	public bool hdr = false;
	private bool initialized = false;

	public Sprite GetPenumbraSprite() {
		if (penumbraSprite == null) {
			penumbraSprite = Resources.Load<Sprite>("textures/penumbra"); 
		}
		return(penumbraSprite);
	}

	public Sprite GetAtlasPenumbraSprite() {
		if (atlasPenumbraSprite == null) {
			atlasPenumbraSprite = AtlasSystem.Manager.RequestSprite(GetPenumbraSprite(), AtlasSystem.Request.Type.BlackMask);
		}
		return(atlasPenumbraSprite);
	}

	public Sprite GetBlackSprite() {
		if (blackSprite == null) {
			blackSprite = Resources.Load<Sprite>("textures/black"); 
		}
		return(blackSprite);
	}

	public Sprite GetWhiteSprite() {
		if (whiteSprite == null) {
			whiteSprite = Resources.Load<Sprite>("textures/white"); 
		}
		return(whiteSprite);
	}

	public Sprite GetAtlasWhiteMaskSprite() {
		if (atlasSpriteMaskTexture == null) {
			atlasSpriteMaskTexture = AtlasSystem.Manager.RequestSprite(GetWhiteSprite(), AtlasSystem.Request.Type.WhiteMask);
		}
		return(atlasSpriteMaskTexture);
	}

	public Sprite GetAtlasBlackMaskSprite() {
		if (atlasBlackMaskSprite == null) {
			atlasBlackMaskSprite = AtlasSystem.Manager.RequestSprite(GetBlackSprite(), AtlasSystem.Request.Type.Normal);
		}
		return(atlasBlackMaskSprite);
	}

	public bool Initialize(bool allowHDR) {
		if (initialized == true) {
			if (allowHDR == hdr) {
				return(false);
			}
		}

		hdr = allowHDR;

		Reset();

		initialized = true;

		GetPenumbraSprite();
		GetAtlasPenumbraSprite();

		GetWhiteSprite();
		GetBlackSprite();

		GetAtlasWhiteMaskSprite();
		GetAtlasBlackMaskSprite();

		GetAdditive();
		GetLight();

		GetOcclusionBlur();
		GetOcclusionEdge();
		GetShadowBlur();

		GetMask();

		GetRoomMask();
		GetRoomMultiply();

		GetSpriteShadow();

		GetNormalMapSpritePixelToLight();
		
		GetBumpedDaySprite();

		GetAtlasMaterial();

		return(true);
	}

	public void Reset() {
		initialized = false; // is it the best way?
	
		penumbraSprite = null;
		atlasPenumbraSprite = null;

		whiteSprite = null;
		blackSprite = null;

		atlasSpriteMaskTexture = null;
		atlasBlackMaskSprite = null;

		occlusionEdge = null;
		occlusionBlur = null;
		shadowBlur = null;
		additive = null;
		multiplyHDR = null;
		alphablend = null;

		spriteProjection = null;

		mask = null;
		spriteShadow = null;

		atlasMaterial = null;
	}

		
	public Material GetLight() {
		if (light == null || light.Get() == null) {
			light = LightingMaterial.Load("Light2D/Internal/Light");
		}

		if (Lighting2D.ProjectSettings.colorSpace == LightingSettings.ColorSpace.Linear) {
			light.Get().SetFloat("_LinearColor", 1);
		} else {
			light.Get().SetFloat("_ColorSpace", 0);
		}
	
		return(light.Get());
	}


	public Material GetAtlasMaterial() {
		if (atlasMaterial == null || atlasMaterial.Get() == null) {
			atlasMaterial = LightingMaterial.Load("Light2D/Internal/AlphaBlended");
		}
		
		atlasMaterial.SetTexture(AtlasSystem.Manager.GetAtlasPage().GetTexture());

		return(atlasMaterial.Get());
	}

	public Material GetSpriteProjectionMaterial() {
		if (spriteProjection == null || spriteProjection.Get() == null) {
			spriteProjection = LightingMaterial.Load("Light2D/Internal/SpriteProjection");
		}
		
		return(spriteProjection.Get());
	}
	
	public Material GetAdditive() {
		if (additive == null || additive.Get() == null) {
			additive = LightingMaterial.Load("Light2D/Internal/Additive");
		}
		return(additive.Get());
	}

	public Material GetMultiplyHDR() {
		if (multiplyHDR == null || multiplyHDR.Get() == null) {
			if (hdr == true) {
				multiplyHDR = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				multiplyHDR = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
		}
		return(multiplyHDR.Get());
	}

	public Material GetRoomMultiply() {
		if (roomMultiply == null ||roomMultiply.Get() == null) {
		
			roomMultiply = LightingMaterial.Load("Light2D/Internal/RoomMultiply");
		
		}
		return(roomMultiply.Get());
	}

	public Material GetAlphaBlend() {
		if (alphablend == null || alphablend.Get() == null) {
			alphablend = LightingMaterial.Load("Light2D/Internal/AlphaBlended");

			alphablend.SetTexture("textures/white");
		}
		return(alphablend.Get());
	}

	public Material GetOcclusionEdge() {
		if (occlusionEdge == null || occlusionEdge.Get() == null) {
			if (hdr == true) {
				occlusionEdge = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				occlusionEdge = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
			
			occlusionEdge.SetTexture("textures/occlusionedge");
		}
		return(occlusionEdge.Get());
	}

	public Material GetShadowBlur() {
		if (shadowBlur == null || shadowBlur.Get() == null) {
			shadowBlur = LightingMaterial.Load("Light2D/Internal/AlphaBlended");
		
			shadowBlur.SetTexture("textures/shadowblur");
		}
		return(shadowBlur.Get());
	}

	public Material GetOcclusionBlur() {
		if (occlusionBlur == null || occlusionBlur.Get() == null) {
			if (hdr == true) {
				occlusionBlur = LightingMaterial.Load("Light2D/Internal/Multiply HDR");
			} else {
				occlusionBlur = LightingMaterial.Load("Light2D/Internal/Multiply");
			}
			
			occlusionBlur.SetTexture("textures/occlussionblur");
		}
		return(occlusionBlur.Get());
	}

	public Material GetMask() {
		if (mask == null || mask.Get() == null) {
			mask = LightingMaterial.Load("Light2D/Internal/Mask");
		}
		return(mask.Get());
	}

	public Material GetSpriteShadow() {
		if (spriteShadow == null || spriteShadow.Get() == null) {
			spriteShadow = LightingMaterial.Load("Light2D/Internal/SpriteShadow");
		}
		return(spriteShadow.Get());
	}


	public Material GetRoomMask() {
		if (roomMask == null || roomMask.Get() == null) {
			roomMask = LightingMaterial.Load("Light2D/Internal/RoomMask");
		}
		return(roomMask.Get());
	}

	public Material GetSoftShadow() {
		if (softShadow == null || softShadow.Get() == null) {
			softShadow = LightingMaterial.Load("Light2D/Internal/SoftShadow");
		}
		return(softShadow.Get());
	}

	


	public Material GetNormalMapSpritePixelToLight() {
		if (normalPixelToLightSprite == null || normalPixelToLightSprite.Get() == null) {
			normalPixelToLightSprite = LightingMaterial.Load("Light2D/Internal/NormalMapPixelToLight");
		}
		return(normalPixelToLightSprite.Get());
	}

	public Material GetNormalMapSpriteObjectToLight() {
		if (normalObjectToLightSprite== null || normalObjectToLightSprite.Get() == null) {
			normalObjectToLightSprite = LightingMaterial.Load("Light2D/Internal/NormalMapObjectToLight");
		}
		return(normalObjectToLightSprite.Get());
	}

	public Material GetBumpedDaySprite() {
		if (bumpedDaySprite == null || bumpedDaySprite.Get() == null) {
			bumpedDaySprite = LightingMaterial.Load("Light2D/Internal/DayBump");
		}
		return(bumpedDaySprite.Get());
	}


}