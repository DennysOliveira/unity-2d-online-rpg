using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mesh2DSubmesh {
	public Vector2[] uv;
	public Vector3[] vertices;

	public Mesh2DSubmesh(int size) {
		uv = new Vector2[size];
		vertices = new Vector3[size];
	}
}

public class Mesh2DMesh {
	public List<Mesh2DSubmesh> submeshes = new List<Mesh2DSubmesh>();
	public int verticesCount = 0;
	
	public void Add(Mesh2DSubmesh m) {
		submeshes.Add(m);
		verticesCount += m.vertices.Length;
	}
}

public class Max2DMesh {
	const float pi = Mathf.PI;
	const float pi2 = pi / 2;
	const float uv0 = 1f / 32;
	const float uv1 = 1f - uv0;

	static Vector2D A1 = Vector2D.Zero();
	static Vector2D A2 = Vector2D.Zero();
	static Vector2D B1 = Vector2D.Zero();
	static Vector2D B2 = Vector2D.Zero();

	static Vector2D A3 = Vector2D.Zero();
	static Vector2D A4 = Vector2D.Zero();

	private static Pair2D pair2D = Pair2D.Zero();

	static public void Draw(Mesh mesh, Transform transform, Material material) {
		Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

		Graphics.DrawMesh(mesh, matrix, material, 0);
	}

	static public void Draw(Mesh mesh, Material material) {
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

		Graphics.DrawMesh(mesh, matrix, material, 0);
	}

	static public Mesh CreatePolygon(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
		Mesh2DMesh trianglesList = new Mesh2DMesh();

		int count = polygon.pointsList.Count;
		int lastID = count - 1;
		int startID = 0;
		
		if (connectedLine == false) {
			lastID = 0;
			startID = 1;
		}
		
		Pair2D p = pair2D;
		p.A = polygon.pointsList[lastID];
		
		for(int i = startID; i < count; i++) {
			p.B = polygon.pointsList[i];

			trianglesList.Add(Max2DMesh.CreateLine(p, transform.localScale, lineWidth, lineOffset));

			p.A = p.B;
		}
		
		foreach(Polygon2D hole in polygon.holesList) {
			count = hole.pointsList.Count;
			lastID = count - 1;
			startID = 0;
		
			if (connectedLine == false) {
				lastID = 0;
				startID = 1;
			}

			p.A = hole.pointsList[lastID];
			
			for(int i = startID; i < count; i++) {
				p.B = hole.pointsList[i];

				trianglesList.Add(Max2DMesh.CreateLine(p, transform.localScale, lineWidth, lineOffset));

				p.A = p.B;
			}
		} 
		
		return(Max2DMesh.Export(trianglesList));
	}

	static public Mesh2DSubmesh CreateLine(Pair2D pair, Vector3 transformScale, float lineWidth, float z = 0f) {
		Mesh2DSubmesh result = new Mesh2DSubmesh(18);

		float xuv0 = 0; 
		float xuv1 = 1f - xuv0;
		float yuv0 = 0;
		float yuv1 = 1f - yuv0;

		float size = lineWidth / 6;
		float rot = (float)Vector2D.Atan2 (pair.A, pair.B);

		A1.x = pair.A.x;
		A1.y = pair.A.y;

		A2.x = pair.A.x;
		A2.y = pair.A.y;

		B1.x = pair.B.x;
		B1.y = pair.B.y;

		B2.x = pair.B.x;
		B2.y = pair.B.y;

		Vector2 scale = new Vector2(1f / transformScale.x, 1f / transformScale.y);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);
		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		result.vertices[0] = new Vector3((float)B1.x, (float)B1.y, z);
		result.vertices[1] = new Vector3((float)A1.x, (float)A1.y, z);
		result.vertices[2] = new Vector3((float)A2.x, (float)A2.y, z);
		
		result.vertices[3] = new Vector3((float)A2.x, (float)A2.y, z);
		result.vertices[4] = new Vector3((float)B2.x, (float)B2.y, z);
		result.vertices[5] = new Vector3((float)B1.x, (float)B1.y, z);

		result.uv[0] = new Vector2(xuv1 / 3, yuv1); 
		result.uv[1] = new Vector2(1 - xuv1 / 3, yuv1);
		result.uv[2] = new Vector2(1 - xuv1 / 3, yuv0);
		
		result.uv[3] = new Vector2(1 - xuv1 / 3, yuv0);
		result.uv[4] = new Vector2(yuv1 / 3, xuv0);
		result.uv[5] = new Vector2(xuv1 / 3, yuv1);

