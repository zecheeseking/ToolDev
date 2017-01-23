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
        struct RandomIntervals
        {
            public float _interval;
            public float _rotX;
            public float _rotY;
            public float _rotZ;
            public float _scaleX;
            public float _scaleY;
            public float _scaleZ;

            public RandomIntervals(Random rand, float minInterval, float maxInterval, 
                float minRot, float maxRot, float minScale, float maxScale)
            {
                _interval = rand.NextFloat(minInterval, maxInterval);
                _rotX = rand.NextFloat(minRot, maxRot);
                _rotY = rand.NextFloat(minRot, maxRot);
                _rotZ = rand.NextFloat(minRot, maxRot);
                _scaleX = rand.NextFloat(minScale, maxScale);
                _scaleY = rand.NextFloat(minScale, maxScale);
                _scaleZ = rand.NextFloat(minScale, maxScale);
            }
        }
        //Transform
        public Matrix WorldMatrix { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }
        //Ivy params
        public Spline Stem { get; set; }
        private Model _leafModel;
        public Model LeafModel { get { return _leafModel; } set { _leafModel = value; } }

        private List<Model> _leaves = new List<Model>();
        public float LeafInterval { get; set; }
        public float Offset { get; set; }
        public bool Symmetrical { get; set; }
        public bool Selected { get { return Stem.Selected; } set { Stem.Selected = value; } }
        private Matrix _leafBaseRotation { get; set; }
        private List<RandomIntervals> _leafRandomIntervals = new List<RandomIntervals>();

        public float MinIntervalRange { get; set; }
        public float MaxIntervalRange { get; set; }

        public float MinRotationRange { get; set; }
        public float MaxRotationRange { get; set; }

        public float MinScalingRange { get; set; }
        public float MaxScalingRange { get; set; }

        Random randomGenerator = new Random(DateTime.Now.Millisecond);

        public Ivy()
        {
            Stem = new Spline();

            Position = new Vec3();
            Rotation = new Vec3();
            Scale = new Vec3(new Vector3(1.0f));

            LeafInterval = 0.2f;

            MinRotationRange = 0.0f;
            MaxRotationRange = 0.0f;


            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
        }

        public void Initialize(Device device)
        {
            Stem.Initialize(device);
        }

        public void Update(float deltaT)
        {
            PopulateLeaves();

            WorldMatrix = MathHelper.CalculateWorldMatrix(Scale, Rotation, Position);
            Stem.Update(deltaT);
            //foreach (Model leaf in _leaves)
            //    leaf.Update(deltaT);
        }

        public void Draw(Device device, ICamera camera)
        {
            Stem.Draw(device, camera);
            if(Stem.Render)
                foreach (Model leaf in _leaves)
                    leaf.Draw(device, camera);
        }

        public void RefreshRandomValues(int size)
        {
            _leafRandomIntervals.Clear();

            for (int i = 0; i < size; ++i)
                _leafRandomIntervals.Add(new RandomIntervals(randomGenerator, 
                    MinIntervalRange, MaxIntervalRange, 
                    MinRotationRange, MaxRotationRange,
                    MinScalingRange, MaxScalingRange));
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

            int frequency = Convert.ToInt32(1.0 / LeafInterval);
            int leafInt = 0;
            for(int cp = 0; cp < Stem.ControlPoints.Count - 1; ++cp)
            {
                if (Symmetrical)
                {
                    if ((Stem.ControlPoints.Count - 1) * (frequency * 2) > _leafRandomIntervals.Count)
                        RefreshRandomValues((Stem.ControlPoints.Count - 1) * (frequency * 2));

                    for (int i = 0; i < frequency; ++i)
                    {
                        _leaves.Add(CreateLeaf(i, leafInt, cp));
                        _leaves.Add(CreateLeaf(i, leafInt + 1, cp, true));

                        leafInt += 2;
                    }
                }
                else
                {
                    float dir = 1;
                    if ((Stem.ControlPoints.Count - 1) * (frequency) > _leafRandomIntervals.Count)
                        RefreshRandomValues((Stem.ControlPoints.Count - 1) * (frequency));

                    for (int i = 0; i < frequency; ++i)
                    {
                        if(dir == 1)
                            _leaves.Add(CreateLeaf(i, leafInt, cp));
                        else
                            _leaves.Add(CreateLeaf(i, leafInt, cp, true));

                        leafInt += 1;
                        dir *= -1;
                    }
                }
            }
        }

        private Model CreateLeaf(int index, int randIndex, int controlPointIndex, bool flipSide = false)
        {
            Model leaf = new Model();

            leaf.Mesh = _leafModel.Mesh;

            float interval = LeafInterval + _leafRandomIntervals[randIndex]._interval;

            var pos = Vector3.Hermite(Stem.ControlPoints[controlPointIndex].Position.Value,
                                Stem.ControlPoints[controlPointIndex].Tangent.Value,
                                Stem.ControlPoints[controlPointIndex + 1].Position.Value,
                                Stem.ControlPoints[controlPointIndex + 1].Tangent.Value, interval * index);

            var forward = Vector3.Hermite(Stem.ControlPoints[controlPointIndex].Position.Value,
                                Stem.ControlPoints[controlPointIndex].Tangent.Value,
                                Stem.ControlPoints[controlPointIndex + 1].Position.Value,
                                Stem.ControlPoints[controlPointIndex + 1].Tangent.Value, interval * index + 0.05f);
            forward -= pos;
            forward.Normalize();
            var posThicknessOffset = Vector3.Cross(forward, Vector3.Up);

            leaf.Position.Value = new Vector3(0.0f);

            var q = Quaternion.LookAtRH(pos, pos + forward, Vector3.Up);
            q.Normalize();
            q.Invert();

            Matrix transform;

            if(flipSide)
                transform = MathHelper.CalculateWorldMatrix(leaf.Scale, q,
                            new Vec3(pos + (-posThicknessOffset * (Stem.Thickness + Offset))));
            else
                transform = MathHelper.CalculateWorldMatrix(leaf.Scale, q,
                            new Vec3(pos + (posThicknessOffset * (Stem.Thickness + Offset))));

            leaf.Rotation = new Vec3(_leafModel.Rotation.Value +
                            new Vector3(_leafRandomIntervals[randIndex]._rotX,
                            _leafRandomIntervals[randIndex]._rotY,
                            _leafRandomIntervals[randIndex]._rotZ));
            if (flipSide)
                leaf.Rotation.Value += new Vector3(0, 180, 0);

            leaf.Scale = new Vec3(_leafModel.Scale.Value +
                            new Vector3(_leafRandomIntervals[randIndex]._scaleX,
                            _leafRandomIntervals[randIndex]._scaleY,
                            _leafRandomIntervals[randIndex]._scaleZ));

            leaf.WorldMatrix = MathHelper.CalculateWorldMatrix(leaf.Scale, leaf.Rotation, leaf.Position);
            leaf.WorldMatrix *= transform;

            leaf.Material = _leafModel.Material;

            return leaf;
        }
    }
}
