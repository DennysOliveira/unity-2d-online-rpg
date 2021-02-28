using System;
using System.Collections.Generic;
using System.Text;


namespace Polygon2DTriangulation
{
	public class TriangulationUtil
	{
		public static bool SmartIncircle(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
		{
			double pdx = pd.X;
			double pdy = pd.Y;
			double adx = pa.X - pdx;
			double ady = pa.Y - pdy;
			double bdx = pb.X - pdx;
			double bdy = pb.Y - pdy;

			double adxbdy = adx * bdy;
			double bdxady = bdx * ady;
			double oabd = adxbdy - bdxady;

			if (oabd <= 0)
				return false;

			double cdx = pc.X - pdx;
			double cdy = pc.Y - pdy;

			double cdxady = cdx * ady;
			double adxcdy = adx * cdy;
			double ocad = cdxady - adxcdy;

			if (ocad <= 0)
				return false;

			double bdxcdy = bdx * cdy;
			double cdxbdy = cdx * bdy;

			double alift = adx * adx + ady * ady;
			double blift = bdx * bdx + bdy * bdy;
			double clift = cdx * cdx + cdy * cdy;

			double det = alift * (bdxcdy - cdxbdy) + blift * ocad + clift * oabd;

			return det > 0;
		}


		public static bool InScanArea(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
		{
			double pdx = pd.X;
			double pdy = pd.Y;
			double adx = pa.X - pdx;
			double ady = pa.Y - pdy;
			double bdx = pb.X - pdx;
			double bdy = pb.Y - pdy;

			double adxbdy = adx * bdy;
			double bdxady = bdx * ady;
			double oabd = adxbdy - bdxady;

			if (oabd <= 0)
				return false;

			double cdx = pc.X - pdx;
			double cdy = pc.Y - pdy;

			double cdxady = cdx * ady;
			double adxcdy = adx * cdy;
			double ocad = cdxady - adxcdy;

			if (ocad <= 0)
				return false;

			return true;
		}

		public static Orientation Orient2d(Point2D pa, Point2D pb, Point2D pc)
		{
			double detleft = (pa.X - pc.X) * (pb.Y - pc.Y);
			double detright = (pa.Y - pc.Y) * (pb.X - pc.X);
			double val = detleft - detright;
			if (val > -MathUtil.EPSILON && val < MathUtil.EPSILON)
				return Orientation.Collinear;
			else if (val > 0)
				return Orientation.CCW;

			return Orientation.CW;
		}

		public static bool PointInBoundingBox(double xmin, double xmax, double ymin, double ymax, Point2D p)
		{
			return (p.X > xmin && p.X < xmax && p.Y > ymin && p.Y < ymax);
		}

		public static bool PointOnLineSegment2D(Point2D lineStart, Point2D lineEnd, Point2D p, double epsilon)
		{
			return TriangulationUtil.PointOnLineSegment2D(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, p.X, p.Y, epsilon);
		}

		public static bool PointOnLineSegment2D(double x1, double y1, double x2, double y2, double x, double y, double epsilon)
		{
			if (MathUtil.IsValueBetween(x, x1, x2, epsilon) && MathUtil.IsValueBetween(y, y1, y2, epsilon))
			{
				if (MathUtil.AreValuesEqual(x2 - x1, 0.0f, epsilon))
					return true;

				double slope = (y2 - y1) / (x2 - x1);
				double yIntercept = -(slope * x1) + y1;

				double t = y - ((slope * x) + yIntercept);

				return MathUtil.AreValuesEqual(t, 0.0f, epsilon);
			}

			return false;
		}

		public static bool RectsIntersect(Rect2D r1, Rect2D r2)
		{
			return  (r1.Right > r2.Left) && (r1.Left < r2.Right) && (r1.Bottom > r2.Top) && (r1.Top < r2.Bottom);
		}

		public static bool LinesIntersect2D(Point2D ptStart0, Point2D ptEnd0, Point2D ptStart1, Point2D ptEnd1, bool firstIsSegment, bool secondIsSegment, bool coincidentEndPointCollisions, ref Point2D pIntersectionPt, double epsilon)
		{
			double d = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptEnd0.Y - ptStart0.Y);
			if (Math.Abs(d) < epsilon)
				return false;

			double d0 = (ptStart1.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptStart1.Y - ptStart0.Y);
			double d1 = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptStart0.Y) - (ptStart1.X - ptStart0.X) * (ptEnd0.Y - ptStart0.Y);
			double kOneOverD = 1 / d;
			double t0 = d0 * kOneOverD;
			double t1 = d1 * kOneOverD;

			if ((!firstIsSegment  || ((t0 >= 0.0) && (t0 <= 1.0))) && (!secondIsSegment || ((t1 >= 0.0) && (t1 <= 1.0))) && (coincidentEndPointCollisions || (!MathUtil.AreValuesEqual(0.0, t0, epsilon) && !MathUtil.AreValuesEqual(0.0, t1, epsilon)))) {
				if (pIntersectionPt != null) {
					pIntersectionPt.X = ptStart0.X + t0 * (ptEnd0.X - ptStart0.X);
					pIntersectionPt.Y = ptStart0.Y + t0 * (ptEnd0.Y - ptStart0.Y);
				}
				return true;
			}
			return false;
		}

