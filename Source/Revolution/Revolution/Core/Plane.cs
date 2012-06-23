using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace Revolution.Core
{
    public class Plane
    {
        float[] equation = new float[4];
        public Vector3 Origin { get; set; }
        public Vector3 Normal { get; set; }

        public Plane(Vector3 origin, Vector3 normal)
        {
            Origin = origin;
            Normal = normal;

            InitaliseEquation();
        }

        private void InitaliseEquation()
        {
            Vector3 origin = Origin;
            Vector3 normal = Normal;
            equation[0] = normal.X;
            equation[1] = normal.Y;
            equation[2] = normal.Z;
            equation[3] = (-normal.X*origin.X + normal.Y*origin.Y + normal.Z*origin.Z);
        }

        public Plane(Vector3[] points)
        {
            Normal = Vector3.Cross(points[1] - points[0], points[2] - points[0]);
            Normal.Normalize();
            Origin = points[0];
            
            InitaliseEquation();
        }

        public bool IsFrontFacingTo(Vector3 direction)
        {
            double dot = Vector3.Dot(Normal, direction);
            return (dot <= 0);
        }

        public double SignedDistanceTo(Vector3 point)
        {
            return (Vector3.Dot(point, Normal) + equation[3]);
        }
    }
}
