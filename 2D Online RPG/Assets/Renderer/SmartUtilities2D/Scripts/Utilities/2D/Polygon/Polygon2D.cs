using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class Polygon2D  {
	public List<Vector2D> pointsList = new List<Vector2D>();
	public List<Polygon2D> holesList = new List<Polygon2D>();

	public void AddPoint(Vector2D point) {
		pointsList.Add (point);
	}

	public void AddPoint(Vector2 point) {
		pointsList.Add (new Vector2D(point));
	}

	public void AddPoint(float pointX, float pointY) {
		pointsList.Add (new Vector2D(pointX, pointY));
	}

	public void AddPoints(List<Vector2D> points) { 
		foreach (Vector2D point in points) {
			AddPoint (point);
		}
	}

	public Polygon2D() {}
	public Polygon2D(List<Vector2D> polygonPointsList) {
		pointsList = polygonPointsList; //new List<Vector2D>(
	}

	public Polygon2D(Polygon2D polygon) {
		pointsList = polygon.pointsList;
		holesList = polygon.holesList;
	}

	public void AddHole(Polygon2D poly) {
		holesList.Add (poly);
	}
		
	public bool PointInPoly(Vector2D point) {
		return(Math2D.PointInPoly(point, this));
	}

	// ***********8


	static Vector2D v = Vector2D.Zero();
	public bool PointInPoly(Vector2 point) {
		v.x = point.x;
		v.y = point.y;
		if (PointInHole (v) != null) {
			return(false);
		}
		
		return(Math2D.PointInPoly(v, this));
	}







	//**********

	public bool PolyInPoly(Polygon2D poly) { // Not Finished? 
		foreach (Polygon2D holes in holesList) {
			if (Math2D.PolyIntersectPoly (poly, holes) == true) { 
				return(false);
			}
		}
		
		return(Math2D.PolyInPoly(this, poly));
	}

	public Polygon2D PointInHole(Vector2D point) {
		foreach (Polygon2D p in holesList) {
			if (p.PointInPoly (point) == true) {
				return(p);
			}
		}

		return(null);
	}

	private static Pair2D id = new Pair2D(new Vector2D(0, 0), new Vector2D(0, 0));
	public bool IsClockwise() {
		if (pointsList.Count < 1) {
			return (true);
		}
		
		double sum = 0;

		id.A = pointsList[pointsList.Count - 1];
		id.B = null;

		for(int i = 0; i < pointsList.Count; i++) {
			id.B = pointsList[i];

			sum += (id.B.x - id.A.x) * (id.B.y + id.A.y);

			id.A = id.B;
		}

		return(sum > 0);
	}

	public void Normalize() {
		if (IsClockwise () == false) {
			pointsList.Reverse ();
		}

		foreach (Polygon2D p in holesList) {
			p.Normalize ();
		}
	}

	public double GetArea() {
		double area = 0f;
		foreach (Pair2D id in Pair2D.GetList(pointsList)) {
			area += ((id.B.x - id.A.x) * (id.B.y + id.A.y)) / 2.0f;
		}

		foreach (Polygon2D p in holesList) {
			area -= p.GetArea ();
		}

		return(System.Math.Abs(area)); 
	}


	public Rect GetRect() {	
		Rect rect = new Rect();

		float minX = 100000;
		float minY = 100000;
		float maxX = -100000;
		float maxY = -100000;

		foreach (Vector2D id in pointsList) {
			minX = Mathf.Min(minX, (float)id.x);
			minY = Mathf.Min(minY, (float)id.y);
			maxX = Mathf.Max(maxX, (float)id.x);
			maxY = Mathf.Max(maxY, (float)id.y);
		}

		rect.x = minX;
		rect.y = minY;
		rect.width = maxX - minX;
		rect.height = maxY - minY;

		return(rect);
	}




 
	public List<Polygon2D> LineIntersectHoles(Pair2D pair) {
		List<Polygon2D> resultList = new List<Polygon2D>();
		foreach (Polygon2D poly in holesList) {
			if (Math2D.LineIntersectPoly(pair, poly) == true) {
				resultList.Add (poly);
			}
		}

		return(resultList);
	}

	public bool SliceIntersectPoly(List <Vector2D> slice) {
		if (Math2D.SliceIntersectPoly (slice, this)) {
			return(true);
		}
		
		foreach (Polygon2D poly in holesList) {
			if (Math2D.SliceIntersectPoly (slice, poly)) {
				return(true);
			}
		}

		return(false);
	}
		
	public List<Polygon2D> SliceIntersectHoles(List <Vector2D> slice) {
		List<Polygon2D> resultList = new List<Polygon2D> ();
		foreach (Polygon2D poly in holesList) {
			if (Math2D.SliceIntersectPoly(slice, poly) == true) {
				resultList.Add (poly);
			}
		}

		return(resultList);
	}

	public List<Vector2D> GetListLineIntersectPoly(Pair2D line) {
		List<Vector2D> intersections = Math2D.GetListLineIntersectPoly(line, this);

		foreach (Polygon2D poly in holesList) {
			foreach (Vector2D p in Math2D.GetListLineIntersectPoly(line, poly)) {
				intersections.Add (p);
			}
		}
		
		return(intersections);
	}



	///// Constructors - Polygon Creating //////

	static public Polygon2D CreateRect(Vector2 size) {
		size = size / 2;
		
		Polygon2D polygon = new Polygon2D();
		polygon.pointsList.Add(new Vector2D(-size.x, -size.y));
		polygon.pointsList.Add(new Vector2D(size.x, -size.y));
		polygon.pointsList.Add(new Vector2D(size.x, size.y));
		polygon.pointsList.Add(new Vector2D(-size.x, size.y));
		polygon.Normalize();
		return(polygon);
	}

	static public Polygon2D CreateIsometric(Vector2 size) {
		size = size / 2;
		
		Polygon2D polygon = new Polygon2D();
		polygon.pointsList.Add(new Vector2D(-size.x, size.y ));
		polygon.pointsList.Add(new Vector2D(0, 0 ));
		polygon.pointsList.Add(new Vector2D(size.x,  size.y));
		polygon.pointsList.Add(new Vector2D(0, size.y * 2));
		polygon.Normalize();
		return(polygon);
	}

	static public Polygon2D CreateHexagon(Vector2 size) {
		size = size / 2;

		Polygon2D polygon = new Polygon2D();
		polygon.pointsList.Add(new Vector2D(-size.x, size.y));

		polygon.pointsList.Add(new Vector2D(-size.x, -size.y));
		polygon.pointsList.Add(new Vector2D(0, -size.y * 2));
		polygon.pointsList.Add(new Vector2D(size.x, -size.y));

		polygon.pointsList.Add(new Vector2D(size.x,  size.y));
		polygon.pointsList.Add(new Vector2D(0, size.y * 2));
		polygon.Normalize();
		return(polygon);
	}

	///// Mesh Creating

	public Mesh CreateMesh(GameObject gameObject, Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced) {		
		if (gameObject.GetComponent<MeshRenderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
		}

		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		if (filter == null) {
			filter = gameObject.AddComponent<MeshFilter>() as MeshFilter;
		}
		
		filter.sharedMesh = PolygonTriangulator2D.Triangulate (this, UVScale, UVOffset, triangulation);
		if (filter.sharedMesh == null) {
			UnityEngine.Object.Destroy(gameObject);
		}

		return(filter.sharedMesh);
	}
	
	public Mesh CreateMesh(Vector2 UVScale, Vector2 UVOffset, PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced) {        
        return(PolygonTriangulator2D.Triangulate (this, UVScale, UVOffset, triangulation));
    }

	///// Collider Creating /////

	public PolygonCollider2D CreatePolygonCollider(GameObject gameObject) {
		PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D> ();

		if (collider == null) {
			collider = gameObject.AddComponent<PolygonCollider2D> ();
		}

		List<Vector2> points = new List<Vector2> ();

		foreach (Vector2D p in pointsList) {
			points.Add(p.ToVector2());
		}

		collider.pathCount = (1 + holesList.Count);

		collider.enabled = false;

		collider.SetPath(0, points.ToArray());

		if (holesList.Count > 0) {
			int pathID = 1;
			List<Vector2> pointList = null;

			foreach (Polygon2D poly in holesList) {
				pointList = new List<Vector2> ();

				foreach (Vector2D p in poly.pointsList) {
					pointList.Add (p.ToVector2());
				}

				collider.SetPath (pathID, pointList.ToArray ());
				pathID += 1;
			}
		}

		collider.enabled = true;

		return(collider);
	}

	public EdgeCollider2D CreateEdgeCollider(GameObject gameObject) {
		EdgeCollider2D collider = gameObject.GetComponent<EdgeCollider2D> ();

		if (collider == null) {
			collider = gameObject.AddComponent<EdgeCollider2D> ();
		}

		List<Vector2> points = new List<Vector2> ();

		foreach (Vector2D p in pointsList) {
			points.Add(p.ToVector2());
		}

		collider.points = points.ToArray();

		return(collider);
	}

	// Sprite To Mesh

	public static void SpriteToMesh(GameObject gameObject, VirtualSpriteRenderer spriteRenderer, PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced) {
		Texture2D texture = null;
		Sprite sprite = null;

		if (spriteRenderer.sprite != null) {
			sprite = spriteRenderer.sprite;
			texture = sprite.texture;
		}

		Rect spriteRect = sprite.textureRect;
		
		float spriteSheetU = (float)(texture.width) / spriteRect.width;
		float spriteSheetV = (float)(texture.height) / spriteRect.height;

	
		Rect uvRect = new Rect((float)spriteRect.x / texture.width, (float)spriteRect.y / texture.height, (float)spriteRect.width / texture.width, (float)spriteRect.height / texture.height);

		Vector2 scale = new Vector2(spriteSheetU * spriteRect.width / sprite.pixelsPerUnit, spriteSheetV * spriteRect.height / spriteRenderer.sprite.pixelsPerUnit);
		
		if (spriteRenderer.flipX) {
			scale.x = -scale.x;
		}

		if (spriteRenderer.flipY) {
			scale.y = -scale.y;
		}

		float pivotX = sprite.pivot.x / spriteRect.width - 0.5f;
		float pivotY = sprite.pivot.y / spriteRect.height - 0.5f;

		float ix = -0.5f + pivotX / spriteSheetU;
		float iy = -0.5f + pivotY / spriteSheetV;

		Vector2 uvOffset = new Vector2(uvRect.center.x + ix, uvRect.center.y + iy);
		
		Polygon2D polygon2D = Polygon2DListCollider2D.CreateFromGameObject (gameObject)[0];
		polygon2D.CreateMesh (gameObject, scale, uvOffset, triangulation);

		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
		if (meshRenderer == null) {
			meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		}
		
		meshRenderer.sharedMaterial = spriteRenderer.material;
		meshRenderer.sharedMaterial.mainTexture = texture;
		meshRenderer.sharedMaterial.color = spriteRenderer.color;

		//meshRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
		//meshRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
		//meshRenderer.sortingOrder = spriteRenderer.sortingOrder;
	}

	///// Main Operators /////

	// HOLES MISSING??????
	public Polygon2D Copy() {
		Polygon2D newPolygon = new Polygon2D();

		for(int id = 0; id < pointsList.Count; id++) {
			newPolygon.pointsList.Add (new Vector2D(pointsList[id].x, pointsList[id].y));
		}

		return(newPolygon);
	}

	///// Slow Operators /////
	
	public Polygon2D ToLocalSpace(Transform transform) {
		Polygon2D newPolygon = new Polygon2D();

		for(int id = 0; id < pointsList.Count; id++) {
			newPolygon.AddPoint (transform.InverseTransformPoint (pointsList[id].ToVector2()));
		}

		for(int id = 0; id < holesList.Count; id++) {
			newPolygon.AddHole (holesList[id].ToLocalSpace (transform));
		}

		return(newPolygon);
	}

	public Polygon2D ToWorldSpace(Transform transform) {
		Polygon2D newPolygon = new Polygon2D();

		for(int id = 0; id < pointsList.Count; id++) {
			newPolygon.AddPoint (transform.TransformPoint (pointsList[id].ToVector2()));
		}

		for(int id = 0; id < holesList.Count; id++) {
			newPolygon.AddHole (holesList[id].ToWorldSpace (transform));
		}

		return(newPolygon);
	}

	public Polygon2D ToOffset(Vector2D pos) {
		Polygon2D newPolygon = new Polygon2D (pointsList);
		
		for(int id = 0; id < newPolygon.pointsList.Count; id++) {
			newPolygon.pointsList[id].Inc (pos);
		}

		for(int id = 0; id < holesList.Count; id++) {
			newPolygon.AddHole (holesList[id].ToOffset(pos));
		}

		return(newPolygon);
	}

	// Does not include holes????????????
	public Polygon2D ToScale(Vector2 scale, Vector2D center = null) {
		Polygon2D newPolygon = new Polygon2D();
		
		if (center == null) {
			center = new Vector2D(Vector2.zero);
		}

		float dist, rot;
		Vector2D point;
		for(int id = 0; id < pointsList.Count; id++) {
			point = pointsList[id];
			dist = (float)Mathd.Distance(point.x, point.y, center.x, center.y);
			rot = (float)System.Math.Atan2 (point.y - center.y, point.x - center.x);
			newPolygon.AddPoint((float)center.x + Mathf.Cos(rot) * dist * scale.x, (float)center.y + Mathf.Sin(rot) * dist * scale.y);
		}

		return(newPolygon);
	}
	
	// Does not include holes???
	public Polygon2D ToRotation(float rotation, Vector2D center = null) {
		Polygon2D newPolygon = new Polygon2D();
		
		if (center == null) {
			center = new Vector2D(Vector2.zero);
		}

		float dist, rot;
		Vector2D point;

		for(int id = 0; id < pointsList.Count; id++) {
			point = pointsList[id];
			dist = (float)Mathd.Distance(point.x, point.y, center.x, center.y);
			rot = (float)System.Math.Atan2 (point.y - center.y, point.x - center.x) + rotation;
			newPolygon.AddPoint((float)center.x + Mathf.Cos(rot) * dist, (float)center.y + Mathf.Sin(rot) * dist);
		}

		return(newPolygon);
	}

	///// Fast Operators /////
	static Vector2D zero = Vector2D.Zero();
	
	public void ToScaleItself(Vector2 scale, Vector2D center = null) {
		if (center == null) {
			center = zero;
		}

		float dist, rot;
		Vector2D point;

		for(int id = 0; id < pointsList.Count; id++) {
			point = pointsList[id];
			dist = (float)Mathd.Distance(point.x, point.y, center.x, center.y);
			rot = (float)System.Math.Atan2 (point.y - center.y, point.x - center.x);
			point.x = center.x + Mathf.Cos(rot) * dist * scale.x;
			point.y = center.y + Mathf.Sin(rot) * dist * scale.y;
		}
	}

	public void ToRotationItself(float rotation, Vector2D center = null) {
		if (center == null) {
			center = zero;
		}

		float dist, rot;
		Vector2D point;

		for(int id = 0; id < pointsList.Count; id++) {
			point = pointsList[id];
			dist = (float)Mathd.Distance(point.x, point.y, center.x, center.y);
			rot = (float)System.Math.Atan2 (point.y - center.y, point.x - center.x) + rotation;

			point.x = (float)center.x + Mathf.Cos(rot) * dist;
			point.y = (float)center.y + Mathf.Sin(rot) * dist;
		}
	}

	public void ToOffsetItself(Vector2 pos) {
		for(int id = 0; id < pointsList.Count; id++) {
			pointsList[id].x += pos.x;
			pointsList[id].y += pos.y;
		}

		for(int id = 0; id < holesList.Count; id++) {
			holesList[id].ToOffsetItself(pos);
		}
	}
	//

	
	public void ToWorldSpaceItselfUNIVERSAL(Transform transform) {
		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				ToWorldSpaceItself(transform);
			break;

			case CoreAxis.XYFLIPPED:
				ToWorldSpaceItselfFlipped(transform);
			break;

			case CoreAxis.XZFLIPPED:
				ToWorldSpaceItselfXZFlipped(transform);
			break;

			case CoreAxis.XZ:
				ToWorldSpaceItselfXZ(transform);
			break;	
		}
	}

	public void ToWorldSpaceItself(Transform transform) {
		int count = pointsList.Count;
		
		for(int id = 0; id < count; id++) {
			pointsList[id].TransformToWorldXY(transform);
		}

		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItself(transform);
			}
		}
	}

	public void ToWorldSpaceItselfFlipped(Transform transform) {
		int count = pointsList.Count;
		
		for(int id = 0; id < count; id++) {
			pointsList[id].TransformToWorldXYFlipped(transform);
		}

		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfFlipped(transform);
			}
		}
	}

	public void ToWorldSpaceItselfXZ(Transform transform) {
		int count = pointsList.Count;
		
		for(int id = 0; id < count; id++) {
			pointsList[id].TransformToWorldXZ(transform);
		}

		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfXZ(transform);
			}
		}
	}

	public void ToWorldSpaceItselfXZFlipped(Transform transform) {
		int count = pointsList.Count;
		
		for(int id = 0; id < count; id++) {
			pointsList[id].TransformToWorldXZFlipped(transform);
		}

		count = holesList.Count;

		if (count > 0) {
			for(int id = 0; id < holesList.Count; id++) {
				holesList[id].ToWorldSpaceItselfXZFlipped(transform);
			}
		}
	}
}
