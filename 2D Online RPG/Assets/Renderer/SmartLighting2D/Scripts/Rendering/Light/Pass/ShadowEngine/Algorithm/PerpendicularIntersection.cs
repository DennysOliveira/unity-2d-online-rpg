using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public static class PerpendicularIntersection {
        
        public static Pair2 pair = Pair2.Zero();

        public static void Draw(List<Polygon2> polygons, float shadowDistance) {
            if (polygons == null) {
                return;
            }

            UVRect fill = ShadowEngine.FillBlack.uvRect;

            Light2D light = ShadowEngine.light;
            Vector2 offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            float lightSizeSquared = ShadowEngine.lightSize * 2;
            float z = ShadowEngine.shadowZ;

            if (shadowDistance == 0) {
                shadowDistance = lightSizeSquared;
            }

            float outerAngle = light.outerAngle;
            bool drawInside = false;
            bool culling = true;
            
            Vector2 vA, pA, vB, pB, vC, vD;
            float angleA, angleB, rotA, rotB;

            int PolygonCount = polygons.Count;

            Vector2? intersectionLeft;
            Vector2? intersectionRight;

            Vector2 intersectionLeftOffset = Vector2.zero;
            Vector2 intersectionRightOffset = Vector2.zero;

            GL.Color(Color.white);

            for(int i = 0; i < PolygonCount; i++) {

                if (ShadowEngine.lightDrawAbove == false && polygons[i].PointInPoly (-offset)) {
                    drawInside = true;
                } else {
                    drawInside = false;
                }

                Vector2[] pointsList = polygons[i].points;
                int pointsCount = pointsList.Length;
            
                for(int x = 0; x < pointsCount; x++) {
                    int next = (x + 1) % pointsCount;
                    
                    pair.A = pointsList[x];
                    pair.B = pointsList[next];

                    float edgeALocalX = (float)pair.A.x;
                    float edgeALocalY = (float)pair.A.y;

                    float edgeBLocalX = (float)pair.B.x;
                    float edgeBLocalY = (float)pair.B.y;

                    float edgeAWorldX = edgeALocalX + offset.x;
                    float edgeAWorldY = edgeALocalY + offset.y;

                    float edgeBWorldX = edgeBLocalX + offset.x;
                    float edgeBWorldY = edgeBLocalY + offset.y;

                    float lightDirection = Mathf.Atan2((edgeAWorldY + edgeBWorldY) / 2 , (edgeAWorldX + edgeBWorldX) / 2 ) * Mathf.Rad2Deg;
                    float EdgeDirection = (Mathf.Atan2(edgeALocalY - edgeBLocalY, edgeALocalX - edgeBLocalX) * Mathf.Rad2Deg - 180 + 720) % 360;

                    lightDirection -= EdgeDirection;
    
                    lightDirection = (lightDirection + 720) % 360;
                
                    if (culling) {
                        if (drawInside) {
                            if (lightDirection > 180) {
                                continue;
                            }
                        } else {
                            if (lightDirection < 180) {
                                continue;
                            }
                        }
                    }

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
                    vA.x += Mathf.Cos(angleA) * lightSizeSquared;
                    vA.y += Mathf.Sin(angleA) * lightSizeSquared;

                    // Left Inner
                    vB.x = edgeBWorldX;
                    vB.y = edgeBWorldY;
                    vB.x += Mathf.Cos(angleB) * lightSizeSquared;
                    vB.y += Mathf.Sin(angleB) * lightSizeSquared;

                    // Outer Right
                    pA.x = edgeAWorldX;
                    pA.y = edgeAWorldY;
                    pA.x += Mathf.Cos(rotA) * lightSizeSquared;
                    pA.y += Mathf.Sin(rotA) * lightSizeSquared;

                    // Outer Left
                    pB.x = edgeBWorldX;
                    pB.y = edgeBWorldY;
                    pB.x += Mathf.Cos(rotB) * lightSizeSquared;
                    pB.y += Mathf.Sin(rotB) * lightSizeSquared;

                    

                    // Right Intersection
                    intersectionRight = LineIntersectPolygons(vC - offset, vA - offset,  polygons);
                  
                    if (intersectionRight != null) {
                        if (intersectionRight.Value.y < 0) {
                            intersectionRight = null;
                        } else {
                            intersectionRight = intersectionRight + offset;

                            vA.x = (float)intersectionRight.Value.x;
                            vA.y = (float)intersectionRight.Value.y;

                            intersectionRightOffset = intersectionRight.Value;
                            intersectionRightOffset.y += shadowDistance;
                        }
                    }

                    // Left Intersection
                    intersectionLeft = LineIntersectPolygons(vD - offset, vB - offset, polygons);
               
                    if (intersectionLeft != null) {
                        if (intersectionLeft.Value.y < 0) {
                            intersectionLeft = null;
                        } else {
                            intersectionLeft = intersectionLeft + offset;

                            vB.x = (float)intersectionLeft.Value.x;
                            vB.y = (float)intersectionLeft.Value.y;

                            intersectionLeftOffset = intersectionLeft.Value;
                            intersectionLeftOffset.y += shadowDistance;
                        }     
                    }

                    if (intersectionLeft != null && intersectionRight != null) {
                        // Right
                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(vA.x, vA.y, z);

                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, z);

                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(intersectionRightOffset.x, intersectionRightOffset.y, z);

                        //Left
                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(vB.x, vB.y, z);

                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(vA.x, vA.y, z);

                        GL.TexCoord3(fill.x0, fill.y0, 0);
                        GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, z);

                    } else {

                        if (intersectionRight != null) {
                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(vA.x, vA.y, z);

                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(vB.x, vB.y, z);

                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(intersectionRightOffset.x, intersectionRightOffset.y, z);
                        }

                        if (intersectionLeft != null) {
                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(vB.x, vB.y, z);

                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(vA.x, vA.y, z);

                            GL.TexCoord3(fill.x0, fill.y0, 0);
                            GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, z);
                        }
                    }

                    // Right Fin
                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vA.x, vA.y, z);

                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vB.x, vB.y, z);

                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vC.x, vC.y, z);


                    // Left Fin
                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vB.x, vB.y, z);

                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vD.x, vD.y, z);

                    GL.TexCoord3(fill.x0, fill.y0, 0);
                    GL.Vertex3(vC.x, vC.y, z);

                
                }
            }
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