using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public static class Main {

         public static void Draw(Light2D light) {
            if (light == null) {
                return;
            }

            UnityEngine.Sprite lightSprite = light.GetSprite();

            if (lightSprite == null) {
                return;
            }

            if (lightSprite.texture == null) {
                return;
            }

            Vector2 position = Vector2.zero;
            Vector2 size = new Vector2(light.size, light.size);
            float z = 0;

            Material material = Lighting2D.materials.GetMultiplyHDR();
            material.mainTexture = lightSprite.texture;

            if (light.IsPixelPerfect()) {
                position = ShadowEngine.drawOffset;
            }

            if (light.applyRotation) {
                material.SetPass(0);

                material.color = Color.white;

                LightSprite.Sprite.Draw(position, size, light.transform.rotation.eulerAngles.z, z, light.spriteFlipX, light.spriteFlipY);

                material.color = Color.black;

                Bounds.Draw(light, position, material, z);
                
            } else {
                material.SetPass (0); 

                material.color = Color.white;

                LightSprite.Sprite.Draw(position, size, 0, z, light.spriteFlipX, light.spriteFlipY);
            }

            if (light.spotAngle != 360) {
                Lighting2D.materials.GetAtlasMaterial().SetPass(0);

                GL.Begin(GL.TRIANGLES);

                GL.Color(Color.black);

                Angle.Draw(light, z);

                GL.End ();
            }
        }
    }
}