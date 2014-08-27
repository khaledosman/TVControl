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
        private VideoPlayer currentPlayer;
        public bool joiningHands;
        private bool shouldPlay;
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
        //Video video;
        Video[] videos;
        VideoPlayer[] players;
        Texture2D videoTexture;

        TvManager tv;

        PopupScreen tvPopup;

        Texture2D leftArcTex, rightArcTex;
        Texture2D arrowTex;

        float leftAngle, leftDist;
        float rightAngle, rightDist;

        Texture2D whitePixel;

        float suppressUiTimer;

        Gesture joinZoom;

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
            videos = new Video[4];
            /*        button = new Button();
                    hand = new HandCursor();
                    hand.Initialize(ScreenManager.Kinect);
                    button.Initialize("Buttons/ok", this.ScreenManager.Kinect, new Vector2(820, 350));
                    button.Clicked += new Button.ClickedEventHandler(button_Clicked);
                    Text = "1)This Application allows you to control home devices by providing voice and gesture recognition systems. \n2)The avatar on top right represents your distance from the kinect sensor.";
                    textPosition = new Vector2(75, 145);
                    textBox = new Rectangle((int)textPosition.X, (int)textPosition.Y, 1020, 455);*/

            tv = new TvManager();
            tvPopup = new PopupScreen("", 240);
            ScreenManager.AddScreen(tvPopup);

            suppressUiTimer = 0;

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
            videos[0] = content.Load<Video>("Videos/1");
            videos[1] = content.Load<Video>("Videos/2");
            videos[2] = content.Load<Video>("Videos/3");
            videos[3] = content.Load<Video>("Videos/4");
            //    videos[4] = content.Load<Video>("Videos/5");
            //video = content.Load<Video>("Videos\\Wildlife");
            players = new VideoPlayer[4];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new VideoPlayer();
                players[i].IsLooped = true;
                players[i].Play(videos[i]);
                players[i].Pause();
                players[i].IsMuted = true;
            }
            currentPlayer = players[0];
            currentPlayer.Resume();
            //font2.LineSpacing = 21;
            //hand.LoadContent(content);
            //button.LoadContent(content);
            //textToDraw = WrapText(font2, text, 9000);

            leftArcTex = content.Load<Texture2D>("Textures/Left Arc");
            rightArcTex = content.Load<Texture2D>("Textures/Right Arc");
            arrowTex = content.Load<Texture2D>("Textures/Arrow");

            whitePixel = new Texture2D(graphics, 1, 1);
            whitePixel.SetData(new[] { Color.White });

            joinZoom = kinect.gestureController.gestures.Find(g => g.type == GestureType.JoinedZoom);

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
            if (FrameNumber % 120 == 0)
                joiningHands = false;
            if (voiceCommands != null)
            {
                if (voiceCommands.HeardString.Equals("Open"))
                {
                    shouldPlay = true;
                    foreach (var player in players)
                        player.IsMuted = false;
                }
                if (voiceCommands.HeardString.Equals("Close"))
                {
                    shouldPlay = false;
                    foreach (var player in players)
                        player.IsMuted = true;
                }
            }
            //if (gesture.Equals("Zoom out"))
            //    joiningHands = true;
            if (gesture.Equals("Joined Zoom"))
            {
                if (shouldPlay)
                {
                    shouldPlay = false;
                    //player.Volume = 0;
                    foreach (var player in players)
                        player.IsMuted = true;
                }
                else
                {
                    shouldPlay = true;
                    //player.Volume = tv.Volume / 100f;
                    foreach (var player in players)
                        player.IsMuted = false;
                }
                kinect.Gesture = "";
            }

            if (joinZoom.currentGesturePart >= 1 && joinZoom.currentGesturePart <= 10)
                joiningHands = true;

            if (joiningHands)
                suppressUiTimer = 2;

            if (suppressUiTimer > 0)
                suppressUiTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                if (suppressUiTimer > 0 || !shouldPlay)
                {
                    leftDist = 0;
                    rightDist = 0;
                }

                var lastCh = tv.Channel;
                tv.UpdateValues(leftAngle, rightAngle, leftDist > 0, rightDist > 0);

                foreach (var player in players)
                    player.Volume = (float)tv.Volume / 100;

                if (tv.Channel != lastCh)
                {
                    currentPlayer.Pause();
                    currentPlayer = players[tv.Channel - 1];
                    currentPlayer.Resume();
                }

                tvPopup.message = tv.Status;
            }

            base.Update(gameTime);
        }
        public override void Remove()
        {
            ScreenManager.AddScreen(new BlankScreen());
            base.Remove();
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
            //spriteBatch.Draw(gradientTexture, new Rectangle(0, 0, 1280, 720), Color.White);
      //      if (!(gesture.Equals("")))
        //        tvPopup.message = gesture;
            // spriteBatch.DrawString(font, "gesture recognized: " + gesture, new Vector2(500, 500), Color.Orange);
            if (shouldPlay)
            {
                if (currentPlayer.State != MediaState.Stopped)
                    videoTexture = currentPlayer.GetTexture();
            }

            // Drawing to the rectangle will stretch the 
            // video to fill the screen
            Rectangle screen = new Rectangle(graphics.Viewport.X,
                graphics.Viewport.Y,
                graphics.Viewport.Width,
                graphics.Viewport.Height);

            // Draw the video, if we have a texture to draw.
            if (videoTexture != null && shouldPlay)
            {
                spriteBatch.Draw(videoTexture, screen, Color.White);

                var br = tv.Brightness / 50f - 1f;
                if (br > 0)
                    spriteBatch.Draw(whitePixel, screen, new Color(br, br, br, br));
                else
                    spriteBatch.Draw(whitePixel, screen, new Color(0f, 0f, 0f, -br));
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
