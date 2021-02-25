using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightShape;

[System.Serializable]
public class LightingRoomShape {
    public LightRoom2D.RoomType type = LightRoom2D.RoomType.Collider;

    public Collider2DShape colliderShape = new Collider2DShape();
	public SpriteShape spriteShape = new SpriteShape();

    public void SetTransform(Transform t) {
		colliderShape.SetTransform(t);
		spriteShape.SetTransform(t);
	}

    public void ResetLocal() {
        colliderShape.ResetLocal();

		spriteShape.ResetLocal();
	}

	public void ResetWorld() {
		colliderShape.ResetWorld();

		colliderShape.ResetWorld();
	}

    public List<MeshObject> GetMeshes() {
		switch(type) {
			case LightRoom2D.RoomType.Collider:
				return(colliderShape.GetMeshes());

		}
	
		return(null);
	}

}
