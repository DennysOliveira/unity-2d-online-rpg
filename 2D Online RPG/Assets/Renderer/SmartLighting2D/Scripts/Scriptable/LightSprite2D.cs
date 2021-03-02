using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable {

    [System.Serializable]
    public class LightSprite2D {

        public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

        public LightSpriteShape lightSpriteShape = new LightSpriteShape();

        [SerializeField]
        private int nightLayer;
        public int NightLayer {
            set => nightLayer = value;
            get => nightLayer;
        }

        [SerializeField]
        private Sprite sprite;
        public Sprite Sprite {
            set => sprite = value;
            get => sprite;
        }

        [SerializeField]
        private Vector2 position = Vector2.zero;
        public Vector2 Position {
            set => position = value;
            get => position;
        }

        [SerializeField]
        private Vector2 scale = Vector2.one;
        public Vector2 Scale {
            set => scale = value;
            get => scale;
        }

        [SerializeField]
        private Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color Color {
            set => color = value;
            get => color;
        }

        [SerializeField]
        private float rotation;
        public float Rotation {
            set => rotation = value;
            get => rotation;
        }

        public bool InCamera(Camera camera) {
            Rect cameraRect = CameraTransform.GetWorldRect(camera);

            lightSpriteShape.Update(this);

            if (cameraRect.Overlaps(lightSpriteShape.GetWorldRect())) {
                return(true);
            }

            return(false);
        }

      	public static List<LightSprite2D> List = new List<LightSprite2D>();

	    public LightSprite2D() {
            List.Add(this);
        }

        public void SetActive(bool active) {
            if (active) {
                if (List.Contains(this) == false) {
                     List.Add(this);
                }
               
            } else {
                List.Remove(this);
            }
        }

    }

    
[System.Serializable]
public class LightSpriteTransform {
	public Vector2 scale = new Vector2(1, 1);
	public float rotation = 0;
	public Vector2 position = new Vector2(0, 0);	
}

[System.Serializable]
public class LightSpriteShape {
    public bool update = false;

    private Sprite sprite;

    private Vector2 position = Vector2.zero;
    private float rotation = 0;
    private Vector2 scale = Vector2.one;
 
    public void Update(LightSprite2D light) {
        Vector2 position2D = light.Position;
        float rotation2D = light.Rotation;
        Vector2 scale2D = light.Scale;

        sprite = light.Sprite;

        if (position != position2D) {
            position = position2D;

            update = true;
        }

        if (rotation != rotation2D) {
            rotation = rotation2D;

            update = true;
        }

        if (scale != scale2D) {
            scale = scale2D;

            update = true;
        }

        if (update) {
            worldPolygon = null;

            update = false;
        }
    }

	public Rect GetWorldRect() {
        GetSpriteWorldPolygon();
		return(worldrect);
	}

    private Polygon2D worldPolygon = null;
    private Rect worldrect = new Rect();

	public Polygon2D GetSpriteWorldPolygon() {
        if (worldPolygon != null) {
            return(worldPolygon);
        }

		Vector2 position = this.position;
		Vector2 scale = this.scale;
		float rotation = this.rotation;

        VirtualSpriteRenderer virtualSprite = new VirtualSpriteRenderer();
        virtualSprite.sprite = sprite;

		SpriteTransform spriteTransform = new SpriteTransform(virtualSprite, position, scale, rotation);

		float rot = spriteTransform.rotation;
		Vector2 size = spriteTransform.scale;
		Vector2 pos = spriteTransform.position;

		rot = rot * Mathf.Deg2Rad + Mathf.PI;

		float rectAngle = Mathf.Atan2(size.y, size.x);
		float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

		Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
		Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
		Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
		Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
	
		worldPolygon = GetPolygon();
		
		worldPolygon.pointsList[0].x = v1.x;
		worldPolygon.pointsList[0].y = v1.y;

		worldPolygon.pointsList[1].x = v2.x;
		worldPolygon.pointsList[1].y = v2.y;

		worldPolygon.pointsList[2].x = v3.x;
		worldPolygon.pointsList[2].y = v3.y;

		worldPolygon.pointsList[3].x = v4.x;
		worldPolygon.pointsList[3].y = v4.y;

        worldrect = worldPolygon.GetRect();

		return(worldPolygon);
	}

	private Polygon2D polygon = null;
	private Polygon2D GetPolygon() {
		if (polygon == null) {
			polygon = new Polygon2D();
			polygon.AddPoint(0, 0);
			polygon.AddPoint(0, 0);
			polygon.AddPoint(0, 0);
			polygon.AddPoint(0, 0);
		}

		return(polygon);
	}
}

}
