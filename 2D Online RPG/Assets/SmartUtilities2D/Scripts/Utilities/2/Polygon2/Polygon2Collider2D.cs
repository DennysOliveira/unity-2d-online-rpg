using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2Collider2D  {
	static public int defaultCircleVerticesCount = 25;

	public static List<Polygon2> CreateFromCompositeCollider(CompositeCollider2D compositeCollider) {
		List<Polygon2> list = new List<Polygon2>();

		if (compositeCollider != null) {
			int pathCount = compositeCollider.pathCount;
			

			for (int i = 0; i < pathCount; i++) {
				int pointCount = compositeCollider.GetPathPointCount(i);

				Vector2[] pointsInPath = new Vector2[pointCount];

				compositeCollider.GetPath(i, pointsInPath);

				Polygon2 polygon = new Polygon2(pointsInPath);
				
				polygon.Normalize();
				
				list.Add(polygon);
			}
		}
		
		return(list);
	}

	static public Polygon2 CreateFromEdgeCollider(EdgeCollider2D edgeCollider) {
		List<Vector2> points = new List<Vector2>();

		if (edgeCollider != null) {
			foreach (Vector2 p in edgeCollider.points) {
				points.Add (p + edgeCollider.offset);
			}
			//newPolygon.AddPoint (edgeCollider.points[0] + edgeCollider.offset);
		}
		return(new Polygon2(points));
	}

	static public Polygon2 CreateFromCircleCollider(CircleCollider2D circleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		List<Vector2> points = new List<Vector2>();

		float size = circleCollider.radius;
		float i = 0;

		while (i < 360) {
			points.Add(new Vector2(Mathf.Cos (i * Mathf.Deg2Rad) * size, Mathf.Sin (i * Mathf.Deg2Rad) * size) + circleCollider.offset);
			i += 360f / (float)pointsCount;
		}

		return(new Polygon2(points));
	}

	static public Polygon2 CreateFromBoxCollider(BoxCollider2D boxCollider) {


		Vector2 size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

		List<Vector2> points = new List<Vector2>();

		points.Add( new Vector2(-size.x, -size.y) + boxCollider.offset);
		points.Add( new Vector2(-size.x, size.y) + boxCollider.offset);
		points.Add( new Vector2(size.x, size.y) + boxCollider.offset);
		points.Add( new Vector2(size.x, -size.y) + boxCollider.offset);

		return(new Polygon2(points));
	}

	static public Polygon2 CreateFromCapsuleCollider(CapsuleCollider2D capsuleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		List<Vector2> points = new List<Vector2>();

		Vector2 size = new Vector2(capsuleCollider.size.x / 2, capsuleCollider.size.y / 2);
		Vector2 point;
		float offset = 0;
		float angle = 0;
		float sizeRatio = 0;
		float step = 360f / (float)pointsCount;

		switch (capsuleCollider.direction) {
			case CapsuleDirection2D.Vertical:
				sizeRatio = (capsuleCollider.transform.localScale.x / capsuleCollider.transform.localScale.y);
				size.x *= sizeRatio;
				angle = 0;

				if (capsuleCollider.size.x < capsuleCollider.size.y) {
					offset = (capsuleCollider.size.y - capsuleCollider.size.x) / 2;
				}
					
				while (angle < 180) {
					point.x = Mathf.Cos (angle * Mathf.Deg2Rad) * size.x;
					point.y = offset + Mathf.Sin (angle * Mathf.Deg2Rad) * size.x;

					points.Add (point + capsuleCollider.offset);
					angle += step;
				}

				while (angle < 360) {
					point.x = Mathf.Cos (angle * Mathf.Deg2Rad) * size.x;
					point.y = -offset + Mathf.Sin (angle * Mathf.Deg2Rad) * size.x;

					points.Add (point + capsuleCollider.offset);
					angle += step;
				}
				break;

			case CapsuleDirection2D.Horizontal:
				sizeRatio = (capsuleCollider.transform.localScale.y / capsuleCollider.transform.localScale.x);
				size.x *= sizeRatio;
				angle = -90;

				if (capsuleCollider.size.y < capsuleCollider.size.x)  {
					offset = (capsuleCollider.size.x - capsuleCollider.size.y) / 2;
				}

				while (angle < 90) {
					point.x = offset + Mathf.Cos (angle * Mathf.Deg2Rad) * size.y;
					point.y = Mathf.Sin (angle * Mathf.Deg2Rad) * size.y;

					points.Add(point + capsuleCollider.offset);
					angle += step;
				}

				while (angle < 270) {
					point.x = -offset + Mathf.Cos (angle * Mathf.Deg2Rad) * size.y;
					point.y = Mathf.Sin (angle * Mathf.Deg2Rad) * size.y;

					points.Add (point + capsuleCollider.offset);
					angle += step;
				}
				break;
		}

		return(new Polygon2(points));
	}
}