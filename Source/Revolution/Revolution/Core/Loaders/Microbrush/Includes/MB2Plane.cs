//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;
using Shrinker.Common;

namespace Shrinker.Microbrush2
{
	public class Plane
	{
		public readonly vec3 P0, U, V;
		public readonly Dictionary<uint, Property> Properties;
		///<summary>This may be null.</summary>
		public readonly vec3[] OptionalPolygonCorners;

		public Plane(vec3 p0, vec3 u, vec3 v, Dictionary<uint, Property> properties, vec3[] optionalPolygonCorners)
		{
			P0                     = p0                    ;
			U                      = u                     ;
			V                      = v                     ;
			Properties             = properties            ;
			OptionalPolygonCorners = optionalPolygonCorners;
		}
	}
}
