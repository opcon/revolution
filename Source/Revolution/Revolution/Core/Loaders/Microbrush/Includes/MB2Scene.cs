//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.

namespace Shrinker.Microbrush2
{
	public class Scene
	{
		public readonly PropertyType[] PropertyTypes;
		public readonly Brush       [] Brushes      ;

		public Scene(
			PropertyType[] propertyTypes,
			Brush       [] brushes      )
		{
			PropertyTypes = propertyTypes;
			Brushes       = brushes      ;
		}
	}
}
