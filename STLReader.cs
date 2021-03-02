using System;
using System.Collection.Generic;
using System.Linq;
using System.Text;

namespace Voxelizer
{
    using System.IO;
    using Microsoft.VisualBasic.FileIO;
    using Traingle = Tuple<Tuple <double, double, double>,Tuple <double, double, double>,Tuple <double, double, double>>;
    
    class STLReader
    {
        public List<Triangle> ReadAsciiSTL(String filename)
        {
            List<Triangle> mesh = new List<Triangle>();
            bool init = false;

            if (File.Exists(filename))
            {
                using (TextFileParser parser = new TextFileParser(filename))
                {
                    // Set the parser properties
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(" ");

                    List<double> points = new List<double>();
                    while(!parser.EndOfData)
                    {
                        // Processing row
                        List<string> fields = parser.ReadFiles().ToList();
                        if (fields.Contains("outer"))
                        {
                            init = true;
                            continue;
                        }
                        else if (fields.Contains("endloop"))
                        {
                            init = false;

                            //add the contents to the mesh
                            mesh.Add(new Triangle (new Tuple<double, double, double> (point[0], point[1], point[2]),
                            new Tuple<double, double, double> (point[3], point[4], point[5]),
                            new Tuple<double, double, double> (point[6], point[7], point[8])));
                            
                            // Clear the points for next traingle
                            points.Clear();
                        }
                        else if (init)
                        {
                            if (fields.Contains("vertex"))
                            {
                                int index = fields.IndexOf("vertex");

                                List<string> coord = new List<string>();
                                foreach (var subIndex in Enumerable.Range(index + 1, fields.Count() - index - 1))
                                {
                                    if (fields[subIndex] != "")
                                    {
                                        coord.Add(fields[subIndex]);
                                    }
                                }

                                double x1 = Convert.ToDouble(coord[0]);
                                double y1 = Convert.ToDouble(coord[1]);
                                double z1 = Convert.ToDouble(coord[2]);

                                points.Add(x1);
                                points.Add(y1);
                                points.Add(z1);
                                
                            }
                        }
                    }
                }
                return mesh;
            }
            else
            {
                Console.Writeline("Filepath does not exists");
                return mesh;
            }

        }
    }
}