using KinectControl.UI;
using Microsoft.Xna.Framework;
using KinectControl.Common;

namespace KinectControl.Screens
{
    public class BlankScreen : GameScreen
    {
        string gesture;
        Kinect kinect;
        PopupScreen tvPopup;
        public override void LoadContent()
        {
            kinect = ScreenManager.Kinect;
            gesture = kinect.Gesture;
            base.LoadContent();
        }
        public override void Initialize()
        {
            //tvPopup = new PopupScreen("", 240);
            tvPopup = new PopupScreen("");
            ScreenManager.AddScreen(tvPopup);
            base.Initialize();
        }
        public override void Draw(GameTime gameTime)
        {
            if (!(gesture.Equals("")))
                tvPopup.message = gesture;
            tvPopup.Draw(gameTime);
            base.Draw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            gesture = kinect.Gesture;
            if (gesture.Equals("Joined Zoom"))
            {
                    ScreenManager.AddScreen(new MainScreen());
                    kinect.Gesture = "";
                    this.Remove();
            }
            if (FrameNumber % 240 == 0)
            {
                kinect.Gesture = "";
                tvPopup.message = "";
            }
            base.Update(gameTime);
        }
        public override void Remove()
        {
            base.Remove();
        }
    }
}
