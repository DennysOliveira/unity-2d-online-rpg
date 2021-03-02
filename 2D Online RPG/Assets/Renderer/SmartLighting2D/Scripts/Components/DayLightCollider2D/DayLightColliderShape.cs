using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DayLighting;
using LightShape;

[System.Serializable]
public class DayLightColliderShape {
	public DayLightCollider2D.ShadowType shadowType = DayLightCollider2D.ShadowType.SpritePhysicsShape;
    
	public DayLightCollider2D.MaskType maskType = DayLightCollider2D.MaskType.Sprite;
    
	public Transform transform;
   
    public DayLightingColliderTransform transform2D = new DayLightingColliderTransform();

    public SpriteShape spriteShape = new SpriteShape();
    public SpritePhysicsShape spritePhysicsShape = new SpritePhysicsShape();
	public Collider2DShape colliderShape = new Collider2DShape();

    public float height = 1;
    public ShadowMesh shadowMesh = new ShadowMesh();

    public void SetTransform(Transform t) {
        transform = t;

        transform2D.SetShape(this);

        spriteShape.SetTransform(t);
        spritePhysicsShape.SetTransform(t);
		
		colliderShape.SetTransform(t);
    }

    public void ResetLocal() {
		spriteShape.ResetLocal();
		spritePhysicsShape.ResetLocal();

		colliderShape.ResetLocal();
	}

    public void ResetWorld() {
		spritePhysicsShape.ResetWorld();

		colliderShape.ResetWorld();
	}

	public List<Polygon2> GetPolygonsLocal() {
		switch(shadowType) {
			case DayLightCollider2D.ShadowType.SpritePhysicsShape:
				return(spritePhysicsShape.GetPolygonsLocal());

			case DayLightCollider2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsLocal());

		}

		return(null);
	}

    public List<Polygon2> GetPolygonsWorld() {
		switch(shadowType) {
			case DayLightCollider2D.ShadowType.SpritePhysicsShape:
				return(spritePhysicsShape.GetPolygonsWorld());

			case DayLightCollider2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsWorld());
		}

		return(null);
	}
}
