//Generated with Imperator Parser Generator v. 2009-08-15
//Imperator by Max 'Shrinker' Wieden
//http://shrinker.beyond-veils.de/
//http://shrinker.scottbaker.eu/

using System;
using System.Collections.Generic;

namespace Shrinker.Vmf.Parser
{
	/// <summary/>
	public class ParserException: Exception
	{
		/// <summary/><param name="message"/>
		public ParserException(string message):
			base(message)
		{
		}
	}

	/// <summary/>
	public enum NodeType
	{
		/// <summary/>
		Tok_EOF,
		/// <summary/>
		Tok_LBrace,
		/// <summary/>
		Tok_RBrace,
		/// <summary/>
		Tok_Name,
			/// <summary/>
			Tok_allowed_verts,
			/// <summary/>
			Tok_alphas,
			/// <summary/>
			Tok_camera,
			/// <summary/>
			Tok_cameras,
			/// <summary/>
			Tok_connections,
			/// <summary/>
			Tok_cordon,
			/// <summary/>
			Tok_dispinfo,
			/// <summary/>
			Tok_distances,
			/// <summary/>
			Tok_editor,
			/// <summary/>
			Tok_entity,
			/// <summary/>
			Tok_group,
			/// <summary/>
			Tok_hidden,
			/// <summary/>
			Tok_normals,
			/// <summary/>
			Tok_offsets,
			/// <summary/>
			Tok_offset_normals,
			/// <summary/>
			Tok_side,
			/// <summary/>
			Tok_solid,
			/// <summary/>
			Tok_triangle_tags,
			/// <summary/>
			Tok_versioninfo,
			/// <summary/>
			Tok_viewsettings,
			/// <summary/>
			Tok_visgroup,
			/// <summary/>
			Tok_visgroups,
			/// <summary/>
			Tok_world,
		/// <summary/>
		Tok_StringLit,
		/// <summary/>
		Tok_Whitespace,
		/// <summary/>
		Rule_Start,
		/// <summary/>
		Rule_VersionInfo,
		/// <summary/>
		Rule_VisGroups,
		/// <summary/>
		Rule_VisGroup,
		/// <summary/>
		Rule_ViewSettings,
		/// <summary/>
		Rule_World,
		/// <summary/>
		Rule_Solid,
		/// <summary/>
		Rule_HiddenSolid,
		/// <summary/>
		Rule_Group,
		/// <summary/>
		Rule_Side,
		/// <summary/>
		Rule_DispInfo,
		/// <summary/>
		Rule_Normals,
		/// <summary/>
		Rule_Distances,
		/// <summary/>
		Rule_Offsets,
		/// <summary/>
		Rule_Offset_Normals,
		/// <summary/>
		Rule_Alphas,
		/// <summary/>
		Rule_Triangle_Tags,
		/// <summary/>
		Rule_Allowed_Verts,
		/// <summary/>
		Rule_Editor,
		/// <summary/>
		Rule_Entity,
		/// <summary/>
		Rule_HiddenEntity,
		/// <summary/>
		Rule_Connections,
		/// <summary/>
		Rule_Cameras,
		/// <summary/>
		Rule_Camera,
		/// <summary/>
		Rule_Cordon
	}

	/// <summary/>
	public abstract class Node
	{
		/// <summary/>
		public readonly NodeType Type;

		/// <summary/><param name="type"/>
		public Node(NodeType type)
		{
			Type = type;
		}
	}

	/// <summary/>
	public class RuleInstance: Node //represents an inner node
	{
		/// <summary/>
		public readonly Node[] Children;

		/// <summary/><param name="type"/><param name="children"/>
		public RuleInstance(NodeType type, Node[] children):
			base(type)
		{
			Children = children;
		}
	}

	/// <summary/>
	public class Token: Node //represents a leaf node
	{
		/// <summary/>
		public readonly Token[] PrecedingFillerTokens;
		/// <summary/>
		public readonly string Image;
		/// <summary/>
		public readonly int LineNr;
		/// <summary/>
		public readonly int CharNr;

		private static Dictionary<string, NodeType> specForTok_Name;

