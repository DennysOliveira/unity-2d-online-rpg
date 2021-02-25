using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public static class SpriteProjection {
        
        public static Pair2 pair = Pair2.Zero();

        public static void Draw(List<Polygon2> polygons, float shadowDistance) {
            if (polygons == null) {
                return;
            }

            Vector2[] uv = new Vector2[4];
            Vector2[] vertices = new Vector2[4];

            Material mat = Lighting2D.materials.GetSpriteProjectionMaterial();

            if (ShadowEngine.spriteProjection == null) {
                return;
            }

            mat.mainTexture = ShadowEngine.spriteProjection.texture;

            mat.SetPass(0);

            GL.Color(Color.white);

      
            GL.Begin(GL.QUADS);


            UVRect fill = ShadowEngine.FillBlack.uvRect;

            Light2D light = ShadowEngine.light;
            Vector2 offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            float lightSize = ShadowEngine.lightSize * 0.15f;


            if (shadowDistance == 0) {
                shadowDistance = lightSize;
            }

            float outerAngle = light.outerAngle;

            
            Vector2 vA, vB, vC, vD;
            float angleA, angleB, rotA, rotB;

            int PolygonCount = polygons.Count;

            for(int i = 0; i < PolygonCount; i++) {
                Vector2[] pointsList = polygons[i].points;
                int pointsCount = pointsList.Length;

                SoftShadowSorter.Set(polygons[i], light);


                pair.A.x = SoftShadowSorter.minPoint.x;
                pair.A.y = SoftShadowSorter.minPoint.y;

                pair.B.x = SoftShadowSorter.maxPoint.x;
                pair.B.y = SoftShadowSorter.maxPoint.y;

                float edgeALocalX = pair.A.x;
                float edgeALocalY = pair.A.y;

                float edgeBLocalX = pair.B.x;
                float edgeBLocalY = pair.B.y;

                float edgeAWorldX = edgeALocalX + offset.x;
                float edgeAWorldY = edgeALocalY + offset.y;

                float edgeBWorldX = edgeBLocalX + offset.x;
                float edgeBWorldY = edgeBLocalY + offset.y;


                float lightDirection = Mathf.Atan2((edgeAWorldY + edgeBWorldY) / 2 , (edgeAWorldX + edgeBWorldX) / 2 ) * Mathf.Rad2Deg;
                float EdgeDirection = (Mathf.Atan2(edgeALocalY - edgeBLocalY, edgeALocalX - edgeBLocalX) * Mathf.Rad2Deg - 180 + 720) % 360;

                lightDirection -= EdgeDirection;
                lightDirection = (lightDirection + 720) % 360;

                if (lightDirection > 180) {
                    // continue;
                }
        
                for(float s = 0; s <= 1; s += 0.1f) {
                    float step0 = s;
                    float step1 = s + 0.1f;

                    float dir = SoftShadowSorter.minPoint.Atan2(SoftShadowSorter.maxPoint) - Mathf.PI;
                    float distance = Vector2.Distance(SoftShadowSorter.minPoint, SoftShadowSorter.maxPoint);

                    pair.A.x = SoftShadowSorter.minPoint.x + Mathf.Cos(dir) * distance * step0;
                    pair.A.y = SoftShadowSorter.minPoint.y + Mathf.Sin(dir) * distance * step0;

                    pair.B.x = SoftShadowSorter.minPoint.x + Mathf.Cos(dir) * distance * step1;
                    pair.B.y = SoftShadowSorter.minPoint.y + Mathf.Sin(dir) * distance * step1;

                    edgeALocalX = pair.A.x;
                    edgeALocalY = pair.A.y;

                    edgeBLocalX = pair.B.x;
                    edgeBLocalY = pair.B.y;

                    edgeAWorldX = edgeALocalX + offset.x;
                    edgeAWorldY = edgeALocalY + offset.y;

                    edgeBWorldX = edgeBLocalX + offset.x;
                    edgeBWorldY = edgeBLocalY + offset.y;

                    angleA = (float)System.Math.Atan2 (edgeAWorldY, edgeAWorldX);
                    angleB = (float)System.Math.Atan2 (edgeBWorldY, edgeBWorldX);

                    rotA = angleA - Mathf.Deg2Rad * light.outerAngle;
                    rotB = angleB + Mathf.Deg2Rad * light.outerAngle;
                                        
                    // Right Collision
                    vC.x = edgeAWorldX;
                    vC.y = edgeAWorldY;

                    // Left Collision
                    vD.x = edgeBWorldX;
                    vD.y = edgeBWorldY;

                    // Right Inner
                    vA.x = edgeAWorldX;
                    vA.y = edgeAWorldY;
                    vA.x += Mathf.Cos(angleA) * lightSize;
                    vA.y += Mathf.Sin(angleA) * lightSize;

                    // Left Inner
                    vB.x = edgeBWorldX;
                    vB.y = edgeBWorldY;
                    vB.x += Mathf.Cos(angleB) * lightSize;
                    vB.y += Mathf.Sin(angleB) * lightSize;

                    vertices[0] = new Vector2(vD.x, vD.y);
                    vertices[1] = new Vector2(vC.x, vC.y);
                    vertices[2] = new Vector2(vA.x, vA.y);
                    vertices[3] = new Vector2(vB.x, vB.y);
        
                    uv[0] = new Vector2(step1, 0);
                    uv[1] = new Vector2(step0, 0);
                    uv[2] = new Vector2(step0, 1);
                    uv[3] = new Vector2(step1, 1);

                    // Right Fin
                    GL.MultiTexCoord3(0, uv[2].x, uv[2].y, 0);
                    GL.Vertex3(vertices[2].x, vertices[2].y, 0);

                    GL.MultiTexCoord3(0, uv[3].x, uv[3].y, 0);
                    GL.Vertex3(vertices[3].x, vertices[3].y, 0);

                    GL.MultiTexCoord3(0, uv[0].x, uv[0].y, 0);
                    GL.Vertex3(vertices[0].x, vertices[0].y, 0);

                    GL.MultiTexCoord3(0, uv[1].x, uv[1].y, 0);
                    GL.Vertex3(vertices[1].x, vertices[1].y, 0);
                }
            }

            GL.End();
        }


        static Pair2D pairA = new Pair2D(Vector2D.Zero(), Vector2D.Zero());
        static Pair2D pairB = new Pair2D(Vector2D.Zero(), Vector2D.Zero());

        static Vector2? PolygonClosestIntersection(Polygon2 poly, Vector2 startPoint, Vector2 endPoint) {
            float distance = 1000000000;
            Vector2? result = null;

            for(int i = 0; i < poly.points.Length; i++) {
                Vector2 pa = poly.points[i];
                Vector2 pb = poly.points[(i + 1) % poly.points.Length];

                pairA.A.x = startPoint.x;
                pairA.A.y = startPoint.y;
                pairA.B.x = endPoint.x;
                pairA.B.y = endPoint.y;

                pairB.A.x = pa.x;
                pairB.A.y = pa.y;
                pairB.B.x = pb.x;
                pairB.B.y = pb.y;

                Vector2? intersection = Math2D.GetPointLineIntersectLine2(pairA, pairB);

                if (intersection != null) {
                    float d = Vector2.Distance(intersection.Value, startPoint);

                    if (result != null) {

                        if (d < distance) {
                            result = intersection.Value;
                            d = distance;
                        }
                    } else {
                        result = intersection.Value;
                        distance = d;
                    }
                    
                }
            }

            return(result);
        }


        static public Vector2? LineIntersectPolygons(Vector2 startPoint, Vector2 endPoint, List<Polygon2> originlPoly) {
            Vector2? result = null;
            float distance = 1000000000;

            foreach(List<Polygon2> polygons in ShadowEngine.effectPolygons) {
                if (originlPoly == polygons) {
                    continue;
                }

                foreach(Polygon2 polygon in polygons) {
                    Vector2? intersection = PolygonClosestIntersection(polygon, startPoint, endPoint);

                    if (intersection != null) {
                        float d = Vector2.Distance(intersection.Value, startPoint);
                        if (result != null) {
                            if (d < distance) {
                                result = intersection.Value;
                                d = distance;
                            }
                        } else {
                            result = intersection.Value;
                            distance = d;
                        }
                    }
                }
                
            }
            
            return(result);
        }

    }
}