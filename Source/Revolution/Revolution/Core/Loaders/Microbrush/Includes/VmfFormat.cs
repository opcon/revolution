//Copyright © 2009-2010 Max 'Shrinker' Wieden
//Refer to license for license details.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Shrinker.Common;
using Shrinker.Vmf.Parser;

namespace Shrinker.Vmf
{
	class VmfFormat
	{
		public const string DefaultMaterial = "TOOLS/TOOLSNODRAW";
		public const uint
			DefaultLightmapScale   = 16,
			DefaultSmoothingGroups =  0;
		public const float
			DefaultRotation = 0     ,
			SmallestGrid    =  .125f;

		private static char[]
			cParArr   = new char[]{'(', ')'},
			cSpaceArr = new char[]{' '     };
		private static StreamWriter w;
		private static CultureInfo invariantCulture = CultureInfo.InvariantCulture;

		public static void Save(World world, string path)
		{
			w = null;
			try
			{
				w = new StreamWriter(new BufferedStream(File.Create(path)));
				w.Write("versioninfo{\"editorversion\" \"400\" \"editorbuild\" \"4715\" \"mapversion\" \"5\" \"formatversion\" \"100\" \"prefab\" \"0\"}visgroups{}viewsettings{\"bSnapToGrid\" \"1\" \"bShowGrid\" \"1\" \"bShowLogicalGrid\" \"0\" \"nGridSpacing\" \"16\" \"bShow3DGrid\" \"0\"}cameras{\"activecamera\" \"0\" camera{\"position\" \"[0 0 0]\" \"look\" \"[0 64 0]\"}}cordon{\"mins\" \"(-1024 -1024 -1024)\" \"maxs\" \"(1024 1024 1024)\" \"active\" \"0\"}");
				w.Write("world{\"id\" \"");
				w.Write(world.ID);
				w.Write("\" ");
				foreach (KeyValuePair<string, string> entry in world.OtherProperties)
				{
					w.Write('\"');
					w.Write(entry.Key);
					w.Write("\" \"");
					w.Write(entry.Value);
					w.Write("\" ");
				}
				saveSolids(world.Solids);
				w.Write('}');
				foreach (Entity ent in world.Entities)
					if (ent.GetType() == typeof(PointEntity))
					{
						PointEntity ent2 = (PointEntity)ent;
						w.Write("entity{\"id\" \"");
						w.Write(ent.ID);
						w.Write("\" \"classname\" \"");
						w.Write(ent.ClassName);
						w.Write("\" \"origin\" \"");
						w.Write(ent2.Origin.X.ToString(invariantCulture));
						w.Write(' ');
						w.Write(ent2.Origin.Y.ToString(invariantCulture));
						w.Write(' ');
						w.Write(ent2.Origin.Z.ToString(invariantCulture));
						w.Write("\" ");
						foreach (KeyValuePair<string, string> entry in ent.OtherProperties)
						{
							w.Write('\"');
							w.Write(entry.Key);
							w.Write("\" \"");
							w.Write(entry.Value);
							w.Write("\" ");
						}
						if (ent.Connections.Length != 0)
						{
							w.Write("connections{");
							for (int i = ent.Connections.Length-1; i != -1; i--)
							{
								w.Write('\"');
								w.Write(ent.Connections[i].Key);
								w.Write("\" \"");
								w.Write(ent.Connections[i].Value);
								w.Write("\" ");
							}
							w.Write('}');
						}
						w.Write('}');
					}
					else //ent.GetType() == typeof(SolidEntity)
					{
						SolidEntity ent2 = (SolidEntity)ent;
						w.Write("entity{\"id\" \"");
						w.Write(ent.ID);
						w.Write("\" \"classname\" \"");
						w.Write(ent.ClassName);
						if ((object)ent2.OptionalOrigin != null)
						{
							w.Write("\" \"origin\" \"");
							w.Write(ent2.OptionalOrigin.X.ToString(invariantCulture));
							w.Write(' ');
							w.Write(ent2.OptionalOrigin.Y.ToString(invariantCulture));
							w.Write(' ');
							w.Write(ent2.OptionalOrigin.Z.ToString(invariantCulture));
						}
						w.Write("\" ");
						foreach (KeyValuePair<string, string> entry in ent.OtherProperties)
						{
							w.Write('\"');
							w.Write(entry.Key);
							w.Write("\" \"");
							w.Write(entry.Value);
							w.Write("\" ");
						}
						if (ent.Connections.Length != 0)
						{
							w.Write("connections{");
							for (int i = ent.Connections.Length-1; i != -1; i--)
							{
								w.Write('\"');
								w.Write(ent.Connections[i].Key);
								w.Write("\" \"");
								w.Write(ent.Connections[i].Value);
								w.Write("\" ");
							}
							w.Write('}');
						}
						saveSolids(ent2.Solids);
						w.Write('}');
					}
			}
			finally
			{
				if (w != null)
					w.Close();
			}
		}

