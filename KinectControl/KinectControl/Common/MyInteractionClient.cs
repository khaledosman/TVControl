namespace KinectControl.Common
{
    using Microsoft.Kinect.Toolkit.Interaction;

    /// <summary>
    /// Simple sample application that shows minimal code needed to use Kinect interaction stream,
    /// regardless of specific UI framework being used.
    /// </summary>
    /// <remarks>
    /// Note that this is a console application only for simplicity. The InteractionStream was
    /// designed to be used to interact with 2D UI elements, so a UI application developer should
    /// port this code to the appropriate entry points for the UI framework being used.
    /// </remarks>
    public class MyInteractionClient : IInteractionClient
    {
        //// TODO: Rather than hardcoding size of interaction region, use dimensions of the
        //// TODO: region of your UI that allows Kinect interactions.

        /// <summary>
        /// Information about a button control laid out in application UI.
        /// </summary>
        public readonly UIElementInfo buttonControl = new UIElementInfo
        {
            Left = 100.0,
            Top = 100.0,
            Right = 300.0,
            Bottom = 300.0,
            Id = "button1"
        };

        /// <summary>
        /// Returns information about the UI element located at the specified coordinates.
        /// This simulates a hit testing operation that would normally be performed by
        /// some UI framework.
        /// </summary>
        /// <param name="x">
        /// Horizontal position, in UI coordinates.
        /// </param>
        /// <param name="y">
        /// Vertical position, in UI coordinates.
        /// </param>
        /// <returns>
        /// Information about topmost UI control located at the specified UI position.
        /// </returns>
        public UIElementInfo PerformHitTest(double x, double y)
        {
            //// TODO: Rather than manually checking against bounds of each control, use
            //// TODO: UI framework hit testing functionality, if available
            if ((this.buttonControl.Left <= x) && (x <= this.buttonControl.Right) &&
                (this.buttonControl.Top <= y) && (y <= this.buttonControl.Bottom))
            {
                return this.buttonControl;
            }

            return null;
        }

        /// <summary>
        /// Gets interaction information available for a specified location in UI.
        /// </summary>
        /// <param name="skeletonTrackingId">
        /// The skeleton tracking ID for which interaction information is being retrieved.
        /// </param>
        /// <param name="handType">
        /// The hand type for which interaction information is being retrieved.
        /// </param>
        /// <param name="x">
        /// X-coordinate of UI location for which interaction information is being retrieved.
        /// 0.0 corresponds to left edge of interaction region and 1.0 corresponds to right edge
        /// of interaction region.
        /// </param>
        /// <param name="y">
        /// Y-coordinate of UI location for which interaction information is being retrieved.
        /// 0.0 corresponds to top edge of interaction region and 1.0 corresponds to bottom edge
        /// of interaction region.
        /// </param>
        /// <returns>
        /// An <see cref="InteractionInfo"/> object instance.
        /// </returns>
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var interactionInfo = new InteractionInfo
            {
                IsPressTarget = false,
                IsGripTarget = false
            };

            // Map coordinates from [0.0,1.0] coordinates to UI-relative coordinates
            double xUI = x * Constants.InteractionRegionWidth;
            double yUI = y * Constants.InteractionRegionHeight;

            var uiElement = this.PerformHitTest(xUI, yUI);

            if (uiElement != null)
            {
                interactionInfo.IsPressTarget = true;

                // If UI framework uses strings as button IDs, use string hash code as ID
                interactionInfo.PressTargetControlId = uiElement.Id.GetHashCode();

                // Designate center of button to be the press attraction point
                //// TODO: Create your own logic to assign press attraction points if center
                //// TODO: is not always the desired attraction point.
                interactionInfo.PressAttractionPointX = ((uiElement.Left + uiElement.Right) / 2.0) / Constants.InteractionRegionWidth;
                interactionInfo.PressAttractionPointY = ((uiElement.Top + uiElement.Bottom) / 2.0) / Constants.InteractionRegionHeight;
            }

            return interactionInfo;
        }    
    }
}