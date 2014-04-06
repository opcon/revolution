﻿// Released to the public domain. Use, modify and relicense at will.

using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using Poly2Tri;
using Revolution.Core;
using Revolution.Core.Loaders;
using Revolution.Core.Loaders.Microbrush;
using Polygon = Revolution.Core.Polygon;
using BEPUphysics;
using Revolution.Core.AdvancedMovement;

namespace Revolution
{
	class Game : GameWindow
	{
		private Matrix4 modelview, projection;
		private Grid grid;
		private MicrobrushScene TestScene;
		Space physicsSpace;
		internal static bool focused = true, fullscreen;
		internal static int WindowHeight, WindowWidth;
		private List<Polygon> PolysToTriangulate = new List<Polygon>();
		private List<Polygon> triPolys = new List<Polygon>();
		private CollisionPacket collisionTest = new CollisionPacket();
		private Player GamePlayer;
		Camera gameCamera;
		KeyboardState prevKeyboardState = new KeyboardState();
		bool rotated = false;

		protected Point WindowCenter
		{ get { return new Point((ClientRectangle.Left + ClientRectangle.Right) / 2, (ClientRectangle.Top + ClientRectangle.Bottom) / 2); } }

		/// <summary>Creates a 1600*900 window with the specified title.</summary>
		public Game()
            : base(800, 600, GraphicsMode.Default, "Revolution")
		{
			VSync = VSyncMode.On;
		}

