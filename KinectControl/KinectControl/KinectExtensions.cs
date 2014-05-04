using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace KinectControl
{
    public static class KinectExtensions
    {
        public static Vector3 ToVector3(this SkeletonPoint v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector2 ToVector2(this SkeletonPoint v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Matrix GetMatrix(this BoneRotation r)
        {
            var m = r.Matrix;

            return new Matrix(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }

        public static Vector3 GetPosition(this Skeleton skel, JointType jointId)
        {
            var v = skel.Joints[jointId].Position;
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static float GetPositionX(this Skeleton skel, JointType jointId)
        {
            return skel.Joints[jointId].Position.X;
        }

        public static float GetPositionY(this Skeleton skel, JointType jointId)
        {
            return skel.Joints[jointId].Position.Y;
        }

        public static float GetPositionZ(this Skeleton skel, JointType jointId)
        {
            return skel.Joints[jointId].Position.Z;
        }

        public static float GetDistance(this Skeleton skel, JointType joint1, JointType joint2, char ignoreAxis = '\0')
        {
            var p1 = skel.GetPosition(joint1);
            var p2 = skel.GetPosition(joint2);

            switch (ignoreAxis)
            {
                case 'x': p1.X = p2.X = 0; break;
                case 'y': p1.Y = p2.Y = 0; break;
                case 'z': p1.Z = p2.Z = 0; break;
            }

            return (p1 - p2).Length();
        }

        public static float GetDistanceX(this Skeleton skel, JointType joint1, JointType joint2)
        {
            return skel.Joints[joint1].Position.X - skel.Joints[joint2].Position.X;
        }

        public static float GetDistanceY(this Skeleton skel, JointType joint1, JointType joint2)
        {
            return skel.Joints[joint1].Position.Y - skel.Joints[joint2].Position.Y;
        }

        public static float GetDistanceZ(this Skeleton skel, JointType joint1, JointType joint2)
        {
            return skel.Joints[joint1].Position.Z - skel.Joints[joint2].Position.Z;
        }

        public static float GetRotationX(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var y1 = skel.Joints[joint1].Position.Y;
            var y2 = skel.Joints[joint2].Position.Y;
            var z1 = skel.Joints[joint1].Position.Z;
            var z2 = skel.Joints[joint2].Position.Z;

            return (float)Math.Atan((y1 - y2) / (z1 - z2));
        }

        public static float GetRotationX2(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var y1 = skel.Joints[joint1].Position.Y;
            var y2 = skel.Joints[joint2].Position.Y;
            var z1 = skel.Joints[joint1].Position.Z;
            var z2 = skel.Joints[joint2].Position.Z;

            return (float)Math.Atan((z1 - z2) / (y1 - y2));
        }

        public static float GetRotationY(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var x1 = skel.Joints[joint1].Position.X;
            var x2 = skel.Joints[joint2].Position.X;
            var z1 = skel.Joints[joint1].Position.Z;
            var z2 = skel.Joints[joint2].Position.Z;

            return (float)Math.Atan((z1 - z2) / (x1 - x2));
        }

        public static float GetRotationY2(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var x1 = skel.Joints[joint1].Position.X;
            var x2 = skel.Joints[joint2].Position.X;
            var z1 = skel.Joints[joint1].Position.Z;
            var z2 = skel.Joints[joint2].Position.Z;

            return (float)Math.Atan((x1 - x2) / (z1 - z2));
        }

        public static float GetRotationZ(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var x1 = skel.Joints[joint1].Position.X;
            var x2 = skel.Joints[joint2].Position.X;
            var y1 = skel.Joints[joint1].Position.Y;
            var y2 = skel.Joints[joint2].Position.Y;

            return (float)Math.Atan((y1 - y2) / (x1 - x2));
        }

        public static float GetRotationZ2(this Skeleton skel, JointType joint1, JointType joint2)
        {
            var x1 = skel.Joints[joint1].Position.X;
            var x2 = skel.Joints[joint2].Position.X;
            var y1 = skel.Joints[joint1].Position.Y;
            var y2 = skel.Joints[joint2].Position.Y;

            return (float)Math.Atan((x1 - x2) / (y1 - y2));
        }
    }
}
