using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControl.Common
{
    /// <summary>
    /// This class is meant to be analogous to a structure used by your UI framework to
    /// represent hit-testable components such as button controls, sliders, etc.
    /// </summary>
    /// <remarks>
    /// Rather than defining your own class, you should leverage hit testing functionality
    /// provided by your UI framework.
    /// </remarks>
    public class UIElementInfo
    {
        /// <summary>
        /// Position of the left edge of UI element.
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Position of the right edge of UI element.
        /// </summary>
        public double Right { get; set; }

        /// <summary>
        /// Position of the top edge of UI element.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Position of the bottom edge of UI element.
        /// </summary>
        public double Bottom { get; set; }

        /// <summary>
        /// Identifier of UI element.
        /// </summary>
        public string Id { get; set; }
    }
}
