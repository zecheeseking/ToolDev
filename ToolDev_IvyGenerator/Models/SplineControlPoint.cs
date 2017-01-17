using SharpDX;
using System.ComponentModel;

namespace ToolDev_IvyGenerator.Models
{
    public class SplineControlPoint : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
