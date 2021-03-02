using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LightTilemapCollider;

#if UNITY_2017_4_OR_NEWER

	[System.Serializable]
	public class LightingTile {
		public Vector3Int gridPosition;
		// public TilemapCollider2D or TilemapRoom

		public Vector2? worldPosition = null;
		public float worldRotation = 0;
		public Vector2 worldScale = Vector2.one;
		public float worldRadius = 1.4f;
			
		public Tile.ColliderType colliderType;

		private Sprite originalSprite;

		public Rect uv; // STE
		public Vector2 scale = Vector2.one;
		public float rotation = 0;
		
		public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

		private MeshObject shapeMesh = null;

		private SpriteExtension.PhysicsShape spritePhysicsShape = null;
		private List<Polygon2> spritePhysicsShapePolygons = null;

		private List<Polygon2> localPolygons = null;
		private List<Polygon2> worldPolygons = null;
		private List<Polygon2> worldPolygonsCache = null;

		public void SetOriginalSprite(Sprite sprite) {
			originalSprite = sprite;
		}

		public Sprite GetOriginalSprite() {
			return(originalSprite);
		}

		public bool NotInRange(Vector2 pos, float sourceSize) {
			return(Vector2.Distance(pos, Vector2.zero) > sourceSize + worldRadius);
		}

		public void ResetWorld() {
			worldPolygons = null;
			worldPosition = null;
			worldRotation = 0;
		}

		public void UpdateTransform(LightTilemapCollider.Base tilemap) {
			if (worldPosition != null) {
				return;
			}

			worldPosition = tilemap.TileWorldPosition(this);
			worldRotation = tilemap.TileWorldRotation(this);
			worldScale = tilemap.TileWorldScale();
		}

		// Remove
		public Vector2 GetWorldPosition(LightTilemapCollider.Base tilemap) {
			if (worldPosition == null) {
				worldPosition = tilemap.TileWorldPosition(this);
				worldRotation = tilemap.TileWorldRotation(this);
				worldScale = tilemap.TileWorldScale();
			}
			
			return(worldPosition.Value);
		}

		public void SetLocalPolygons(List<Polygon2> localPolygons) {
			this.localPolygons = localPolygons;
		}

		public List<Polygon2> GetWorldPolygons(LightTilemapCollider.Base tilemap) {
			if (worldPolygons == null) {
				List<Polygon2> localPolygons = GetLocalPolygons(tilemap);
				if (worldPolygonsCache == null) {

					worldPolygons = new List<Polygon2>();
					worldPolygonsCache = worldPolygons;
					
					UpdateTransform(tilemap);
	
					foreach(Polygon2 polygon in localPolygons) {
						Polygon2 worldPolygon = polygon.Copy();

						if (scale != Vector2.one) {
							worldPolygon.ToScaleSelf(scale);
						}

						worldPolygon.ToScaleSelf(worldScale);
						worldPolygon.ToRotationSelf((tilemap.transform.eulerAngles.z + rotation) * Mathf.Deg2Rad);
						worldPolygon.ToOffsetSelf(worldPosition.Value);

						worldPolygons.Add(worldPolygon);
					}
					
				} else {
					worldPolygons = worldPolygonsCache;

					UpdateTransform(tilemap);

					for(int i = 0; i < localPolygons.Count; i++) {
						Polygon2 polygon = localPolygons[i];
						Polygon2 worldPolygon = worldPolygons[i];

						for(int j = 0; j < polygon.points.Length; j++) {
							worldPolygon.points[j].x = polygon.points[j].x;
							worldPolygon.points[j].y = polygon.points[j].y;
						}

						if (scale != Vector2.one) {
							worldPolygon.ToScaleSelf(scale);
						}

						worldPolygon.ToScaleSelf(worldScale);
						worldPolygon.ToRotationSelf(tilemap.transform.eulerAngles.z * Mathf.Deg2Rad);
						worldPolygon.ToOffsetSelf(worldPosition.Value);

					
					}

						
				}
			}

			return(worldPolygons);
		}

		public List<Polygon2> GetLocalPolygons(LightTilemapCollider.Base tilemap) {
			if (localPolygons == null) {

				if (tilemap.IsPhysicsShape()) {

					List<Polygon2> customShapePolygons = GetPhysicsShapePolygons();
				
					if (customShapePolygons.Count > 0) {
						localPolygons = customShapePolygons;
					} else {
						localPolygons = new List<Polygon2>();
					}

				} else {
					localPolygons = new List<Polygon2>();

					switch(tilemap.TilemapType()) {
						case MapType.UnityRectangle:
						case MapType.SuperTilemapEditor:

							localPolygons.Add(Polygon2.CreateRect(Vector2.one));

						break;

						case MapType.UnityIsometric:

							localPolygons.Add(Polygon2.CreateIsometric(new Vector2(1, 0.5f)));

						break;

						case MapType.UnityHexagon:

							localPolygons.Add(Polygon2.CreateHexagon(new Vector2(1, 0.5f)));

						break;

					}
				}
			}
			return(localPolygons);
		}

		public List<Polygon2> GetPhysicsShapePolygons() {
			if (spritePhysicsShapePolygons == null) {

				spritePhysicsShapePolygons = new List<Polygon2>();

				if (originalSprite == null) {
					return(spritePhysicsShapePolygons);
				}

				#if UNITY_2017_4_OR_NEWER
					if (spritePhysicsShape == null) {
						spritePhysicsShape = SpriteExtension.PhysicsShapeManager.RequesCustomShape(originalSprite);
					}

					if (spritePhysicsShape != null) {
						spritePhysicsShapePolygons = spritePhysicsShape.Get();
					}
				#endif
			}

			return(spritePhysicsShapePolygons);
		}

		public MeshObject GetDynamicMesh() {
			if (shapeMesh == null) {
				if (spritePhysicsShapePolygons != null && spritePhysicsShapePolygons.Count > 0) {
					shapeMesh = spritePhysicsShape.GetMesh();
				}
			}
			return(shapeMesh);
		}





		public static MeshObject GetStaticMesh(LightTilemapCollider.Base tilemap) {
			switch(tilemap.TilemapType()) {
				case MapType.UnityRectangle:
					return(Rectangle.GetStaticMesh());

				case MapType.UnityIsometric:
					return(Isometric.GetStaticMesh());

				case MapType.UnityHexagon:
					return(Hexagon.GetStaticMesh());
			}

			return(null);
		}




		public class Rectangle {
			public static MeshObject meshObject = null;

			public static MeshObject GetStaticMesh() {
				if (meshObject == null) {
					// Can be optimized?
					Mesh mesh = new Mesh();

					float x = 0.5f;
					float y = 0.5f;

					mesh.vertices = new Vector3[]{new Vector2(-x, -y), new Vector2(x, -y), new Vector2(x, y), new Vector2(-x, y)};
					mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
					mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
			
			
					meshObject = MeshObject.Get(mesh);	
				}
				return(meshObject);
			}
		}

		public static class Isometric {
			private static MeshObject meshObject = null;

			public static MeshObject GetStaticMesh() {
				if (meshObject == null) {
					Mesh mesh = new Mesh();

					float x = 0.5f;
					float y = 0.5f;

					mesh.vertices = new Vector3[]{new Vector2(0, y), new Vector2(x, y / 2), new Vector2(0, 0), new Vector2(-x, y / 2)};
					mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
					mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
			
					meshObject = MeshObject.Get(mesh);		
				}
				return(meshObject);
			}
		}

		public static class Hexagon {
			private static MeshObject meshObject = null;

			public static MeshObject GetStaticMesh() {
				if (meshObject == null) {
					Mesh mesh = new Mesh();

					float x = 0.5f ;
					float y = 0.5f;

					float yOffset = - 0.25f;
					mesh.vertices = new Vector3[]{new Vector2(0, y * 1.5f + yOffset), new Vector2(x, y + yOffset), new Vector2(0, -y * 0.5f + yOffset), new Vector2(-x, y + yOffset), new Vector2(-x, 0 + yOffset), new Vector2(x, 0 + yOffset)};
					mesh.triangles = new int[]{0, 1, 5, 4, 3, 0, 0, 5, 2, 0, 2, 4};
					mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 1),  new Vector2(0, 1) };
			
			
					meshObject = MeshObject.Get(mesh);	
				}
				return(meshObject);
			}
		}

		public static class STE {
			private static MeshObject meshObject = null;

			public static MeshObject GetStaticMesh() {
				if (meshObject == null) {
					Mesh mesh = new Mesh();

					float x = 0.5f;
					float y = 0.5f;

					mesh.vertices = new Vector3[]{new Vector2(-x, -y), new Vector2(x, -y), new Vector2(x, y), new Vector2(-x, y)};
					mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
					mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

					meshObject = MeshObject.Get(mesh);	
				}

				return(meshObject);
			}
		}
	}

#endif