using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightMeshTransform {

	private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

	public Vector2 position = Vector2.zero;
	public float rotation = 0f;
	private float size = 0f;

	private Color color = Color.white;

	private Sprite sprite;

	public void ForceUpdate() {
		update = true;
	}

	public void Update(LightMesh2D source) {
		Transform transform = source.transform;

		Vector2 position2D = LightingPosition.GetPosition2D(transform.position);

		float rotation2D = transform.rotation.eulerAngles.z;

		if (position != position2D) {
			position = position2D;

			update = true;
		}

		if (rotation != rotation2D) {
			rotation = rotation2D;

			update = true;
		}

		if (size != source.size) {
			size = source.size;

			update = true;
		}

		if (sprite != source.sprite) {
			sprite = source.sprite;

			update = true;
		}

		// No need to update for Color and Alpha
		if (color != source.color) {
			color = source.color;
		}
	}
}