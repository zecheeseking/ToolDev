﻿using SharpDX;
using System.ComponentModel;
using ToolDev_IvyGenerator.DirectX;
using ToolDev_IvyGenerator.Utilities;
using ToolDev_IvyGenerator.Interfaces;
using Device = SharpDX.Direct3D11.Device;
using System;

namespace ToolDev_IvyGenerator.Models
{
    public class SplineControlPoint : INotifyPropertyChanged, IIntersect
    {
        private int _id;
        public int ID { get { return _id; } }
        public Vec3 Position { get; set; }
        public Vec3 Tangent { get; set; }

        public bool Selected { get; set; }

        public TransformHandle TransformHandlePosition;
        private Line _handleLine;
        public TransformHandle TransformHandleTangent;

        public SplineControlPoint(int number, Vector3 pos, Vector3 tan)
        {
            this._id = number;
            Position = new Vec3(pos);
            Tangent = new Vec3(tan - pos);
            TransformHandlePosition = new TransformHandle();
            TransformHandlePosition.HandleLength = 5.0f;
            TransformHandlePosition.Position = new Vec3();
            TransformHandlePosition.Position.Value = Position.Value;
            TransformHandlePosition.Rotation = new Vec3();
            TransformHandlePosition.Scale = new Vec3();
            TransformHandlePosition.Scale.Value = new Vector3(1.0f);

            TransformHandleTangent = new TransformHandle();
            TransformHandleTangent.HandleLength = 3.0f;
            TransformHandleTangent.Position = new Vec3();
            TransformHandleTangent.Position.Value = Position.Value + Tangent.Value;
            TransformHandleTangent.Rotation = new Vec3();
            TransformHandleTangent.Scale = new Vec3();
            TransformHandleTangent.Scale.Value = new Vector3(1.0f);

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
            TransformHandlePosition.Update(deltaT);
            TransformHandleTangent.Update(deltaT);
            _handleLine.StartPosition = TransformHandlePosition.Position;
            _handleLine.EndPosition = TransformHandleTangent.Position;
        }

        public void Draw(AppContext appContext)
        {
            TransformHandlePosition.Draw(appContext);
            _handleLine.Draw(appContext);
            TransformHandleTangent.Draw(appContext);
        }

        public override string ToString()
        {
            return "CP: " + _id.ToString() + " Position: " + Position.ToString() + " Tangent: " + Tangent.ToString();
        }

        public bool Intersects(Ray ray, out Vector3 intersectionPoint)
        {
            TransformHandlePosition.ResetCollisionFlags();
            TransformHandlePosition.ResetCollisionFlags();

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
