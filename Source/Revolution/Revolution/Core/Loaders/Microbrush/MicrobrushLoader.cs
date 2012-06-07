using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Shrinker.Microbrush2;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Revolution.Core.Loaders.Microbrush
{
    public static class MicrobrushLoader
    {
        public static Scene LoadScene(string file)
        {
            Scene scene = null;
            if (Path.GetExtension(file) == ".dat")
            {
                scene = MB2DatFormat.Load(file);
            }
            else if (Path.GetExtension(file) == ".xml")
            {
                scene = MB2XmlFormat.Load(file);
            }
            return scene;
        }

        public static Vector3[] GetPlanePoints(Plane p)
        {
            var ret = new Vector3[p.OptionalPolygonCorners.Count()];
            for (int i = 0; i < p.OptionalPolygonCorners.Count(); i++)
            {
                ret[i] = new Vector3(p.OptionalPolygonCorners[i].X, p.OptionalPolygonCorners[i].Y, p.OptionalPolygonCorners[i].Z);
            }
            return ret;
        }

        public static List<Polygon> GetPolygons(Brush b)
        {
            var ret = new List<Polygon>();
            foreach (var p in b.Planes)
            {
                ret.Add(new Polygon(GetPlanePoints(p)));
            }
            return ret;
        }
    }
}
