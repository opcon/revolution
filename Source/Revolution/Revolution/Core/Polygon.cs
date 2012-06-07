using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Revolution.Core
{
    public class Polygon
    {
        public Vector3[] Points;

        public Polygon(Vector3[] points)
        {
            Points = points;
        }

        public void Draw(double time)
        {
            foreach (var p in Points)
            {
                GL.Vertex3(p);
            }
        }
    }
}
