using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LightTilemapCollider;

public class LightingTilemapRoomTransform {
    private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

    private Vector2 scale = Vector2.one;
    public Vector2 position = Vector2.one;
	public float rotation = 0;
	public Vector3 tilemapAnchor = Vector3.zero;
	public Vector3 tilemapCellSize = Vector3.zero;
	public Vector3 tilemapGapSize = Vector3.zero;

	public void Update(LightTilemapRoom2D tilemapRoom2D) {
		Transform transform = tilemapRoom2D.transform;

	    Vector2 position2D = LightingPosition.GetPosition2D(transform.position);
		Vector2 scale2D = transform.lossyScale;
		float rotation2D = transform.rotation.eulerAngles.z;

		update = false;

        if (scale != scale2D) {
			scale = scale2D;

			update = true;
		}

        if (position != position2D) {
			position = position2D;

			update = true;
		}

		if (rotation != rotation2D) {
			rotation = rotation2D;

			update = true;
		}

		if (tilemapRoom2D.mapType != MapType.SuperTilemapEditor) {
			Tilemap tilemap = GetTilemap(tilemapRoom2D.gameObject);

			if (tilemap) {
				if (tilemapAnchor != tilemap.tileAnchor) {
					tilemapAnchor = tilemap.tileAnchor;
					update = true;
				}
			}

			Grid grid = GetGrid(tilemapRoom2D.gameObject);

			if (grid) {
				if(tilemapCellSize != grid.cellSize) {
					tilemapCellSize = grid.cellSize;

					update = true;
				}

				if (tilemapGapSize != grid.cellGap) {
					tilemapGapSize = grid.cellGap;

					update = true;
				}
			}
		}
	}
	
	Tilemap tilemap = null;
	public Tilemap GetTilemap(GameObject gameObject) {
		if (tilemap == null) {
			tilemap = gameObject.GetComponent<Tilemap>();
		}
		return(tilemap);
	}

	Grid grid = null;
	public Grid GetGrid(GameObject gameObject) {
		if (grid == null) {
			Tilemap tilemap = GetTilemap(gameObject);

			if (tilemap != null) {
				grid = tilemap.layoutGrid;
			}
			
		}
		return(grid);
	}
}
