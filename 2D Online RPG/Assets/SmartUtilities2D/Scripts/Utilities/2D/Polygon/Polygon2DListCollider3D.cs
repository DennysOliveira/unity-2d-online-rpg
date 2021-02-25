using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2DListCollider3D : Polygon2DCollider3D {
	public static List<Polygon2D> CreateFromGameObject(GameObject gameObject) {
		List<Polygon2D> result = new List<Polygon2D>();
		
		foreach(Collider collider in gameObject.GetComponents<Collider> ()) {
			System.Type type = collider.GetType();

			if (type == typeof(BoxCollider)) {
				BoxCollider boxCollider = (BoxCollider)collider;

				result.Add(CreateFromBoxCollider(boxCollider));
			}

			if (type == typeof(SphereCollider)) {
				SphereCollider sphereCollider = (SphereCollider)collider;

				result.Add(CreateFromSphereCollider(sphereCollider));
			}

			if (type == typeof(CapsuleCollider)) {
				CapsuleCollider capsuleCollider = (CapsuleCollider)collider;

				result.Add(CreateFromCapsuleCollider(capsuleCollider));
			}

			if (type == typeof(MeshCollider)) {
				MeshCollider meshCollider = (MeshCollider)collider;

                List<Polygon2D> polygons = CreateFromMeshCollider(meshCollider);

                foreach(Polygon2D polygon in polygons) {
                    result.Add(polygon);
                }
			}
		}

		foreach(Polygon2D poly in result) {
			poly.Normalize();
		}

		return(result);
	}
}