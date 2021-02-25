using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UITextScale : MonoBehaviour {
	public float ratio = 10f;
	public Rect rect = new Rect(0, 0, 100, 100);

	private Text text;
	private RectTransform rectTransform;

	void Start () {
		text = GetComponent<Text>();
		rectTransform = GetComponent<RectTransform>();
		Update ();
	}
	
	void Update () {
		text.fontSize = (int)(Screen.height * (ratio / 100f));
		rectTransform.anchorMin = rect.min / 100;
		rectTransform.anchorMax = rect.max / 100;
	}
}
