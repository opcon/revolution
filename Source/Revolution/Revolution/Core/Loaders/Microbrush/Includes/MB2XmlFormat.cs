//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Xml;
using Shrinker.Common;

namespace Shrinker.Microbrush2
{
	class MB2XmlFormat
	{
		private static string[] XmlFormatVersionIn = new string[]{"2009-08-08", "2009-12-01", "2010-04-29"};
		private const string XmlFormatVersionOut = "2010-04-29";
		private static StreamWriter w;
		private static CultureInfo invariantCulture = CultureInfo.InvariantCulture;

		public static void Save(Scene scene, string path)
		{
			w = null;
			try
			{
				w = new StreamWriter(new BufferedStream(File.Create(path)));
				w.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><mb2Scene version=\""+XmlFormatVersionOut+"\">");
				foreach (PropertyType type in scene.PropertyTypes)
				{
					w.Write("<propertyType name=\"");
					w.Write(type.Name);
					w.Write("\" summary=\"");
					w.Write(type.Summary);
					w.Write("\" type=\"");
					switch (type.Type)
					{
						case PropertyTypeType.Int   : w.Write("int"   ); break;
						case PropertyTypeType.Float : w.Write("float" ); break;
						case PropertyTypeType.String: w.Write("string"); break;
						default: w.Write("color"); break; //PropertyTypeType.Color
					}
					w.Write("\"/>");
				}
				writeBrushes(scene, scene.Brushes);
				w.Write("</mb2Scene>");
			}
			finally
			{
				if (w != null)
					w.Close();
			}
		}

		public static Scene Load(string path)
		{
			XmlDocument doc = XmlUtils.LoadXmlDocument(path, "schema\\mb2Scene.xsd");
			XmlElement mb2ModelElem = (XmlElement)doc.ChildNodes[1];
			string headerVersion = mb2ModelElem.GetAttribute("version");
			bool headerValid = false;
			foreach (string okVersion in XmlFormatVersionIn)
				if (headerVersion.Equals(okVersion))
				{
					headerValid = true;
					break;
				}
			if (!headerValid)
				throw new ApplicationException("This file format version ("+headerVersion+") is not supported.");
			List<PropertyType> propertyTypesArr = new List<PropertyType>();
			List<Brush> brushes = new List<Brush>();
			XmlElement[] propertyTypeElems = XmlUtils.FindAllChildren(mb2ModelElem, "propertyType");
			foreach (XmlElement propertyTypeElem in propertyTypeElems)
			{
				string
					name    = propertyTypeElem.GetAttribute("name"   ),
					summary = propertyTypeElem.GetAttribute("summary"),
					typeStr = propertyTypeElem.GetAttribute("type"   );
				PropertyTypeType type;
				if (typeStr.Equals("int"   )) type = PropertyTypeType.Int   ; else
				if (typeStr.Equals("float" )) type = PropertyTypeType.Float ; else
				if (typeStr.Equals("string")) type = PropertyTypeType.String; else
					type = PropertyTypeType.Color; //typeStr.Equals("color")
				PropertyType propType = new PropertyType(name, summary, type);
				propertyTypesArr.Add(propType);
			}
			PropertyType[] propertyTypes = propertyTypesArr.ToArray();
			return new Scene(propertyTypes, loadBrushes(mb2ModelElem, propertyTypes));
		}

		private static Brush[] loadBrushes(XmlElement elem, PropertyType[] propertyTypes)
		{
			List<Brush> brushes = new List<Brush>();
			XmlElement[] brushElems = XmlUtils.FindAllChildren(elem, "brush");
			foreach (XmlElement brushElem in brushElems)
				brushes.Add(loadBrush(brushElem, propertyTypes));
			return brushes.ToArray();
		}

		private static Brush loadBrush(XmlElement brushElem, PropertyType[] propertyTypes)
		{
			Dictionary<uint, Property> bProperties = new Dictionary<uint, Property>();
			loadProperties(brushElem, bProperties, propertyTypes);
			List<Plane> planes = new List<Plane>();
			XmlElement[] planeElems = XmlUtils.FindAllChildren(brushElem, "plane");
			foreach (XmlElement planeElem in planeElems)
			{
				Dictionary<uint, Property> pProperties = new Dictionary<uint, Property>();
				vec3[] computedPolygonCorners;
				loadProperties(planeElem, pProperties, propertyTypes);
				XmlElement computedPolygonElem = XmlUtils.FindFirstChild(planeElem, "computedPolygon");
				if (computedPolygonElem != null)
				{
					List<vec3> corners = new List<vec3>();
					XmlElement[] cornerElems = XmlUtils.FindAllChildren(computedPolygonElem, "corner");
					foreach (XmlElement cornerElem in cornerElems)
						corners.Add(new vec3(
							float.Parse(((XmlElement)cornerElem).GetAttribute("x"), invariantCulture),
							float.Parse(((XmlElement)cornerElem).GetAttribute("y"), invariantCulture),
							float.Parse(((XmlElement)cornerElem).GetAttribute("z"), invariantCulture)));
					computedPolygonCorners = corners.ToArray();
				}
				else
					computedPolygonCorners = null;
				planes.Add(new Plane(
					new vec3(float.Parse(planeElem.GetAttribute("p0X"), invariantCulture), float.Parse(planeElem.GetAttribute("p0Y"), invariantCulture), float.Parse(planeElem.GetAttribute("p0Z"), invariantCulture)),
					new vec3(float.Parse(planeElem.GetAttribute("uX" ), invariantCulture), float.Parse(planeElem.GetAttribute("uY" ), invariantCulture), float.Parse(planeElem.GetAttribute("uZ" ), invariantCulture)),
					new vec3(float.Parse(planeElem.GetAttribute("vX" ), invariantCulture), float.Parse(planeElem.GetAttribute("vY" ), invariantCulture), float.Parse(planeElem.GetAttribute("vZ" ), invariantCulture)),
					pProperties, computedPolygonCorners));
			}
			Brush[] children;
			XmlElement childrenElem = XmlUtils.FindFirstChild(brushElem, "children");
			if (childrenElem != null)
				children = loadBrushes(childrenElem, propertyTypes);
			else
				children = new Brush[]{};
			return new Brush(bProperties, planes.ToArray(), children);
		}

