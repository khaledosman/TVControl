using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using KinectControl.Common;
using System.Collections.Generic;
using System;
using KinectControl.Screens;

namespace KinectControl.UI
{
    #region ScreenState
    /// <summary>
    /// Represents the screen states.
    /// </summary>
    public enum ScreenState
    {
        Active,
        Frozen,
        Hidden
    }
    #endregion

    /// <summary>
    /// This class represents a screen.
    /// </summary>
    public abstract class GameScreen
    {
        //private MediaLibrary sampleMediaLibrary;
        private Random random;
        //private int playQueue;
        private ContentManager content;
        //private Song[] songsarray;
        private SpriteBatch spriteBatch;
        protected PrimitiveBatch PrimitiveBatch { get; private set; }
        public bool enablePause;
        public bool screenPaused;
        private SpriteFont font;
        private int frameNumber;
        public int FrameNumber
        {
            get
            {
                return frameNumber;
            }
            set
            {
            }
        }
        private UserAvatar userAvatar;
        //private List<Song> songs;
        public UserAvatar UserAvatar
        {
            get { return userAvatar; }
            set { userAvatar = value; }
        }
        private VoiceCommands voiceCommands;

        public bool IsFrozen
        {
            get
            {
                return screenState == ScreenState.Frozen;
            }
        }

