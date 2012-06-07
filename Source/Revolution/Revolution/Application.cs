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
using Revolution.Core;
using Revolution.Core.Loaders;
using Revolution.Core.Loaders.Microbrush;

namespace Revolution
{
    class Game : GameWindow
    {
        private QuaternionCamera GameCamera;
        private Matrix4 modelview, projection;
        private Grid grid;
        private List<Polygon> MBTest;

        internal static bool focused = true;
        internal static int WindowHeight, WindowWidth;
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

            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GameCamera = new QuaternionCamera(Mouse, Keyboard, this, new Vector3(0, 5, 0),Quaternion.Identity);
            GameCamera.SetCameraMode(CamMode.FlightCamera);
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
            MBTest =
                MicrobrushLoader.GetPolygons(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\boxTest.mb2.xml").Brushes[0]);
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

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GameCamera.GetModelviewMatrix(out modelview);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            grid.Render(e.Time);
            GL.Begin(BeginMode.Triangles);

            //GL.Color3(Color.Purple);
            //GL.Vertex3(-1.0f, -1.0f, 4.0f);
            //GL.Color3(Color.Violet);
            //GL.Vertex3(1.0f, -1.0f, 4.0f);
            //GL.Color3(Color.RoyalBlue);
            //GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.End();

            GL.Color3(Color.Black);
            GL.PointSize(5);
            GL.Begin(BeginMode.Points);
            foreach (var poly in MBTest)
            {
                poly.Draw(e.Time);
            }
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