using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpriteTransform {
    public Vector2 position;
    public Vector2 scale;
    public float rotation;

    public Rect uv;

    public SpriteTransform(VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
        UnityEngine.Sprite sprite = spriteRenderer.sprite;

        if (spriteRenderer == null || sprite == null) {
            this.rotation = 0;
            this.scale = Vector2.zero;
            this.uv = new Rect();
            this.position = Vector2.zero;

            return;
        }

        Texture2D spriteTexture = sprite.texture;

        float textureWidth = spriteTexture.width;
        float textureHeight = spriteTexture.height;

        Rect spriteRect = sprite.textureRect;

        Rect uvRect = new Rect();
        uvRect.x = spriteRect.x / textureWidth;
        uvRect.y = spriteRect.y / textureHeight;
        uvRect.width = spriteRect.width / textureWidth + uvRect.x;
        uvRect.height = spriteRect.height / textureHeight + uvRect.y;

        // Scale
        Vector2 textureScale = new Vector2();
        textureScale.x = textureWidth / spriteRect.width;
        textureScale.y = textureHeight / spriteRect.height;

        scale.x /= textureScale.x;
        scale.y /= textureScale.y;

        scale.x *= textureWidth / (sprite.pixelsPerUnit * 2);
        scale.y *= textureHeight / (sprite.pixelsPerUnit * 2);

        if (spriteRenderer.flipX) {
            scale.x = -scale.x;
        }

        if (spriteRenderer.flipY) {
            scale.y = -scale.y;
        }

        // Pivot
        Vector2 pivot = sprite.pivot;
        
        pivot.x /= spriteRect.width;
        pivot.y /= spriteRect.height;

        pivot.x -= 0.5f;
        pivot.y -= 0.5f;

        pivot.x *= scale.x * 2;
        pivot.y *= scale.y * 2;

        float pivotDist = Mathf.Sqrt(pivot.x * pivot.x + pivot.y * pivot.y);
        float pivotAngle = Mathf.Atan2(pivot.y, pivot.x);
        float rot = rotation * Mathf.Deg2Rad + Mathf.PI;
        
        this.position.x = position.x + Mathf.Cos(pivotAngle + rot) * pivotDist;
        this.position.y = position.y + Mathf.Sin(pivotAngle + rot) * pivotDist;

        this.uv = uvRect;

        this.scale = scale;

        this.rotation = rotation;
    }
}