using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoublePair2 {

	public Vector2 A;
	public Vector2 B;
	public Vector2 C;

	public DoublePair2(Vector2 pointA, Vector2 pointB, Vector2 pointC)
	{
		A = pointA;
		B = pointB;
		C = pointC;
	}

	static public List<DoublePair2> GetList(Vector2[] list, bool connect = true)
	{
		List<DoublePair2> pairsList = new List<DoublePair2>();
		if (list.Length > 0) {
			for(int i = 0; i < list.Length; i++) {
				Vector2 pB = list[i];

				int indexB = i;

				int indexA = (indexB - 1);
				if (indexA < 0) {
					indexA += list.Length;
				}

				int indexC = (indexB + 1);
				if (indexC >= list.Length) {
					indexC -= list.Length;
				}

				pairsList.Add (new DoublePair2 (list[indexA], pB, list[indexC]));
			}
		}
		return(pairsList);
	}

	static public List<DoublePair2> GetListCopy(List<Vector2> list, bool connect = true)
	{
		List<DoublePair2> pairsList = new List<DoublePair2>();
		if (list.Count > 0) {
			foreach (Vector2 pB in list) {
				int indexB = list.IndexOf (pB);

				int indexA = (indexB - 1);
				if (indexA < 0) {
					indexA += list.Count;
				}

				int indexC = (indexB + 1);
				if (indexC >= list.Count) {
					indexC -= list.Count;
				}

				pairsList.Add (new DoublePair2 (list[indexA], pB, list[indexC]));
			}
		}
		return(pairsList);
	}
}
