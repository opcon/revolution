// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|

using System.IO;

namespace Revolution.Core
{


	public class ASCIIFileHelper
	{
		
		protected static FileStream m_File;
		protected static StreamReader m_Reader;
		protected static StreamWriter m_Writer;
		
		public static void ReadStringToEnd (string f, out string result)
		{
			
			
			m_File = File.OpenRead (f);			
			m_Reader = new StreamReader (m_File);
			
			result = m_Reader.ReadToEnd ();
			
			m_Reader.Close ();
		}
		
		public static void WriteString (string f, string input)
		{
			m_File = File.OpenWrite (f);
			m_Writer = new StreamWriter (m_File);
			
			m_Writer.Write (input);
			
			m_Writer.Close ();
		}
		
		
		//TODO:Write a method for loading a string from a path, and return the string and a FileBase object.
//		public void ReadStringToEnd(out string result)
//		{
//			
//			
//		}
	}
}
