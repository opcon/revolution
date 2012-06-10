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
            //GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 1.0f);
            //GL.Light(LightName.Light0, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
            //GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            //GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            //GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.CornflowerBlue);

            //GL.ShadeModel(ShadingModel.Smooth);
            GameCamera = new QuaternionCamera(Mouse, Keyboard, this, new Vector3(0, 5, 0),Quaternion.Identity);
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
            MBTest =
                MicrobrushLoader.GetPolygons(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\complexeTest.mb2.dat").Brushes[0]);
            TestScene = new MicrobrushScene(MicrobrushLoader.GetBrushes(MicrobrushLoader.LoadScene(Directories.MapsDirectory + @"\ka_Building.mb2.dat")));
            //foreach (var b in TestScene.Brushes)
            //{
            //    foreach (var p in b.Polygons)
            //    {
            //       if (p.Points.Length == 5)
            //       {
            //           PolysToTriangulate.Add(p);
            //       }
            //    }
            //}

            //foreach (var p in PolysToTriangulate)
            //{
            //    List<Poly2Tri.PolygonPoint> points = new List<PolygonPoint>();
            //    var a = p.Points[1] - p.Points[0];
            //    var b = p.Points[2] - p.Points[0];
            //    var normal = Vector3.Cross(a, b);
            //    normal.Normalize();
            //    a.Normalize();
            //    var dot = Vector3.Dot(a, Vector3.UnitX);
            //    var angle = Math.Acos(dot);
            //    var xang = Math.Acos(Vector3.Dot(Vector3.UnitX, normal));
            //    var yang = Math.Acos(Vector3.Dot(Vector3.UnitY, normal));
            //    var xrot = Matrix4.CreateFromAxisAngle(Vector3.UnitX, (float)xang);
            //    var yrot = Matrix4.CreateFromAxisAngle(Vector3.UnitY, (float) yang);
                
            //    //var src = new Matrix4(new Vector4(a), new Vector4(up), new Vector4(Vector3.Dot(a, up)), Vector4.Zero);
            //    //var dest = new Matrix4(Vector4.UnitX, Vector4.UnitY,
            //    //                       new Vector4(Vector3.Cross(Vector3.UnitX, Vector3.UnitY)), Vector4.Zero);
            //    //src.Invert();
            //    var rot = Matrix4.CreateFromAxisAngle(Vector3.Cross(a, normal), (float)angle);
            //    rot = xrot*yrot;
            //    var newPoints = new List<Vector3>();
            //    foreach (var point in p.Points)
            //    {
            //        //v = Vector4.Transform(v, projection);
            //        //var v = Vector3.Transform(point, xrot);
            //        //v = Vector3.Transform(v, yrot);
            //        var v = Vector3.Transform(point, rot);
            //        //points.Add(new PolygonPoint(v.X, v.Y));
            //        newPoints.Add(v);
            //    }
            //    p.Points = newPoints.ToArray();
            //    var invRot = Matrix4.Invert(rot);

            //    var yval = p.Points[0].Y;

            //    foreach (var point in p.Points)
            //    {
            //       points.Add(new PolygonPoint(point.X, point.Z));
            //    }
            //    Poly2Tri.Polygon polys = new Poly2Tri.Polygon(points);
            //    Poly2Tri.P2T.Triangulate(polys);
            //    foreach (var tri in polys.Triangles)
            //    {
            //        Vector3[] triPoints = new Vector3[3];
            //        for (int i = 0; i < 3; i++)
            //        {
            //            var triPoint = new Vector3();
            //            triPoint.X = (float)tri.Points[i].X;
            //            triPoint.Y = yval;
            //            triPoint.Z = (float)tri.Points[i].Y;
            //            triPoints[i] = Vector3.Transform(triPoint, invRot);
            //        }

            //        triPolys.Add(new Polygon(triPoints));
            //    }
            //}
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
            //GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(GameCamera.Position, 1));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GameCamera.GetModelviewMatrix(out modelview);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            //GL.Enable(EnableCap.Texture2D);
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
            
            GL.Enable(EnableCap.Lighting);
            GL.Begin(BeginMode.Triangles);
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
            //foreach (var poly in MBTest)
            //{
            //    poly.Draw(e.Time);
            //}
            TestScene.Draw(e.Time);
            GL.End();
            GL.LineWidth(5);
            GL.Color3(Color.DarkGreen);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(GameCamera.Position);
            //foreach (var polygon in triPolys)
            //{
            //    polygon.Draw(e.Time);
            //}
            GL.End();
            GL.LineWidth(1);
            GL.Color3(Color.Black);
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