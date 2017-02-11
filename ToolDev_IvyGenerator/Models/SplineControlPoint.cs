﻿using SharpDX;
using System.ComponentModel;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.Interfaces;
using Device = SharpDX.Direct3D10.Device1;
using System;

namespace ToolDev_IvyGenerator.Models
{
    public class SplineControlPoint :  IIntersect
    {
        private int _id;
        public int ID { get { return _id; } }
        public Vec3 Position { get; set; }
        public Vec3 Tangent { get; set; }

        public bool Selected { get; set; }

        public TransformComponent TransformHandlePosition;
        private Line _handleLine;
        public TransformComponent TransformHandleTangent;

        public SplineControlPoint(int number, Vector3 pos, Vector3 tan)
        {
            this._id = number;
            Position = new Vec3(pos);
            Tangent = new Vec3(tan - pos);
            TransformHandlePosition = new TransformComponent();
            TransformHandlePosition.HandleLength = 5.0f;

            TransformHandleTangent = new TransformComponent();
            TransformHandleTangent.HandleLength = 3.0f;

            _handleLine = new Line();
            _handleLine.StartPosition = TransformHandlePosition.Position;
            _handleLine.EndPosition = TransformHandleTangent.Position;
        }

        public void Initialize(Device device)
        {
            TransformHandlePosition.Initialize(device);
            TransformHandleTangent.Initialize(device);
            _handleLine.Initialize(device);
        }

        public void Update(float deltaT)
        {
            //Tangent.Value = TransformHandleTangent.Position.Value - TransformHandlePosition.Position.Value;
            TransformHandlePosition.Update(deltaT);
            //TransformHandleTangent.Position.Value = TransformHandlePosition.Position.Value + Tangent.Value;
            TransformHandleTangent.Update(deltaT);
            TransformHandleTangent.WorldMatrix *= TransformHandlePosition.WorldMatrix;
            _handleLine.StartPosition = TransformHandlePosition.Position;
            _handleLine.EndPosition = new Vec3(_handleLine.StartPosition.Value + Tangent.Value);
            Position = new Vec3(TransformHandlePosition.Position.Value);
            Tangent = new Vec3(TransformHandleTangent.Position.Value);

        }

        public void Draw(Device device, ICamera camera)
        {
            TransformHandlePosition.Draw(device, camera);
            _handleLine.Draw(device, camera);
            TransformHandleTangent.Draw(device, camera);
        }

        public override string ToString()
        {
            return "CP: " + _id.ToString() + " Position: " + Position.ToString() + " Tangent: " + Tangent.ToString();
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            if (TransformHandlePosition.Intersects(ray, out intersectionPoint))
                return true;
            else if (TransformHandleTangent.Intersects(ray, out intersectionPoint))
                return true;

            return false;
        }

        public void ResetCollisionFlags()
        {
            TransformHandlePosition.ResetCollisionFlags();
            TransformHandleTangent.ResetCollisionFlags();
        }
    }
}
