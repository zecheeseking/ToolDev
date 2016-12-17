
using SharpDX.Direct3D;
using DaeSharpWpf;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpDX;
using ToolDev_IvyGenerator.Annotations;
using ToolDev_IvyGenerator.Interfaces;

namespace ToolDev_IvyGenerator.Models
{

    public class Model : IModel, INotifyPropertyChanged, ITransform
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public VertexPosColNorm[] Vertices { get; set; }
        public uint[] Indices { get; set; }
        public SharpDX.Direct3D10.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D10.Buffer VertexBuffer { get; set; }

        public Matrix WorldMatrix { get; set; }

        public void Translate(float x, float y, float z)
        {
        }

        public void Rotate()
        {
        }

        public void Scale()
        {
        }

        public Model()
        {
            WorldMatrix = Matrix.Scaling(1.0f) * Matrix.RotationQuaternion(Quaternion.Identity) * Matrix.Translation(Vector3.Zero);
        }

        public bool Intersects(Ray ray)
        {
            Matrix worldInverse = WorldMatrix;
            worldInverse.Invert();

            //Still have to transform ray to world space.
            Vector3 rPos = Vector3.Zero;
            Vector3 rDir = Vector3.Zero;

            Vector3.Transform(ray.Position, worldInverse);
            Vector3.TransformNormal(ray.Direction, worldInverse);

            rPos.Normalize();
            rDir.Normalize();

            Ray r = new Ray(rPos, rDir);

            for (int i = 0; i < IndexCount; i += 3)
            {
                uint t1 = Indices[i];
                uint t2 = Indices[i + 1];
                uint t3 = Indices[i + 2];

                if(r.Intersects(ref Vertices[t1].Position, ref Vertices[t2].Position, ref Vertices[t3].Position))
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