		/// <summary>Load resources here.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad(EventArgs e)
		{
			//his.WindowState = OpenTK.WindowState.Fullscreen;

			base.OnLoad(e);
			GL.Light(LightName.Light0, LightParameter.Position, new float[] { 1.0f, 1.0f, -0.5f });
			GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
			GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1f, 1f, 1f, 1.0f });
			GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1f, 1f, 1f, 1.0f });
			GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 0.5f);
			GL.Light(LightName.Light0, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
			GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
			//GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Enable(EnableCap.DepthTest);

			GL.ClearColor(Color.CornflowerBlue);

			Keyboard.KeyDown += (o, args) => InputSystem.KeyDown(args);
			Keyboard.KeyUp += (o, args) => InputSystem.KeyUp(args);
			Mouse.ButtonDown += (o, args) => InputSystem.MouseDown(args);
			Mouse.ButtonUp += (o, args) => InputSystem.MouseUp(args);
			Mouse.WheelChanged += (o, args) => InputSystem.MouseWheelChanged(args);
			Mouse.Move += (o, args) => InputSystem.MouseMoved(args);

			grid = new Grid();
			TestScene = new MicrobrushScene(MicrobrushLoader.GetBrushes(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\groundTest.mb2.dat")));
			TestScene.Triangulate();

			gameCamera = new Camera(new Vector3(5, 5, 2), 0, 0, BEPUutilities.Matrix.CreatePerspectiveFieldOfViewRH(MathHelper.PiOver4, ClientRectangle.Width / (float)ClientRectangle.Height, .1f, 10000));

			physicsSpace = new Space();
			physicsSpace.ForceUpdater.Gravity = new Vector3(0f, -9.8f, 0f);

            var bph = new BEPUphysics.BroadPhaseSystems.SortAndSweep.SortAndSweep1D();
            physicsSpace.BroadPhase = bph;
            bph.Enabled = true;

			TestScene.AddBrushesToPhysicsScene(physicsSpace);

			GamePlayer = new Player(gameCamera, physicsSpace);

            CursorVisible = false;

		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (Focused)
				InputSystem.KeyPressed(e);
			base.OnKeyPress(e);
		}

		public static Vertex[] CalculateVertices2(float radius, float height, byte segments, byte rings)
		{
			var data = new Vertex[segments * rings];

			int i = 0;

			for (double y = 0; y < rings; y++) {
				double phi = (y / (rings - 1)) * Math.PI; //was /2 
				for (double x = 0; x < segments; x++) {
					double theta = (x / (segments - 1)) * 2 * Math.PI;

					Vector3 v = new Vector3() {
						X = (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
						Y = (float)(height * Math.Cos(phi)),
						Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta)),
					};
					Vector3 n = Vector3.Normalize(v);
					Vector2 uv = new Vector2() {
						X = (float)(x / (segments - 1)),
						Y = (float)(y / (rings - 1))
					};
					// Using data[i++] causes i to be incremented multiple times in Mono 2.2 (bug #479506).
					data[i] = new Vertex() { Position = v, Normal = n, TexCoord = uv };
					i++;
				}

			}

			return data;
		}

		public static ushort[] CalculateElements(float radius, float height, byte segments, byte rings)
		{
			var num_vertices = segments * rings;
			var data = new ushort[num_vertices * 6];

			ushort i = 0;

			for (byte y = 0; y < rings - 1; y++) {
				for (byte x = 0; x < segments - 1; x++) {
					data[i++] = (ushort)((y + 0) * segments + x);
					data[i++] = (ushort)((y + 1) * segments + x);
					data[i++] = (ushort)((y + 1) * segments + x + 1);

					data[i++] = (ushort)((y + 1) * segments + x + 1);
					data[i++] = (ushort)((y + 0) * segments + x + 1);
					data[i++] = (ushort)((y + 0) * segments + x);
				}
			}

			// Verify that we don't access any vertices out of bounds:
			foreach (int index in data)
				if (index >= segments * rings)
					throw new IndexOutOfRangeException();

			return data;
		}

		public struct Vertex
		{
			// mimic InterleavedArrayFormat.T2fN3fV3f
			public Vector2 TexCoord;
			public Vector3 Normal;
			public Vector3 Position;
		}

		/// <summary>
		/// Called when your window is resized. Set your viewport here. It is also
		/// a good place to set up your projection matrix (which probably changes
		/// along when the aspect ratio of your window).
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			WindowHeight = this.Height;
			WindowWidth = this.Width;

			fullscreen = this.WindowState == OpenTK.WindowState.Fullscreen;

			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView((float)(90 * Math.PI / 180.0), (float)Math.Round((double)(ClientRectangle.Width/ClientRectangle.Height), 4), 0.1f, 64000f);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);



		}

		/// <summary>
		/// Called when it is time to setup the next frame. Add you game logic here.
		/// </summary>
		/// <param name="e">Contains timing information for framerate independent logic.</param>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			if (Keyboard[Key.Escape])
				Exit();
			if (InputSystem.NewKeys.Contains(Key.Number1)) {
				GamePlayer.playerController.CharacterController.Down *= -1;
                //GamePlayer.playerController.CharacterController.Body.GravityRotation = BEPUutilities.Matrix3x3.CreateFromAxisAngle(Vector3.UnitX, (!rotated) ? MathHelper.Pi : 0);
				gameCamera.LockedUp *= -1;
				rotated = !rotated;
			}

			if (InputSystem.CurrentKeys.Contains(Key.Number2)) {
				gameCamera.RotateToNewLockedUp(new Vector3(0, -1, 0));
                
			}
			if (InputSystem.CurrentKeys.Contains(Key.Number3))
				gameCamera.RotateToNewLockedUp(new Vector3(0, 1, 0));

			physicsSpace.Update((float)e.Time);

			GamePlayer.Update(e.Time);

			ResetMouse();
			InputSystem.Update();
		}

		/// <summary>
		/// Called when it is time to render the next frame. Add your rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			//GL.Enable(EnableCap.Normalize);
			//GL.Enable(EnableCap.Lighting);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			modelview = gameCamera.ViewMatrix;

			//GL.Enable(EnableCap.CullFace);
			GL.Disable(EnableCap.CullFace);
			//GL.CullFace(CullFaceMode.Back);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0, 0, 0, 1));

			//GL.Translate(-GamePlayer.CameraPos);
			//GL.Translate(-GamePlayer.CameraOffset);
			float radius = 1;
			float height = 1;
			byte segments = 50;
			byte rings = 50;
			var vertices = CalculateVertices2(radius, height, segments, rings);
			var elements = CalculateElements(radius, height, segments, rings);

			//GL.Translate(0, -10, 0);
			//GL.Begin(BeginMode.Triangles);
			//for (int index = elements.Length - 1; index >= 0; index--)
			//{
			//    var element = elements[index];
			//    var vertex = vertices[element];
			//    GL.TexCoord2(vertex.TexCoord);
			//    GL.Normal3(vertex.Normal);
			//    GL.Vertex3(vertex.Position);
			//}
			//GL.End();

			GL.LoadMatrix(ref modelview);
            
			grid.Render(e.Time);



			GL.Enable(EnableCap.Light0);
			GL.Color3(Color.White);
			GL.Enable(EnableCap.Lighting);
			//GL.Translate(0, 10, 0);
			GL.Begin(BeginMode.Triangles);

			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
			//GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			//GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
			//GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });

			TestScene.Draw(e.Time);
			GL.End();
			GL.Color3(Color.Black);

            
			SwapBuffers();
		}

		public void ResetMouse()
		{
			if (fullscreen) {
				System.Windows.Forms.Cursor.Position = WindowCenter;
			}
			else {
				System.Windows.Forms.Cursor.Position = PointToScreen(WindowCenter);
			}

		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Game application = new Game()) {
				application.Run(30.0);
			}
		}
	}
}