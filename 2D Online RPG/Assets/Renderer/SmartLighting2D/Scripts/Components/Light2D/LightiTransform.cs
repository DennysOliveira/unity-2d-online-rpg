using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTransform {

	private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

	public Vector2 position = Vector2.zero;
	public float rotation = 0f;
	private float size = 0f;
	private float spotAngle = 360;
	private float outerAngle = 15;

	private float maskTranslucency = 1;

	private Color color = Color.white;

	private Sprite sprite;
	private bool flipX = false;
	private bool flipY = false;

	private float normalIntensity = 1;
	private float normalDepth = 1;

	public void ForceUpdate() {
		update = true;
	}

	public void Update(Light2D source) {
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

		if (flipX != source.spriteFlipX) {
			flipX = source.spriteFlipX;

			update = true;
		}

		if (flipY != source.spriteFlipY) {
			flipY = source.spriteFlipY;

			update = true;
		}

		if (spotAngle != source.spotAngle) {
			spotAngle = source.spotAngle;

			update = true;
		}
		
		if (outerAngle != source.outerAngle) {
			outerAngle = source.outerAngle;

			update = true;
		}
		
		if (normalIntensity != source.bumpMap.intensity) {
			normalIntensity = source.bumpMap.intensity;

			update = true;
		}

		if (normalDepth != source.bumpMap.depth) {
			normalDepth = source.bumpMap.depth;

			update = true;
		}

		if (maskTranslucency != source.maskTranslucency) {
			maskTranslucency = source.maskTranslucency;

			update = true;
		}

		// No need to update for Color and Alpha
		if (color != source.color) {
			color = source.color;
		}
	}
}