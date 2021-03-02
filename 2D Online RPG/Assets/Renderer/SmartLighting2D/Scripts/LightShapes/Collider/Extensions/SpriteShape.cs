using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightShape;

namespace LightShape {
		
	public class SpriteShape : Base {

		private Sprite originalSprite;

		private SpriteRenderer spriteRenderer;

		VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

		public override int GetSortingLayer() {
			return(UnityEngine.SortingLayer.GetLayerValueFromID(GetSpriteRenderer().sortingLayerID));
		}

        public override int GetSortingOrder()
        {
            return(GetSpriteRenderer().sortingOrder);
        }

		private Polygon2 rectPolygon;
		private Polygon2 GetRectPolygon() {
			if (rectPolygon == null) {
				rectPolygon = new Polygon2(4);
			}

        	return(rectPolygon);
		}

		public override List<Polygon2> GetPolygonsLocal() {

			if (LocalPolygons == null) {
				LocalPolygons = new List<Polygon2>();

				if (spriteRenderer == null) {
					return(LocalPolygons);
				}

				if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {

					float rot = transform.eulerAngles.z;
					Vector2 size = transform.localScale * spriteRenderer.size * 0.5f;
					Vector2 pos = Vector3.zero;

					rot = rot * Mathf.Deg2Rad + Mathf.PI;

					float rectAngle = Mathf.Atan2(size.y, size.x);
					float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

					Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
					Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
					Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
					Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
				
					Polygon2 polygon = new Polygon2(4);

					polygon.points[0] = v1;
					polygon.points[1] = v2;
					polygon.points[2] = v3;
					polygon.points[3] = v4;

					LocalPolygons.Add(polygon);

				} else {

					virtualSpriteRenderer.Set(spriteRenderer);

					Vector2 position = Vector3.zero;
					Vector2 scale = transform.localScale;
					float rotation = transform.eulerAngles.z;
		
					SpriteTransform spriteTransform = new SpriteTransform(virtualSpriteRenderer, position, scale, rotation);

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
				
					Polygon2 polygon = new Polygon2(4);

					polygon.points[0] = v1;
					polygon.points[1] = v2;
					polygon.points[2] = v3;
					polygon.points[3] = v4;

					LocalPolygons.Add(polygon);
				}


			}

			return(LocalPolygons);
		}

		public override List<Polygon2> GetPolygonsWorld() {
			if (WorldPolygons == null) {

				if (WorldCache == null) {

					WorldPolygons = new List<Polygon2>();

					if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {

						float rot = transform.eulerAngles.z;
						Vector2 size = transform.lossyScale * spriteRenderer.size * 0.5f;
						Vector2 pos = transform.position;

						rot = rot * Mathf.Deg2Rad + Mathf.PI;

						float rectAngle = Mathf.Atan2(size.y, size.x);
						float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

						Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
						Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
						Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
						Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
					
						Polygon2 polygon = new Polygon2(4);

						polygon.points[0] = v1;
						polygon.points[1] = v2;
						polygon.points[2] = v3;
						polygon.points[3] = v4;

						WorldPolygons.Add(polygon);

					} else {

						virtualSpriteRenderer.Set(spriteRenderer);

						Vector2 position = transform.position;
						Vector2 scale = transform.lossyScale;
						float rotation = transform.eulerAngles.z;
			
						SpriteTransform spriteTransform = new SpriteTransform(virtualSpriteRenderer, position, scale, rotation);

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
					
						Polygon2 polygon = new Polygon2(4);

						polygon.points[0] = v1;
						polygon.points[1] = v2;
						polygon.points[2] = v3;
						polygon.points[3] = v4;

						WorldPolygons.Add(polygon);

						WorldCache = WorldPolygons;
					}
				} else {
					
					WorldPolygons = WorldCache;

					if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {

						float rot = transform.eulerAngles.z;
						Vector2 size = transform.lossyScale * spriteRenderer.size * 0.5f;
						Vector2 pos = transform.position;

						rot = rot * Mathf.Deg2Rad + Mathf.PI;

						float rectAngle = Mathf.Atan2(size.y, size.x);
						float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

						Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
						Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
						Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
						Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
					
						Polygon2 polygon = WorldPolygons[0];

						polygon.points[0] = v1;
						polygon.points[1] = v2;
						polygon.points[2] = v3;
						polygon.points[3] = v4;

					} else {
						
						virtualSpriteRenderer.Set(spriteRenderer);

						Vector2 position = transform.position;
						Vector2 scale = transform.lossyScale;
						float rotation = transform.eulerAngles.z;
			
						SpriteTransform spriteTransform = new SpriteTransform(virtualSpriteRenderer, position, scale, rotation);

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
					
						Polygon2 polygon = WorldPolygons[0];
						
						polygon.points[0] = v1;
						polygon.points[1] = v2;
						polygon.points[2] = v3;
						polygon.points[3] = v4;
					}
				}
			}

			return(WorldPolygons);
		}

		public override void ResetLocal() {
			base.ResetLocal();

			originalSprite = null;
		}

		public SpriteRenderer GetSpriteRenderer() {
			if (spriteRenderer != null) {
				return(spriteRenderer);
			}
			
			if (transform == null) {
				return(spriteRenderer);
			}

			if (spriteRenderer == null) {
				spriteRenderer = transform.GetComponent<SpriteRenderer>();
			}
			
			return(spriteRenderer);
		}

		public Sprite GetOriginalSprite() {
            if (originalSprite == null) {
                GetSpriteRenderer();

                if (spriteRenderer != null) {
                    originalSprite = spriteRenderer.sprite;
                }
            }
			return(originalSprite);
		}
	}
}