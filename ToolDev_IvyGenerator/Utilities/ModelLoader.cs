
using Assimp;
using Assimp.Configs;
using System.IO;
using SharpDX.Direct3D;
using System.Runtime.InteropServices;
using SharpDX;
using Device = SharpDX.Direct3D10.Device1;
using SharpDX.Direct3D10;

namespace ToolDev_IvyGenerator.Utilities
{
    public class ModelLoader<T> where T: struct
    {
        public static MeshData<T> LoadModel(string path, Device device)
        {
            var extension = path.Split('.');

            if (extension[1] == "ovm")
                return LoadOvm(path, device);

            return AssimpLoad(path, device);
        }

        private static MeshData<T> LoadOvm(string path, Device device)
        {
            MeshData<T> Mesh = new MeshData<T>();

            //Set Primitive Topology
            Mesh.PrimitiveTopology = PrimitiveTopology.TriangleList;

            //Set VertexStride
            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            Mesh.Vertices = null;
            Mesh.Indices = null;

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

                            Mesh.IndexCount = (int)indexCount;
                            Mesh.Vertices = new T[vertexCount];
                            Mesh.Indices = new uint[indexCount];
                            break;
                        case 2:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                Mesh.Positions[i].X = reader.ReadSingle();
                                Mesh.Positions[i].Y = reader.ReadSingle();
                                Mesh.Positions[i].Z = reader.ReadSingle();
                            }
                            break;
                        case 3:
                            for (var i = 0; i < indexCount; ++i)
                            {
                                Mesh.Indices[i] = reader.ReadUInt32();
                            }
                            break;
                        case 4:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                Mesh.Normals[i].X = reader.ReadSingle();
                                Mesh.Normals[i].Y = reader.ReadSingle();
                                Mesh.Normals[i].Z = reader.ReadSingle();
                            }
                            break;
                        case 7:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                //Mesh.Colors[i].R = reader.ReadSingle();
                                //Mesh.Colors[i].G = reader.ReadSingle();
                                //Mesh.Colors[i].B = reader.ReadSingle();
                                //Mesh.Colors[i].A = reader.ReadSingle();

                                reader.ReadSingle();
                                reader.ReadSingle();
                                reader.ReadSingle();
                                reader.ReadSingle();
                            }
                            break;
                        default:
                            reader.ReadBytes((int)blockLength);
                            break;
                    }
                }
            }

            //Create buffers
            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            return Mesh;
        }

        private static MeshData<T> AssimpLoad(string path, Device device)
        {
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene AssimpModel = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            MeshData<T> Mesh = new MeshData<T>();

            Mesh.PrimitiveTopology = PrimitiveTopology.TriangleList;

            Mesh.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            int vertCount = AssimpModel.Meshes[0].VertexCount;
            Mesh.Positions = new Vector3[vertCount];
            Mesh.Normals = new Vector3[vertCount];
            Mesh.Colors = new Color[vertCount];

            Mesh.Vertices = new T[vertCount];

            for (int i = 0; i < vertCount; ++i)
            {
                var pos = AssimpModel.Meshes[0].Vertices[i];
                Mesh.Positions[i] = new Vector3(pos.X, pos.Y, pos.Z);
            }

            for (int i = 0; i < vertCount; ++i)
            {
                var norm = AssimpModel.Meshes[0].Normals[i];
                Mesh.Normals[i].X = norm.X;
                Mesh.Normals[i].Y = norm.Y;
                Mesh.Normals[i].Z = norm.Z;
            }

            for (int i = 0; i < vertCount; ++i)
                Mesh.Colors[i] = Color.Gray;

            int indicesCount = AssimpModel.Meshes[0].GetIndices().Length;
            Mesh.Indices = new uint[indicesCount];

            for (int i = 0; i < indicesCount; ++i)
                Mesh.Indices[i] = (uint)AssimpModel.Meshes[0].GetIndices()[i];

            Mesh.IndexCount = Mesh.Indices.Length;

            Mesh.CreateVertexBuffer(device);
            Mesh.CreateIndexBuffer(device);

            importer.Dispose();

            return Mesh;
        }
    }
}