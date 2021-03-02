using System.Text;
using System;
using System.Collections.Generic;

namespace Polygon2DTriangulation
{
	public static class DTSweep
	{
		private const double PI_div2 = Math.PI / 2;
		private const double PI_3div4 = 3 * Math.PI / 4;

		public static void Triangulate(DTSweepContext tcx)
		{
			tcx.CreateAdvancingFront();

			Sweep(tcx);

			FixupConstrainedEdges(tcx);

			if (tcx.TriangulationMode == TriangulationMode.Polygon)
				FinalizationPolygon(tcx);
			else 
			{
				FinalizationConvexHull(tcx);
				if (tcx.TriangulationMode == TriangulationMode.Constrained)
					tcx.FinalizeTriangulation();
				else
					tcx.FinalizeTriangulation();
			}

			tcx.Done();
		}

		private static void Sweep(DTSweepContext tcx)
		{
			var points = tcx.Points;
			TriangulationPoint point;
			AdvancingFrontNode node;

			for (int i = 1; i < points.Count; i++)
			{
				point = points[i];
				node = PointEvent(tcx, point);

				if (node != null && point.HasEdges)
				{
					foreach (DTSweepConstraint e in point.Edges)
					{
						if (tcx.IsDebugEnabled)
							tcx.DTDebugContext.ActiveConstraint = e;
						EdgeEvent(tcx, e, node);
					}
				}
				tcx.Update(null);
			}
		}


		private static void FixupConstrainedEdges(DTSweepContext tcx)
		{
			foreach(DelaunayTriangle t in tcx.Triangles)
				for (int i = 0; i < 3; ++i)
				{
					bool isConstrained = t.GetConstrainedEdgeCCW(t.Points[i]);
					if (!isConstrained)
					{
						DTSweepConstraint edge = null;
						bool hasConstrainedEdge = t.GetEdgeCCW(t.Points[i], out edge);
						if (hasConstrainedEdge)
							t.MarkConstrainedEdge((i + 2) % 3);
					}
				}
		}

		private static void FinalizationConvexHull(DTSweepContext tcx)
		{
			AdvancingFrontNode n1, n2;
			DelaunayTriangle t1, t2;
			TriangulationPoint first, p1;

			n1 = tcx.Front.Head.Next;
			n2 = n1.Next;
			first = n1.Point;

			TurnAdvancingFrontConvex(tcx, n1, n2);

			n1 = tcx.Front.Tail.Prev;
			if (n1.Triangle.Contains(n1.Next.Point) && n1.Triangle.Contains(n1.Prev.Point))
			{
				t1 = n1.Triangle.NeighborAcrossFrom(n1.Point);
				RotateTrianglePair(n1.Triangle, n1.Point, t1, t1.OppositePoint(n1.Triangle, n1.Point));
				tcx.MapTriangleToNodes(n1.Triangle);
				tcx.MapTriangleToNodes(t1);
			}
			n1 = tcx.Front.Head.Next;
			if (n1.Triangle.Contains(n1.Prev.Point) && n1.Triangle.Contains(n1.Next.Point))
			{
				t1 = n1.Triangle.NeighborAcrossFrom(n1.Point);
				RotateTrianglePair(n1.Triangle, n1.Point, t1, t1.OppositePoint(n1.Triangle, n1.Point));
				tcx.MapTriangleToNodes(n1.Triangle);
				tcx.MapTriangleToNodes(t1);
			}

			first = tcx.Front.Head.Point;
			n2 = tcx.Front.Tail.Prev;
			t1 = n2.Triangle;
			p1 = n2.Point;
			n2.Triangle = null;
			do
			{
				tcx.RemoveFromList(t1);
				p1 = t1.PointCCWFrom(p1);
				if (p1 == first)
					break;
				t2 = t1.NeighborCCWFrom(p1);
				t1.Clear();
				t1 = t2;
			} while (true);

			first = tcx.Front.Head.Next.Point;
			p1 = t1.PointCWFrom(tcx.Front.Head.Point);
			t2 = t1.NeighborCWFrom(tcx.Front.Head.Point);
			t1.Clear();
			t1 = t2;
			while (p1 != first)
			{
				tcx.RemoveFromList(t1);
				p1 = t1.PointCCWFrom(p1);
				t2 = t1.NeighborCCWFrom(p1);
				t1.Clear();
				t1 = t2;
			}

			tcx.Front.Head = tcx.Front.Head.Next;
			tcx.Front.Head.Prev = null;
			tcx.Front.Tail = tcx.Front.Tail.Prev;
			tcx.Front.Tail.Next = null; 
		}