		private static void saveSolids(Solid[] solids)
		{
			foreach (Solid solid in solids)
			{
				w.Write("solid{\"id\" \"");
				w.Write(solid.ID);
				w.Write("\" ");
				foreach (Side side in solid.Sides)
				{
					w.Write("side{\"id\" \"");
					w.Write(side.ID);
					w.Write("\" \"plane\" \"(");
					w.Write(side.P0.X.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P0.Y.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P0.Z.ToString(invariantCulture));
					w.Write(") (");
					w.Write(side.P1.X.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P1.Y.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P1.Z.ToString(invariantCulture));
					w.Write(") (");
					w.Write(side.P2.X.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P2.Y.ToString(invariantCulture));
					w.Write(' ');
					w.Write(side.P2.Z.ToString(invariantCulture));
					w.Write(")\" \"material\" \"");
					w.Write(side.Material);
					w.Write("\" ");
					if (side.OptionalDisplacement != null)
					{
						//WARNING, there can be a (bad surface extent) CRASH in-game if we do not sort the side properties in front of a dispinfo node in a certain error.
						//I could determine this only via trial and error, and considering that VMFs do look pretty much like containing a lot of "properties" in the Java sense, or "dictionaries" in the C# sense, it absolutely makes no sense that the game would only accept these things if the properties are sorted at this point, but that is exactly the case.
						//This is an indicator for really, really bad programming by Valve.
						//ONE ORDER THAT WORKS: id, plane, material, uaxis, vaxis, rotation, THEN other props
						string value;
						if (side.OtherProperties.TryGetValue("uaxis", out value))
						{
							w.Write("\"uaxis\" \"");
							w.Write(value);
							w.Write("\" ");
						}
						if (side.OtherProperties.TryGetValue("vaxis", out value))
						{
							w.Write("\"vaxis\" \"");
							w.Write(value);
							w.Write("\" ");
						}
						if (side.OtherProperties.TryGetValue("rotation", out value))
						{
							w.Write("\"rotation\" \"");
							w.Write(value);
							w.Write("\" ");
						}
						foreach (KeyValuePair<string, string> entry in side.OtherProperties)
							if (!entry.Key.Equals("uaxis"   ) &&
									!entry.Key.Equals("vaxis"   ) &&
									!entry.Key.Equals("rotation"))
							{
								w.Write('\"');
								w.Write(entry.Key);
								w.Write("\" \"");
								w.Write(entry.Value);
								w.Write("\" ");
							}
						w.Write("dispinfo{\"startposition\" \"[");
						w.Write(side.OptionalDisplacement.Origin.X.ToString(invariantCulture));
						w.Write(' ');
						w.Write(side.OptionalDisplacement.Origin.Y.ToString(invariantCulture));
						w.Write(' ');
						w.Write(side.OptionalDisplacement.Origin.Z.ToString(invariantCulture));
						w.Write("]\" ");
						w.Write(side.OptionalDisplacement.Data);
						w.Write('}');
					}
					else
					{
						foreach (KeyValuePair<string, string> entry in side.OtherProperties)
						{
							w.Write('\"');
							w.Write(entry.Key);
							w.Write("\" \"");
							w.Write(entry.Value);
							w.Write("\" ");
						}
					}
					w.Write("\"smoothing_groups\" \"");
					w.Write(side.SmoothingGroups);
					w.Write("\" \"lightmapscale\" \"");
					w.Write(side.LightmapScale);
					w.Write("\"}");
				}
				w.Write('}');
			}
		}

		public static World Load(string path)
		{
			Parser.Parser p = new Parser.Parser();
			RuleInstance root = p.Parse(File.ReadAllText(path));
			Solid[] solids = null;
			List<Entity> entities = new List<Entity>();
			Dictionary<string, string> otherProperties = new Dictionary<string, string>();
			uint id = 0;
			foreach (Node n in root.Children)
				switch (n.Type)
				{
					case NodeType.Rule_World:
					{
						Node[] children = ((RuleInstance)n).Children;
						for (uint i = 2; children[i].Type != NodeType.Tok_RBrace; i++)
							if (children[i].Type == NodeType.Tok_StringLit)
							{
								string
									key   = ((Token)children[i  ]).Image,
									value = ((Token)children[i+1]).Image;
								key   = key  .Substring(1, key  .Length-2);
								value = value.Substring(1, value.Length-2);
								i++;
								if (key.Equals("id"))
									id = uint.Parse(value);
								else
									otherProperties.Add(key, value);
							}
						solids = loadSolids((RuleInstance)n);
						break;
					}
					case NodeType.Rule_Entity:
						loadEntity((RuleInstance)n, entities);
						break;
					case NodeType.Rule_HiddenEntity:
						loadEntity((RuleInstance)((RuleInstance)n).Children[2], entities);
						break;
				}
			if (solids == null)
				throw new ApplicationException("No world found.");
			return new World(solids, entities.ToArray(), otherProperties, id);
		}

