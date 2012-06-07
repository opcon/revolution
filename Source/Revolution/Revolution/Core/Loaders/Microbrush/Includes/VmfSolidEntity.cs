//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;
using Shrinker.Common;

namespace Shrinker.Vmf
{
	public class SolidEntity: Entity
	{
		public readonly Solid[] Solids        ;
		public readonly vec3    OptionalOrigin;

		public SolidEntity(string className, Dictionary<string, string> otherProperties, KeyValuePair<string, string>[] connections, uint id, Solid[] solids, vec3 optionalOrigin):
			base(className, otherProperties, connections, id)
		{
			Solids         = solids        ;
			OptionalOrigin = optionalOrigin;
		}
	}
}
