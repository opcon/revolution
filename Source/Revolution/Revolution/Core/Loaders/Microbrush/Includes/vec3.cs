//Copyright © 2008-2010 Max 'Shrinker' Wieden
//Refer to license.txt for license details.
using System;

namespace Shrinker.Common
{
	public class vec3
	{
		public float
			X,
			Y,
			Z;

		public vec3()
		{
		}

		public vec3(vec3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public vec3(
			float x,
			float y,
			float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static vec3 operator-(vec3 v)
		{
			return new vec3(
				-v.X,
				-v.Y,
				-v.Z);
		}

		public static vec3 operator+(vec3 a, vec3 b)
		{
			return new vec3(
				a.X+b.X,
				a.Y+b.Y,
				a.Z+b.Z);
		}

		public static vec3 operator-(vec3 a, vec3 b)
		{
			return new vec3(
				a.X-b.X,
				a.Y-b.Y,
				a.Z-b.Z);
		}

		//dot product
		public static float operator*(vec3 a, vec3 b)
		{
			return
				a.X*b.X+
				a.Y*b.Y+
				a.Z*b.Z;
		}

		public static vec3 operator*(vec3 v, float f)
		{
			return new vec3(
				v.X*f,
				v.Y*f,
				v.Z*f);
		}

		public static vec3 operator/(vec3 v, float f)
		{
			return new vec3(
				v.X/f,
				v.Y/f,
				v.Z/f);
		}

		public static bool operator==(vec3 a, vec3 b)
		{
			return 
				a.X == b.X &&
				a.Y == b.Y &&
				a.Z == b.Z;
		}

		public static bool operator!=(vec3 a, vec3 b)
		{
			return 
				a.X != b.X ||
				a.Y != b.Y ||
				a.Z != b.Z;
		}

		public vec3 CrossP(vec3 v)
		{
			return new vec3(
				Y*v.Z-Z*v.Y,
				Z*v.X-X*v.Z,
				X*v.Y-Y*v.X);
		}

		public float Length()
		{
			return (float)Math.Sqrt(
				X*X+
				Y*Y+
				Z*Z);
		}

		//faster, use for comparisons
		public float SquaredLength()
		{
			return
				X*X+
				Y*Y+
				Z*Z;
		}

		public void Rotate(float h, float p, float b)
		{
			RotateB(b);
			RotateP(p);
			RotateH(h);
		}

		public void RotateH(float h)
		{
			Common.RotateBaseDeg(ref Z, ref X, h);
		}

		public void RotateP(float p)
		{
			Common.RotateBaseDeg(ref Y, ref Z, p);
		}

		public void RotateB(float b)
		{
			Common.RotateBaseDeg(ref X, ref Y, b);
		}

		public void RotateOrtho(float h, float p, float b)
		{
			RotateBOrtho(b);
			RotatePOrtho(p);
			RotateHOrtho(h);
		}

		public void RotateHOrtho(float h)
		{
			Common.RotateBaseDegOrtho(ref Z, ref X, h);
		}

		public void RotatePOrtho(float p)
		{
			Common.RotateBaseDegOrtho(ref Y, ref Z, p);
		}

		public void RotateBOrtho(float b)
		{
			Common.RotateBaseDegOrtho(ref X, ref Y, b);
		}

		public float Heading()
		{
			return (float)(Math.Atan2(-X, -Z)/Math.PI)*180;
		}

		public float Pitch()
		{
			return (float)(Math.Atan2(Y, Math.Sqrt(X*X+Z*Z))/Math.PI)*180;
		}

		public static vec3 DirectionVector(float h, float p)
		{
			float
				_h = (float)Math.PI/180*h,
				_p = (float)Math.PI/180*p,
				t = (float)Math.Cos(_p);
			return new vec3(
				(-(float)Math.Sin(_h)*t),
				( (float)Math.Sin(_p)  ),
				(-(float)Math.Cos(_h)*t));
		}

		public static vec3 operator*(float f, vec3 v)
		{
			return new vec3(
				f*v.X,
				f*v.Y,
				f*v.Z);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;
			return this == (vec3)obj;
		}

		public override int GetHashCode()
		{
			return (int)(X+Y+Z);
		}
	}
}
