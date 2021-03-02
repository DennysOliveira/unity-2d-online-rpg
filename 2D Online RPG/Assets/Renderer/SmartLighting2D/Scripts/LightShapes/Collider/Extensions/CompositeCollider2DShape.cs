using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightShape;

namespace LightShape {

    public class CompositeCollider2DShape : Base {

        CompositeCollider2D compositeCollider = null;

        public CompositeCollider2D GetCompositeCollider() {
            if (compositeCollider == null) {
                compositeCollider = transform.GetComponent<CompositeCollider2D>();
            }

            return(compositeCollider);
        }

        public override List<MeshObject> GetMeshes() {
			if (Meshes == null) {
				List<Polygon2> polygons = GetPolygonsLocal();

				if (polygons.Count > 0) {
					Meshes = new List<MeshObject>();
					
					foreach(Polygon2 poly in polygons) {
						if (poly.points.Length < 3) {
							continue;
						}
						
						Mesh mesh = PolygonTriangulator2.Triangulate (poly, Vector2.zero, Vector2.zero, PolygonTriangulator2.Triangulation.Advanced);
						
						if (mesh) {							
							MeshObject meshObject = MeshObject.Get(mesh);

							if (meshObject != null) {
								Meshes.Add(meshObject);
							}
						}
					}
				}
			}
			return(Meshes);
		}

        public override List<Polygon2> GetPolygonsWorld() {
	
			if (WorldPolygons != null) {
				return(WorldPolygons);
			}

			if (WorldCache != null) {

				WorldPolygons = WorldCache;

				Polygon2 poly;
				Vector2 point;
				List<Polygon2> list = GetPolygonsLocal();

				for(int i = 0; i < list.Count; i++) {
					poly = list[i];
					for(int p = 0; p < poly.points.Length; p++) {
						point = poly.points[p];
						
						WorldPolygons[i].points[p].x = point.x;
						WorldPolygons[i].points[p].y = point.y;
					}
					WorldPolygons[i].ToWorldSpaceSelf(transform);
				}

			} else {

				WorldPolygons = new List<Polygon2>();
				
				foreach(Polygon2 poly in GetPolygonsLocal()) {
					WorldPolygons.Add(poly.ToWorldSpace(transform));
				}

				WorldCache = WorldPolygons;
			
			}
		
			return(WorldPolygons);
		}

        public override List<Polygon2> GetPolygonsLocal() {
			if (LocalPolygons != null) {
				return(LocalPolygons);
			}

            CompositeCollider2D compositeCollider = GetCompositeCollider();

			LocalPolygons = Polygon2Collider2D.CreateFromCompositeCollider(compositeCollider);	

			if (LocalPolygons.Count <= 0) {
				Debug.LogWarning("SmartLighting2D: LightingCollider2D object is missing CompositeCollider2D Component", transform.gameObject);
			}
		
			return(LocalPolygons);
		}
    }
}