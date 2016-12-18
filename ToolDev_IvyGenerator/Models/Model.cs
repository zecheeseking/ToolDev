
using SharpDX.Direct3D;
using DaeSharpWpf;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpDX;
using ToolDev_IvyGenerator.Annotations;
using ToolDev_IvyGenerator.Interfaces;

namespace ToolDev_IvyGenerator.Models
{

    public class Model : IModel<VertexPosColNorm>, INotifyPropertyChanged, ITransform
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public VertexPosColNorm[] Vertices { get; set; }
        public uint[] Indices { get; set; }
        public SharpDX.Direct3D10.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D10.Buffer VertexBuffer { get; set; }

        private Matrix _worldMatrix;

        public Matrix WorldMatrix{ get { return _worldMatrix; } }

        private Vector3 _position;

        public Vector3 Position { get { return _position; } }

        private Quaternion _rotation;
        public Quaternion Rotation { get { return _rotation; } }
        private Vector3 _scale;
        public Vector3 Scale { get {return _scale; } }

        public void Translate(float x, float y, float z)
        {
            _position = new Vector3(x,y,z);

            UpdateWorldMatrix();
        }

        public void Rotate()
        {

        }

        public void Scaling(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);

            UpdateWorldMatrix();
        }

        private void UpdateWorldMatrix()
        {
            _worldMatrix = Matrix.Scaling(Scale) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Position);
        }

        public Model()
        {
            _position = Vector3.Zero;
            _rotation = Quaternion.Identity;
            _scale = new Vector3(1.0f);

            UpdateWorldMatrix();
        }

        public bool Intersects(Ray ray)
        {
            for (int i = 0; i < IndexCount; i += 3)
            {
                uint t1 = Indices[i];
                uint t2 = Indices[i + 1];
                uint t3 = Indices[i + 2];

                if(ray.Intersects(ref Vertices[t1].Position, ref Vertices[t2].Position, ref Vertices[t3].Position))
                    return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}