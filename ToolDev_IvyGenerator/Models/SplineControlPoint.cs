using SharpDX;
using System.ComponentModel;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator.Models
{
    public class SplineControlPoint : INotifyPropertyChanged
    {
        private int number;
        public int ID { get; }
        public Vec3 Position { get; set; }
        public Vec3 Tangent { get; set; }

        public SplineControlPoint(int number, Vector3 pos, Vector3 tan)
        {
            this.number = number;
            Position = new Vec3(pos);
            Tangent = new Vec3(tan - pos);
        }

        public override string ToString()
        {
            return "CP: " + number.ToString() + " Position: " + Position.ToString() + " Tangent: " + Tangent.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
