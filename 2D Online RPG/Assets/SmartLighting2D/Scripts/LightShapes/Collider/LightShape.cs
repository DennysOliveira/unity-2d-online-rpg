using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightShape {
		
	public class Base {
		
		public List<Polygon2> WorldPolygons = null;
		public List<Polygon2> WorldCache = null;

		public Vector2? WorldPoint = null;
		public Rect WorldRect;

		// Iso Vars
		public Rect IsoWorldRect;

		public List<Polygon2> LocalPolygons = null;

		public List<MeshObject> Meshes = null;
	
		public Transform transform;

		public virtual int GetSortingOrder() {
			return(0);
		}

		public virtual int GetSortingLayer() {
			return(0);
		}

		public void SetTransform(Transform transform) {
			this.transform = transform;
		}

		virtual public List<Polygon2> GetPolygonsLocal() {
			return(LocalPolygons);
		}

		virtual public List<Polygon2> GetPolygonsWorld() {
			return(WorldPolygons);
		}

		virtual public void ResetLocal() {
			Meshes = null;

			LocalPolygons = null;

			WorldPolygons = null;
			WorldCache = null;

			ResetWorld();
		}

		virtual public void ResetWorld() {
			WorldPolygons = null;

			WorldRect = new Rect();

			IsoWorldRect = new Rect();

			WorldPoint = null;
		}

		public Rect GetWorldRect() {
			if (WorldRect.width < 0.01f) {
				WorldRect = Polygon2Helper.GetRect(GetPolygonsWorld());
			}

			return(WorldRect);
		}

		
		public Rect GetIsoWorldRect() {
			if (IsoWorldRect.width < 0.01f) {
				IsoWorldRect = Polygon2Helper.GetIsoRect(GetPolygonsWorld());
			}

			return(IsoWorldRect);
		}
		
		public Vector2 GetPivotPoint_ShapeCenter() {
			if (WorldPoint == null) {

				List<Polygon2> polys = GetPolygonsWorld();

				WorldPoint = GetWorldRect().center;
			}

			return(WorldPoint.Value);
		}

		public Vector2 GetPivotPoint_TransformCenter() {
			if (WorldPoint == null) {
				WorldPoint = transform.position;
			}

			return(WorldPoint.Value);
		}

		public Vector2 GetPivotPoint_LowestY() {
			if (WorldPoint == null) {

				List<Polygon2> polys = GetPolygonsWorld();

				Vector2 lowestPoint = Vector2.zero;
				lowestPoint.y = 999999;

				foreach(Polygon2 poly in polys) {
					foreach(Vector2 p in poly.points) {
						if (p.y < lowestPoint.y) {
							lowestPoint = p;
						}
					}
				}
				WorldPoint = lowestPoint;
			}

			return(WorldPoint.Value);
		}

		public virtual List<MeshObject> GetMeshes() {
			return(null);
		}

	}
}