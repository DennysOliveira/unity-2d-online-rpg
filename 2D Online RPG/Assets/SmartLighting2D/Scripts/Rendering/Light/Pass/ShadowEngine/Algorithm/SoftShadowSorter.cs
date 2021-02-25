using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoftShadowSorter {

	public static Polygon2 polygon;

	public static Light2D light;

	public static Vector2 center;

	public static float[] direction = new float[1000];

	public static Vector2 minPoint;
	public static Vector2 maxPoint;

	public static void Set(Polygon2 poly, Light2D light2D) {
		polygon = poly;
		
		light = light2D;

		Vector2 lightPosition = -light.transform2D.position;

		center.x = 0;
		center.y = 0;

		foreach(Vector2 p in polygon.points) {
			center.x += p.x + lightPosition.x;
			center.y += p.y + lightPosition.y;
		}

		center.x /= polygon.points.Length;
		center.y /= polygon.points.Length;

		float centerDirection = Mathf.Atan2(center.x, center.y) * Mathf.Rad2Deg;

		centerDirection = (centerDirection + 720) % 360 + 180;

	
		for(int id = 0; id < polygon.points.Length; id++) {
			Vector2 p = polygon.points[id];

			float dir = Mathf.Atan2((float)p.x + lightPosition.x, (float)p.y + lightPosition.y) * Mathf.Rad2Deg;
			dir = (dir + 720 - centerDirection) % 360;
			
			direction[id] = dir;
		}


		float min = 10000;
		float max = -10000;
	
		for(int id = 0; id < polygon.points.Length; id++) {
			Vector2 p = polygon.points[id];

			if (direction[id] < min) {
				min = direction[id];
				minPoint = p;
			}

			if (direction[id] > max) {
				max = direction[id];
				maxPoint = p;
			}
		}
	}
}