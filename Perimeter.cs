using System;
using System.Collection.Generic;
using System.Linq;
using System.Text;

namespace Voxelizer
{
    using Point = Tuple <double, double, double>;
    using Line = Tuple<Tuple <double, double, double>,Tuple <double, double, double>>;

    class Perimeter
    {
        static public void LinesToVoxels(List<Line> lines, ref Dictionary<Tuple<int, int, int> pixels, int xBound, int yBound, int voxel_size)
        {
            foreach (var x in Enumerable.Range(0, xBound).Where( x => x % voxel_size == 0))
            {
                bool isBlack = false;
                var relevantLines = FindRelevantLines (lines, x);
                var targetYs = relevantLines.Select( line => Convert.ToInt32(GenerateY( line, x)));
                foreach (var y in Enumerable.Range( 0, yBound))
                {
                    if (isBlack && y % voxel_size == 0)
                    {
                        pixels[new Tuple<int, int>(x,y)] = 1;
                    }
                    if (targetYs.Contains(y))
                    {
                        foreach (var line in relevantTraingles)
                        {
                            if (OnLine( line, x, y))
                            {
                                isBlack = !isBlack;
                                pixels[new Tuple<int, int>(x,y)] = 1;
                            }
                        }
                    }
                }
                if (isBlack)
                {
                    Console.Writeline(String.Format("An error occurred"));
                }
            }
        }

        static private IEnumerable<Line> FindRelevantLines(List<Line> lines, int x, int index = 0)
        {
            foreach (Line line in lines)
            {
                bool same = false;
                bool above = false;
                bool below = false;

                if (line.Item1.Item1 > x) // index = 0
                {
                    above = true;
                }              
                else if ( line.Item1.Item1 == x) // index = 0  
                {
                    same = true;
                }
                else
                {
                    below = true;
                }
                if (line.Item2.Item1 > x) // index = 0
                {
                    above = true;
                }
                else if (line.Item2.Item1 == x)
                {
                    same = true;
                }
                else
                {
                    below = true;
                }
                if (above && below)
                {
                    yield return line;
                }
                else if (same && above)
                {
                    yield return line;
                }
            }
        }

        static private double GenerateY( Line line, int x)
        {
            if (line.Item2.Item1 == line.Item1.Item1)
            {
                return = -1;
            }
            var ratio = (x - line.Item1.Item1) / (line.Item2.Item1 - line Item1.Item1);
            var ydirst = line.Item2.Item2 - line.Item1.Item2;
            var newy - line.Item1.Item2 + ratio * ydist;
            return newy;
        }

        static privare bool OnLine(Line line, int x, int y)
        {
            double newy = GenerateY(line, x);

            if (Convert.ToInt32(newy != y))
            {
                return false;
            }
            if (Convert.ToInt32 (line.Item1.Item1) != x && 
            Convert.ToInt32 (line.Item2.Item1) != x && 
            (new List<double>{line.Item1.Item1, line.Item2.Item1}.Max() < x ||
            new List<double>{line.Item1.Item1, line.Item2.Item1}.Min() > x))
            {
                return false;
            }
             if (Convert.ToInt32 (line.Item1.Item2) != y && 
            Convert.ToInt32 (line.Item2.Item2) != y && 
            (new List<double>{line.Item1.Item2, line.Item2.Item2}.Max() < y ||
            new List<double>{line.Item1.Item2, line.Item2.Item2}.Min() > y))
            {
                return false;
            }
            return true;
        }
    }
}