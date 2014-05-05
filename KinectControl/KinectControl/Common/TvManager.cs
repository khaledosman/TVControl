﻿using Microsoft.Xna.Framework;

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

            if (leftAngle > -0.2f) // VOLUME!
            {
                volume += MathHelper.Clamp(rightAngle, -0.4f, 0.4f) / 4f;
                if (volume < 0)
                    volume = 0;
                
                Status = "Volume: " + Volume;
            }
            else if (leftAngle > -0.4f) // CHANNEL!
            {
                channel += MathHelper.Clamp(rightAngle, -0.4f, 0.4f) / 4f;
                if (channel < 0)
                    channel = 0;
                Status = "Channel: " + Channel;
            }
            else if (leftAngle > -0.6f) // BRIGHTNESS
            {
                brightness += MathHelper.Clamp(rightAngle, -0.4f, 0.4f) / 4f;
                if (brightness < 0)
                    brightness = 0;
                Status = "Brightness: " + Brightness;
            }
        }
    }
}
