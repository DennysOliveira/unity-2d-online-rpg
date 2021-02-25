using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rendering.Light.Shadow {

    public static class Legacy {

        public static Pair2 pair = Pair2.Zero();
        public static Vector2 projectedMiddle, projectedLeft, projectedRight, outerLeft, outerRight;
        public static Vector2 edgeAWorld, edgeBWorld, edgeALocal, edgeBLocal;
        public static Vector2 closestPoint;
 
        public static void Draw(List<Polygon2> polygons, float distance, float translucency) {
            if (polygons == null) {
                return;
            }

            Light2D light = ShadowEngine.light;
            Vector2 position = ShadowEngine.lightOffset + ShadowEngine.objectOffset;

            float shadowDistance = ShadowEngine.lightSize;
            float z = ShadowEngine.shadowZ;

            float outerAngle = light.outerAngle;
            bool drawInside = (ShadowEngine.lightDrawAbove == false);
            bool culling = true;

            UVRect penumbraRect = ShadowEngine.Penumbra.uvRect;
            UVRect fillRect = ShadowEngine.FillBlack.uvRect;
			
            float angleA, angleB;
            float rotA, rotB, rotM;

            Vector2 middle = Vector2.zero;

            int PolygonCount = polygons.Count;

            Vector2 draw = ShadowEngine.drawOffset;

            if (translucency > 0) {
                fillRect = ShadowEngine.FillWhite.uvRect;
                GL.Color(new Color(0, 0, 0, 1 - translucency));
                outerAngle = 0;
            }

            if (distance > 0) {
                shadowDistance = distance;
                outerAngle = 0;
                culling = false;
            }

            for(int i = 0; i < PolygonCount; i++) {
  
                Vector2[] pointsList = polygons[i].points;
                int pointsCount = pointsList.Length;
            
                for(int x = 0; x < pointsCount; x++) {
                    int next = (x + 1) % pointsCount;
                    
                    pair.A = pointsList[x];
                    pair.B = pointsList[next];

                    edgeALocal.x = pair.A.x;
                    edgeALocal.y = pair.A.y;

                    edgeBLocal.x = pair.B.x;
                    edgeBLocal.y = pair.B.y;

                    edgeAWorld.x = edgeALocal.x + position.x;
                    edgeAWorld.y = edgeALocal.y + position.y;

                    edgeBWorld.x = edgeBLocal.x + position.x;
                    edgeBWorld.y = edgeBLocal.y + position.y;

                    closestPoint = Math2D.ClosestPointOnLine(middle, edgeAWorld, edgeBWorld);
                    if (Vector2.Distance(middle, closestPoint) > light.size) {
                        continue;
                    }

                    float lightDirection = (float)Math.Atan2((edgeAWorld.y + edgeBWorld.y) / 2 , (edgeAWorld.x + edgeBWorld.x) / 2 ) * Mathf.Rad2Deg;
                    float EdgeDirection = (float)Math.Atan2(edgeALocal.y - edgeBLocal.y, edgeALocal.x - edgeBLocal.x) * Mathf.Rad2Deg - 180;

                    lightDirection -= EdgeDirection;
                    lightDirection = (lightDirection + 720) % 360;
                    
                    if (culling && drawInside == false) {
                        if (lightDirection < 180) {
                            continue;
                        }
                    }

                    angleA = (float)System.Math.Atan2 (edgeAWorld.y, edgeAWorld.x);
                    angleB = (float)System.Math.Atan2 (edgeBWorld.y, edgeBWorld.x);

                    projectedRight.x = edgeAWorld.x + Mathf.Cos(angleA) * shadowDistance;
                    projectedRight.y = edgeAWorld.y + Mathf.Sin(angleA) * shadowDistance;

                    projectedLeft.x = edgeBWorld.x + Mathf.Cos(angleB) * shadowDistance;
                    projectedLeft.y = edgeBWorld.y + Mathf.Sin(angleB) * shadowDistance;

                    if (outerAngle > 0) {
                        rotA = angleA - Mathf.Deg2Rad * light.outerAngle;
                        rotB = angleB + Mathf.Deg2Rad * light.outerAngle;

                        outerRight.x = edgeAWorld.x + Mathf.Cos(rotA) * shadowDistance;
                        outerRight.y = edgeAWorld.y + Mathf.Sin(rotA) * shadowDistance;
                        
                        outerLeft.x = edgeBWorld.x + Mathf.Cos(rotB) * shadowDistance;
                        outerLeft.y = edgeBWorld.y + Mathf.Sin(rotB) * shadowDistance;

                        // Right Penumbra
                        GL.TexCoord3(penumbraRect.x0, penumbraRect.y0, 0);
                        GL.Vertex3(draw.x + edgeAWorld.x, draw.y + edgeAWorld.y, z);

                        GL.TexCoord3(penumbraRect.x1, penumbraRect.y0, 0);
                        GL.Vertex3(draw.x + outerRight.x, draw.y + outerRight.y, z);
                        
                        GL.TexCoord3(penumbraRect.x0, penumbraRect.y1, 0);
                        GL.Vertex3(draw.x + projectedRight.x, draw.y + projectedRight.y, z);
                        
                        // Left Penumbra
                        GL.TexCoord3(penumbraRect.x0, penumbraRect.y0, 0);
                        GL.Vertex3(draw.x + edgeBWorld.x, draw.y + edgeBWorld.y, z);

                        GL.TexCoord3(penumbraRect.x1, penumbraRect.y0, 0);
                        GL.Vertex3(draw.x + outerLeft.x, draw.y + outerLeft.y, z);
                        
                        GL.TexCoord3(penumbraRect.x0, penumbraRect.y1, 0);
                        GL.Vertex3(draw.x + projectedLeft.x, draw.y + projectedLeft.y, z);
                    }

                    // Right Fin
                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedLeft.x, draw.y + projectedLeft.y, z);

                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedRight.x, draw.y + projectedRight.y, z);

                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + edgeAWorld.x, draw.y + edgeAWorld.y, z);
                    
                    // Left Fin
                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + edgeAWorld.x, draw.y + edgeAWorld.y, z);

                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + edgeBWorld.x, draw.y + edgeBWorld.y, z);
                    
                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedLeft.x, draw.y + projectedLeft.y, z);

                    closestPoint = Math2D.ClosestPointOnLine(middle, projectedLeft, projectedRight);
                    rotM = (float)System.Math.Atan2 (closestPoint.y, closestPoint.x);

                    // Detailed Shadow
                    projectedMiddle.x = (edgeAWorld.x + edgeBWorld.x) / 2 + Mathf.Cos(rotM) * shadowDistance;
                    projectedMiddle.y = (edgeAWorld.y + edgeBWorld.y) / 2 + Mathf.Sin(rotM) * shadowDistance;                        
                                
                    // Middle Fin
                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedLeft.x, draw.y + projectedLeft.y, z);

                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedRight.x, draw.y + projectedRight.y, z);

                    GL.TexCoord3(fillRect.x0, fillRect.y0, 0);
                    GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, z);   
            
                }
            }

            if (translucency > 0) {
                GL.Color(Color.white);
            }
        }
    }
}