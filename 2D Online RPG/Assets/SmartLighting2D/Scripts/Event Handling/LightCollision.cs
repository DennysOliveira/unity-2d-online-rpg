using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

public struct LightCollision2D {

	public Light2D light;
	public LightCollider2D collider;

	public List<Vector2> points;
	public LightEventState state;

	public LightCollision2D(bool _active) {
		light = null;

		collider = null;
		
		points = null;

		state = LightEventState.None;
	}
}