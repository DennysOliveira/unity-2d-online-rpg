using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMaterial {
	public Material material = null;

	public SmartMaterial(string path) {
		Shader shader = LoadShader(path);
		if (shader != null) {
			material = new Material(shader);
		}	
	}

	public SmartMaterial(SmartMaterial met) {
		if (met.material != null) {
			material = new Material(met.material);
			material.mainTexture = met.material.mainTexture;
		}
	}

	static public Shader LoadShader(string path) {
		Shader shader = Shader.Find (path);
		if (shader == null) {
			Debug.Log("Shader Not Found: " + path);
			return(null);
		}
		return(shader);
	}

	public void SetTexture(Texture texture) {
		material.mainTexture = texture;
	}

	public void SetColor(Color color) {
		if (material != null) {
		//	if (Slicer2DSettings.GetRenderingPipeline() == Slicer2DSettings.RenderingPipeline.Universal) {
				material.SetColor ("_Emission", color);
		//	} else {
		//		material.color = color;
		//	}
		}
	}
}