		A3.x = A1.x;
		A3.y = A1.y;

		A4.x = A1.x;
		A4.y = A1.y;
	
		A3.Push (rot - pi2, size, scale);

		A3.x = A1.x;
		A3.y = A1.y;
		
		A4.x = A2.x;
		A4.y = A2.y;

		A1.Push (rot, size, scale);
		A2.Push (rot, size, scale);

		result.vertices[6] = new Vector3((float)A3.x, (float)A3.y, z);
		result.vertices[7] = new Vector3((float)A1.x, (float)A1.y, z);
		result.vertices[8] = new Vector3((float)A2.x, (float)A2.y, z);
		
		result.vertices[9] = new Vector3((float)A2.x, (float)A2.y, z);
		result.vertices[10] = new Vector3((float)A4.x, (float)A4.y, z);
		result.vertices[11] = new Vector3((float)A3.x, (float)A3.y, z);
		
		result.uv[6] = new Vector2(xuv1 / 3, yuv1); 
		result.uv[7] = new Vector2(xuv0, yuv1);
		result.uv[8] = new Vector2(xuv0, yuv0);

	 	result.uv[9] = new Vector2(xuv0, yuv0);
		result.uv[10] = new Vector2(yuv1 / 3, xuv0);
		result.uv[11] = new Vector2(xuv1 / 3, yuv1);

		A1.x = B1.x;
		A1.y = B1.y;

		A2.x = B2.x;
		A2.y = B2.y;

		B1.Push (rot - Mathf.PI, size, scale);
		B2.Push (rot - Mathf.PI, size, scale);
		
		result.vertices[12] = new Vector3((float)B1.x, (float)B1.y, z);
		result.vertices[13] = new Vector3((float)A1.x, (float)A1.y, z);
		result.vertices[14] = new Vector3((float)A2.x, (float)A2.y, z);

		result.vertices[15] = new Vector3((float)A2.x, (float)A2.y, z);
		result.vertices[16] = new Vector3((float)B2.x, (float)B2.y, z);
		result.vertices[17] = new Vector3((float)B1.x, (float)B1.y, z);

		result.uv[12] = new Vector2(xuv0, yuv1); 
		result.uv[13] = new Vector2(xuv1 / 3, yuv1);
		result.uv[14] = new Vector2(xuv1 / 3, yuv0);

		result.uv[15] = new Vector2(xuv1 / 3, yuv0);
		result.uv[16] = new Vector2(yuv0, xuv0);
		result.uv[17] = new Vector2(xuv0, yuv1);
		
		return(result);
	}

	static public Mesh Export(Mesh2DMesh trianglesList) {
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[trianglesList.verticesCount];
		Vector2[] uv = new Vector2[trianglesList.verticesCount];
		int[] triangles = new int[trianglesList.verticesCount];

		int vCount = 0;
		int count = 0;
		
		Mesh2DSubmesh triangle;
		
		for(int x = 0; x < trianglesList.submeshes.Count; x++) {
			triangle = trianglesList.submeshes[x];

			for(int v = 0; v < triangle.vertices.Length; v++) {
				vertices[vCount] = triangle.vertices[v];
				uv[vCount] = triangle.uv[v];

				vCount += 1;
			}

			int iCount = triangle.vertices.Length;
			for(int i = 0; i < iCount; i++) {
				triangles[count + i] = count + i;
			}
			
			count += iCount;
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		return(mesh);
	}

	
	public class Legacy {

		static public Mesh2DSubmesh CreateBox(float size) {
			Mesh2DSubmesh result = new Mesh2DSubmesh(6);

			result.vertices[0] = new Vector3(-size, -size, 0);
			result.vertices[1] = new Vector3(size, -size, 0);
			result.vertices[2] = new Vector3(size, size, 0);

			result.vertices[3] = new Vector3(size, size, 0);
			result.vertices[4] = new Vector3(-size, size, 0);
			result.vertices[5] = new Vector3(-size, -size, 0);
			
			result.uv[0] = new Vector2(uv0, uv0);
			result.uv[1] = new Vector2(uv1, uv0);
			result.uv[2] = new Vector2(uv1, uv1);

			result.uv[3] = new Vector2(uv1, uv1);
			result.uv[4] = new Vector2(uv1, uv0);
			result.uv[5] = new Vector2(uv0, uv0);
			
			return(result);
		}
	}
}