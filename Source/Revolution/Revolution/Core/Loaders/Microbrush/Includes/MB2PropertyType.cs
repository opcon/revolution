//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.

namespace Shrinker.Microbrush2
{
	public class PropertyType
	{
		public readonly PropertyTypeType Type;
		public readonly string Name;
		public readonly string Summary;
		////public int  ? IntRangeMin  , IntRangeMax  ;
		////public float? FloatRangeMin, FloatRangeMax;

		public PropertyType(string name, string summary, PropertyTypeType type)
		{
			Name    = name   ;
			Summary = summary;
			Type    = type   ;
			////IntRangeMin = null;
			////IntRangeMax = null;
			////FloatRangeMin = null;
			////FloatRangeMax = null;
		}
	}
}
