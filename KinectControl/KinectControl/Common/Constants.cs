
namespace KinectControl.Common
{
    class Constants
    {
        public const int hPad = 32;
        public const int vPad = 16;
        public const int screenWidth = 1280;
        public const int screenHeight = 720;
        public const int InvalidTrackingId = 0;
        /// <summary>
        /// Height of interaction region in UI coordinates.
        /// </summary>
        public const double InteractionRegionHeight = 768.0;

        /// <summary>
        /// Width of interaction region in UI coordinates.
        /// </summary>
        public const double InteractionRegionWidth = 1024.0;
        public const float SkeletonMaxX = 0.60f;
        public const float SkeletonMaxY = 0.40f;


        public static void ResetFlags()
        {
        }
    }
}
