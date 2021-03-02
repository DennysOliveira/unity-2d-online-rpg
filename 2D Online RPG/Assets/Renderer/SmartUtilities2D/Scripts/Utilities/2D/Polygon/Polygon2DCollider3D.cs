using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2DCollider3D  {
	static public int defaultCircleVerticesCount = 25;

	static public Polygon2D CreateFromBoxCollider(BoxCollider boxCollider) {
		Polygon2D newPolygon = new Polygon2D();

		Vector2 size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

		Vector2 offset = boxCollider.center;

		newPolygon.AddPoint (new Vector2(-size.x, -size.y) + offset);
		newPolygon.AddPoint (new Vector2(-size.x, size.y) + offset);
		newPolygon.AddPoint (new Vector2(size.x, size.y) + offset);
		newPolygon.AddPoint (new Vector2(size.x, -size.y) + offset);

		return(newPolygon);
	}

	static public List<Polygon2D> CreateFromMeshCollider(MeshCollider meshCollider) {
		List<Polygon2D> newPolygons = new List<Polygon2D>();

		Vector2 size = new Vector2(1, 1);

		Vector2 offset = Vector2.zero;

        Mesh mesh = meshCollider.sharedMesh;

        int length = mesh.triangles.GetLength (0);

        for (int i = 0; i < length; i = i + 3) {
            Vector2 vecA = mesh.vertices [mesh.triangles [i]];
            Vector2 vecB = mesh.vertices [mesh.triangles [i + 1]];
            Vector2 vecC = mesh.vertices [mesh.triangles [i + 2]];

            Polygon2D poly = new Polygon2D();
            poly.AddPoint(vecA.x, vecA.y);
            poly.AddPoint(vecB.x, vecB.y);
            poly.AddPoint(vecC.x, vecC.y);

            newPolygons.Add(poly);
        }	

		return(newPolygons);
	}


	static public Polygon2D CreateFromSphereCollider(SphereCollider sphereCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D ();

		float size = sphereCollider.radius;
		float i = 0;

		Vector2 offset = sphereCollider.center;

		while (i < 360) {
			newPolygon.AddPoint (new Vector2(Mathf.Cos (i * Mathf.Deg2Rad) * size, Mathf.Sin (i * Mathf.Deg2Rad) * size) + offset);
			i += 360f / (float)pointsCount;
		}

		return(newPolygon);
	}

	static public Polygon2D CreateFromCapsuleCollider(CapsuleCollider capsuleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D();

		float radius = capsuleCollider.radius;
		float height = capsuleCollider.height / 2;

		Vector2 size = new Vector2(capsuleCollider.radius, capsuleCollider.radius);
		Vector2 offset = capsuleCollider.center;

		float off = 0;

		if (height > radius) {
			off = height - radius;
		}
	
		float i = 0;
	
		while (i < 180) {
			Vector2 v = new Vector2 (Mathf.Cos (i * Mathf.Deg2Rad) * size.x, off + Mathf.Sin (i * Mathf.Deg2Rad) * size.x);
			newPolygon.AddPoint (v + offset);
			i += 360f / (float)pointsCount;
		}

		while (i < 360) {
			Vector2 v = new Vector2 (Mathf.Cos (i * Mathf.Deg2Rad) * size.x, -off + Mathf.Sin (i * Mathf.Deg2Rad) * size.x);
			newPolygon.AddPoint (v + offset);
			i += 360f / (float)pointsCount;
		}
		
		return(newPolygon);
	}
}