//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shrinker.Common;

namespace Shrinker.Microbrush2
{
	class MB2DatFormat
	{
		private const string DatFormatStringIn = "MB2SCENE";
		private static readonly uint[] DatFormatVersionIn = new uint[]{2, 20091201, 20100429};

		private const string DatFormatStringOut  = "MB2SCENE";
		private const uint   DatFormatVersionOut =   20100429;

		private static uint fileVersion;
		private static Stream r, w;
		private static byte[] buf;

		private static readonly Encoding utf8 = Encoding.UTF8;

		public static void Save(Scene scene, string path)
		{
			w = null;
			try
			{
				w = new BufferedStream(File.Create(path));
				//Data Layout
				write(utf8.GetBytes(DatFormatStringOut));
				write(BitConverter.GetBytes(DatFormatVersionOut));
				//PropertyTypeSection
				write(BitConverter.GetBytes(scene.PropertyTypes.Length));
				foreach (PropertyType pt in scene.PropertyTypes)
				{
					//PropertyType
					switch (pt.Type)
					{
						case PropertyTypeType.Int   : w.WriteByte(0); break;
						case PropertyTypeType.Float : w.WriteByte(1); break;
						case PropertyTypeType.String: w.WriteByte(2); break;
						default:                      w.WriteByte(3); break; //PropertyTypeType.Color
					}
					write(BitConverter.GetBytes(pt.Name.Length));
					write(utf8.GetBytes(pt.Name));
					write(BitConverter.GetBytes(pt.Summary.Length));
					write(utf8.GetBytes(pt.Summary));
					//////[PropertyRange]
					////if (pt.Type == PropertyTypeType.Int)
					////  if (pt.IntRangeMin != null)
					////  {
					////    w.WriteByte(1);
					////    write(BitConverter.GetBytes((int)pt.IntRangeMin));
					////    write(BitConverter.GetBytes((int)pt.IntRangeMax));
					////  }
					////  else
					////    w.WriteByte(0);
					////else
					////if (pt.Type == PropertyTypeType.Float)
					////  if (pt.FloatRangeMin != null)
					////  {
					////    w.WriteByte(1);
					////    write(BitConverter.GetBytes((float)pt.FloatRangeMin));
					////    write(BitConverter.GetBytes((float)pt.FloatRangeMax));
					////  }
					////  else
					////    w.WriteByte(0);
				}
				//BrushSection
				serializeBrushSection(scene, scene.Brushes);
			}
			finally
			{
				if (w != null)
					w.Close();
			}
		}

		public static Scene Load(string path)
		{
			r = null;
			buf = new byte[]{};
			try
			{
				r = new BufferedStream(File.OpenRead(path));
				//Data Layout
				bool headerValid = false;
				if (utf8.GetString(read(DatFormatStringIn.Length), 0, DatFormatStringIn.Length).Equals(DatFormatStringIn))
				{
					fileVersion = BitConverter.ToUInt32(read(sizeof(uint)), 0);
					foreach (uint okVersion in DatFormatVersionIn)
						if (fileVersion == okVersion)
						{
							headerValid = true;
							break;
						}
				}
				if (!headerValid)
					throw new ApplicationException("This .mb2.dat file format version ("+fileVersion.ToString()+") is not supported, or the header is invalid.");
				//PropertyTypeSection
				List<PropertyType> propertyTypes = new List<PropertyType>();
				uint propertyTypeCount = BitConverter.ToUInt32(read(sizeof(uint)), 0);
				for (uint i = 0; i != propertyTypeCount; i++)
				{
					//PropertyType
					PropertyType pt;
					PropertyTypeType type;
					switch (read(1)[0])
					{
						case  0: type = PropertyTypeType.Int   ; break;
						case  1: type = PropertyTypeType.Float ; break;
						case  2: type = PropertyTypeType.String; break;
						default: type = PropertyTypeType.Color ; break; //3
					}
					int nameLength = BitConverter.ToInt32(read(sizeof(int)), 0);
					string name = utf8.GetString(read(nameLength), 0, nameLength);
					int summaryLength = BitConverter.ToInt32(read(sizeof(int)), 0);
					string summary = utf8.GetString(read(summaryLength), 0, summaryLength);
					pt = new PropertyType(name, summary, type);
					if (fileVersion <= 20091201)
					{
						//[PropertyRange]
						if (type == PropertyTypeType.Int)
						{
							if (read(1)[0] != 0)
							{
								/*pt.IntRangeMin = BitConverter.ToInt32(*/read(4)/*, 0)*/;
								/*pt.IntRangeMax = BitConverter.ToInt32(*/read(4)/*, 0)*/;
							}
						}
						else
						if (type == PropertyTypeType.Float)
						{
							if (read(1)[0] != 0)
							{
								/*pt.FloatRangeMin = BitConverter.ToSingle(*/read(4)/*, 0)*/;
								/*pt.FloatRangeMax = BitConverter.ToSingle(*/read(4)/*, 0)*/;
							}
						}
					}
					propertyTypes.Add(pt);
				}
				//BrushSection
				List<Brush> brushes = new List<Brush>();
				deserializeBrushSection(propertyTypes, brushes);
				return new Scene(propertyTypes.ToArray(), brushes.ToArray());
			}
			finally
			{
				if (r != null)
					r.Close();
				buf = null;
			}
		}

