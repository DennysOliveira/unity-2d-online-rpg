using System.Collections.Generic;
using UnityEngine;
using System.Linq;


static public class Math2D {

	static public Vector2 ClosestPointOnLine(Vector2 vPoint, Vector2 vA, Vector2 vB) {
		Vector2 v;
		v.x = (vB.x - vA.x);
		v.y = (vB.y - vA.y);
    
		float distance = Mathf.Sqrt(v.x * v.x + v.y * v.y);
		v.x = v.x / distance;
		v.y = v.y / distance;

		Vector2 v1;
		v1.x = vPoint.x - vA.x;
		v1.y = vPoint.y - vA.y;

        float t = Vector2.Dot(v, v1);
    
        if (t <= 0) {
            return vA;
        }
            
        if (t >= Vector2.Distance(vA, vB)) {
            return vB;
        }

		vA.x += v.x * t;
		vA.y += v.y * t;
        
        return vA;
    }

	public static Vector3 GetPitchYawRollRad(Quaternion rotation) {
		float roll = Mathf.Atan2(2*rotation.y*rotation.w - 2*rotation.x*rotation.z, 1 - 2*rotation.y*rotation.y - 2*rotation.z*rotation.z);
		float pitch = Mathf.Atan2(2*rotation.x*rotation.w - 2*rotation.y*rotation.z, 1 - 2*rotation.x*rotation.x - 2*rotation.z*rotation.z);
		float yaw = Mathf.Asin(2*rotation.x*rotation.y + 2*rotation.z*rotation.w);
		
		return new Vector3(pitch, roll, yaw);
	}

	public static Vector3 GetPitchYawRollDeg(Quaternion rotation) {
		Vector3 radResult = GetPitchYawRollRad(rotation);
		return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
	}

	public static Rect GetBounds(List<Vector2D> pointsList) {
		double rMinX = 1e+10f;
		double rMinY = 1e+10f;
		double rMaxX = -1e+10f;
		double rMaxY = -1e+10f;

		foreach (Vector2D id in pointsList) {
			rMinX = System.Math.Min (rMinX, id.x);
			rMinY = System.Math.Min (rMinY, id.y);
			rMaxX = System.Math.Max (rMaxX, id.x);
			rMaxY = System.Math.Max (rMaxY, id.y);
		}

		return(new Rect((float)rMinX, (float)rMinY, (float)System.Math.Abs(rMinX - rMaxX), (float)System.Math.Abs(rMinY - rMaxY))); 
	}

	public static Rect GetBounds(Pair2D pair) {
		double rMinX = 1e+10f;
		double rMinY = 1e+10f;
		double rMaxX = -1e+10f;
		double rMaxY = -1e+10f;

		Vector2D id = pair.A;
		rMinX = System.Math.Min (rMinX, id.x);
		rMinY = System.Math.Min (rMinY, id.y);
		rMaxX = System.Math.Max (rMaxX, id.x);
		rMaxY = System.Math.Max (rMaxY, id.y);
	
		id = pair.B;
		rMinX = System.Math.Min (rMinX, id.x);
		rMinY = System.Math.Min (rMinY, id.y);
		rMaxX = System.Math.Max (rMaxX, id.x);
		rMaxY = System.Math.Max (rMaxY, id.y);

		return(new Rect((float)rMinX, (float)rMinY, (float)System.Math.Abs(rMinX - rMaxX), (float)System.Math.Abs(rMinY - rMaxY))); 
	}


	public static bool PolyInPoly(Polygon2D polyA, Polygon2D polyB) {
		foreach (Pair2D p in Pair2D.GetList(polyB.pointsList)) {
			if (PointInPoly (p.A, polyA) == false) {
				return(false);
			}
		}

		if (PolyIntersectPoly (polyA, polyB) == true) {
			return(false);
		}
		
		return(true);
	}

	// Is it not finished?
	public static bool PolyCollidePoly(Polygon2D polyA, Polygon2D polyB) {
		if (PolyIntersectPoly (polyA, polyB) == true) {
			return(true);
		}

		if (PolyInPoly (polyA, polyB) == true) {
			return(true);
		}

		if (PolyInPoly (polyB, polyA) == true) {
			return(true);
		}
		
		return(false);
	}

