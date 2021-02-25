using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
	public class MaterialManager {
		static SmartMaterial vertexLit = null;
		static SmartMaterial additive = null;
		static SmartMaterial alpha = null;
		static SmartMaterial sprite = null;

		static private SmartMaterial GetVertexLit() {
			if (vertexLit == null || vertexLit.material == null) {
				//if (Slicer2DSettings.GetRenderingPipeline() == Slicer2DSettings.RenderingPipeline.Universal) {
					vertexLit =  new SmartMaterial ("Legacy Shaders/Transparent/VertexLit");
				//} else {
				//	vertexLit =  new SmartMaterial ("Sprites/Default");
				//}

				if (vertexLit != null) {
					vertexLit.SetTexture(Resources.Load ("Textures/LineTexture16") as Texture);
				}
			}
			return(vertexLit);
		}

		static private SmartMaterial GetAdditive() {
			if (additive == null || additive.material == null) {
				additive =  new SmartMaterial ("Mobile/Particles/Additive");
			}
			return(additive);
		}

		static private SmartMaterial GetAlpha() {
			if (alpha == null || alpha.material == null) {
				alpha =  new SmartMaterial ("Mobile/Particles/Alpha Blended");
			}
			return(alpha);
		}

		static private SmartMaterial GetSprite() {
			if (sprite == null || sprite.material == null) {
				sprite =  new SmartMaterial ("Sprites/Default");
			}
			return(sprite);
		}

		static public SmartMaterial GetVertexLitCopy() {
			return(new SmartMaterial(GetVertexLit()));
		}
		
		static public SmartMaterial GetAdditiveCopy() {
			return(new SmartMaterial(GetAdditive()));
		}

		static public SmartMaterial GetAlphaCopy() {
			return(new SmartMaterial(GetAlpha()));
		}

		static public SmartMaterial GetSpriteCopy() {
			return(new SmartMaterial(GetSprite()));
		}
	}
}