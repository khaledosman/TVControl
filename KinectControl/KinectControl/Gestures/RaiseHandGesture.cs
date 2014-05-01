using Microsoft.Kinect;

namespace KinectControl.Common
{
    class RaiseHandSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
                if (skeleton.Joints[JointType.HandRight].Position.X >= skeleton.Joints[JointType.HipLeft].Position.X)
                    return GesturePartResult.Succeed;
                else return GesturePartResult.Pausing;
            else return GesturePartResult.Fail;
        }
    }
    class RaiseHandSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                    return GesturePartResult.Succeed;
                else return GesturePartResult.Pausing;
            else return GesturePartResult.Fail;
        }
    }
}
