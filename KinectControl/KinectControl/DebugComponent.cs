using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using KinectControl.Common;

namespace KinectControl
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DebugComponent : DrawableGameComponent
    {
        public volatile static string DebugString;

        SpriteBatch spriteBatch;
        SpriteFont font;

        MouseState currMouse, lastMouse;

        double avgFrameTime;

        static Stopwatch stopwatch;
        static float stopwatchAvg;

        public DebugComponent(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            DebugExtensions.Values = new Dictionary<float, float>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Game.Content.Load<SpriteFont>("DebugFont");

            base.LoadContent();
        }

        public static void StartStopwatch()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static void StopStopwatch()
        {
            if (stopwatchAvg == 0)
                stopwatchAvg = stopwatch.ElapsedMilliseconds;
            else
                stopwatchAvg = stopwatchAvg * 0.99f + stopwatch.ElapsedMilliseconds * 0.01f;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            lastMouse = currMouse;
            currMouse = Mouse.GetState();

            if (currMouse.LeftButton == ButtonState.Pressed)
                DebugExtensions.DeltaVal++;
            if (currMouse.RightButton == ButtonState.Pressed)
                DebugExtensions.DeltaVal--;

            DebugExtensions.DeltaVal += (currMouse.ScrollWheelValue - lastMouse.ScrollWheelValue) / 120f;

            if (currMouse.MiddleButton == ButtonState.Pressed)
                DebugExtensions.DeltaVal = 0;

            if (DebugExtensions.Values.Count > 0)
            {
                DebugString = "Debug Values:\r\n";
                foreach (var f in DebugExtensions.Values)
                    DebugString += f.Value + "\r\n";
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (font == null) return;

            spriteBatch.Begin();

            if (stopwatchAvg != 0)
                DebugString = "sw: " + stopwatchAvg;

            if (!String.IsNullOrEmpty(DebugString))
                spriteBatch.DrawString(font, DebugString, new Vector2(10), Color.OrangeRed);
            DebugString = "";

            if (avgFrameTime == 0)
                avgFrameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                avgFrameTime = avgFrameTime * 0.99f + gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
           // spriteBatch.DrawString(font,
             //   gameTime.ElapsedGameTime.TotalMilliseconds.ToString("0.00") + " / " + avgFrameTime.ToString("0.00"),
               // new Vector2(10, Constants.screenHeight - 60), Color.OrangeRed);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public static class DebugExtensions
    {
        public static float DeltaVal;

        public static Dictionary<float, float> Values;

        public static float Debug(this float val, float scale = 1)
        {
            return Values[val] = val + DeltaVal * scale;
        }

        public static T Show<T>(this T obj)
        {
            if (!string.IsNullOrEmpty(DebugComponent.DebugString))
                DebugComponent.DebugString += "\r\n";
            DebugComponent.DebugString += obj.ToString();
            return obj;
        }
    }
}
