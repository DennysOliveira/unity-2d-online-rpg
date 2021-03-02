using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Polygon2Helper {

 	static public Rect GetRect(List<Polygon2> polygons) {	
		Rect rect = new Rect();

		if (polygons == null) {
			return(rect);
		}

		if (polygons.Count > 0) {
			float minX = 100000;
			float minY = 100000;
			float maxX = -100000;
			float maxY = -100000;

			foreach(Polygon2 poly in polygons) {

				int pointsCount = poly.points.Length;

				for(int i = 0; i < pointsCount; i++) {
					
					Vector2 id = poly.points[i];
	
					minX = Mathf.Min(minX, id.x);
					minY = Mathf.Min(minY, id.y);
					maxX = Mathf.Max(maxX, id.x);
					maxY = Mathf.Max(maxY, id.y);
				}

			}
		
			rect.x = minX;
			rect.y = minY;
			rect.width = maxX - minX;
			rect.height = maxY - minY;
		}

		return(rect);
	}

	static public Rect GetIsoRect(List<Polygon2> polygons) {	
		Rect rect = new Rect();

		if (polygons == null) {
			return(rect);
		}

		if (polygons.Count > 0) {
			float minX = 100000;
			float minY = 100000;
			float maxX = -100000;
			float maxY = -100000;

			foreach(Polygon2 poly in polygons) {

				int pointsCount = poly.points.Length;

				for(int i = 0; i < pointsCount; i++) {
					
					Vector2 id = poly.points[i];

					float x = id.y + id.x / 2;
					float y = id.y - id.x / 2;
	
					minX = Mathf.Min(minX, x);
					minY = Mathf.Min(minY, y);
					maxX = Mathf.Max(maxX, x);
					maxY = Mathf.Max(maxY, y);
				}

			}
		
			rect.x = minX;
			rect.y = minY;
			rect.width = maxX - minX;
			rect.height = maxY - minY;
		}

		return(rect);
	}
    
}