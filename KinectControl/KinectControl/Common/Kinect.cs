using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Kinect;
using System.Threading;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect.Toolkit;
using System.Collections.Generic;

namespace KinectControl.Common
{
   
    public class Kinect
    {
        #region Gestures variables
        private string _gesture;
        private GestureController gestureController;
        public String Gesture
        {
            get { return _gesture; }

            set
            {
                if (_gesture == value)
                    return;
                _gesture = value;
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }
        #endregion

        #region KinectInteraction
        private MyInteractionClient myInteractionClient;
        private DepthImagePixel[] depthBuffer;
        /// <summary>
        /// Intermediate storage for the user information received from interaction stream.
        /// </summary>
        private UserInfo[] userInfos;
        /// <summary>
        /// Entry point for interaction stream functionality.
        /// </summary>
        private InteractionStream interactionStream;
        #endregion

        #region Kinect variables
        /// <summary>
        /// Sensor chooser used to obtain a Kinect sensor, if one is available.
        /// </summary>
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();
        private VoiceCommands _voiceCommands;
        public event PropertyChangedEventHandler PropertyChanged;
        private static int framesCount;

        private Skeleton[] skeletons;
        public KinectSensor nui;
        public Skeleton trackedSkeleton;
        private int ScreenWidth, ScreenHeight;
        /// <summary>
        /// Keeps track of set of interacting users.
        /// </summary>
        private readonly HashSet<int> trackedUsers = new HashSet<int>();
        public static int FramesCount
        {
            get { return framesCount; }
            set { framesCount = value; }
        }
        private string[] commands;
        public VoiceCommands voiceCommands
        {
            get { return _voiceCommands; }
            set { _voiceCommands = value; }
        }
        #endregion

        #region Communication & Devices
        public CommunicationManager comm;
        public Device[] devices;
        #endregion

        public Kinect(int screenWidth, int screenHeight)
        {
            skeletons = new Skeleton[0];
            trackedSkeleton = null;
            //swapHand = new SwapHand();
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth; 
            this.InitializeNui();
        }
        #region destructors
        public void closeKinect()
        {
            this.sensorChooser.Stop();
        }
        /// <summary>
        /// Clean up interaction stream and associated data structures.
        /// </summary>
        /// <param name="sensor">
        /// Sensor from which we were streaming depth and skeleton data.
        /// </param>
        private void UninitializeInteractions(KinectSensor sensor)
        {
            nui.DepthFrameReady -= this.SensorDepthFrameReady;
            nui.SkeletonFrameReady -= this.OnSkeletonFrameReady;

            this.skeletons = null;
            this.userInfos = null;

            this.interactionStream.InteractionFrameReady -= this.InteractionFrameReady;
            this.interactionStream.Dispose();
            this.interactionStream = null;
        }


        #endregion

        #region Initialize metohds
        /// <summary>
        /// Handle insertion of Kinect sensor.
        /// </summary>
        private void InitializeNui()
        {
            _gesture = "";
            var index = 0;
            this.sensorChooser.KinectChanged += this.OnSensorChanged;
            while (this.nui == null && index < KinectSensor.KinectSensors.Count)
            {
                this.nui = KinectSensor.KinectSensors[index];
                try
                {
                    this.nui.Start();
                }
               catch(Exception)
                {
                }
            }
            try
            {
                this.skeletons = new Skeleton[nui.SkeletonStream.FrameSkeletonArrayLength];
                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.75f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };
                this.nui.SkeletonStream.Enable(parameters);
                this.nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.nui.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            }
            catch (Exception)
            { return; }
            this.nui.SkeletonFrameReady += this.OnSkeletonFrameReady;
            InitializeInteractions();
            InitializeGestures();
            InitializeVoiceGrammar();
            InitializeDevices();
        }
        public void InitializeGestures()
        {
            gestureController = new GestureController();
            nui.ElevationAngle = 15;
            comm = new CommunicationManager("9600");
            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            this.gestureController.AddGesture(GestureType.WaveLeft, waveLeftSegments);
            IRelativeGestureSegment[] JoinedHandsSegments = new IRelativeGestureSegment[10];
            JoinedHandsSegment1 JoinedHandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 10; i++)
            {
                // gesture consists of the same thing 10 times 
                JoinedHandsSegments[i] = JoinedHandsSegment;
            }
            //JoinedHandsSegment2 JoinedHandsSegment2 = new JoinedHandsSegment2();
            //JoinedHandsSegments[20] = JoinedHandsSegment2;
            this.gestureController.AddGesture(GestureType.JoinedHands, JoinedHandsSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddGesture(GestureType.SwipeUp, swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            gestureController.AddGesture(GestureType.SwipeDown, swipeDownSegments);

            IRelativeGestureSegment[] swipeLeftSegments = new IRelativeGestureSegment[3];
            swipeLeftSegments[0] = new SwipeLeftSegment1();
            swipeLeftSegments[1] = new SwipeLeftSegment2();
            swipeLeftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture(GestureType.SwipeLeft, swipeLeftSegments);

            IRelativeGestureSegment[] swipeRightSegments = new IRelativeGestureSegment[3];
            swipeRightSegments[0] = new SwipeRightSegment1();
            swipeRightSegments[1] = new SwipeRightSegment2();
            swipeRightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture(GestureType.SwipeRight, swipeRightSegments);
            gestureController.GestureRecognized += OnGestureRecognized;
        }
       
        /// <summary>
        /// Prepare to feed data and skeleton frames to a new interaction stream and receive
        /// interaction data from interaction stream.
        /// </summary>
        /// <param name="sensor">
        /// Sensor from which we will stream depth and skeleton data.
        /// </param>
        private void InitializeInteractions()
        {
            // Allocate space to put the skeleton and interaction data we'll receive
            this.userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
            nui.DepthFrameReady += this.SensorDepthFrameReady;
            myInteractionClient = new MyInteractionClient();
            this.interactionStream = new InteractionStream(nui, myInteractionClient);
        }
        
        public void InitializeDevices()
        {
            devices = new Device[2];
            devices[0] = new Device("LED1", "1", "0");
            devices[1] = new Device("LED2", "2", "9");
            foreach(Device d in devices)
            {
                d.switchOff(comm);
            }
        }
        public void InitializeVoiceGrammar()
        {
            commands = new string[10];
            commands[0] = "play mediaplayer";
            commands[1] = "stop";
            commands[2] = "next";
            commands[3] = "previous";
            commands[4] = "mute";
            commands[5] = "unmute";
            commands[6] = "resume";
            commands[7] = "play project music";
            commands[8] = "device one";
            commands[9] = "device two";
            _voiceCommands = new VoiceCommands(nui, commands);
            var voiceThread = new Thread(_voiceCommands.StartAudioStream);
            voiceThread.Start();
        }
        #endregion
        #region Processing
        /// <summary>
        /// Handler for sensor chooser's KinectChanged event.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="args">event arguments</param>
        private void OnSensorChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                this.UninitializeInteractions(args.OldSensor);

                try
                {
                    args.OldSensor.SkeletonStream.AppChoosesSkeletons = false;
                    DisableNearModeSkeletalTracking();
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // nui might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            this.nui = null;

            if (args.NewSensor != null)
            {
                try
                {
                    // InteractionStream needs 640x480 depth data stream
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        // Interactions work better in near range
                        EnableNearModeSkeletalTracking();
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        DisableNearModeSkeletalTracking();
                    }
                }
                catch (InvalidOperationException)
                {
                    // nui might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }

                this.nui = args.NewSensor;
            }
        }
        /// <summary>
        /// Handler for the Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="depthImageFrameReadyEventArgs">event arguments</param>
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs depthImageFrameReadyEventArgs)
        {
            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the nui delivers events.  So check again here.
            if (this.nui != sender)
            {
                return;
            }

            using (DepthImageFrame depthFrame = depthImageFrameReadyEventArgs.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    depthBuffer = new DepthImagePixel[depthFrame.PixelDataLength];
                    depthFrame.CopyDepthImagePixelDataTo(depthBuffer);
                try
                {
                    // Hand data to Interaction framework to be processed
                    this.interactionStream.ProcessDepth(depthBuffer, depthFrame.Timestamp);
                }
                catch (InvalidOperationException)
                {
                    // DepthFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
                }
               
            }
        }

