using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Revolution.Core
{
    public class Polygon
    {
        public Vector3[] Points;
        public Vector3 Normal = Vector3.Zero;
        public Vector3 NormalisedNormal = Vector3.Zero;
        public bool RandomColour = true;

        public Polygon(Vector3[] points, bool randomColour = true)
        {
            Points = points;
            Visible = true;
            if (RandomColour)
            {
                Colour = Color.FromArgb(Utilities.RandomGenerator.Next(0, 255), Utilities.RandomGenerator.Next(0, 255), Utilities.RandomGenerator.Next(0, 255));
            }
            else
            {
			    Colour = Color4.White;
            }
        }

        public bool IsColliding { get; set; }

        public bool Visible { get; set; }

        public Color4 Colour
        {
            get; set; 
        }

        public void Draw(double time)
        {
            if (!Visible) return;
            if (IsColliding)
                Colour = Color4.Red;
            
            GL.Color3((Color)Colour);
            foreach (var p in Points)
            {
                GL.Vertex3(p);
            }
          
        }

        public void DrawNormals(double time)
        {
            if (!Visible) return;
            GL.Vertex3((Points[0] + Points[1] + Points[2]) / 3);
            GL.Vertex3(NormalisedNormal * 5 + ((Points[0] + Points[1] + Points[2]) / 3));
        }
    }
}
