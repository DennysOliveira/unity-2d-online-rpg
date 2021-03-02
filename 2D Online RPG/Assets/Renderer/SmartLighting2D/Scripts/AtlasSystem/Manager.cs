using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace AtlasSystem {

    public class Manager {
        public static Dictionaries dictionaries = new Dictionaries();

        private static Texture atlasPage = null;
        public static Texture GetAtlasPage() {
            if (atlasPage == null) {
                atlasPage = new Texture();
            }
            return(atlasPage);
        }

        public static void Initialize() {
            atlasPage = new Texture();

            dictionaries.Clear();

            Request.requestList.Clear();

            Texture atlasTexture = GetAtlasPage();
            atlasTexture.Initialize();
        }

        static public void Update() {
            Request.Update();
        }

        static public Sprite RequestAccess(Sprite originalSprite, Request.Type type) {
            Sprite spriteObject = null;

            Dictionary<Sprite, Sprite> dictionary = dictionaries.Get(type);

            bool exist = dictionary.TryGetValue(originalSprite, out spriteObject);

            if (exist) {
                if (spriteObject == null || spriteObject.texture == null) {
                    dictionary.Remove(originalSprite);

                    spriteObject = AddSprite(originalSprite, type);

                    dictionary.Add(originalSprite, spriteObject);
                } 
                return(spriteObject);
            } else {		
                spriteObject = AddSprite(originalSprite, type);

                dictionary.Add(originalSprite, spriteObject);

                return(spriteObject);
            }
        }

        static public Sprite RequestSprite(Sprite originalSprite, Request.Type type) {
            if (originalSprite == null) {
                return(null);
            }
            
            Sprite spriteObject = null;
            Dictionary<Sprite, Sprite> dictionary = dictionaries.Get(type);

            bool exist = dictionary.TryGetValue(originalSprite, out spriteObject);

            if (exist) {
                if (spriteObject == null || spriteObject.texture == null) {
                    Request.requestList.Add(new Request(originalSprite, type));
                    return(null);
                } 
                return(spriteObject);
            } else {
                Request.requestList.Add(new Request(originalSprite, type));
                return(null);
            }
        }

        static private Sprite AddSprite(Sprite sprite, Request.Type type) {
            if (sprite == null || sprite.texture == null) {
                return(null);
            }

            return(GenerateSprite(sprite, type));
        }

        static public Texture2D GetTextureFromSprite(Sprite sprite) {
            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;

            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(sprite.texture, tmp);

            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;

            // Create a new readable Texture2D to copy the pixels to it
        // Texture2D myTexture2D = new Texture2D(sprite.texture.width, sprite.texture.height);
            Texture2D myTexture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

            Rect tempRect = sprite.rect;
            tempRect.y = sprite.texture.height - tempRect.y - sprite.rect.height;

            myTexture2D.ReadPixels(tempRect, 0, 0);

            // Copy the pixels from the RenderTexture to the new Texture
            //myTexture2D.ReadPixels(sprite.rect, 0, 0);
            myTexture2D.Apply();

            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            // RenderTexture.active = null;
            RenderTexture.active = previous;
            
            return(myTexture2D);
        }

        static public Sprite GenerateSprite(Sprite sprite, Request.Type type) {
            Texture atlasTexture = GetAtlasPage();
            Texture2D texture = atlasTexture.GetTexture();

            if (texture == null) {
                return(null);
            }

            if (atlasTexture.currentX + sprite.rect.width >= atlasTexture.atlasSize) {
                atlasTexture.currentX = 1;
                atlasTexture.currentY += atlasTexture.currentHeight;
                atlasTexture.currentHeight = 0;
            }

            if (atlasTexture.currentY + sprite.rect.height >= atlasTexture.atlasSize) {
                Debug.Log("Error: Lighting Atlas Overhead (" + atlasTexture.atlasSize + ") (" + sprite + ")");
                Lighting2D.disable = true;
                return(null);
            }

            Texture2D myTexture2D = GetTextureFromSprite(sprite);

            Color color;

            switch(type) {
                case Request.Type.Normal:
                    for(int x = 0; x < myTexture2D.width; x++) {
                        for(int y = 0; y < myTexture2D.height; y++) {
                            color = myTexture2D.GetPixel(x, y);
                            
                            color.a = 1;

                            texture.SetPixel(atlasTexture.currentX + x, atlasTexture.currentY + y, color);
                        }
                    }
                    break;

                case Request.Type.WhiteMask:
                    for(int x = 0; x < myTexture2D.width; x++) {
                        for(int y = 0; y < myTexture2D.height; y++) {
                            color = myTexture2D.GetPixel(x, y);

                            color.r = 1;
                            color.g = 1;
                            color.b = 1;

                            texture.SetPixel(atlasTexture.currentX + x, atlasTexture.currentY + y, color);
                        }
                    }
                    break;
                    
                case Request.Type.BlackMask:
                    for(int x = 0; x < myTexture2D.width; x++) {
                        for(int y = 0; y < myTexture2D.height; y++) {
                            color = myTexture2D.GetPixel(x, y);
                        
                            color.a = ((1 - color.r) + (1 - color.g) + (1 - color.b)) / 3;
                            color.r = 0;
                            color.g = 0;
                            color.b = 0;

                            texture.SetPixel(atlasTexture.currentX + x, atlasTexture.currentY + y, color);
                        }
                    }
                    break;
            }
            
            texture.Apply();

            atlasTexture.spriteCount ++;

            Vector2 pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
            
            Sprite output = Sprite.Create(texture, new Rect(atlasTexture.currentX, atlasTexture.currentY, myTexture2D.width, myTexture2D.height), pivot, sprite.pixelsPerUnit);

            atlasTexture.currentX += (int)sprite.rect.width;
            atlasTexture.currentHeight = Mathf.Max(atlasTexture.currentHeight, (int)sprite.rect.height);

            return(output);
        }
    }

}