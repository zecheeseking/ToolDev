using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using ToolDev_IvyGenerator.Interfaces;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.DirectX;

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
        private Spline _stem;
        public Spline Stem { get { return _stem; } set { _stem = value; } }
        private List<Model> _leaves;
        private float _leafInterval = 0.2f;
        private bool _symmetrical = true;
        public bool Symmetrical { get { return _symmetrical; } set { _symmetrical = value; } }
        //IIntersect
        public bool Selected { get; set; }

        public Ivy()
        {
            _stem = new Spline();

            Position = new Vec3();
            Rotation = new Vec3();
            Scale = new Vec3(new Vector3(1.0f));

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Initialize(Device device)
        {
            _stem.Initialize(device);
        }

        public void Update(float deltaT)
        {
            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Draw(AppContext appContext)
        {
            _stem.Draw(appContext);
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            //throw new NotImplementedException();
            intersectionPoint = Vector3.Zero;
            return false;
        }

        public void ResetCollisionFlags()
        {
            //throw new NotImplementedException();
        }

        //public void PopulateLeaves()
        //{
        //    if (_leafModel == null)
        //        return;

        //    _leaves.Clear();

        //    int frequency = Convert.ToInt32(1.0 / _leafInterval);

        //    if (_symmetrical)
        //    {
        //        for (int i = 0; i < frequency; ++i)
        //        {
        //            var leaf1 = new Model();
        //            var leaf2 = new Model();
        //            leaf1.Mesh = _leafModel.Mesh;
        //            leaf2.Mesh = _leafModel.Mesh;
        //            leaf1.Position.Value = Vector3.Hermite(_controlPoints[0].Position.Value,
        //                    _controlPoints[0].Tangent.Value,
        //                    _controlPoints[0 + 1].Position.Value,
        //                    _controlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.ForwardRH * Thickness);
        //            leaf2.Position.Value = Vector3.Hermite(_controlPoints[0].Position.Value,
        //                    _controlPoints[0].Tangent.Value,
        //                    _controlPoints[0 + 1].Position.Value,
        //                    _controlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.BackwardRH * Thickness);

        //            leaf1.Rotation = _leafModel.Rotation;
        //            leaf2.Rotation = _leafModel.Rotation;

        //            leaf1.Material = _leafModel.Material;
        //            leaf2.Material = _leafModel.Material;

        //            _leaves.Add(leaf1);
        //            _leaves.Add(leaf2);
        //        }
        //    }
        //    else
        //    {
        //        float dir = 1;
        //        for (int i = 0; i < frequency; ++i)
        //        {
        //            var leaf = new Model();
        //            leaf.Mesh = _leafModel.Mesh;
        //            leaf.Position.Value = Vector3.Hermite(_controlPoints[0].Position.Value,
        //                    _controlPoints[0].Tangent.Value,
        //                    _controlPoints[0 + 1].Position.Value,
        //                    _controlPoints[0 + 1].Tangent.Value, _leafInterval * i) + (Vector3.ForwardRH * dir * Thickness);

        //            leaf.Material = _leafModel.Material;
        //            dir = dir * -1;
        //            _leaves.Add(leaf);
        //        }
        //    }
        //}
    }
}
