using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Reflection;
using System.Runtime.Serialization;

[ExecuteInEditMode]
public class LightTilemapOcclusion2D : MonoBehaviour {
	public enum MapType {UnityEngineTilemapRectangle};

    public MapType tilemapType = MapType.UnityEngineTilemapRectangle;

	public bool onlyColliders = false;

	public LightingSettings.SortingLayer sortingLayer = new LightingSettings.SortingLayer();

	private OcclusionMesh tilemapMesh = null;
	private RectangleMap map = null;

	static OcclusionTileset tileset = null;

	Tilemap tilemap2D;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;

    private void OnEnable() {
		tilemap2D = GetComponent<Tilemap>();

		Initialize();
    }

	public void Initialize() {
		if (tilemap2D == null) {
			return;
		}
		
		SetupMap();

		if (tileset == null) {
			tileset = OcclusionTileset.Load("Textures/OclussionMap");
		}

		GenerateMesh();

		ExportMesh();
	}

	public void Update() {
		sortingLayer.ApplyToMeshRenderer(meshRenderer);
	}

	public void SetupMap() {
		map = new RectangleMap();

		ITilemap tilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
		typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tilemap, tilemap2D);

		foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
			TileData tileData = new TileData();

			TileBase tilebase = tilemap2D.GetTile(position);

