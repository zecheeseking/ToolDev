﻿
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
        public static IModel LoadModel(string path, Device device)
        {
            var extension = path.Split('.');

            if (extension[1] == "ovm")
                return LoadOvm(path, device);

            return AssimpLoad(path, device);
        }

        private static IModel LoadOvm(string path, Device device)
        {
            Model model = new Model();

            //Set Primitive Topology
            model.PrimitiveTopology = PrimitiveTopology.TriangleList;

            //Set VertexStride
            model.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            VertexPosColNorm[] verts = null;
            uint[] indices = null;

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
                            verts = new VertexPosColNorm[vertexCount];
                            indices = new uint[indexCount];
                            break;
                        case 2:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                verts[i] = new VertexPosColNorm(Vector3.Zero, Color.Gray, Vector3.Zero);
                                verts[i].Position.X = reader.ReadSingle();
                                verts[i].Position.Y = reader.ReadSingle();
                                verts[i].Position.Z = reader.ReadSingle();
                            }
                            break;
                        case 3:
                            for (var i = 0; i < indexCount; ++i)
                            {
                                indices[i] = reader.ReadUInt32();
                            }
                            break;
                        case 4:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                verts[i].Normal.X = reader.ReadSingle();
                                verts[i].Normal.Y = reader.ReadSingle();
                                verts[i].Normal.Z = reader.ReadSingle();
                            }
                            break;
                        case 7:
                            for (var i = 0; i < vertexCount; ++i)
                            {
                                verts[i].Color.X = reader.ReadSingle();
                                verts[i].Color.Y = reader.ReadSingle();
                                verts[i].Color.Z = reader.ReadSingle();
                                verts[i].Color.W = reader.ReadSingle();
                            }
                            break;
                        default:
                            reader.ReadBytes((int)blockLength);
                            break;
                    }
                }
            }

            //Create buffers
            model.VertexBuffer?.Dispose();
            model.VertexBuffer = CreateVertexBuffer(device, verts, model.VertexStride);

            model.IndexBuffer?.Dispose();
            model.IndexBuffer = CreateIndexBuffer(device, indices, model.IndexCount);

            return model;
        }

        private static IModel AssimpLoad(string path, Device device)
        {
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            Scene AssimpModel = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            Model model = new Model();

            model.PrimitiveTopology = PrimitiveTopology.TriangleList;

            model.VertexStride = Marshal.SizeOf(typeof(VertexPosColNorm));

            int vertCount = AssimpModel.Meshes[0].VertexCount;
            VertexPosColNorm[] verts = new VertexPosColNorm[vertCount];

            for (int i = 0; i < vertCount; ++i)
            {
                var pos = AssimpModel.Meshes[0].Vertices[i];
                verts[i] = new VertexPosColNorm(new Vector3(pos.X, pos.Y, pos.Z), Color.Gray, Vector3.Zero);
            }

            for (int i = 0; i < vertCount; ++i)
            {
                var norm = AssimpModel.Meshes[0].Normals[i];
                verts[i].Normal.X = norm.X;
                verts[i].Normal.Y = norm.Y;
                verts[i].Normal.Z = norm.Z;
            }

            int indicesCount = AssimpModel.Meshes[0].GetIndices().Length;
            uint[] indices = new uint[indicesCount];

            for (int i = 0; i < indicesCount; ++i)
                indices[i] = (uint)AssimpModel.Meshes[0].GetIndices()[i];

            model.IndexCount = indices.Length;

            model.VertexBuffer?.Dispose();
            model.VertexBuffer = CreateVertexBuffer(device, verts, model.VertexStride);

            model.IndexBuffer?.Dispose();
            model.IndexBuffer = CreateIndexBuffer(device, indices, model.IndexCount);

            importer.Dispose();

            return model;
        }

        private static Buffer CreateVertexBuffer(Device device, VertexPosColNorm[] verts, int vertexStride)
        {
            var bufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
                SizeInBytes = vertexStride * verts.Length
            };

            return new Buffer(device, DataStream.Create(verts, false, false), bufferDescription);
        }

        private static Buffer CreateIndexBuffer(Device device, uint[] indices, int indexCount)
        {
            var bufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
                SizeInBytes = sizeof(uint) * indexCount
            };

            return new Buffer(device, DataStream.Create(indices, false, false), bufferDescription);
        }
    }
}