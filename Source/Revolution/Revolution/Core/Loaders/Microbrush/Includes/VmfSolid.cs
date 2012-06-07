//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.

namespace Shrinker.Vmf
{
	public class Solid
	{
		public readonly Side[] Sides;
		public readonly uint ID;

		public Solid(Side[] sides, uint id)
		{
			Sides = sides;
			ID    = id   ;
		}
	}
}