		private static void TurnAdvancingFrontConvex(DTSweepContext tcx, AdvancingFrontNode b, AdvancingFrontNode c)
		{
			AdvancingFrontNode first = b;
			while (c != tcx.Front.Tail)
			{
				if (tcx.IsDebugEnabled)
				{
					tcx.DTDebugContext.ActiveNode = c;
				}

				if (TriangulationUtil.Orient2d(b.Point, c.Point, c.Next.Point) == Orientation.CCW)
				{
					Fill(tcx, c);
					c = c.Next;
				}
				else
				{
					if (b != first && TriangulationUtil.Orient2d(b.Prev.Point, b.Point, c.Point) == Orientation.CCW)
					{
						Fill(tcx, b);
						b = b.Prev;
					}
					else
					{
						b = c;
						c = c.Next;
					}
				}
			}
		}


		private static void FinalizationPolygon(DTSweepContext tcx)
		{
			DelaunayTriangle t = tcx.Front.Head.Next.Triangle;
			TriangulationPoint p = tcx.Front.Head.Next.Point;
			while (!t.GetConstrainedEdgeCW(p))
			{
				DelaunayTriangle tTmp = t.NeighborCCWFrom(p);
				if (tTmp == null)
				{
					break;
				}
				t = tTmp;
			}

			tcx.MeshClean(t);
		}

		private static void FinalizationConstraints(DTSweepContext tcx)
		{
			DelaunayTriangle t = tcx.Front.Head.Triangle;
			TriangulationPoint p = tcx.Front.Head.Point;
			while (!t.GetConstrainedEdgeCW(p))
			{
				DelaunayTriangle tTmp = t.NeighborCCWFrom(p);
				if (tTmp == null)
				{
					break;
				}
				t = tTmp;
			}

			tcx.MeshClean(t);
		}

		private static AdvancingFrontNode PointEvent(DTSweepContext tcx, TriangulationPoint point)
		{
			AdvancingFrontNode node, newNode;

			node = tcx.LocateNode(point);
			if (tcx.IsDebugEnabled)
				tcx.DTDebugContext.ActiveNode = node;
			if (node == null || point == null)
				return null;
			newNode = NewFrontTriangle(tcx, point, node);

			if (point.X <= node.Point.X + MathUtil.EPSILON)
				Fill(tcx, node);

			tcx.AddNode(newNode);

			FillAdvancingFront(tcx, newNode);
			return newNode;
		}

		private static AdvancingFrontNode NewFrontTriangle(DTSweepContext tcx, TriangulationPoint point, AdvancingFrontNode node)
		{
			AdvancingFrontNode newNode;
			DelaunayTriangle triangle;

			triangle = new DelaunayTriangle(point, node.Point, node.Next.Point);
			triangle.MarkNeighbor(node.Triangle);
			tcx.Triangles.Add(triangle);

			newNode = new AdvancingFrontNode(point);
			newNode.Next = node.Next;
			newNode.Prev = node;
			node.Next.Prev = newNode;
			node.Next = newNode;

			tcx.AddNode(newNode); 

			if (tcx.IsDebugEnabled)
				tcx.DTDebugContext.ActiveNode = newNode;

			if (!Legalize(tcx, triangle))
				tcx.MapTriangleToNodes(triangle);

			return newNode;
		}


		private static void EdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			try
			{
				tcx.EdgeEvent.ConstrainedEdge = edge;
				tcx.EdgeEvent.Right = edge.P.X > edge.Q.X;

				if (tcx.IsDebugEnabled)
					tcx.DTDebugContext.PrimaryTriangle = node.Triangle;

				if (IsEdgeSideOfTriangle(node.Triangle, edge.P, edge.Q))
					return;

				FillEdgeEvent(tcx, edge, node);

				EdgeEvent(tcx, edge.P, edge.Q, node.Triangle, edge.Q);
			}
			catch (PointOnEdgeException)
			{
				throw;
			}
		}


