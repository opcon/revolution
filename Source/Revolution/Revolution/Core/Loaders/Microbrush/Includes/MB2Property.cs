//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.

namespace Shrinker.Microbrush2
{
	public class Property
	{
		public readonly uint   TypeID ;
		public readonly int    IntV   ;
		public readonly float  FloatV ;
		public readonly string StringV;
		public readonly byte   ColorVR;
		public readonly byte   ColorVG;
		public readonly byte   ColorVB;

		public Property(uint typeID, int i)
		{
			TypeID = typeID;
			IntV = i;
		}

		public Property(uint typeID, float f)
		{
			TypeID = typeID;
			FloatV = f;
		}

		public Property(uint typeID, string s)
		{
			TypeID = typeID;
			StringV = s;
		}

		public Property(uint typeID, byte r, byte g, byte b)
		{
			TypeID = typeID;
			ColorVR = r;
			ColorVG = g;
			ColorVB = b;
		}
	}
}
