using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Polygon2DTriangulation
{
	public struct FixedBitArray3 : IEnumerable<bool>
	{
		public bool _0, _1, _2;
		public bool this[int index] {
			get {
				switch (index) {
				case 0:
					return _0;
				case 1:
					return _1;
				case 2:
					return _2;
				default:
					throw new IndexOutOfRangeException();
				}
			} set {
				switch (index) {
				case 0:
					_0 = value;
					break;
				case 1:
					_1 = value;
					break;
				case 2:
					_2 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool Contains(bool value)
		{
			for (int i = 0; i < 3; ++i)
				if (this[i] == value)
					return true;
			return false;
		}

		public int IndexOf(bool value)
		{
			for (int i = 0; i < 3; ++i)
				if (this[i] == value)
					return i;
			return -1;
		}

		public void Clear()
		{
			_0 = _1 = _2 = false;
		}

		public void Clear(bool value)
		{
			for (int i = 0; i < 3; ++i)
				if (this[i] == value)
					this[i] = false;
		}

		private IEnumerable<bool> Enumerate()
		{
			for (int i = 0; i < 3; ++i)
				yield return this[i];
		}

		public IEnumerator<bool> GetEnumerator() { return Enumerate().GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}

	public struct FixedArray3<T> : IEnumerable<T> where T : class
	{
		public T _0, _1, _2;
		public T this[int index] {
			get {
				switch (index) {
				case 0:
					return _0;
				case 1:
					return _1;
				case 2:
					return _2;
				default:
					throw new IndexOutOfRangeException();
				}
			} set {
				switch (index) {
				case 0:
					_0 = value;
					break;
				case 1:
					_1 = value;
					break;
				case 2:
					_2 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool Contains(T value)
		{
			for (int i = 0; i < 3; ++i)
				if (this[i] != null && this[i].Equals(value))
					return true;

			return false;
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < 3; ++i)
				if (this[i] != null && this[i].Equals(value))
					return i;

			return -1;
		}

		public void Clear()
		{
			_0 = _1 = _2 = null;
		}

		public void Clear(T value)
		{
			for (int i = 0; i < 3; ++i)
				if (this [i] != null && this [i].Equals (value))
					this [i] = null;
		}

		private IEnumerable<T> Enumerate()
		{
			for (int i = 0; i < 3; ++i)
				yield return this[i];
		}

		public IEnumerator<T> GetEnumerator() { return Enumerate().GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}

	public class MathUtil
	{
		public static double EPSILON = 1e-12;

		public static bool AreValuesEqual(double val1, double val2)
		{
			return(AreValuesEqual(val1, val2, EPSILON));
		}

		public static bool AreValuesEqual(double val1, double val2, double tolerance)
		{
			return (val1 >= (val2 - tolerance) && val1 <= (val2 + tolerance));
		}

		public static bool IsValueBetween(double val, double min, double max, double tolerance)
		{
			if (min > max) {
				double tmp = min;
				min = max;
				max = tmp;
			}
			return ((val + tolerance) >= min && (val - tolerance) <= max);
		}

		public static double RoundWithPrecision(double f, double precision)
		{
			if (precision < 0.0)
				return f;
			double mul = Math.Pow(10.0, precision);
			double fTemp = Math.Floor(f * mul) / mul;
			return fTemp;
		}

		public static double Clamp(double a, double low, double high)
		{
			return Math.Max(low, Math.Min(a, high));
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

		public static uint Jenkins32Hash(byte[] data, uint nInitialValue)
		{
			foreach (byte b in data)
			{
				nInitialValue += (uint)b;
				nInitialValue += (nInitialValue << 10);
				nInitialValue += (nInitialValue >> 6);
			}

			nInitialValue += (nInitialValue << 3);
			nInitialValue ^= (nInitialValue >> 11);
			nInitialValue += (nInitialValue << 15);

			return nInitialValue;
		}
	}

	public class Point2D : IComparable<Point2D>
	{
		protected double mX = 0.0;
		public virtual double X { get { return mX; } set { mX = value; } }
		protected double mY = 0.0;
		public virtual double Y { get { return mY; } set { mY = value; } }

		public float Xf { get { return (float)X; } }
		public float Yf { get { return (float)Y; } }

		public Point2D()
		{
			mX = 0.0;
			mY = 0.0;
		}

		public Point2D(double x, double y)
		{
			mX = x;
			mY = y;
		}

		public Point2D(Point2D p)
		{
			mX = p.X;
			mY = p.Y;
		}

		public override string ToString()
		{
			return "[" + X.ToString() + "," + Y.ToString() + "]";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(Object obj)
		{
			Point2D p = obj as Point2D;
			if (p != null)
				return Equals(p);
			return base.Equals(obj);
		}


		public bool Equals(Point2D p)
		{
			return Equals(p, 0.0);
		}

		public bool Equals(Point2D p, double epsilon)
		{
			if ((object)p == null || !MathUtil.AreValuesEqual(X, p.X, epsilon) || !MathUtil.AreValuesEqual(Y, p.Y, epsilon))
				return false;
			return true;
		}

		public int CompareTo(Point2D other)
		{
			if (Y < other.Y)
				return -1;
			else if (Y > other.Y)
				return 1;
			else if (X < other.X)
				return -1;
			else if (X > other.X)
				return 1;
			return 0;
		}

		public virtual void Set(double x, double y) { X = x; Y = y; }
		public virtual void Set(Point2D p) { X = p.X; Y = p.Y; }

		public void Add(Point2D p) { X += p.X; Y += p.Y; }
		public void Add(double scalar) { X += scalar; Y += scalar; }
		public void Subtract(Point2D p) { X -= p.X; Y -= p.Y; }
		public void Subtract(double scalar) { X -= scalar; Y -= scalar; }
		public void Multiply(Point2D p) { X *= p.X; Y *= p.Y; }
		public void Multiply(double scalar) { X *= scalar; Y *= scalar; }
		public void Divide(Point2D p) { X /= p.X; Y /= p.Y; }
		public void Divide(double scalar) { X /= scalar; Y /= scalar; }
		public void Negate() { X = -X; Y = -Y; }
		public double Magnitude() { return Math.Sqrt((X * X) + (Y * Y)); }
		public double MagnitudeSquared() { return (X * X) + (Y * Y); }
		public double MagnitudeReciprocal() { return 1.0 / Magnitude(); }
		public void Normalize() { Multiply(MagnitudeReciprocal()); }
		public double Dot(Point2D p) { return (X * p.X) + (Y * p.Y); }
		public double Cross(Point2D p) { return (X * p.Y) - (Y * p.X); }
		public void Clamp(Point2D low, Point2D high) { X = Math.Max(low.X, Math.Min(X, high.X)); Y = Math.Max(low.Y, Math.Min(Y, high.Y)); }
		public void Abs() { X = Math.Abs(X); Y = Math.Abs(Y); }
		public void Reciprocal() { if (X != 0.0 && Y != 0.0) { X = 1.0 / X; Y = 1.0 / Y; } }

		public void Translate(Point2D vector) { Add(vector); }
		public void Translate(double x, double y) { X += x; Y += y; }
		public void Scale(Point2D vector) { Multiply(vector); }
		public void Scale(double scalar) { Multiply(scalar); }
		public void Scale(double x, double y) { X *= x; Y *= y; }

		public void Rotate(double radians)
		{
			double cosr = Math.Cos(radians);
			double sinr = Math.Sin(radians);
			double xold = X;
			double yold = Y;
			X = (xold * cosr) - (yold * sinr);
			Y = (xold * sinr) + (yold * cosr);
		}

		public void RotateDegrees(double degrees)
		{
			double radians = degrees * Math.PI / 180.0;
			Rotate(radians);
		}

		public static double Dot(Point2D lhs, Point2D rhs) { return (lhs.X * rhs.X) + (lhs.Y * rhs.Y); }
		public static double Cross(Point2D lhs, Point2D rhs) { return (lhs.X * rhs.Y) - (lhs.Y * rhs.X); }
		public static Point2D Clamp(Point2D a, Point2D low, Point2D high) { Point2D p = new Point2D(a); p.Clamp(low, high); return p; }
		public static Point2D Min(Point2D a, Point2D b) { Point2D p = new Point2D(); p.X = Math.Min(a.X, b.X); p.Y = Math.Min(a.Y, b.Y); return p; }
		public static Point2D Max(Point2D a, Point2D b) { Point2D p = new Point2D(); p.X = Math.Max(a.X, b.X); p.Y = Math.Max(a.Y, b.Y); return p; }
		public static Point2D Abs(Point2D a) { Point2D p = new Point2D(Math.Abs(a.X), Math.Abs(a.Y)); return p; }
		public static Point2D Reciprocal(Point2D a) { Point2D p = new Point2D(1.0 / a.X, 1.0 / a.Y); return p; }

		public static Point2D Perpendicular(Point2D lhs, double scalar) { Point2D p = new Point2D(lhs.Y * scalar, lhs.X * -scalar); return p; }
		public static Point2D Perpendicular(double scalar, Point2D rhs) { Point2D p = new Point2D(-scalar * rhs.Y, scalar * rhs.X); return p; }

		public static Point2D operator +(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs);  result.Add(rhs);  return result; }
		public static Point2D operator +(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Add(scalar); return result; }
		public static Point2D operator -(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Subtract(rhs); return result; }
		public static Point2D operator -(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Subtract(scalar); return result; }
		public static Point2D operator *(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Multiply(rhs); return result; }
		public static Point2D operator *(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Multiply(scalar); return result; }
		public static Point2D operator *(double scalar, Point2D lhs) { Point2D result = new Point2D(lhs); result.Multiply(scalar); return result; }
		public static Point2D operator /(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Divide(rhs); return result; }
		public static Point2D operator /(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Divide(scalar); return result; }

		public static Point2D operator -(Point2D p) { Point2D tmp = new Point2D(p); tmp.Negate(); return tmp; }

		public static bool operator <(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) == -1) ? true : false; }
		public static bool operator >(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) == 1) ? true : false; }
		public static bool operator <=(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) <= 0) ? true : false; }
		public static bool operator >=(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) >= 0) ? true : false; }
	}

	public class Point2DEnumerator : IEnumerator<Point2D>
	{
		protected IList<Point2D> mPoints;
		protected int position = -1;  

		public Point2DEnumerator(IList<Point2D> points)
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

		public Point2D Current
		{
			get {
				if (position < 0 || position >= mPoints.Count)
					return null;
				return mPoints[position];
			}
		}
	}

	public class Point2DList : IEnumerable<Point2D>, IList<Point2D>
	{
		public static readonly int kMaxPolygonVertices = 100000;
		public static readonly double kLinearSlop = 0.005;
		public static readonly double kAngularSlop = (2.0 / (180.0 * Math.PI));

		public enum WindingOrderType
		{
			CW,
			CCW,
			Unknown,
			Default = CCW,
		}

		[Flags]
		public enum PolygonError : uint
		{
			None = 0,
			NotEnoughVertices           = 1 << 0,
			NotConvex                   = 1 << 1,
			NotSimple                   = 1 << 2,
			AreaTooSmall                = 1 << 3,
			SidesTooCloseToParallel     = 1 << 4,
			TooThin                     = 1 << 5,
			Degenerate                  = 1 << 6,
			Unknown                     = 1 << 30,
		}

		protected List<Point2D> mPoints = new List<Point2D>();
		protected Rect2D mBoundingBox = new Rect2D();
		protected WindingOrderType mWindingOrder = WindingOrderType.Unknown;
		protected double mEpsilon = MathUtil.EPSILON;  

		public Rect2D BoundingBox { get { return mBoundingBox; } }
		public WindingOrderType WindingOrder
		{
			get { return mWindingOrder; }
			set {
				if (mWindingOrder == WindingOrderType.Unknown)
					mWindingOrder = CalculateWindingOrder();

				if (value != mWindingOrder)
				{
					mPoints.Reverse();
					mWindingOrder = value;
				}
			}
		}
		public double Epsilon { get { return mEpsilon; } }
		public Point2D this[int index]
		{
			get { return mPoints[index]; }
			set { mPoints[index] = value; }
		}
		public int Count { get { return mPoints.Count; } }
		public virtual bool IsReadOnly { get { return false; } }

		public Point2DList()
		{
		}

		public Point2DList(int capacity)
		{
			mPoints.Capacity = capacity;
		}

		public Point2DList(IList<Point2D> l)
		{
			AddRange(l.GetEnumerator(), WindingOrderType.Unknown);
		}

		public Point2DList(Point2DList l)
		{
			int numPoints = l.Count;
			for (int i = 0; i < numPoints; ++i)
				mPoints.Add(l[i]);
			mBoundingBox.Set(l.BoundingBox);
			mEpsilon = l.Epsilon;
			mWindingOrder = l.WindingOrder;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < Count; i++)
			{
				builder.Append(this[i].ToString());
				if (i < Count - 1)
					builder.Append(" ");
			}
			return builder.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return mPoints.GetEnumerator();
		}

		IEnumerator<Point2D> IEnumerable<Point2D>.GetEnumerator()
		{
			return new Point2DEnumerator(mPoints);
		}

		public void Clear()
		{
			mPoints.Clear();
			mBoundingBox.Clear();
			mEpsilon = MathUtil.EPSILON;
			mWindingOrder = WindingOrderType.Unknown;
		}

		public int IndexOf(Point2D p)
		{
			return mPoints.IndexOf(p);
		}

		public virtual void Add(Point2D p)
		{
			Add(p, -1, true);
		}

		protected virtual void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
		{
			if (idx < 0)
				mPoints.Add(p);
			else
				mPoints.Insert(idx, p);

			mBoundingBox.AddPoint(p);
			if (bCalcWindingOrderAndEpsilon)
			{
				if (mWindingOrder == WindingOrderType.Unknown)
					mWindingOrder = CalculateWindingOrder();
				mEpsilon = CalculateEpsilon();
			}
		}

		public virtual void AddRange(Point2DList l)
		{
			AddRange(l.mPoints.GetEnumerator(), l.WindingOrder);
		}

		public virtual void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
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
				if (!bAddedFirst)
				{
					bAddedFirst = true;
					mPoints.Add(iter.Current);
				}
				else if (bReverseReadOrder)
				{
					mPoints.Insert(startCount, iter.Current);
				}
				else
				{
					mPoints.Add(iter.Current);
				}
				mBoundingBox.AddPoint(iter.Current);
			}
			if (mWindingOrder == WindingOrderType.Unknown && windingOrder == WindingOrderType.Unknown)
				mWindingOrder = CalculateWindingOrder();

			mEpsilon = CalculateEpsilon();
		}

		public virtual void Insert(int idx, Point2D item)
		{
			Add(item, idx, true);
		}

		public virtual bool Remove(Point2D p)
		{
			if (mPoints.Remove(p))
			{
				CalculateBounds();
				mEpsilon = CalculateEpsilon();
				return true;
			}
			return false;
		}

		public virtual void RemoveAt(int idx)
		{
			if (idx < 0 || idx >= Count)
				return;
			mPoints.RemoveAt(idx);
			CalculateBounds();
			mEpsilon = CalculateEpsilon();
		}

		public virtual void RemoveRange(int idxStart, int count)
		{
			if (idxStart < 0 || idxStart >= Count)
				return;
			if (count == 0)
				return;

			mPoints.RemoveRange(idxStart, count);
			CalculateBounds();
			mEpsilon = CalculateEpsilon();
		}

		public bool Contains(Point2D p)
		{
			return mPoints.Contains(p);
		}

		public void CopyTo(Point2D[] array, int arrayIndex)
		{
			int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
			for (int i = 0; i < numElementsToCopy; ++i)
				array[arrayIndex + i] = mPoints[i];
		}

		public void CalculateBounds()
		{
			mBoundingBox.Clear();
			foreach (Point2D pt in mPoints)
				mBoundingBox.AddPoint(pt);
		}

		public double CalculateEpsilon()
		{
			return Math.Max(Math.Min(mBoundingBox.Width, mBoundingBox.Height) * 0.001f, MathUtil.EPSILON);
		}

		public WindingOrderType CalculateWindingOrder()
		{
			double area = GetSignedArea();
			if (area < 0.0)
				return WindingOrderType.CW;
			else if (area > 0.0)
				return WindingOrderType.CCW;

			return WindingOrderType.Unknown;
		}

		public int NextIndex(int index)
		{
			if (index == Count - 1)
				return 0;
			return index + 1;
		}

		public int PreviousIndex(int index)
		{
			if (index == 0)
				return Count - 1;
			return index - 1;
		}

		public double GetSignedArea()
		{
			double area = 0.0;
			for (int i = 0; i < Count; i++)
			{
				int j = (i + 1) % Count;
				area += this[i].X * this[j].Y;
				area -= this[i].Y * this[j].X;
			}
			area /= 2.0f;

			return area;
		}

		public double GetArea()
		{
			int i;
			double area = 0;

			for (i = 0; i < Count; i++)
			{
				int j = (i + 1) % Count;
				area += this[i].X * this[j].Y;
				area -= this[i].Y * this[j].X;
			}
			area /= 2.0f;
			return (area < 0 ? -area : area);
		}

		public Point2D GetCentroid()
		{
			Point2D c = new Point2D();
			double area = 0.0f;

			const double inv3 = 1.0 / 3.0;
			Point2D pRef = new Point2D();
			for (int i = 0; i < Count; ++i)
			{
				Point2D p1 = pRef;
				Point2D p2 = this[i];
				Point2D p3 = i + 1 < Count ? this[i + 1] : this[0];

				Point2D e1 = p2 - p1;
				Point2D e2 = p3 - p1;

				double D = Point2D.Cross(e1, e2);

				double triangleArea = 0.5f * D;
				area += triangleArea;

				c += triangleArea * inv3 * (p1 + p2 + p3);
			}

			c *= 1.0f / area;
			return c;
		}

		public void Translate(Point2D vector)
		{
			for (int i = 0; i < Count; i++)
				this[i] += vector;
		}

		public void Scale(Point2D value)
		{
			for (int i = 0; i < Count; i++)
				this[i] *= value;
		}

		public void Rotate(double radians)
		{
			double cosr = Math.Cos(radians);
			double sinr = Math.Sin(radians);
			foreach (Point2D p in mPoints)
			{
				double xold = p.X;
				p.X = xold * cosr - p.Y * sinr;
				p.Y = xold * sinr + p.Y * cosr;
			}
		}

		public bool IsDegenerate()
		{
			if (Count < 3)
				return false;

			if (Count < 3)
				return false;

			for (int k = 0; k < Count; ++k)
			{
				int j = PreviousIndex(k);
				if (mPoints[j].Equals(mPoints[k], Epsilon))
					return true;
				int i = PreviousIndex(j);
				Orientation orientation = TriangulationUtil.Orient2d(mPoints[i], mPoints[j], mPoints[k]);
				if (orientation == Orientation.Collinear)
					return true;
			}

			return false;
		}

		public bool IsConvex()
		{
			bool isPositive = false;

			for (int i = 0; i < Count; ++i)
			{
				int lower = (i == 0) ? (Count - 1) : (i - 1);
				int middle = i;
				int upper = (i == Count - 1) ? (0) : (i + 1);

				double dx0 = this[middle].X - this[lower].X;
				double dy0 = this[middle].Y - this[lower].Y;
				double dx1 = this[upper].X - this[middle].X;
				double dy1 = this[upper].Y - this[middle].Y;

				double cross = dx0 * dy1 - dx1 * dy0;

				bool newIsP = (cross >= 0) ? true : false;
				if (i == 0)
					isPositive = newIsP;

				else if (isPositive != newIsP)
					return false;
			}

			return true;
		}

		public bool IsSimple()
		{
			for (int i = 0; i < Count; ++i)
			{
				int iplus = NextIndex(i);
				for (int j = i + 1; j < Count; ++j)
				{
					int jplus = NextIndex(j);
					Point2D temp = null;
					if (TriangulationUtil.LinesIntersect2D(mPoints[i], mPoints[iplus], mPoints[j], mPoints[jplus], ref temp, mEpsilon))
						return false;
				}
			}
			return true;
		}

		public PolygonError CheckPolygon()
		{
			PolygonError error = PolygonError.None;
			if (Count < 3 || Count > Point2DList.kMaxPolygonVertices)
			{
				error |= PolygonError.NotEnoughVertices;
				return error;
			}
			if (IsDegenerate())
				error |= PolygonError.Degenerate;

			if (!IsSimple())
				error |= PolygonError.NotSimple;

			if (GetArea() < MathUtil.EPSILON)
				error |= PolygonError.AreaTooSmall;

			if ((error & PolygonError.NotSimple) != PolygonError.NotSimple)
			{
				bool bReversed = false;
				WindingOrderType expectedWindingOrder = WindingOrderType.CCW;
				WindingOrderType reverseWindingOrder = WindingOrderType.CW;
				if (WindingOrder == reverseWindingOrder)
				{
					WindingOrder = expectedWindingOrder;
					bReversed = true;
				}

				Point2D[] normals = new Point2D[Count];
				Point2DList vertices = new Point2DList(Count);
				for (int i = 0; i < Count; ++i)
				{
					vertices.Add(new Point2D(this[i].X, this[i].Y));
					int i1 = i;
					int i2 = NextIndex(i);
					Point2D edge = new Point2D(this[i2].X - this[i1].X, this[i2].Y - this[i1].Y);
					normals[i] = Point2D.Perpendicular(edge, 1.0);
					normals[i].Normalize();
				}

				for (int i = 0; i < Count; ++i)
				{
					int iminus = PreviousIndex(i);

					double cross = Point2D.Cross(normals[iminus], normals[i]);
					cross = MathUtil.Clamp(cross, -1.0f, 1.0f);
					float angle = (float)Math.Asin(cross);
					if (Math.Abs(angle) <= Point2DList.kAngularSlop)
					{
						error |= PolygonError.SidesTooCloseToParallel;
						break;
					}
				}

				if (bReversed)
					WindingOrder = reverseWindingOrder;
			}

			return error;
		}

		public static string GetErrorString(PolygonError error)
		{
			StringBuilder sb = new StringBuilder(256);
			if (error == PolygonError.None)
				sb.AppendFormat("No errors.\n");
			else
			{
				if ((error & PolygonError.NotEnoughVertices) == PolygonError.NotEnoughVertices)
					sb.AppendFormat("NotEnoughVertices: must have between 3 and {0} vertices.\n", kMaxPolygonVertices);
				if ((error & PolygonError.NotConvex) == PolygonError.NotConvex)
					sb.AppendFormat("NotConvex: Polygon is not convex.\n");
				if ((error & PolygonError.NotSimple) == PolygonError.NotSimple)
					sb.AppendFormat("NotSimple: Polygon is not simple (i.e. it intersects itself).\n");
				if ((error & PolygonError.AreaTooSmall) == PolygonError.AreaTooSmall)
					sb.AppendFormat("AreaTooSmall: Polygon's area is too small.\n");
				if ((error & PolygonError.SidesTooCloseToParallel) == PolygonError.SidesTooCloseToParallel)
					sb.AppendFormat("SidesTooCloseToParallel: Polygon's sides are too close to parallel.\n");
				if ((error & PolygonError.TooThin) == PolygonError.TooThin)
					sb.AppendFormat("TooThin: Polygon is too thin or core shape generation would move edge past centroid.\n");
				if ((error & PolygonError.Degenerate) == PolygonError.Degenerate)
					sb.AppendFormat("Degenerate: Polygon is degenerate (contains collinear points or duplicate coincident points).\n");
				if ((error & PolygonError.Unknown) == PolygonError.Unknown)
					sb.AppendFormat("Unknown: Unknown Polygon error!.\n");
			}

			return sb.ToString();
		}

		public void RemoveDuplicateNeighborPoints()
		{
			int numPoints = Count;
			int i = numPoints - 1;
			int j = 0;
			while (numPoints > 1 && j < numPoints)
			{
				if(mPoints[i].Equals(mPoints[j]))
				{
					int idxToRemove = Math.Max(i, j);
					mPoints.RemoveAt(idxToRemove);
					--numPoints;
					if (i >= numPoints)
						i = numPoints - 1;
				}
				else
				{
					i = NextIndex(i);
					++j; 
				}
			}
		}

		public void Simplify()
		{
			Simplify(0.0);
		}

		public void Simplify(double bias)
		{
			if (Count < 3)
				return;

			int curr = 0;
			int numVerts = Count;
			double biasSquared = bias * bias;
			while (curr < numVerts && numVerts >= 3)
			{
				int prevId = PreviousIndex(curr);
				int nextId = NextIndex(curr);

				Point2D prev = this[prevId];
				Point2D current = this[curr];
				Point2D next = this[nextId];

				if ((prev - current).MagnitudeSquared() <= biasSquared)
				{
					RemoveAt(curr);
					--numVerts;
					continue;
				}

				Orientation orientation = TriangulationUtil.Orient2d(prev, current, next);
				if (orientation == Orientation.Collinear)
				{
					RemoveAt(curr);
					--numVerts;
					continue;
				}

				++curr;
			}
		}

		public void MergeParallelEdges(double tolerance)
		{
			if (Count <= 3)
				return;

			bool[] mergeMe = new bool[Count];
			int newNVertices = Count;

			for (int i = 0; i < Count; ++i)
			{
				int lower = (i == 0) ? (Count - 1) : (i - 1);
				int middle = i;
				int upper = (i == Count - 1) ? (0) : (i + 1);

				double dx0 = this[middle].X - this[lower].X;
				double dy0 = this[middle].Y - this[lower].Y;
				double dx1 = this[upper].Y - this[middle].X;
				double dy1 = this[upper].Y - this[middle].Y;
				double norm0 = Math.Sqrt(dx0 * dx0 + dy0 * dy0);
				double norm1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);

				if (!(norm0 > 0.0 && norm1 > 0.0) && newNVertices > 3)
				{
					mergeMe[i] = true;
					--newNVertices;
				}

				dx0 /= norm0;
				dy0 /= norm0;
				dx1 /= norm1;
				dy1 /= norm1;
				double cross = dx0 * dy1 - dx1 * dy0;
				double dot = dx0 * dx1 + dy0 * dy1;

				if (Math.Abs(cross) < tolerance && dot > 0 && newNVertices > 3)
				{
					mergeMe[i] = true;
					--newNVertices;
				}
				else
					mergeMe[i] = false;
			}

			if (newNVertices == Count || newNVertices == 0)
				return;

			int currIndex = 0;

			Point2DList oldVertices = new Point2DList(this);
			Clear();

			for (int i = 0; i < oldVertices.Count; ++i)
			{
				if (mergeMe[i] || newNVertices == 0 || currIndex == newNVertices)
					continue;

				if (currIndex >= newNVertices)
					throw new Exception("Point2DList::MergeParallelEdges - currIndex[ " + currIndex.ToString() + "] >= newNVertices[" + newNVertices + "]");

				mPoints.Add(oldVertices[i]);
				mBoundingBox.AddPoint(oldVertices[i]);
				++currIndex;
			}
			mWindingOrder = CalculateWindingOrder();
			mEpsilon = CalculateEpsilon();
		}

		public void ProjectToAxis(Point2D axis, out double min, out double max)
		{
			double dotProduct = Point2D.Dot(axis, this[0]);
			min = dotProduct;
			max = dotProduct;

			for (int i = 0; i < Count; i++)
			{
				dotProduct = Point2D.Dot(this[i], axis);
				if (dotProduct < min)
					min = dotProduct;
				else if (dotProduct > max)
					max = dotProduct;

			}
		}
	}

	public class Rect2D
	{
		private double mMinX; 
		private double mMaxX;
		private double mMinY; 
		private double mMaxY;

		public double MinX { get { return mMinX; } set { mMinX = value; } }
		public double MaxX { get { return mMaxX; } set { mMaxX = value; } }
		public double MinY { get { return mMinY; } set { mMinY = value; } }
		public double MaxY { get { return mMaxY; } set { mMaxY = value; } }
		public double Left { get { return mMinX; } set { mMinX = value; } }
		public double Right { get { return mMaxX; } set { mMaxX = value; } }
		public double Top { get { return mMaxY; } set { mMaxY = value; } }
		public double Bottom { get { return mMinY; } set { mMinY = value; } }

		public double Width { get { return (Right - Left); } }
		public double Height { get { return (Top - Bottom); } }
		public bool Empty { get { return (Left == Right) || (Top == Bottom); } }

		public Rect2D()
		{
			Clear();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(Object obj)
		{
			Rect2D r = obj as Rect2D;
			if( r != null)
				return Equals(r);
			return base.Equals(obj);
		}

		public bool Equals(Rect2D r)
		{
			return Equals(r, MathUtil.EPSILON);
		}

		public bool Equals(Rect2D r, double epsilon)
		{
			if (!MathUtil.AreValuesEqual(MinX, r.MinX, epsilon))
				return false;
			if (!MathUtil.AreValuesEqual(MaxX, r.MaxX))
				return false;
			if (!MathUtil.AreValuesEqual(MinY, r.MinY, epsilon))
				return false;
			if (!MathUtil.AreValuesEqual(MaxY, r.MaxY, epsilon))
				return false;
			return true;
		}

		public void Clear()
		{
			MinX = Double.MaxValue;
			MaxX = Double.MinValue;
			MinY = Double.MaxValue;
			MaxY = Double.MinValue;
		}

		public void Set(double xmin, double xmax, double ymin, double ymax)
		{
			MinX = xmin;
			MaxX = xmax;
			MinY = ymin;
			MaxY = ymax;
			Normalize();
		}

		public void Set(Rect2D b)
		{
			MinX = b.MinX;
			MaxX = b.MaxX;
			MinY = b.MinY;
			MaxY = b.MaxY;
		}

		public void SetSize(double w, double h)
		{
			Right = Left + w;
			Top = Bottom + h;
		}

		public bool Contains(double x, double y)
		{
			return (x > Left) && (y > Bottom) && (x < Right) && (y < Top);
		}

		public bool Contains(Point2D p) { return Contains(p.X, p.Y); }

		public bool Contains(Rect2D r)
		{
			return (Left < r.Left) && (Right > r.Right) && (Top < r.Top) && (Bottom > r.Bottom);
		}

		public bool ContainsInclusive(double x, double y)
		{
			return (x >= Left) && (y >= Top) && (x <= Right) && (y <= Bottom);
		}

		public bool ContainsInclusive(double x, double y, double epsilon)
		{
			return ((x + epsilon) >= Left) && ((y + epsilon) >= Top) && ((x - epsilon) <= Right) && ((y - epsilon) <= Bottom);
		}

		public bool ContainsInclusive(Point2D p) { return ContainsInclusive(p.X, p.Y); }
		public bool ContainsInclusive(Point2D p, double epsilon) { return ContainsInclusive(p.X, p.Y, epsilon); }

		public bool ContainsInclusive(Rect2D r)
		{
			return (Left <= r.Left) && (Right >= r.Right) && (Top <= r.Top) && (Bottom >= r.Bottom);
		}

		public bool ContainsInclusive(Rect2D r, double epsilon)
		{
			return ((Left - epsilon) <= r.Left) && ((Right + epsilon) >= r.Right) && ((Top - epsilon) <= r.Top) && ((Bottom + epsilon) >= r.Bottom);
		}

		public bool Intersects(Rect2D r)
		{
			return  (Right > r.Left) && (Left < r.Right) && (Bottom < r.Top) && (Top > r.Bottom);
		}

		public Point2D GetCenter()
		{
			return new Point2D((Left + Right ) / 2, (Bottom + Top) / 2);
		}

		public bool IsNormalized()
		{
			return (Right >= Left) && (Bottom <= Top);
		}

		public void Normalize()
		{
			if (Left > Right)
				MathUtil.Swap<double>(ref mMinX, ref mMaxX);

			if (Bottom < Top)
				MathUtil.Swap<double>(ref mMinY, ref mMaxY);
		}

		public void AddPoint(Point2D p)
		{
			MinX = Math.Min(MinX, p.X);
			MaxX = Math.Max(MaxX, p.X);
			MinY = Math.Min(MinY, p.Y);
			MaxY = Math.Max(MaxY, p.Y);
		}

		public void Inflate(double w, double h)
		{ 
			Left   -= w; 
			Top    += h; 
			Right  += w; 
			Bottom -= h; 
		}

		public void Inflate(double left, double top, double right, double bottom)
		{ 
			Left   -= left; 
			Top    += top; 
			Right  += right; 
			Bottom -= bottom; 
		}

		public void Offset(double w, double h)
		{
			Left   += w; 
			Top    += h; 
			Right  += w; 
			Bottom += h;
		}

		public void SetPosition(double x, double y)
		{
			double w = Right  - Left;
			double h = Bottom - Top;
			Left   = x; 
			Bottom = y; 
			Right  = x + w; 
			Top    = y + h;
		}

		public bool Intersection(Rect2D r1, Rect2D r2)
		{
			if (!TriangulationUtil.RectsIntersect(r1, r2))
			{
				Left = Right = Top = Bottom = 0.0;
				return false;
			}

			Left   = (r1.Left   > r2.Left)   ? r1.Left   : r2.Left;
			Top    = (r1.Top    < r2.Top )   ? r1.Top    : r2.Top;
			Right  = (r1.Right  < r2.Right)  ? r1.Right  : r2.Right;
			Bottom = (r1.Bottom > r2.Bottom) ? r1.Bottom : r2.Bottom;

			return true;
		}

		public void Union(Rect2D r1, Rect2D r2)
		{
			if ((r2.Right == r2.Left) || (r2.Bottom == r2.Top))
				Set(r1);
			else if ((r1.Right == r1.Left) || (r1.Bottom == r1.Top))
				Set(r2);
			else {
				Left = (r1.Left < r2.Left) ? r1.Left : r2.Left;
				Top = (r1.Top > r2.Top) ? r1.Top : r2.Top;
				Right = (r1.Right > r2.Right) ? r1.Right : r2.Right;
				Bottom = (r1.Bottom < r2.Bottom) ? r1.Bottom : r2.Bottom;
			}
		}
	}
}
