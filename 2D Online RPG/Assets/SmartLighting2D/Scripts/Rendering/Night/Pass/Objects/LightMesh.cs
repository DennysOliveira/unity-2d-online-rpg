using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

namespace Rendering.Night {
	
	public class LightMesh {

       static public void Draw(LightMesh2D id, Camera camera) {
            if (id.isActiveAndEnabled == false) {
                return;
            }

            if (id.InAnyCamera() == false) {
                return;
            }

            Vector2 pos = id.transform.position;
           

            Vector2 size = new Vector2(id.size, id.size);

            Color lightColor = id.color;
            lightColor.a = id.color.a / 2;

            Material material = Lighting2D.materials.GetAdditive();
            if (id.sprite != null) {
                material.mainTexture = id.sprite.texture;
            } else {
                material.mainTexture = null;
            }
            
            material.SetColor ("_TintColor", lightColor);

            material.SetPass (0); 

            GL.Begin (GL.TRIANGLES);

            float uvScale = (1f / id.size) / 2;
            Vector2 uvC = new Vector2(0.5f, 0.5f);
  
            int pointsCount = id.geometry.optimizedPointsCount;

            Vector2 offset = -camera.transform.position;

            for(int i = 0; i < pointsCount; i++) {
                Vector2 pointA = id.geometry.optimizedPoints[(i) % pointsCount] - pos;
                Vector2 pointB = id.geometry.optimizedPoints[(i + 1) % pointsCount] - pos;

                Vector2 uvA = pointA * uvScale + new Vector2(0.5f, 0.5f);
                Vector2 uvB = pointB * uvScale + new Vector2(0.5f, 0.5f);

                pointA += pos + offset;
                pointB += pos + offset;

               // float c = ((float)i) / pointsCount * 41;
                //GL.Color(new Color(1,0,i,0.5f));

                //GL.Color(Color.red);

                if (id.useUV) {
                    GL.TexCoord3 (uvA.x, uvA.y, 0);
                    GL.Vertex3 (pointA.x, pointA.y, 0);

                    GL.TexCoord3 (uvB.x, uvB.y, 0);
                    GL.Vertex3 (pointB.x, pointB.y, 0);

                    GL.TexCoord3 (uvC.x, uvC.y, 0);
                    GL.Vertex3 (pos.x  + offset.x, pos.y + offset.y, 0);
                    
                } else {
                    GL.TexCoord3 (0.5f, 0.5f, 0);
                    GL.Vertex3 (pointA.x, pointA.y, 0);

                    GL.TexCoord3 (0.5f, 0.5f, 0);
                    GL.Vertex3 (pointB.x, pointB.y, 0);

                    GL.TexCoord3 (0.5f, 0.5f, 0);
                    GL.Vertex3 (pos.x  + offset.x, pos.y + offset.y, 0);
                }
            }

            GL.End ();

        }
    }
}