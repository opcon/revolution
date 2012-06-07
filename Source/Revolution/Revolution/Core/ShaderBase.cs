// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|


using System;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Revolution.Core
{


	public class ShaderBase
	{

		//Get Property for Shadersource
		public string Shadersource {
			get { return m_Shadersource; }
		}
		protected string m_Shadersource;

		//Get Property for ShaderName
		public string ShaderName {
			get { return m_Shadername; }
		}
		protected string m_Shadername;

		//Get Property for ShaderID
		public int ShaderID {
			get { return m_ShaderID; }
		}
		protected int m_ShaderID;

		protected static string[] m_ShaderNames = new string[50];

		public static int ShaderAmount {
			get { return m_ShaderAmount; }
		}
		protected static int m_ShaderAmount;

		public bool IsAttached {
			get { return m_IsAttached; }
			set { m_IsAttached = value; }
		}
		protected bool m_IsAttached;

		public ShaderProgram ParentProgram {
			get { return m_ParentProgram; }
			set { m_ParentProgram = value; }
		}
		protected ShaderProgram m_ParentProgram;
		
		public int StatusCode
		{
			get
			{
				return m_StatusCode;
			}
		}
		protected int m_StatusCode;
		
		public string Info
		{
			get 
			{ 
				return m_Info;
			}
		}
		protected string m_Info;
		
		public bool Verbose
		{
			get
			{
				return m_verbose;
			}
			set
			{
				m_verbose = value;
			}
		}
		protected bool m_verbose;

		
		public ShaderBase()
		{
			
		}
		
		/// <summary>
		/// Creates a new shader using the specified source and name
		/// </summary>
		/// <param name="source">
		/// A <see cref="System.String"/> containing the shader source
		/// </param>
		/// <param name="name">
		/// A <see cref="System.String"/> containing the shader name
		/// </param>
		public ShaderBase (string source, string name, ShaderType type, bool v)
		{
			m_verbose = v;
			CreateShader (source, name, type);
		}
		
		public ShaderBase(string path, ShaderType type, bool v)
		{
			m_verbose = v;
			string source;
			ASCIIFileHelper.ReadStringToEnd(path, out source);
			CreateShader (source, System.IO.Path.GetFileNameWithoutExtension(path), type);
		}
		
		public ShaderBase(string path, bool v)
		{
			m_verbose = v;
			string source;
			ASCIIFileHelper.ReadStringToEnd(path, out source);
			string type = Path.GetExtension(path);
			ShaderType sType;
			switch (type) {
			case ".fs":
				sType = ShaderType.FragmentShader;
				break;
			case ".vs":
				sType = ShaderType.VertexShader;
				break;
			default:
				Console.WriteLine("Unknown File Extensions. Shader not Created");
				return;
			}
			CreateShader (source, System.IO.Path.GetFileNameWithoutExtension(path), sType);
		}
		
		/// <summary>
		/// Creates a new shader
		/// </summary>
		/// <param name="source"> A string containg the shader source </param>
		/// <param name="name"> A string containing the shader name</param>
		/// <param name="type"> A ShaderType describing which type of shader to create
		public void CreateShader (string source, string name, ShaderType type)
		{
			SetupVariables (source, name);
			m_ShaderID = GL.CreateShader (type);
			m_ShaderNames[m_ShaderID] = m_Shadername;
			SetSource ();
		}
		
		/// <summary>
		/// Initializes the shader source and name variables, also increments the shader amount.
		/// </summary>
		/// <param name="source">
		/// A <see cref="System.String"/> containing the shader source
		/// </param>
		/// <param name="name">
		/// A <see cref="System.String"/> containing the shader name
		/// </param>
		protected void SetupVariables (string source, string name)
		{
			m_ShaderAmount++;
			m_Shadersource = source;
			m_Shadername = name;
		}


		/// <summary>
		/// Compiles this shader (needs to be called 
		/// each time the Source changes)
		/// </summary>
		public void CompileShader ()
		{
			if (ShaderID > 0)
			{
				GL.CompileShader (m_ShaderID);
			}
				
			else if (ShaderID <= 0)
			{
				Console.WriteLine ("Shader not created");
			}
			
			GetCompileErrors ();
		}
		
		protected void GetCompileErrors ()
		{
			GL.GetShaderInfoLog(ShaderID, out m_Info);
            GL.GetShader(ShaderID, ShaderParameter.CompileStatus, out m_StatusCode);

            if (m_StatusCode != 1)
                throw new ApplicationException(m_Info);
			if (m_verbose && m_StatusCode != 1)
				Console.WriteLine("Shader : " + ShaderName + " Compile Error : " + m_Info);
		}

		/// <summary>
		/// Returns the Shader name
		/// </summary>
		/// <param name="Shader">
		/// The index of the Shader
		/// </param>
		/// <returns>
		/// The name of the Shader
		/// </returns>
		public static string LookupShaderName (ShaderBase Shader)
		{
			return m_ShaderNames[Shader.ShaderID];
			
		}

		/// <summary>
		/// Updates the shaders source code to the specified string and
		/// compiles if compile is true. If the ParentProgram is active
		/// and the shader is attached then it automatically compiles and
		/// attches the shader, and links and makes the program active,
		/// else if the shader is attached but the ParentProgram is not in
		/// use then it will just detach, compile and re-attach the shader.
		/// </summary>
		/// <param name="source">
		/// A <see cref="System.String"/> containing the source
		/// </param>
		/// <param name="compile">
		/// A <see cref="System.Boolean"/> containing wether to compile or not
		/// </param>
		public void UpdateSource (string source, bool compile)
		{
			m_Shadersource = source;
			
//			if (IsAttached && ParentProgram.IsinUse) {
//				ParentProgram.DetachShader (this);
//				SetSource ();
//				CompileShader ();
//				ParentProgram.AttachShader (this);
//				ParentProgram.LinkProgram ();
//				ParentProgram.ActivateProgram ();
//			} else if (IsAttached) {
//				ParentProgram.DetachShader (this);
//				SetSource ();
//				CompileShader ();
//				ParentProgram.AttachShader (this);
//			} else {
				SetSource ();
				if (compile) 
			{
					CompileShader ();
				}
			}
			
			
			

		/// <summary>
		/// Sets this shaders source code to the string Shadersource
		/// </summary>
		protected void SetSource ()
		{
			GL.ShaderSource (this.ShaderID, Shadersource);
		}
		
		public void DeleteShader ()
		{
			GL.DeleteShader (ShaderID);
			m_ShaderNames[ShaderID] = null;
			m_ShaderAmount--;
		}
		
	}
}