		private static void deserializeBrushSection(List<PropertyType> propertyTypes, List<Brush> brushes)
		{
			//BrushSection
			uint brushCount = BitConverter.ToUInt32(read(sizeof(uint)), 0);
			for (uint i = 0; i != brushCount; i++)
			{
				//Brush
				List<Plane> planes = new List<Plane>();
				uint planeCount = BitConverter.ToUInt32(read(sizeof(uint)), 0);
				for (uint j = 0; j != planeCount; j++)
				{
					//Plane
					Dictionary<uint, Property> properties = new Dictionary<uint, Property>();
					vec3
						p0 = new vec3(BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0)),
						u  = new vec3(BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0)),
						v  = new vec3(BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0), BitConverter.ToSingle(read(sizeof(float)), 0));
					//PropertyData - of plane
					deserializePropertyData(propertyTypes, properties);
					//ComputedPolygonCorners
					bool hasComputedPolygon = read(1)[0] != 0;
					List<vec3> corners = new List<vec3>();
					if (hasComputedPolygon)
					{
						uint cornerCount = BitConverter.ToUInt32(read(sizeof(uint)), 0);
						for (uint k = 0; k != cornerCount; k++)
							corners.Add(new vec3(
								BitConverter.ToSingle(read(sizeof(float)), 0),
								BitConverter.ToSingle(read(sizeof(float)), 0),
								BitConverter.ToSingle(read(sizeof(float)), 0)));
					}
					planes.Add(new Plane(p0, u, v, properties, hasComputedPolygon ? corners.ToArray() : null));
				}
				//PropertyData - of brush
				Dictionary<uint, Property> brushProperties = new Dictionary<uint, Property>();
				if (fileVersion >= 20091201)
					deserializePropertyData(propertyTypes, brushProperties);
				//BrushSection - children
				List<Brush> children = new List<Brush>();
				if (fileVersion >= 20091201)
					deserializeBrushSection(propertyTypes, children);
				brushes.Add(new Brush(brushProperties, planes.ToArray(), children.ToArray()));
			}
		}

		private static void deserializePropertyData(List<PropertyType> propertyTypes, Dictionary<uint, Property> properties)
		{
			//PropertyData
			uint propCount = BitConverter.ToUInt32(read(sizeof(uint)), 0);
			for (uint k = 0; k != propCount; k++)
			{
				//Property
				uint typeID = BitConverter.ToUInt32(read(sizeof(uint)), 0);
				switch (propertyTypes[(int)typeID].Type)
				{
					case PropertyTypeType.Int   : properties.Add(typeID, new Property(typeID, BitConverter.ToInt32 (read(sizeof(int  )), 0))); break;
					case PropertyTypeType.Float : properties.Add(typeID, new Property(typeID, BitConverter.ToSingle(read(sizeof(float)), 0))); break;
					case PropertyTypeType.String: {int len = BitConverter.ToInt32 (read(sizeof(int  )), 0); properties.Add(typeID, new Property(typeID, utf8.GetString(read(len), 0, len))); break;}
					default: //PropertyTypeType.Color
					{
						ushort colorV = BitConverter.ToUInt16(read(sizeof(ushort)), 0);
						properties.Add(typeID, new Property(typeID,
							(byte)( colorV & 0x001F       ),
							(byte)((colorV & 0x03E0) >>  5),
							(byte)((colorV & 0x7C00) >> 10)));
						break;
					}
				}
			}
		}

		private static void serializeBrushSection(Scene scene, Brush[] brushes)
		{
			//BrushSection
			write(BitConverter.GetBytes(brushes.Length));
			foreach (Brush brush in brushes)
			{
				//Brush
				write(BitConverter.GetBytes(brush.Planes.Length));
				foreach (Plane plane in brush.Planes)
				{
					//Plane
					write(BitConverter.GetBytes(plane.P0.X));
					write(BitConverter.GetBytes(plane.P0.Y));
					write(BitConverter.GetBytes(plane.P0.Z));
					write(BitConverter.GetBytes(plane.U .X));
					write(BitConverter.GetBytes(plane.U .Y));
					write(BitConverter.GetBytes(plane.U .Z));
					write(BitConverter.GetBytes(plane.V .X));
					write(BitConverter.GetBytes(plane.V .Y));
					write(BitConverter.GetBytes(plane.V .Z));
					//PropertyData - of plane
					write(BitConverter.GetBytes(plane.Properties.Count));
					serializePropertyData(scene, plane.Properties.Values);
					//ComputedPolygonCorners
					w.WriteByte(0); //computed polygon corners, if present, are ignored
				}
				//PropertyData - of brush
				write(BitConverter.GetBytes(brush.Properties.Count));
				serializePropertyData(scene, brush.Properties.Values);
				//children
				serializeBrushSection(scene, brush.Children);
			}
		}

		private static void serializePropertyData(Scene scene, Dictionary<uint, Property>.ValueCollection properties)
		{
			//PropertyData
			foreach (Property property in properties)
			{
				//Property
				write(BitConverter.GetBytes(property.TypeID));
				switch (scene.PropertyTypes[property.TypeID].Type)
				{
					case PropertyTypeType.Int   : write(BitConverter.GetBytes(property.IntV  )); break;
					case PropertyTypeType.Float : write(BitConverter.GetBytes(property.FloatV)); break;
					case PropertyTypeType.String: write(BitConverter.GetBytes(property.StringV.Length)); write(utf8.GetBytes(property.StringV)); break;
					default: //PropertyTypeType.Color
						write(BitConverter.GetBytes((ushort)(
							property.ColorVR       |
							property.ColorVG <<  5 |
							property.ColorVB << 10)));
						break;
				}
			}
		}

		private static byte[] read(int count)
		{
			if (buf.Length < count)
				buf = new byte[count];
			r.Read(buf, 0, count);
			return buf;
		}

		private static void write(byte[] bytes)
		{
			w.Write(bytes, 0, bytes.Length);
		}
	}
}
