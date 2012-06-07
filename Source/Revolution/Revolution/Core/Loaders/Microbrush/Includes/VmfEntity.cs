//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;
using Shrinker.Common;

namespace Shrinker.Vmf
{
	public class Entity
	{
		public readonly string                         ClassName      ;
		public readonly Dictionary  <string, string>   OtherProperties;
		public readonly KeyValuePair<string, string>[] Connections    ;
		public readonly uint                           ID             ;

		public Entity(string className, Dictionary<string, string> otherProperties, KeyValuePair<string, string>[] connections, uint id)
		{
			ClassName       = className      ;
			OtherProperties = otherProperties;
			Connections     = connections    ;
			ID              = id             ;
		}
	}
}
