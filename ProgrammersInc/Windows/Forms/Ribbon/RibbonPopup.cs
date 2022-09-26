/*
 
 2008 José Manuel Menéndez Poo
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
using System.Drawing.Design;
using System.ComponentModel;

namespace System.Windows.Forms
{
    [ToolboxItemAttribute(false)]
    public class RibbonPopup
        : Control
    {
        #region Fields
        private ToolStripDropDown _toolStripDropDown;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the popup is closed
        /// </summary>
        public event EventHandler Closed;

        #endregion

        #region Props

        /// <summary>
        /// Gets the related ToolStripDropDown
        /// </summary>
        public ToolStripDropDown ToolStripDropDown
        {
            get { return _toolStripDropDown; }
            set { _toolStripDropDown = value; }
        }


        #endregion

        #region Methods

        public void Show(Point screenLocation)
        {
            ToolStripControlHost host = new ToolStripControlHost(this);
            ToolStripDropDown = new ToolStripDropDown();
            ToolStripDropDown.Items.Clear();
            ToolStripDropDown.Items.Add(host);
            
            ToolStripDropDown.Padding = Padding.Empty;
            ToolStripDropDown.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            host.Margin = Padding.Empty;

            ToolStripDropDown.Closed += new ToolStripDropDownClosedEventHandler(ToolStripDropDown_Closed);

            ToolStripDropDown.Show(screenLocation);
        }

        void ToolStripDropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnClosed(EventArgs.Empty);
        }

        public void Close()
        {
            if (ToolStripDropDown != null)
            {
                ToolStripDropDown.Close();
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        #endregion
    }
}
