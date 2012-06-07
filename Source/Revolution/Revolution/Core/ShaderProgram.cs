// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|


using OpenTK.Graphics.OpenGL;

namespace Revolution.Core
{

	
	public class ShaderProgram
	{
		
		//Get Property for Program ID
		public int ProgramID
		{
			get
			{
				return programID;
			}
		}		
		protected int programID;

		public bool IsinUse
		{
			get
			{
				return isinUse;
			}
		}
		protected bool isinUse;
		
		public ShaderBase Fragment, Vertex;
		
		public ShaderProgram ()
		{
			programID = GL.CreateProgram();
		}
		
		public ShaderProgram(string vert, string frag, bool v)
		{
			programID = GL.CreateProgram();
			Vertex = new ShaderBase(vert, v);
			Fragment = new ShaderBase(frag, v);
			Vertex.CompileShader();
			Fragment.CompileShader();
			AttachShaders(Vertex, Fragment);
		}
		
		/// <summary>
		/// Attaches a Shader
		/// </summary>
		/// <param name="Shader">
		/// A Geometry or Fragment Shader Object
		/// </param>
		public void AttachShader (ShaderBase Shader)
		{
			GL.AttachShader(programID, Shader.ShaderID);
			Shader.IsAttached = true;
			Shader.ParentProgram = this;
		}
		
		public void AttachShaders(ShaderBase Vert, ShaderBase Frag)
		{
			AttachShader(Vert);
			AttachShader(Frag);
		}
		
		/// <summary>
		/// Links a Program
		/// </summary>
		public void LinkProgram()
		{
			GL.LinkProgram(programID);
		}
		
		/// <summary>
		/// Sets this program to be used in the current rendering pipeline
		/// </summary>
		public void Activate()
		{
			GL.UseProgram(programID);
			isinUse = true;
		}
		
		/// <summary>
		/// Detaches a Shader
		/// </summary>
		/// <param name="Shader">
		/// A Geometry or Fragment Shader object
		/// </param>
		public void DetachShader(ShaderBase Shader)
		{
			GL.DetachShader(this.ProgramID, Shader.ShaderID);
		}
		
		/// <summary>
		/// Returns shaders attached to this program object
		/// </summary>
		/// <param name="maxCount">
		/// The size of the array (set this around 20)
		/// </param>
		/// <returns>
		/// An index of all the shaders associated with this program object
		/// </returns>
		public int GetShaders(int maxCount)
		{
			int count = 0;
			int Shaders = 0;
			GL.GetAttachedShaders(this.ProgramID, maxCount, out count, out Shaders);
			return Shaders;
		}
		
	}
}