		static Token()
		{
			specForTok_Name = new Dictionary<string, NodeType>();
			specForTok_Name.Add("allowed_verts", NodeType.Tok_allowed_verts);
			specForTok_Name.Add("alphas", NodeType.Tok_alphas);
			specForTok_Name.Add("camera", NodeType.Tok_camera);
			specForTok_Name.Add("cameras", NodeType.Tok_cameras);
			specForTok_Name.Add("connections", NodeType.Tok_connections);
			specForTok_Name.Add("cordon", NodeType.Tok_cordon);
			specForTok_Name.Add("dispinfo", NodeType.Tok_dispinfo);
			specForTok_Name.Add("distances", NodeType.Tok_distances);
			specForTok_Name.Add("editor", NodeType.Tok_editor);
			specForTok_Name.Add("entity", NodeType.Tok_entity);
			specForTok_Name.Add("group", NodeType.Tok_group);
			specForTok_Name.Add("hidden", NodeType.Tok_hidden);
			specForTok_Name.Add("normals", NodeType.Tok_normals);
			specForTok_Name.Add("offsets", NodeType.Tok_offsets);
			specForTok_Name.Add("offset_normals", NodeType.Tok_offset_normals);
			specForTok_Name.Add("side", NodeType.Tok_side);
			specForTok_Name.Add("solid", NodeType.Tok_solid);
			specForTok_Name.Add("triangle_tags", NodeType.Tok_triangle_tags);
			specForTok_Name.Add("versioninfo", NodeType.Tok_versioninfo);
			specForTok_Name.Add("viewsettings", NodeType.Tok_viewsettings);
			specForTok_Name.Add("visgroup", NodeType.Tok_visgroup);
			specForTok_Name.Add("visgroups", NodeType.Tok_visgroups);
			specForTok_Name.Add("world", NodeType.Tok_world);
		}

		private static NodeType specializeType(NodeType type, string image)
		{
			NodeType newType;
			switch (type)
			{
				case NodeType.Tok_Name:
					if (specForTok_Name.TryGetValue(image, out newType))
						type = newType;
				break;
			}
			return type;
		}

		/// <summary/><param name="precedingFillerTokens"/><param name="type"/><param name="image"/><param name="lineNr"/><param name="charNr"/>
		public Token(Token[] precedingFillerTokens, NodeType type, string image, int lineNr, int charNr):
			base(specializeType(type, image))
		{
			PrecedingFillerTokens = precedingFillerTokens;
			Image                 = image                ;
			LineNr                = lineNr               ;
			CharNr                = charNr               ;
		}
	}

	/// <summary/>
	public class Parser
	{
		private static readonly Dictionary<NodeType, string> ruleNames;

		static Parser()
		{
			ruleNames = new Dictionary<NodeType,string>();
			ruleNames.Add(NodeType.Tok_EOF, "EOF");
			ruleNames.Add(NodeType.Tok_LBrace, "LBrace");
			ruleNames.Add(NodeType.Tok_RBrace, "RBrace");
			ruleNames.Add(NodeType.Tok_Name, "Name");
			ruleNames.Add(NodeType.Tok_StringLit, "StringLit");
			ruleNames.Add(NodeType.Tok_Whitespace, "Whitespace");
			ruleNames.Add(NodeType.Rule_Start, "Start");
			ruleNames.Add(NodeType.Rule_VersionInfo, "VersionInfo");
			ruleNames.Add(NodeType.Rule_VisGroups, "VisGroups");
			ruleNames.Add(NodeType.Rule_VisGroup, "VisGroup");
			ruleNames.Add(NodeType.Rule_ViewSettings, "ViewSettings");
			ruleNames.Add(NodeType.Rule_World, "World");
			ruleNames.Add(NodeType.Rule_Solid, "Solid");
			ruleNames.Add(NodeType.Rule_HiddenSolid, "HiddenSolid");
			ruleNames.Add(NodeType.Rule_Group, "Group");
			ruleNames.Add(NodeType.Rule_Side, "Side");
			ruleNames.Add(NodeType.Rule_DispInfo, "DispInfo");
			ruleNames.Add(NodeType.Rule_Normals, "Normals");
			ruleNames.Add(NodeType.Rule_Distances, "Distances");
			ruleNames.Add(NodeType.Rule_Offsets, "Offsets");
			ruleNames.Add(NodeType.Rule_Offset_Normals, "Offset_Normals");
			ruleNames.Add(NodeType.Rule_Alphas, "Alphas");
			ruleNames.Add(NodeType.Rule_Triangle_Tags, "Triangle_Tags");
			ruleNames.Add(NodeType.Rule_Allowed_Verts, "Allowed_Verts");
			ruleNames.Add(NodeType.Rule_Editor, "Editor");
			ruleNames.Add(NodeType.Rule_Entity, "Entity");
			ruleNames.Add(NodeType.Rule_HiddenEntity, "HiddenEntity");
			ruleNames.Add(NodeType.Rule_Connections, "Connections");
			ruleNames.Add(NodeType.Rule_Cameras, "Cameras");
			ruleNames.Add(NodeType.Rule_Camera, "Camera");
			ruleNames.Add(NodeType.Rule_Cordon, "Cordon");
		}

