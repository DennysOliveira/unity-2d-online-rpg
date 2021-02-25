using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DayLighting {
    
    public class ShadowMesh {
        public List<List<DoublePair2>> polygonsPairs = new List<List<DoublePair2>>();
        public List<MeshObject> meshes = new List<MeshObject>();
        public List<MeshObject> softMeshes = new List<MeshObject>();

        const float pi2 = Mathf.PI / 2;

        static Vector2D zA = Vector2D.Zero(), zB = Vector2D.Zero(), zC = Vector2D.Zero();
        static Vector2D pA = Vector2D.Zero(), pB = Vector2D.Zero();
        static Vector2D objectOffset = Vector2D.Zero();
        static Vector2D vecA, vecB, vecC;

        MeshBrush meshBrush;

        public void Generate(DayLightColliderShape shape) {
            float height = shape.height;

            if (shape.shadowType== DayLightCollider2D.ShadowType.Sprite) {
                return;
			}

            Clear();

            List<Polygon2> polys = shape.GetPolygonsLocal();

            if (polys == null) {
                return;
            }

            if (meshBrush == null) {
                meshBrush = new MeshBrush();
            }

            float sunHeight = Lighting2D.DayLightingSettings.height;

            GenerateFill(shape.transform, shape, height * sunHeight);
            
            bool softness = Lighting2D.DayLightingSettings.softness.enable;

            if (softness) {
                GenerateSoftness();
            }
        }

        public void Clear() {
            foreach(MeshObject mesh in meshes) {
                UnityEngine.Object.DestroyImmediate(mesh.mesh);
            }
            
            softMeshes.Clear();
            meshes.Clear();
            polygonsPairs.Clear();

            if (meshBrush != null) {
                meshBrush.Clear();
            }
        }

        public void GenerateFill(Transform transform, DayLightColliderShape shape, float height) {
            List<Polygon2> polys = shape.GetPolygonsLocal();
            float direction = Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;

            foreach(Polygon2 polygon in polys) {
                if (polygon.points.Length < 2) {
                    continue;
                }

                Polygon2 worldPolygon = polygon.Copy();
                
                worldPolygon.ToScaleSelf(transform.localScale); 
                worldPolygon.ToRotationSelf(transform.rotation.eulerAngles.z * Mathf.Deg2Rad);

                Polygon2 polygonShadow = GenerateShadow(worldPolygon, direction, height); 
                List<DoublePair2> polygonPairs = DoublePair2.GetList(polygonShadow.points);

                Polygon2 polygonShadowFill = polygonShadow.Copy();

                int pointsCount = polygonShadowFill.points.Length;

                for(int i = 0; i < pointsCount; i++) {
                    Vector2 a = polygonShadowFill.points[(i - 1 + pointsCount) % pointsCount];
                    Vector2 b = polygonShadowFill.points[i];
                    Vector2 c = polygonShadowFill.points[(i + 1) % pointsCount];

                   	float angle_a = (float)System.Math.Atan2 (a.y - b.y, a.x - b.x) + pi2;
                    float angle_c = (float)System.Math.Atan2 (b.y - c.y, b.x - c.x) + pi2;

                    b.x += Mathf.Cos(angle_a) * 0.001f;
                    b.y += Mathf.Sin(angle_a) * 0.001f;

                    b.x += Mathf.Cos(angle_c) * 0.001f;
                    b.y += Mathf.Sin(angle_c) * 0.001f;

                    polygonShadowFill.points[i] = b;
                }

                polygonsPairs.Add(polygonPairs);

                Mesh mesh = PolygonTriangulator2.TriangulateSimple(polygonShadowFill, Vector2.zero, Vector2.zero);

                MeshObject meshObject = MeshObject.Get(mesh);

                if (meshObject != null) {
                    meshes.Add(meshObject);
                }
            }
        }

        public void GenerateSoftness() {
            List<DoublePair2> polygonPairs;
            DoublePair2 p;
            int polygonCount = polygonsPairs.Count;
            int trianglesCount = 0;
            float intensity = Lighting2D.DayLightingSettings.softness.intensity;

            for(int i = 0; i < polygonCount; i++) {
                polygonPairs = polygonsPairs[i];

                int polygonPairsCount = polygonPairs.Count;

                for(int x = 0; x < polygonPairsCount; x++) {
                    p = polygonPairs[x];

                    zA.x = (float)p.A.x;
                    zA.y = (float)p.A.y;

                    zB.x = (float)p.B.x;
                    zB.y = (float)p.B.y;

                    zC.x = zB.x;
                    zC.y = zB.y;

                    pA.x = zA.x;
                    pA.y = zA.y;

                    pB.x = zB.x;
                    pB.y = zB.y;					

                    float angleA = (float)System.Math.Atan2 (p.A.y - p.B.y, p.A.x - p.B.x) + pi2;
                    float angleB = (float)System.Math.Atan2 (p.A.y - p.B.y, p.A.x - p.B.x) + pi2;
                    float angleC = (float)System.Math.Atan2 (p.B.y - p.C.y, p.B.x - p.C.x) + pi2;

                    zA.x += (float)System.Math.Cos(angleA) * intensity;
                    zA.y += (float)System.Math.Sin(angleA) * intensity;

                    zB.x += (float)System.Math.Cos(angleB) * intensity;
                    zB.y += (float)System.Math.Sin(angleB) * intensity;

                    zC.x += (float)System.Math.Cos(angleC) * intensity;
                    zC.y += (float)System.Math.Sin(angleC) * intensity;

                    meshBrush.uv.Add(new Vector2(0, 0));
                    meshBrush.vertices.Add(new Vector3((float)pB.x, (float)pB.y, 0));

                    meshBrush.uv.Add(new Vector2(0.5f - 0, 0));
                    meshBrush.vertices.Add(new Vector3((float)pA.x, (float)pA.y, 0));


                    meshBrush.uv.Add(new Vector2(0.5f - 0, 1));
                    meshBrush.vertices.Add(new Vector3((float)zA.x, (float)zA.y, 0));



                    meshBrush.uv.Add(new Vector2(0, 1));
                    meshBrush.vertices.Add(new Vector3((float)zA.x, (float)zA.y, 0));

                    meshBrush.uv.Add(new Vector2(0.5f - 0, 1));
                    meshBrush.vertices.Add(new Vector3((float)zB.x, (float)zB.y, 0));

                    meshBrush.uv.Add(new Vector2(0.5f - 0, 0));
                    meshBrush.vertices.Add(new Vector3((float)pB.x, (float)pB.y, 0));



                    meshBrush.uv.Add(new Vector2(0, 1));
                    meshBrush.vertices.Add(new Vector3((float)zB.x, (float)zB.y, 0));

                    meshBrush.uv.Add(new Vector2(0.5f - 0, 0));
                    meshBrush.vertices.Add(new Vector3((float)pB.x, (float)pB.y, 0));

                    meshBrush.uv.Add(new Vector2(0.5f - 0, 1));
                    meshBrush.vertices.Add(new Vector3((float)zC.x, (float)zC.y, 0));

                    meshBrush.triangles.Add(trianglesCount + 0);
                    meshBrush.triangles.Add(trianglesCount + 1);
                    meshBrush.triangles.Add(trianglesCount + 2);

                    meshBrush.triangles.Add(trianglesCount + 3);
                    meshBrush.triangles.Add(trianglesCount + 4);
                    meshBrush.triangles.Add(trianglesCount + 5);

                    meshBrush.triangles.Add(trianglesCount + 6);
                    meshBrush.triangles.Add(trianglesCount + 7);
                    meshBrush.triangles.Add(trianglesCount + 8);

                    trianglesCount += 9;
                }
            }

            if (trianglesCount > 0) {
                Mesh mesh = meshBrush.Export();

                MeshObject meshObject = MeshObject.Get(mesh);

                if (meshObject != null) {
                    softMeshes.Add(meshObject);
                }
            }
        }

        static public Polygon2 GenerateShadow(Polygon2 poly, float sunDirection, float height) {
            Polygon2D convexHull = new Polygon2D ();
            Vector2D vA;

            sunDirection = -sunDirection;
            
            foreach (Vector2 p in poly.points) {
                vA = new Vector2D(p);
                vA.Push (sunDirection, height);
                
                convexHull.pointsList.Add (vA);
                convexHull.pointsList.Add (new Vector2D(p));
            }

            convexHull.pointsList = Math2D.GetConvexHull (convexHull.pointsList);
            
            return(new Polygon2(convexHull));
        }
    }     
}