// Released to the public domain. Use, modify and relicense at will.

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

namespace Revolution
{
    class Game : GameWindow
    {
        private QuaternionCamera GameCamera;
        private Matrix4 modelview, projection;
        private Grid grid;
        private List<Polygon> MBTest;
        private MicrobrushScene TestScene;

        internal static bool focused = true;
        internal static int WindowHeight, WindowWidth;
        private List<Polygon> PolysToTriangulate = new List<Polygon>();
        private List<Polygon> triPolys = new List<Polygon>();


        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "Revolution")
        {
            VSync = VSyncMode.On;
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
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

            //GL.ShadeModel(ShadingModel.Flat);
            GameCamera = new QuaternionCamera(Mouse, Keyboard, this, new Vector3(0, 15, 0),Quaternion.Identity);
            GameCamera.SetCameraMode(CamMode.NoClip);
            GameCamera.UpdateParentGame(this);
            GameCamera.MouseXSensitivity = 0.1f;
            GameCamera.MouseYSensitivity = 0.1f;
            GameCamera.ZFar = 64000.0f;
            GameCamera.Speed = 20f;

            Keyboard.KeyDown += (o, args) => InputSystem.KeyDown(args);
            Keyboard.KeyUp += (o, args) => InputSystem.KeyUp(args);
            Mouse.ButtonDown += (o, args) => InputSystem.MouseDown(args);
            Mouse.ButtonUp += (o, args) => InputSystem.MouseUp(args);
            Mouse.WheelChanged += (o, args) => InputSystem.MouseWheelChanged(args);
            Mouse.Move += (o, args) => InputSystem.MouseMoved(args);

            grid = new Grid();
            TestScene = new MicrobrushScene(MicrobrushLoader.GetBrushes(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\PhysicsTest.mb2.dat")));
            TestScene = new MicrobrushScene(MicrobrushLoader.GetBrushes(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\complexeTest.mb2.dat")));
            TestScene.Triangulate();
           
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (Focused)
                InputSystem.KeyPressed(e);
            base.OnKeyPress(e);
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

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            GameCamera.AspectRatio = (float)Math.Round((double)(ClientRectangle.Width / ClientRectangle.Height), 4);

            GameCamera.GetModelviewMatrix(out modelview);
            GameCamera.GetProjectionMatrix(out projection);

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
            GameCamera.Update(e.Time);
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
            GameCamera.GetModelviewMatrix(out modelview);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0, 0, 0, 1));
            GL.LoadMatrix(ref modelview);
            
            grid.Render(e.Time);

            GL.Enable(EnableCap.Light0);
            GL.Color3(Color.White);
            GL.Enable(EnableCap.Lighting);
            GL.Translate(0, 10, 0);
            GL.Begin(BeginMode.Triangles);

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });

            TestScene.Draw(e.Time);
            GL.End();
            GL.Color3(Color.Black);
            GL.Begin(BeginMode.Lines);
            TestScene.DrawNormals(e.Time);
            GL.End();
            SwapBuffers();
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
            using (Game application = new Game())
            {
                application.Run(30.0);
            }
        }
    }
}