		/// <summary/><param name="type"/><returns/>
		public static string GetRuleName(NodeType type)
		{
			return ruleNames[type];
		}

		private string source;
		private int index, length, laOffset, currentLine, currentChar;
		private readonly List <Token> fetchedTokens;
		private readonly Stack<bool > laSuccess    ;
		private readonly Stack<int  > laOffsets    ;
		private readonly Stack<bool > onceOrMoreB  ;

		/// <summary/>
		public Parser()
		{
			fetchedTokens = new List <Token>();
			laSuccess     = new Stack<bool >();
			laOffsets     = new Stack<int  >();
			onceOrMoreB   = new Stack<bool >();
		}

		/// <summary/><param name="source"/><returns/>
		public RuleInstance Parse(string source)
		{
			this.source = source;
			index    =             0;
			length   = source.Length;
			laOffset =             0;
			currentLine = 1;
			currentChar = 1;
			RuleInstance r;
			try
			{
				r = matchStart();
			}
			finally
			{
				this.source = null;
				fetchedTokens.Clear();
				laSuccess    .Clear();
				laOffsets    .Clear();
				onceOrMoreB  .Clear();
			}
			return r;
		}

		private Token nextToken()
		{
			return nextToken(true);
		}

		private Token fetchToken(int offset)
		{
			while (fetchedTokens.Count <= offset)
			{
				Token tok = nextToken(false);
				fetchedTokens.Add(tok);
				//if token stream is exhausted, return EOF
				if (tok.Type == NodeType.Tok_EOF)
				{
					offset = fetchedTokens.Count-1;
					break;
				}
			}
			return fetchedTokens[offset];
		}

		private Token[] getFillerTokens()
		{
			List<Token> result = new List<Token>();
			Stack<int> indices = new Stack<int>(), currentLines = new Stack<int>(), currentChars = new Stack<int>();
			char c;
			bool pass;
			while (true)
			{
				int oldIndex = index, lastLine = currentLine, lastChar = currentChar;

				//Token "Whitespace"
				pass = true;
				indices     .Push(index      );
				currentLines.Push(currentLine);
				currentChars.Push(currentChar);
				indices     .Push(index      );
				currentLines.Push(currentLine);
				currentChars.Push(currentChar);
				onceOrMoreB .Push(false      );
				while (true)
				{
					indices     .Push(index      );
					currentLines.Push(currentLine);
					currentChars.Push(currentChar);
					if (index < length && ((c = source[index]) == '\t' || c == ' ' || c == '\r' || c == '\n'))
					{
						index++;
						if (c == '\r')
						{
						}
						else
						if (c == '\n')
						{
							currentLine++  ;
							currentChar = 1;
						}
						else
							currentChar++;
					}
					else
						pass = false;
					if (pass)
					{
						onceOrMoreB .Pop (    );
						onceOrMoreB .Push(true);
						indices     .Pop (    );
						currentLines.Pop (    );
						currentChars.Pop (    );
					}
					else
					{
						index       = indices     .Pop();
						currentLine = currentLines.Pop();
						currentChar = currentChars.Pop();
						break;
					}
				}
				pass = onceOrMoreB.Pop();
				if (pass)
				{
					indices     .Pop();
					currentLines.Pop();
					currentChars.Pop();
				}
				else
				{
					index       = indices     .Pop();
					currentLine = currentLines.Pop();
					currentChar = currentChars.Pop();
				}
				if (pass)
				{
					indices     .Pop();
					currentLines.Pop();
					currentChars.Pop();
					result.Add(new Token(null, NodeType.Tok_Whitespace, source.Substring(oldIndex, index-oldIndex), lastLine, lastChar));
					continue;
				}
				else
				{
					index       = indices     .Pop();
					currentLine = currentLines.Pop();
					currentChar = currentChars.Pop();
				}

				break;
			}

			return result.ToArray();
		}

