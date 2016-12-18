﻿using SharpDX;

namespace DaeSharpWPF
{
    public interface ICamera
    {
        void Initialize(float width, float height);

        Matrix TransformationMatrix { get; }

        Matrix ViewMatrix { get; }

        Matrix ProjectionMatrix { get; }

        void Update(float deltaT);
    }
}