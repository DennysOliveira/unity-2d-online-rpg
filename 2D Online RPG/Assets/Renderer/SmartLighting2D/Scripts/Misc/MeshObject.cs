using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshObject {
	public Mesh mesh;
	public Vector3[] vertices;
	public Vector2[] uv;
	public int[] triangles;

	public static MeshObject Get(Mesh meshOrigin) {
		if (meshOrigin.isReadable) {
			MeshObject meshObject = new MeshObject();
			meshObject.vertices = meshOrigin.vertices;
			meshObject.uv = meshOrigin.uv;
			meshObject.triangles = meshOrigin.triangles;
			meshObject.mesh = meshOrigin;

			return(meshObject);
		}
		
		return(null);
	}
}