		private Token nextToken(bool useFetched)
		{
			if (useFetched && fetchedTokens.Count != 0)
			{
				Token tok = fetchedTokens[0];
				fetchedTokens.RemoveAt(0);
				return tok;
			}

			Token[] fillers = getFillerTokens();

			if (index == length)
				return new Token(fillers, NodeType.Tok_EOF, "", currentLine, currentChar);

			Stack<int> indices = new Stack<int>(), currentLines = new Stack<int>(), currentChars = new Stack<int>();
			char c;
			bool pass;
			int oldIndex = index, lastLine = currentLine, lastChar = currentChar;

			//Token "LBrace"
			pass = true;
			if (index < length && source[index] == '{')
			{
				index++;
				currentChar++;
			}
			else
				pass = false;
			if (pass)
				return new Token(fillers, NodeType.Tok_LBrace, source.Substring(oldIndex, index-oldIndex), lastLine, lastChar);

			//Token "RBrace"
			pass = true;
			if (index < length && source[index] == '}')
			{
				index++;
				currentChar++;
			}
			else
				pass = false;
			if (pass)
				return new Token(fillers, NodeType.Tok_RBrace, source.Substring(oldIndex, index-oldIndex), lastLine, lastChar);

			//Token "Name"
			pass = true;
			indices     .Push(index      );
			currentLines.Push(currentLine);
			currentChars.Push(currentChar);
			indices     .Push(index      );
			currentLines.Push(currentLine);
			currentChars.Push(currentChar);
			onceOrMoreB .Push(false      );
			while (true)
			{
				indices     .Push(index      );
				currentLines.Push(currentLine);
				currentChars.Push(currentChar);
				if (index < length && (c = source[index]) >= 'a' && c <= 'z')
				{
					index      ++;
					currentChar++;
				}
				else
					pass = false;
				if (!pass)
				{
					pass = true;
					if (index < length && source[index] == '_')
					{
						index++;
						currentChar++;
					}
					else
						pass = false;
				}
				if (pass)
				{
					onceOrMoreB .Pop (    );
					onceOrMoreB .Push(true);
					indices     .Pop (    );
					currentLines.Pop (    );
					currentChars.Pop (    );
				}
				else
				{
					index       = indices     .Pop();
					currentLine = currentLines.Pop();
					currentChar = currentChars.Pop();
					break;
				}
			}
			pass = onceOrMoreB.Pop();
			if (pass)
			{
				indices     .Pop();
				currentLines.Pop();
				currentChars.Pop();
			}
			else
			{
				index       = indices     .Pop();
				currentLine = currentLines.Pop();
				currentChar = currentChars.Pop();
			}
			if (pass)
			{
				indices     .Pop();
				currentLines.Pop();
				currentChars.Pop();
				return new Token(fillers, NodeType.Tok_Name, source.Substring(oldIndex, index-oldIndex), lastLine, lastChar);
			}
			else
			{
				index       = indices     .Pop();
				currentLine = currentLines.Pop();
				currentChar = currentChars.Pop();
			}

			//Token "StringLit"
			pass = true;
			if (index < length && source[index] == '\"')
			{
				index++;
				currentChar++;
			}
			else
				pass = false;
			if (pass)
			{
				while (true)
				{
					indices     .Push(index      );
					currentLines.Push(currentLine);
					currentChars.Push(currentChar);
					if (index < length && (c = source[index]) != '\"')
					{
						index++;
						if (c == '\r')
						{
						}
						else
						if (c == '\n')
						{
							currentLine++  ;
							currentChar = 1;
						}
						else
							currentChar++;
					}
					else
						pass = false;
					if (pass)
					{
						indices     .Pop();
						currentLines.Pop();
						currentChars.Pop();
					}
					else
					{
						pass        = true              ;
						index       = indices     .Pop();
						currentLine = currentLines.Pop();
						currentChar = currentChars.Pop();
						break;
					}
				}
				if (pass)
				{
					if (index < length && source[index] == '\"')
					{
						index++;
						currentChar++;
					}
					else
						pass = false;
				}
			}
			if (pass)
				return new Token(fillers, NodeType.Tok_StringLit, source.Substring(oldIndex, index-oldIndex), lastLine, lastChar);

			throw new ParserException("Line "+currentLine+", Char "+currentChar+": No token match");
		}

