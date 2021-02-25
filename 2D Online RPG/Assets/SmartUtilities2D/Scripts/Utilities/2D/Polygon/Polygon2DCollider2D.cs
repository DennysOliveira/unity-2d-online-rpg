using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2DCollider2D  {
	static public int defaultCircleVerticesCount = 25;

	public static List<Polygon2D> CreateFromCompositeCollider(CompositeCollider2D compositeCollider) {
		List<Polygon2D> list = new List<Polygon2D>();

		if (compositeCollider != null) {
			int pathCount = compositeCollider.pathCount;
			

			for (int i = 0; i < pathCount; i++) {
				int pointCount = compositeCollider.GetPathPointCount(i);

				Vector2[] pointsInPath = new Vector2[pointCount];

				compositeCollider.GetPath(i, pointsInPath);

				Polygon2D polygon = new Polygon2D();
				for (int j = 0; j < pointsInPath.Length; j++) {
					polygon.AddPoint(pointsInPath[j]);
				}
				
				polygon.Normalize();
				
				list.Add(polygon);
			}
		}
		
		return(list);
	}

	static public Polygon2D CreateFromEdgeCollider(EdgeCollider2D edgeCollider) {
		Polygon2D newPolygon = new Polygon2D ();
		if (edgeCollider != null) {
			foreach (Vector2 p in edgeCollider.points) {
				newPolygon.AddPoint (p + edgeCollider.offset);
			}
			//newPolygon.AddPoint (edgeCollider.points[0] + edgeCollider.offset);
		}
		return(newPolygon);
	}

	static public Polygon2D CreateFromCircleCollider(CircleCollider2D circleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D ();

		float size = circleCollider.radius;
		float i = 0;

		while (i < 360) {
			newPolygon.AddPoint (new Vector2(Mathf.Cos (i * Mathf.Deg2Rad) * size, Mathf.Sin (i * Mathf.Deg2Rad) * size) + circleCollider.offset);
			i += 360f / (float)pointsCount;
		}

		return(newPolygon);
	}

	static public Polygon2D CreateFromBoxCollider(BoxCollider2D boxCollider) {
		Polygon2D newPolygon = new Polygon2D();

		Vector2 size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

		newPolygon.AddPoint (new Vector2(-size.x, -size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(-size.x, size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(size.x, size.y) + boxCollider.offset);
		newPolygon.AddPoint (new Vector2(size.x, -size.y) + boxCollider.offset);

		return(newPolygon);
	}

	static public Polygon2D CreateFromCapsuleCollider(CapsuleCollider2D capsuleCollider, int pointsCount = -1) {
		if (pointsCount < 1) {
			pointsCount = defaultCircleVerticesCount;
		}

		Polygon2D newPolygon = new Polygon2D();

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

					newPolygon.AddPoint (point + capsuleCollider.offset);
					angle += step;
				}

				while (angle < 360) {
					point.x = Mathf.Cos (angle * Mathf.Deg2Rad) * size.x;
					point.y = -offset + Mathf.Sin (angle * Mathf.Deg2Rad) * size.x;

					newPolygon.AddPoint (point + capsuleCollider.offset);
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

					newPolygon.AddPoint(point + capsuleCollider.offset);
					angle += step;
				}

				while (angle < 270) {
					point.x = -offset + Mathf.Cos (angle * Mathf.Deg2Rad) * size.y;
					point.y = Mathf.Sin (angle * Mathf.Deg2Rad) * size.y;

					newPolygon.AddPoint (point + capsuleCollider.offset);
					angle += step;
				}
				break;
		}

		return(newPolygon);
	}
}