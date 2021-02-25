using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Polygon2DTriangulation
{
	public class DelaunayTriangle
	{

		public FixedArray3<TriangulationPoint> Points;
		public FixedArray3<DelaunayTriangle> Neighbors;
		private FixedBitArray3 mEdgeIsConstrained = new FixedBitArray3();
		public FixedBitArray3 EdgeIsConstrained { get { return mEdgeIsConstrained; } }
		public FixedBitArray3 EdgeIsDelaunay;
		public bool IsInterior { get; set; }

		public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
		{
			Points[0] = p1;
			Points[1] = p2;
			Points[2] = p3;
		}

		public int IndexOf(TriangulationPoint p)
		{
			int i = Points.IndexOf(p);
			if (i == -1)
				throw new Exception("Calling index with a point that doesn't exist in triangle");

			return i;
		}
			
		public int IndexCWFrom(TriangulationPoint p)
		{
			return (IndexOf(p) + 2) % 3;
		}

		public int IndexCCWFrom(TriangulationPoint p)
		{
			return (IndexOf(p) + 1) % 3;
		}

		public bool Contains(TriangulationPoint p)
		{
			return Points.Contains(p);
		}
			
		private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
		{
			int i = EdgeIndex(p1, p2);
			if (i == -1)
				throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
			Neighbors[i] = t;
		}
			
		public void MarkNeighbor(DelaunayTriangle t)
		{
			if (t == null) {
				return;
			}
			bool a = t.Contains(Points[0]);
			bool b = t.Contains(Points[1]);
			bool c = t.Contains(Points[2]);

			if (b && c)
			{
				Neighbors[0] = t;
				t.MarkNeighbor(Points[1], Points[2], this);
			}
			else if (a && c)
			{
				Neighbors[1] = t;
				t.MarkNeighbor(Points[0], Points[2], this);
			}
			else if (a && b)
			{
				Neighbors[2] = t;
				t.MarkNeighbor(Points[0], Points[1], this);
			}
			else
				throw new Exception("Failed to mark neighbor, doesn't share an edge!");
		}

		public void ClearNeighbors()
		{
			Neighbors[0] = Neighbors[1] = Neighbors[2] = null;
		}
			
		public void ClearNeighbor(DelaunayTriangle triangle)
		{
			if (Neighbors[0] == triangle)
				Neighbors[0] = null;
			else if (Neighbors[1] == triangle)
				Neighbors[1] = null;
			else if( Neighbors[2] == triangle)
				Neighbors[2] = null;
		}
			
		public void Clear()
		{
			DelaunayTriangle t;
			for (int i = 0; i < 3; i++)
			{
				t = Neighbors[i];
				if (t != null)
					t.ClearNeighbor(this);
			}
			ClearNeighbors();
			Points[0] = Points[1] = Points[2] = null;
		}
			
		public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
		{
			Debug.Assert(t != this, "self-pointer error");
			return PointCWFrom(t.PointCWFrom(p));
		}


		public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
		{
			return Neighbors[(Points.IndexOf(point) + 1) % 3];
		}

		public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
		{
			return Neighbors[(Points.IndexOf(point) + 2) % 3];
		}
			
		public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
		{
			return Neighbors[Points.IndexOf(point)];
		}


		public TriangulationPoint PointCCWFrom(TriangulationPoint point)
		{
			return Points[(IndexOf(point) + 1) % 3];
		}


		public TriangulationPoint PointCWFrom(TriangulationPoint point)
		{
			return Points[(IndexOf(point) + 2) % 3];
		}
			
		private void RotateCW()
		{
			var t = Points[2];
			Points[2] = Points[1];
			Points[1] = Points[0];
			Points[0] = t;
		}
			
		public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
		{
			RotateCW();
			Points[IndexCCWFrom(oPoint)] = nPoint;
		}


		public override string ToString()
		{
			return Points[0] + "," + Points[1] + "," + Points[2];
		}
			
		public void MarkNeighborEdges()
		{
			for (int i = 0; i < 3; i++)
				if (EdgeIsConstrained[i] && Neighbors[i] != null)
					Neighbors[i].MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
		}

		public void MarkEdge(DelaunayTriangle triangle)
		{
			for (int i = 0; i < 3; i++) if (EdgeIsConstrained[i])
				triangle.MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
		}

		public void MarkEdge(List<DelaunayTriangle> tList)
		{
			foreach (DelaunayTriangle t in tList)
				for (int i = 0; i < 3; i++)
					if (t.EdgeIsConstrained[i])
						MarkConstrainedEdge(t.Points[(i + 1) % 3], t.Points[(i + 2) % 3]);
		}

		public void MarkConstrainedEdge(int index)
		{
			mEdgeIsConstrained[index] = true;
		}

		public void MarkConstrainedEdge(DTSweepConstraint edge)
		{
			MarkConstrainedEdge(edge.P, edge.Q);
		}
			
		public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
		{
			int i = EdgeIndex(p, q);
			if (i != -1)
				mEdgeIsConstrained[i] = true;
		}
			
		public double Area()
		{
			double b = Points[0].X - Points[1].X;
			double h = Points[2].Y - Points[1].Y;

			return Math.Abs((b * h * 0.5f));
		}

		public TriangulationPoint Centroid()
		{
			double cx = (Points[0].X + Points[1].X + Points[2].X) / 3f;
			double cy = (Points[0].Y + Points[1].Y + Points[2].Y) / 3f;
			return new TriangulationPoint(cx, cy);
		}
			
		public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
		{
			int i1 = Points.IndexOf(p1);
			int i2 = Points.IndexOf(p2);

			bool a = (i1 == 0 || i2 == 0);
			bool b = (i1 == 1 || i2 == 1);
			bool c = (i1 == 2 || i2 == 2);

			if (b && c)
				return 0;
			if (a && c)
				return 1;
			if (a && b)
				return 2;

			return -1;
		}

		public bool GetConstrainedEdgeCCW(TriangulationPoint p) { return EdgeIsConstrained[(IndexOf(p) + 2) % 3]; }
		public bool GetConstrainedEdgeCW(TriangulationPoint p) { return EdgeIsConstrained[(IndexOf(p) + 1) % 3]; }
		public bool GetConstrainedEdgeAcross(TriangulationPoint p) { return EdgeIsConstrained[IndexOf(p)]; }

		protected void SetConstrainedEdge(int idx, bool ce)
		{
			mEdgeIsConstrained[idx] = ce;
		}
		public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce)
		{
			int idx = (IndexOf(p) + 2) % 3;
			SetConstrainedEdge(idx, ce);
		}
		public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce)
		{
			int idx = (IndexOf(p) + 1) % 3;
			SetConstrainedEdge(idx, ce);
		}
		public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce)
		{
			int idx = IndexOf(p);
			SetConstrainedEdge(idx, ce);
		}

		public bool GetDelaunayEdgeCCW(TriangulationPoint p) { return EdgeIsDelaunay[(IndexOf(p) + 2) % 3]; }
		public bool GetDelaunayEdgeCW(TriangulationPoint p) { return EdgeIsDelaunay[(IndexOf(p) + 1) % 3]; }
		public bool GetDelaunayEdgeAcross(TriangulationPoint p) { return EdgeIsDelaunay[IndexOf(p)]; }
		public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce) { EdgeIsDelaunay[(IndexOf(p) + 2) % 3] = ce; }
		public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce) { EdgeIsDelaunay[(IndexOf(p) + 1) % 3] = ce; }
		public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce) { EdgeIsDelaunay[IndexOf(p)] = ce; }

		public bool GetEdge(int idx, out DTSweepConstraint edge)
		{
			edge = null;
			if (idx < 0 || idx > 2)
				return false;
			TriangulationPoint p1 = Points[(idx + 1) % 3];
			TriangulationPoint p2 = Points[(idx + 2) % 3];
			if (p1.GetEdge(p2, out edge))
				return true;
			else if (p2.GetEdge(p1, out edge))
				return true;

			return false;
		}

		public bool GetEdgeCCW(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int pointIndex = IndexOf(p);
			int edgeIdx = (pointIndex + 2)%3;

			return GetEdge(edgeIdx, out edge);
		}

		public bool GetEdgeCW(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int pointIndex = IndexOf(p);
			int edgeIdx = (pointIndex + 1) % 3;

			return GetEdge(edgeIdx, out edge);
		}

		public bool GetEdgeAcross(TriangulationPoint p, out DTSweepConstraint edge)
		{
			int pointIndex = IndexOf(p);
			int edgeIdx = pointIndex;

			return GetEdge(edgeIdx, out edge);
		}
	}
}
