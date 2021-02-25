using UnityEngine;

namespace Rendering.Night.Sorting {

	public struct SortObject : System.Collections.Generic.IComparer<SortObject> {

		public enum Type {Light, Room, LightSprite, TilemapRoom};

		public float distance;

		public object lightObject;
		public Type type;

		public SortObject(int a) {
			distance = 0;

			type = Type.Light;
			lightObject = null;
		}

		public int Compare(SortObject a, SortObject b) {
			SortObject c1 = (SortObject)a;
			SortObject c2 = (SortObject)b;

			if (c1.distance > c2.distance) {
				return 1;
			}
		
			if (c1.distance < c2.distance) {
				return -1;
			} else {
				return 0;
			}
		}

		public static System.Collections.Generic.IComparer<SortObject> Sort() {      
			return (System.Collections.Generic.IComparer<SortObject>) new SortObject();
		}
	}
}