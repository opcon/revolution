//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using Shrinker.Common;

namespace Shrinker.Vmf
{
	public class Displacement
	{
		public readonly vec3   Origin;
		public readonly string Data  ;

		public Displacement(vec3 origin, string data)
		{
			Origin = origin;
			Data   = data  ;
		}
	}
}
