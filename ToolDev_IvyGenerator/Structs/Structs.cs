using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace ToolDev_IvyGenerator.Structs
{
    public struct SplineControlPoint
    {
        public Vector3 Position { get; set; }
        public Vector3 Tangent { get; set; }

        public SplineControlPoint(Vector3 pos, Vector3 tan)
        {
            Position = pos;
            Tangent = tan;
        }
    }
}
