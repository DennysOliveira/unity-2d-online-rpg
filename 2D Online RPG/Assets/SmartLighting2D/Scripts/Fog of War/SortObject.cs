using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace  FogOfWar.Sorting {
	public struct SortObject : System.Collections.Generic.IComparer<SortObject> {
		public float value; // value

		public FogOfWarSprite sprite;

		public SortObject(int a) {
			value = 0;

			sprite = null;
		}

		public int Compare(SortObject a, SortObject b) {
			if (a.value > b.value) {
				return 1;
			}
		
			if (a.value < b.value) {
				return -1;
			} else {
				return 0;
			}
		}

		private static System.Collections.Generic.IComparer<SortObject> comparer = (System.Collections.Generic.IComparer<SortObject>) new SortObject();

		public static System.Collections.Generic.IComparer<SortObject> Sort() { 
			return (comparer);
		}
	}
}