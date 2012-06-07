using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace Revolution
{
    public class Directories
    {
        private static readonly string Resources = @"..\..\Resources";
        private static readonly string Libraries = @"\Libraries";
        private static readonly string Tiles = @"\Tiles";
        private static readonly string Entities = @"\Entities";
        private static readonly string Worlds = @"\Worlds";
        private static readonly string Maps = @"\Maps";
        private static readonly string UI = @"\UI";
        public static readonly string TextFile = @"\Comfortaa-Regular.ttf";

        public static DirectoryInfo ResourcesDirectory,
                                    LibrariesDirectory,
                                    TilesDirectory,
                                    EntitiesDirectory,
                                    WorldsDirectory,
                                    MapsDirectory,
                                    UIDirectory;

        static Directories()
        {
            FixPathSeparators(ref Resources);
            FixPathSeparators(ref Libraries);
            FixPathSeparators(ref Tiles);
            FixPathSeparators(ref Entities);
            FixPathSeparators(ref Worlds);
            FixPathSeparators(ref Maps);
            FixPathSeparators(ref UI);

            ResourcesDirectory = new DirectoryInfo(Resources);
            LibrariesDirectory = new DirectoryInfo(Resources + Libraries);
            TilesDirectory = new DirectoryInfo(Resources + Tiles);
            EntitiesDirectory = new DirectoryInfo(Resources + Entities);
            WorldsDirectory = new DirectoryInfo(Resources + Worlds);
            MapsDirectory = new DirectoryInfo(Resources + Maps);
            UIDirectory = new DirectoryInfo(Resources + UI);
        }

        private static void FixPathSeparators(ref string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
        }
    }
    public static class Utilities
    {
        private static Random m_Random;

        public static Random RandomGenerator
        {
            get { return m_Random ?? (m_Random = new Random()); }
        }

        public static List<Vector2> StringToVecList(IEnumerable<string> bcoords)
        {
            try
            {
                return bcoords.Select(s => Array.ConvertAll(s.Split(','), str => (Single.Parse(str)))).Select(
            point => new Vector2(point[0], point[1])).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static string VecListToString(List<Vector2> positions)
        {
            string s = "";
            foreach (var p in positions)
            {
                s += String.Format("{0},{1} ", p.X, p.Y);
            }
            return s.Trim();
        }

        // Handles IPv4 and IPv6 notation.
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            IPAddress ip;
            if (ep.Length == 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid IP Address");
                }
            }
            else if (ep.Length ==1)
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid IP Address");
                }
                return new IPEndPoint(ip, 0);
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid IP Address");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid Port");
            }
            return new IPEndPoint(ip, port);
        }

        public static void TakeScreenShot(string f)
        {
            Bitmap b = ScreenToBitmap();
            b.Save(f, ImageFormat.Png);
        }

        public static void Dummy(bool b) { }

        public static Bitmap ScreenToBitmap()
        {
            Bitmap b = new Bitmap(Game.WindowWidth, Game.WindowHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bd = b.LockBits(new Rectangle(0, 0, Game.WindowWidth, Game.WindowHeight), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.ReadPixels(0, 0, Game.WindowWidth, Game.WindowHeight, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bd.Scan0);

            b.UnlockBits(bd);
            b.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return b;
        }
    }
}