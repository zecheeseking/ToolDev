using System.IO;
using DaeSharpWpf;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D10;
using Device1 = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Effects
{
    public class SceneGridEffect : IEffect
    {
        public EffectTechnique Technique { get; set; }
        public Effect Effect { get; set; }
        public InputLayout InputLayout { get; set; }
        public void Create(Device1 device)
        {
            var shaderSource = File.ReadAllText("Resources\\SceneGrid.fx");
            var shaderByteCode = ShaderBytecode.Compile(shaderSource, "fx_4_0", ShaderFlags.None, EffectFlags.None);
            Effect = new Effect(device, shaderByteCode);
            Technique = Effect.GetTechniqueByIndex(0);

            var pass = Technique.GetPassByIndex(0);
            InputLayout = new InputLayout(device, pass.Description.Signature, InputLayouts.PosCol);
        }

        public void SetWorld(Matrix world){}

        public void SetWorldViewProjection(Matrix wvp)
        {
            if (Effect != null)
                Effect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(wvp);
        }

        public void SetLightDirection(Vector3 dir){}
    }
}