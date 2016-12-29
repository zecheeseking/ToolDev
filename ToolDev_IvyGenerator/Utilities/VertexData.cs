using SharpDX;
using SharpDX.Direct3D10;
using SharpDX.DXGI;

namespace ToolDev_IvyGenerator.Utilities
{
    public struct VertexPosCol
    {
        public Vector3 Position;
        public Vector4 Color;

        public VertexPosCol(Vector3 pos, Color col)
        {
            Position = pos;
            Color = col.ToVector4(); 
        }
    }

    public struct VertexPosColNorm
    {
        public Vector3 Position;
        public Vector4 Color;
        public Vector3 Normal;

        public VertexPosColNorm(Vector3 pos, Color col)
        {
            Position = pos;
            Color = col.ToVector4();
            Normal = Vector3.Up;
        }

        public VertexPosColNorm(Vector3 pos, Color col, Vector3 norm)
        {
            Position = pos;
            Color = col.ToVector4();
            Normal = norm;
        }
    }

    public static class InputLayouts
    {
        public static readonly InputElement[] PosCol = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)  
        };

        public static readonly InputElement[] PosNormCol = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("NORMAL", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)
        };
    }
}