        /// <summary>
        /// Event handler for InteractionStream's InteractionFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            // Check for a null userInfos since we may still get posted events
            // from the stream after we have unregistered our event handler and
            // deleted our buffers.
            if (this.userInfos == null)
            {
                return;
            }
            UserInfo[] localUserInfos = null;
            long timestamp = 0;

            using (InteractionFrame interactionFrame = e.OpenInteractionFrame())
            {
                if (interactionFrame != null)
                {
                    // Copy interaction frame data so we can dispose interaction frame
                    // right away, even if data processing/event handling takes a while.
                    interactionFrame.CopyInteractionDataTo(this.userInfos);
                    timestamp = interactionFrame.Timestamp;
                    localUserInfos = this.userInfos;
                }
            }

            if (localUserInfos != null)
            {
                //// TODO: Process user info data, perform hit testing with UI, route UI events, etc.
                //// TODO: See KinectRegion and KinectAdapter in Microsoft.Kinect.Toolkit.Controls assembly
                //// TODO: For a more comprehensive example on how to do this.

                var currentUserSet = new HashSet<int>();
                var usersToRemove = new HashSet<int>();

                // Keep track of current users in scene
                foreach (var info in localUserInfos)
                {
                    if (info.SkeletonTrackingId == Constants.InvalidTrackingId)
                    {
                        // Only look at user information corresponding to valid users
                        continue;
                    }

                    if (!this.trackedUsers.Contains(info.SkeletonTrackingId))
                    {
                        Console.WriteLine("New user '{0}' entered scene at time {1}", info.SkeletonTrackingId, timestamp);
                    }

                    currentUserSet.Add(info.SkeletonTrackingId);
                    this.trackedUsers.Add(info.SkeletonTrackingId);

                    // Perform hit testing and look for Grip and GripRelease events
                    foreach (var handPointer in info.HandPointers)
                    {
                        double xUI = handPointer.X * Constants.InteractionRegionWidth;
                        double yUI = handPointer.Y * Constants.InteractionRegionHeight;
                        var uiElement = myInteractionClient.PerformHitTest(xUI, yUI);

                        if (uiElement != null)
                        {
                            Console.WriteLine(
                                "User '{0}' has {1} hand within element {2}",
                                info.SkeletonTrackingId,
                                handPointer.HandType);
                        }

                        if (handPointer.HandEventType != InteractionHandEventType.None)
                        {
                            //Gesture = "{1}";
                            Console.WriteLine(
                                "User '{0}' performed {1} action with {2} hand at time {3}",
                                info.SkeletonTrackingId,
                                handPointer.HandEventType,
                                handPointer.HandType,
                                timestamp);
                        }
                    }
                }

                foreach (var id in this.trackedUsers)
                {
                    if (!currentUserSet.Contains(id))
                    {
                        usersToRemove.Add(id);
                    }
                }

                foreach (var id in usersToRemove)
                {
                    this.trackedUsers.Remove(id);
                    Console.WriteLine("User '{0}' left scene at time {1}", id, timestamp);
                }
            }
        }
        /// <summary>
        /// Handler for the Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="skeletonFrameReadyEventArgs">event arguments</param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {

            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the nui delivers events.  So check again here.
            if (this.nui != sender)
            {
                return;
            }

            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    try
                    {
                        // Copy the skeleton data from the frame to an array used for temporary storage
                        skeletonFrame.CopySkeletonDataTo(this.skeletons);
                        var accelerometerReading = this.nui.AccelerometerGetCurrentReading();

                        // Hand data to Interaction framework to be processed
                        this.interactionStream.ProcessSkeleton(this.skeletons, accelerometerReading, skeletonFrame.Timestamp);
                        for (int i = 0; i < this.skeletons.Length; i++)
                        {
                            Skeleton skeleton = this.skeletons[i];
                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                this.trackedSkeleton = skeleton;
                            }
                        }
                        framesCount++;
                        if (trackedSkeleton != null)
                        {
                            //if ((trackedSkeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.Inferred) ||
                              //        (trackedSkeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.NotTracked))
                             //   EnableNearModeSkeletalTracking();
                            //else if (this.nui.SkeletonStream.EnableTrackingInNearRange == true)
                              //  DisableNearModeSkeletalTracking();
                          //  EnableNearModeSkeletalTracking();
                            if (GenerateDepth() > 140)
                            {
                                this.interactionStream.InteractionFrameReady += this.InteractionFrameReady;
                                gestureController.UpdateAllGestures(trackedSkeleton);
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // SkeletonFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                    }
                }
            }
        }
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            Debug.WriteLine(e.GestureType);
            framesCount = 0;
            switch (e.GestureType)
            {
                case GestureType.WaveLeft:
                    Gesture = "WaveLeft";
                    //1,0
                    if (devices[0].IsSwitchedOn)
                        devices[0].switchOff(comm);
                    else
                        devices[0].switchOn(comm);
                    break;
                case GestureType.SwipeUp:
                    Gesture = "SwipeUp";
                    //2,9
                    if (!devices[1].IsSwitchedOn)
                        devices[1].switchOn(comm);
                    break;
                case GestureType.SwipeDown:
                    Gesture = "SwipeDown";
                    if (devices[1].IsSwitchedOn)
                        devices[1].switchOff(comm);
                    break;
                case GestureType.SwipeLeft:
                    Gesture = "SwipeLeft";
                    break;
                case GestureType.SwipeRight:
                    Gesture = "SwipeRight";
                    break;
                case GestureType.JoinedHands:
                    Gesture = "JoinedHands";
                    break;
            }
        }
        #endregion

        public Skeleton[] requestSkeleton()
        {
            return skeletons;
        }

        /// <summary>
        /// Returns right hand position scaled to screen.
        /// </summary>
        public Joint GetCursorPosition()
        {
            if (trackedSkeleton != null)
                return trackedSkeleton.Joints[JointType.HandRight].ScaleTo(ScreenWidth, ScreenHeight, Constants.SkeletonMaxX, Constants.SkeletonMaxY);
            else
                return new Joint();
        }
     
        private void EnableNearModeSkeletalTracking()
        {
            if (this.nui != null && this.nui.DepthStream != null && this.nui.SkeletonStream != null)
            {
                this.nui.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
                this.nui.SkeletonStream.EnableTrackingInNearRange = true; // enable returning skeletons while depth is in Near Range
                this.nui.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use seated tracking
            }
        }
        private void DisableNearModeSkeletalTracking()
        {
            if (this.nui != null && this.nui.DepthStream != null && this.nui.SkeletonStream != null)
            {
                this.nui.DepthStream.Range = DepthRange.Default; // Depth in near range enabled
                this.nui.SkeletonStream.EnableTrackingInNearRange = false; // enable returning skeletons while depth is in Near Range
                this.nui.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default; // Use seated tracking
            }
        }
        
        /// <returns>
        /// Int number which is the calculated depth.
        /// </returns>
        public int GenerateDepth()
        {
            try
            {
                return (int)(100 * this.trackedSkeleton.Joints[JointType.ShoulderCenter].Position.Z);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
