using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingOcclussion {
    VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();
    
    public List<List<Pair2D>> polygonPoints = new List<List<Pair2D>>();
    public List<List<Pair2D>> outlinePoints = new List<List<Pair2D>>();
    public List<List<DoublePair2>> polygonPairs = new List<List<DoublePair2>>();
    
    public LightingOcclussion(LightingOcclusionShape shape, float size) {
	
        spriteRenderer.sprite = shape.spritePhysicsShape.GetOriginalSprite();

        if (shape.spritePhysicsShape.GetSpriteRenderer() != null) {
            spriteRenderer.flipX = shape.spritePhysicsShape.GetSpriteRenderer().flipX;
            spriteRenderer.flipY = shape.spritePhysicsShape.GetSpriteRenderer().flipY;
        } else {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        polygonPoints.Clear();
       	outlinePoints.Clear();
        polygonPairs.Clear();

       	List<Polygon2> polygons = null;

		switch(shape.shadowType) {
			case LightOcclusion2D.ShadowType.Collider:
				polygons = new List<Polygon2>();

				List<Polygon2> polygons3 = shape.GetPolygonsLocal();

				foreach(Polygon2 p in polygons3) {
					Polygon2 poly = p.Copy();
				
					polygons.Add(poly);
				}
				

			break;

			case LightOcclusion2D.ShadowType.SpritePhysicsShape:
				SpriteRenderer sRenderer = shape.spritePhysicsShape.GetSpriteRenderer();
			
				SpriteExtension.PhysicsShape customShape = SpriteExtension.PhysicsShapeManager.RequesCustomShape(sRenderer.sprite);

				List<Polygon2> polygons2 = customShape.Get();

				polygons = new List<Polygon2>();

				foreach(Polygon2 p in polygons2) {
					Polygon2 poly = p.Copy();
					polygons.Add(poly);
				}

			break;
		}

        if (polygons == null || polygons.Count < 1) {
            return;
        }

        foreach(Polygon2 polygon in polygons) {
            polygon.Normalize();
            
            polygonPoints.Add(Pair2D.GetList(polygon.points));
            outlinePoints.Add(Pair2D.GetList(PreparePolygon(polygon, size).points));
            polygonPairs.Add(DoublePair2.GetList(polygon.points));
        }
    }

    static public Polygon2 PreparePolygon(Polygon2 polygon, float size) {
		Polygon2D newPolygon = new Polygon2D();

		DoublePair2 pair = new DoublePair2 (Vector2.zero, Vector2.zero, Vector2.zero);
		Vector2D pairA = Vector2D.Zero();
		Vector2D pairC = Vector2D.Zero();
		Vector2D vecA = Vector2D.Zero();
		Vector2D vecC = Vector2D.Zero();

		for(int i = 0; i < polygon.points.Length; i++) {
			Vector2 pB = polygon.points[i];
			
			int indexB = i;

			int indexA = (indexB - 1);
			if (indexA < 0) {
				indexA += polygon.points.Length;
			}

			int indexC = (indexB + 1);
			if (indexC >= polygon.points.Length) {
				indexC -= polygon.points.Length;
			}

			pair.A = polygon.points[indexA];
			pair.B = pB;
			pair.C = polygon.points[indexC];

			float rotA = pair.B.Atan2(pair.A);
			float rotC = pair.B.Atan2(pair.C);

			pairA.x = pair.A.x;
			pairA.y = pair.A.y;
			pairA.Push(rotA - Mathf.PI / 2, -size);

			pairC.x = pair.C.x;
			pairC.y = pair.C.y;
			pairC.Push(rotC + Mathf.PI / 2, -size);
			
			vecA.x = pair.B.x;
			vecA.y = pair.B.y;
			vecA.Push(rotA - Mathf.PI / 2, -size);
			vecA.Push(rotA, 110f);

			vecC.x = pair.B.x;
			vecC.y = pair.B.y;
			vecC.Push(rotC + Mathf.PI / 2, -size);
			vecC.Push(rotC, 110f);

			Vector2D result = Math2D.GetPointLineIntersectLine(new Pair2D(pairA, vecA), new Pair2D(pairC, vecC));

			if (result != null) {
				newPolygon.AddPoint(result);
			}
		}

		return(new Polygon2(newPolygon));
	}   
}