		private static KeyValuePair<string, string>[] loadConnections(RuleInstance rule)
		//if no delay is used, events are fired in the reverse order that they are listed in the VMF
		//to make events easier to handle, they are stored in the preferred firing order
		{
			Node[] children = rule.Children;
			KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[(children.Length-3)/2];
			for (int i = result.Length-1; i != -1; i--)
			{
				string
					part1 = ((Token)children[2*i+2]).Image,
					part2 = ((Token)children[2*i+3]).Image;
				part1 = part1.Substring(1, part1.Length-2);
				part2 = part2.Substring(1, part2.Length-2);
				result[i] = new KeyValuePair<string, string>(part1, part2);
			}
			return result;
		}

		private static Displacement loadDisplacement(RuleInstance rule)
		{
			vec3 origin = null;
			Node[] children = rule.Children;
			StringBuilder s = new StringBuilder();
			for (uint i = 2; i+1 != children.Length; i++)
			{
				Node child = children[i];
				if (child.Type == NodeType.Tok_StringLit)
				{
					if (((Token)child).Image.Equals("\"startposition\""))
					{
						string value = ((Token)children[i+1]).Image;
						value = value.Substring(2, value.Length-4);
						string[]
							points = value.Split(cParArr, StringSplitOptions.RemoveEmptyEntries),
							pointCoords;
						pointCoords = points[0].Split(cSpaceArr);
						origin = new vec3(
							float.Parse(pointCoords[0], invariantCulture),
							float.Parse(pointCoords[1], invariantCulture),
							float.Parse(pointCoords[2], invariantCulture));
						alignPointToGrid(origin);
					}
					else
					{
						dump(child, s);
						dump(children[i+1], s);
						s.Append("\r\n");
					}
					i++;
				}
				else
					dump(child, s);
			}
			if ((object)origin == null)
				throw new ApplicationException("Origin (\"startposition\") must be present for displacement");
			return new Displacement(origin, s.ToString());
		}

		private static void dump(Node node, StringBuilder s)
		{
			if (node.GetType() == typeof(RuleInstance))
			{
				Node[] children = ((RuleInstance)node).Children;
				for (uint i = 0; i != children.Length; i++)
				{
					Node child = children[i];
					dump(child, s);
					if (child.Type == NodeType.Tok_StringLit)
					{
						dump(children[i+1], s);
						s.Append("\r\n");
						i++;
					}
				}
			}
			else
			{
				Token tok = (Token)node;
				foreach (Token ft in tok.PrecedingFillerTokens)
					s.Append(' ');
				s.Append(tok.Image);
				if (tok.Image.Equals("{"))
					s.Append("\r\n");
			}
		}

		private static Solid[] loadSolids(RuleInstance rule)
		{
			List<Solid> result = new List<Solid>();
			foreach (Node n in rule.Children)
				if (n.Type == NodeType.Rule_Solid)
					result.Add(loadSolid((RuleInstance)n));
				else
				if (n.Type == NodeType.Rule_HiddenSolid)
					result.Add(loadSolid((RuleInstance)((RuleInstance)n).Children[2]));
			return result.ToArray();
		}

		private static Solid loadSolid(RuleInstance rule)
		{
			Node[] children = rule.Children;
			List<Side> sides = new List<Side>();
			uint id = 0;
			for (uint i = 2; children[i].Type != NodeType.Tok_RBrace; i++)
				if (children[i].Type == NodeType.Tok_StringLit)
				{
					string
						key   = ((Token)children[i  ]).Image,
						value = ((Token)children[i+1]).Image;
					key   = key  .Substring(1, key  .Length-2);
					value = value.Substring(1, value.Length-2);
					i++;
					if (key.Equals("id"))
						id = uint.Parse(value);
				}
				else
				if (children[i].Type == NodeType.Rule_Side)
					sides.Add(loadSide((RuleInstance)children[i]));
			return new Solid(sides.ToArray(), id);
		}