	public static bool PolyIntersectPoly(Polygon2D polyA, Polygon2D polyB) {
		foreach (Pair2D a in Pair2D.GetList(polyA.pointsList)) {
			foreach (Pair2D b in Pair2D.GetList(polyB.pointsList)) {
				if (LineIntersectLine (a, b)) {
					return(true);
				}
			}
		}

		return(false);
	}

	public static bool SliceIntersectPoly(List <Vector2D> slice, Polygon2D poly) {
		Pair2D pairA = new Pair2D(null,  null);
		foreach (Vector2D pointA in slice) {
			pairA.B = pointA;
			
			if (pairA.A != null && pairA.B != null) {
				Pair2D pairB = new Pair2D(new Vector2D(poly.pointsList.Last()),  null);
				foreach (Vector2D pointB in poly.pointsList) {
					pairB.B = pointB;

					if (LineIntersectLine (pairA, pairB)) {
						return(true);
					}

					pairB.A = pointB;
				}
			}

			pairA.A = pointA;
		}

		return(false);
	}

	public static bool SliceIntersectSlice(List <Vector2D> sliceA, List <Vector2D> sliceB) {
		Pair2D pairA = new Pair2D(null,  null);
		foreach (Vector2D pointA in sliceA) {
			pairA.B = pointA;

			if (pairA.A != null && pairA.B != null) {

				Pair2D pairB = new Pair2D(null,  null);
				foreach (Vector2D pointB in sliceB) {
					pairB.B = pointB;

					if (pairB.A != null && pairB.B != null) {
						if (LineIntersectLine (pairA, pairB)) {
							return(true);
						}
					}

					pairB.A = pointB;
				}
			}

			pairA.A = pointA;
		}

		return(false);
	}
		
	public static bool LineIntersectPoly(Pair2D line, Polygon2D poly) {
		Pair2D pair = new Pair2D(new Vector2D(poly.pointsList.Last()), new Vector2D(Vector2.zero));
		foreach (Vector2D point in poly.pointsList) {
			pair.B = point;

			if (LineIntersectLine (line, pair)) {
				return(true);
			}
			
			pair.A = point;
		}
		
		return(false);
	}

	public static bool LineIntersectLine(Pair2D lineA, Pair2D lineB) {
		if (GetPointLineIntersectLine (lineA, lineB) != null) {
			return(true);
		}

		return(false);
	}

	public static bool SliceIntersectItself(List<Vector2D> slice) {
		Pair2D pairA = new Pair2D(null,  null);
		foreach (Vector2D va in slice) {
			pairA.B = va;

			if (pairA.A != null && pairA.B != null) {

				Pair2D pairB = new Pair2D(null,  null);
				foreach (Vector2D vb in slice) {
					pairB.B = vb;

					if (pairB.A != null && pairB.B != null) {
						if (GetPointLineIntersectLine (pairA, pairB) != null) {
							if (pairA.A != pairB.A && pairA.B != pairB.B && pairA.A != pairB.B && pairA.B != pairB.A) {
								return(true);
							}
						}
					}
					pairB.A = vb;
				}
			}
			
			pairA.A = va;
		}
		
		return(false);
	}

	public static Vector2D GetPointLineIntersectLine(Pair2D lineA, Pair2D lineB) {
		double ay_cy, ax_cx, px, py;
		double dx_cx = lineB.B.x - lineB.A.x;
		double dy_cy = lineB.B.y - lineB.A.y;
		double bx_ax = lineA.B.x - lineA.A.x;
		double by_ay = lineA.B.y - lineA.A.y;
		double de = bx_ax * dy_cy - by_ay * dx_cx;
		double tor = 1E-10;

		if (System.Math.Abs(de) < 0.0001d) {
			return(null);
		}	

		if (de > - tor && de < tor) {
			return(null);
		}

		ax_cx = lineA.A.x - lineB.A.x;
		ay_cy = lineA.A.y - lineB.A.y;

		double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
		double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

		if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
			return(null);
		}

