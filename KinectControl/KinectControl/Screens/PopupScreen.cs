using KinectControl.UI;
using KinectControl.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KinectControl.Screens
{
    public class PopupScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private GraphicsDevice graphics;
        private int screenWidth;
        private int screenHeight;
        private int counter;
        private ContentManager content;
        public string message;
        private Texture2D gradientTexture;

        public PopupScreen() { message = "No user detected, Game paused"; counter = 1; showAvatar = false; }
        public PopupScreen(string message) { this.message = message; counter = 1; showAvatar = false; }
        public PopupScreen(string message, int counter) { this.message = message; this.counter = counter; showAvatar = false; }

        public override void LoadContent()
        {
            content = ScreenManager.Game.Content;
            graphics = ScreenManager.GraphicsDevice;
            spriteBatch = ScreenManager.SpriteBatch;
            screenHeight = graphics.Viewport.Height;
            screenWidth = graphics.Viewport.Width;
            //gradientTexture = content.Load<Texture2D>("Textures\\gradient");
            gradientTexture = new Texture2D(graphics, 1, 1);
            gradientTexture.SetData(new[] { new Color(40, 80, 40) });
            font = content.Load<SpriteFont>("SpriteFont1");
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            counter--;
            if (counter == 0)
            {
                this.Remove();
                //UnfreezeScreen();
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Vector2 viewportSize = new Vector2(screenWidth, screenHeight);
            Vector2 textSize = font.MeasureString(message ?? "");
            Vector2 textPosition = (viewportSize - textSize) / 2;
            int hPad = Constants.hPad;
            int vPad = Constants.vPad;
            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            spriteBatch.Begin();
            if (!string.IsNullOrEmpty(message))
                spriteBatch.Draw(gradientTexture, backgroundRectangle, Color.White);
            if(message!=null)
            spriteBatch.DrawString(font, message, textPosition, Color.Orange);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
