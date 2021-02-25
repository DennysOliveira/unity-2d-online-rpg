using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightRoom2D : MonoBehaviour {
	public enum RoomType {Collider, Sprite};

	public int nightLayer = 0;
	public Color color = Color.black;

	public LightingRoomShape shape = new LightingRoomShape();

	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public static List<LightRoom2D> List = new List<LightRoom2D>();

	public void OnEnable() {
		List.Add(this);

		LightingManager2D.Get();

		shape.SetTransform(transform);
	}

	public void OnDisable() {
		List.Remove(this);
	}
	
	public void Awake() {
		Initialize();
	}

	public void Initialize() {
		shape.ResetLocal();
	}
}