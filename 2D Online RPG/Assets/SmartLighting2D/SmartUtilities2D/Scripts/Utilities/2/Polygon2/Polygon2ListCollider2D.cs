using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2ListCollider2D : Polygon2Collider2D {

	public static List<Polygon2> CreateFromGameObject(GameObject gameObject) {
		List<Polygon2> result = new List<Polygon2>();
		
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

				List<Polygon2> polygonColliders = CreateFromPolygonColliderToLocalSpace(polygonCollider2D);

				foreach(Polygon2 poly in polygonColliders) {
					result.Add(poly);
				}

			}
		}

		foreach(Polygon2 poly in result) {
			poly.Normalize();
		}

		return(result);
	}


	
	// Get List Of Polygons from Collider (Usually Used Before Creating Slicer2D Object)
	static public List<Polygon2> CreateFromPolygonColliderToWorldSpace(PolygonCollider2D collider) {
		List<Polygon2> result = new List<Polygon2> ();

		if (collider != null && collider.pathCount > 0) {
			Vector2[] array = collider.GetPath (0);

			Polygon2 newPolygon = new Polygon2 (array.Length);

			for(int i = 0; i < array.Length; i++) {
				Vector2 p = array[i];
				
				newPolygon.points[i] = p + collider.offset;
			}
			
			newPolygon = newPolygon.ToWorldSpace(collider.transform);

			result.Add (newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Vector2[] arrayHole = collider.GetPath (i);

				Polygon2 hole = new Polygon2 (arrayHole.Length);

				for(int x = 0; x < arrayHole.Length; x++) {
					hole.points[i] = arrayHole[x] + collider.offset;
				}

				hole = hole.ToWorldSpace(collider.transform);

				result.Add(hole);
			}
		}
		return(result);
	}

	static public List<Polygon2> CreateFromPolygonColliderToLocalSpace(PolygonCollider2D collider) {
		List<Polygon2> result = new List<Polygon2>();

		if (collider != null && collider.pathCount > 0) {
			Vector2[] array = collider.GetPath (0);

			Polygon2 newPolygon = new Polygon2 (array.Length);

			for(int i = 0; i < array.Length; i++) {
				Vector2 p = array[i];
				
				newPolygon.points[i] = (p + collider.offset);
			}

			result.Add(newPolygon);

			for (int i = 1; i < collider.pathCount; i++) {
				Vector2[] arrayHole = collider.GetPath (i);

				Polygon2 hole = new Polygon2 (arrayHole.Length);

				for(int x = 0; x < arrayHole.Length; x++) {
					hole.points[i] = arrayHole[x] + collider.offset;
				}

				result.Add(hole);
			}
		}
		return(result);
	}

}