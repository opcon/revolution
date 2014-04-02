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
using Plane = Shrinker.Microbrush2.Plane;
using BEPUphysics;

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

        public static Vector3[] GetPlanePoints(Shrinker.Microbrush2.Plane p)
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
                ret.Add(new Polygon(GetPlanePoints(p)){Normal = Vector3.Normalize(Vector3.Cross(new Vector3(p.U.X, p.U.Y, p.U.Z), new Vector3(p.V.X, p.V.Y, p.V.Z)))});
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

        public void AddBrushesToPhysicsScene(Space sp)
        {
            foreach (var b in Brushes)
            {
                foreach (var p in b.Polygons)
                {
                    //sp.Add(new BEPUphysics.Entities.Prefabs.Triangle(new BEPUutilities.Vector3(p.Points[0].X, p.Points[0].Y, p.Points[0].Z), new BEPUutilities.Vector3(p.Points[1].X, p.Points[1].Y, p.Points[1].Z)
                    //    ,new BEPUutilities.Vector3(p.Points[2].X, p.Points[2].Y, p.Points[2].Z))); 

					var t = new BEPUphysics.Entities.Prefabs.Triangle(p.Points[0], p.Points[1], p.Points[2])
					{Material = new BEPUphysics.Materials.Material(0.5f, 1f, 1.0f)};
					sp.Add(t);
                }
            }
        }

        public void CheckCollisions(ref CollisionPacket colpacket)
        {
            foreach (var brush in Brushes)
            {
                foreach (var pol in brush.Polygons)
                {
                    bool colFound = false;
                    //Collisions.CheckTriangle(ref colpacket, Vector3.Divide(pol.Points[0], colpacket.ERadius),
                    //                         Vector3.Divide(pol.Points[1], colpacket.ERadius),
                    //                         Vector3.Divide(pol.Points[2], colpacket.ERadius), ref colFound);
                    CollisionTest.CheckTriangle(ref colpacket, Vector3.Divide(pol.Points[0], colpacket.ERadius),
                                             Vector3.Divide(pol.Points[1], colpacket.ERadius),
                                             Vector3.Divide(pol.Points[2], colpacket.ERadius));
                    //pol.IsColliding = colFound;
                    pol.Colour = new Plane(new Vector3[] {Vector3.Divide(pol.Points[0], colpacket.ERadius),
                                          Vector3.Divide(pol.Points[1], colpacket.ERadius),
                                          Vector3.Divide(pol.Points[2], colpacket.ERadius) }).IsFrontFacingTo(
                                          colpacket.NormalisedVelocity) ? Color4.Purple : Color4.White;
                    if (colpacket.FoundCollision) pol.IsColliding = true;
                }
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
                    var normal = Vector3.Cross(p.Points[1] - p.Points[0], p.Points[2] - p.Points[0]);
                    normal.Normalize();

                    if (p.Points.Count() == 3)
                    {
                        TriangulatedPolygons.Add(p);
                        continue;
                    }

                    List<Poly2Tri.PolygonPoint> points = new List<PolygonPoint>();

                    var a = p.Points[1] - p.Points[0];
                    
                    a.Normalize();
                    var dot = Vector3.Dot(normal, Vector3.UnitZ);
                    if (Math.Abs(Math.Abs(dot) - 1) > 0.0001)
                    {
                        var angle = Math.Acos(dot);
                        var xang = Math.Acos(Vector3.Dot(Vector3.UnitX, normal));
                        var yang = Math.Acos(Vector3.Dot(Vector3.UnitY, normal));
                        var xrot = Matrix4.CreateFromAxisAngle(Vector3.UnitX, (float) xang);
                        var yrot = Matrix4.CreateFromAxisAngle(Vector3.UnitY, (float) yang);
                        
                        var rot = xrot*yrot;
                        rot = Matrix4.CreateFromAxisAngle(Vector3.Cross(normal, Vector3.UnitZ), (float) angle);
                        float zval = 0;
                        foreach (var point in p.Points)
                        {
                            var v = Vector3.Transform(point, rot);
                            zval = v.Z;
                            points.Add(new PolygonPoint(v.X, v.Y));
                        }

                        var invRot = Matrix4.Invert(rot);


                        var poly = new Poly2Tri.Polygon(points);
                        P2T.Triangulate(poly);
                        foreach (var tri in poly.Triangles)
                        {
                            Vector3[] triPoints = new Vector3[3];
                            for (int i = 0; i < 3; i++)
                            {
                                var triPoint = new Vector3();
                                triPoint.X = (float) tri.Points[i].X;
                                triPoint.Y = (float) tri.Points[i].Y;
                                triPoint.Z = zval;
                                triPoints[i] = Vector3.Transform(triPoint, invRot);
                            }
                            normal = Vector3.Cross(triPoints[1] - triPoints[0], triPoints[2] - triPoints[0]);
                            if (Vector3.Dot(normal, p.Normal) < 0)
                            {
                                var correctPoints = triPoints.Reverse().ToArray();
                                triPoints = correctPoints;
                            }
                            TriangulatedPolygons.Add(new Polygon(triPoints) { NormalisedNormal = Vector3.Normalize(p.Normal), Normal = p.Normal});
                        }
                    }
                    else
                    {
                        var zval = p.Points[0].Z;
                        foreach (var point in p.Points)
                        {
                            points.Add(new PolygonPoint(point.X, point.Y));
                        }

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
                            normal = Vector3.Cross(triPoints[1] - triPoints[0], triPoints[2] - triPoints[0]);
                            if (Vector3.Dot(normal, p.Normal) < 0)
                            {
                                var correctPoints = triPoints.Reverse().ToArray();
                                triPoints = correctPoints;
                            }
                            TriangulatedPolygons.Add(new Polygon(triPoints){ NormalisedNormal = Vector3.Normalize(p.Normal), Normal = p.Normal});
                        }
                    }

                }
                Brushes[index].Polygons = new List<Polygon>(TriangulatedPolygons);
            }
        }

        public void DrawNormals(double time)
        {
            foreach (var brush in Brushes)
            {
                brush.DrawNormals(time);
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

        public void DrawNormals(double time)
        {
            foreach (var p in Polygons)
            {
                p.DrawNormals(time);
            }
        }
    } 
}

