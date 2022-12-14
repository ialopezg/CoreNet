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
    public sealed class RibbonTabRenderEventArgs : RibbonRenderEventArgs
    {
        private RibbonTab _tab;

        public RibbonTabRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonTab tab)
            : base(owner, g, clip)
        {
            Tab = tab;
        }

        /// <summary>
        /// Gets or sets the RibbonTab related to the evennt
        /// </summary>
        public RibbonTab Tab
        {
            get
            {
                return _tab;
            }
            set
            {
                _tab = value;
            }
        }
    }
}
