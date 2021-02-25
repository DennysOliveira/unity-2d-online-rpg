using System;
using System.Collections.Generic;
using System.Text;


namespace Polygon2DTriangulation
{
	public class PointSet : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
	{
		protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();
		public IList<TriangulationPoint> Points { get { return this; } private set { } }
		public IList<DelaunayTriangle> Triangles { get; private set; }

		public string FileName { get; set; }
		public bool DisplayFlipX { get; set; }
		public bool DisplayFlipY { get; set; }
		public float DisplayRotate { get; set; }

		protected double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;
		public double Precision { get { return mPrecision; } set { mPrecision = value; } }

		public double MinX { get { return mBoundingBox.MinX; } }
		public double MaxX { get { return mBoundingBox.MaxX; } }
		public double MinY { get { return mBoundingBox.MinY; } }
		public double MaxY { get { return mBoundingBox.MaxY; } }
		public Rect2D Bounds { get { return mBoundingBox; } }

		public virtual TriangulationMode TriangulationMode { get { return TriangulationMode.Unconstrained; } }

		public new TriangulationPoint this[int index]
		{
			get { return mPoints[index] as TriangulationPoint; }
			set { mPoints[index] = value; }
		}

		public PointSet(List<TriangulationPoint> bounds)
		{
			foreach (TriangulationPoint p in bounds)
			{
				Add(p, -1, false);
				mBoundingBox.AddPoint(p);
			}
			mEpsilon = CalculateEpsilon();
			mWindingOrder = WindingOrderType.Unknown; 
		}

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
			Add(p as TriangulationPoint, -1, false);
		}

		public virtual void Add(TriangulationPoint p)
		{
			Add(p, -1, false);
		}

		protected override void Add(Point2D p, int idx, bool constrainToBounds)
		{
			Add(p as TriangulationPoint, idx, constrainToBounds);
		}

		protected bool Add(TriangulationPoint p, int idx, bool constrainToBounds)
		{
			if (p == null)
				return false;

			if (constrainToBounds)
				ConstrainPointToBounds(p);

			if (mPointMap.ContainsKey(p.VertexCode))
				return true;

			mPointMap.Add(p.VertexCode, p);

			if (idx < 0)
				mPoints.Add(p);
			else
				mPoints.Insert(idx, p);

			return true;
		}

		public override void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
		{
			if (iter == null)
				return;

			iter.Reset();
			while (iter.MoveNext())
				Add(iter.Current);
		}

		public virtual bool AddRange(List<TriangulationPoint> points)
		{
			bool bOK = true;
			foreach (TriangulationPoint p in points)
				bOK = Add(p, -1, false) && bOK;

			return bOK;
		}

		public bool TryGetPoint(double x, double y, out TriangulationPoint p)
		{
			uint vc = TriangulationPoint.CreateVertexCode(x, y, Precision);
			if (mPointMap.TryGetValue(vc, out p))
				return true;

			return false;
		}

		public void Insert(int idx, TriangulationPoint item)
		{
			mPoints.Insert(idx, item);
		}

		public override bool Remove(Point2D p)
		{
			return mPoints.Remove(p);
		}

		public bool Remove(TriangulationPoint p)
		{
			return mPoints.Remove(p);
		}

		public override void RemoveAt(int idx)
		{
			if (idx < 0 || idx >= Count)
				return;
			mPoints.RemoveAt(idx);
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

		protected bool ConstrainPointToBounds(Point2D p)
		{
			double oldX = p.X;
			double oldY = p.Y;
			p.X = Math.Max(MinX, p.X);
			p.X = Math.Min(MaxX, p.X);
			p.Y = Math.Max(MinY, p.Y);
			p.Y = Math.Min(MaxY, p.Y);

			return (p.X != oldX) || (p.Y != oldY);
		}

		protected bool ConstrainPointToBounds(TriangulationPoint p)
		{
			double oldX = p.X;
			double oldY = p.Y;
			p.X = Math.Max(MinX, p.X);
			p.X = Math.Min(MaxX, p.X);
			p.Y = Math.Max(MinY, p.Y);
			p.Y = Math.Min(MaxY, p.Y);

			return (p.X != oldX) || (p.Y != oldY);
		}

		public virtual void AddTriangle(DelaunayTriangle t)
		{
			Triangles.Add(t);
		}

		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			foreach (var tri in list)
				AddTriangle(tri);
		}

		public void ClearTriangles()
		{
			Triangles.Clear();
		}

		public virtual bool Initialize()
		{
			return true;
		}

