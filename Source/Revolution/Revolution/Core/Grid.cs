// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|

using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Revolution.Core
{
	public class Grid
	{
		
		private int m_GridSize;		// Size of our display grid in OpenGL units (has equal sides).
		public int GridSize { get { return m_GridSize; } set { m_GridSize = value; } }
		private int m_GridDetail;	// In (x) grid units, how many lines would we like to display?
		public int GridDetail { get { return m_GridDetail; } set { m_GridDetail = value; } }
		private Color m_GridColor;
		public Color GridColor { get { return m_GridColor; } set { m_GridColor = value; } }
		
		//private VBO m_RenderData;
		//private VertexP3CNet[] m_Data1;
		
		
		
		public Grid ()
		{
			GridDetail = 100;
			GridSize = 100;
            GridColor = Color.White;
		}
		
		public Grid(int gridSize, int gridDetail)
		{
			GridSize = gridSize;
			GridDetail = gridDetail;
		}
		
		public void Render(double time)
		{
			RenderGrid(0, 0);
		}


	    public void RenderGrid(double xOffset, double zOffset)
		{
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
			GL.Begin(BeginMode.Lines);
            GL.Color3(GridColor);

//            for (int gs = -GridSize; gs < 0; gs += (GridSize / GridDetail))
//            {
//                GL.Vertex3(-GridSize, 0.0f, (float)gs);
//                GL.Vertex3(GridSize, 0.0f, (float)gs);
//
//            }
//
//            for (int gs = 1; gs <= GridSize; gs += (GridSize / GridDetail))
//            {
//                GL.Vertex3(-GridSize, 0.0f, (float)gs);
//                GL.Vertex3(GridSize, 0.0f, (float)gs);
//
//            }
//
//            for (int gs = -GridSize; gs < 0; gs += (GridSize / GridDetail))
//            {
//
//                GL.Vertex3((float)gs, 0.0f, -GridSize);
//                GL.Vertex3((float)gs, 0.0f, GridSize);
//
//            }

//            for (int gs = 1; gs <= GridSize; gs += (GridSize / GridDetail))
//            {
//
//                GL.Vertex3((float)gs, 0.0f, -GridSize);
//                GL.Vertex3((float)gs, 0.0f, GridSize);
//
//            }

            //Draw X Line Red
            GL.Color3(Color.Red);
            GL.Vertex3(GridSize, 0, 0);
            GL.Vertex3(0, 0, 0);

            //Draw Z Line Green
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, GridSize);
            GL.Vertex3(0, 0, 0);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(-GridSize, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -GridSize);

            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, GridSize, 0);
			GL.End();
		}
    }
	
	
}



