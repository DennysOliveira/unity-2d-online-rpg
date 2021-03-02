using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 2D points list connected by pairs
/// </summary>
public class Pair2D {

	/// <summary>
	/// First vector of a pair
	/// </summary>
	public Vector2D A;

	/// <summary>
	/// Second vector of a pair
	/// </summary>
	public Vector2D B;

	/// <summary>
	/// 2D points list connected by pairs
	/// </summary>
	public Pair2D(Vector2D pointA, Vector2D pointB)
	{
		A = pointA;
		B = pointB;
	}

	public Pair2D(Vector2 pointA, Vector2 pointB)
	{
		A = new Vector2D(pointA);
		B = new Vector2D(pointB);
	}
		
	/// <summary>
	/// 2D points list connected by pairs
	/// </summary>
	static public List<Pair2D> GetList(List<Vector2D> list, bool connect = true)
	{
		List<Pair2D> pairsList = new List<Pair2D>();
		if (list.Count > 0) {
			Vector2D p0 = null;
			if (connect == true) {
				p0 = list.Last ();
			}
			
			Vector2D p1;
			for(int i = 0; i < list.Count; i++) {
				p1 = list[i];
				
				if (p0 != null) {
					pairsList.Add (new Pair2D (p0, p1));
				}

				p0 = p1;
			}
		}
		return(pairsList);
	}

	static public List<Pair2D> GetList(Vector2[] list, bool connect = true)
	{
		List<Pair2D> pairsList = new List<Pair2D>();
		if (list.Length > 0) {
			Vector2? p0 = null;

			if (connect == true) {
				p0 = list[list.Length - 1];
			}
			
			Vector2 p1;
			for(int i = 0; i < list.Length; i++) {
				p1 = list[i];
				
				if (p0 != null) {
					pairsList.Add (new Pair2D (p0.Value, p1));
				}

				p0 = p1;
			}
		}
		return(pairsList);
	}

	/// <summary>
	/// Creates a pair with 2 vectors using (0, 0) coordinates
	/// </summary>
	public static Pair2D Zero()
	{
		return(new Pair2D (Vector2.zero, Vector2.zero));
	}
}



public class Pair2 {

	/// <summary>
	/// First vector of a pair
	/// </summary>
	public Vector2 A;

	/// <summary>
	/// Second vector of a pair
	/// </summary>
	public Vector2 B;

	/// <summary>
	/// 2D points list connected by pairs
	/// </summary>
	public Pair2(Vector2 pointA, Vector2 pointB)
	{
		A = pointA;
		B = pointB;
	}

	public static Pair2 Zero()
	{
		return(new Pair2 (Vector2.zero, Vector2.zero));
	}
}
