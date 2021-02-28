using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Polygon2ListCollider3D : Polygon2Collider3D {
	public static List<Polygon2> CreateFromGameObject(GameObject gameObject) {
		List<Polygon2> result = new List<Polygon2>();
		
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

                List<Polygon2> polygons = CreateFromMeshCollider(meshCollider);

                foreach(Polygon2 polygon in polygons) {
                    result.Add(polygon);
                }
			}
		}

		foreach(Polygon2 poly in result) {
			poly.Normalize();
		}

		return(result);
	}
}
