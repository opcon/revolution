using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Revolution.Core
{
    public class Polygon
    {
        public Vector3[] Points;
        public Vector3 Normal = Vector3.Zero;

        public Polygon(Vector3[] points)
        {
            Points = points;
        }

        public void Draw(double time)
        {
            //if (Points.Count() == 5)
            //{
            //    GL.End();
            //    GL.Begin(BeginMode.Points);
            //    GL.Color3(Color.Red);
            //}
            //if (Points.Count() == 3)
            //{
            //    GL.End();
            //    GL.Begin(BeginMode.Triangles);
            //    GL.Color3(Color.Blue);
            //}
            //if (Points.Count() == 4)
            //{
            //    bool f = true;
            //}
            //GL.Normal3(Vector3.Cross(Points[1] - Points[0], Points[2] - Points[1]));
            GL.Normal3(Normal + Points[0]);
            //GL.End();
            //GL.Disable(EnableCap.Lighting);
            //GL.LineWidth(5);
            //GL.Begin(BeginMode.Lines);
            //GL.Color3(Color.Red);
            //GL.Vertex3(Points[0]);
            //GL.Vertex3(Normal + Points[0]);
            //GL.End();
            //GL.Enable(EnableCap.Lighting);
            //GL.Begin(BeginMode.Triangles);
            foreach (var p in Points)
            {
                GL.Vertex3(p);
            }
            //GL.End();
            //GL.Color3(Color.DarkGreen);
            //GL.LineWidth(5);
            //GL.Begin(BeginMode.Lines);
            //GL.Vertex3(Points[0]);
            //GL.Vertex3(Vector3.Cross(Points[1] - Points[0], Points[2] - Points[0]) + Points[0]);
            //GL.Color3(Color.Red);
            //GL.Vertex3(Points[0]);
            //GL.Vertex3(Vector3.Cross(Vector3.Cross(Points[1] - Points[0], Points[2] - Points[0]), Vector3.UnitZ) + Points[0]);
            //GL.Color3(Color.Black);
            //GL.End();
            //GL.LineWidth(1);
            //GL.Begin(BeginMode.Quads);
        }
    }
}
