using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

namespace Rendering.Light {

    public class Pass {

        public Light2D light;
        public LayerSetting layer;
        public int layerID;

        public float lightSizeSquared;

        public List<LightCollider2D> colliderList;
        public List<LightCollider2D> layerShadowList;
        public List<LightCollider2D> layerMaskList;

        #if UNITY_2017_4_OR_NEWER
            public List<LightTilemapCollider2D> tilemapList;
        #endif

        public bool drawMask = false;
        public bool drawShadows = false;

        public Material materialWhite;
        public Material materialNormalMap_PixelToLight;
        public Material materialNormalMap_ObjectToLight;

        public Sorting.SortPass sortPass = new Sorting.SortPass();

        public bool Setup(Light2D light, LayerSetting setLayer) {
            // Layer ID
            layerID = setLayer.GetLayerID();
            if (layerID < 0) {
                return(false);
            }

            layer = setLayer;

            // Calculation Setup
            this.light = light;
            lightSizeSquared = Mathf.Sqrt(light.size * light.size + light.size * light.size);
        
            colliderList = LightCollider2D.List;

            layerShadowList = LightCollider2D.GetShadowList(layerID);
            layerMaskList = LightCollider2D.GetMaskList(layerID);

            #if UNITY_2017_4_OR_NEWER
                tilemapList = LightTilemapCollider2D.GetList();
            #endif

            // Draw Mask & Shadows?
            drawMask = (layer.type != LightLayerType.ShadowOnly);
            drawShadows = (layer.type != LightLayerType.MaskOnly);

            // Materials
            materialWhite = Lighting2D.materials.GetMask();

            if (materialWhite != null) {
                materialWhite.SetFloat("translucency", (light.maskTranslucency * 100) / light.size);

                materialWhite.SetFloat("intensity", light.maskTranslucencyIntensity);

                if (light.maskTranslucency > 0) {
                    if (light.Buffer.collisionTexture != null) {
                        materialWhite.SetTexture("_SecTex", light.Buffer.collisionTexture.renderTexture);

                        materialWhite.SetFloat("textureSize", light.GetTextureSize().x);
                    }
                }
            }
            
            materialNormalMap_PixelToLight = Lighting2D.materials.GetNormalMapSpritePixelToLight();

            if (materialNormalMap_PixelToLight != null) {
                materialNormalMap_PixelToLight.SetFloat("translucency", (light.maskTranslucency * 100) / light.size);

                materialNormalMap_PixelToLight.SetFloat("intensity", light.maskTranslucencyIntensity);

                if (light.maskTranslucency > 0) {
                    if (light.Buffer.collisionTexture != null) {
                        materialNormalMap_PixelToLight.SetTexture("_SecTex", light.Buffer.collisionTexture.renderTexture);

                        materialNormalMap_PixelToLight.SetFloat("textureSize", light.GetTextureSize().x);
                    }
                }
            }

            materialNormalMap_ObjectToLight = Lighting2D.materials.GetNormalMapSpriteObjectToLight();

            if (materialNormalMap_ObjectToLight != null) {
                materialNormalMap_ObjectToLight.SetFloat("translucency", (light.maskTranslucency * 100) / light.size);

                if (light.maskTranslucency > 0) {
                    if (light.Buffer.collisionTexture != null) {
                        materialNormalMap_ObjectToLight.SetTexture("_SecTex", light.Buffer.collisionTexture.renderTexture);

                        materialNormalMap_ObjectToLight.SetFloat("textureSize", light.GetTextureSize().x);
                    }
                }
            }


            materialNormalMap_PixelToLight.SetFloat("_LightSize", light.size);
            materialNormalMap_PixelToLight.SetFloat("_LightIntensity", light.bumpMap.intensity);
            materialNormalMap_PixelToLight.SetFloat("_LightZ", light.bumpMap.depth);

            materialNormalMap_ObjectToLight.SetFloat("_LightSize", light.size);
            materialNormalMap_ObjectToLight.SetFloat("_LightIntensity", light.bumpMap.intensity);
            materialNormalMap_ObjectToLight.SetFloat("_LightZ", light.bumpMap.depth);

            sortPass.pass = this;
            
            // Sort
            sortPass.Clear();

            return(true);
        }
    }
}