//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;

namespace Shrinker.Vmf
{
	public class World
	{
		public readonly Solid                     [] Solids         ;
		public readonly Entity                    [] Entities       ;
		public readonly Dictionary<string, string>   OtherProperties;
		public readonly uint                         ID             ;

		public World(Solid[] solids, Entity[] entities, Dictionary<string, string> otherProperties, uint id)
		{
			Solids          = solids         ;
			Entities        = entities       ;
			OtherProperties = otherProperties;
			ID              = id             ;
		}
	}
}
