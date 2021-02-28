using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

namespace Polygon2DTriangulation
{
	public enum TriangulationAlgorithm { DTSweep }
	public enum Orientation { CW, CCW, Collinear }
	public enum TriangulationMode { Unconstrained, Constrained, Polygon }

	public interface ITriangulatable
	{
		IList<DelaunayTriangle> Triangles { get; }
		TriangulationMode TriangulationMode { get; }
		string FileName { get; set; }
		bool DisplayFlipX { get; set; }
		bool DisplayFlipY { get; set; }
		float DisplayRotate { get; set; }
		double Precision { get; set; }
		double MinX { get; }
		double MaxX { get; }
		double MinY { get; }
		double MaxY { get; }
		Rect2D Bounds { get; }

		void Prepare(TriangulationContext tcx);
		void AddTriangle(DelaunayTriangle t);
		void AddTriangles(IEnumerable<DelaunayTriangle> list);
		void ClearTriangles();
	}

	public class Edge
	{
		protected Point2D mP = null;
		protected Point2D mQ = null;

		public Point2D EdgeStart { get { return mP; } set { mP= value;} }
		public Point2D EdgeEnd { get { return mQ; } set { mQ = value; } }

		public Edge() { mP = null; mQ = null; }
		public Edge(Point2D edgeStart, Point2D edgeEnd)
		{
			mP = edgeStart;
			mQ = edgeEnd;
		}
	}

	public class TriangulationConstraint : Edge
	{
		private uint mContraintCode = 0;

		public TriangulationPoint P
		{
			get { return mP as TriangulationPoint; } 
			set  {
				if (value != null && mP != value)
				{
					mP = value;
					CalculateContraintCode();
				}
			}
		}

		public TriangulationPoint Q {
			get { return mQ as TriangulationPoint; }
			set {
				if (value != null && mQ != value)
				{
					mQ = value;
					CalculateContraintCode();
				}
			}
		}
		public uint ConstraintCode { get { return mContraintCode; } }

		public TriangulationConstraint(TriangulationPoint p1, TriangulationPoint p2)
		{
			mP = p1;
			mQ = p2;
			if (p1.Y > p2.Y)
			{
				mQ = p1;
				mP = p2;
			} else if (p1.Y == p2.Y) {
				if (p1.X > p2.X)
				{
					mQ = p1;
					mP = p2;
				} else if (p1.X == p2.X) {
				}
			}
			CalculateContraintCode();
		}

		public override string ToString()
		{
			return "[P=" + P.ToString() + ", Q=" + Q.ToString() + " : {" + mContraintCode.ToString() + "}]";
		}

		public void CalculateContraintCode()
		{
			mContraintCode = TriangulationConstraint.CalculateContraintCode(P, Q);
		}


		public static uint CalculateContraintCode(TriangulationPoint p, TriangulationPoint q)
		{
			if (p == null || p == null)
				throw new ArgumentNullException();

			uint constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(p.VertexCode), 0);
			constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(q.VertexCode), constraintCode);

			return constraintCode;
		}

	}
	public abstract class TriangulationContext
	{
		public TriangulationDebugContext DebugContext { get; protected set; }

		public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();
		public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>(200);
		public TriangulationMode TriangulationMode { get; protected set; }
		public ITriangulatable Triangulatable { get; private set; }

		public int StepCount { get; private set; }

		public void Done()
		{
			StepCount++;
		}

		public abstract TriangulationAlgorithm Algorithm { get; }

		public virtual void PrepareTriangulation(ITriangulatable t)
		{
			Triangulatable = t;
			TriangulationMode = t.TriangulationMode;
			t.Prepare(this);
		}

		public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);
		public void Update(string message) { }

		public virtual void Clear()
		{
			Points.Clear();
			if (DebugContext != null)
				DebugContext.Clear();
			StepCount = 0;
		}

		public virtual bool IsDebugEnabled { get; protected set; }
		public DTSweepDebugContext DTDebugContext { get { return DebugContext as DTSweepDebugContext; } }
	}

	public abstract class TriangulationDebugContext
	{
		protected TriangulationContext _tcx;

		public TriangulationDebugContext(TriangulationContext tcx)
		{
			_tcx = tcx;
		}

		public abstract void Clear();
	}

	public class TriangulationPoint : Point2D
	{
		public static readonly double kVertexCodeDefaultPrecision = 3.0;

		public override double X
		{
			get { return mX; }
			set {
				if (value != mX)
				{
					mX = value;
					mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
				}
			}
		}
		public override double Y
		{
			get { return mY; }
			set {
				if (value != mY)
				{
					mY = value;
					mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
				}
			}
		}

		protected uint mVertexCode = 0;
		public uint VertexCode { get { return mVertexCode; } }

		public List<DTSweepConstraint> Edges { get; private set; }
		public bool HasEdges { get { return Edges != null; } }

		public TriangulationPoint(double x, double y) : this(x, y, kVertexCodeDefaultPrecision) {}

		public TriangulationPoint(double x, double y, double precision) : base(x,y)
		{
			mVertexCode = TriangulationPoint.CreateVertexCode(x, y, precision);
		}

		public override string ToString()
		{
			return base.ToString() + ":{" + mVertexCode.ToString() + "}";
		}

		public override int GetHashCode()
		{
			return (int)mVertexCode;
		}

		public override bool Equals(object obj)
		{
			TriangulationPoint p2 = obj as TriangulationPoint;
			if (p2 != null)
				return mVertexCode == p2.VertexCode;
			else
				return base.Equals(obj);
		}

		public override void Set(double x, double y)
		{
			if (x != mX || y != mY)
			{
				mX = x;
				mY = y;
				mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
			}
		}

		public static uint CreateVertexCode(double x, double y, double precision)
		{
			float fx = (float)MathUtil.RoundWithPrecision(x, precision);
			float fy = (float)MathUtil.RoundWithPrecision(y, precision);
			uint vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fx), 0);
			vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fy), vc);
			return vc;
		}

		public void AddEdge(DTSweepConstraint e)
		{
			if (Edges == null)
				Edges = new List<DTSweepConstraint>();
			Edges.Add(e);
		}

		public bool HasEdge(TriangulationPoint p)
		{
			DTSweepConstraint tmp = null;
			return GetEdge(p, out tmp);
		}

		public bool GetEdge(TriangulationPoint p, out DTSweepConstraint edge)
		{
			edge = null;
			if (Edges == null || Edges.Count < 1 || p == null || p.Equals(this))
				return false;

			foreach (DTSweepConstraint sc in Edges)
				if ((sc.P.Equals(this) && sc.Q.Equals(p)) || (sc.P.Equals(p) && sc.Q.Equals(this)))
				{
					edge = sc;
					return true;
				}

			return false;
		}

		public static Point2D ToPoint2D(TriangulationPoint p) { return p as Point2D; }
	}


	public class TriangulationPointEnumerator : IEnumerator<TriangulationPoint>
	{
		protected IList<Point2D> mPoints;
		protected int position = -1; 

		public TriangulationPointEnumerator(IList<Point2D> points)
		{
			mPoints = points;
		}

		public bool MoveNext()
		{
			position++;
			return (position < mPoints.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		void IDisposable.Dispose() { }

		Object IEnumerator.Current { get { return Current; } }

		public TriangulationPoint Current
		{
			get {
				if (position < 0 || position >= mPoints.Count)
					return null;
				return mPoints[position] as TriangulationPoint;
			}
		}
	}

	public class TriangulationPointList : Point2DList {}
}