/*
 
 2008 Jos? Manuel Men?ndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class RibbonItemBoundsEventArgs
        : RibbonItemRenderEventArgs
    {
        public RibbonItemBoundsEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonItem item, Rectangle bounds)
            : base(owner, g, clip, item)
        {
            Bounds = bounds;
        }

        #region Properties

        private Rectangle _bounds;

        /// <summary>
        /// Gets or sets the suggested bounds
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }


        #endregion
    }
}
