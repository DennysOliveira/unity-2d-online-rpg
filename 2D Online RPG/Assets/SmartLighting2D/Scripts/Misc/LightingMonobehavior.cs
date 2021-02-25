using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingMonoBehaviour : MonoBehaviour
{
   	public void DestroySelf() {
		if (Application.isPlaying) {
			Destroy(this.gameObject);
		} else {
			if (this != null && this.gameObject != null) {
				DestroyImmediate(this.gameObject);
			}
			
		}
	}
}
