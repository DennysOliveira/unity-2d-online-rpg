using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMeshObject {
   	SpriteTiledMesh spriteTiledMesh = null;

    public SpriteTiledMesh GetTiledMesh() {
		if (spriteTiledMesh == null) {
			spriteTiledMesh = new SpriteTiledMesh();
		}

		return(spriteTiledMesh);
	}

	/*
	private Mesh rectMesh = null;

	public Mesh GetRectMesh(SpriteTransform spriteTransform) {
		
		if (rectMesh == null) {
			Vector2[] uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

			uv[0].x = spriteTransform.uv.x;
			uv[0].y = spriteTransform.uv.y;

			uv[1].x = spriteTransform.uv.width;
			uv[1].y = spriteTransform.uv.y;

			uv[2].x = spriteTransform.uv.width;
			uv[2].y = spriteTransform.uv.height;

			uv[3].x = spriteTransform.uv.x;
			uv[3].y = spriteTransform.uv.height;

			rectMesh = new Mesh();

			rectMesh.vertices = new Vector3[]{new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1)};
			rectMesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
			rectMesh.uv = uv;
		}
		
		return(rectMesh);
	}*/
}
