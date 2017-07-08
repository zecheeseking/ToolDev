using System;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

namespace IvyGenerator.Utilities
{
    class ObjExporter
    {
        public static string MeshToString(string s, MeshGeometry3D meshGeometry3D, Transform3D transform)
        {
            StringBuilder meshString = new StringBuilder();

            //meshString.Append("#" + s + ".obj"
            //                  + "\n#" + System.DateTime.Now.ToLongDateString()
            //                  + "\n#" + System.DateTime.Now.ToLongTimeString()
            //                  + "\n#-------"
            //                  + "\n\n");

            //meshString.Append("# vertices position data\n");
            foreach (var v in meshGeometry3D.Positions)
            {
                var pt = transform.Transform(v.ToPoint3D());
                meshString.Append(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}\n", pt.X, pt.Y, pt.Z));
            }

            meshString.Append("# vertices normal data\n");
            foreach (var vn in meshGeometry3D.Normals)
            {
                var pt = transform.Transform(vn.ToPoint3D());
                meshString.Append(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}\n", pt.X, pt.Y, pt.Z));
            }

            //meshString.Append("# vertices indices data\n");
            for (int i = 0; i < meshGeometry3D.Indices.Count; i += 3)
                meshString.Append("f " + (meshGeometry3D.Indices[i] + 1) + " "
                                  + (meshGeometry3D.Indices[i + 1] + 1) + " "
                                  + (meshGeometry3D.Indices[i + 2] + 1) + "\n");

            return meshString.ToString();
        }
    }
}
