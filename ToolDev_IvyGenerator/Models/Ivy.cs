using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using ToolDev_IvyGenerator.Interfaces;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.DirectX;
using Device = SharpDX.Direct3D10.Device1;

namespace ToolDev_IvyGenerator.Models
{
    public class Ivy : ISceneObject, IIntersect
    {
        //Transform
        public Matrix WorldMatrix { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }
        //Ivy params
        public Spline Stem { get; set; }
        private bool _refreshLeaves = false;
        private Model _leafModel;
        public Model LeafModel { get { return _leafModel; } set { _leafModel = value; _refreshLeaves = true; } }

        private List<Model> _leaves = new List<Model>();
        private float _leafInterval = 0.2f;
        public float Offset { get; set; }
        public bool Symmetrical { get; set; }
        //IIntersect
        public bool Selected { get { return Stem.Selected; } set { Stem.Selected = value; } }

        public Ivy()
        {
            Stem = new Spline();

            Position = new Vec3();
            Rotation = new Vec3();
            Scale = new Vec3(new Vector3(1.0f));

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Initialize(Device device)
        {
            Stem.Initialize(device);
        }

        public void Update(float deltaT)
        {
            if(_refreshLeaves)
            {
                PopulateLeaves();
                _refreshLeaves = false;
            }

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
            Stem.Update(deltaT);
            foreach (Model leaf in _leaves)
                leaf.Update(deltaT);
        }

        public void Draw(Device device, ICamera camera)
        {
            Stem.Draw(device, camera);
            if(Stem.Render)
                foreach (Model leaf in _leaves)
                    leaf.Draw(device, camera);
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            bool collision = false;

            collision = Stem.Intersects(ray, out intersectionPoint);

            return collision;
        }

        public void ResetCollisionFlags()
        {
            Stem.ResetCollisionFlags();
        }

        public void PopulateLeaves()
        {
            if (_leafModel == null)
                return;

            _leaves.Clear();

            int frequency = Convert.ToInt32(1.0 / _leafInterval);

            if (Symmetrical)
            {
                for (int i = 0; i < frequency; ++i)
                {
                    var leaf1 = new Model();
                    var leaf2 = new Model();
                    leaf1.Mesh = _leafModel.Mesh;
                    leaf2.Mesh = _leafModel.Mesh;
                    leaf1.Position.Value = Vector3.Hermite(Stem.ControlPoints[0].Position.Value,
                            Stem.ControlPoints[0].Tangent.Value,
                            Stem.ControlPoints[0 + 1].Position.Value,
                            Stem.ControlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.ForwardRH * (Stem.Thickness + Offset));
                    leaf2.Position.Value = Vector3.Hermite(Stem.ControlPoints[0].Position.Value,
                            Stem.ControlPoints[0].Tangent.Value,
                            Stem.ControlPoints[0 + 1].Position.Value,
                            Stem.ControlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.BackwardRH * (Stem.Thickness + Offset));

                    leaf1.Rotation = new Vec3(_leafModel.Rotation.Value);
                    leaf2.Rotation = new Vec3(_leafModel.Rotation.Value);
                    leaf2.Rotation.Value += new Vector3(0, 180, 0);

                    leaf1.Scale = new Vec3(_leafModel.Scale.Value);
                    leaf2.Scale = new Vec3(_leafModel.Scale.Value);

                    leaf1.Material = _leafModel.Material;
                    leaf2.Material = _leafModel.Material;

                    _leaves.Add(leaf1);
                    _leaves.Add(leaf2);
                }
            }
            else
            {
                float dir = 1;
                for (int i = 0; i < frequency; ++i)
                {
                    var leaf = new Model();
                    leaf.Mesh = _leafModel.Mesh;
                    leaf.Position.Value = Vector3.Hermite(Stem.ControlPoints[0].Position.Value,
                            Stem.ControlPoints[0].Tangent.Value,
                            Stem.ControlPoints[0 + 1].Position.Value,
                            Stem.ControlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.ForwardRH * dir * (Stem.Thickness + Offset));

                    leaf.Material = _leafModel.Material;
                    dir = dir * -1;
                    _leaves.Add(leaf);
                }
            }
        }
    }
}
