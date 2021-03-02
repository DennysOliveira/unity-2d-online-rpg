using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightingMaterial {
    private string path = "";
    private Texture texture = null;

    private Material material = null;

    static public LightingMaterial Load(Material material) {
        LightingMaterial lightingMaterial = new LightingMaterial();

        //lightingMaterial.path = material.name;

        lightingMaterial.material = material;;
        
        return(lightingMaterial);
    }

    static public LightingMaterial Load(string path) {
        LightingMaterial lightingMaterial = new LightingMaterial();

        lightingMaterial.path = path;

        Shader shader = Shader.Find (path);
        if (shader == null) {
            Debug.LogError("Smart Lighting: Shader Not Found '" + path + "'");
        } else {
           // Debug.Log("Smart Lighting: Loading Material " + path);
            lightingMaterial.material = new Material (shader);
        }

        return(lightingMaterial);
    }

    public void SetTexture(string path) {
        texture = Resources.Load (path) as Texture;

        if (material != null) {
            material.mainTexture = texture;
        }
    }

     public void SetTexture(Texture setTexture) {
        texture = setTexture;

        if (material != null) {
            material.mainTexture = texture;
        }
    }

    public Material Get() {
        if (material == null) {
            Shader shader = Shader.Find (path);
            if (shader != null) {
               // Debug.Log("Smart Lighting: Reloading Material '" + path + "'");

                material = new Material (shader);

                if (texture != null) {
                    material.mainTexture = texture;
                }
            }
        }
        return(material);
    }
}