		private static void FillEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (tcx.EdgeEvent.Right)
			{
				FillRightAboveEdgeEvent(tcx, edge, node);
			}
			else
			{
				FillLeftAboveEdgeEvent(tcx, edge, node);
			}
		}


		private static void FillRightConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			Fill(tcx, node.Next);
			if (node.Next.Point != edge.P)
				if (TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P) == Orientation.CCW)
					if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
						FillRightConcaveEdgeEvent(tcx, edge, node);
		}


		private static void FillRightConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Next.Point, node.Next.Next.Point, node.Next.Next.Next.Point) == Orientation.CCW)
				FillRightConcaveEdgeEvent(tcx, edge, node.Next);
			else
				if (TriangulationUtil.Orient2d(edge.Q, node.Next.Next.Point, edge.P) == Orientation.CCW)
					FillRightConvexEdgeEvent(tcx, edge, node.Next);
		}

		private static void FillRightBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (tcx.IsDebugEnabled)
				tcx.DTDebugContext.ActiveNode = node;

			if (node.Point.X < edge.P.X)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
					FillRightConcaveEdgeEvent(tcx, edge, node);
				else
				{
					FillRightConvexEdgeEvent(tcx, edge, node);
					FillRightBelowEdgeEvent(tcx, edge, node);
				}
			}
		}


		private static void FillRightAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			while (node.Next.Point.X < edge.P.X)
			{
				if (tcx.IsDebugEnabled) { tcx.DTDebugContext.ActiveNode = node; }
				Orientation o1 = TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P);
				if (o1 == Orientation.CCW)
					FillRightBelowEdgeEvent(tcx, edge, node);
				else
					node = node.Next;
			}
		}


		private static void FillLeftConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Prev.Point, node.Prev.Prev.Point, node.Prev.Prev.Prev.Point) == Orientation.CW)
				FillLeftConcaveEdgeEvent(tcx, edge, node.Prev);
			else
			{
				if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Prev.Point, edge.P) == Orientation.CW)
					FillLeftConvexEdgeEvent(tcx, edge, node.Prev);
			}
		}


		private static void FillLeftConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			Fill(tcx, node.Prev);
			if (node.Prev.Point != edge.P)
				if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P) == Orientation.CW)
				{
					if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
						FillLeftConcaveEdgeEvent(tcx, edge, node);
				}
		}


		private static void FillLeftBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			if (tcx.IsDebugEnabled)
				tcx.DTDebugContext.ActiveNode = node;

			if (node.Point.X > edge.P.X)
			{
				if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
				{ 
					FillLeftConcaveEdgeEvent(tcx, edge, node);
				}
				else
				{

					FillLeftConvexEdgeEvent(tcx, edge, node);
					FillLeftBelowEdgeEvent(tcx, edge, node);
				}

			}
		}


		private static void FillLeftAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
		{
			while (node.Prev.Point.X > edge.P.X)
			{
				if (tcx.IsDebugEnabled)
					tcx.DTDebugContext.ActiveNode = node;

				Orientation o1 = TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P);
				if (o1 == Orientation.CW)
				{
					FillLeftBelowEdgeEvent(tcx, edge, node);
				}
				else
				{
					node = node.Prev;
				}
			}
		}


		private static bool IsEdgeSideOfTriangle(DelaunayTriangle triangle, TriangulationPoint ep, TriangulationPoint eq)
		{
			int index = triangle.EdgeIndex(ep, eq);
			if (index == -1)
				return false;
			triangle.MarkConstrainedEdge(index);
			triangle = triangle.Neighbors[index];
			if (triangle != null)
				triangle.MarkConstrainedEdge(ep, eq);
			return true;
		}


		private static void EdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle triangle, TriangulationPoint point)
		{
			TriangulationPoint p1, p2;

			if (tcx.IsDebugEnabled)
				tcx.DTDebugContext.PrimaryTriangle = triangle;

			if (IsEdgeSideOfTriangle(triangle, ep, eq))
				return;

			p1 = triangle.PointCCWFrom(point);
			Orientation o1 = TriangulationUtil.Orient2d(eq, p1, ep);
			if (o1 == Orientation.Collinear)
			{
				if (triangle.Contains(eq) && triangle.Contains(p1))
				{
					triangle.MarkConstrainedEdge(eq, p1);

					tcx.EdgeEvent.ConstrainedEdge.Q = p1;
					triangle = triangle.NeighborAcrossFrom(point);
					EdgeEvent(tcx, ep, p1, triangle, p1);
				}
				else
				{
					throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, p1);
				}
				if (tcx.IsDebugEnabled)
					Console.WriteLine("EdgeEvent - Point on constrained edge");

				return;
			}

			p2 = triangle.PointCWFrom(point);
			Orientation o2 = TriangulationUtil.Orient2d(eq, p2, ep);
			if (o2 == Orientation.Collinear)
			{
				if (triangle.Contains(eq) && triangle.Contains(p2))
				{
					triangle.MarkConstrainedEdge(eq, p2);

					tcx.EdgeEvent.ConstrainedEdge.Q = p2;
					triangle = triangle.NeighborAcrossFrom(point);
					EdgeEvent(tcx, ep, p2, triangle, p2);
				}
				else
				{
					throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, p2);
				}
				if (tcx.IsDebugEnabled)
				{
					Console.WriteLine("EdgeEvent - Point on constrained edge");
				}

				return;
			}

			if (o1 == o2)
			{
				if (o1 == Orientation.CW)
				{
					triangle = triangle.NeighborCCWFrom(point);
				}
				else
				{
					triangle = triangle.NeighborCWFrom(point);
				}
				EdgeEvent(tcx, ep, eq, triangle, point);
			}
			else
			{
				FlipEdgeEvent(tcx, ep, eq, triangle, point);
			}
		}


		private static void FlipEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle t, TriangulationPoint p)
		{
			DelaunayTriangle ot = t.NeighborAcrossFrom(p);
			TriangulationPoint op = ot.OppositePoint(t, p);

			if (ot == null)
			{
				throw new InvalidOperationException("[BUG:FIXME] FLIP failed due to missing triangle");
			}

			if (tcx.IsDebugEnabled)
			{
				tcx.DTDebugContext.PrimaryTriangle = t;
				tcx.DTDebugContext.SecondaryTriangle = ot;
			}

			bool inScanArea = TriangulationUtil.InScanArea(p, t.PointCCWFrom(p), t.PointCWFrom(p), op);
			if (inScanArea)
			{
				RotateTrianglePair(t, p, ot, op);
				tcx.MapTriangleToNodes(t);
				tcx.MapTriangleToNodes(ot);

				if (p == eq && op == ep)
				{
					if (eq == tcx.EdgeEvent.ConstrainedEdge.Q && ep == tcx.EdgeEvent.ConstrainedEdge.P)
					{
						if (tcx.IsDebugEnabled)
						{
							Console.WriteLine("[FLIP] - constrained edge done");
						}
						t.MarkConstrainedEdge(ep, eq);
						ot.MarkConstrainedEdge(ep, eq);
						Legalize(tcx, t);
						Legalize(tcx, ot);
					}
					else
					{
						if (tcx.IsDebugEnabled)
						{
							Console.WriteLine("[FLIP] - subedge done");
						}
					}
				}
				else
				{
					Orientation o = TriangulationUtil.Orient2d(eq, op, ep);
					t = NextFlipTriangle(tcx, o, t, ot, p, op);
					FlipEdgeEvent(tcx, ep, eq, t, p);
				}
			}
			else
			{
				TriangulationPoint newP = null;
				if (NextFlipPoint(ep, eq, ot, op, out newP))
				{
					FlipScanEdgeEvent(tcx, ep, eq, t, ot, newP);
					EdgeEvent(tcx, ep, eq, t, p);
				}
			}
		}

		private static bool NextFlipPoint(TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle ot, TriangulationPoint op, out TriangulationPoint newP)
		{
			newP = null;
			Orientation o2d = TriangulationUtil.Orient2d(eq, op, ep);
			switch (o2d)
			{
			case Orientation.CW:
				newP = ot.PointCCWFrom(op);
				return true;
			case Orientation.CCW:
				newP = ot.PointCWFrom(op);
				return true;
			case Orientation.Collinear:
				return false;
			default:
				throw new NotImplementedException("Orientation not handled");
			}
		}

		private static DelaunayTriangle NextFlipTriangle(DTSweepContext tcx, Orientation o, DelaunayTriangle t, DelaunayTriangle ot, TriangulationPoint p, TriangulationPoint op)
		{
			int edgeIndex;
			if (o == Orientation.CCW)
			{
				edgeIndex = ot.EdgeIndex(p, op);
				ot.EdgeIsDelaunay[edgeIndex] = true;
				Legalize(tcx, ot);
				ot.EdgeIsDelaunay.Clear();
				return t;
			}
			edgeIndex = t.EdgeIndex(p, op);
			t.EdgeIsDelaunay[edgeIndex] = true;
			Legalize(tcx, t);
			t.EdgeIsDelaunay.Clear();
			return ot;
		}

		private static void FlipScanEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle flipTriangle, DelaunayTriangle t, TriangulationPoint p)
		{
			DelaunayTriangle ot;
			TriangulationPoint op, newP;
			bool inScanArea;

			ot = t.NeighborAcrossFrom(p);
			op = ot.OppositePoint(t, p);

			if (ot == null)
			{
				throw new Exception("FLIP failed due to missing triangle");
			}

			if (tcx.IsDebugEnabled)
			{
				Console.WriteLine("scan next point");
				tcx.DTDebugContext.PrimaryTriangle = t;
				tcx.DTDebugContext.SecondaryTriangle = ot;
			}

			inScanArea = TriangulationUtil.InScanArea(eq, flipTriangle.PointCCWFrom(eq), flipTriangle.PointCWFrom(eq), op);
			if (inScanArea)
			{
				FlipEdgeEvent(tcx, eq, op, ot, op);
			}
			else
			{
				if (NextFlipPoint(ep, eq, ot, op, out newP))
				{
					FlipScanEdgeEvent(tcx, ep, eq, flipTriangle, ot, newP);
				}
			}
		}

		private static void FillAdvancingFront(DTSweepContext tcx, AdvancingFrontNode n)
		{
			AdvancingFrontNode node;
			double angle;

			node = n.Next;
			while (node.HasNext)
			{
				angle = HoleAngle(node);
				if (angle > PI_div2 || angle < -PI_div2)
				{
					break;
				}
				Fill(tcx, node);
				node = node.Next;
			}

			node = n.Prev;
			while (node.HasPrev)
			{
				angle = HoleAngle(node);
				if (angle > PI_div2 || angle < -PI_div2)
				{
					break;
				}
				Fill(tcx, node);
				node = node.Prev;
			}

			if (n.HasNext && n.Next.HasNext)
			{
				angle = BasinAngle(n);
				if (angle < PI_3div4)
				{
					FillBasin(tcx, n);
				}
			}
		}

		private static void FillBasin(DTSweepContext tcx, AdvancingFrontNode node)
		{
			if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
			{
				tcx.Basin.leftNode = node;
			}
			else
			{
				tcx.Basin.leftNode = node.Next;
			}

			tcx.Basin.bottomNode = tcx.Basin.leftNode;
			while (tcx.Basin.bottomNode.HasNext && tcx.Basin.bottomNode.Point.Y >= tcx.Basin.bottomNode.Next.Point.Y)
			{
				tcx.Basin.bottomNode = tcx.Basin.bottomNode.Next;
			}

			if (tcx.Basin.bottomNode == tcx.Basin.leftNode)
			{
				return;
			}

			tcx.Basin.rightNode = tcx.Basin.bottomNode;
			while (tcx.Basin.rightNode.HasNext && tcx.Basin.rightNode.Point.Y < tcx.Basin.rightNode.Next.Point.Y)
			{
				tcx.Basin.rightNode = tcx.Basin.rightNode.Next;
			}

			if (tcx.Basin.rightNode == tcx.Basin.bottomNode)
				return; 

			tcx.Basin.width = tcx.Basin.rightNode.Point.X - tcx.Basin.leftNode.Point.X;
			tcx.Basin.leftHighest = tcx.Basin.leftNode.Point.Y > tcx.Basin.rightNode.Point.Y;

			FillBasinReq(tcx, tcx.Basin.bottomNode);
		}

		private static void FillBasinReq(DTSweepContext tcx, AdvancingFrontNode node)
		{
			if (IsShallow(tcx, node))
				return;

			Fill(tcx, node);
			if (node.Prev == tcx.Basin.leftNode && node.Next == tcx.Basin.rightNode)
				return;
			else if (node.Prev == tcx.Basin.leftNode)
			{
				Orientation o = TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point);
				if (o == Orientation.CW)
					return;
				node = node.Next;
			}
			else if (node.Next == tcx.Basin.rightNode)
			{
				Orientation o = TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point);
				if (o == Orientation.CCW)
					return;
				node = node.Prev;
			}
			else
			{
				if (node.Prev.Point.Y < node.Next.Point.Y)
					node = node.Prev;
				else
					node = node.Next;
			}
			FillBasinReq(tcx, node);
		}


		private static bool IsShallow(DTSweepContext tcx, AdvancingFrontNode node)
		{
			double height;

			if (tcx.Basin.leftHighest)
			{
				height = tcx.Basin.leftNode.Point.Y - node.Point.Y;
			}
			else
			{
				height = tcx.Basin.rightNode.Point.Y - node.Point.Y;
			}
			if (tcx.Basin.width > height)
			{
				return true;
			}
			return false;
		}

		private static double HoleAngle(AdvancingFrontNode node)
		{
			double px = node.Point.X;
			double py = node.Point.Y;
			double ax = node.Next.Point.X - px;
			double ay = node.Next.Point.Y - py;
			double bx = node.Prev.Point.X - px;
			double by = node.Prev.Point.Y - py;
			return Math.Atan2((ax * by) - (ay * bx), (ax * bx) + (ay * by));
		}

		private static double BasinAngle(AdvancingFrontNode node)
		{
			double ax = node.Point.X - node.Next.Next.Point.X;
			double ay = node.Point.Y - node.Next.Next.Point.Y;
			return Math.Atan2(ay, ax);
		}

		private static void Fill(DTSweepContext tcx, AdvancingFrontNode node)
		{
			DelaunayTriangle triangle = new DelaunayTriangle(node.Prev.Point, node.Point, node.Next.Point);

			triangle.MarkNeighbor(node.Prev.Triangle);
			triangle.MarkNeighbor(node.Triangle);
			tcx.Triangles.Add(triangle);

			node.Prev.Next = node.Next;
			node.Next.Prev = node.Prev;
			tcx.RemoveNode(node);

			if (!Legalize(tcx, triangle))
			{
				tcx.MapTriangleToNodes(triangle);
			}
		}

		private static bool Legalize(DTSweepContext tcx, DelaunayTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				if (t.EdgeIsDelaunay[i])
				{
					continue;
				}

				DelaunayTriangle ot = t.Neighbors[i];
				if (ot == null)
				{
					continue;
				}

				TriangulationPoint p = t.Points[i];
				TriangulationPoint op = ot.OppositePoint(t, p);
				int oi = ot.IndexOf(op);

				if (ot.EdgeIsConstrained[oi] || ot.EdgeIsDelaunay[oi])
				{
					t.SetConstrainedEdgeAcross(p, ot.EdgeIsConstrained[oi]); 
					continue;
				}

				if (!TriangulationUtil.SmartIncircle(p, t.PointCCWFrom(p), t.PointCWFrom(p), op))
				{
					continue;
				}


				t.EdgeIsDelaunay[i] = true;
				ot.EdgeIsDelaunay[oi] = true;

				RotateTrianglePair(t, p, ot, op);

				if (!Legalize(tcx, t))
				{
					tcx.MapTriangleToNodes(t);
				}
				if (!Legalize(tcx, ot))
				{
					tcx.MapTriangleToNodes(ot);
				}

				t.EdgeIsDelaunay[i] = false;
				ot.EdgeIsDelaunay[oi] = false;

				return true;
			}
			return false;
		}

		private static void RotateTrianglePair(DelaunayTriangle t, TriangulationPoint p, DelaunayTriangle ot, TriangulationPoint op)
		{
			DelaunayTriangle n1, n2, n3, n4;
			n1 = t.NeighborCCWFrom(p);
			n2 = t.NeighborCWFrom(p);
			n3 = ot.NeighborCCWFrom(op);
			n4 = ot.NeighborCWFrom(op);

			bool ce1, ce2, ce3, ce4;
			ce1 = t.GetConstrainedEdgeCCW(p);
			ce2 = t.GetConstrainedEdgeCW(p);
			ce3 = ot.GetConstrainedEdgeCCW(op);
			ce4 = ot.GetConstrainedEdgeCW(op);

			bool de1, de2, de3, de4;
			de1 = t.GetDelaunayEdgeCCW(p);
			de2 = t.GetDelaunayEdgeCW(p);
			de3 = ot.GetDelaunayEdgeCCW(op);
			de4 = ot.GetDelaunayEdgeCW(op);

			t.Legalize(p, op);
			ot.Legalize(op, p);

			ot.SetDelaunayEdgeCCW(p, de1);
			t.SetDelaunayEdgeCW(p, de2);
			t.SetDelaunayEdgeCCW(op, de3);
			ot.SetDelaunayEdgeCW(op, de4);

			ot.SetConstrainedEdgeCCW(p, ce1);
			t.SetConstrainedEdgeCW(p, ce2);
			t.SetConstrainedEdgeCCW(op, ce3);
			ot.SetConstrainedEdgeCW(op, ce4);

			t.Neighbors.Clear();
			ot.Neighbors.Clear();
			if (n1 != null)
				ot.MarkNeighbor(n1);
			if (n2 != null)
				t.MarkNeighbor(n2);
			if (n3 != null)
				t.MarkNeighbor(n3);
			if (n4 != null)
				ot.MarkNeighbor(n4);
			t.MarkNeighbor(ot);
		}
	}


	public class AdvancingFront
	{
		public AdvancingFrontNode Head;
		public AdvancingFrontNode Tail;
		protected AdvancingFrontNode Search;

		public AdvancingFront(AdvancingFrontNode head, AdvancingFrontNode tail)
		{
			this.Head = head;
			this.Tail = tail;
			this.Search = head;
			AddNode(head);
			AddNode(tail);
		}

		public void AddNode(AdvancingFrontNode node) { }
		public void RemoveNode(AdvancingFrontNode node) { }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			AdvancingFrontNode node = Head;
			while (node != Tail)
			{
				sb.Append(node.Point.X).Append("->");
				node = node.Next;
			}
			sb.Append(Tail.Point.X);
			return sb.ToString();
		}

		private AdvancingFrontNode FindSearchNode(double x)
		{
			return Search;
		}

		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return LocateNode(point.X);
		}

		private AdvancingFrontNode LocateNode(double x)
		{
			AdvancingFrontNode node = FindSearchNode(x);
			if (x < node.Value)
			{
				while ((node = node.Prev) != null)
					if (x >= node.Value)
					{
						Search = node;
						return node;
					}
			}
			else
				while ((node = node.Next) != null)
					if (x < node.Value)
					{
						Search = node.Prev;
						return node.Prev;
					}

			return null;
		}

		public AdvancingFrontNode LocatePoint(TriangulationPoint point)
		{
			double px = point.X;
			AdvancingFrontNode node = FindSearchNode(px);
			double nx = node.Point.X;

			if (px == nx) {
				if (point != node.Point) {
					if (point == node.Prev.Point)
						node = node.Prev;
					else if (point == node.Next.Point)
						node = node.Next;
					else
						throw new Exception ("Failed to find Node for given afront point");
				}
			} else if (px < nx) {
				while ((node = node.Prev) != null)
					if (point == node.Point)
						break;
			} else
				while ((node = node.Next) != null)
					if (point == node.Point)
						break;

			Search = node;
			return node;
		}
	}

	public class AdvancingFrontNode
	{
		public AdvancingFrontNode Next;
		public AdvancingFrontNode Prev;
		public double Value;
		public TriangulationPoint Point;
		public DelaunayTriangle Triangle;

		public AdvancingFrontNode(TriangulationPoint point)
		{
			this.Point = point;
			Value = point.X;
		}

		public bool HasNext { get { return Next != null; } }
		public bool HasPrev { get { return Prev != null; } }
	}

	public class DTSweepBasin
	{
		public AdvancingFrontNode leftNode;
		public AdvancingFrontNode bottomNode;
		public AdvancingFrontNode rightNode;
		public double width;
		public bool leftHighest;
	}

	public class DTSweepConstraint : TriangulationConstraint {
		public DTSweepConstraint(TriangulationPoint p1, TriangulationPoint p2) : base(p1, p2) { Q.AddEdge(this); }
	}

	public class DTSweepDebugContext : TriangulationDebugContext
	{
		public DelaunayTriangle PrimaryTriangle { get { return _primaryTriangle; } set { _primaryTriangle = value; _tcx.Update("set PrimaryTriangle"); } }
		public DelaunayTriangle SecondaryTriangle { get { return _secondaryTriangle; } set { _secondaryTriangle = value; _tcx.Update("set SecondaryTriangle"); } }
		public TriangulationPoint ActivePoint { get { return _activePoint; } set { _activePoint = value; _tcx.Update("set ActivePoint"); } }
		public AdvancingFrontNode ActiveNode { get { return _activeNode; } set { _activeNode = value; _tcx.Update("set ActiveNode"); } }
		public DTSweepConstraint ActiveConstraint { get { return _activeConstraint; } set { _activeConstraint = value; _tcx.Update("set ActiveConstraint"); } }

		public DTSweepDebugContext(DTSweepContext tcx) : base(tcx) { }

		public bool IsDebugContext { get { return true; } }

		public override void Clear()
		{
			PrimaryTriangle = null;
			SecondaryTriangle = null;
			ActivePoint = null;
			ActiveNode = null;
			ActiveConstraint = null;
		}

		private DelaunayTriangle _primaryTriangle;
		private DelaunayTriangle _secondaryTriangle;
		private TriangulationPoint _activePoint;
		private AdvancingFrontNode _activeNode;
		private DTSweepConstraint _activeConstraint;
	}

	public class DTSweepEdgeEvent
	{
		public DTSweepConstraint ConstrainedEdge;
		public bool Right;
	}

	public class DTSweepPointComparator : IComparer<TriangulationPoint>
	{
		public int Compare(TriangulationPoint p1, TriangulationPoint p2)
		{
			if (p1.Y < p2.Y)
				return -1;
			else if (p1.Y > p2.Y)
				return 1;
			else {
				if (p1.X < p2.X)
					return -1;
				else if (p1.X > p2.X)
					return 1;
				else
					return 0;
			}
		}
	}

	public class PointOnEdgeException : NotImplementedException
	{
		public readonly TriangulationPoint A, B, C;

		public PointOnEdgeException(string message, TriangulationPoint a, TriangulationPoint b, TriangulationPoint c) : base(message)
		{
			A = a;
			B = b;
			C = c;
		}
	}

	public class DTSweepContext : TriangulationContext
	{
		private readonly float ALPHA = 0.3f;

		public AdvancingFront Front;
		public TriangulationPoint Head { get; set; }
		public TriangulationPoint Tail { get; set; }

		public DTSweepBasin Basin = new DTSweepBasin();
		public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

		private DTSweepPointComparator _comparator = new DTSweepPointComparator();

		public override TriangulationAlgorithm Algorithm { get { return TriangulationAlgorithm.DTSweep; } }

		public DTSweepContext()
		{
			Clear();
		}

		public override bool IsDebugEnabled
		{
			get { return base.IsDebugEnabled; }
			protected set {
				if (value && DebugContext == null)
					DebugContext = new DTSweepDebugContext(this);
				base.IsDebugEnabled = value;
			}
		}

		public void RemoveFromList(DelaunayTriangle triangle)
		{
			Triangles.Remove(triangle);
		}

		public void MeshClean(DelaunayTriangle triangle)
		{
			MeshCleanReq(triangle);
		}

		private void MeshCleanReq(DelaunayTriangle triangle)
		{
			if (triangle != null && !triangle.IsInterior)
			{
				triangle.IsInterior = true;
				Triangulatable.AddTriangle(triangle);

				for (int i = 0; i < 3; i++)
					if (!triangle.EdgeIsConstrained[i])
						MeshCleanReq(triangle.Neighbors[i]);
			}
		}

		public override void Clear()
		{
			base.Clear();
			Triangles.Clear();
		}

		public void AddNode(AdvancingFrontNode node)
		{
			Front.AddNode(node);
		}

		public void RemoveNode(AdvancingFrontNode node)
		{
			Front.RemoveNode(node);
		}

		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return Front.LocateNode(point);
		}

		public void CreateAdvancingFront()
		{
			AdvancingFrontNode head, tail, middle;
			DelaunayTriangle iTriangle = new DelaunayTriangle(Points[0], Tail, Head);
			Triangles.Add(iTriangle);

			head = new AdvancingFrontNode(iTriangle.Points[1]);
			head.Triangle = iTriangle;
			middle = new AdvancingFrontNode(iTriangle.Points[0]);
			middle.Triangle = iTriangle;
			tail = new AdvancingFrontNode(iTriangle.Points[2]);

			Front = new AdvancingFront(head, tail);
			Front.AddNode(middle);

			Front.Head.Next = middle;
			middle.Next = Front.Tail;
			middle.Prev = Front.Head;
			Front.Tail.Prev = middle;
		}

		public void MapTriangleToNodes(DelaunayTriangle t)
		{
			for (int i = 0; i < 3; i++)
				if (t.Neighbors[i] == null)
				{
					AdvancingFrontNode n = Front.LocatePoint(t.PointCWFrom(t.Points[i]));
					if (n != null)
						n.Triangle = t;
				}
		}

		public override void PrepareTriangulation(ITriangulatable t)
		{
			base.PrepareTriangulation(t);

			double xmax, xmin;
			double ymax, ymin;

			xmax = xmin = Points[0].X;
			ymax = ymin = Points[0].Y;

			foreach (TriangulationPoint p in Points)
			{
				if (p.X > xmax)
					xmax = p.X;
				if (p.X < xmin)
					xmin = p.X;
				if (p.Y > ymax)
					ymax = p.Y;
				if (p.Y < ymin)
					ymin = p.Y;
			}

			double deltaX = ALPHA * (xmax - xmin);
			double deltaY = ALPHA * (ymax - ymin);
			TriangulationPoint p1 = new TriangulationPoint(xmax + deltaX, ymin - deltaY);
			TriangulationPoint p2 = new TriangulationPoint(xmin - deltaX, ymin - deltaY);

			Head = p1;
			Tail = p2;

			Points.Sort(_comparator);
		}

		public void FinalizeTriangulation()
		{
			Triangulatable.AddTriangles(Triangles);
			Triangles.Clear();
		}

		public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
		{
			return new DTSweepConstraint(a, b);
		}

	}
}
