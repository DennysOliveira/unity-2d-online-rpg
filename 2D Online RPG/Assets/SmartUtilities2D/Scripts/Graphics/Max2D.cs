using System.Collections.Generic;
using UnityEngine;

public class Max2D {
	
	#if UNITY_2018_3_OR_NEWER 
		public const string shaderPath = "Legacy Shaders/";
	#else
		public const string shaderPath = "";
	#endif

	public static Vector2 texCoord = Vector2.zero;

	public static void DrawTriangle(float x0, float y0, float x1, float y1, float x2, float y2, Vector2 offset, float z) {
		GL.Vertex3(x0 + offset.x, y0 + offset.y, z);
		GL.Vertex3(x1 + offset.x, y1 + offset.y, z);
		GL.Vertex3(x2 + offset.x, y2 + offset.y, z);
	}
	 
	public static void DrawTriangle(Vector2 vA, Vector2 vB, Vector2 vC, Vector2 scale, float z) {
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vA.x * scale.x, vA.y * scale.y, z);
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vB.x * scale.x, vB.y * scale.y, z);
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vC.x * scale.x, vC.y * scale.y, z);
	}
	
	public static void DrawTriangle(Vector2 vA, Vector2 vB, Vector2 vC, Vector2 offset, Vector2 scale, float z) {
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vA.x * scale.x + offset.x, vA.y * scale.y + offset.y, z);
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vB.x * scale.x + offset.x, vB.y * scale.y + offset.y, z);
		GL.TexCoord3(texCoord.x, texCoord.y, 0);
		GL.Vertex3(vC.x * scale.x + offset.x, vC.y * scale.y + offset.y, z);
	}

	public static void DrawQuad(Vector2 vA, Vector2 vB, Vector2 vC, Vector2 vD, float z) {
		GL.Vertex3(vA.x, vA.y, z);
		GL.Vertex3(vB.x, vB.y, z);
		GL.Vertex3(vC.x, vC.y, z);
		GL.Vertex3(vD.x, vD.y, z);
	}
}