		px = lineA.A.x + r * bx_ax;
		py = lineA.A.y + r * by_ay;

		return(new Vector2D (px, py));
	}

	public static Vector2? GetPointLineIntersectLine2(Pair2D lineA, Pair2D lineB) {
		double ay_cy, ax_cx;
		double dx_cx = lineB.B.x - lineB.A.x;
		double dy_cy = lineB.B.y - lineB.A.y;
		double bx_ax = lineA.B.x - lineA.A.x;
		double by_ay = lineA.B.y - lineA.A.y;
		double de = bx_ax * dy_cy - by_ay * dx_cx;
		double tor = 1E-10;

		if (System.Math.Abs(de) < 0.0001d) {
			return(null);
		}	

		if (de > - tor && de < tor) {
			return(null);
		}

		ax_cx = lineA.A.x - lineB.A.x;
		ay_cy = lineA.A.y - lineB.A.y;

		double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
		double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

		if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
			return(null);
		}

		float px = (float)(lineA.A.x + r * bx_ax);
		float py = (float)(lineA.A.y + r * by_ay);

		return(new Vector2 (px, py));
	}

	const double tor = 1E-10;

	public static Vector2? GetPointLineIntersectLine3(Vector2 a_a, Vector2 a_b, Vector2 b_a, Vector2 b_b) {
		double dx_cx = b_b.x - b_a.x;
		double dy_cy = b_b.y - b_a.y;
		double bx_ax = a_b.x - a_a.x;
		double by_ay = a_b.y - a_a.y;
		double de = bx_ax * dy_cy - by_ay * dx_cx;

		if (System.Math.Abs(de) < 0.0001d) {
			return(null);
		}	

		if (de > - tor && de < tor) {
			return(null);
		}

		double ax_cx = a_a.x - b_a.x;
		double ay_cy = a_a.y - b_a.y;

		double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
		double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

		if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
			return(null);
		}

		return(new Vector2 ((float)(a_a.x + r * bx_ax),  (float)(a_a.y + r * by_ay)));
	}

	public static bool GetPointLineIntersectLine4(Vector2 a_a, Vector2 a_b, Vector2 b_a, Vector2 b_b) {
		double dx_cx = b_b.x - b_a.x;
		double dy_cy = b_b.y - b_a.y;
		double bx_ax = a_b.x - a_a.x;
		double by_ay = a_b.y - a_a.y;
		double de = bx_ax * dy_cy - by_ay * dx_cx;

		if (System.Math.Abs(de) < 0.0001d) {
			return(false);
		}	

		if (de > - tor && de < tor) {
			return(false);
		}

		double ax_cx = a_a.x - b_a.x;
		double ay_cy = a_a.y - b_a.y;

		double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
		double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

		if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
			return(false);
		}

		return(true);
	}





	private static Pair2D pointInPoly_Pair2D = Pair2D.Zero();
	public static bool PointInPoly(Vector2D point, Polygon2D poly) {
		int count = poly.pointsList.Count;
		if (count < 3) {
			return(false);
		}

		int total = 0;
		int diff = 0;

		Pair2D id = pointInPoly_Pair2D;
		id.A = poly.pointsList[count - 1];

		Vector2D p;
		
		for(int i = 0; i < count; i++) {
			p = poly.pointsList[i];
			
			id.B = p;

			diff = (GetQuad (point, id.A) - GetQuad (point, id.B));

			switch (diff) {
				case -2: case 2:
					if ((id.B.x - (((id.B.y - point.y) * (id.A.x - id.B.x)) / (id.A.y - id.B.y))) < point.x)
						diff = -diff;

					break;

				case 3:
					diff = -1;
					break;

				case -3:
					diff = 1;
					break;

				default:
					break;   
			}

			total += diff;

			id.A = id.B;
		}

		return(Mathf.Abs(total) == 4);
	}

	private static int GetQuad(Vector2D axis, Vector2D vert) {
		if (vert.x < axis.x) {
			if (vert.y < axis.y) {
				return(1);
			}
			return(4);
		}
		if (vert.y < axis.y) {
			return(2);
		}
		return(3);
	}





	public static bool PointInPoly(Vector2 point, Polygon2 poly) {
		int count = poly.points.Length;
		if (count < 3) {
			return(false);
		}

		int total = 0;
		int diff = 0;

		Vector2 A = poly.points[count - 1];
		Vector2 B;

		Vector2 p;
		
		for(int i = 0; i < count; i++) {
			p = poly.points[i];
			
			B = p;

			diff = (GetQuad (point, A) - GetQuad (point, B));

			switch (diff) {
				case -2: case 2:
					if ((B.x - (((B.y - point.y) * (A.x - B.x)) / (A.y - B.y))) < point.x)
						diff = -diff;

					break;

				case 3:
					diff = -1;
					break;

				case -3:
					diff = 1;
					break;

				default:
					break;   
			}

			total += diff;

			A = B;
		}

		return(Mathf.Abs(total) == 4);
	}

	private static int GetQuad(Vector2 axis, Vector2 vert) {
		if (vert.x < axis.x) {
			if (vert.y < axis.y) {
				return(1);
			}
			return(4);
		}
		if (vert.y < axis.y) {
			return(2);
		}
		return(3);
	}



























		
	// Getting List is Slower
	public static List <Vector2D> GetListLineIntersectPoly(Pair2D line, Polygon2D poly) {
		List <Vector2D> result = new List <Vector2D>() ;

		Pair2D pair = new Pair2D(new Vector2D(poly.pointsList.Last()),  null);
		foreach (Vector2D point in poly.pointsList) {
			pair.B = point;

			Vector2D intersection = GetPointLineIntersectLine (line, pair);
			if (intersection != null) {
				result.Add(intersection);
			}

			pair.A = point;
		}
		return(result);
	}

	public static List<Vector2D> GetListLineIntersectSlice(Pair2D pair, List<Vector2D> slice) {
		List<Vector2D> resultList = new List<Vector2D> ();
		
		Pair2D id = new Pair2D(null,  null);
		foreach (Vector2D point in slice) {
			id.B = point;

			if (id.A != null && id.B != null) {
				Vector2D result = GetPointLineIntersectLine(id, pair);
				if (result != null) {
					resultList.Add(result);
				}
			}

			id.A = point;
		}
		return(resultList);
	}

	public static Vector2 ReflectAngle(Vector2 v, float wallAngle) {
		//normal vector to the wall
		Vector2 n = new Vector2(Mathf.Cos(wallAngle + Mathf.PI / 2), Mathf.Sin(wallAngle + Mathf.PI / 2));

		// p is the projection of V onto the normal
		float dotproduct = v.x * n.x + v.y * n.y;

		// the velocity after hitting the wall is V - 2p, so just subtract 2*p from V
		return(new Vector2(v.x - 2f * (dotproduct * n.x), v.y - 2f * (dotproduct * n.y)));
	}

	static public bool PolygonIntersectCircle(Polygon2D poly, Vector2D circle, float radius) {
		foreach (Pair2D id in Pair2D.GetList(poly.pointsList)) {
			if (LineIntersectCircle(id, circle, radius) == true) {
				return(true);
			}
		}
		return(false);
	}
	
	static public bool SliceIntersectCircle(List<Vector2D> points, Vector2D circle, float radius) {
		foreach (Pair2D id in Pair2D.GetList(points, false)) {
			if (LineIntersectCircle(id, circle, radius) == true) {
				return(true);
			}
		}
		return(false);
	}

	static public bool LineIntersectCircle(Pair2D line, Vector2D circle, float radius) {
		double sx = line.B.x - line.A.x;
		double sy = line.B.y - line.A.y;

		double q = ((circle.x - line.A.x) * (line.B.x - line.A.x) + (circle.y - line.A.y) * (line.B.y - line.A.y)) / (sx * sx + sy * sy);
			
		if (q < 0.0f) {
			q = 0.0f;
		} else if (q > 1.0) {
			q = 1.0f;
		}

		double dx = circle.x - ((1.0f - q) * line.A.x + q * line.B.x);
		double dy = circle.y - ((1.0f - q) * line.A.y + q * line.B.y);

		if (dx * dx + dy * dy < radius * radius) {
			return(true);
		} else {
			return(false);
		}
	}

	public static List<Vector2D> GetConvexHull(List<Vector2D> points) {
		//If we have just 3 points, then they are the convex hull, so return those
		if (points.Count == 3) {
			//These might not be ccw, and they may also be colinear
			return points;
		}

		//If fewer points, then we cant create a convex hull
		if (points.Count < 3) {
			return null;
		}
			
		//The list with points on the convex hull
		List<Vector2D> convexHull = new List<Vector2D>();

		//Step 1. Find the vertex with the smallest x coordinate
		//If several have the same x coordinate, find the one with the smallest z
		Vector2D startVertex = points[0];

		Vector2 startPos = startVertex.ToVector2() ;

		for (int i = 1; i < points.Count; i++) {
			Vector2 testPos = points[i].ToVector2() ;

			//Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
			if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.y < startPos.y))
			{
				startVertex = points[i];

				startPos = startVertex.ToVector2() ;
			}
		}

		//This vertex is always on the convex hull
		convexHull.Add(startVertex);

		points.Remove(startVertex);

		//Step 2. Loop to generate the convex hull
		Vector2D currentPoint = convexHull[0];

		//Store colinear points here - better to create this list once than each loop
		List<Vector2D> colinearPoints = new List<Vector2D>();

		int counter = 0;

		while (true) {
			//After 2 iterations we have to add the start position again so we can terminate the algorithm
			//Cant use convexhull.count because of colinear points, so we need a counter
			if (counter == 2) {            
				points.Add(convexHull[0]);
			}

			if (points.Count < 1) {
				break; // ERROR!!!
			}

			//Pick next point randomly
			Vector2D nextPoint = points[Random.Range(0, points.Count)];

			//To 2d space so we can see if a point is to the left is the vector ab
			Vector2 a = currentPoint.ToVector2();
			Vector2 b = nextPoint.ToVector2();

			//Test if there's a point to the right of ab, if so then it's the new b
			for (int i = 0; i < points.Count; i++) {
				//Dont test the point we picked randomly
				if (points[i].Equals(nextPoint)) {
					continue;
				}

				Vector2 c = points[i].ToVector2() ;

				//Where is c in relation to a-b
				// < 0 -> to the right
				// = 0 -> on the line
				// > 0 -> to the left
				float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

				//Colinear points
				//Cant use exactly 0 because of floating point precision issues
				//This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
				float accuracy = 0.00001f;

				if (relation < accuracy && relation > -accuracy) {
					colinearPoints.Add(points[i]);
				}
				//To the right = better point, so pick it as next point on the convex hull
				else if (relation < 0f) {
					nextPoint = points[i];

					b = nextPoint.ToVector2();

					//Clear colinear points
					colinearPoints.Clear();
				}
				//To the left = worse point so do nothing
			}



			//If we have colinear points
			if (colinearPoints.Count > 0) {
				colinearPoints.Add(nextPoint);

				//Sort this list, so we can add the colinear points in correct order
				colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n.ToVector2() - currentPoint.ToVector2())).ToList();

				convexHull.AddRange(colinearPoints);

				currentPoint = colinearPoints[colinearPoints.Count - 1];

				//Remove the points that are now on the convex hull
				for (int i = 0; i < colinearPoints.Count; i++) {
					points.Remove(colinearPoints[i]);
				}

				colinearPoints.Clear();
			} else {
				convexHull.Add(nextPoint);

				points.Remove(nextPoint);

				currentPoint = nextPoint;
			}

			//Have we found the first point on the hull? If so we have completed the hull
			if (currentPoint.Equals(convexHull[0])) {
				//Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
				convexHull.RemoveAt(convexHull.Count - 1);

				break;
			}

			counter ++;
		}

		return convexHull;
	}

	public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p) {
		return ((a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x));
	}

}