        private ScreenState screenState;
        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }
        private ScreenManager screenManager;
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }

        public bool IsActive
        {
            get
            {
                return screenState == ScreenState.Active;
            }
        }
        public bool showAvatar = true;
        /// <summary>
        /// LoadContent will be called only once before drawing and it's the place to load
        /// all of your content.
        /// </summary>
        public virtual void LoadContent()
        {
            frameNumber = Kinect.FramesCount;
            content = ScreenManager.Game.Content;
            spriteBatch = ScreenManager.SpriteBatch;
            PrimitiveBatch = new PrimitiveBatch(ScreenManager.GraphicsDevice);
            font = content.Load<SpriteFont>("SpriteFont1");
         //   songs = MyExtension.LoadListContent<Song>(content, "Audio\\");
            //songsarray = songs.ToArray();
           // sampleMediaLibrary = new MediaLibrary();
            random = new Random();
            //MediaPlayer.Stop(); // stop current audio playback 
            // generate a random valid index into Albums
            voiceCommands = ScreenManager.Kinect.voiceCommands;
            if (showAvatar)
            {
                userAvatar = new UserAvatar(ScreenManager.Kinect, content, ScreenManager.GraphicsDevice, spriteBatch);
                userAvatar.LoadContent();
            }
        }
        /// <summary>
        /// Initializes the GameScreen.
        /// </summary
        public virtual void Initialize()
        {
            screenPaused = false;
        }

        /// <summary>
        /// Unloads the content of GameScreen.
        /// </summary>
        public virtual void UnloadContent() {
        }
        /// <summary>
        /// Allows the game screen to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (showAvatar)
            {
                userAvatar.Update(gameTime);
                if (!IsFrozen)
                if (enablePause)
                {
                    if (userAvatar.Avatar == userAvatar.AllAvatars[0])
                    {
                        //Freeze Screen, Show pause Screen\
                        screenPaused = true;
                        ScreenManager.AddScreen(new PopupScreen());
                        this.FreezeScreen();
                    }
                    else if (userAvatar.Avatar.Equals(userAvatar.AllAvatars[2]) && screenPaused == true)
                    {
                        //exit pause screen, unfreeze screen
                        this.UnfreezeScreen();
                    }
                }
            }

            if (frameNumber % 360 == 0 && voiceCommands!=null)
            {
                voiceCommands.HeardString = "";
            }
            frameNumber++;
            /*if (voiceCommands != null)
            {
                switch (voiceCommands.HeardString)
                {
                    case "play project music":
                        if (MediaPlayer.State.Equals(MediaState.Stopped) || MediaPlayer.State.Equals(MediaState.Paused))
                        {
                            playQueue = random.Next(songsarray.Length-1);
                            MediaPlayer.Play(songsarray[playQueue]);
                            /*
                                                        screenManager.AddScreen(new PauseScreen());
                             * MediaPlayer.PlayPosition.Minutes;
                             *
                        }
                        break;
                    case "stop":
                        if (MediaPlayer.State.Equals(MediaState.Playing))
                            MediaPlayer.Pause();
                        break;

                    case "play mediaplayer":
                        if (MediaPlayer.State.Equals(MediaState.Stopped) || MediaPlayer.State.Equals(MediaState.Paused))
                        {
                            //int i = random.Next(0, sampleMediaLibrary.Songs.Count - 1);
                            //MediaPlayer.Play(sampleMediaLibrary.Songs[i]);
                            MediaPlayer.Play(sampleMediaLibrary.Songs);
                        }
                        break;
                    case "resume":
                        if (MediaPlayer.State.Equals(MediaState.Paused))
                            MediaPlayer.Resume();
                        break;
                    case "next":
                        //MediaPlayer.MoveNext();
                        //MediaPlayer.Stop();
                        MediaPlayer.MoveNext();
                        //MediaPlayer.Resume();
                        //MediaPlayer.Play(sampleMediaLibrary.Songs[++playQueue]);
                        break;
                    case "previous":
                        //MediaPlayer.Stop();
                        MediaPlayer.MovePrevious();
                        //MediaPlayer.Resume();
                        //MediaPlayer.Play(sampleMediaLibrary.Songs[--playQueue]);
                        break;
                    case "mute":
                        MediaPlayer.IsMuted = true;
                        break;
                    case "unmute":
                        MediaPlayer.IsMuted = false;
                        break;
                    case "device one":
                        if (ScreenManager.Kinect.devices[0].IsSwitchedOn)
                            ScreenManager.Kinect.devices[0].switchOff(ScreenManager.Kinect.comm);
                        else
                            ScreenManager.Kinect.devices[0].switchOn(ScreenManager.Kinect.comm);
                        //this.FreezeScreen();
                        screenManager.AddScreen(new PopupScreen(ScreenManager.Kinect.devices[0].Name +" is "+ScreenManager.Kinect.devices[0].Status,300));
                        voiceCommands.HeardString = "";
                        break;
                    case "device two":
                        if (ScreenManager.Kinect.devices[1].IsSwitchedOn)
                            ScreenManager.Kinect.devices[1].switchOff(ScreenManager.Kinect.comm);
                        else
                            ScreenManager.Kinect.devices[1].switchOn(ScreenManager.Kinect.comm);
                        screenManager.AddScreen(new PopupScreen(ScreenManager.Kinect.devices[1].Name + " is " + ScreenManager.Kinect.devices[1].Status,300));
                        //this.FreezeScreen();
                        voiceCommands.HeardString = "";
                        break;
                    //case "volume up":
                    //    MediaPlayer.Volume++;
                    //    voiceCommands.HeardString="";
                    //    break;
                    //case "volume down":
                    //    MediaPlayer.Volume--;
                    //    voiceCommands.HeardString = "";
                    //    break;
                    default: break;
                }
            }*/
        }
             

        /// <summary>
        /// Removes the current screen.
        /// </summary>
        public virtual void Remove()
        {
            screenManager.RemoveScreen(this);
        }

        /// <summary>
        /// This is called when the game screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            if (showAvatar)
                userAvatar.Draw(gameTime);
            spriteBatch.Begin();
            if(voiceCommands!=null && !voiceCommands.HeardString.Equals(""))
            spriteBatch.DrawString(font,"voice command: " + voiceCommands.HeardString, new Vector2(300,300), Color.Orange);
            spriteBatch.End();
        }
        public void FreezeScreen()
        {
            screenState = ScreenState.Frozen;
        }

        public void UnfreezeScreen()
        {
            screenState = ScreenState.Active;
        }


    }
}