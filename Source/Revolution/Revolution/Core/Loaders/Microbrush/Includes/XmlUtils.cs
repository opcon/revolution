//Copyright © 2008-2010 Max 'Shrinker' Wieden
//Refer to license.txt for license details.
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Shrinker
{
	class XmlUtils
	{
		public static XmlDocument LoadXmlDocument(string xmlPath)
		{
			XmlDocument d = new XmlDocument();
			d.Load(xmlPath);
			return d;
		}

		public static XmlDocument LoadXmlDocument(string xmlPath, string xsdPath)
		{
			XmlReader schemaReader = XmlReader.Create(xsdPath);
			XmlSchemaSet schemas = new XmlSchemaSet();
			ValidationEventHandler handler = new ValidationEventHandler(validationEventHandler);
			XmlSchema schema = XmlSchema.Read(schemaReader, handler);
			schemas.Add(schema);
			schemaReader.Close();
			XmlDocument d = new XmlDocument();
			d.Load(xmlPath);
			d.Schemas = schemas;
			d.Validate(handler);
			return d;
		}

		public static XmlElement FindFirstChild(XmlNode node)
		{
			return FindFirstChild(node, null);
		}

		public static XmlElement FindNextSibling(XmlNode node)
		{
			return FindNextSibling(node, null);
		}

		public static XmlElement[] FindAllChildren(XmlNode node)
		{
			return FindAllChildren(node, null);
		}

		public static XmlElement FindFirstChild(XmlNode node, string name)
		{
			foreach (XmlNode node2 in node.ChildNodes)
				if (node2.NodeType == XmlNodeType.Element && (name == null || ((XmlElement)node2).Name.Equals(name)))
					return (XmlElement)node2;
			return null;
		}

		public static XmlElement FindNextSibling(XmlNode node, string name)
		{
			node = node.NextSibling;
			while (node != null)
			{
				if (node.NodeType == XmlNodeType.Element && (name == null || ((XmlElement)node).Name.Equals(name)))
					return (XmlElement)node;
				node = node.NextSibling;
			}
			return null;
		}

		public static XmlElement[] FindAllChildren(XmlNode node, string name)
		{
			List<XmlElement> elems = new List<XmlElement>();
			XmlElement elem = FindFirstChild(node, name);
			while (elem != null)
			{
				elems.Add(elem);
				elem = FindNextSibling(elem, name);
			}
			return elems.ToArray();
		}

		public static XmlElement MakeNewChild(XmlElement elem, string name)
		{
			XmlElement elem2 = elem.OwnerDocument.CreateElement(name);
			elem.AppendChild(elem2);
			return elem2;
		}

		private static void validationEventHandler(Object o, ValidationEventArgs args)
		{
			throw args.Exception;
		}
	}
}