		public static bool LinesIntersect2D(    Point2D ptStart0, Point2D ptEnd0, Point2D ptStart1, Point2D ptEnd1, ref Point2D pIntersectionPt, double epsilon)
		{
			return TriangulationUtil.LinesIntersect2D(ptStart0, ptEnd0, ptStart1, ptEnd1, true, true, false, ref pIntersectionPt, epsilon);
		}

		public static double LI2DDotProduct(Point2D v0, Point2D v1)
		{
			return ((v0.X * v1.X) + (v0.Y * v1.Y));
		}

		public static bool RaysIntersect2D( Point2D ptRayOrigin0, Point2D ptRayVector0, Point2D ptRayOrigin1, Point2D ptRayVector1, ref Point2D ptIntersection)
		{
			double kEpsilon = 0.01;

			if (ptIntersection != null) {
				Point2D ptTemp1 = new Point2D(ptRayOrigin1.X - ptRayOrigin0.X, ptRayOrigin1.Y - ptRayOrigin0.Y);
				Point2D ptTemp2 = new Point2D(-ptRayVector1.Y, ptRayVector1.X);

				double fDot1 = TriangulationUtil.LI2DDotProduct(ptRayVector0, ptTemp2);

				if (Math.Abs(fDot1) < kEpsilon)
					return false; 

				double fDot2 = TriangulationUtil.LI2DDotProduct(ptTemp1, ptTemp2);
				double s = fDot2 / fDot1;
				ptIntersection.X = ptRayOrigin0.X + ptRayVector0.X * s;
				ptIntersection.Y = ptRayOrigin0.Y + ptRayVector0.Y * s;
				return true;
			}

			double delta = ptRayVector1.X - ptRayVector0.X;
			if (Math.Abs(delta) > kEpsilon)
			{
				delta = ptRayVector1.Y - ptRayVector0.Y;
				if (Math.Abs(delta) > kEpsilon)
					return true; 
			}

			return false;
		}
	}

	public class PolygonGenerator
	{
		static readonly Random RNG = new Random();
		private static double PI_2 = 2.0 * Math.PI;

		public static Polygon RandomCircleSweep(double scale, int vertexCount)
		{
			PolygonPoint point;
			PolygonPoint[] points;
			double radius = scale / 4;

			points = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++) {
				do {
					if (i % 250 == 0)
						radius += scale / 2 * (0.5 - RNG.NextDouble());
					else if (i % 50 == 0)
						radius += scale / 5 * (0.5 - RNG.NextDouble());
					else
						radius += 25 * scale / vertexCount * (0.5 - RNG.NextDouble());
					radius = radius > scale / 2 ? scale / 2 : radius;
					radius = radius < scale / 10 ? scale / 10 : radius;
				} while (radius < scale / 10 || radius > scale / 2);
				point = new PolygonPoint(radius * Math.Cos((PI_2 * i) / vertexCount), radius * Math.Sin((PI_2 * i) / vertexCount));
				points[i] = point;
			}
			return new Polygon(points);
		}

		public static Polygon RandomCircleSweep2(double scale, int vertexCount)
		{
			PolygonPoint point;
			PolygonPoint[] points;
			double radius = scale / 4;

			points = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++) {
				do {
					radius += scale / 5 * (0.5 - RNG.NextDouble());
					radius = radius > scale / 2 ? scale / 2 : radius;
					radius = radius < scale / 10 ? scale / 10 : radius;
				} while (radius < scale / 10 || radius > scale / 2);
				point = new PolygonPoint(radius * Math.Cos((PI_2 * i) / vertexCount), radius * Math.Sin((PI_2 * i) / vertexCount));
				points[i] = point;
			}
			return new Polygon(points);
		}
	}

	public class PointGenerator
	{
		static readonly Random RNG = new Random();

		public static List<TriangulationPoint> UniformDistribution(int n, double scale)
		{
			List<TriangulationPoint> points = new List<TriangulationPoint>();
			for (int i = 0; i < n; i++)
				points.Add(new TriangulationPoint(scale * (0.5 - RNG.NextDouble()), scale * (0.5 - RNG.NextDouble())));

			return points;
		}

		public static List<TriangulationPoint> UniformGrid(int n, double scale)
		{
			double x = 0;
			double size = scale / n;
			double halfScale = 0.5 * scale;

			List<TriangulationPoint> points = new List<TriangulationPoint>();
			for (int i = 0; i < n + 1; i++)
			{
				x = halfScale - i * size;
				for (int j = 0; j < n + 1; j++)
					points.Add(new TriangulationPoint(x, halfScale - j * size));
			}

			return points;
		}
	}
}
