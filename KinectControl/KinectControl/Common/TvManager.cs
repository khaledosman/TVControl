using Microsoft.Xna.Framework;
using System;

namespace KinectControl.Common
{
    public class TvManager
    {
        private float volume;
        public int Volume { get { return (int)volume; } }

        private float channel;
        public int Channel { get { return (int)channel; } }

        private float brightness;
        public int Brightness { get { return (int)brightness; } }

        public string Status { get; private set; }

        public TvManager()
        {
            volume = 20;
            channel = 0;
            brightness = 50;
        }

        public void UpdateValues(float leftAngle, float rightAngle, bool isLeftCtrl, bool isRightCtrl)
        {
            if (!isLeftCtrl)
            {
                volume = (int)volume;
                channel = (int)channel;
                brightness = (int)brightness;
                Status = "";

                return;
            }

            var deadzone = 0.08f;
            var delta = MathHelper.ToRadians(30);

            if (leftAngle > 0)
                Status = "";
            else if (leftAngle > -delta) // VOLUME!
            {
                if (Math.Abs(rightAngle) > deadzone && isRightCtrl)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    volume += MathHelper.Clamp(rightAngle, -MathHelper.PiOver2, MathHelper.PiOver2) / 4f;
                    volume = MathHelper.Clamp(volume, 0, 100);
                }

                Status = "Volume: " + Volume;
            }
            else if (leftAngle > -delta * 2) // CHANNEL!
            {
                if (Math.Abs(rightAngle) > deadzone && isRightCtrl)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    channel += MathHelper.Clamp(rightAngle, -MathHelper.PiOver2, MathHelper.PiOver2) / 6f;
                    channel = MathHelper.Clamp(channel, 0, 100);
                }

                Status = "Channel: " + Channel;
            }
            else if (leftAngle > -delta * 3) // BRIGHTNESS!
            {
                if (Math.Abs(rightAngle) > deadzone && isRightCtrl)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    brightness += MathHelper.Clamp(rightAngle, -MathHelper.PiOver2, MathHelper.PiOver2) / 4f;
                    brightness = MathHelper.Clamp(brightness, 0, 100);
                }

                Status = "Brightness: " + Brightness;
            }
        }
    }
}
