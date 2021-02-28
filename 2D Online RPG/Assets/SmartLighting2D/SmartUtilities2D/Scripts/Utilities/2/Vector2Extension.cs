using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{

	public static float Atan2(this Vector2 a) {
		return(Mathf.Atan2 (a.y, a.x));
	}

    public static float Atan2(this Vector2 a, Vector2 b) {
		return(Mathf.Atan2 (a.y - b.y, a.x - b.x));
	}

    public static Vector2 Push(this Vector2 a, float direction, float distance) {
		a.x += Mathf.Cos(direction) * distance;
		a.y += Mathf.Sin(direction) * distance;

        return(a);
	}

	public static Vector2 RotToVec(this Vector2 a,float rotation, float distance) {
		a.x = Mathf.Cos(rotation) * distance;
		a.y = Mathf.Sin(rotation) * distance;

		return(a);
	}
	
	public static Vector2 RotToVec(this Vector2 a, float rotation) {
		a.x = Mathf.Cos(rotation);
		a.y = Mathf.Sin(rotation);

		return(a);
	}


	public static Vector2 TransformToWorldXY(this Vector2 a, Transform transform) {
		float sx = transform.lossyScale.x;
		float sy = transform.lossyScale.y;

		float dist = Mathf.Sqrt(a.x * a.x * sx * sx + a.y * a.y * sy* sy);
		float angle = Mathf.Atan2(a.y * sy, a.x * sx);
		angle += transform.eulerAngles.z * Mathf.Deg2Rad;

		a.x = Mathf.Cos(angle) * dist;
		a.y = Mathf.Sin(angle) * dist;

		a.x = a.x  + transform.position.x;
		a.y = a.y  + transform.position.y;

		return(a);
	}

	public static Vector2 TransformToWorldXYFlipped(this Vector2 a, Transform transform) {
		float dist = Mathf.Sqrt(a.x * a.x + a.y * a.y);
		float angle = Mathf.Atan2(a.y, a.x);
		angle += transform.eulerAngles.z * Mathf.Deg2Rad;

		a.x = Mathf.Cos(angle) * dist;
		a.y = Mathf.Sin(angle) * dist;

		a.x = -(a.x * transform.lossyScale.x + transform.position.x);
		a.y = a.y * transform.lossyScale.x + transform.position.y;

		return(a);
	}

	public static Vector2 TransformToWorldXZFlipped(this Vector2 a, Transform transform) {
		float dist = Mathf.Sqrt(a.x * a.x + a.y * a.y);
		float angle = Mathf.Atan2(a.y, a.x);
		angle -= transform.eulerAngles.y * Mathf.Deg2Rad;

		a.x = Mathf.Cos(angle) * dist;
		a.y = Mathf.Sin(angle) * dist;

		a.x = a.x * transform.lossyScale.x + transform.position.x;
		a.y = a.y * transform.lossyScale.z + transform.position.z;

		return(a);
	}

	public static Vector2 TransformToWorldXZ(this Vector2 a, Transform transform) {
		float dist = Mathf.Sqrt(a.x * a.x + a.y * a.y);
		float angle = Mathf.Atan2(a.y, a.x);
		angle -= transform.eulerAngles.y * Mathf.Deg2Rad;

		a.x = Mathf.Cos(angle) * dist;
		a.y = Mathf.Sin(angle) * dist;

		a.x = a.x * transform.lossyScale.x + transform.position.x;
		a.y = -(a.y * transform.lossyScale.z + transform.position.z);

		return(a);
	}

}