
using SharpDX.Direct3D;
using DaeSharpWpf;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToolDev_IvyGenerator.Annotations;

namespace ToolDev_IvyGenerator.Models
{

    public class Model : IModel, INotifyPropertyChanged
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public int VertexStride { get; set; }
        public int IndexCount { get; set; }
        public SharpDX.Direct3D10.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D10.Buffer VertexBuffer { get; set; }

        public Model(){}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}