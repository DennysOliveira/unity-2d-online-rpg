using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2DListCollider2D : Polygon2DCollider2D {

	// Get List Of Polygons from Collider (Usually Used Before Creating Slicer2D Object)
	static public List<Polygon2D> CreateFromPolygonColliderToWorldSpace(PolygonCollider2D collider) {
		List<Polygon2D> result = new List<Polygon2D> ();

		if (collider != null && collider.pathCount > 0) {
			Polygon2D newPolygon = new Polygon2D ();

			foreach (Vector2 p in collider.GetPath (0)) {
				newPolygon.AddPoint (p + collider.offset);
			}
			
			newPolygon = newPolygon.ToWorldSpace(collider.transform);

			result.Add (newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Polygon2D hole = new Polygon2D ();
				foreach (Vector2 p in collider.GetPath (i)) {
					hole.AddPoint (p + collider.offset);
				}

				hole = hole.ToWorldSpace(collider.transform);

				if (newPolygon.PolyInPoly (hole) == true) {
					newPolygon.AddHole(hole);
				} else {
					result.Add(hole);
				}
			}
		}
		return(result);
	}

	static public List<Polygon2D> CreateFromPolygonColliderToLocalSpace(PolygonCollider2D collider) {
		List<Polygon2D> result = new List<Polygon2D>();

		if (collider != null && collider.pathCount > 0) {
			Polygon2D newPolygon = new Polygon2D ();

			foreach (Vector2 p in collider.GetPath (0)) {
				newPolygon.AddPoint (p + collider.offset);
			}

			result.Add(newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Polygon2D hole = new Polygon2D ();
				foreach (Vector2 p in collider.GetPath (i)) {
					hole.AddPoint (p + collider.offset);
				}

				if (newPolygon.PolyInPoly (hole) == true) {
					newPolygon.AddHole (hole);
				} else {
					result.Add(hole);
				}
			}
		}
		return(result);
	}

	// Slower CreateFromCollider
	public static List<Polygon2D> CreateFromGameObject(GameObject gameObject) {
		List<Polygon2D> result = new List<Polygon2D>();
		
		foreach(Collider2D c in gameObject.GetComponents<Collider2D> ()) {
			System.Type type = c.GetType();

			if (type == typeof(BoxCollider2D)) {
				BoxCollider2D boxCollider2D = (BoxCollider2D)c;
				

				result.Add(CreateFromBoxCollider(boxCollider2D));
			}

			if (type == typeof(CircleCollider2D)) {
				CircleCollider2D circleCollider2D = (CircleCollider2D)c;

				result.Add(CreateFromCircleCollider(circleCollider2D));
			}

			if (type == typeof(CapsuleCollider2D)) {
				CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)c;

				result.Add(CreateFromCapsuleCollider(capsuleCollider2D));
			}

			if (type == typeof(EdgeCollider2D)) {
				EdgeCollider2D edgeCollider2D = (EdgeCollider2D)c;

				result.Add(CreateFromEdgeCollider(edgeCollider2D));
			}

			if (type == typeof(PolygonCollider2D)) {
				PolygonCollider2D polygonCollider2D = (PolygonCollider2D)c;

				List<Polygon2D> polygonColliders = CreateFromPolygonColliderToLocalSpace(polygonCollider2D);

				foreach(Polygon2D poly in polygonColliders) {
					result.Add(poly);
				}

			}
		}

		foreach(Polygon2D poly in result) {
			poly.Normalize();
		}

		return(result);
	}
}