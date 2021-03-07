using System;
using System.Collection.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using Syste.Collections.Concurrent;

namespace Voxelizer
{
    using BoundingDimensions = Tuple<int, int, int>;
    using Point = Tuple <double, double, double>;
    using Traingle = Tuple<Tuple <double, double, double>,Tuple <double, double, double>,Tuple <double, double, double>>;
    using Line = Tuple<Tuple <double, double, double>,Tuple <double, double, double>>;

    class Voxelify 
    {
        private List<Triangle> mesh = null;
        private int resolution = 1000;
        private int voxel_size = 5;
        private int pixelbrush_thickness = 0.75; // 75% of pixel_size

        public Voxelify(string inputfilename, string outputfilename)
        {
            STLReader reader = new STLReader();

            // Read the STL data
            var mesh = reader.ReadAsciiSTL(inputfilename);

            // Compute the scale factor and shift
            Tuple<double, Point, BoundingDimensions> meta = Utilities.CalculateScaleandShift(mesh, resolution);

            mesh = Utilities.ScaleAndShiftMesh(mesh, meta.Item1, meta.Item2).ToList();

            // 3D box cluster of point bounded within bounding dimensions
            // Each of the dictionary key is X,Y,Z and value represents 0 or 1 (1 being a voxel)
            ConcurrentDictionary<Tuple<int, int, int>, int> vol = new ConcurrentDictionary<Tuple<int, int, int>, int>();

            Stopwatch sw = Stopwatch.StartNew();
            Parallel.ForEach(Enumerable.Range(0, meta.Item3.Item3 - 1).Where(x => x % voxel_size == 0), height =>
            {
                Console.Writeline(String.Format("Processing Layer : {0}", height));
                var lines = Utilities.ToIntersectingLines(mesh, height);

                //Each of the dictionary key is X,Y index (Z is the height)  and value represents 0 or 1 (1 being a pixel)
                Dictionary<Tuple<int, int>, int> prepixel = new Dictionary <Tuple<int, int>, int>();

                //At teh Z plane (Represented by the height) we compute the pixel definition
                Perimeter.LinesToVoxels(lines, ref prepixel, meta.Item3.Item1, meta.Item3.Item2, voxel_size);

                //Add the pixel definition at Z place to the voxel definiton
                foreach( var key in prepixel.Keys)
                {
                    vol.GetOrAdd(new Tuple<int, int, int>(height, key.Item1, key.Item2), prepixel[key]);
                }
            });

            Console.Writeline("Computed Voxels in {0:f2} s", sw.Elapsed.TotalSeconds);

            //Transform the point to actual coordinates
            List<Point> points = new List<Points>();
            foreach (var key in vol.Keys)
            {
                if (vol[key] == 1)
                {
                    Point point = new Point(Convert.ToDouble(key.Item2 + 1),
                    Convert.ToDouble(key.Item3 + 1),
                    Convert.ToDouble(key.Item1 + 1));
                    points.Add(point);
                }
            }
            List<Point> transformedPoints = Utilities.UnScaleAndTransformShiftMesh(points, meta.Item1, meta.Item2);

            ExportXYZ(transformedPoints, outputfilename);
            FileInfo f - new FileInfo(outputfilename);
            ExportPNG(vol, meta.Item3.Item1, meta.Item3.Item2, f.Directory.ToString());
        }

        public void ExportXYZ(List<Point> voxels, string outputfilename)
        {
            using (StreamWriter writer = new StreamWriter(outputfilename))
            {
                foreach( var point in voxels)
                {
                    writer.Writeline("{0} {1} {2}", point.Item1, point.Item2, point.Item3);
                }
            }
        }

        public void ExportPNG(ConcurrentDictionary<Tuple<int, int, int>, int> voxels, int xBound, int yBound, string outputfilepath)
        {
            Bitmap bml = new Bitmap(xBound, yBound);

            Dictionary<int, List<Tuple<int, int>>> bitmappixels = new Dictionary<int, List<Tuple<int, int>>>();

            foreach (var key in voxel.keys)
            {
                if (voxels[key] == 1)
                {
                    if (bitmappixels.ContainsKey(key.Item1))
                    {
                        bitmappixels.Add(key.Item1, new List<Tuple<int, int>> {new Tuple<int, int> (key.Item2, key.Item3)});
                    }
                    else
                    {
                        bitmappixels[key.Item1].Add(new Tuple<int, int>(key.Item2, key.Item3));
                    }
                }    
            }
            foreach (var key in bitmappixels.Keys)
            {
                bmp = new Bitmap(xBound, yBound);
                foreach (var value in bitmappixels[key])
                {
                    Color color = Color.FromArgb(255,255,255);
                    for (int xwidth = value.Item1 - (int)(voxel_size * pixelbrush_thickness); xwidth < value.Item1 + (int)(voxel_size * pixelbrush_thickness); xwidth++)
                        for (int ywidth = value.Item2 - (int)(voxel_size * pixelbrush_thickness); ywidth < value.Item2 + (int)(voxel_size * pixelbrush_thickness); ywidth++)
                            if (xwidth > 0 && xwidth < xBound && ywidth > 0 && ywidth < yBound)
                                bmp.SetPixel(xwidth, ywidth, color);
                }
                bmp.Save(Path.Combine(outputfilepath, key.ToString()+ ".jpg"), ImageFormat.Jpeg);
                bmp.Dispose();  
            }
        }
    }
}