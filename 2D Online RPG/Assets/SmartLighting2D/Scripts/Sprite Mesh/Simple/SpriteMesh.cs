using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class SpriteMesh {
    Mesh mesh;

    public SpriteMesh () {
        mesh = new Mesh();
    }

    public Mesh Get(SpriteRenderer spriteRenderer , Vector2 size, Vector2 uv0, Vector2 uv1) {
        Rect uvRect = new Rect();
        Vector2 scale = Vector2.one;

        UnityEngine.Sprite sprite = spriteRenderer.sprite;
        if (spriteRenderer == null || sprite == null || sprite.texture == null) {
            return(null);
        }

        Rect spriteRect = sprite.textureRect;
        
        uvRect.x = spriteRect.x / sprite.texture.width;
        uvRect.y = spriteRect.y / sprite.texture.height;
        uvRect.width = spriteRect.width / sprite.texture.width;
        uvRect.height = spriteRect.height / sprite.texture.height;

        // Vertex Position Calculation
        scale.x = (float)sprite.texture.width / spriteRect.width;
        scale.y = (float)sprite.texture.height / spriteRect.height;

        size.x /= scale.x;
        size.y /= scale.y;

        size.x *= (float)sprite.texture.width / (sprite.pixelsPerUnit * 2);
        size.y *= (float)sprite.texture.height / (sprite.pixelsPerUnit * 2);

        mesh.vertices = new Vector3[]{new Vector3(-size.x, -size.y), new Vector3(size.x, -size.y), new Vector3(size.x, size.y), new Vector3(-size.x, size.y)};
        mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
    
        Vector2[] meshUV = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

        meshUV[0].x = uvRect.x + uvRect.width * uv0.x;
        meshUV[0].y = uvRect.y + uvRect.height * uv0.y;

        meshUV[1].x = uvRect.x + uvRect.width * uv1.x;
        meshUV[1].y = uvRect.y + uvRect.height * uv0.y;;

        meshUV[2].x = uvRect.x + uvRect.width * uv1.x;
        meshUV[2].y = uvRect.y + uvRect.height * uv1.y;

        meshUV[3].x = uvRect.x + uvRect.width * uv0.x;;
        meshUV[3].y = uvRect.y + uvRect.height * uv1.y;

        mesh.uv = meshUV;

        return(mesh);
    }
}