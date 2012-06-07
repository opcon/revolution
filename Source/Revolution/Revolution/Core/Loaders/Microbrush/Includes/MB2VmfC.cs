//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System;
using System.Collections.Generic;
using System.Text;
using Shrinker.Common;
using Shrinker.Microbrush2;
using Shrinker.Vmf;

namespace Shrinker.Converters
{
	//Property Types
	//--------------
	//- int    aggregate        - for B (VMFAB), attach ents.
	//- string connections      - for B, entity connections
	//- string displacementData - for B, linked to by d.-Link
	//- int    displacementLink - for B/S, links displacement
	//- int    lightmapScale    - for S, d. 16
	//- string material         - for S, d. TOOLS/TOOLSNODRAW
	//- int    origin           - marks B, att. to solid ent
	//- int    originalEntityID - for B
	//- int    originalID       - for everything
	//- string otherProperties  - for B/S
	//- string pointClassName   - for B, marks point ents.
	//- int    smoothingGroups  - for S, d. 0
	//- string solidClassName   - for B, marks solid ents.
	public class MB2VmfC
	{
		private const float scale = 16;
		private static char[] cQuotArr = new char[]{'\"'};
		private static int
			propAggregate       ,
			propConnections     ,
			propDisplacementData,
			propDisplacementLink,
			propLightmapScale   ,
			propMaterial        ,
			propOrigin          ,
			propOriginalEntityID,
			propOriginalID      ,
			propOtherProperties ,
			propPointClassName  ,
			propSmoothingGroups ,
			propSolidClassName  ;
		private static uint vmfIDSeq, vmfSideIDSeq, vmfAggregateSeq;

