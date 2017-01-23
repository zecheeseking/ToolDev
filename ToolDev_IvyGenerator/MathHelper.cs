
using System;
using SharpDX;
using ToolDev_IvyGenerator.Utilities;

namespace ToolDev_IvyGenerator
{
    public class MathHelper
    {
        public static float AngleToRadians(float angle)
        {
            return angle * ((float)Math.PI / 180.0f);
        }

        public static float RadiansToAngle(float rads)
        {
            return rads * (180.0f / (float)Math.PI);
        }

        public static Vector3 QuatToEulerExtrinic(Quaternion q)
        {
            Vector3 eulerAngles = Vector3.Zero;
            double ysqr = q.Y * q.Y;

            double t0 = +2.0f * (q.W * q.X + q.Y * q.Z);
            double t1 = +1.0f - 2.0f * (q.X * q.X + ysqr);

            eulerAngles.X = (float)Math.Atan2(t0, t1);

            double t2 = +2.0f * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > 1.0f ? 1.0f : t2;
            t2 = t2 < -1.0f ? -1.0f : t2;

            eulerAngles.Y = (float)Math.Asin(t2);

            double t3 = 2.0f * (q.W * q.Z + q.X * q.Y);
            double t4 = 1.0f - 2.0f * (ysqr + q.Z * q.Z);
            eulerAngles.Z = (float)Math.Atan2(t3, t4);

            return eulerAngles;
        }

        public static Vector3 QuatToEulerIntrinic(Quaternion q)
        {
            Vector3 eulerAngles = Vector3.Zero;

            eulerAngles.X = (float)Math.Atan2(((q.X * q.Z) + (q.Y * q.W)), -((q.X * q.Z) - (q.Y * q.W)));
            eulerAngles.Y = (float)Math.Acos(-(q.X * q.X) - (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W));
            eulerAngles.Z = (float)Math.Atan2(2 * ((q.X * q.Z) - (q.Y * q.W)), ((q.X * q.Z) + (q.Y * q.W)));

            return eulerAngles;
        }

        public static Matrix CalculateWorldMatrix(Vec3 scale, Vec3 rot, Vec3 pos)
        {
            return Matrix.Scaling(scale.Value) * 
                Matrix.RotationYawPitchRoll(MathHelper.AngleToRadians(rot.Y), MathHelper.AngleToRadians(rot.X), MathHelper.AngleToRadians(rot.Z)) * 
                Matrix.Translation(pos.Value);
        }

        public static Matrix CalculateWorldMatrix(Vec3 scale, Quaternion rot, Vec3 pos)
        {
            return Matrix.Scaling(scale.Value) *
                Matrix.RotationQuaternion(rot) *
                Matrix.Translation(pos.Value);
        }
    }
}