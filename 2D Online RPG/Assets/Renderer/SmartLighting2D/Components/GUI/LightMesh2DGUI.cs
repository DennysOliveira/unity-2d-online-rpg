using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class LightMesh2DGUI : MonoBehaviour {
    public bool drawRayCastPoints = false;
    public bool drawCollisionPoints = false;
    public bool drawOptimizedPoints = false;

     void OnGUI() {
        if (Camera.main == null) {
            return;
        }

        LightMesh2D light = LightMesh2D.List[0];

        
        if (drawRayCastPoints) {
            for(int i = 0; i < light.geometry.rayCastPoints.count; i++) {
                LightMeshGeometry.Point point = light.geometry.rayCastPoints.list[i];

                Vector2 screenPoint = Camera.main.WorldToScreenPoint(point.value);
                Vector2 textPoint = screenPoint;

                if (point.geometry) {
                    GUI.color = Color.white;
                    GUI.Label(new Rect(textPoint.x, Screen.height - textPoint.y, 100, 100), i.ToString() );
                    
                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(screenPoint.x - 5, Screen.height - screenPoint.y - 5, 10, 10), GetPointTexture());
                } else {
                    GUI.color = Color.white;
                    GUI.Label(new Rect(textPoint.x, Screen.height - textPoint.y, 100, 100), i.ToString() );

                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(screenPoint.x - 5, Screen.height - screenPoint.y - 5, 10, 10), GetPointTexture());
                }
            }
        }
        

    
        if (drawCollisionPoints) {
            foreach(Vector2 point in light.geometry.collisionPoints) {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(point);
                Vector2 textPoint = screenPoint;

                GUI.color = Color.white;
                GUI.Label(new Rect(textPoint.x, Screen.height - textPoint.y, 100, 100), light.geometry.collisionPoints.IndexOf(point).ToString() );

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(screenPoint.x - 5, Screen.height - screenPoint.y - 5, 10, 10), GetPointTexture());
            }
        }
        
        
        if (drawOptimizedPoints) {
            for(int i = 0; i < light.geometry.optimizedPointsCount; i++) {
                Vector2 point = light.geometry.optimizedPoints[i];

                Vector2 screenPoint = Camera.main.WorldToScreenPoint(point);
                Vector2 textPoint = screenPoint;

                GUI.color = Color.white;
                GUI.Label(new Rect(textPoint.x, Screen.height - textPoint.y, 100, 100), i.ToString() );

                //float degre = (point.Atan2(light.transform.position) * Mathf.Rad2Deg + 720) % 360;
                //GUI.color = Color.white;
                //GUI.Label(new Rect(textPoint.x, Screen.height - textPoint.y, 100, 100), ((int)degre).ToString());

                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(screenPoint.x - 3, Screen.height - screenPoint.y - 3, 6, 6), GetPointTexture());
            }

        }
     
    }

    static private Texture pointTexture;

    static private Texture GetPointTexture() {
		if (pointTexture == null) {
			pointTexture = Resources.Load<Texture>("Textures/dot");
		}
		return(pointTexture);
	}
}
