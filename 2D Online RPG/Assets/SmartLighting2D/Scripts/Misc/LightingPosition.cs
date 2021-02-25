using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class LightingPosition
{
	public static Vector2 GetPosition2D(Vector3 position3D) {  
		switch(Lighting2D.CoreAxis) {
            case CoreAxis.XY:
                return(new Vector2(position3D.x, position3D.y));

			case CoreAxis.XYFLIPPED:
                return(new Vector2(-position3D.x, position3D.y));

			case CoreAxis.XZ:
				return(new Vector2(position3D.x, -position3D.z));

            case CoreAxis.XZFLIPPED:
				return(new Vector2(position3D.x, position3D.z));
		}

        return(Vector2.zero);
    }

	public static Vector3 GetPosition3D(Vector2 position2D) {  
		Vector3 position3D = Vector3.zero;

		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				position3D.x += position2D.x;
				position3D.y += position2D.y;
			break;

			case CoreAxis.XYFLIPPED:
				position3D.x -= position2D.x;
				position3D.y += position2D.y;
			break;

			case CoreAxis.XZFLIPPED:
				position3D.x += position2D.x;
				position3D.z += position2D.y;
			break;	

			case CoreAxis.XZ:
				position3D.x += position2D.x;
				position3D.z -= position2D.y;
			break;	
		}

        return(position3D);
    }

	public static Vector3 GetPosition3D(Vector2 position2D, Vector3 position3D) { 
		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				position3D.x += position2D.x;
				position3D.y += position2D.y;
			break;

			case CoreAxis.XYFLIPPED:
				position3D.x -= position2D.x;
				position3D.y += position2D.y;
			break;

			case CoreAxis.XZFLIPPED:
				position3D.x += position2D.x;
				position3D.z += position2D.y;
			break;	

			case CoreAxis.XZ:
				position3D.x += position2D.x;
				position3D.z -= position2D.y;
			break;	
		}

        return(position3D);
    }

	
	public static Vector3 GetPosition3DWorld(Vector2 position2D, Vector3 position3D) { 
		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				position3D.x = position2D.x;
				position3D.y = position2D.y;
			break;

			case CoreAxis.XYFLIPPED:
				position3D.x = -position2D.x;
				position3D.y = position2D.y;
			break;

			case CoreAxis.XZFLIPPED:
				position3D.x = position2D.x;
				position3D.z = position2D.y;
			break;	

			case CoreAxis.XZ:
				position3D.x = position2D.x;
				position3D.z = -position2D.y;
			break;	
		}

        return(position3D);
    }


	public static float GetRotation2D(Transform transform) {
        float rotation = 0;

		switch(Lighting2D.CoreAxis) {
            case CoreAxis.XY:
                rotation = transform.eulerAngles.z;

			break;

			case CoreAxis.XYFLIPPED:
                rotation = -transform.eulerAngles.z;

			break;

			case CoreAxis.XZ:
                rotation = transform.eulerAngles.y - 180;

			break;

            case CoreAxis.XZFLIPPED:
				rotation = -transform.eulerAngles.y;

			break;
		}

        return(rotation);
    }

    public static Vector3 GetCameraPlanePosition(Camera camera) {
		Vector3 pos = camera.transform.position;
        Vector3 offset = camera.nearClipPlane * camera.transform.forward;

		if (camera.nearClipPlane > 0) {
			offset *= 1.01f;
		} else {
			offset *= 0.99f;
		}

		pos += offset;
		
		return(pos);
	}

	public static float GetCameraRotation(Camera camera) {
		float rotation = 0;
		switch(Lighting2D.CoreAxis) {
			case CoreAxis.XY:
				rotation = -camera.transform.rotation.eulerAngles.z;
			break;

			case CoreAxis.XYFLIPPED:
				rotation = camera.transform.rotation.eulerAngles.z;
			break;


			case CoreAxis.XZ:
				rotation = -camera.transform.rotation.eulerAngles.y;
			break;

			case CoreAxis.XZFLIPPED:
				rotation = camera.transform.rotation.eulerAngles.y;
			break;
		} 
		
		return(rotation);
	}
				

}
