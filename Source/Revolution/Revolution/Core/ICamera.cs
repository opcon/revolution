// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|

using OpenTK;

namespace Revolution.Core
{
	/// <summary>
	/// An Interface for Cameras
	/// </summary>
	public interface ICamera
	{
		
		#region Methods
		
		/// <summary>
		/// Updates this camera
		/// </summary>
		/// <param name="time">
		/// A <see cref="System.Double"/> containg the amount of time since the last update
		/// </param>
		void Update(double time);
		
		/// <summary>
		/// Returns the Projection matrix for this camera
		/// </summary>
		/// <param name="matrix">
		/// A <see cref="Matrix4"/> containing the Projection matrix
		/// </param>
		void GetProjectionMatrix(out Matrix4 matrix);
		
		/// <summary>
		/// Returns the Modelview Matrix for this camera
		/// </summary>
		/// <param name="matrix">
		/// A <see cref="Matrix4"/> containing the Modelview Matrix
		/// </param>
		void GetModelviewMatrix(out Matrix4 matrix);
		
		/// <summary>
		/// Returns the Modelview Matrix multiplied by the Projection matrix
		/// </summary>
		/// <param name="result">
		/// A <see cref="Matrix4"/> containg the multiplied matrices
		/// </param>
		void GetModelviewProjectionMatrix(out Matrix4 matrix);
		
		#endregion
		
	}
}

