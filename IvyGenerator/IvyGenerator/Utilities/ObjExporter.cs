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
        public static string MeshesToString(string s, MeshGeometry3D[] meshes, Transform3D transform)
        {
            StringBuilder meshString = new StringBuilder();

            //meshString.Append("#" + s + ".obj"
            //                  + "\n#" + System.DateTime.Now.ToLongDateString()
            //                  + "\n#" + System.DateTime.Now.ToLongTimeString()
            //                  + "\n#-------"
            //                  + "\n\n");
            int iOffset = 0;
            int meshIndex = 0;
            foreach (var meshGeometry3D in meshes)
            {
                meshString.Append("o mesh\n" + meshIndex);
                meshString.Append("#vertices position data\n");
                foreach (var v in meshGeometry3D.Positions)
                {
                    var pt = transform.Transform(v.ToPoint3D());
                    meshString.Append(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}\n", pt.X, pt.Y, pt.Z));
                }

                meshString.Append("#vertices UV data\n");
                foreach (var uv in meshGeometry3D.TextureCoordinates)
                {
                    meshString.Append(string.Format(CultureInfo.InvariantCulture, "vt {0} {1} 0.000000\n", uv.X, uv.Y));
                }

                meshString.Append("#vertices normal data\n");
                foreach (var vn in meshGeometry3D.Normals)
                {
                    var pn = transform.Transform(vn.ToPoint3D());
                    meshString.Append(string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}\n", pn.X, pn.Y,
                        pn.Z));
                }

                meshString.Append("#vertices indices data\n");
                int lastIndex = 0;
                for (int i = 0; i < meshGeometry3D.Indices.Count; i += 3)
                {
                    meshString.Append("f " 
                        + (iOffset + meshGeometry3D.Indices[i] + 1) + "/" + (iOffset + meshGeometry3D.Indices[i] + 1) + " "
                        + (iOffset + meshGeometry3D.Indices[i + 1] + 1) + "/" + (iOffset + meshGeometry3D.Indices[i + 1] + 1) + " "
                        + (iOffset + meshGeometry3D.Indices[i + 2] + 1) + "/" + (iOffset + meshGeometry3D.Indices[i + 2] + 1) + "\n");
                    lastIndex = Math.Max(lastIndex, Math.Max(meshGeometry3D.Indices[i] + 1, Math.Max(meshGeometry3D.Indices[i + 1] + 1, meshGeometry3D.Indices[i + 2] + 1)));
                }

                meshString.Append("\n");

                iOffset = lastIndex;
                meshIndex++;
            }


            return meshString.ToString();
        }
    }
}
