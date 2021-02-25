using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarSprite : MonoBehaviour {
    public static List<FogOfWarSprite> List = new List<FogOfWarSprite>();

    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private Material material;

    public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

    public Sprite GetSprite() {
        SpriteRenderer spriteRenderer = GetSpriteRenderer();

        if (spriteRenderer != null) {
            return(spriteRenderer.sprite);
        }

        return(null);
    }

    public int GetSortingOrder() {
        SpriteRenderer spriteRenderer = GetSpriteRenderer();

        if (spriteRenderer != null) {
            return(spriteRenderer.sortingOrder);
        } else {
            return(0);
        }
    }

    public int GetSortingLayer() {
        SpriteRenderer spriteRenderer = GetSpriteRenderer();

        if (spriteRenderer != null) {
            return(UnityEngine.SortingLayer.GetLayerValueFromID(spriteRenderer.sortingLayerID));
        } else {
            return(0);
        } 
    }

    public void Update() {
        SpriteRenderer spriteRenderer = GetSpriteRenderer();

        if (spriteRenderer == null) {
            return;
        }

        LightingManager2D manager = LightingManager2D.Get();
        
        if (manager.fogOfWarCameras.Length > 0) {
            if (Lighting2D.FogOfWar.useOnlyInPlay) {
                if (Application.isPlaying) {
                    spriteRenderer.enabled = false;
                } else {
                    spriteRenderer.enabled = true;
                }
            } else {
                spriteRenderer.enabled = false;
            }
        } else {
            spriteRenderer.enabled = true;
        }
        
    }

    public SpriteRenderer GetSpriteRenderer() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        material = spriteRenderer.sharedMaterial;
        return(spriteRenderer);
    }

    public void OnEnable() {
		List.Add(this);
	}

	public void OnDisable() {
		List.Remove(this);
	}

	private void OnDrawGizmos() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos == EditorDrawGizmos.Disabled) {
			return;
		}
		
		Gizmos.DrawIcon(transform.position, "fow_v2", true);
	}
}
