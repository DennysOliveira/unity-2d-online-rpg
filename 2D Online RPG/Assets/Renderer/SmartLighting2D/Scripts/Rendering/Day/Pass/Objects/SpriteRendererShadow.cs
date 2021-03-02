using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day {

    public class SpriteRendererShadow {
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

        static public void Draw(DayLightCollider2D id, Vector2 offset) {
            if (id.mainShape.shadowType != DayLightCollider2D.ShadowType.Sprite) {
                return;
            }

            if (id.InAnyCamera() == false) {
                return;
            }

            Material material = Lighting2D.materials.GetSpriteShadow();
            material.color = Color.black;

            foreach(DayLightColliderShape shape in id.shapes) {
                
                SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();
                if (spriteRenderer == null) {
                    continue;
                }
                
                virtualSpriteRenderer.sprite = spriteRenderer.sprite;
                virtualSpriteRenderer.flipX = spriteRenderer.flipX;
                virtualSpriteRenderer.flipY = spriteRenderer.flipY;

                if (virtualSpriteRenderer.sprite == null) {
                    continue;
                }
                                    
                float x = id.transform.position.x + offset.x;
                float y = id.transform.position.y + offset.y;

                float rot = -Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;

                float sunHeight = Lighting2D.DayLightingSettings.height;

                x += Mathf.Cos(rot) * id.mainShape.height * sunHeight;
                y += Mathf.Sin(rot) * id.mainShape.height * sunHeight;

                material.mainTexture = virtualSpriteRenderer.sprite.texture;

                Vector2 scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

                Universal.Sprite.FullRect.Simple.Draw(id.spriteMeshObject, material, virtualSpriteRenderer, new Vector2(x, y), scale, id.transform.rotation.eulerAngles.z);
            }

            material.color = Color.white;
        }
    }
}