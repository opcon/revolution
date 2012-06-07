//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;
using Shrinker.Common;

namespace Shrinker.Vmf
{
	public class Side
	{
		public readonly vec3 P0, P1, P2;
		public readonly string Material;
		public readonly uint LightmapScale, SmoothingGroups;
		public readonly Displacement OptionalDisplacement;
		public readonly Dictionary<string, string> OtherProperties;
		public readonly uint ID;

		public Side(vec3 p0, vec3 p1, vec3 p2, string material, uint lightmapScale, uint smoothingGroups, Displacement optionalDisplacement, Dictionary<string, string> otherProperties, uint id)
		{
			P0                   = p0                  ;
			P1                   = p1                  ;
			P2                   = p2                  ;
			Material             = material            ;
			LightmapScale        = lightmapScale       ;
			SmoothingGroups      = smoothingGroups     ;
			OptionalDisplacement = optionalDisplacement;
			OtherProperties      = otherProperties     ;
			ID                   = id                  ;
		}
	}
}