		public virtual void Prepare(TriangulationContext tcx)
		{
			if (Triangles == null)
				Triangles = new List<DelaunayTriangle>(Points.Count);
			else
				Triangles.Clear();
			tcx.Points.AddRange(Points);
		}
	}
	public class ConstrainedPointSet : PointSet
	{
		protected Dictionary<uint, TriangulationConstraint> mConstraintMap = new Dictionary<uint, TriangulationConstraint>();
		protected List<Contour> mHoles = new List<Contour>();

		public override TriangulationMode TriangulationMode { get { return TriangulationMode.Constrained; } }

		public ConstrainedPointSet(List<TriangulationPoint> bounds) : base(bounds)
		{
			AddBoundaryConstraints();
		}

		public ConstrainedPointSet(List<TriangulationPoint> bounds, List<TriangulationConstraint> constraints) : base(bounds)
		{
			AddBoundaryConstraints();
			AddConstraints(constraints);
		}

		public ConstrainedPointSet(List<TriangulationPoint> bounds, int[] indices) : base(bounds)
		{
			AddBoundaryConstraints();
			List<TriangulationConstraint> l = new List<TriangulationConstraint>();
			for (int i = 0; i < indices.Length; i += 2)
			{
				TriangulationConstraint tc = new TriangulationConstraint(bounds[i], bounds[i + 1]);
				l.Add(tc);
			}
			AddConstraints(l);
		}

		protected void AddBoundaryConstraints()
		{
			TriangulationPoint ptLL = null;
			TriangulationPoint ptLR = null;
			TriangulationPoint ptUR = null;
			TriangulationPoint ptUL = null;
			if (!TryGetPoint(MinX, MinY, out ptLL))
			{
				ptLL = new TriangulationPoint(MinX, MinY);
				Add(ptLL);
			}
			if (!TryGetPoint(MaxX, MinY, out ptLR))
			{
				ptLR = new TriangulationPoint(MaxX, MinY);
				Add(ptLR);
			}
			if (!TryGetPoint(MaxX, MaxY, out ptUR))
			{
				ptUR = new TriangulationPoint(MaxX, MaxY);
				Add(ptUR);
			}
			if (!TryGetPoint(MinX, MaxY, out ptUL))
			{
				ptUL = new TriangulationPoint(MinX, MaxY);
				Add(ptUL);
			}
			TriangulationConstraint tcLLtoLR = new TriangulationConstraint(ptLL, ptLR);
			AddConstraint(tcLLtoLR);
			TriangulationConstraint tcLRtoUR = new TriangulationConstraint(ptLR, ptUR);
			AddConstraint(tcLRtoUR);
			TriangulationConstraint tcURtoUL = new TriangulationConstraint(ptUR, ptUL);
			AddConstraint(tcURtoUL);
			TriangulationConstraint tcULtoLL = new TriangulationConstraint(ptUL, ptLL);
			AddConstraint(tcULtoLL);
		}

		public override void Add(Point2D p)
		{
			Add(p as TriangulationPoint, -1, true);
		}

		public override void Add(TriangulationPoint p)
		{
			Add(p, -1, true);
		}

		public override bool AddRange(List<TriangulationPoint> points)
		{
			bool bOK = true;
			foreach (TriangulationPoint p in points)
				bOK = Add(p, -1, true) && bOK;

			return bOK;
		}

		public bool AddHole(List<TriangulationPoint> points, string name)
		{
			if (points == null)
				return false;

			List<Contour> pts = new List<Contour>();
			int listIdx = 0;
			{
				Contour c = new Contour(this, points, WindingOrderType.Unknown);
				pts.Add(c);

				if (mPoints.Count > 1)
				{
					int numPoints = pts[listIdx].Count;
					for (int i = 0; i < numPoints; ++i)
						ConstrainPointToBounds(pts[listIdx][i]);
				}
			}

			while (listIdx < pts.Count)
			{
				pts[listIdx].RemoveDuplicateNeighborPoints();
				pts[listIdx].WindingOrder = Point2DList.WindingOrderType.Default;

				bool bListOK = true;
				Point2DList.PolygonError err = pts[listIdx].CheckPolygon();
				while (bListOK && err != PolygonError.None)
				{
					if ((err & PolygonError.NotEnoughVertices) == PolygonError.NotEnoughVertices)
					{
						bListOK = false;
						continue;
					}
					if ((err & PolygonError.NotSimple) == PolygonError.NotSimple)
					{
						List<Point2DList> l = PolygonUtil.SplitComplexPolygon(pts[listIdx], pts[listIdx].Epsilon);
						pts.RemoveAt(listIdx);
						foreach (Point2DList newList in l)
						{
							Contour c = new Contour(this);
							c.AddRange(newList);
							pts.Add(c);
						}
						err = pts[listIdx].CheckPolygon();
						continue;
					}
					if ((err & PolygonError.Degenerate) == PolygonError.Degenerate) {
						pts[listIdx].Simplify(this.Epsilon);
						err = pts[listIdx].CheckPolygon();
						continue;
					}
					if ((err & PolygonError.AreaTooSmall) == PolygonError.AreaTooSmall || (err & PolygonError.SidesTooCloseToParallel) == PolygonError.SidesTooCloseToParallel || (err & PolygonError.TooThin) == PolygonError.TooThin || (err & PolygonError.Unknown) == PolygonError.Unknown)
					{
						bListOK = false;
						continue;
					}
				}
				if (!bListOK && pts[listIdx].Count != 2)
					pts.RemoveAt(listIdx);
				else
					++listIdx;
			}

			bool bOK = true;
			listIdx = 0;
			while (listIdx < pts.Count)
			{
				int numPoints = pts[listIdx].Count;
				if (numPoints < 2) {
					++listIdx;
					bOK = false;
					continue;
				} else if (numPoints == 2) {
					uint constraintCode = TriangulationConstraint.CalculateContraintCode(pts[listIdx][0], pts[listIdx][1]);
					TriangulationConstraint tc = null;
					if (!mConstraintMap.TryGetValue(constraintCode, out tc)) {
						tc = new TriangulationConstraint(pts[listIdx][0], pts[listIdx][1]);
						AddConstraint(tc);
					}
				} else {
					Contour ph = new Contour(this, pts[listIdx], Point2DList.WindingOrderType.Unknown);
					ph.WindingOrder = Point2DList.WindingOrderType.Default;
					ph.Name = name + ":" + listIdx.ToString();
					mHoles.Add(ph);
				}
				++listIdx;
			}

			return bOK;
		}

		public bool AddConstraints(List<TriangulationConstraint> constraints)
		{
			if (constraints == null || constraints.Count < 1)
				return false;

			bool bOK = true;
			foreach (TriangulationConstraint tc in constraints)
			{
				if (ConstrainPointToBounds(tc.P) || ConstrainPointToBounds(tc.Q))
					tc.CalculateContraintCode();

				TriangulationConstraint tcTmp = null;
				if (!mConstraintMap.TryGetValue(tc.ConstraintCode, out tcTmp))
				{
					tcTmp = tc;
					bOK = AddConstraint(tcTmp) && bOK;
				}
			}

			return bOK;
		}


		public bool AddConstraint(TriangulationConstraint tc)
		{
			if (tc == null || tc.P == null || tc.Q == null)
				return false;

			if (mConstraintMap.ContainsKey(tc.ConstraintCode))
				return true;

			TriangulationPoint p;
			if (TryGetPoint(tc.P.X, tc.P.Y, out p))
				tc.P = p;
			else
				Add(tc.P);

			if (TryGetPoint(tc.Q.X, tc.Q.Y, out p))
				tc.Q = p;
			else
				Add(tc.Q);

			mConstraintMap.Add(tc.ConstraintCode, tc);

			return true;
		}

		public bool TryGetConstraint(uint constraintCode, out TriangulationConstraint tc)
		{
			return mConstraintMap.TryGetValue(constraintCode, out tc);
		}

		public int GetNumConstraints()
		{
			return mConstraintMap.Count;
		}

		public Dictionary<uint, TriangulationConstraint>.Enumerator GetConstraintEnumerator()
		{
			return mConstraintMap.GetEnumerator();
		}

		public int GetNumHoles()
		{
			int numHoles = 0;
			foreach (Contour c in mHoles)
				numHoles += c.GetNumHoles(false);

			return numHoles;
		}

		public Contour GetHole(int idx)
		{
			if (idx < 0 || idx >= mHoles.Count)
				return null;

			return mHoles[idx];
		}

		public int GetActualHoles(out List<Contour> holes)
		{
			holes = new List<Contour>();
			foreach (Contour c in mHoles)
				c.GetActualHoles(false, ref holes);

			return holes.Count;
		}

		protected void InitializeHoles()
		{
			Contour.InitializeHoles(mHoles, this, this);
			foreach (Contour c in mHoles)
				c.InitializeHoles(this);
		}

		public override bool Initialize()
		{
			InitializeHoles();
			return base.Initialize();
		}

		public override void Prepare(TriangulationContext tcx)
		{
			if (!Initialize())
				return;

			base.Prepare(tcx);

			Dictionary<uint, TriangulationConstraint>.Enumerator it = mConstraintMap.GetEnumerator();
			while (it.MoveNext())
			{
				TriangulationConstraint tc = it.Current.Value;
				tcx.NewConstraint(tc.P, tc.Q);
			}
		}

		public override void AddTriangle(DelaunayTriangle t)
		{
			Triangles.Add(t);
		}
	}
}
