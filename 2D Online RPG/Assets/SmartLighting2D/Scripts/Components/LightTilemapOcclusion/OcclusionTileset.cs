using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public Sprite[] tiles = new Sprite[100];
//tiles[id] = Sprite.Create(tilemapTexture, new Rect(x * 32, tilemapTexture.height - y * 32 - 32, 32, 32), new Vector2(0.5f, 0.5f));

public class OcclusionTileset {
    public TileUV[] uv = new TileUV[100];
    public Texture2D texture = null;
    public Sprite[] sprites = new Sprite[30];

    public class TileUV {
        public Vector2 uv0 = Vector2.zero;
        public Vector2 uv1 = Vector2.zero;
        
        
        
        public Vector2 uv2 = Vector2.zero;
        public Vector2 uv3 = Vector2.zero;
    }

    public enum TileRotation {
        up, 
        right, 
        down, 
        left
    };

    public static OcclusionTileset Load(string path) {
        OcclusionTileset tilemap = new OcclusionTileset();
        tilemap.LoadSelf(path);
        return(tilemap);
    }

    void LoadSelf(string path) {
        texture = Resources.Load<Texture2D>(path);

        int tileSize = 64;

        int sizeX = texture.width / tileSize;
        int sizeY = texture.height / tileSize;

        int tilemapSize = sizeX * sizeY;

        int id = 0;

        float precisionX = 1f / texture.width;
        float precisionY = 1f / texture.height;
            
        for(int y = 0; y < sizeY; y ++) {
            for(int x = 0; x < sizeX; x ++) {
                uv[id] = new TileUV();

                uv[id].uv0.x = x * tileSize;
                uv[id].uv0.y = texture.height - y * tileSize - tileSize;

                uv[id].uv1.x = uv[id].uv0.x + tileSize;
                uv[id].uv1.y = uv[id].uv0.y;

                uv[id].uv2.x = uv[id].uv0.x + tileSize;
                uv[id].uv2.y = uv[id].uv0.y + tileSize;

                uv[id].uv3.x = uv[id].uv0.x;
                uv[id].uv3.y = uv[id].uv0.y + tileSize;

                sprites[id] = Sprite.Create(texture, new Rect(uv[id].uv0.x, uv[id].uv0.y, tileSize, tileSize), new Vector2(0.5f, 0.5f));
            

                uv[id].uv0.x /= texture.width;
                uv[id].uv0.y /= texture.height; 

                uv[id].uv1.x /= texture.width;
                uv[id].uv1.y /= texture.height; 

                uv[id].uv2.x /= texture.width;
                uv[id].uv2.y /= texture.height; 

                uv[id].uv3.x /= texture.width;
                uv[id].uv3.y /= texture.height; 

                uv[id].uv0.x += precisionX;
                uv[id].uv0.y += precisionY; 
    
                uv[id].uv1.x -= precisionX;
                uv[id].uv1.y += precisionY; 

                uv[id].uv2.x -= precisionX;
                uv[id].uv2.y -= precisionY; 

                uv[id].uv3.x += precisionX;
                uv[id].uv3.y -= precisionY; 

              
                id += 1;
            }  
        }
    }
}