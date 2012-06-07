//Copyright © 2008-2010 Max 'Shrinker' Wieden
//Refer to license.txt for license details.
using System;

namespace Shrinker.Common
{
	public class Common
	{
		private static Random random = new Random();

		public static float PModf(float x, float y)
		{
			return (float)(x-y*Math.Floor(x/y));
		}

		//0 <= result < 1
		public static float FRand()
		{
			return (float)random.NextDouble();
		}

		public static void SRand(int seed)
		{
			random = new Random(seed);
		}

		//x' = cos(a)*x-sin(a)*y
		//y' = sin(a)*x+cos(a)*y
		public static void RotateBase(ref float x, ref float y, float a)
		{
			float t = x;
			x = (float)(Math.Cos(a)*t-System.Math.Sin(a)*y);
			y = (float)(Math.Sin(a)*t+System.Math.Cos(a)*y);
		}

		public static void RotateBaseDeg(ref float x, ref float y, float a)
		{
			RotateBase(ref x, ref y, a*(float)Math.PI/180);
		}

		public static void RotateBaseDegOrtho(ref float x, ref float y, float a)
		{
			//special cases (angle | (x', y')):
			//  0 | ( x,  y)
			// 90 | (-y,  x)
			//180 | (-x, -y)
			//270 | ( y, -x)
			a %= 360;
			if (a == 0)
			{
			}
			else
			if (a == 90)
			{
				float t = x;
				x = -y;
				y =  t;
			}
			else
			if (a == 180)
			{
				x *= -1;
				y *= -1;
			}
			else
			if (a == 270)
			{
				float t = x;
				x =  y;
				y = -t;
			}
			else
				RotateBaseDeg(ref x, ref y, a);
		}
	}
}
