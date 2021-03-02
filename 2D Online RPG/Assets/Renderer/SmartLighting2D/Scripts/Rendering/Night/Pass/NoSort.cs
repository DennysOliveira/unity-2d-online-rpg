using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night {

    public class NoSort {

        public static void Draw(Pass pass) {
            // Rooms
            DrawRooms(pass);
            
            DrawTilemapRooms(pass);

            // Light Emissions
            DrawLightSprites(pass);

            DrawLightSprites_scriptable(pass);

            DrawLightTextures(pass);

            DrawLightParticleSystem(pass);

            // Light Sources

            DrawLight(pass);

            DrawLightMesh(pass);
        }

        private static void DrawRooms(Pass pass) {
            List<LightRoom2D> roomList = LightRoom2D.List;
            int roomCount = roomList.Count;

            if (roomCount < 1) {
                return;
            }

            for(int i = 0; i < roomCount; i++) {
                LightRoom2D id = roomList[i];
   
                if (id.nightLayer != pass.layerId) {
                    continue;
                }

                Room.Draw(id, pass.camera);
            }
        }

        private static void DrawTilemapRooms(Pass pass) {
            #if UNITY_2017_4_OR_NEWER
                List<LightTilemapRoom2D > roomTilemapList = LightTilemapRoom2D.List;
                int roomTilemapCount = roomTilemapList.Count;

                if (roomTilemapCount < 1) {
                    return;
                }

                for(int i = 0; i < roomTilemapCount; i++) {
                    LightTilemapRoom2D id = roomTilemapList[i];
                    
                    if (id.nightLayer != pass.layerId) {
                        continue;
                    }
                    
                    TilemapRoom.Draw(id, pass.camera);
                }
            #endif
        }

        private static void DrawLightSprites(Pass pass) {
            List<LightSprite2D> spriteRendererList = LightSprite2D.List;
            int spriteRendererCount = spriteRendererList.Count;

            if (spriteRendererCount < 1) {
                return;
            }

            for(int i = 0; i < spriteRendererCount; i++) {
                LightSprite2D id = spriteRendererList[i];

                if (id.nightLayer != pass.layerId) {
                    continue;
                }

                LightSprite.Draw(id, pass.camera);
            }
        }

        private static void DrawLightSprites_scriptable(Pass pass) {
            List<Scriptable.LightSprite2D> spriteRendererList = Scriptable.LightSprite2D.List;
            int spriteRendererCount = spriteRendererList.Count;

            if (spriteRendererCount < 1) {
                return;
            }

            for(int i = 0; i < spriteRendererCount; i++) {
                Scriptable.LightSprite2D id = spriteRendererList[i];

                if (id.NightLayer != pass.layerId) {
                    continue;
                }

                LightSprite.Draw_scriptable(id, pass.camera);
            }
        }






        private static void DrawLightTextures(Pass pass) {
            List<LightTexture2D> lightTextureList = LightTexture2D.List;
            int lightTextureCount = lightTextureList.Count;

            if (lightTextureCount < 1) {
                return;
            }

			for(int i = 0; i < lightTextureCount; i++) {
				LightTexture2D id = lightTextureList[i];

				if (id.nightLayer != pass.layerId) {
					continue;
				}

				TextureRenderer.Draw(id, pass.camera);
			}
        }

        private static void DrawLightParticleSystem(Pass pass) {
            List<LightParticleSystem2D> particleRendererList = LightParticleSystem2D.List;
            int lightParticleSystemCount = particleRendererList.Count;

            if (lightParticleSystemCount < 1) {
                return;
            }

			for(int i = 0; i < lightParticleSystemCount; i++) {
				LightParticleSystem2D id = particleRendererList[i];

				if (id.nightLayer != pass.layerId) {
					continue;
				}

				ParticleRenderer.Draw(id, pass.camera);
			}
        }

        private static void DrawLight(Pass pass) {
            List<Light2D> lightList = Light2D.List;
            int lightCount = lightList.Count;

            if (lightCount < 1) {
                return;
            }

            for(int i = 0; i < lightCount; i++) {
                Light2D id = lightList[i];
			
                if (id.nightLayer != pass.layerId) {
                    continue;
                }

                Rendering.Night.LightSource.Draw(id, pass.camera);
            }
        }

        private static void DrawLightMesh(Pass pass) {
            List<LightMesh2D> lightMeshList = LightMesh2D.List;
            int lightMeshCount = lightMeshList.Count;

            if (lightMeshCount < 1) {
                return;
            }

            for(int i = 0; i < lightMeshCount; i++) {
                LightMesh2D id = lightMeshList[i];

                if (id.nightLayer != pass.layerId) {
                    continue;
                }

               Rendering.Night.LightMesh.Draw(id, pass.camera);
            }
        }
    }
}