using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager<T> {
	public List<T>[] layerList;

	public LayerManager() {
		layerList = new List<T>[10];

		for(int i = 0; i < 10; i++) {
			layerList[i] = new List<T>();
		}
	}

	public int Update(int targetLayer, int newLayer, T obj) {
		if (targetLayer != newLayer) {
			if (targetLayer > -1) {
				layerList[targetLayer].Remove(obj);
			}

			targetLayer = newLayer;

			layerList[targetLayer].Add(obj);
		}

		return(targetLayer);
	}

	public void Remove(int targetLayer, T obj) {
		if (targetLayer > -1) {
			layerList[targetLayer].Remove(obj);
		}
	}
}