		private static Side loadSide(RuleInstance rule)
		{
			Node[] children = rule.Children;
			vec3 p0 = null, p1 = null, p2 = null;
			string material = null;
			Displacement displacement = null;
			uint
				lightmapScale   = DefaultLightmapScale  ,
				smoothingGroups = DefaultSmoothingGroups;
			Dictionary<string, string> otherProperties = new Dictionary<string, string>();
			uint id = 0;
			for (uint i = 2; children[i].Type != NodeType.Tok_RBrace; i++)
				if (children[i].Type == NodeType.Tok_StringLit)
				{
					string
						key   = ((Token)children[i  ]).Image,
						value = ((Token)children[i+1]).Image;
					key   = key  .Substring(1, key  .Length-2);
					value = value.Substring(1, value.Length-2);
					i++;
					if (key.Equals("id"))
						id = uint.Parse(value);
					else
					if (key.Equals("lightmapscale"))
						lightmapScale = uint.Parse(value);
					else
					if (key.Equals("material"))
						material = value;
					else
					if (key.Equals("plane"))
					{
						string[]
							points = value.Split(cParArr, StringSplitOptions.RemoveEmptyEntries),
							pointCoords;
						pointCoords = points[0].Split(cSpaceArr);
						p0 = new vec3(
							float.Parse(pointCoords[0], invariantCulture),
							float.Parse(pointCoords[1], invariantCulture),
							float.Parse(pointCoords[2], invariantCulture));
						pointCoords = points[2].Split(cSpaceArr);
						p1 = new vec3(
							float.Parse(pointCoords[0], invariantCulture),
							float.Parse(pointCoords[1], invariantCulture),
							float.Parse(pointCoords[2], invariantCulture));
						pointCoords = points[4].Split(cSpaceArr);
						p2 = new vec3(
							float.Parse(pointCoords[0], invariantCulture),
							float.Parse(pointCoords[1], invariantCulture),
							float.Parse(pointCoords[2], invariantCulture));
						alignPointToGrid(p0);
						alignPointToGrid(p1);
						alignPointToGrid(p2);
					}
					else
					if (key.Equals("smoothing_groups"))
						smoothingGroups = uint.Parse(value);
					else
						otherProperties.Add(key, value);
				}
				else //children[i].Type == NodeType.Rule_DispInfo
					displacement = loadDisplacement((RuleInstance)children[i]);
			if ((object)p0 == null || material == null)
				throw new ApplicationException("Side incomplete.");
			return new Side(p0, p1, p2, material, lightmapScale, smoothingGroups, displacement, otherProperties, id);
		}

		private static void loadEntity(RuleInstance rule, List<Entity> entities)
		{
			Node[] children = rule.Children;
			vec3 origin = null;
			string className = null;
			Dictionary<string, string> otherProperties = new Dictionary<string, string>();
			KeyValuePair<string, string>[] connections = null;
			uint id = 0;
			for (uint i = 2; children[i].Type != NodeType.Tok_RBrace; i++)
			{
				Node n = children[i];
				switch (n.Type)
				{
					case NodeType.Tok_StringLit:
					{
						string
							key   = ((Token)children[i  ]).Image,
							value = ((Token)children[i+1]).Image;
						key   = key  .Substring(1, key  .Length-2);
						value = value.Substring(1, value.Length-2);
						i++;
						if (key.Equals("id"))
							id = uint.Parse(value);
						else
						if (key.Equals("classname"))
							className = value;
						else
						if (key.Equals("origin"))
						{
							string[]
								points = value.Split(cParArr, StringSplitOptions.RemoveEmptyEntries),
								pointCoords;
							pointCoords = points[0].Split(cSpaceArr);
							origin = new vec3(
								float.Parse(pointCoords[0], invariantCulture),
								float.Parse(pointCoords[1], invariantCulture),
								float.Parse(pointCoords[2], invariantCulture));
							alignPointToGrid(origin);
						}
						else
							otherProperties.Add(key, value);
						break;
					}
					case NodeType.Rule_Connections:
						connections = loadConnections((RuleInstance)n);
						break;
				}
			}
			if (className == null)
				throw new ApplicationException("Class name required for entity.");
			if (connections == null)
				connections = new KeyValuePair<string, string>[0]{};
			Solid[] solids = loadSolids(rule);
			if (solids.Length == 0)
			{
				if ((object)origin == null)
					throw new ApplicationException("Origin required for point entity.");
				entities.Add(new PointEntity(className, otherProperties, connections, id, origin));
			}
			else
				entities.Add(new SolidEntity(className, otherProperties, connections, id, solids, origin));
		}

		private static void alignPointToGrid(vec3 v)
		{
			alignPointCoordinateToGrid(ref v.X);
			alignPointCoordinateToGrid(ref v.Y);
			alignPointCoordinateToGrid(ref v.Z);
		}

		private static void alignPointCoordinateToGrid(ref float x)
		{
			x = (float)Math.Floor(x/SmallestGrid+.5f)*SmallestGrid;
		}
	}
}