			if (tilebase != null) {
				tilebase.GetTileData(position, tilemap, ref tileData);

				if (onlyColliders && tileData.colliderType == Tile.ColliderType.None) {
					continue;
				}

				RectangleTile tile = new RectangleTile();
				tile.position.x = position.x;
				tile.position.y = position.y;

				map.mapTiles.Add(tile);
			}
		}

		map.width = tilemap2D.cellBounds.size.x * 2;
		map.height = tilemap2D.cellBounds.size.y * 2;

		map.Init();
	}

    public void GenerateMesh() {
		tilemapMesh = new OcclusionMesh(tileset);

        int x9y1, x1y9, x1y1, x0y1, x1y0, x9y9, x0y9, x9y0;

		 for(int x = 0; x < map.width; x++) {
            for(int y = 0; y < map.height; y++) {
				if (map.GetUnwalkableInt(x, y) == 1) {
                    continue;
				}

        		x0y9 = map.GetUnwalkableInt(x, y + 1);
				x9y0 = map.GetUnwalkableInt(x + 1, y);
				x1y0 = map.GetUnwalkableInt(x - 1, y);
				x0y1 = map.GetUnwalkableInt(x, y - 1);

                x1y1 = map.GetUnwalkableInt(x - 1, y - 1);
				x9y9 = map.GetUnwalkableInt(x + 1, y + 1);
				x1y9 = map.GetUnwalkableInt(x - 1, y + 1);
				x9y1 = map.GetUnwalkableInt(x + 1, y - 1);

				Vector2Int position = new Vector2Int(x - map.width / 2, y - map.height / 2);

				if ((x1y1 == 1) && (x0y1 + x1y0 == 0)) {
					AddTile(8, position, OcclusionTileset.TileRotation.up); 
				} 
				if  ((x9y9 == 1) && (x0y9 + x9y0 == 0)) {
					AddTile(8, position, OcclusionTileset.TileRotation.down);
				} 
				if ((x9y1 == 1) && (x0y1 + x9y0 == 0)) { 
					AddTile(8, position, OcclusionTileset.TileRotation.right); 
				} 
				if ((x1y9 == 1) && (x0y9 + x1y0 == 0)) {
					AddTile(8, position, OcclusionTileset.TileRotation.left);
				}

				if (x1y0 == 1 && x0y1 == 1 && x0y9  == 1&& x9y0 == 1) {
					AddTile(0, position, OcclusionTileset.TileRotation.up);
					continue;
				}

				if (x1y0 == 0 && x0y1 == 1 && x0y9 == 1 && x9y0 == 1 && x1y1 == 1 && x1y9 == 1) {
					AddTile(1, position, OcclusionTileset.TileRotation.up);
					continue;
				} 
				if (x9y0 == 0 && x0y1 == 1 && x0y9 == 1&& x1y0 == 1 && x9y9 == 1 && x9y1 == 1) {
					AddTile(1, position, OcclusionTileset.TileRotation.down);
					continue;
				}
				if(x0y9 == 0 && x0y1 == 1 && x9y0 == 1 && x1y0 == 1 && x9y9 == 1 && x1y9 == 1){
					AddTile(1, position, OcclusionTileset.TileRotation.left);
					continue;
				}
				if (x0y1 == 0 && x0y9 == 1 && x9y0 == 1 && x1y0 == 1 && x9y1 == 1 && x1y1 == 1) {
					AddTile(1, position, OcclusionTileset.TileRotation.right);
					continue;
				}

				if (x0y1 == 1 && x1y0 == 1 && x9y1 == 1 && x1y9 == 1 && (x9y0 + x0y9 == 0)) {
					AddTile(3, position, OcclusionTileset.TileRotation.up);
					continue;
				}
				if (x0y9 == 1 && x9y0 == 1 && x1y9 == 1 && x9y1 == 1 && (x1y0 + x0y1 == 0)) {
					AddTile(3, position, OcclusionTileset.TileRotation.down);
					continue;
				} 
				if (x0y1 == 1 && x9y0 == 1 && x1y1 == 1 && x9y9 == 1 && (x1y0 + x0y9 == 0)) {
					AddTile(3, position, OcclusionTileset.TileRotation.right);
					continue;
				}
				if (x0y9 == 1 && x1y0 == 1 && x9y9 == 1 && x1y1 == 1 && (x9y0 + x0y1 == 0)) {
					AddTile(3, position, OcclusionTileset.TileRotation.left);
					continue;
				} 

				// SMOOTH CORNERS COMPLEX
				if (x0y1 == 1 && x1y0 == 1 && x9y1 == 1 && (x1y9 + x9y0 + x0y9 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.up, false, false);
					continue;
				} 
				if (x0y1 == 1 && x1y0 == 1 && x1y9 == 1 && (x9y1 + x9y0 + x0y9 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.right, true, false);
					continue;
				} 
				if (x0y9 == 1 && x9y0 == 1 && x1y9 == 1 && (x9y1 + x1y0+ x0y1 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.down, false, false);
					continue;
				} 
				if (x0y9 == 1 && x9y0 == 1 && x9y1 == 1 && (x1y9 + x1y0+ x0y1 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.left, true, false);
					continue;
				} 
				if (x9y0 == 1 && x0y1 == 1 && x1y1 == 1 && (x9y9 + x0y9 + x1y0 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.up, true, false);
					continue;
				}
				if (x1y0 == 1 && x0y9 == 1 && x1y1 == 1 && (x9y9 + x0y1 + x9y0 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.left, false, false);
					continue;
				}
				if (x1y0 == 1 && x0y9 == 1 && x9y9 == 1 && (x1y1 + x0y1 + x9y0 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.down, true, false);
					continue;
				}
				if (x9y0 == 1 && x0y1 == 1 && x9y9 == 1 && (x1y1 + x0y9 + x1y0 == 0)) {
					AddTile(4, position, OcclusionTileset.TileRotation.right, false, false);
					continue;
				}

				if (x0y1 == 1 && (x9y0  + x1y0 + x1y1 + x9y1 == 0)) {
					AddTile(5, position, OcclusionTileset.TileRotation.up);
				}
				if (x0y9 == 1 && (x9y0 + x1y0+ x1y9 + x9y9 == 0)) {
					AddTile(5, position, OcclusionTileset.TileRotation.down);
				}
				if (x9y0 == 1 && (x0y1 +x0y9 + x9y9 + x9y1 == 0)) {
					AddTile(5, position, OcclusionTileset.TileRotation.right);
				}
				if (x1y0 == 1 && (x0y1 + x0y9 + x1y9 + x1y1 == 0)) {
					AddTile(5, position, OcclusionTileset.TileRotation.left);
				}

				if (x0y1 == 1 && x1y1 == 1 && (x9y0  + x1y0 + x9y1 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.up, false, false);
				}
				if (x0y1 == 1 && x9y1 == 1 && (x9y0+ x1y0+ x1y1 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.up, true, false);
				}
				if (x0y9 == 1 && x9y9 == 1 && (x1y0+ x9y0 + x1y9 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.down, false, false);
				}
				if (x0y9 == 1 && x1y9 == 1 && (x1y0+ x9y0 + x9y9 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.down, true, false);
				}
				if (x1y0 == 1 && x1y1 == 1 && (x0y9+ x0y1 + x1y9 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.left, false, true);
				}
				if (x1y0 == 1 && x1y9 == 1 && (x0y9  + x0y1 + x1y1 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.left, false, false);	
				}
				if (x9y0 == 1 && x9y1 == 1 && (x0y9+ x0y1 + x9y9 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.right, false, false);
				}
				if (x9y0 == 1 && x9y9 == 1 && (x0y9 + x0y1 + x9y1 == 0)) {
					AddTile(7, position, OcclusionTileset.TileRotation.left, true, false);
				} 
				
				// SIMPLE UNFINISHED
				if (x0y1 == 1 && x1y1 == 1 && x9y1 == 1 && (x9y0 + x1y0 == 0)) {
					AddTile(6, position, OcclusionTileset.TileRotation.up, true, false);
				}
				if (x0y9 == 1 && x1y9 == 1 && x9y9 == 1 && (x1y0+ x9y0 == 0)) {
					AddTile(6, position, OcclusionTileset.TileRotation.down, true, false);
				}
				if (x1y0 == 1 && x1y1 == 1 && x1y9 == 1 && (x0y9 + x0y1 == 0)) {
					AddTile(6, position, OcclusionTileset.TileRotation.right, true, false);
				}
				if (x9y0 == 1 && x9y1 == 1 && x9y9 == 1 && (x0y9 + x0y1 == 0)) {
					AddTile(6, position, OcclusionTileset.TileRotation.left, true, false);
				}
	
				if (x0y1 == 1 && x9y0 == 1 && x0y9 == 1 && x1y9 == 1 && (x1y1 + x1y0 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.up, false, false);
					continue;
				}
				if (x1y0 == 1 && x0y1 == 1 && x9y0 == 1 && x9y9 == 1 && (x1y9 + x0y9 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.left, false, false);
					continue;
				}
				if (x9y0 == 1 && x0y9 == 1 && x1y0 == 1 && x1y1 == 1 && (x9y1 + x0y1 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.right, false, false);
					continue;
				}
				if (x0y9 == 1 && x1y0 == 1 && x0y1 == 1 && x9y1 == 1 && (x9y9 + x9y0 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.down, false, false);
					continue;
				}
				if (x1y1 == 1 && x0y1 == 1 && x9y0 == 1 && x0y9 == 1 && (x1y9 + x1y0 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.up, false, true);
					continue;
				}
				if (x9y9 == 1 && x0y9 == 1 && x1y0 == 1 && x0y1 == 1 && (x9y1 + x9y0 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.down, false, true);
					continue;
				}
				if (x9y1 == 1 && x9y0 == 1 && x0y9 == 1 && x1y0 == 1 && (x1y1 + x0y1 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.right, false, true);
					continue;
				}
				if (x1y9 == 1 && x1y0 == 1 && x0y1 == 1 && x9y0 == 1 && (x9y9 + x0y9 == 0)) {
					AddTile(9, position, OcclusionTileset.TileRotation.left, false, true);
					continue;
				}

				if (x9y0 == 1 && x0y9 == 1 && (x1y9 + x9y1 + x1y0 + x0y1 == 0)) {
					AddTile(2, position, OcclusionTileset.TileRotation.up);
				}
				if (x1y0 == 1 && x0y9 == 1 && (x9y9 + x1y1 + x9y0 + x0y1 == 0)) {
					AddTile(2, position, OcclusionTileset.TileRotation.right);
				}
				if (x1y0 == 1 && x0y1 == 1 && (x1y9 + x9y1 + x9y0 + x0y9 == 0)) {
					AddTile(2, position, OcclusionTileset.TileRotation.down);
				}
				if (x9y0 == 1 && x0y1 == 1 && (x9y9 + x1y1 + x0y9 + x1y0 == 0)) {
					AddTile(2, position, OcclusionTileset.TileRotation.left);
				}

				if (x0y1 == 1 && x9y0 == 1 && x0y9 == 1 && (x1y9 + x1y0 + x1y1 == 0)) {
					AddTile(10, position, OcclusionTileset.TileRotation.up);
				}
				if (x0y1 == 1 && x1y0 == 1 && x0y9 == 1 && (x9y9 + x9y0 + x9y1 == 0)) {
					AddTile(10, position, OcclusionTileset.TileRotation.down);
				}
				if (x9y0 == 1 && x0y9 == 1 && x1y0 == 1 && (x1y1 + x0y1 + x9y1 == 0)) {
					AddTile(10, position, OcclusionTileset.TileRotation.right);
				}
				if (x1y0 == 1 && x0y1 == 1 && x9y0 == 1 && (x9y9 + x0y9 + x1y9 == 0)) {
					AddTile(10, position, OcclusionTileset.TileRotation.left);
				}	
			}
		 }
    }

	void AddTile(int id, Vector2Int tilePosition, OcclusionTileset.TileRotation tileRotation, bool flipX = false, bool flipY = false) {
		tilemapMesh.AddTile(id, tilePosition, tileRotation, Color.white, flipX, flipY);
    }

	void ExportMesh() {
		GameObject gameObject = GetChild();

        gameObject.transform.parent = transform;
		gameObject.transform.localPosition = new Vector3(0, 0, 0);

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null) {

			Material material = null;
			if (Lighting2D.QualitySettings.HDR == true) {
				material = new Material(Shader.Find("Light2D/Internal/Multiply HDR"));
			} else {
				material = new Material(Shader.Find("Mobile/Particles/Multiply"));
			}
			
        	material.mainTexture =  tileset.texture;

			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
		}
        
        meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null) {
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}

        meshFilter.mesh = tilemapMesh.Export();

	}

	GameObject GetChild() {
		foreach(Transform t in transform) {
			if (t.gameObject.name == "Occlusion") {
				return(t.gameObject);
			}
		}
		return(new GameObject("Occlusion"));
	}

	public class RectangleMap {
		public List<RectangleTile> mapTiles = new List<RectangleTile>();
		public RectangleTile[,] mapArray;
		public int width;
		public int height;

		public int GetUnwalkableInt(int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
				return(0);
			}
	
			if (mapArray[x, y] != null) {
				return(1);
			} else {
				return(0);
			}
		}

		public void Init() {
			mapArray = new RectangleTile[width, height];

			foreach(RectangleTile tile in mapTiles) {
				mapArray[tile.position.x + width / 2, tile.position.y + height / 2] = tile;
			}
		}
	}

	public class RectangleTile {
		public Vector2Int position;
	}

}