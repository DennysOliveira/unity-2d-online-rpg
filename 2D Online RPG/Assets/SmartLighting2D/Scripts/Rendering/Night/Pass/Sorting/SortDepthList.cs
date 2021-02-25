using UnityEngine;
using System;

namespace Rendering.Night.Sorting {

    public class SortList {

        public SortObject[] list = new SortObject[1024];

        public int count = 0;

        public SortList() {
            for(int i = 0; i < list.Length; i++) {
                list[i] = new SortObject();
            }
        }

        public void Add(object lightObject, SortObject.Type objectType, float dist) {
            if (count + 1 < list.Length) {
                list[count].lightObject = lightObject;
                list[count].type = objectType;
                list[count].distance = dist;
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