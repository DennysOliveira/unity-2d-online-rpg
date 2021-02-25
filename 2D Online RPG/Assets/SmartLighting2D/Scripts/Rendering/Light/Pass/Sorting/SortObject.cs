using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rendering.Light.Sorting {
	public struct SortObject : System.Collections.Generic.IComparer<SortObject> {
		public enum Type {Collider, Tile, TilemapMap};

		public Type type;
		public float value; // value

		public object lightObject;

		// Lighting Tilemap Tile
		#if UNITY_2017_4_OR_NEWER
			public LightTilemapCollider2D tilemap;
		#endif

		public SortObject(int a) {
			type = Type.Collider;

			value = 0;

			lightObject = null;

			tilemap = null;
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