		private static void loadProperties(XmlElement elem, Dictionary<uint, Property> properties, PropertyType[] propertyTypes)
		{
			XmlElement[] propertyElems = XmlUtils.FindAllChildren(elem, "property");
			foreach (XmlElement propertyElem in propertyElems)
			{
				uint typeID = uint.Parse(propertyElem.GetAttribute("typeID"));
				switch (propertyTypes[(int)typeID].Type)
				{
					case PropertyTypeType.Int   : properties.Add(typeID, new Property(typeID, int  .Parse(propertyElem.GetAttribute("value")))); break;
					case PropertyTypeType.Float : properties.Add(typeID, new Property(typeID, float.Parse(propertyElem.GetAttribute("value"), invariantCulture))); break;
					case PropertyTypeType.String: properties.Add(typeID, new Property(typeID, propertyElem.GetAttribute("value"))); break;
					default: //PropertyTypeType.Color
					{
						ushort colorV = heptToColor(propertyElem.GetAttribute("value"));
						properties.Add(typeID, new Property(typeID,
							(byte)( colorV & 0x001F       ),
							(byte)((colorV & 0x03E0) >>  5),
							(byte)((colorV & 0x7C00) >> 10)));
						break;
					}
				}
			}
		}

		private static string colorToHept(ushort color)
		{
			byte
				r = (byte)( color & 0x001F       ),
				g = (byte)((color & 0x03E0) >>  5),
				b = (byte)((color & 0x7C00) >> 10);
			return ""+
				(char)(r < 10 ? r+'0' : r-10+'A')+
				(char)(g < 10 ? g+'0' : g-10+'A')+
				(char)(b < 10 ? b+'0' : b-10+'A');
		}

		private static ushort heptToColor(string hept)
		{
			return (ushort)(
				((hept[0] <= '9' ? hept[0]-'0' : hept[0]-'A'+10)      ) |
				((hept[1] <= '9' ? hept[1]-'0' : hept[1]-'A'+10) <<  5) |
				((hept[2] <= '9' ? hept[2]-'0' : hept[2]-'A'+10) << 10));
		}

		private static void writeProperties(Scene scene, Dictionary<uint, Property>.ValueCollection properties)
		{
			foreach (Property prop in properties)
			{
				w.Write("<property typeID=\"");
				w.Write(prop.TypeID);
				w.Write("\" value=\"");
				switch (scene.PropertyTypes[prop.TypeID].Type)
				{
					case PropertyTypeType.Int   : w.Write(prop.IntV); break;
					case PropertyTypeType.Float : w.Write(prop.FloatV.ToString(invariantCulture)); break;
					case PropertyTypeType.String: w.Write(SecurityElement.Escape(prop.StringV)); break;
					default: //PropertyTypeType.Color
						w.Write(colorToHept((ushort)(
							prop.ColorVR       |
							prop.ColorVG <<  5 |
							prop.ColorVB << 10)));
						break;
				}
				w.Write("\"/>");
			}
		}

		private static void writeBrushes(Scene scene, Brush[] brushes)
		{
			foreach (Brush brush in brushes)
			{
				w.Write("<brush>");
				writeProperties(scene, brush.Properties.Values);
				Plane[] planes = brush.Planes;
				foreach (Plane plane in planes)
				{
					w.Write("<plane p0X=\"");
					w.Write(plane.P0.X.ToString(invariantCulture));
					w.Write("\" p0Y=\"");
					w.Write(plane.P0.Y.ToString(invariantCulture));
					w.Write("\" p0Z=\"");
					w.Write(plane.P0.Z.ToString(invariantCulture));
					w.Write("\" uX=\"");
					w.Write(plane.U.X.ToString(invariantCulture));
					w.Write("\" uY=\"");
					w.Write(plane.U.Y.ToString(invariantCulture));
					w.Write("\" uZ=\"");
					w.Write(plane.U.Z.ToString(invariantCulture));
					w.Write("\" vX=\"");
					w.Write(plane.V.X.ToString(invariantCulture));
					w.Write("\" vY=\"");
					w.Write(plane.V.Y.ToString(invariantCulture));
					w.Write("\" vZ=\"");
					w.Write(plane.V.Z.ToString(invariantCulture));
					w.Write('\"');
					if (plane.OptionalPolygonCorners == null && plane.Properties.Count == 0)
						w.Write("/>");
					else
					{
						w.Write('>');
						writeProperties(scene, plane.Properties.Values);
						if (plane.OptionalPolygonCorners != null)
						{
							w.Write("<computedPolygon>");
							foreach (vec3 corner in plane.OptionalPolygonCorners)
							{
								w.Write("<corner x=\"");
								w.Write(corner.X.ToString(invariantCulture));
								w.Write("\" y=\"");
								w.Write(corner.Y.ToString(invariantCulture));
								w.Write("\" z=\"");
								w.Write(corner.Z.ToString(invariantCulture));
								w.Write("\"/>");
							}
							w.Write("</computedPolygon>");
						}
						w.Write("</plane>");
					}
				}
				if (brush.Children.Length != 0)
				{
					w.Write("<children>");
					writeBrushes(scene, brush.Children);
					w.Write("</children>");
				}
				w.Write("</brush>");
			}
		}
	}
}
