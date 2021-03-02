using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rendering.Light.Sorting {
	public class SortList {
		public SortObject[] list = new SortObject[1024];

		public int count = 0;

		public SortList() {
			for(int i = 0; i < list.Length; i++) {
				list[i] = new SortObject();
			}
		}

		public void Add(LightCollider2D collider2D, float dist) {
			if (count < list.Length) {
				list[count].value = dist;

				list[count].type = SortObject.Type.Collider;
				list[count].lightObject = (object)collider2D;

				count++;
			} else {
				Debug.LogError("Collider Depth Overhead!");
			}
		}

		#if UNITY_2017_4_OR_NEWER
			public void AddTilemap(LightTilemapCollider2D tilemap, float value) {
				if (count < list.Length) {
					list[count].value = value;

					list[count].type = SortObject.Type.TilemapMap;
					list[count].tilemap = tilemap;

					count++;
				} else {
					Debug.LogError("Tile Depth Overhead!");
				}
			}

			public void Add(LightTilemapCollider2D tilemap, LightingTile tile2D, float value) {
				if (count < list.Length) {
					list[count].value = value;

					list[count].type = SortObject.Type.Tile;
					list[count].lightObject = tile2D;
					list[count].tilemap = tilemap;

					count++;
				} else {
					Debug.LogError("Tile Depth Overhead!");
				}
			}
		#endif

		public void Reset() {
			count = 0;
		}

		public void Sort() {
			Array.Sort<SortObject>(list, 0, count, SortObject.Sort());
		}
	}
}