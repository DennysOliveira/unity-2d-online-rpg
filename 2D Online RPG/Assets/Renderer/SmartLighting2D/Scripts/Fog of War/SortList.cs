using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FogOfWar.Sorting {
	public class SortList {
		public SortObject[] list = new SortObject[1024];

		public int count = 0;

		public SortList() {
			for(int i = 0; i < list.Length; i++) {
				list[i] = new SortObject();
			}
		}

		public void Add(FogOfWarSprite sprite, float value) {
			if (count < list.Length) {
				list[count].value = value;

				list[count].sprite = sprite;

				count++;
			} else {
				Debug.LogError("Collider Depth Overhead!");
			}
		}

		public void Reset() {
			count = 0;
		}

		public void Sort() {
			Array.Sort<SortObject>(list, 0, count, SortObject.Sort());
		}
	}
}