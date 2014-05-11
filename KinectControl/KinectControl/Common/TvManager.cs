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
            volume = 0;
            channel = 0;
            brightness = 50;
        }

        public void UpdateValues(float leftAngle, float rightAngle, bool isControlling)
        {
            if (!isControlling)
            {
                volume = (int)volume;
                channel = (int)channel;
                brightness = (int)brightness;
                Status = "";

                return;
            }

            var deadzone = 0.08f;

            if (leftAngle > -0.3f) // VOLUME!
            {
                if (Math.Abs(rightAngle) > deadzone)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    volume += MathHelper.Clamp(rightAngle.Show(), -0.6f, 0.6f) / 4f;
                    volume = MathHelper.Clamp(volume, 0, 20);
                }

                Status = "Volume: " + Volume;
            }
            else if (leftAngle > -0.6f) // CHANNEL!
            {
                if (Math.Abs(rightAngle) > deadzone)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    channel += MathHelper.Clamp(rightAngle, -0.6f, 0.6f) / 6f;
                }
                if (channel < 0)
                    channel = 0;

                Status = "Channel: " + Channel;
            }
            else if (leftAngle > -0.9f) // BRIGHTNESS
            {
                if (Math.Abs(rightAngle) > deadzone)
                {
                    if (rightAngle > 0) rightAngle -= deadzone;
                    else rightAngle += deadzone;

                    brightness += MathHelper.Clamp(rightAngle, -0.6f, 0.6f) / 4f;
                }
                if (brightness < 0)
                    brightness = 0;

                Status = "Brightness: " + Brightness;
            }
        }
    }
}
