using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Polygon2DTriangulation;
using System.Linq;

public static class TriangulationWrapper {
	public class Polygon {
		
		public List<Vector2> outside;
		public List<List<Vector2>> holes;
		public List<Vector2> outsideUVs;
		public List<List<Vector2>> holesUVs;

		public Polygon() {
			outside = new List<Vector2>();
			holes = new List<List<Vector2>>();

			outsideUVs = new List<Vector2>();
			holesUVs = new List<List<Vector2>>();
		}

		public Vector2 ClosestUV(Vector2 pos) {
			Vector2 bestUV = outsideUVs[0];
			float bestDSqr = (outside[0] - pos).sqrMagnitude;

			for (int i = 1; i < outsideUVs.Count; i++) {
				float dsqr = (outside[i] - pos).sqrMagnitude;
				if (dsqr < bestDSqr) {
					bestDSqr = dsqr;
					bestUV = outsideUVs[i];
				}
			}

			List<Vector2> hole = null;
			List<Vector2> holeUVs = null;

			for (int h = 0; h<holes.Count; h++) {
				hole = holes[h];
				holeUVs = holesUVs[h];
				for (int i = 0; i < holeUVs.Count; i++) {
					float dsqr = (hole[i] - pos).sqrMagnitude;
					if (dsqr < bestDSqr) {
						bestDSqr = dsqr;
						bestUV = holeUVs[i];
					}
				}
			}
			return(bestUV);
		}
	}
		
	static List<PolygonPoint> ConvertPoints(List<Vector2> points, Dictionary<uint, Vector2> codeToPosition) {
		int count = points.Count;
		List<PolygonPoint> result = new List<PolygonPoint>(count);
		Vector2 pos = Vector2.zero;
		PolygonPoint pp = null;
		for (int i = 0; i < count; i++) {
			pos = points[i];
			pp = new PolygonPoint(pos.x, - pos.y);
			codeToPosition[pp.VertexCode] = pos;
			result.Add(pp);
		}
		return result;
	}
		
	public static Mesh CreateMesh(Polygon polygon) {
		if (polygon.holes.Count == 0 && (polygon.outside.Count == 3 || (polygon.outside.Count == 4 && polygon.outside[3] == polygon.outside[0])))
			return CreateTriangle(polygon);

		Dictionary <uint, Vector2> codeToPosition = new Dictionary <uint, Vector2>();

		Polygon2DTriangulation.Polygon poly = new Polygon2DTriangulation.Polygon(ConvertPoints(polygon.outside, codeToPosition));

		foreach (List<Vector2> hole in polygon.holes) 
			poly.AddHole(new Polygon2DTriangulation.Polygon(ConvertPoints(hole, codeToPosition)));

		try {
			DTSweepContext tcx = new DTSweepContext();
			tcx.PrepareTriangulation(poly);
			DTSweep.Triangulate(tcx);
			tcx = null;
		} catch (System.Exception e) {
			throw(e);
		}
			
		Dictionary<uint, int> codeToIndex = new Dictionary<uint, int>();
		List<Vector2> vertexList = new List<Vector2>();

		foreach (DelaunayTriangle t in poly.Triangles) {
			foreach (var p in t.Points) {
				if (codeToIndex.ContainsKey(p.VertexCode))
					continue;
				
				codeToIndex[p.VertexCode] = vertexList.Count;

				Vector2 pos;
				if (!codeToPosition.TryGetValue(p.VertexCode, out pos))
					pos = new Vector2(p.Xf, -p.Yf);

				vertexList.Add(pos);
			}
		}

		int[] indices = new int[poly.Triangles.Count * 3];
		{
			int i = 0;
			foreach (DelaunayTriangle t in poly.Triangles) {
				indices[i++] = codeToIndex[t.Points[0].VertexCode];
				indices[i++] = codeToIndex[t.Points[1].VertexCode];
				indices[i++] = codeToIndex[t.Points[2].VertexCode];
			}
		}

		Vector2[] uv = null;
		if (polygon.outsideUVs != null) {
			uv = new Vector2[vertexList.Count];
			for (int i = 0; i<vertexList.Count; i++)
				uv[i] = polygon.ClosestUV(vertexList[i]);
		}

		return CreateMesh (vertexList.ToArray(),  indices, uv);
	}
		
	public static Mesh CreateTriangle(Polygon polygon) {
		Vector2[] vertices = new Vector2[3] { polygon.outside[0], polygon.outside[1], polygon.outside[2] };
		int[] indices = new int[3] { 0, 1, 2 };

		Vector2[] uv = null;
		if (polygon.outsideUVs != null) {
			uv = new Vector2[3];
			for (int i = 0; i < 3; i++) 
				uv[i] = polygon.ClosestUV(vertices[i]);
		}
	
		return CreateMesh(vertices, indices, uv);
	}

	public static Mesh CreateMesh(Vector2[] vertices, int[] indices, Vector2[] uv)
	{
		Mesh msh = new Mesh();

		Vector3[] v = new Vector3[vertices.Length];
		for (int i = 0; i < v.Length; i++)
			v[i] = vertices[i];

		msh.vertices = v;
		msh.triangles = indices;
		msh.uv = uv;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		return msh;
	}
}
