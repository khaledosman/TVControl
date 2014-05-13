using Microsoft.Xna.Framework.Graphics;
using KinectControl.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using KinectControl.Common;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace KinectControl.Screens
{
    class MainScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private SpriteFont font2;
        private Kinect kinect;
        private string gesture;
        private GraphicsDevice graphics;
        private int screenWidth, screenHeight;
        //private Button button;
        //private HandCursor hand;
        private ContentManager content;
        private Texture2D gradientTexture;
        //private string textToDraw;
        private string text;
        //private Rectangle textBox;
        //private Vector2 textPosition;
        private Skeleton skel;
        Video video;
        VideoPlayer player;
        Texture2D videoTexture;

        TvManager tv;

        PopupScreen tvPopup;

        Texture2D leftArcTex, rightArcTex;
        Texture2D arrowTex;

        float leftAngle, leftDist;
        float rightAngle, rightDist;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public MainScreen()
        {

        }

        public override void Initialize()
        {
            //showAvatar = false;
            enablePause = false;
            /*        button = new Button();
                    hand = new HandCursor();
                    hand.Initialize(ScreenManager.Kinect);
                    button.Initialize("Buttons/ok", this.ScreenManager.Kinect, new Vector2(820, 350));
                    button.Clicked += new Button.ClickedEventHandler(button_Clicked);
                    Text = "1)This Application allows you to control home devices by providing voice and gesture recognition systems. \n2)The avatar on top right represents your distance from the kinect sensor.";
                    textPosition = new Vector2(75, 145);
                    textBox = new Rectangle((int)textPosition.X, (int)textPosition.Y, 1020, 455);*/

            tv = new TvManager();
            tvPopup = new PopupScreen("");
            ScreenManager.AddScreen(tvPopup);

            base.Initialize();
        }
        void button_Clicked(object sender, System.EventArgs a)
        {
            // ScreenManager.Kinect.comm.ClosePort();
            //this.Remove();
            kinect.CloseKinect(kinect.nui);
            this.ScreenManager.Game.Exit();
        }
        public override void LoadContent()
        {
            kinect = ScreenManager.Kinect;
            gesture = kinect.Gesture;
            content = ScreenManager.Game.Content;
            graphics = ScreenManager.GraphicsDevice;
            spriteBatch = ScreenManager.SpriteBatch;
            screenHeight = graphics.Viewport.Height;
            screenWidth = graphics.Viewport.Width;
            gradientTexture = content.Load<Texture2D>("Textures/gradientTexture");
            font = content.Load<SpriteFont>("SpriteFont1");
            font2 = content.Load<SpriteFont>("Fontopo");
            video = content.Load<Video>("video");
            //video = content.Load<Video>("Videos\\Wildlife");
            player = new VideoPlayer();
            //font2.LineSpacing = 21;
            //hand.LoadContent(content);
            //button.LoadContent(content);
            //textToDraw = WrapText(font2, text, 9000);

            leftArcTex = content.Load<Texture2D>("Textures/Left Arc");
            rightArcTex = content.Load<Texture2D>("Textures/Right Arc");
            arrowTex = content.Load<Texture2D>("Textures/Arrow");

            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            //  hand.Update(gameTime);
            //button.Update(gameTime);
            //button.Clicked += button_Clicked;
            gesture = kinect.Gesture;
            if (FrameNumber % 240 == 0)
            {
                kinect.Gesture = "";
                tvPopup.message = "";
            }
            if (player.State == MediaState.Stopped)
            {
                player.IsLooped = true;
                player.Play(video);
            }

            skel = kinect.trackedSkeleton;
            if ((skel != null))
            //&& skel.Joints[JointType.WristLeft].Position.Y > skel.Joints[JointType.ElbowLeft].Position.Y) &&
            //skel.Joints[JointType.WristRight].Position.Y > skel.Joints[JointType.ElbowRight].Position.Y)
            {
                leftAngle = skel.GetRotationZ2(JointType.WristLeft, JointType.HipLeft) + 0.32f;
                if (skel.GetDistanceY(JointType.WristLeft, JointType.HipLeft) < 0)
                    leftAngle -= MathHelper.Pi;
                leftAngle = MathHelper.Clamp(leftAngle, -MathHelper.PiOver2, MathHelper.PiOver2);
                
                leftDist = skel.GetDistance(JointType.WristLeft, JointType.HipLeft, 'z') - 0.2f;

                rightAngle = skel.GetRotationZ2(JointType.WristRight, JointType.HipRight) - 0.32f;
                if (skel.GetDistanceY(JointType.WristRight, JointType.HipRight) < 0)
                    rightAngle += MathHelper.Pi;
                rightAngle = MathHelper.Clamp(rightAngle, -MathHelper.PiOver2, MathHelper.PiOver2);
                
                rightDist = skel.GetDistance(JointType.WristRight, JointType.HipRight, 'z') - 0.2f;

                tv.UpdateValues(leftAngle, rightAngle, leftDist > 0, rightDist > 0);
                tvPopup.message = tv.Status;
                // tvPopup.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder builder = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    builder.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    builder.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return builder.ToString();
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(gradientTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            if (!(gesture.Equals("")))
                tvPopup.message = gesture;
            // spriteBatch.DrawString(font, "gesture recognized: " + gesture, new Vector2(500, 500), Color.Orange);
            if (player.State != MediaState.Stopped)
                videoTexture = player.GetTexture();

            // Drawing to the rectangle will stretch the 
            // video to fill the screen
            Rectangle screen = new Rectangle(graphics.Viewport.X,
                graphics.Viewport.Y,
                graphics.Viewport.Width,
                graphics.Viewport.Height);

            // Draw the video, if we have a texture to draw.
            if (videoTexture != null)
            {
                //spriteBatch.Begin();
                spriteBatch.Draw(videoTexture, screen, Color.White);
                //spriteBatch.End();
            }

            //spriteBatch.DrawString(font2, textToDraw, textPosition, Color.White);
            //   button.Draw(spriteBatch);
            // hand.Draw(spriteBatch);
            if (skel != null)
                if (leftDist > 0)
                //skel.Joints[JointType.WristLeft].Position.Y > skel.Joints[JointType.ElbowLeft].Position.Y &&
                //skel.Joints[JointType.WristRight].Position.Y > skel.Joints[JointType.ElbowRight].Position.Y)
                {
                    spriteBatch.Draw(leftArcTex, new Vector2(180, 660), null, Color.White, 0,
                        new Vector2(160, 160), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(arrowTex, new Vector2(180, 660), null, Color.OrangeRed, leftAngle,
                        new Vector2(7, 160), 1f, SpriteEffects.None, 0);

                    spriteBatch.Draw(rightArcTex, new Vector2(1100, 660), null, Color.White, 0,
                        new Vector2(160, 160), 1f, SpriteEffects.None, 0);
                    if (rightDist > 0)
                        spriteBatch.Draw(arrowTex, new Vector2(1100, 660), null, Color.OrangeRed, rightAngle,
                            new Vector2(7, 160), 1f, SpriteEffects.None, 0);
                    showAvatar = true;
                }

            spriteBatch.End();
            tvPopup.Draw(gameTime);

            //PrimitiveBatch.Begin(PrimitiveType.TriangleList, null);

            //var leftTrans = Matrix.CreateRotationZ(leftAngle);

            //PrimitiveBatch.AddVertex(new Vector2(-580, 280), Color.OrangeRed, Vector2.Zero);
            //PrimitiveBatch.AddVertex(new Vector2(-588, 280), Color.OrangeRed, Vector2.Zero);
            //PrimitiveBatch.AddVertex(new Vector2(-584, 220), Color.OrangeRed, Vector2.Zero);

            //PrimitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}
