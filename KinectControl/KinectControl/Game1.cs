using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using KinectControl.UI;
using KinectControl.Screens;
using KinectControl.Common;

namespace KinectControl
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;
        public Kinect Kinect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Constants.screenWidth;
            graphics.PreferredBackBufferHeight = Constants.screenHeight;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            Kinect = new Kinect();
            screenManager = new ScreenManager(this, Kinect);
            Components.Add(screenManager);
            Components.Add(new DebugComponent(this));
        }

        protected override void Initialize()
        {
            //initializations
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenManager.AddScreen(new IntroScreen());
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
