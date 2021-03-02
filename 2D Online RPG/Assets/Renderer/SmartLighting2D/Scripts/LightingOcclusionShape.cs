using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightShape;

[System.Serializable]
public class LightingOcclusionShape {
    public LightOcclusion2D.ShadowType shadowType = LightOcclusion2D.ShadowType.Collider;

	public Collider2DShape colliderShape = new Collider2DShape();
    public SpritePhysicsShape spritePhysicsShape = new SpritePhysicsShape();

	public Transform transform;
	
	public void SetTransform(Transform t) {
		transform = t.transform;

		colliderShape.SetTransform(t);

        spritePhysicsShape.SetTransform(t);
	}

	public void ResetLocal() {
		colliderShape.ResetLocal();

        spritePhysicsShape.ResetLocal();

		ResetWorld();
	}

	public void ResetWorld() {
		colliderShape.ResetWorld();

        spritePhysicsShape.ResetWorld();
	}

	public bool IsEdgeCollider() {
		switch(shadowType) {
			case LightOcclusion2D.ShadowType.Collider:
				return(colliderShape.edgeCollider2D);
		}
		
		return(false);
	}

	public List<MeshObject> GetMeshes() {
		switch(shadowType) {
			case LightOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetMeshes());

           case LightOcclusion2D.ShadowType.SpritePhysicsShape:
				return(spritePhysicsShape.GetMeshes());
		}
	
		return(null);
	}

	public List<Polygon2> GetPolygonsLocal() {
		switch(shadowType) {
			case LightOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsLocal());

            case LightOcclusion2D.ShadowType.SpritePhysicsShape:
                return(spritePhysicsShape.GetPolygonsLocal());
		}
		return(null);
	}

	public List<Polygon2> GetPolygonsWorld() {
		switch(shadowType) {
			case LightOcclusion2D.ShadowType.Collider:
				return(colliderShape.GetPolygonsWorld());

            case LightOcclusion2D.ShadowType.SpritePhysicsShape:
                return(spritePhysicsShape.GetPolygonsWorld());
		}
		return(null);
	}
}