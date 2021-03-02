using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace AtlasSystem {
    
    public class Texture {
        private Texture2D texture;
        public int currentX = 1;
        public int currentY = 1;
        public int currentHeight = 0;

        public int atlasSize = 1024;

        public int spriteCount = 0;

        public void Initialize() {
            Color32 resetColor = new Color32(0, 0, 0, 0);
            Color32[] resetColorArray = texture.GetPixels32();
            for (int i = 0; i < resetColorArray.Length; i++) {
                resetColorArray[i] = resetColor;
            }
            texture.SetPixels32(resetColorArray);
        }

        public Texture() {
            GetTexture();
        }

        public Texture2D GetTexture() {
            if (texture == null) {
                /*
                if (Lighting2D.atlasSettings.lightingSpriteAtlas) {
                    switch(Lighting2D.atlasSettings.spriteAtlasSize) {
                        case SpriteAtlasSize.px2048:
                            atlasSize = 2048;
                            break;
                        case SpriteAtlasSize.px1024:
                            atlasSize = 1024;
                            break;
                        case SpriteAtlasSize.px512:
                            atlasSize = 512;
                            break;
                        case SpriteAtlasSize.px256:
                            atlasSize = 256;
                            break;
                    }
                } else {
                    atlasSize = 256;
                }*/

                atlasSize = 1024;

                texture = new Texture2D(atlasSize, atlasSize);
            }
            return(texture);
        }
    }
}