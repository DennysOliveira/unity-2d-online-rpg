using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class LightSource {

       static public void Draw(Light2D light, Camera camera) {
            if (light.Buffer == null) {
                return;
            }

            if (light.isActiveAndEnabled == false) {
                return;
            }

            if (light.InAnyCamera() == false) {
                return;
            }

            Vector2 pos = LightingPosition.GetPosition2D(-camera.transform.position);
            Vector2 size = new Vector2(light.size, light.size);

            if (light.IsPixelPerfect()) {
                size = LightingRender2D.GetSize(camera);
                pos = Vector2.zero;
            } else {
                pos += light.transform2D.position;
            }
         
            Color lightColor = light.color;
            lightColor.a = light.color.a / 2;

            Material material = Lighting2D.materials.GetLight();
            material.mainTexture = light.Buffer.renderTexture.renderTexture;
            material.SetColor ("_TintColor", lightColor);

            Rendering.Universal.Texture.Draw(material, pos, size, 0);
        }
    }
}