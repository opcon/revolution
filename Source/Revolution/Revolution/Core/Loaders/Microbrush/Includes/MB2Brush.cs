//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System.Collections.Generic;

namespace Shrinker.Microbrush2
{
	public class Brush
	{
		public readonly Dictionary<uint, Property> Properties;
		public readonly Plane[] Planes  ;
		public readonly Brush[] Children;
		public Brush Parent {get {return parent;}}

		private Brush parent;

		public Brush(Dictionary<uint, Property> properties, Plane[] planes, Brush[] children)
		{
			Properties = properties;
			Planes     = planes    ;
			Children   = children  ;

			parent = null;
			foreach (Brush child in Children)
				child.parent = this;
		}
	}
}
