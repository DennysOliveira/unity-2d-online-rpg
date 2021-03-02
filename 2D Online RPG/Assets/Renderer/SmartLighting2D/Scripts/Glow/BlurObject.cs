using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Glow name space
[System.Serializable]
public class GlowObject {
	public Sprite sprite;
	public int glowSize;
	public int glowIterations;

	public GlowObject(Sprite image, int size, int iterations) {
		sprite = image;
		glowSize = size;
		glowIterations = iterations;
	}
}