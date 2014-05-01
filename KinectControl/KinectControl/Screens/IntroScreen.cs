using KinectControl.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectControl.Screens
{
    /// <summary>
    /// This class Represents the Terasoft screen.
    /// </summary>
    class IntroScreen : FadingScreen
    {
        private float scale;
        private Texture2D gucLogo;
        /// <summary>
        /// Creates a new instance of the TeraSoftScreen.
        /// </summary>
        public IntroScreen()
            : base("Textures/GUC", 0.6f, 0, 0, -0.06f)
        {
            scale = 0.3f;
        }

        /// <summary>
        /// Loads the content of this screen.
        /// </summary>
        public override void LoadContent()
        {
            gucLogo = ScreenManager.Game.Content.Load<Texture2D>(@"Textures/GUC");
            base.LoadContent();
        }
        /// <summary>
        /// Updates the content of this screen.
        /// </summary>
        /// <remarks><para>AUTHOR: Ahmed Badr.</para></remarks>
        /// <param name="gameTime">Represents the time of the game.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Done)
            {
                base.Remove();
                ScreenManager.AddScreen(new MainScreen());
            }
        }

        /// <summary>
        /// Draws the content of this screen.
        /// </summary>
        /// <remarks><para>AUTHOR: Ahmed Badr.</para></remarks>
        /// <param name="gameTime">Represents the time of the game.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.White);
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(gucLogo, new Vector2(((ScreenManager.GraphicsDevice.Viewport.Width
                - gucLogo.Width * scale) / 1.1f), ((ScreenManager.GraphicsDevice.Viewport.Height - (gucLogo.Height)
                * scale) / 1.1f)), null, Color.White, 0, new Vector2(0, 0), new Vector2(scale, scale),
                SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);

        }
    }
}
