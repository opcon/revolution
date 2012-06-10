using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Poly2Tri;
using Shrinker.Microbrush2;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Revolution.Core.Loaders.Microbrush
{
    public static class MicrobrushLoader
    {
        public static Scene LoadScene(string file)
        {
            Scene scene = null;
            if (Path.GetExtension(file) == ".dat")
            {
                scene = MB2DatFormat.Load(file);
            }
            else if (Path.GetExtension(file) == ".xml")
            {
                scene = MB2XmlFormat.Load(file);
            }
            return scene;
        }

        public static Vector3[] GetPlanePoints(Plane p)
        {
            var ret = new Vector3[p.OptionalPolygonCorners.Count()];
            
            for (int i = 0; i < p.OptionalPolygonCorners.Count(); i++)
            {
                ret[i] = new Vector3(p.OptionalPolygonCorners[i].X, p.OptionalPolygonCorners[i].Y, p.OptionalPolygonCorners[i].Z);
            }
            return ret;
        }

        public static List<Polygon> GetPolygons(Brush b)
        {
            var ret = new List<Polygon>();
            foreach (var p in b.Planes)
            {
                ret.Add(new Polygon(GetPlanePoints(p)){Normal = Vector3.Cross(new Vector3(p.U.X, p.U.Y, p.U.Z), new Vector3(p.V.X, p.V.Y, p.V.Z))});
            }
            return ret;
        }

        public static List<MicrobrushBrush> GetBrushes(Scene s)
        {
            List<MicrobrushBrush> ret = new List<MicrobrushBrush>();
            foreach (var b in s.Brushes)
            {
               ret.Add(new MicrobrushBrush(GetPolygons(b))); 
            }
            return ret;
        }
    }

    public class MicrobrushScene
    {
        public List<MicrobrushBrush> Brushes;

        public MicrobrushScene(List<MicrobrushBrush> brushes)
        {
            Brushes = brushes;
        }
        public void Draw(double time)
        {
            foreach (var brush in Brushes)
            {
                brush.Draw(time);
            }
        }

        public void Triangulate()
        {
            for (int index = 0; index < Brushes.Count; index++)
            {
                var brush = Brushes[index];
                var TriangulatedPolygons = new List<Polygon>();
                foreach (var p in brush.Polygons)
                {
                    if (p.Points.Count() == 3)
                    {
                        TriangulatedPolygons.Add(p);
                        continue;
                    }

                    List<Poly2Tri.PolygonPoint> points = new List<PolygonPoint>();

                    var a = p.Points[1] - p.Points[0];
                    //var b = p.Points[2] - p.Points[0];
                    //var normal = Vector3.Cross(a, b);
                    var normal = Vector3.Cross(p.Points[1] - p.Points[0], p.Points[2] - p.Points[0]);
                    normal.Normalize();
                    a.Normalize();
                    var dot = Vector3.Dot(normal, Vector3.UnitZ);
                    if (Math.Abs(Math.Abs(dot) - 1) > 0.0001)
                    {
                        var angle = Math.Acos(dot);
                        var xang = Math.Acos(Vector3.Dot(Vector3.UnitX, normal));
                        var yang = Math.Acos(Vector3.Dot(Vector3.UnitY, normal));
                        var zang = Math.Acos(Vector3.Dot(Vector3.UnitZ, normal));
                        var xrot = Matrix4.CreateFromAxisAngle(Vector3.UnitX, (float) xang);
                        var yrot = Matrix4.CreateFromAxisAngle(Vector3.UnitY, (float) yang);
                        var zrot = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, (float) zang);

                        //var src = new Matrix4(new Vector4(a), new Vector4(up), new Vector4(Vector3.Dot(a, up)), Vector4.Zero);
                        //var dest = new Matrix4(Vector4.UnitX, Vector4.UnitY,
                        //                       new Vector4(Vector3.Cross(Vector3.UnitX, Vector3.UnitY)), Vector4.Zero);
                        //src.Invert();
                        //var rot = Matrix4.CreateFromAxisAngle(Vector3.Cross(a, normal), (float) angle);
                        var rot = xrot*yrot;
                        rot = Matrix4.CreateFromAxisAngle(Vector3.Cross(normal, Vector3.UnitZ), (float) angle);
                        //rot = Matrix4.Identity;
                        var newPoints = new List<Vector3>();
                        float yval = 0, xval = 0, zval = 0;
                        foreach (var point in p.Points)
                        {
                            //v = Vector4.Transform(v, projection);
                            //var v = Vector3.Transform(point, xrot);
                            //v = Vector3.Transform(v, yrot);
                            var v = Vector3.Transform(point, rot);
                            //points.Add(new PolygonPoint(v.X, v.Y));
                            //newPoints.Add(v);
                            yval = v.Z;
                            points.Add(new PolygonPoint(v.X, v.Y));
                        }
                        //p.Points = newPoints.ToArray();

                        var invRot = Matrix4.Invert(rot);

                        //foreach (var point in p.Points)
                        //{
                        //    points.Add(new PolygonPoint(point.X, point.Z));
                        //}

                        var poly = new Poly2Tri.Polygon(points);
                        P2T.Triangulate(poly);
                        foreach (var tri in poly.Triangles)
                        {
                            Vector3[] triPoints = new Vector3[3];
                            for (int i = 0; i < 3; i++)
                            {
                                var triPoint = new Vector3();
                                triPoint.X = (float) tri.Points[i].X;
                                triPoint.Z = yval;
                                triPoint.Y = (float) tri.Points[i].Y;
                                triPoints[i] = Vector3.Transform(triPoint, invRot);
                            }
                            TriangulatedPolygons.Add(new Polygon(triPoints));
                        }
                        //p.Points = newPoints.ToArray();
                    }
                    else
                    {
                        var zval = p.Points[0].Z;
                        foreach (var point in p.Points)
                        {
                            //v = Vector4.Transform(v, projection);
                            //var v = Vector3.Transform(point, xrot);
                            //v = Vector3.Transform(v, yrot);
                            //points.Add(new PolygonPoint(v.X, v.Y));
                            //newPoints.Add(v);
                            points.Add(new PolygonPoint(point.X, point.Y));
                        }
                        //p.Points = newPoints.ToArray();


                        //foreach (var point in p.Points)
                        //{
                        //    points.Add(new PolygonPoint(point.X, point.Z));
                        //}

                        var poly = new Poly2Tri.Polygon(points);
                        P2T.Triangulate(poly);
                        foreach (var tri in poly.Triangles)
                        {
                            Vector3[] triPoints = new Vector3[3];
                            for (int i = 0; i < 3; i++)
                            {
                                var triPoint = new Vector3();
                                triPoint.X = (float) tri.Points[i].X;
                                triPoint.Z = zval;
                                triPoint.Y = (float) tri.Points[i].Y;
                                triPoints[i] = triPoint;
                            }
                            TriangulatedPolygons.Add(new Polygon(triPoints){Normal = p.Normal});
                        }
                    }

                }
                Brushes[index].Polygons = new List<Polygon>(TriangulatedPolygons);
            }
        }
    }

    public class MicrobrushBrush
    {
        public List<Polygon> Polygons;

        public MicrobrushBrush(List<Polygon> polygons)
        {
            Polygons = polygons;
        }

        public void Draw(double time)
        {
            foreach (var p in Polygons)
            {
                p.Draw(time);
            }
        }
    } 
}

