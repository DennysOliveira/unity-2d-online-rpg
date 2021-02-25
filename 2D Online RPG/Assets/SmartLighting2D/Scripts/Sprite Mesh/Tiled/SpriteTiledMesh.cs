using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTiledMesh {
    private MeshBrush brush;
    private SpriteMesh spriteMesh;

    private Sprite cacheSprite;
    private Vector2 cacheSize;
    private Mesh cacheMesh;
    private MeshObject cacheMeshObject;

    public SpriteTiledMesh() {
        brush = new MeshBrush();
        spriteMesh = new SpriteMesh();
    }
    
    public MeshObject GetMesh(SpriteRenderer spriteRenderer) {
        if (cacheSize.Equals(spriteRenderer.size) == false || cacheSprite.Equals(spriteRenderer.sprite) == false) {
            cacheMesh = Generate(spriteRenderer);
            cacheMeshObject = MeshObject.Get(cacheMesh);

            cacheSize = spriteRenderer.size;
            cacheSprite = spriteRenderer.sprite;
        }

        return(cacheMeshObject);
    }

    Mesh Generate(SpriteRenderer spriteRenderer) {
        brush.Clear();

        Rect spriteRect = spriteRenderer.sprite.textureRect;

        float spriteRatioX = (float)spriteRect.width / spriteRenderer.sprite.texture.width;
        float spriteRatioY = (float)spriteRect.height / spriteRenderer.sprite.texture.height;

        float stretchX = ((float)spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit);
        float stretchY = ((float)spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit);

        float scaleX, scaleY;

        float sizeX = Mathf.Abs(spriteRenderer.size.x) / spriteRatioX;
        float sizeY = Mathf.Abs(spriteRenderer.size.y) / spriteRatioY;

        float borderX0 = 0;
        float borderX1 = spriteRenderer.sprite.border.z / spriteRect.width;

        float borderY0;
        float borderY1 = spriteRenderer.sprite.border.w / spriteRect.height;

        float fullX = 1f - borderX1;
        float fullY;

        float sizeLeftX = sizeX / stretchX;
        float offset_x = 0;

        float sizeLeftY;
        float offset_y;

        while(sizeLeftX > 0) {
            scaleX = sizeLeftX > fullX ? scaleX = fullX : scaleX = sizeLeftX;

            if (sizeLeftX > fullX) {
                sizeLeftX -= fullX;

                float sizeOffsetX = offset_x - (sizeLeftX / 2 * stretchX * spriteRatioX);

                sizeLeftY = sizeY / stretchY;
                offset_y = 0;

                borderY0 = 0;
                fullY = 1f - borderY1;

                while(sizeLeftY > 0) {
                    scaleY = sizeLeftY > fullY ? scaleY = fullY : scaleY = sizeLeftY;

                    if (sizeLeftY > fullY) {
                        sizeLeftY -= fullY;

                        float sizeOffsetY = offset_y - (sizeLeftY / 2 * stretchY * spriteRatioY);

                        brush.AddMesh(spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),  new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)), new Vector3(sizeOffsetX, sizeOffsetY,0));

                    } else {
                        brush.AddMesh(spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),  new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)), new Vector3(sizeOffsetX, offset_y, 0));

                        sizeLeftY -= fullY;
                    }

                    offset_y += (fullY / 2) * stretchY * spriteRatioY;
                    
                    borderY0 = spriteRenderer.sprite.border.y / spriteRect.height;
                    fullY = 1f - borderY1 - borderY0;
                }

            } else {

                sizeLeftY = sizeY / stretchY;
                offset_y = 0;

                borderY0 = 0;
                fullY = 1f - borderY1;

                while(sizeLeftY > 0) {
                    scaleY = sizeLeftY > fullY ? scaleY = fullY : scaleY = sizeLeftY;

                    if (sizeLeftY > fullY) {
                        sizeLeftY -= fullY;

                        float sizeOffsetY = offset_y - (sizeLeftY / 2 * stretchY * spriteRatioY);

                        brush.AddMesh(spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),  new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)), new Vector3(offset_x, sizeOffsetY, 0));

                    } else {
                        brush.AddMesh(spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),  new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)), new Vector3(offset_x, offset_y, 0));

                        sizeLeftY -= fullY;
                    }

                    offset_y += (fullY / 2) * stretchY * spriteRatioY;

                    
                    borderY0 = spriteRenderer.sprite.border.y / spriteRect.height;
                    fullY = 1f - borderY1 - borderY0;
                }

                sizeLeftX -= fullX;
            }

            offset_x += (fullX / 2) * stretchX * spriteRatioX;

            borderX0 = spriteRenderer.sprite.border.x / spriteRect.width;
            fullX = 1f - borderX1 - borderX0;
        }

        return(brush.Export());
    }
}