//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;
using Shrinker.Common;

namespace Shrinker.Vmf
{
	public class PointEntity: Entity
	{
		public readonly vec3 Origin;

		public PointEntity(string className, Dictionary<string, string> otherProperties, KeyValuePair<string, string>[] connections, uint id, vec3 origin):
			base(className, otherProperties, connections, id)
		{
			Origin = origin;
		}
	}
}
