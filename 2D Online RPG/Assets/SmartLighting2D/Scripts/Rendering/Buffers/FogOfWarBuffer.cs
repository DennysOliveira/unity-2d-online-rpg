using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering {

	public class FogOfWarBuffer {
		public class Check {
			static public void RenderTexture(FogOfWarBuffer2D buffer) {
                Vector2Int screen = buffer.GetScreen();

                if (screen.x > 0 && screen.y > 0) {
                    Camera camera = buffer.fogOfWarCamera.GetCamera();

                    if (buffer.renderTexture == null || screen.x != buffer.renderTexture.width || screen.y != buffer.renderTexture.height) {

                        switch(camera.cameraType) {
                            case CameraType.Game:
                                buffer.SetUpRenderTexture();
                            
                            break;

                            case CameraType.SceneView:
                                // Scene view pixel rect is constantly changing (Unity Bug?)
                                int differenceX = Mathf.Abs(screen.x - buffer.renderTexture.width);
                                int differenceY = Mathf.Abs(screen.y - buffer.renderTexture.height);
                                
                                if (differenceX > 5 || differenceY > 5) {
                                    buffer.SetUpRenderTexture();
                                }
                            
                            break;

                        }
                    }
                }
            }
		}
		public static void LateUpdate(FogOfWarBuffer2D buffer) {
			if (buffer.CameraSettingsCheck() == false) {
				buffer.DestroySelf();
				return;
			}

			Camera camera = buffer.fogOfWarCamera.GetCamera();

			if (camera == null) {
				return;
			}

			LightingManager2D manager = LightingManager2D.Get();
    
        	if (manager.fogOfWarCameras.Length < 1) {
				buffer.DestroySelf();

				return;
			}
		}

		public static void DrawOn(FogOfWarBuffer2D buffer) {
			LightingManager2D manager = LightingManager2D.Get();
    
        	if (manager.fogOfWarCameras.Length < 1) {
				return;
			}

			switch(Lighting2D.RenderingMode) {
				case RenderingMode.OnRender:
					FogOfWarRender.OnRender(buffer);
	
				break;

				case RenderingMode.OnPreRender:
					FogOfWarRender.PreRender(buffer);
				break;
			}
		}

		static public void Render(FogOfWarBuffer2D buffer) {
			Camera camera = buffer.fogOfWarCamera.GetCamera();

			if (camera == null) {
				return;
			}

			bool draw = true;

			if (Lighting2D.FogOfWar.useOnlyInPlay) {
            	if (Application.isPlaying == false) {
					draw = false;
				}
			}

			float sizeY = camera.orthographicSize;
			float sizeX = sizeY * ( (float)camera.pixelWidth / camera.pixelHeight );

			GL.PushMatrix();
			GL.LoadPixelMatrix( -sizeX, sizeX, -sizeY, sizeY );

			if (draw) {
				if (Lighting2D.Profile.fogOfWar.sorting == FogOfWarSorting.None) {
					FogOfWar.NoSort.Draw(camera);
				} else {
					FogOfWar.Sorted.Draw(camera);
				}
			}

			GL.PopMatrix();
		}
	}
}