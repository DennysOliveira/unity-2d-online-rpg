using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;
using LightSettings;

namespace Rendering.Day {

    public class NoSort {

        static public void Draw(Pass pass) {
            bool drawShadows = pass.layer.type != LayerType.MaskOnly;
            bool drawMask = pass.layer.type != LayerType.ShadowsOnly;

            if (drawShadows) {
                Lighting2D.materials.GetShadowBlur().SetPass(0);
                
                GL.Begin(GL.TRIANGLES);

                DrawColliderShadows(pass);
                DrawTilemapColliderShadows(pass);
                
                GL.End();

                GL.Color(Color.white);

                DrawColliderSpriteShadows(pass);
            }

            
            if (drawMask) {
                GL.Color(Color.white);

                DrawColliderMasks(pass);

                DrawTilemapColliderMasks(pass);
            }
        }

        private static void DrawColliderMasks(Pass pass) {
            for(int i = 0; i < pass.colliderCount; i++) {
                DayLightCollider2D id = pass.colliderList[i];

                if (id.maskLayer != pass.layerId) {
                    continue;
                }

                SpriteRenderer2D.Draw(id, pass.offset);
            }
        }

        private static void DrawTilemapColliderMasks(Pass pass) {
            for(int i = 0; i < pass.tilemapColliderCount; i++) {
                DayLightTilemapCollider2D id = pass.tilemapColliderList[i];

                if (id.maskLayer != pass.layerId) {
                    continue;
                }

                SpriteRenderer2D.DrawTilemap(id, pass.offset);
            }
        }

        private static void DrawColliderShadows(Pass pass) {
            for(int i = 0; i < pass.colliderCount; i++) {
                DayLightCollider2D id = pass.colliderList[i];
                
                if (id.shadowLayer != pass.layerId) {
                    continue;
                }

                Shadow.Draw(id, pass.offset);                
            }
        }


        private static void DrawTilemapColliderShadows(Pass pass) {
            for(int i = 0; i < pass.tilemapColliderCount; i++) {
                DayLightTilemapCollider2D id = pass.tilemapColliderList[i];
                
                if (id.shadowLayer != pass.layerId) {
                    continue;
                }

                Shadow.DrawTilemap(id, pass.offset);                
            }
        }

        private static void DrawColliderSpriteShadows(Pass pass) {
            for(int i = 0; i < pass.colliderCount; i++) {
                DayLightCollider2D id = pass.colliderList[i];

                if (id.shadowLayer != pass.layerId) {
                    continue;
                }
            
                SpriteRendererShadow.Draw(id, pass.offset);
            }
        }
    }
}