		public static World MB2SceneToVmfWorld(Scene scene)
		{
			propAggregate        = -1;
			propConnections      = -1;
			propDisplacementData = -1;
			propDisplacementLink = -1;
			propLightmapScale    = -1;
			propMaterial         = -1;
			propOrigin           = -1;
			propOriginalEntityID = -1;
			propOriginalID       = -1;
			propOtherProperties  = -1;
			propPointClassName   = -1;
			propSmoothingGroups  = -1;
			propSolidClassName   = -1;
			vmfIDSeq        = 0;
			vmfSideIDSeq    = 0;
			vmfAggregateSeq = 0;
			for (int i = 0; i != scene.PropertyTypes.Length; i++)
			{
				string name = scene.PropertyTypes[i].Name;
				if (name.Equals("aggregate"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propAggregate = i;
				}
				else
				if (name.Equals("connections"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propConnections = i;
				}
				else
				if (name.Equals("displacementData"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propDisplacementData = i;
				}
				else
				if (name.Equals("displacementLink"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propDisplacementLink = i;
				}
				else
				if (name.Equals("lightmapScale"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propLightmapScale = i;
				}
				else
				if (name.Equals("material"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propMaterial = i;
				}
				else
				if (name.Equals("origin"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propOrigin = i;
				}
				else
				if (name.Equals("originalEntityID"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propOriginalEntityID = i;
				}
				else
				if (name.Equals("originalID"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propOriginalID = i;
				}
				else
				if (name.Equals("otherProperties"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propOtherProperties = i;
				}
				else
				if (name.Equals("pointClassName"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propPointClassName = i;
				}
				else
				if (name.Equals("smoothingGroups"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.Int)
						propSmoothingGroups = i;
				}
				else
				if (name.Equals("solidClassName"))
				{
					if (scene.PropertyTypes[i].Type == PropertyTypeType.String)
						propSolidClassName = i;
				}
			}
			Dictionary<string, string> otherProperties = new Dictionary<string, string>();
			List<Solid> solids = new List<Solid>();
			List<Entity> entities = new List<Entity>();
			foreach (Brush brush in scene.Brushes)
			{
				processWorldSpawn(brush, otherProperties);
				processSolids(brush, solids);
				processEntities(brush, null, entities);
			}
			return new World(solids.ToArray(), entities.ToArray(), otherProperties, vmfIDSeq++);
		}

		public static Scene VmfWorldToMB2Scene(World world)
		{
			PropertyType[] propertyTypes = new PropertyType[]{
				new PropertyType("aggregate"       , "for B (VMFAB), attach ents.", PropertyTypeType.Int   ),
				new PropertyType("connections"     , "for B, entity connections"  , PropertyTypeType.String),
				new PropertyType("displacementData", "for B, linked to by d.-Link", PropertyTypeType.String),
				new PropertyType("displacementLink", "for B/S, links displacement", PropertyTypeType.Int   ),
				new PropertyType("lightmapScale"   , "for S, d. 16"               , PropertyTypeType.Int   ),
				new PropertyType("material"        , "for S, d. TOOLS/TOOLSNODRAW", PropertyTypeType.String),
				new PropertyType("origin"          , "marks B, att. to solid ent" , PropertyTypeType.Int   ),
				new PropertyType("originalEntityID", "for B"                      , PropertyTypeType.Int   ),
				new PropertyType("originalID"      , "for everything"             , PropertyTypeType.Int   ),
				new PropertyType("otherProperties" , "for B/S"                    , PropertyTypeType.String),
				new PropertyType("pointClassName"  , "for B, marks point ents."   , PropertyTypeType.String),
				new PropertyType("smoothingGroups" , "for S, d. 0"                , PropertyTypeType.Int   ),
				new PropertyType("solidClassName"  , "for B, marks solid ents."   , PropertyTypeType.String)};
			propAggregate        =  0;
			propConnections      =  1;
			propDisplacementData =  2;
			propDisplacementLink =  3;
			propLightmapScale    =  4;
			propMaterial         =  5;
			propOrigin           =  6;
			propOriginalEntityID =  7;
			propOriginalID       =  8;
			propOtherProperties  =  9;
			propPointClassName   = 10;
			propSmoothingGroups  = 11;
			propSolidClassName   = 12;
			List<Brush> brushes = new List<Brush>();
			foreach (Solid solid in world.Solids)
				brushes.Add(vmfSolidToMB2Brush(solid, null, new List<Brush>()));
			foreach (Entity ent in world.Entities)
				if (ent.GetType() == typeof(PointEntity))
				{
					Dictionary<uint, Property> properties = new Dictionary<uint, Property>();
					properties.Add((uint)propOriginalEntityID, new Property((uint)propOriginalEntityID, (int)ent.ID       ));
					properties.Add((uint)propPointClassName  , new Property((uint)propPointClassName  ,      ent.ClassName));
					StringBuilder connections = new StringBuilder();
					if (ent.Connections.Length != 0)
					{
						foreach (KeyValuePair<String, String> entry in ent.Connections)
						{
							connections.Append('\"');
							connections.Append(entry.Key);
							connections.Append("\" \"");
							connections.Append(entry.Value);
							connections.Append("\"\r\n");
						}
						properties.Add((uint)propConnections, new Property((uint)propConnections, connections.ToString()));
					}
					if (ent.OtherProperties.Count != 0)
					{
						StringBuilder otherProps = new StringBuilder();
						foreach (KeyValuePair<string, string> entry in ent.OtherProperties)
						{
							otherProps.Append('\"');
							otherProps.Append(entry.Key);
							otherProps.Append("\" \"");
							otherProps.Append(entry.Value);
							otherProps.Append("\"\r\n");
						}
						properties.Add((uint)propOtherProperties, new Property((uint)propOtherProperties, otherProps.ToString()));
					}
					brushes.Add(makeOriginBox(vmfToMB2(((PointEntity)ent).Origin), properties, new Brush[]{}));
				}
				else //ent.GetType() == typeof(SolidEntity)
				{
					SolidEntity ent2 = (SolidEntity)ent;
					Dictionary<uint, Property> properties = new Dictionary<uint, Property>();
					properties.Add((uint)propOriginalEntityID, new Property((uint)propOriginalEntityID, (int)ent.ID       ));
					properties.Add((uint)propSolidClassName  , new Property((uint)propSolidClassName  ,      ent.ClassName));
					StringBuilder connections = new StringBuilder();
					if (ent.Connections.Length != 0)
					{
						foreach (KeyValuePair<String, String> entry in ent.Connections)
						{
							connections.Append('\"');
							connections.Append(entry.Key);
							connections.Append("\" \"");
							connections.Append(entry.Value);
							connections.Append("\"\r\n");
						}
						properties.Add((uint)propConnections, new Property((uint)propConnections, connections.ToString()));
					}
					if (ent.OtherProperties.Count != 0)
					{
						StringBuilder otherProps = new StringBuilder();
						foreach (KeyValuePair<string, string> entry in ent.OtherProperties)
						{
							otherProps.Append('\"');
							otherProps.Append(entry.Key);
							otherProps.Append("\" \"");
							otherProps.Append(entry.Value);
							otherProps.Append("\"\r\n");
						}
						properties.Add((uint)propOtherProperties, new Property((uint)propOtherProperties, otherProps.ToString()));
					}
					List<Brush> children = new List<Brush>();
					for (uint i = 1; i != ent2.Solids.Length; i++)
						children.Add(vmfSolidToMB2Brush(ent2.Solids[i], null, new List<Brush>()));
					if ((object)ent2.OptionalOrigin != null)
					{
						Dictionary<uint, Property> properties2 = new Dictionary<uint, Property>();
						properties2.Add((uint)propOrigin, new Property((uint)propOrigin, 0));
						children.Add(makeOriginBox(vmfToMB2(ent2.OptionalOrigin), properties2, new Brush[]{}));
					}
					brushes.Add(vmfSolidToMB2Brush(ent2.Solids[0], properties, children));
				}
				Dictionary<uint, Property> propertiesW = new Dictionary<uint, Property>();
				propertiesW.Add((uint)propOriginalID, new Property((uint)propOriginalID, world.ID));
				if (world.OtherProperties.Count != 0)
				{
					StringBuilder otherProps = new StringBuilder();
					foreach (KeyValuePair<string, string> entry in world.OtherProperties)
					{
						otherProps.Append('\"');
						otherProps.Append(entry.Key);
						otherProps.Append("\" \"");
						otherProps.Append(entry.Value);
						otherProps.Append("\"\r\n");
					}
					propertiesW.Add((uint)propOtherProperties, new Property((uint)propOtherProperties, otherProps.ToString()));
				}
				brushes.Add(makeOriginBox(new vec3(0, 0, 0), propertiesW, new Brush[]{}));
				return new Scene(propertyTypes, brushes.ToArray());
			}

		private static void processWorldSpawn(Brush brush, Dictionary<string, string> properties)
		{
			if (propOtherProperties != -1)
			{
				if ((propPointClassName   == -1 || !brush.Properties.ContainsKey((uint)propPointClassName  )) &&
						(propSolidClassName   == -1 || !brush.Properties.ContainsKey((uint)propSolidClassName  )) &&
						(propDisplacementLink == -1 || !brush.Properties.ContainsKey((uint)propDisplacementLink)))
				{
					Property value;
					if (brush.Properties.TryGetValue((uint)propOtherProperties, out value))
						parseProperties(value.StringV, properties);
				}
				foreach (Brush child in brush.Children)
					processWorldSpawn(child, properties);
			}
		}

		private static vec3 processOrigin(Brush brush)
		{
			if (propOrigin != -1)
			{
				if ((propPointClassName   == -1 || !brush.Properties.ContainsKey((uint)propPointClassName  )) &&
						(propSolidClassName   == -1 || !brush.Properties.ContainsKey((uint)propSolidClassName  )) &&
						(propDisplacementLink == -1 || !brush.Properties.ContainsKey((uint)propDisplacementLink)))
				{
					if (brush.Properties.ContainsKey((uint)propOrigin))
						return readOrigin(brush);
				}
				foreach (Brush child in brush.Children)
				{
					vec3 v = processOrigin(child);
					if ((object)v != null)
						return v;
				}
			}
			return null;
		}

		private static void processSolids(Brush brush, List<Solid> solids)
		{
			//we do not go into entities or such
			if ((propOtherProperties  == -1 || !brush.Properties.ContainsKey((uint)propOtherProperties )) &&
					(propOrigin           == -1 || !brush.Properties.ContainsKey((uint)propOrigin          )) &&
					(propPointClassName   == -1 || !brush.Properties.ContainsKey((uint)propPointClassName  )) &&
					(propSolidClassName   == -1 || !brush.Properties.ContainsKey((uint)propSolidClassName  )) &&
					(propDisplacementLink == -1 || !brush.Properties.ContainsKey((uint)propDisplacementLink)))
			{
				if (propAggregate == -1 || !brush.Properties.ContainsKey((uint)propAggregate))
					solids.Add(mb2BrushToVmfSolid(brush));
				foreach (Brush child in brush.Children)
					processSolids(child, solids);
			}
		}

		private static void processEntities(Brush brush, string aggregate, List<Entity> ents)
		{
			if (propAggregate != -1 && brush.Properties.ContainsKey((uint)propAggregate))
			{
				aggregate = "agr"+vmfAggregateSeq;
				vmfAggregateSeq++;
			}
			else
				if (propPointClassName != -1 && brush.Properties.ContainsKey((uint)propPointClassName))
				{
					string className = brush.Properties[(uint)propPointClassName].StringV;
					Dictionary<string, string> otherProperties = new Dictionary<string, string>();
					Property value;
					if (propOtherProperties != -1 && brush.Properties.TryGetValue((uint)propOtherProperties, out value))
						parseProperties(value.StringV, otherProperties);
					if (aggregate != null)
						otherProperties.Add("$aggregate", aggregate);
					List<KeyValuePair<string, string> > connections = new List<KeyValuePair<string, string> >();
					if (propConnections != -1 && brush.Properties.TryGetValue((uint)propConnections, out value))
						parseConnections(value.StringV, connections);
					KeyValuePair<string, string>[] connectionsArr = new KeyValuePair<string, string>[connections.Count];
					ents.Add(new PointEntity(className, otherProperties, connections.ToArray(), vmfIDSeq++, mb2ToVmf(readOrigin(brush))));
				}
				else
				if (propSolidClassName != -1 && brush.Properties.ContainsKey((uint)propSolidClassName))
				{
					List<Solid> solids = new List<Solid>();
					solids.Add(mb2BrushToVmfSolid(brush));
					foreach (Brush child in brush.Children)
						processSolids(child, solids);
					vec3 optionalOrigin = processOrigin(brush);
					string className = brush.Properties[(uint)propSolidClassName].StringV;
					Dictionary<string, string> otherProperties = new Dictionary<string, string>();
					Property value;
					if (propOtherProperties != -1 && brush.Properties.TryGetValue((uint)propOtherProperties, out value))
						parseProperties(value.StringV, otherProperties);
					if (aggregate != null)
						otherProperties.Add("$aggregate", aggregate);
					List<KeyValuePair<string, string> > connections = new List<KeyValuePair<string, string> >();
					if (propConnections != -1 && brush.Properties.TryGetValue((uint)propConnections, out value))
						parseConnections(value.StringV, connections);
					KeyValuePair<string, string>[] connectionsArr = new KeyValuePair<string, string>[connections.Count];
					ents.Add(new SolidEntity(className, otherProperties, connections.ToArray(), vmfIDSeq++, solids.ToArray(), optionalOrigin));
				}
			foreach (Brush child in brush.Children)
				processEntities(child, aggregate, ents);
		}

		private static vec3 readOrigin(Brush brush)
		{
			if (brush.Planes.Length != 6)
				throw new ApplicationException("Origin brushes must be simple axis-aligned boxes.");
			float
				x = 0,
				y = 0,
				z = 0;
			foreach (Plane plane in brush.Planes)
			{
				if (plane.U.X == 0 && plane.V.X == 0) x += plane.P0.X;
				if (plane.U.Y == 0 && plane.V.Y == 0) y += plane.P0.Y;
				if (plane.U.Z == 0 && plane.V.Z == 0) z += plane.P0.Z;
			}
			return new vec3(x/2, y/2, z/2);
		}

		private static Brush makeOriginBox(vec3 origin, Dictionary<uint, Property> properties, Brush[] children)
		{
			Plane[] planes = new Plane[]{
				new Plane(origin+new vec3(- .5f,  0   ,  0   ), new vec3( 0, 0,  1), new vec3(0, 1,  0), new Dictionary<uint, Property>(), null),
				new Plane(origin+new vec3(  .5f,  0   ,  0   ), new vec3( 0, 0, -1), new vec3(0, 1,  0), new Dictionary<uint, Property>(), null),
				new Plane(origin+new vec3( 0   , - .5f,  0   ), new vec3( 1, 0,  0), new vec3(0, 0,  1), new Dictionary<uint, Property>(), null),
				new Plane(origin+new vec3( 0   ,   .5f,  0   ), new vec3( 1, 0,  0), new vec3(0, 0, -1), new Dictionary<uint, Property>(), null),
				new Plane(origin+new vec3( 0   ,  0   , - .5f), new vec3(-1, 0,  0), new vec3(0, 1,  0), new Dictionary<uint, Property>(), null),
				new Plane(origin+new vec3( 0   ,  0   ,   .5f), new vec3( 1, 0,  0), new vec3(0, 1,  0), new Dictionary<uint, Property>(), null)};
			return new Brush(properties, planes, children);
		}

		private static void parseProperties(string propertiesStr, Dictionary<string, string> properties)
		{
			string[] lines = propertiesStr.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				string[] parts = line.Split(cQuotArr);
				if (parts.Length == 5)
					properties.Add(parts[1], parts[3]);
			}
		}

		private static void parseConnections(string propertiesStr, List<KeyValuePair<string, string> > connections)
		{
			string[] lines = propertiesStr.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				string[] parts = line.Split(cQuotArr);
				if (parts.Length == 5)
					connections.Add(new KeyValuePair<string, string>(parts[1], parts[3]));
			}
		}

		private static vec3 mb2ToVmf(vec3 v)
		{
			return new vec3(v.X*scale, -v.Z*scale, v.Y*scale);
		}

		private static vec3 vmfToMB2(vec3 v)
		{
			return new vec3(v.X/scale, v.Z/scale, -v.Y/scale);
		}

		private static Solid mb2BrushToVmfSolid(Brush brush)
		{
			Property value;
			Side[] sides = new Side[brush.Planes.Length];
			for (uint i = 0; i != brush.Planes.Length; i++)
			{
				Plane plane = brush.Planes[i];
				string material;
				if (propMaterial != -1 && plane.Properties.TryGetValue((uint)propMaterial, out value))
					material = value.StringV;
				else
					material = VmfFormat.DefaultMaterial;
				uint lightmapScale;
				if (propLightmapScale != -1 && plane.Properties.TryGetValue((uint)propLightmapScale, out value))
					lightmapScale = (uint)value.IntV;
				else
					lightmapScale = VmfFormat.DefaultLightmapScale;
				uint smoothingGroups;
				if (propSmoothingGroups != -1 && plane.Properties.TryGetValue((uint)propSmoothingGroups, out value))
					smoothingGroups = (uint)value.IntV;
				else
					smoothingGroups = VmfFormat.DefaultSmoothingGroups;
				Displacement optionalDisplacement = null;
				if (propDisplacementLink != -1 && propDisplacementData != -1 && plane.Properties.TryGetValue((uint)propDisplacementLink, out value))
				{
					int link = value.IntV;
					foreach (Brush child in brush.Children)
						if (child.Properties.TryGetValue((uint)propDisplacementLink, out value) &&
								value.IntV == link &&
								child.Properties.TryGetValue((uint)propDisplacementData, out value))
						{
							optionalDisplacement = new Displacement(mb2ToVmf(readOrigin(child)), value.StringV);
							break;
						}
				}
				Dictionary<string, string> otherProperties = new Dictionary<string, string>();
				if (propOtherProperties != -1 && plane.Properties.TryGetValue((uint)propOtherProperties, out value))
					parseProperties(value.StringV, otherProperties);
				sides[i] = new Side(
					mb2ToVmf(plane.P0), mb2ToVmf(plane.P0+plane.V), mb2ToVmf(plane.P0+plane.U),
					material, lightmapScale, smoothingGroups, optionalDisplacement, otherProperties, vmfSideIDSeq++);
			}
			return new Solid(sides, vmfIDSeq++);
		}

		private static Brush vmfSolidToMB2Brush(Solid solid, Dictionary<uint, Property> optionalAdditionalProperties, List<Brush> children)
		{
			Dictionary<uint, Property> properties = new Dictionary<uint, Property>();
			Plane[] planes = new Plane[solid.Sides.Length];
			properties.Add((uint)propOriginalID, new Property((uint)propOriginalID, (int)solid.ID));
			int localDisplacementID = 0;
			for (uint i = 0; i != solid.Sides.Length; i++)
			{
				Side side = solid.Sides[i];
				Dictionary<uint, Property> planeProperties = new Dictionary<uint, Property>();
				planeProperties.Add((uint)propOriginalID   , new Property((uint)propOriginalID   , (int)side.ID           ));
				planeProperties.Add((uint)propLightmapScale, new Property((uint)propLightmapScale, (int)side.LightmapScale));
				if (!side.Material.Equals(VmfFormat.DefaultMaterial))
					planeProperties.Add((uint)propMaterial, new Property((uint)propMaterial, side.Material));
				planeProperties.Add((uint)propSmoothingGroups, new Property((uint)propSmoothingGroups, (int)side.SmoothingGroups));
				if (side.OptionalDisplacement != null)
				{
					planeProperties.Add((uint)propDisplacementLink, new Property((uint)propDisplacementLink, localDisplacementID));
					Dictionary<uint, Property> dispProperties = new Dictionary<uint, Property>(); //for brush
					dispProperties.Add((uint)propDisplacementData, new Property((uint)propDisplacementData, side.OptionalDisplacement.Data));
					dispProperties.Add((uint)propDisplacementLink, new Property((uint)propDisplacementLink, localDisplacementID));
					children.Add(makeOriginBox(vmfToMB2(side.OptionalDisplacement.Origin), dispProperties, new Brush[]{}));
					localDisplacementID++;
				}
				if (side.OtherProperties.Count != 0)
				{
					StringBuilder otherProps = new StringBuilder();
					foreach (KeyValuePair<string, string> entry in side.OtherProperties)
					{
						otherProps.Append('\"');
						otherProps.Append(entry.Key);
						otherProps.Append("\" \"");
						otherProps.Append(entry.Value);
						otherProps.Append("\"\r\n");
					}
					planeProperties.Add((uint)propOtherProperties, new Property((uint)propOtherProperties, otherProps.ToString()));
				}
				planes[i] = new Plane(vmfToMB2(side.P0), vmfToMB2(side.P2-side.P0), vmfToMB2(side.P1-side.P0), planeProperties, null);
			}
			if (optionalAdditionalProperties != null)
				foreach (KeyValuePair<uint, Property> entry in optionalAdditionalProperties)
					properties.Add(entry.Key, entry.Value);
			return new Brush(properties, planes, children.ToArray());
		}
	}
}
