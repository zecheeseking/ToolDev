
using Assimp;
using Assimp.Configs;
using System.IO;
using DaeSharpWpf;
using SharpDX.Direct3D10;
using ToolDev_IvyGenerator.Models;
using Buffer = SharpDX.Direct3D10.Buffer;
using SharpDX.Direct3D;
using System.Runtime.InteropServices;
using SharpDX;
using Device = SharpDX.Direct3D10.Device;

namespace ToolDev_IvyGenerator.Utilities
{
    public class ModelLoader
    {
        public static IModel<VertexPosColNorm> LoadModel(string path, Device device)
        {
            var extension = path.Split('.');

            if (extension[1] == "ovm")
                return LoadOvm(path, device);

            return AssimpLoad(path, device);
        }

        private static IModel<VertexPosColNorm> LoadOvm(string path, Device device)
        {
            Model model = new Model();

            //Set Primitive Topology
            model.PrimitiveTopology = PrimitiveTopology.TriangleList;

            //Set VertexStride
            model.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            model.Vertices = null;
            model.Indices = null;

            //Parse OVM file
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                var versionMajor = reader.ReadByte();
                var versionMinor = reader.ReadByte();

                uint vertexCount = 0, indexCount = 0;

                while (true)
                {
                    var blockId = reader.ReadByte();

                    if (blockId == 0)
                        break;

                    var blockLength = reader.ReadUInt32();

                    switch (blockId)
                    {
                        case 1:
                            var name = reader.ReadString();
                            vertexCount = reader.ReadUInt32();
                            indexCount = reader.ReadUInt32();

                            model.IndexCount = (int)indexCount;
                            model.Vertices = new VertexPosColNorm[vertexCount];
                            model.Indices = new uint[indexCount];
                            break;
                        case 2:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                model.Vertices[i] = new VertexPosColNorm(Vector3.Zero, Color.Gray, Vector3.Zero);
                                model.Vertices[i].Position.X = reader.ReadSingle();
                                model.Vertices[i].Position.Y = reader.ReadSingle();
                                model.Vertices[i].Position.Z = reader.ReadSingle();
                            }
                            break;
                        case 3:
                            for (var i = 0; i < indexCount; ++i)
                            {
                                model.Indices[i] = reader.ReadUInt32();
                            }
                            break;
                        case 4:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                model.Vertices[i].Normal.X = reader.ReadSingle();
                                model.Vertices[i].Normal.Y = reader.ReadSingle();
                                model.Vertices[i].Normal.Z = reader.ReadSingle();
                            }
                            break;
                        case 7:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                model.Vertices[i].Color.X = reader.ReadSingle();
                                model.Vertices[i].Color.Y = reader.ReadSingle();
                                model.Vertices[i].Color.Z = reader.ReadSingle();
                                model.Vertices[i].Color.W = reader.ReadSingle();
                            }
                            break;
                        default:
                            reader.ReadBytes((int)blockLength);
                            break;
                    }
                }
            }

            //Create buffers
            model.CreateVertexBuffer(device);
            model.CreateIndexBuffer(device);

            return model;
        }

        private static IModel<VertexPosColNorm> AssimpLoad(string path, Device device)
        {
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene AssimpModel = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            Model model = new Model();

            model.PrimitiveTopology = PrimitiveTopology.TriangleList;

            model.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            int vertCount = AssimpModel.Meshes[0].VertexCount;
            model.Vertices = new VertexPosColNorm[vertCount];

            for (int i = 0; i < vertCount; ++i)
            {
                var pos = AssimpModel.Meshes[0].Vertices[i];
                model.Vertices[i] = new VertexPosColNorm(new Vector3(pos.X, pos.Y, pos.Z), Color.Gray, Vector3.Zero);
            }

            for (int i = 0; i < vertCount; ++i)
            {
                var norm = AssimpModel.Meshes[0].Normals[i];
                model.Vertices[i].Normal.X = norm.X;
                model.Vertices[i].Normal.Y = norm.Y;
                model.Vertices[i].Normal.Z = norm.Z;
            }

            int indicesCount = AssimpModel.Meshes[0].GetIndices().Length;
            model.Indices = new uint[indicesCount];

            for (int i = 0; i < indicesCount; ++i)
                model.Indices[i] = (uint)AssimpModel.Meshes[0].GetIndices()[i];

            model.IndexCount = model.Indices.Length;

            model.CreateVertexBuffer(device);
            model.CreateIndexBuffer(device);

            importer.Dispose();

            return model;
        }
    }
}