		private RuleInstance matchStart()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_cameras || tok.Type == NodeType.Tok_cordon || tok.Type == NodeType.Tok_entity || tok.Type == NodeType.Tok_hidden || tok.Type == NodeType.Tok_versioninfo || tok.Type == NodeType.Tok_viewsettings || tok.Type == NodeType.Tok_visgroups || tok.Type == NodeType.Tok_world)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_versioninfo)
					{
						nodes.Add(matchVersionInfo());
					}
					else
					{
						if (fetchToken(laOffset).Type == NodeType.Tok_visgroups)
						{
							nodes.Add(matchVisGroups());
						}
						else
						{
							if (fetchToken(laOffset).Type == NodeType.Tok_viewsettings)
							{
								nodes.Add(matchViewSettings());
							}
							else
							{
								if (fetchToken(laOffset).Type == NodeType.Tok_world)
								{
									nodes.Add(matchWorld());
								}
								else
								{
									if (fetchToken(laOffset).Type == NodeType.Tok_entity)
									{
										nodes.Add(matchEntity());
									}
									else
									{
										if (fetchToken(laOffset).Type == NodeType.Tok_hidden)
										{
											nodes.Add(matchHiddenEntity());
										}
										else
										{
											if (fetchToken(laOffset).Type == NodeType.Tok_cameras)
											{
												nodes.Add(matchCameras());
											}
											else
											{
												nodes.Add(matchCordon());
											}
										}
									}
								}
							}
						}
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_EOF)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected EOF token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Start, nodes.ToArray());
		}

		private RuleInstance matchVersionInfo()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_versioninfo)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected versioninfo token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_VersionInfo, nodes.ToArray());
		}

		private RuleInstance matchVisGroups()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_visgroups)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected visgroups token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_visgroup)
				{
					nodes.Add(matchVisGroup());
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_VisGroups, nodes.ToArray());
		}

		private RuleInstance matchVisGroup()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_visgroup)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected visgroup token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_VisGroup, nodes.ToArray());
		}

		private RuleInstance matchViewSettings()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_viewsettings)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected viewsettings token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_ViewSettings, nodes.ToArray());
		}

		private RuleInstance matchWorld()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_world)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected world token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_group || tok.Type == NodeType.Tok_hidden || tok.Type == NodeType.Tok_solid || tok.Type == NodeType.Tok_StringLit)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						if (fetchToken(laOffset).Type == NodeType.Tok_solid)
						{
							nodes.Add(matchSolid());
						}
						else
						{
							if (fetchToken(laOffset).Type == NodeType.Tok_hidden)
							{
								nodes.Add(matchHiddenSolid());
							}
							else
							{
								nodes.Add(matchGroup());
							}
						}
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_World, nodes.ToArray());
		}

		private RuleInstance matchSolid()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_solid)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected solid token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_editor || tok.Type == NodeType.Tok_side || tok.Type == NodeType.Tok_StringLit)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						if (fetchToken(laOffset).Type == NodeType.Tok_side)
						{
							nodes.Add(matchSide());
						}
						else
						{
							nodes.Add(matchEditor());
						}
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Solid, nodes.ToArray());
		}

		private RuleInstance matchHiddenSolid()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_hidden)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected hidden token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			nodes.Add(matchSolid());
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_HiddenSolid, nodes.ToArray());
		}

		private RuleInstance matchGroup()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_group)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected group token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_editor || tok.Type == NodeType.Tok_StringLit)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						nodes.Add(matchEditor());
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Group, nodes.ToArray());
		}

		private RuleInstance matchSide()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_side)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected side token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_dispinfo || tok.Type == NodeType.Tok_StringLit)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						nodes.Add(matchDispInfo());
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Side, nodes.ToArray());
		}

		private RuleInstance matchDispInfo()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_dispinfo)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected dispinfo token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_allowed_verts || tok.Type == NodeType.Tok_alphas || tok.Type == NodeType.Tok_distances || tok.Type == NodeType.Tok_normals || tok.Type == NodeType.Tok_offset_normals || tok.Type == NodeType.Tok_offsets || tok.Type == NodeType.Tok_StringLit || tok.Type == NodeType.Tok_triangle_tags)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						if (fetchToken(laOffset).Type == NodeType.Tok_normals)
						{
							nodes.Add(matchNormals());
						}
						else
						{
							if (fetchToken(laOffset).Type == NodeType.Tok_distances)
							{
								nodes.Add(matchDistances());
							}
							else
							{
								if (fetchToken(laOffset).Type == NodeType.Tok_offsets)
								{
									nodes.Add(matchOffsets());
								}
								else
								{
									if (fetchToken(laOffset).Type == NodeType.Tok_offset_normals)
									{
										nodes.Add(matchOffset_Normals());
									}
									else
									{
										if (fetchToken(laOffset).Type == NodeType.Tok_alphas)
										{
											nodes.Add(matchAlphas());
										}
										else
										{
											if (fetchToken(laOffset).Type == NodeType.Tok_triangle_tags)
											{
												nodes.Add(matchTriangle_Tags());
											}
											else
											{
												nodes.Add(matchAllowed_Verts());
											}
										}
									}
								}
							}
						}
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_DispInfo, nodes.ToArray());
		}

		private RuleInstance matchNormals()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_normals)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected normals token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Normals, nodes.ToArray());
		}

		private RuleInstance matchDistances()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_distances)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected distances token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Distances, nodes.ToArray());
		}

		private RuleInstance matchOffsets()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_offsets)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected offsets token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Offsets, nodes.ToArray());
		}

		private RuleInstance matchOffset_Normals()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_offset_normals)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected offset_normals token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Offset_Normals, nodes.ToArray());
		}

		private RuleInstance matchAlphas()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_alphas)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected alphas token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Alphas, nodes.ToArray());
		}

		private RuleInstance matchTriangle_Tags()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_triangle_tags)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected triangle_tags token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Triangle_Tags, nodes.ToArray());
		}

		private RuleInstance matchAllowed_Verts()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_allowed_verts)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected allowed_verts token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Allowed_Verts, nodes.ToArray());
		}

		private RuleInstance matchEditor()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_editor)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected editor token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Editor, nodes.ToArray());
		}

		private RuleInstance matchEntity()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_entity)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected entity token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if ((tok = fetchToken(laOffset)).Type == NodeType.Tok_connections || tok.Type == NodeType.Tok_editor || tok.Type == NodeType.Tok_hidden || tok.Type == NodeType.Tok_solid || tok.Type == NodeType.Tok_StringLit)
				{
					if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
					{
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
						if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
							throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
						nodes.Add(tok);
					}
					else
					{
						if (fetchToken(laOffset).Type == NodeType.Tok_connections)
						{
							nodes.Add(matchConnections());
						}
						else
						{
							if (fetchToken(laOffset).Type == NodeType.Tok_solid)
							{
								nodes.Add(matchSolid());
							}
							else
							{
								if (fetchToken(laOffset).Type == NodeType.Tok_hidden)
								{
									nodes.Add(matchHiddenSolid());
								}
								else
								{
									nodes.Add(matchEditor());
								}
							}
						}
					}
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Entity, nodes.ToArray());
		}

		private RuleInstance matchHiddenEntity()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_hidden)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected hidden token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			nodes.Add(matchEntity());
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_HiddenEntity, nodes.ToArray());
		}

		private RuleInstance matchConnections()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_connections)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected connections token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Connections, nodes.ToArray());
		}

		private RuleInstance matchCameras()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_cameras)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected cameras token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_camera)
				{
					nodes.Add(matchCamera());
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Cameras, nodes.ToArray());
		}

		private RuleInstance matchCamera()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_camera)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected camera token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Camera, nodes.ToArray());
		}

		private RuleInstance matchCordon()
		{
			List<Node> nodes = new List<Node>();
			Token tok;

			if ((tok = nextToken()).Type != NodeType.Tok_cordon)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected cordon token");
			nodes.Add(tok);
			if ((tok = nextToken()).Type != NodeType.Tok_LBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected LBrace token");
			nodes.Add(tok);
			while (true)
			{
				if (fetchToken(laOffset).Type == NodeType.Tok_StringLit)
				{
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
					if ((tok = nextToken()).Type != NodeType.Tok_StringLit)
						throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected StringLit token");
					nodes.Add(tok);
				}
				else
					break;
			}
			if ((tok = nextToken()).Type != NodeType.Tok_RBrace)
				throw new ParserException("Line "+tok.LineNr+", Char "+tok.CharNr+": Expected RBrace token");
			nodes.Add(tok);

			return new RuleInstance(NodeType.Rule_Cordon, nodes.ToArray());
		}
	}
}
