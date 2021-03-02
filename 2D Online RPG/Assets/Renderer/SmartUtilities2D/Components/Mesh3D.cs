using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh3D : MonoBehaviour {
	public float size = 1f;
	public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;

	// Optionable material
	public Material material;

	public string sortingLayerName; 
	public int sortingOrder;

	
}
