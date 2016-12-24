using System.IO;
using DaeSharpWpf;
using DaeSharpWpf.Interfaces;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D10;
using Device1 = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Effects
{
    public class PosNormColEffect : IEffect
    {
        public EffectTechnique Technique { get; set; }
        public Effect Effect { get; set; }
        public InputLayout InputLayout { get; set; }
        public void Create(Device1 device)
        {
            //Load Effect
            var shaderSource = File.ReadAllText("Resources\\PosColNorm3D.fx");
            var shaderByteCode = ShaderBytecode.Compile(shaderSource, "fx_4_0", ShaderFlags.None, EffectFlags.None);
            Effect = new Effect(device, shaderByteCode);
            Technique = Effect.GetTechniqueByIndex(0);

            //InputLayout
            var pass = Technique.GetPassByIndex(0);
            InputLayout = new InputLayout(device, pass.Description.Signature, InputLayouts.PosNormCol);
        }

        public void SetWorld(Matrix world)
        {
            if (Effect != null)
                Effect.GetVariableBySemantic("WORLD").AsMatrix().SetMatrix(world);
        }

        public void SetWorldViewProjection(Matrix wvp)
        {
            if (Effect != null)
                Effect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(wvp);
        }

        public void SetLightDirection(Vector3 dir)
        {
            if (Effect != null)
                Effect.GetVariableByName("gLightDirection").AsVector().Set(dir);
        }

        public void SetColor(Color color)
        {
            if(Effect != null)
                Effect.GetVariableByName("gColor").AsVector().Set(color.ToVector4());
        }
    }
}
