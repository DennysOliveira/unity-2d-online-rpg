using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

namespace Rendering.Light {

    public class SpriteRenderer2D {

        public static void Mask(Light2D light, LightCollider2D id, Material material, LayerSetting layerSetting) {
            if (id.InLight(light) == false) {
                return;
            }

            Texture2D currentTexture = null;
            Color currentColor = Color.black;

            GL.Begin(GL.QUADS);

            foreach(LightColliderShape shape in id.shapes) {
                UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

                Sprite sprite = shape.spriteShape.GetOriginalSprite();
                if (sprite == null || spriteRenderer == null) {
                    continue;
                }

                if (sprite.texture == null) {
                    continue;
                }

                Vector2 position = shape.transform2D.position - light.transform2D.position;
                Color color = LayerSettingColor.Get(shape, position, layerSetting, id.maskEffect, id.maskTranslucency);

                if (currentTexture != sprite.texture || currentColor != color) {
                    currentTexture = sprite.texture;
                    currentColor = color;

                    material.mainTexture = currentTexture;
                    material.color = currentColor;

                    material.SetPass(0);
                }

                Rendering.Universal.Sprite.FullRect.DrawPass(id.spriteMeshObject, spriteRenderer, position, shape.transform2D.scale, shape.transform2D.rotation);	
            }

            GL.End();

            material.mainTexture = null;
		}

        public static void MaskNormalMap(Light2D light, LightCollider2D id, Material material, LayerSetting layerSetting) {
            if (id.InLight(light) == false) {
                return;
            }

            Texture normalTexture = id.bumpMapMode.GetBumpTexture();

            if (normalTexture == null) {
                return;
            }

            float rotation;

            material.SetTexture("_Bump", normalTexture);

            foreach(LightColliderShape shape in id.shapes) {
                UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

                if (spriteRenderer == null) {
                    continue;
                }

                Sprite sprite = shape.spriteShape.GetOriginalSprite();
                if (sprite == null) {
                    continue;
                }

                if (sprite.texture == null) {
                    continue;
                }
               
                Vector2 position = shape.transform2D.position - light.transform2D.position;

                material.mainTexture = sprite.texture;
                material.color = LayerSettingColor.Get(position, layerSetting, id.maskEffect, id.maskTranslucency);
                
                float color = material.color.r;
                
                switch(id.bumpMapMode.type) {
                    case NormalMapType.ObjectToLight:
                        rotation = Mathf.Atan2(light.transform2D.position.y - shape.transform2D.position.y, light.transform2D.position.x - shape.transform2D.position.x);
                        rotation -= Mathf.Deg2Rad * (shape.transform2D.rotation);
                        
                        material.SetFloat("_LightRX", Mathf.Cos(rotation) * 2);
                        material.SetFloat("_LightRY", Mathf.Sin(rotation) * 2);
                        material.SetFloat("_LightColor",  color);

                    break;

                    case NormalMapType.PixelToLight:
                        material.SetFloat("_LightColor",  color);
                    
                        rotation = shape.transform2D.rotation * Mathf.Deg2Rad;

                        Vector2 sc = shape.transform2D.scale;
                        sc = sc.normalized;

                        material.SetFloat("_LightX", Mathf.Cos(rotation) * sc.x );
                        material.SetFloat("_LightY", Mathf.Cos(rotation) * sc.y );

                        material.SetFloat("_Depth", id.bumpMapMode.depth);

                        if (id.bumpMapMode.invertX) {
							material.SetFloat("_InvertX", -1);
						} else {
							material.SetFloat("_InvertX", 1);
						}
						
                        if (id.bumpMapMode.invertY) {
							material.SetFloat("_InvertY", -1);
						} else {
							material.SetFloat("_InvertY", 1);
						}

                    break;
                }

                Rendering.Universal.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, position, shape.transform2D.scale, shape.transform2D.rotation);
            }
        }
    }
}