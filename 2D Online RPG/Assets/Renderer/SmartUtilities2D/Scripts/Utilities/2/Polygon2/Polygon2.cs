using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class Polygon2 {
	public Vector2[] points;

	public Polygon2 Copy() {
		Vector2[] array = new Vector2[points.Length];

		System.Array.Copy(points, array, points.Length);

		return(new Polygon2(array));
	}


	public Polygon2 ToWorldSpace(Transform transform) {
		Polygon2 newPolygon = this.Copy();

		newPolygon.ToWorldSpaceSelf(transform);

		return(newPolygon);
	}

	public void ToScaleSelf(Vector2 scale, Vector2? center = null) {
		if (center == null) {
			center = Vector2.zero;
		}

		float dist, rot;
		Vector2 point;

		for(int id = 0; id < points.Length; id++) {
			point = points[id];

			dist = Vector2.Distance(point, center.Value);
			rot = point.Atan2(center.Value); //??

			point.x = center.Value.x + Mathf.Cos(rot) * dist * scale.x;
			point.y = center.Value.y + Mathf.Sin(rot) * dist * scale.y;

			points[id] = point;
		}
	}

	public void ToRotationSelf(float rotation, Vector2? center = null) {
		if (center == null) {
			center = Vector2.zero;
		}

		float dist, rot;
		Vector2 point;

		for(int id = 0; id < points.Length; id++) {
			point = points[id];
			
			dist = Vector2.Distance(point, center.Value);
			rot = point.Atan2(center.Value) + rotation; //??

			point.x = center.Value.x + Mathf.Cos(rot) * dist;
			point.y = center.Value.y + Mathf.Sin(rot) * dist;

			points[id] = point;
		}
	}

	



	public Polygon2(List<Vector2> pointList) {
		points = pointList.ToArray();
	}

	public Polygon2(int size) {
		points = new Vector2[size];
	}

	public Polygon2(Polygon2D polygon) {
		points = new Vector2[polygon.pointsList.Count];

		for(int id = 0; id < polygon.pointsList.Count; id++) {
			points[id] = polygon.pointsList[id].ToVector2();
		}
	}

	public Polygon2(Vector2[] array) {
		points = array;
	}


	
	public void ToOffsetSelf(Vector2 pos) {
		for(int id = 0; id < points.Length; id++) {
			points[id] += pos;
		}
	}

	public bool IsClockwise() {
		if (points.Length < 1) {
			return (true);
		}
		
		double sum = 0;

		Vector2 A = points[points.Length - 1];
		Vector2 B;

		for(int i = 0; i < points.Length; i++) {
			B = points[i];

			sum += (B.x - A.x) * (B.y + A.y);

			A = B;
		}

		return(sum > 0);
	}

	public void Normalize() {
		if (IsClockwise () == false) {
			System.Array.Reverse(points);
		}
	}














	
	///// Constructors - Polygon Creating //////

	static public Polygon2 CreateRect(Vector2 size) {
		size = size / 2;
		
		Polygon2 polygon = new Polygon2(4);

		polygon.points[0] = new Vector2(-size.x, -size.y);
		polygon.points[1] = new Vector2(size.x, -size.y);
		polygon.points[2] = new Vector2(size.x, size.y);
		polygon.points[3] = new Vector2(-size.x, size.y);

		polygon.Normalize();

		return(polygon);
	}

	static public Polygon2 CreateIsometric(Vector2 size) {
		size = size / 2;
		
		Polygon2 polygon = new Polygon2(4);

		polygon.points[0] = new Vector2(-size.x, size.y );
		polygon.points[1] = new Vector2(0, 0);
		polygon.points[2] = new Vector2(size.x,  size.y);
		polygon.points[3] = new Vector2(0, size.y * 2);

		polygon.Normalize();

		return(polygon);
	}

	static public Polygon2 CreateHexagon(Vector2 size) {
		size = size / 2;

		Polygon2 polygon = new Polygon2(6);


		polygon.points[0] = new Vector2(-size.x, size.y);
		polygon.points[1] = new Vector2(-size.x, -size.y);
		polygon.points[2] = new Vector2(0, -size.y * 2);
		polygon.points[3] = new Vector2(size.x, -size.y);
		polygon.points[4] = new Vector2(size.x,  size.y);
		polygon.points[5] = new Vector2(0, size.y * 2);

		polygon.Normalize();

		return(polygon);
	}

	public Mesh CreateMesh(GameObject gameObject, Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced) {		
		if (gameObject.GetComponent<MeshRenderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
		}

		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		if (filter == null) {
			filter = gameObject.AddComponent<MeshFilter>() as MeshFilter;
		}
		
		filter.sharedMesh = PolygonTriangulator2.Triangulate (this, UVScale, UVOffset, triangulation);
		if (filter.sharedMesh == null) {
			UnityEngine.Object.Destroy(gameObject);
		}

		return(filter.sharedMesh);
	}
	
	public Mesh CreateMesh(Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced) {        
        return(PolygonTriangulator2.Triangulate (this, UVScale, UVOffset, triangulation));
    }































	
	public void ToWorldSpaceSelfUNIVERSAL(Transform transform) {
		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				ToWorldSpaceSelfXY(transform);
			break;

			case CoreAxis.XYFLIPPED:
				ToWorldSpaceSelfFlipped(transform);
			break;

			case CoreAxis.XZFLIPPED:
				ToWorldSpaceSelfXZFlipped(transform);
			break;

			case CoreAxis.XZ:
				ToWorldSpaceSelfXZ(transform);
			break;	
		}
	}

	public void ToWorldSpaceSelf(Transform transform) {
		for(int id = 0; id < points.Length; id++) {
			points[id] = transform.TransformPoint (points[id]);
		}
	}

	public void ToWorldSpaceSelfXY(Transform transform) {
		int count = points.Length;
		
		for(int id = 0; id < count; id++) {
			points[id] = points[id].TransformToWorldXY(transform);
		}

		/*
		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItself(transform);
			}
		}*/
	}

	public void ToWorldSpaceSelfFlipped(Transform transform) {
		int count = points.Length;
		
		for(int id = 0; id < count; id++) {
			points[id] = points[id].TransformToWorldXYFlipped(transform);
		}

		/*
		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfFlipped(transform);
			}
		}*/
	}

	public void ToWorldSpaceSelfXZ(Transform transform) {
		int count = points.Length;
		
		for(int id = 0; id < count; id++) {
			points[id] = points[id].TransformToWorldXZ(transform);
		}
		
		/*
		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfXZ(transform);
			}
		}*/
	}

	public void ToWorldSpaceSelfXZFlipped(Transform transform) {
		int count = points.Length;
		
		for(int id = 0; id < count; id++) {
			points[id] = points[id].TransformToWorldXZFlipped(transform);
		}

		/*
		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfXZFlipped(transform);
			}
		}*/
	}

	public bool PointInPoly(Vector2 point) {
		return(Math2D.PointInPoly(point, this));
	}
}