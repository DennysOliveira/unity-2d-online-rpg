using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Polygon2DTriangulation
{
	public class PolygonUtil
	{
		public enum PolyUnionError
		{
			None,
			NoIntersections,
			Poly1InsidePoly2,
			InfiniteLoop
		}

		[Flags]
		public enum PolyOperation : uint
		{
			None = 0,
			Union = 1 << 0,
			Intersect = 1 << 1,
			Subtract = 1 << 2,
		}

		public static Point2DList.WindingOrderType CalculateWindingOrder(IList<Point2D> l)
		{
			double area = 0.0;
			for (int i = 0; i < l.Count; i++) {
				int j = (i + 1) % l.Count;
				area += l[i].X * l[j].Y;
				area -= l[i].Y * l[j].X;
			}
			area /= 2.0f;

			if (area < 0.0)
				return Point2DList.WindingOrderType.CW;
			else if (area > 0.0)
				return Point2DList.WindingOrderType.CCW;
				
			return Point2DList.WindingOrderType.Unknown;
		}

		public static bool PolygonsAreSame2D(IList<Point2D> poly1, IList<Point2D> poly2)
		{
			int numVerts1 = poly1.Count;
			int numVerts2 = poly2.Count;

			if (numVerts1 != numVerts2)
				return false;
			const double kEpsilon = 0.01;
			const double kEpsilonSq = kEpsilon * kEpsilon;

			Point2D vdelta = new Point2D(0.0, 0.0);
			for (int k = 0; k < numVerts2; ++k)
			{
				vdelta.Set(poly1[0]);
				vdelta.Subtract(poly2[k]);

				if (vdelta.MagnitudeSquared() < kEpsilonSq)
				{
					int matchedVertIndex = k;
					bool bReverseSearch = false;
					while (true)
					{
						bool bMatchFound = true;
						for (int i = 1; i < numVerts1; ++i)
						{
							if (!bReverseSearch)
				 			 	  ++k;
							else
								{
				    				--k;
				    				if (k < 0)
				       					k = numVerts2 - 1;
								}

							vdelta.Set(poly1[i]);
							vdelta.Subtract(poly2[k % numVerts2]);
							if (vdelta.MagnitudeSquared() >= kEpsilonSq)
							{
							    if (bReverseSearch)
							        return false;
							    else
							    {
							        k = matchedVertIndex;
							        bReverseSearch = true;
							        bMatchFound = false;
							        break;
							    }
							}
						}

					if (bMatchFound)
						return true;
					}
				}
			}

			return false;
		}


	public static bool PointInPolygon2D(IList<Point2D> polygon, Point2D p)
	{
	if (polygon == null || polygon.Count < 3)
	{
	return false;
	}

	int numVerts = polygon.Count;
	Point2D p0 = polygon[numVerts - 1];
	bool bYFlag0 = (p0.Y >= p.Y) ? true : false;
	Point2D p1 = null;

	bool bInside = false;
	for (int j = 0; j < numVerts; ++j)
	{
	p1 = polygon[j];
	bool bYFlag1 = (p1.Y >= p.Y) ? true : false;
	if (bYFlag0 != bYFlag1)
	{
	if (((p1.Y - p.Y) * (p0.X - p1.X) >= (p1.X - p.X) * (p0.Y - p1.Y)) == bYFlag1)
	{
	bInside = !bInside;
	}
	}

	bYFlag0 = bYFlag1;
	p0 = p1;
	}

	return bInside;
	}

	public static bool PolygonsIntersect2D( IList<Point2D> poly1, Rect2D boundRect1,
	                    IList<Point2D> poly2, Rect2D boundRect2)
	{
	if (poly1 == null || poly1.Count < 3 || boundRect1 == null || poly2 == null || poly2.Count < 3 || boundRect2 == null)
	{
	return false;
	}

	if (!boundRect1.Intersects(boundRect2))
	{
	return false;
	}

	double epsilon = Math.Max(Math.Min(boundRect1.Width, boundRect2.Width) * 0.001f, MathUtil.EPSILON);

	int numVerts1 = poly1.Count;
	int numVerts2 = poly2.Count;
	for (int i = 0; i < numVerts1; ++i)
	{
	int lineEndVert1 = i + 1;
	if (lineEndVert1 == numVerts1)
	{
	lineEndVert1 = 0;
	}
	for (int j = 0; j < numVerts2; ++j)
	{
	int lineEndVert2 = j + 1;
	if (lineEndVert2 == numVerts2)
	{
	lineEndVert2 = 0;
	}
	Point2D tmp = null;
	if (TriangulationUtil.LinesIntersect2D(poly1[i], poly1[lineEndVert1], poly2[j], poly2[lineEndVert2], ref tmp, epsilon))
	{
	return true;
	}
	}
	}

	return false;
	}


	public bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1,
	                IList<Point2D> poly2, Rect2D boundRect2)
	{
	return PolygonContainsPolygon(poly1, boundRect1, poly2, boundRect2, true);
	}

	public static bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1,
	                        IList<Point2D> poly2, Rect2D boundRect2,
	                        bool runIntersectionTest)
	{
	if (poly1 == null || poly1.Count < 3 || poly2 == null || poly2.Count < 3)
	{
	return false;
	}

	if (runIntersectionTest)
	{
	if (poly1.Count == poly2.Count)
	{
	if (PolygonUtil.PolygonsAreSame2D(poly1, poly2))
	{
	return false;
	}
	}

	bool bIntersect = PolygonUtil.PolygonsIntersect2D(poly1, boundRect1, poly2, boundRect2);
	if (bIntersect)
	{
	return false;
	}
	}

	if (PolygonUtil.PointInPolygon2D(poly1, poly2[0]))
	{
	return true;
	}

	return false;
	}

	public static void ClipPolygonToEdge2D( Point2D edgeBegin,
	                    Point2D edgeEnd,
	                    IList<Point2D> poly,
	                    out List<Point2D> outPoly)
	{
	outPoly = null;
	if (edgeBegin == null ||
	edgeEnd == null ||
	poly == null ||
	poly.Count < 3)
	{
	return;
	}

	outPoly = new List<Point2D>();
	int lastVertex = poly.Count - 1;
	Point2D edgeRayVector = new Point2D(edgeEnd.X - edgeBegin.X, edgeEnd.Y - edgeBegin.Y);

	bool bLastVertexIsToRight = TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[lastVertex]) == Orientation.CW ? true : false;
	Point2D tempRay = new Point2D(0.0, 0.0);

	for (int curVertex = 0; curVertex < poly.Count; curVertex++)
	{
	bool bCurVertexIsToRight = TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[curVertex]) == Orientation.CW ? true : false;
	if (bCurVertexIsToRight)
	{
	if (bLastVertexIsToRight)
	{
	outPoly.Add(poly[curVertex]);
	}
	else
	{
	tempRay.Set(poly[curVertex].X - poly[lastVertex].X, poly[curVertex].Y - poly[lastVertex].Y);
	Point2D ptIntersection = new Point2D(0.0, 0.0);
	bool bIntersect = TriangulationUtil.RaysIntersect2D(poly[lastVertex], tempRay, edgeBegin, edgeRayVector, ref ptIntersection);
	if (bIntersect)
	{
	outPoly.Add(ptIntersection);
	outPoly.Add(poly[curVertex]);
	}
	}
	}
	else if (bLastVertexIsToRight)
	{
	tempRay.Set(poly[curVertex].X - poly[lastVertex].X, poly[curVertex].Y - poly[lastVertex].Y);
	Point2D ptIntersection = new Point2D(0.0, 0.0);
	bool bIntersect = TriangulationUtil.RaysIntersect2D(poly[lastVertex], tempRay, edgeBegin, edgeRayVector, ref ptIntersection);
	if (bIntersect)
	{
	outPoly.Add(ptIntersection);
	}
	}

	lastVertex = curVertex;
	bLastVertexIsToRight = bCurVertexIsToRight;
	}
	}


	public static void ClipPolygonToPolygon(IList<Point2D> poly, IList<Point2D> clipPoly, out List<Point2D> outPoly)
	{
	outPoly = null;
	if (poly == null || poly.Count < 3 || clipPoly == null || clipPoly.Count < 3)
	{
	return;
	}

	outPoly = new List<Point2D>(poly);
	int numClipVertices = clipPoly.Count;
	int lastVertex = numClipVertices - 1;

	for (int curVertex = 0; curVertex < numClipVertices; curVertex++)
	{
	List<Point2D> clippedPoly = null;
	Point2D edgeBegin = clipPoly[lastVertex];
	Point2D edgeEnd = clipPoly[curVertex];
	PolygonUtil.ClipPolygonToEdge2D(edgeBegin, edgeEnd, outPoly, out clippedPoly);
	outPoly.Clear();
	outPoly.AddRange(clippedPoly);

	lastVertex = curVertex;
	}
	}

	public static PolygonUtil.PolyUnionError PolygonUnion(Point2DList polygon1, Point2DList polygon2, out Point2DList union)
	{
	PolygonOperationContext ctx = new PolygonOperationContext();
	ctx.Init(PolygonUtil.PolyOperation.Union, polygon1, polygon2);
	PolygonUnionInternal(ctx);
	union = ctx.Union;
	return ctx.mError;
	}


	protected static void PolygonUnionInternal(PolygonOperationContext ctx)
	{
	Point2DList union = ctx.Union;
	if (ctx.mStartingIndex == -1)
	{
	switch (ctx.mError)
	{
	case PolygonUtil.PolyUnionError.NoIntersections:
	case PolygonUtil.PolyUnionError.InfiniteLoop:
	return;
	case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
	union.AddRange(ctx.mOriginalPolygon2);
	return;
	}
	}

	Point2DList currentPoly = ctx.mPoly1;
	Point2DList otherPoly = ctx.mPoly2;
	List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

	Point2D startingVertex = ctx.mPoly1[ctx.mStartingIndex];
	int currentIndex = ctx.mStartingIndex;
	int firstPoly2Index = -1;
	union.Clear();

	do
	{
	union.Add(currentPoly[currentIndex]);

	foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
	{
	if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
	{
	int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

	int comparePointIndex = otherPoly.NextIndex(otherIndex);
	Point2D comparePoint = otherPoly[comparePointIndex];
	bool bPointInPolygonAngle = false;
	if (currentPolyVectorAngles[comparePointIndex] == -1)
	{
	bPointInPolygonAngle = ctx.PointInPolygonAngle(comparePoint, currentPoly);
	currentPolyVectorAngles[comparePointIndex] = bPointInPolygonAngle ? 1 : 0;
	}
	else
	{
	bPointInPolygonAngle = (currentPolyVectorAngles[comparePointIndex] == 1) ? true : false;
	}

	if (!bPointInPolygonAngle)
	{
	if (currentPoly == ctx.mPoly1)
	{
	    currentPoly = ctx.mPoly2;
	    currentPolyVectorAngles = ctx.mPoly2VectorAngles;
	    otherPoly = ctx.mPoly1;
	    if (firstPoly2Index < 0)
	    {
	        firstPoly2Index = otherIndex;
	    }
	}
	else
	{
	    currentPoly = ctx.mPoly1;
	    currentPolyVectorAngles = ctx.mPoly1VectorAngles;
	    otherPoly = ctx.mPoly2;
	}
		
	currentIndex = otherIndex;

	break;
	}
	}
	}

	currentIndex = currentPoly.NextIndex(currentIndex);

	if (currentPoly == ctx.mPoly1)
	{
	if (currentIndex == 0)
	{
	break;
	}
	}
	else
	{
	if (firstPoly2Index >= 0 && currentIndex == firstPoly2Index)
	{
	break;
	}
	}
	} while ((currentPoly[currentIndex] != startingVertex) && (union.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));

	if (union.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
	{
	ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
	}

	return;
	}

	public static PolygonUtil.PolyUnionError PolygonIntersect(Point2DList polygon1, Point2DList polygon2, out Point2DList intersectOut)
	{
	PolygonOperationContext ctx = new PolygonOperationContext();
	ctx.Init(PolygonUtil.PolyOperation.Intersect, polygon1, polygon2);
	PolygonIntersectInternal(ctx);
	intersectOut = ctx.Intersect;
	return ctx.mError;
	}


	protected static void PolygonIntersectInternal(PolygonOperationContext ctx)
	{
	Point2DList intersectOut = ctx.Intersect;
	if (ctx.mStartingIndex == -1)
	{
	switch (ctx.mError)
	{
	case PolygonUtil.PolyUnionError.NoIntersections:
	case PolygonUtil.PolyUnionError.InfiniteLoop:
	return;
	case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
	intersectOut.AddRange(ctx.mOriginalPolygon2);
	return;
	}
	}

	Point2DList currentPoly = ctx.mPoly1;
	Point2DList otherPoly = ctx.mPoly2;
	List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

	int currentIndex = ctx.mPoly1.IndexOf(ctx.mIntersections[0].IntersectionPoint);
	Point2D startingVertex = ctx.mPoly1[currentIndex];
	int firstPoly1Index = currentIndex;
	int firstPoly2Index = -1;
	intersectOut.Clear();

	do
	{
	if (intersectOut.Contains(currentPoly[currentIndex]))
	{
	break;
	}
	intersectOut.Add(currentPoly[currentIndex]);

	foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
	{
	if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
	{
	int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

	int comparePointIndex = otherPoly.NextIndex(otherIndex);
	Point2D comparePoint = otherPoly[comparePointIndex];
	bool bPointInPolygonAngle = false;
	if (currentPolyVectorAngles[comparePointIndex] == -1)
	{
	bPointInPolygonAngle = ctx.PointInPolygonAngle(comparePoint, currentPoly);
	currentPolyVectorAngles[comparePointIndex] = bPointInPolygonAngle ? 1 : 0;
	}
	else
	{
	bPointInPolygonAngle = (currentPolyVectorAngles[comparePointIndex] == 1) ? true : false;
	}

	if (bPointInPolygonAngle)
	{
	if (currentPoly == ctx.mPoly1)
	{
	    currentPoly = ctx.mPoly2;
	    currentPolyVectorAngles = ctx.mPoly2VectorAngles;
	    otherPoly = ctx.mPoly1;
	    if (firstPoly2Index < 0)
	    {
	        firstPoly2Index = otherIndex;
	    }
	}
	else
	{
	    currentPoly = ctx.mPoly1;
	    currentPolyVectorAngles = ctx.mPoly1VectorAngles;
	    otherPoly = ctx.mPoly2;
	}
		
	currentIndex = otherIndex;

	break;
	}
	}
	}

	currentIndex = currentPoly.NextIndex(currentIndex);

	if (currentPoly == ctx.mPoly1)
	{
	if (currentIndex == firstPoly1Index)
	{
	break;
	}
	}
	else
	{
	if (firstPoly2Index >= 0 && currentIndex == firstPoly2Index)
	{
	break;
	}
	}
	} while ((currentPoly[currentIndex] != startingVertex) && (intersectOut.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));

	if (intersectOut.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
	{
	ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
	}

	return;
	}

	public static PolygonUtil.PolyUnionError PolygonSubtract(Point2DList polygon1, Point2DList polygon2, out Point2DList subtract)
	{
	PolygonOperationContext ctx = new PolygonOperationContext();
	ctx.Init(PolygonUtil.PolyOperation.Subtract, polygon1, polygon2);
	PolygonSubtractInternal(ctx);
	subtract = ctx.Subtract;
	return ctx.mError;
	}

	public static void PolygonSubtractInternal(PolygonOperationContext ctx)
	{
	Point2DList subtract = ctx.Subtract;
	if (ctx.mStartingIndex == -1)
	{
	switch (ctx.mError)
	{
	case PolygonUtil.PolyUnionError.NoIntersections:
	case PolygonUtil.PolyUnionError.InfiniteLoop:
	case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
	return;
	}
	}

	Point2DList currentPoly = ctx.mPoly1;
	Point2DList otherPoly = ctx.mPoly2;
	List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

	Point2D startingVertex = ctx.mPoly1[ctx.mStartingIndex];
	int currentIndex = ctx.mStartingIndex;
	subtract.Clear();

	bool forward = true;

	do
	{
	subtract.Add(currentPoly[currentIndex]);

	foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
	{
	if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
	{
	int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

	if (forward)
	{
	int compareIndex = otherPoly.PreviousIndex(otherIndex);
	Point2D compareVertex = otherPoly[compareIndex];
	bool bPointInPolygonAngle = false;
	if (currentPolyVectorAngles[compareIndex] == -1)
	{
	    bPointInPolygonAngle = ctx.PointInPolygonAngle(compareVertex, currentPoly);
	    currentPolyVectorAngles[compareIndex] = bPointInPolygonAngle ? 1 : 0;
	}
	else
	{
	    bPointInPolygonAngle = (currentPolyVectorAngles[compareIndex] == 1) ? true : false;
	}

	if (bPointInPolygonAngle)
	{
	    if (currentPoly == ctx.mPoly1)
	    {
	        currentPoly = ctx.mPoly2;
	        currentPolyVectorAngles = ctx.mPoly2VectorAngles;
	        otherPoly = ctx.mPoly1;
	    }
	    else
	    {
	        currentPoly = ctx.mPoly1;
	        currentPolyVectorAngles = ctx.mPoly1VectorAngles;
	        otherPoly = ctx.mPoly2;
	    }
			
	    currentIndex = otherIndex;

	    forward = !forward;

	    break;
	}
	}
	else
	{
	int compareIndex = otherPoly.NextIndex(otherIndex);
	Point2D compareVertex = otherPoly[compareIndex];
	bool bPointInPolygonAngle = false;
	if (currentPolyVectorAngles[compareIndex] == -1)
	{
	    bPointInPolygonAngle = ctx.PointInPolygonAngle(compareVertex, currentPoly);
	    currentPolyVectorAngles[compareIndex] = bPointInPolygonAngle ? 1 : 0;
	}
	else
	{
	    bPointInPolygonAngle = (currentPolyVectorAngles[compareIndex] == 1) ? true : false;
	}

	if (!bPointInPolygonAngle)
	{
	    if (currentPoly == ctx.mPoly1)
	    {
	        currentPoly = ctx.mPoly2;
	        currentPolyVectorAngles = ctx.mPoly2VectorAngles;
	        otherPoly = ctx.mPoly1;
	    }
	    else
	    {
	        currentPoly = ctx.mPoly1;
	        currentPolyVectorAngles = ctx.mPoly1VectorAngles;
	        otherPoly = ctx.mPoly2;
	    }
			
	    currentIndex = otherIndex;

	    forward = !forward;

	    break;
	}
	}
	}
	}

	if (forward)
	{
	currentIndex = currentPoly.NextIndex(currentIndex);
	}
	else
	{
	currentIndex = currentPoly.PreviousIndex(currentIndex);
	}
	} while ((currentPoly[currentIndex] != startingVertex) && (subtract.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));

	if (subtract.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
	{
	ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
	}

	return;
	}

	public static PolygonUtil.PolyUnionError PolygonOperation(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2, out Dictionary<uint, Point2DList> results)
	{
	PolygonOperationContext ctx = new PolygonOperationContext();
	ctx.Init(operations, polygon1, polygon2);
	results = ctx.mOutput;
	return PolygonUtil.PolygonOperation(ctx);
	}


	public static PolygonUtil.PolyUnionError PolygonOperation(PolygonOperationContext ctx)
	{
	if ((ctx.mOperations & PolygonUtil.PolyOperation.Union) == PolygonUtil.PolyOperation.Union)
	{
	PolygonUtil.PolygonUnionInternal(ctx);
	}
	if ((ctx.mOperations & PolygonUtil.PolyOperation.Intersect) == PolygonUtil.PolyOperation.Intersect)
	{
	PolygonUtil.PolygonIntersectInternal(ctx);
	}
	if ((ctx.mOperations & PolygonUtil.PolyOperation.Subtract) == PolygonUtil.PolyOperation.Subtract)
	{
	PolygonUtil.PolygonSubtractInternal(ctx);
	}

	return ctx.mError;
	}

	public static List<Point2DList> SplitComplexPolygon(Point2DList verts, double epsilon)
	{
	int numVerts = verts.Count;
	int nNodes = 0;
	List<SplitComplexPolygonNode> nodes = new List<SplitComplexPolygonNode>();

	for (int i = 0; i < verts.Count; ++i)
	{
	SplitComplexPolygonNode newNode = new SplitComplexPolygonNode(new Point2D(verts[i].X, verts[i].Y));
	nodes.Add(newNode);
	}
	for (int i = 0; i < verts.Count; ++i)
	{
	int iplus = (i == numVerts - 1) ? 0 : i + 1;
	int iminus = (i == 0) ? numVerts - 1 : i - 1;
	nodes[i].AddConnection(nodes[iplus]);
	nodes[i].AddConnection(nodes[iminus]);
	}
	nNodes = nodes.Count;

	bool dirty = true;
	int counter = 0;
	while (dirty)
	{
	dirty = false;
	for (int i = 0; !dirty && i < nNodes; ++i)
	{
	for (int j = 0; !dirty && j < nodes[i].NumConnected; ++j)
	{
	for (int k = 0; !dirty && k < nNodes; ++k)
	{
	if (k == i || nodes[k] == nodes[i][j])
	{
	    continue;
	}
	for (int l = 0; !dirty && l < nodes[k].NumConnected; ++l)
	{
	    if (nodes[k][l] == nodes[i][j] || nodes[k][l] == nodes[i])
	    {
	        continue;
	    }

	    Point2D intersectPt = new Point2D();
	    bool crosses = TriangulationUtil.LinesIntersect2D(  nodes[i].Position,
	                                                        nodes[i][j].Position,
	                                                        nodes[k].Position,
	                                                        nodes[k][l].Position,
	                                                        true, true, true,
	                                                        ref intersectPt,
	                                                        epsilon);
	    if (crosses)
	    {
	        dirty = true;
	        SplitComplexPolygonNode intersectionNode = new SplitComplexPolygonNode(intersectPt);
	        int idx = nodes.IndexOf(intersectionNode);
	        if (idx >= 0 && idx < nodes.Count)
	        {
	            intersectionNode = nodes[idx];
	        }
	        else
	        {
	            nodes.Add(intersectionNode);
	            nNodes = nodes.Count;
	        }

	        SplitComplexPolygonNode nodei = nodes[i];
	        SplitComplexPolygonNode connij = nodes[i][j];
	        SplitComplexPolygonNode nodek = nodes[k];
	        SplitComplexPolygonNode connkl = nodes[k][l];
	        connij.RemoveConnection(nodei);
	        nodei.RemoveConnection(connij);
	        connkl.RemoveConnection(nodek);
	        nodek.RemoveConnection(connkl);
	        if (!intersectionNode.Position.Equals(nodei.Position, epsilon))
	        {
	            intersectionNode.AddConnection(nodei);
	            nodei.AddConnection(intersectionNode);
	        }
	        if (!intersectionNode.Position.Equals(nodek.Position, epsilon))
	        {
	            intersectionNode.AddConnection(nodek);
	            nodek.AddConnection(intersectionNode);
	        }
	        if (!intersectionNode.Position.Equals(connij.Position, epsilon))
	        {
	            intersectionNode.AddConnection(connij);
	            connij.AddConnection(intersectionNode);
	        }
	        if (!intersectionNode.Position.Equals(connkl.Position, epsilon))
	        {
	            intersectionNode.AddConnection(connkl);
	            connkl.AddConnection(intersectionNode);
	        }
	    }
	}
	}
	}
	}
	++counter;
	}

	bool foundDupe = true;
	int nActive = nNodes;
	double epsilonSquared = epsilon * epsilon;
	while (foundDupe)
	{
	foundDupe = false;
	for (int i = 0; i < nNodes; ++i)
	{
	if (nodes[i].NumConnected == 0)
	{
	continue;
	}
	for (int j = i + 1; j < nNodes; ++j)
	{
	if (nodes[j].NumConnected == 0)
	{
	continue;
	}
	Point2D diff = nodes[i].Position - nodes[j].Position;
	if (diff.MagnitudeSquared() <= epsilonSquared)
	{
	if (nActive <= 3)
	{
	    throw new Exception("Eliminated so many duplicate points that resulting polygon has < 3 vertices!");
	}
		
	--nActive;
	foundDupe = true;
	SplitComplexPolygonNode inode = nodes[i];
	SplitComplexPolygonNode jnode = nodes[j];
	int njConn = jnode.NumConnected;
	for (int k = 0; k < njConn; ++k)
	{
	    SplitComplexPolygonNode knode = jnode[k];
	    if (knode != inode)
	    {
	        inode.AddConnection(knode);
	        knode.AddConnection(inode);
	    }
	    knode.RemoveConnection(jnode);

	}
	jnode.ClearConnections();
	nodes.RemoveAt(j);
	--nNodes;
	}
	}
	}
	}

	double minY = double.MaxValue;
	double maxX = -double.MaxValue;
	int minYIndex = -1;
	for (int i = 0; i < nNodes; ++i)
	{
	if (nodes[i].Position.Y < minY && nodes[i].NumConnected > 1)
	{
	minY = nodes[i].Position.Y;
	minYIndex = i;
	maxX = nodes[i].Position.X;
	}
	else if (nodes[i].Position.Y == minY && nodes[i].Position.X > maxX && nodes[i].NumConnected > 1)
	{
	minYIndex = i;
	maxX = nodes[i].Position.X;
	}
	}

	Point2D origDir = new Point2D(1.0f, 0.0f);
	List<Point2D> resultVecs = new List<Point2D>();
	SplitComplexPolygonNode currentNode = nodes[minYIndex];
	SplitComplexPolygonNode startNode = currentNode;
	SplitComplexPolygonNode nextNode = currentNode.GetRightestConnection(origDir);
	if (nextNode == null)
	{
	return PolygonUtil.SplitComplexPolygonCleanup(verts);
	}

	resultVecs.Add(startNode.Position);
	while (nextNode != startNode)
	{
	if (resultVecs.Count > (4 * nNodes))
	{
	throw new Exception("nodes should never be visited four times apiece (proof?), so we've probably hit a loop...crap");
	}
	resultVecs.Add(nextNode.Position);
	SplitComplexPolygonNode oldNode = currentNode;
	currentNode = nextNode;
	nextNode = currentNode.GetRightestConnection(oldNode);
	if (nextNode == null)
	{
	return PolygonUtil.SplitComplexPolygonCleanup(resultVecs);
	}
	}

	if (resultVecs.Count < 1)
	{
	return PolygonUtil.SplitComplexPolygonCleanup(verts);
	}
	else
	{
	return PolygonUtil.SplitComplexPolygonCleanup(resultVecs);
	}
	}


	private static List<Point2DList> SplitComplexPolygonCleanup(IList<Point2D> orig)
	{
	List<Point2DList> l = new List<Point2DList>();
	Point2DList origP2DL = new Point2DList(orig);
	l.Add(origP2DL);
	int listIdx = 0;
	int numLists = l.Count;
	while (listIdx < numLists)
	{
	int numPoints = l[listIdx].Count;
	for (int i = 0; i < numPoints; ++i)
	{
	for (int j = i + 1; j < numPoints; ++j)
	{
	if (l[listIdx][i].Equals(l[listIdx][j], origP2DL.Epsilon))
	{
	int numToRemove = j - i;
	Point2DList newList = new Point2DList();
	for (int k = i + 1; k <= j; ++k)
	{
	    newList.Add(l[listIdx][k]);
	}
	l[listIdx].RemoveRange(i + 1, numToRemove);
	l.Add(newList);
	++numLists;
	numPoints -= numToRemove;
	j = i + 1;
	}
	}
	}
	l[listIdx].Simplify();
	++listIdx;
	}

	return l;
	}

	}


	public class EdgeIntersectInfo
	{
	public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Point2D intersectionPoint)
	{
	EdgeOne = edgeOne;
	EdgeTwo = edgeTwo;
	IntersectionPoint = intersectionPoint;
	}

	public Edge EdgeOne { get; private set; }
	public Edge EdgeTwo { get; private set; }
	public Point2D IntersectionPoint { get; private set; }
	}


	public class SplitComplexPolygonNode
	{
	private List<SplitComplexPolygonNode> mConnected = new List<SplitComplexPolygonNode>();
	private Point2D mPosition = null;

	public int NumConnected { get { return mConnected.Count; } }
	public Point2D Position { get { return mPosition; } set { mPosition = value; } }
	public SplitComplexPolygonNode this[int index]
	{
	get { return mConnected[index]; }
	}


	public SplitComplexPolygonNode()
	{
	}

	public SplitComplexPolygonNode(Point2D pos)
	{
	mPosition = pos;
	}


	public override bool Equals(Object obj)
	{
	SplitComplexPolygonNode pn = obj as SplitComplexPolygonNode;
	if (pn == null)
	{
	return base.Equals(obj);
	}

	return Equals(pn);
	}


	public bool Equals(SplitComplexPolygonNode pn)
	{
	if ((Object)pn == null)
	{
	return false;
	}
	if (mPosition == null || pn.Position == null)
	{
	return false;
	}

	return mPosition.Equals(pn.Position);
	}


	public override int GetHashCode()
	{
	return base.GetHashCode();
	}

	public static bool operator ==(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs) { if ((object)lhs != null) { return lhs.Equals(rhs); } if ((Object)rhs == null) { return true; } else { return false; } }
	public static bool operator !=(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs) { if ((object)lhs != null) { return !lhs.Equals(rhs); } if ((Object)rhs == null) { return false; } else { return true; } }

	public override string ToString()
	{
	StringBuilder sb = new StringBuilder(256);
	sb.Append(mPosition.ToString());
	sb.Append(" -> ");
	for (int i = 0; i < NumConnected; ++i)
	{
	if (i != 0)
	{
	sb.Append(", ");
	}
	sb.Append(mConnected[i].Position.ToString());
	}

	return sb.ToString();
	}


	private bool IsRighter(double sinA, double cosA, double sinB, double cosB)
	{
	if (sinA < 0)
	{
	if (sinB > 0 || cosA <= cosB)
	{
	return true;
	}
	else
	{
	return false;
	}
	}
	else
	{
	if (sinB < 0 || cosA <= cosB)
	{
	return false;
	}
	else
	{
	return true;
	}
	}
	}

	private int remainder(int x, int modulus)
	{
	int rem = x % modulus;
	while (rem < 0)
	{
	rem += modulus;
	}
	return rem;
	}

	public void AddConnection(SplitComplexPolygonNode toMe)
	{
	if (!mConnected.Contains(toMe) && toMe != this)
	{
	mConnected.Add(toMe);
	}
	}

	public void RemoveConnection(SplitComplexPolygonNode fromMe)
	{
	mConnected.Remove(fromMe);
	}

	private void RemoveConnectionByIndex(int index)
	{
	if (index < 0 || index >= mConnected.Count)
	{
	return;
	}
	mConnected.RemoveAt(index);
	}

	public void ClearConnections()
	{
	mConnected.Clear();
	}

	private bool IsConnectedTo(SplitComplexPolygonNode me)
	{
	return mConnected.Contains(me);
	}

	public SplitComplexPolygonNode GetRightestConnection(SplitComplexPolygonNode incoming)
	{
	if (NumConnected == 0)
	{
	throw new Exception("the connection graph is inconsistent");
	}
	if (NumConnected == 1)
	{
	return incoming;
	}
	Point2D inDir = mPosition - incoming.mPosition;

	double inLength = inDir.Magnitude();
	inDir.Normalize();

	if (inLength <= MathUtil.EPSILON)
	{
	throw new Exception("Length too small");
	}

	SplitComplexPolygonNode result = null;
	for (int i = 0; i < NumConnected; ++i)
	{
	if (mConnected[i] == incoming)
	{
	continue;
	}
	Point2D testDir = mConnected[i].mPosition - mPosition;
	double testLengthSqr = testDir.MagnitudeSquared();
	testDir.Normalize();
	if (testLengthSqr <= (MathUtil.EPSILON * MathUtil.EPSILON))
	{
	throw new Exception("Length too small");
	}

	double myCos = Point2D.Dot(inDir, testDir);
	double mySin = Point2D.Cross(inDir, testDir);
	if (result != null)
	{
	Point2D resultDir = result.mPosition - mPosition;
	resultDir.Normalize();
	double resCos = Point2D.Dot(inDir, resultDir);
	double resSin = Point2D.Cross(inDir, resultDir);
	if (IsRighter(mySin, myCos, resSin, resCos))
	{
	result = mConnected[i];
	}
	}
	else
	{
	result = mConnected[i];
	}
	}

	return result;
	}

	public SplitComplexPolygonNode GetRightestConnection(Point2D incomingDir)
	{
	Point2D diff = mPosition - incomingDir;
	SplitComplexPolygonNode temp = new SplitComplexPolygonNode(diff);
	SplitComplexPolygonNode res = GetRightestConnection(temp);
	return res;
	}
	}


	public class PolygonOperationContext
	{
	public PolygonUtil.PolyOperation mOperations;
	public Point2DList mOriginalPolygon1;
	public Point2DList mOriginalPolygon2;
	public Point2DList mPoly1;
	public Point2DList mPoly2;
	public List<EdgeIntersectInfo> mIntersections;
	public int mStartingIndex;
	public PolygonUtil.PolyUnionError mError;
	public List<int> mPoly1VectorAngles;
	public List<int> mPoly2VectorAngles;
	public Dictionary<uint, Point2DList> mOutput = new Dictionary<uint, Point2DList>();

	public Point2DList Union
	{
	get
	{
	Point2DList l = null;
	if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Union, out l))
	{
	l = new Point2DList();
	mOutput.Add((uint)PolygonUtil.PolyOperation.Union, l);
	}

	return l;
	}
	}
	public Point2DList Intersect
	{
	get
	{
	Point2DList l = null;
	if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Intersect, out l))
	{
	l = new Point2DList();
	mOutput.Add((uint)PolygonUtil.PolyOperation.Intersect, l);
	}

	return l;
	}
	}
	public Point2DList Subtract
	{
	get
	{
	Point2DList l = null;
	if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Subtract, out l))
	{
	l = new Point2DList();
	mOutput.Add((uint)PolygonUtil.PolyOperation.Subtract, l);
	}

	return l;
	}
	}

	public PolygonOperationContext() { }


	public void Clear()
	{
	mOperations = PolygonUtil.PolyOperation.None;
	mOriginalPolygon1 = null;
	mOriginalPolygon2 = null;
	mPoly1 = null;
	mPoly2 = null;
	mIntersections = null;
	mStartingIndex = -1;
	mError = PolygonUtil.PolyUnionError.None;
	mPoly1VectorAngles = null;
	mPoly2VectorAngles = null;
	mOutput = new Dictionary<uint, Point2DList>();
	}


	public bool Init(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2)
	{
	Clear();

	mOperations = operations;
	mOriginalPolygon1 = polygon1;
	mOriginalPolygon2 = polygon2;

	mPoly1 = new Point2DList(polygon1);
	mPoly1.WindingOrder = Point2DList.WindingOrderType.Default;
	mPoly2 = new Point2DList(polygon2);
	mPoly2.WindingOrder = Point2DList.WindingOrderType.Default;

	if (!VerticesIntersect(mPoly1, mPoly2, out mIntersections))
	{
	mError = PolygonUtil.PolyUnionError.NoIntersections;
	return false;
	}

	int numIntersections = mIntersections.Count;
	for (int i = 0; i < numIntersections; ++i)
	{
	for (int j = i + 1; j < numIntersections; ++j)
	{
	if (mIntersections[i].EdgeOne.EdgeStart.Equals(mIntersections[j].EdgeOne.EdgeStart) &&
	mIntersections[i].EdgeOne.EdgeEnd.Equals(mIntersections[j].EdgeOne.EdgeEnd))
	{
	mIntersections[j].EdgeOne.EdgeStart = mIntersections[i].IntersectionPoint;
	}
	if (mIntersections[i].EdgeTwo.EdgeStart.Equals(mIntersections[j].EdgeTwo.EdgeStart) &&
	mIntersections[i].EdgeTwo.EdgeEnd.Equals(mIntersections[j].EdgeTwo.EdgeEnd))
	{
	mIntersections[j].EdgeTwo.EdgeStart = mIntersections[i].IntersectionPoint;
	}
	}
	}

	foreach (EdgeIntersectInfo intersect in mIntersections)
	{
	if (!mPoly1.Contains(intersect.IntersectionPoint))
	{
	mPoly1.Insert(mPoly1.IndexOf(intersect.EdgeOne.EdgeStart) + 1, intersect.IntersectionPoint);
	}

	if (!mPoly2.Contains(intersect.IntersectionPoint))
	{
	mPoly2.Insert(mPoly2.IndexOf(intersect.EdgeTwo.EdgeStart) + 1, intersect.IntersectionPoint);
	}
	}

	mPoly1VectorAngles = new List<int>();
	for (int i = 0; i < mPoly2.Count; ++i)
	{
	mPoly1VectorAngles.Add(-1);
	}
	mPoly2VectorAngles = new List<int>();
	for (int i = 0; i < mPoly1.Count; ++i)
	{
	mPoly2VectorAngles.Add(-1);
	}

	int currentIndex = 0;
	do
	{
	bool bPointInPolygonAngle = PointInPolygonAngle(mPoly1[currentIndex], mPoly2);
	mPoly2VectorAngles[currentIndex] = bPointInPolygonAngle ? 1 : 0;
	if (bPointInPolygonAngle)
	{
	mStartingIndex = currentIndex;
	break;
	}
	currentIndex = mPoly1.NextIndex(currentIndex);
	} while (currentIndex != 0);

	if (mStartingIndex == -1)
	{
	mError = PolygonUtil.PolyUnionError.Poly1InsidePoly2;
	return false;
	}

	return true;
	}


	private bool VerticesIntersect(Point2DList polygon1, Point2DList polygon2, out List<EdgeIntersectInfo> intersections)
	{
	intersections = new List<EdgeIntersectInfo>();
	double epsilon = Math.Min(polygon1.Epsilon, polygon2.Epsilon);


	for (int i = 0; i < polygon1.Count; i++)
	{
	Point2D p1 = polygon1[i];
	Point2D p2 = polygon1[polygon1.NextIndex(i)];

	for (int j = 0; j < polygon2.Count; j++)
	{
	Point2D point = new Point2D();

	Point2D p3 = polygon2[j];
	Point2D p4 = polygon2[polygon2.NextIndex(j)];

	if (TriangulationUtil.LinesIntersect2D(p1, p2, p3, p4, ref point, epsilon))
	{
	intersections.Add(new EdgeIntersectInfo(new Edge(p1, p2), new Edge(p3, p4), point));
	}
	}
	}

	return (intersections.Count > 0);
	}

	public bool PointInPolygonAngle(Point2D point, Point2DList polygon)
	{
	double angle = 0;
	for (int i = 0; i < polygon.Count; i++)
	{
	Point2D p1 = polygon[i] - point;
	Point2D p2 = polygon[polygon.NextIndex(i)] - point;

	angle += VectorAngle(p1, p2);
	}

	if (Math.Abs(angle) < Math.PI)
	{
	return false;
	}

	return true;
	}

	public double VectorAngle(Point2D p1, Point2D p2)
	{
	double theta1 = Math.Atan2(p1.Y, p1.X);
	double theta2 = Math.Atan2(p2.Y, p2.X);
	double dtheta = theta2 - theta1;
	while (dtheta > Math.PI)
	{
	dtheta -= (2.0 * Math.PI);
	}
	while (dtheta < -Math.PI)
	{
	dtheta += (2.0 * Math.PI);
	}

	return (dtheta);
	}

	}

	public class Contour : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
	{
	private List<Contour> mHoles = new List<Contour>();
	private ITriangulatable mParent = null;
	private string mName = "";
	public new TriangulationPoint this[int index]
	{
	get { return mPoints[index] as TriangulationPoint; }
	set { mPoints[index] = value; }
	}
	public string Name { get { return mName; } set { mName = value; } }
	public IList<DelaunayTriangle> Triangles
	{
	get { throw new NotImplementedException("PolyHole.Triangles should never get called");}
	private set { }
	}
	public TriangulationMode TriangulationMode { get { return mParent.TriangulationMode; } }
	public string FileName { get { return mParent.FileName; } set { } }
	public bool DisplayFlipX { get { return mParent.DisplayFlipX; } set { } }
	public bool DisplayFlipY { get { return mParent.DisplayFlipY; } set { } }
	public float DisplayRotate { get { return mParent.DisplayRotate; } set { } }
	public double Precision { get { return mParent.Precision; } set { } }
	public double MinX { get { return mBoundingBox.MinX; } }
	public double MaxX { get { return mBoundingBox.MaxX; } }
	public double MinY { get { return mBoundingBox.MinY; } }
	public double MaxY { get { return mBoundingBox.MaxY; } }
	public Rect2D Bounds { get { return mBoundingBox; } }
	public Contour(ITriangulatable parent)
	{
	mParent = parent;
	}
	public Contour(ITriangulatable parent, IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
	{
	mParent = parent;
	AddRange(points, windingOrder);
	}

	public override string ToString()
	{
	return mName + " : " + base.ToString();
	}

	IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
	{
	return new TriangulationPointEnumerator(mPoints);
	}

	public int IndexOf(TriangulationPoint p)
	{
	return mPoints.IndexOf(p);
	}

	public void Add(TriangulationPoint p)
	{
	Add(p, -1, true);
	}

	protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
	{
	TriangulationPoint pt = null;
	if (p is TriangulationPoint)
	pt = p as TriangulationPoint;
	else
	pt = new TriangulationPoint(p.X, p.Y);
	if (idx < 0)
	mPoints.Add(pt);
	else
	mPoints.Insert(idx, pt);
	mBoundingBox.AddPoint(pt);
	if (bCalcWindingOrderAndEpsilon)
	{
	if (mWindingOrder == WindingOrderType.Unknown)
	mWindingOrder = CalculateWindingOrder();
	mEpsilon = CalculateEpsilon();
	}
	}

	public override void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
	{
	if (iter == null)
	return;

	if (mWindingOrder == WindingOrderType.Unknown && Count == 0)
	mWindingOrder = windingOrder;

	bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
	bool bAddedFirst = true;
	int startCount = mPoints.Count;
	iter.Reset();
	while (iter.MoveNext())
	{
	TriangulationPoint pt = null;
	if (iter.Current is TriangulationPoint)
	pt = iter.Current as TriangulationPoint;
	else
	pt = new TriangulationPoint(iter.Current.X, iter.Current.Y);
	if (!bAddedFirst)
	{
	bAddedFirst = true;
	mPoints.Add(pt);
	} else if (bReverseReadOrder) {
	mPoints.Insert(startCount, pt);
	} else {
	mPoints.Add(pt);
	}
	mBoundingBox.AddPoint(iter.Current);
	}
	if (mWindingOrder == WindingOrderType.Unknown && windingOrder == WindingOrderType.Unknown)
	mWindingOrder = CalculateWindingOrder();

	mEpsilon = CalculateEpsilon();
	}

	public void AddRange(IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
	{
	if (points == null || points.Count < 1)
	return;

	if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
	mWindingOrder = windingOrder;

	int numPoints = points.Count;
	bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
	for (int i = 0; i < numPoints; ++i)
	{
	int idx = i;
	if (bReverseReadOrder)
	idx = points.Count - i - 1;
	Add(points[idx], -1, false);
	}
	if (mWindingOrder == WindingOrderType.Unknown)
	mWindingOrder = CalculateWindingOrder();
	mEpsilon = CalculateEpsilon();
	}

	public void Insert(int idx, TriangulationPoint p)
	{
	Add(p, idx, true);
	}

	public bool Remove(TriangulationPoint p)
	{
	return Remove(p as Point2D);
	}

	public bool Contains(TriangulationPoint p)
	{
	return mPoints.Contains(p);
	}

	public void CopyTo(TriangulationPoint[] array, int arrayIndex)
	{
	int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
	for (int i = 0; i < numElementsToCopy; ++i)
	array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
	}

	protected void AddHole(Contour c)
	{
	c.mParent = this;
	mHoles.Add(c);
	}

	public int GetNumHoles(bool parentIsHole)
	{
	int numHoles = parentIsHole ? 0 : 1;
	foreach (Contour c in mHoles)
	numHoles += c.GetNumHoles(!parentIsHole);
	return numHoles;
	}

	public int GetNumHoles()
	{
	return mHoles.Count;
	}

	public Contour GetHole(int idx)
	{
	if (idx < 0 || idx >= mHoles.Count)
	return null;
	return mHoles[idx];
	}


	public void GetActualHoles(bool parentIsHole, ref List<Contour> holes)
	{
	if (parentIsHole)
	holes.Add(this);

	foreach (Contour c in mHoles)
	c.GetActualHoles(!parentIsHole, ref holes);
	}

	public List<Contour>.Enumerator GetHoleEnumerator()
	{
	return mHoles.GetEnumerator();
	}

	public void InitializeHoles(ConstrainedPointSet cps)
	{
	Contour.InitializeHoles(mHoles, this, cps);
	foreach (Contour c in mHoles)
	c.InitializeHoles(cps);
	}

	public static void InitializeHoles(List<Contour> holes, ITriangulatable parent, ConstrainedPointSet cps)
	{
	int numHoles = holes.Count;
	int holeIdx = 0;

	while (holeIdx < numHoles)
	{
	int hole2Idx = holeIdx + 1;
	while (hole2Idx < numHoles)
	{
	bool bSamePolygon = PolygonUtil.PolygonsAreSame2D(holes[holeIdx], holes[hole2Idx]);
	if (bSamePolygon)
	{
	holes.RemoveAt(hole2Idx);
	--numHoles;
	}
	else
	{
	++hole2Idx;
	}
	}
	++holeIdx;
	}

	holeIdx = 0;
	while (holeIdx < numHoles)
	{
	bool bIncrementHoleIdx = true;
	int hole2Idx = holeIdx + 1;
	while (hole2Idx < numHoles)
	{
	if (PolygonUtil.PolygonContainsPolygon(holes[holeIdx], holes[holeIdx].Bounds, holes[hole2Idx], holes[hole2Idx].Bounds, false))
	{
	holes[holeIdx].AddHole(holes[hole2Idx]);
	holes.RemoveAt(hole2Idx);
	--numHoles;
	}
	else if (PolygonUtil.PolygonContainsPolygon(holes[hole2Idx], holes[hole2Idx].Bounds, holes[holeIdx], holes[holeIdx].Bounds, false))
	{
	holes[hole2Idx].AddHole(holes[holeIdx]);
	holes.RemoveAt(holeIdx);
	--numHoles;
	bIncrementHoleIdx = false;
	break;
	} else {
	bool bIntersect = PolygonUtil.PolygonsIntersect2D(holes[holeIdx], holes[holeIdx].Bounds, holes[hole2Idx], holes[hole2Idx].Bounds);
	if (bIntersect) {
	PolygonOperationContext ctx = new PolygonOperationContext();
	if (!ctx.Init(PolygonUtil.PolyOperation.Union | PolygonUtil.PolyOperation.Intersect, holes[holeIdx], holes[hole2Idx]))
	{
		if (ctx.mError == PolygonUtil.PolyUnionError.Poly1InsidePoly2)
		{
			holes[hole2Idx].AddHole(holes[holeIdx]);
			holes.RemoveAt(holeIdx);
			--numHoles;
			bIncrementHoleIdx = false;
			break;
		} else
			throw new Exception("PolygonOperationContext.Init had an error during initialization");
	}
	PolygonUtil.PolyUnionError pue = PolygonUtil.PolygonOperation(ctx);
	if (pue == PolygonUtil.PolyUnionError.None)
	{
		Point2DList union = ctx.Union;
		Point2DList intersection = ctx.Intersect;

		Contour c = new Contour(parent);
		c.AddRange(union);
		c.Name = "(" + holes[holeIdx].Name + " UNION " + holes[hole2Idx].Name + ")";
		c.WindingOrder = Point2DList.WindingOrderType.Default;

		int numChildHoles = holes[holeIdx].GetNumHoles();
		for(int i = 0; i < numChildHoles; ++i)
			c.AddHole(holes[holeIdx].GetHole(i));
		numChildHoles = holes[hole2Idx].GetNumHoles();
		for (int i = 0; i < numChildHoles; ++i)
			c.AddHole(holes[hole2Idx].GetHole(i));

		Contour cInt = new Contour(c);
		cInt.AddRange(intersection);
		cInt.Name = "(" + holes[holeIdx].Name + " INTERSECT " + holes[hole2Idx].Name + ")";
		cInt.WindingOrder = Point2DList.WindingOrderType.Default;
		c.AddHole(cInt);

		holes[holeIdx] = c;

		holes.RemoveAt(hole2Idx);
		--numHoles;

		hole2Idx = holeIdx + 1;
	}
	else
		throw new Exception("PolygonOperation had an error!");
	}
	else
	++hole2Idx;
	}
	}
	if (bIncrementHoleIdx)
	++holeIdx;
	}

	numHoles = holes.Count;
	holeIdx = 0;
	while (holeIdx < numHoles)
	{
	int numPoints = holes[holeIdx].Count;
	for (int i = 0; i < numPoints; ++i)
	{
	int j = holes[holeIdx].NextIndex(i);
	uint constraintCode = TriangulationConstraint.CalculateContraintCode(holes[holeIdx][i], holes[holeIdx][j]);
	TriangulationConstraint tc = null;
	if (!cps.TryGetConstraint(constraintCode, out tc))
	{
	tc = new TriangulationConstraint(holes[holeIdx][i], holes[holeIdx][j]);
	cps.AddConstraint(tc);
	}

	if (holes[holeIdx][i].VertexCode == tc.P.VertexCode)
	holes[holeIdx][i] = tc.P;
	else if (holes[holeIdx][j].VertexCode == tc.P.VertexCode)
	holes[holeIdx][j] = tc.P;

	if (holes[holeIdx][i].VertexCode == tc.Q.VertexCode)
	holes[holeIdx][i] = tc.Q;
	else if (holes[holeIdx][j].VertexCode == tc.Q.VertexCode)
	holes[holeIdx][j] = tc.Q;
	}
	++holeIdx;
	}
	}

	public void Prepare(TriangulationContext tcx)
	{
	throw new NotImplementedException("PolyHole.Prepare should never get called");
	}

	public void AddTriangle(DelaunayTriangle t)
	{
	throw new NotImplementedException("PolyHole.AddTriangle should never get called");
	}

	public void AddTriangles(IEnumerable<DelaunayTriangle> list)
	{
	throw new NotImplementedException("PolyHole.AddTriangles should never get called");
	}

	public void ClearTriangles()
	{
	throw new NotImplementedException("PolyHole.ClearTriangles should never get called");
	}

	public Point2D FindPointInContour()
	{
	if (Count < 3)
	return null;

	Point2D p = GetCentroid();
	if (IsPointInsideContour(p))
	return p;

	Random random = new Random();
	while (true)
	{
	p.X = (random.NextDouble() * (MaxX - MinX)) + MinX;
	p.Y = (random.NextDouble() * (MaxY - MinY)) + MinY;
	if (IsPointInsideContour(p))
	return p;
	}
	}

	public bool IsPointInsideContour(Point2D p)
	{
	if (PolygonUtil.PointInPolygon2D(this, p))
	{
	foreach (Contour c in mHoles)
	if (c.IsPointInsideContour(p))
	return false;

	return true;
	}
	return false;
	}

	}

	public class Polygon : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
	{
	protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();
	public IList<TriangulationPoint> Points { get { return this; } }
	protected List<DelaunayTriangle> mTriangles;
	public IList<DelaunayTriangle> Triangles { get { return mTriangles; } }
	public TriangulationMode TriangulationMode { get { return TriangulationMode.Polygon; } }
	public string FileName { get; set; }
	public bool DisplayFlipX { get; set; }
	public bool DisplayFlipY { get; set; }
	public float DisplayRotate { get; set; }
	private double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;
	public double Precision { get { return mPrecision; } set { mPrecision = value; } }
	public double MinX { get { return mBoundingBox.MinX; } }
	public double MaxX { get { return mBoundingBox.MaxX; } }
	public double MinY { get { return mBoundingBox.MinY; } }
	public double MaxY { get { return mBoundingBox.MaxY; } }
	public Rect2D Bounds { get { return mBoundingBox; } }

	public new TriangulationPoint this[int index]
	{
	get { return mPoints[index] as TriangulationPoint; }
	set { mPoints[index] = value; }
	}

	protected List<Polygon> mHoles;
	public IList<Polygon> Holes { get { return mHoles; } }
	protected List<TriangulationPoint> mSteinerPoints;
	protected PolygonPoint _last;

	public Polygon(IList<PolygonPoint> points)
	{
		if (points.Count < 3) {
			throw new ArgumentException("List has fewer than 3 points (" + points.Count +")", "points");
		}
		AddRange(points, WindingOrderType.Unknown);
	}

	public Polygon(IEnumerable<PolygonPoint> points) : this((points as IList<PolygonPoint>) ?? points.ToArray()) {}
	public Polygon(params PolygonPoint[] points) : this((IList<PolygonPoint>)points) {}

	IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
	{
	return new TriangulationPointEnumerator(mPoints);
	}

	public int IndexOf(TriangulationPoint p)
	{
	return mPoints.IndexOf(p);
	}

	public override void Add(Point2D p)
	{
	Add(p, -1, true);
	}

	public void Add(TriangulationPoint p)
	{
	Add(p, -1, true);
	}

	public void Add(PolygonPoint p)
	{
	Add(p, -1, true);
	}

	protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
	{
	TriangulationPoint pt = p as TriangulationPoint;
	if (pt == null)
	return;

	if (mPointMap.ContainsKey(pt.VertexCode))
	return;
	mPointMap.Add(pt.VertexCode, pt);

	base.Add(p, idx, bCalcWindingOrderAndEpsilon);

	PolygonPoint pp = p as PolygonPoint;
	if (pp != null)
	{
	pp.Previous = _last;
	if (_last != null)
	{
	pp.Next = _last.Next;
	_last.Next = pp;
	}
	_last = pp;
	}

	return;
	}


	public void AddRange(IList<PolygonPoint> points, Point2DList.WindingOrderType windingOrder)
	{
	if (points == null || points.Count < 1)
	return;

	if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
	mWindingOrder = windingOrder;

	int numPoints = points.Count;
	bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
	for (int i = 0; i < numPoints; ++i)
	{
	int idx = i;
	if (bReverseReadOrder)
	idx = points.Count - i - 1;
	Add(points[idx], -1, false);
	}
	if (mWindingOrder == WindingOrderType.Unknown)
	mWindingOrder = CalculateWindingOrder();

	mEpsilon = CalculateEpsilon();
	}


	public void AddRange(IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
	{
	if (points == null || points.Count < 1)
	return;

	if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
	mWindingOrder = windingOrder;

	int numPoints = points.Count;
	bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
	for (int i = 0; i < numPoints; ++i)
	{
	int idx = i;
	if (bReverseReadOrder)
	idx = points.Count - i - 1;
	Add(points[idx], -1, false);
	}
	if (mWindingOrder == WindingOrderType.Unknown)
	mWindingOrder = CalculateWindingOrder();
	mEpsilon = CalculateEpsilon();
	}

	public void Insert(int idx, TriangulationPoint p)
	{
	Add(p, idx, true);
	}

	public bool Remove(TriangulationPoint p)
	{
	return base.Remove(p);
	}

	public void RemovePoint(PolygonPoint p)
	{
	PolygonPoint next, prev;

	next = p.Next;
	prev = p.Previous;
	prev.Next = next;
	next.Previous = prev;
	mPoints.Remove(p);

	mBoundingBox.Clear();
	foreach (PolygonPoint tmp in mPoints)
	mBoundingBox.AddPoint(tmp);
	}

	public bool Contains(TriangulationPoint p)
	{
	return mPoints.Contains(p);
	}

	public void CopyTo(TriangulationPoint[] array, int arrayIndex)
	{
	int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
	for (int i = 0; i < numElementsToCopy; ++i)
	array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
	}

	public void AddSteinerPoint(TriangulationPoint point)
	{
	if (mSteinerPoints == null)
	mSteinerPoints = new List<TriangulationPoint>();
	mSteinerPoints.Add(point);
	}

	public void AddSteinerPoints(List<TriangulationPoint> points)
	{
	if (mSteinerPoints == null)
	mSteinerPoints = new List<TriangulationPoint>();
	mSteinerPoints.AddRange(points);
	}

	public void ClearSteinerPoints()
	{
	if (mSteinerPoints != null)
	mSteinerPoints.Clear();
	}

	public void AddHole(Polygon poly)
	{
	if (mHoles == null)
	mHoles = new List<Polygon>();
	mHoles.Add(poly);
	}

	public void AddTriangle(DelaunayTriangle t)
	{
	mTriangles.Add(t);
	}

	public void AddTriangles(IEnumerable<DelaunayTriangle> list)
	{
	mTriangles.AddRange(list);
	}

	public void ClearTriangles()
	{
	if (mTriangles != null)
	mTriangles.Clear();
	}

	public bool IsPointInside(TriangulationPoint p)
	{
	return PolygonUtil.PointInPolygon2D(this, p);
	}

	public void Prepare(TriangulationContext tcx)
	{
	if (mTriangles == null)
	mTriangles = new List<DelaunayTriangle>(mPoints.Count);
	else
	mTriangles.Clear();

	for (int i = 0; i < mPoints.Count - 1; i++)
	tcx.NewConstraint(this[i], this[i + 1]);
	tcx.NewConstraint(this[0], this[Count - 1]);
	tcx.Points.AddRange(this);

	if (mHoles != null)
	{
	foreach (Polygon p in mHoles)
	{
	for (int i = 0; i < p.mPoints.Count - 1; i++)
	tcx.NewConstraint(p[i], p[i + 1]);
	tcx.NewConstraint(p[0], p[p.Count - 1]);
	tcx.Points.AddRange(p);
	}
	}

	if (mSteinerPoints != null)
	tcx.Points.AddRange(mSteinerPoints);
	}
	}

	public class PolygonPoint : TriangulationPoint
	{
	public PolygonPoint(double x, double y) : base(x, y) { }

	public PolygonPoint Next { get; set; }
	public PolygonPoint Previous { get; set; }

	public static Point2D ToBasePoint(PolygonPoint p)
	{
	return (Point2D)p;
	}

	public static TriangulationPoint ToTriangulationPoint(PolygonPoint p)
	{
	return (TriangulationPoint)p;
	}
	}

	public class PolygonSet
	{
	protected List<Polygon> _polygons = new List<Polygon>();

	public PolygonSet() { }

	public PolygonSet(Polygon poly)
	{
	_polygons.Add(poly);
	}

	public void Add(Polygon p)
	{
	_polygons.Add(p);
	}

	public IEnumerable<Polygon> Polygons { get { return _polygons; } }
	}
}
