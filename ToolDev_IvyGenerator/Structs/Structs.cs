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
        private int number;
        public Vector3 Position { get; set; }
        public Vector3 Tangent { get; set; }

        public SplineControlPoint(int number, Vector3 pos, Vector3 tan)
        {
            this.number = number;
            Position = pos;
            Tangent = tan - pos;
        }

        public override string ToString()
        {
            return "CP: " + number.ToString() + " Position: " + Position.ToString() + " Tangent: " + Tangent.ToString();
        }
    }
}
