using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightShape {

	public class SkinnedMeshRendererShape : Base {
		private SkinnedMeshRenderer skinnedMeshRenderer;

		public SkinnedMeshRenderer GetSkinnedMeshRenderer() {
			if (skinnedMeshRenderer == null) {
				if (transform != null) {
					skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
				}
			}
			return(skinnedMeshRenderer);
		}

		public override List<MeshObject> GetMeshes() {
			if (Meshes == null) {
				if (GetSkinnedMeshRenderer() != null) {
					Mesh mesh = GetSkinnedMeshRenderer().sharedMesh;
					
					if (mesh != null) {
						Meshes = new List<MeshObject>();

						MeshObject meshObject = MeshObject.Get(mesh);

						if (meshObject != null) {
							Meshes.Add(meshObject);
						}
					}
				}
			}
			return(Meshes);
		}

		public override List<Polygon2> GetPolygonsWorld() {
			if (WorldPolygons != null) {
				return(WorldPolygons); 
			}

			List<MeshObject> meshes = GetMeshes();

			if (meshes == null) {
				WorldPolygons = new List<Polygon2>();
				return(WorldPolygons);
			}

			MeshObject meshObject = meshes[0];

			if (meshObject == null) {
				WorldPolygons = new List<Polygon2>();
				return(WorldPolygons);
			}

			Vector3 vecA, vecB, vecC;
			Polygon2 poly;

			if (WorldCache == null) {
				WorldPolygons = new List<Polygon2>();

				for (int i = 0; i < meshObject.triangles.GetLength (0); i = i + 3) {
					vecA = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i]]);
					vecB = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 1]]);
					vecC = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 2]]);

					poly = new Polygon2(3);
					poly.points[0] = vecA;
					poly.points[1] = vecB;
					poly.points[2] = vecC;

					WorldPolygons.Add(poly);
				}	

				WorldCache = WorldPolygons;

			} else {
				int count = 0;

				WorldPolygons = WorldCache;

				for (int i = 0; i < meshObject.triangles.GetLength (0); i = i + 3) {
					vecA = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i]]);
					vecB = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 1]]);
					vecC = transform.TransformPoint(meshObject.vertices [meshObject.triangles [i + 2]]);

					poly = WorldPolygons[count];
					poly.points[0].x = vecA.x;
					poly.points[0].y = vecA.y;
					poly.points[1].x = vecB.x;
					poly.points[1].y = vecB.y;
					poly.points[2].x = vecC.x;
					poly.points[2].y = vecC.y;
					
					count += 1;
				}
			}

			return(WorldPolygons);
		}
	}
}