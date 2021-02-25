using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UICanvasScale : MonoBehaviour {
	public bool screenRatioScale = false;
	public Rect rect = new Rect(0, 0, 100, 100);
	
	private RectTransform rectTransform;

	void Start () {
		rectTransform = GetComponent<RectTransform>();
		Update ();
	}
	
	void Update () {
		if (screenRatioScale) {
			Rect tempRect = rect;
		
			tempRect.height *= ((float)Screen.width) / Screen.height;
			tempRect.y -= tempRect.height / 4;

			rectTransform.anchorMin = tempRect.min / 100;
			rectTransform.anchorMax = tempRect.max / 100;
		} else {
			rectTransform.anchorMin = rect.min / 100;
			rectTransform.anchorMax = rect.max / 100;
		}
	}
}
