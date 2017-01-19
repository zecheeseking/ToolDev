using SharpDX;
using System.ComponentModel;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.Interfaces;

namespace ToolDev_IvyGenerator.Models
{
    public class SplineControlPoint : INotifyPropertyChanged
    {
        private int _id;
        public int ID { get { return _id; } }
        public Vec3 Position { get; set; }
        public Vec3 Tangent { get; set; }
        public TransformHandle TransformHandle;

        public SplineControlPoint(int number, Vector3 pos, Vector3 tan)
        {
            this._id = number;
            Position = new Vec3(pos);
            Tangent = new Vec3(tan - pos);
            TransformHandle = new TransformHandle();

            TransformHandle.Position = new Vec3();
            TransformHandle.Position.Value = Position.Value;

            TransformHandle.Rotation = new Vec3();
            TransformHandle.Rotation.Value = Vector3.Zero;

            TransformHandle.Scale = new Vec3();
            TransformHandle.Scale.Value = new Vector3(1.0f);
        }

        public override string ToString()
        {
            return "CP: " + _id.ToString() + " Position: " + Position.ToString() + " Tangent: " + Tangent.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
