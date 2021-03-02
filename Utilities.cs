using System;
using System.Collection.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Voxelizer
{
    using BoundingDimensions = Tuple<int, int, int>;
    using Point = Tuple <double, double, double>;
    using Traingle = Tuple<Tuple <double, double, double>,Tuple <double, double, double>,Tuple <double, double, double>>;
    using Line = Tuple<Tuple <double, double, double>,Tuple <double, double, double>>;

    class Utilities 
    {
        static public List<Line> ToIntersectingLines(List<Triangle> mesh, double height)
        {
            var relevantTraingles = mesh.Select(x => new List<Point { x.Item1, x.Item2, x.Item3}).Where( x=> IsAboveAndBelow(x, height));
            var notSameTraingles = relevantTraingles.Where( x => (!ToIntersectingTriangle(x, height)));
            return lines;
        }

        static private Line TraingleToIntersectingLined(List<Point> triangle, double height)
        {
            List<Point> above = triangle.Where( x => x.Item3 > height).Select( x => x).ToList();
            List<Point> below = triangle.Where( x => x.Item3 < height).Select( x => x).ToList();
            List<Point> same = triangle.Where( x => x.Item3 == height).Select( x => x).ToList();

            if (same.Count == 2)
            {
                return new Tuple<Point, Point>(same[0], same[1]);
            }
            else if (same.Count == 1)
            {
                Point side1 = WhereLineCrossesZ(above[0], below[0], height);
                return new Tuple<Point, Point>(side1, same[0]);
            }
            else
            {
                List<Line> lines = new List<Line>();
                foreach (var a in above)
                {
                    foreach (var b in above)
                    {
                        Line line = new Line(b,a);
                        lines.Add(line);
                    }
                }
                Point side1 = WhereLineCrossesZ(lines[0].Item1, lines[0].Item2, height);
                Point side2 = WhereLineCrossesZ(lines[1].Item1, lines[1].Item2, height);
                return new Tuple<Point, Point>(side1, side2);
            }
        }

        static private Point WhereLineCrossesZ(Point pt1, Point pt2, double height)
        {
            if (pt1.Item3 > pt2.Item3)
            {
                Point temp = pt1;
                pt1 = pt2;
                pt2 = temp;
            }
            // Now p1 is below p2 in z
            if (pt2.Item3 == pt1.Item3)
            {
                distance = 0;
            }
            else
            {
                distance = (height - pt.Item3)/ (pt2.Item3 - pt1.Item3);
            }
            return DoLinearInterpolation(pt1, pt2, distance );
        }

        static private Point DoLinearInterpolation(Point pt1, Point pt2, double distance)
        {
            double slopex = (pt.Item1 - pt2.Item1);
            double slopey = (pt.Item2 - pt2.Item2);
            double slopez = (pt.Item3 - pt2.Item3);
            return new Point( pt.Item1 - distance * slopex, pt.Item2 - distance * slopey, pt.Item3 - distance * slopez);
        }

        static private bool IsAboveAndBelow(List<Point> points, double height)
        {
            List<Point> above = points.Where( x => x.Item3 > height).Select( x => x).ToList();
            List<Point> below = points.Where( x => x.Item3 < height).Select( x => x).ToList();
            List<Point> same = points.Where( x => x.Item3 == height).Select( x => x).ToList();

            if (same.Count == 3 || same.Count == 2)
            {
                return true;
            }
            else if ( above.Count != 0 && below.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool ToIntersectingTriangle(List<Point> triangle,  double height)
        {
            List<Point> same = triangle.Where( x => x.Item3 == height).Select( x => x).ToList();
            return same.Count == 3;
        }

        static public List<Point> PadVoxelArray(double [,,] voxels)
        {
            List<Point> points = new List<Point>();

            foreach (var x in Enumerable.Range(0, voxels.GetUpperBound(0) - voxels.GetLowerBound(0)))
            {
                foreach (var y in Enumerable.Range(0, voxels.GetUpperBound(1) - voxels.GetLowerBound(1)))
                {
                    foreach (var z in Enumerable.Range(0, voxels.GetUpperBound(2) - voxels.GetLowerBound(2)))
                    {
                        //Point point = new Point)voxel[x,y];
                        //points.Add(point);
                    }
                }
            }
            return points;
        }

        static public Tuple<double, Point, BoundingDimensions> CalculateScaleandShift(List<Triangle> mesh , int resolution)
        {
            List<Point> allPoints = mesh.SelecMany(x => new List<Point> { x.Item1, x.Item2, x.Item3}).ToList();

            Point max = new Point(allPoints.Select( x => x.Item1).Max(), allPoints.Select( x => x.Item2).Max(),allPoints.Select( x => x.Item3).Max());
            Point min = new Point(allPoints.Select( x => x.Item1).Min(), allPoints.Select( x => x.Item2).Min(),allPoints.Select( x => x.Item3).Min());
       
            double xyscale = (resolution - 1) / new List<double> { max.Item1 - min.Item1, max.Item2 - min.Item2}.Max();

            Point shift = new Point( -min.Item1, -min.Item2, -min.Item3);

            Tuple<int, int, int> bounding2DBox = new Tuple<int, int, int>(resolution, resolution, Convert.ToInt32((max.Item3 - min.Item3)* xyscale));

            return new Tuple<double, Point, BoundingDimensions> (xyscale, shift, bounding2DBox);
        }

        static public IEnumerable<Triangle> ScaleAndShiftMesh(List<Triangle> mesh , double scale, Point shift)
        {

            foreach (var triangle in mesh)
            {
                // First vertex of traingle 
                double newX1 = (triangle.Item1.Item1 + shift.Item1) * scale;
                double newY1 = (triangle.Item1.Item2 + shift.Item2) * scale;
                double newZ1 = (triangle.Item1.Item3 + shift.Item3) * scale;

                Point newPoint1 = new Point( newX1, newY1, newZ1);

                // Second vertex of traingle 
                double newX2 = (triangle.Item2.Item1 + shift.Item1) * scale;
                double newY2 = (triangle.Item2.Item2 + shift.Item2) * scale;
                double newZ2 = (triangle.Item2.Item3 + shift.Item3) * scale;

                Point newPoint2 = new Point( newX2, newY2, newZ2);

                // Third vertex of traingle 
                double newX3 = (triangle.Item2.Item1 + shift.Item1) * scale;
                double newY3 = (triangle.Item2.Item2 + shift.Item2) * scale;
                double newZ3 = (triangle.Item2.Item3 + shift.Item3) * scale;

                Point newPoint3 = new Point( newX3, newY3, newZ3);

                Triangle newTriangle = new Triangle(newPoint1, newPoint2, newPoint3);

                //TODO : Remove duplicated fro the triangle

                //Yield the new triangle
                yield return newTriangle;
            }
        }

        static public List<Point> UnScaleAndTransformShiftMesh(List<Point> voxels, double scale, Point shift)
        {
            List<Point> points = new List<Point>();

            foreach (var point in voxels)
            {
                double newX1 = (point.Item1 / scale) - shift.Item1;
                double newY1 = (point.Item2 / scale) - shift.Item2;
                double newZ1 = (point.Item3 / scale) - shift.Item3;

                Point newPt = new Point(newX1, newY1, newZ1);

                points.Add(newPt);
            }
            return points;
        }
    }
}