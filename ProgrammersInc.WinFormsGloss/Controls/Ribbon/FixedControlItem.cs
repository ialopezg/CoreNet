/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class FixedControlItem : Item
	{
		public delegate Control CreateControl();

		public FixedControlItem( CreateControl createControl )
		{
			if( createControl == null )
			{
				throw new ArgumentNullException( "createControl" );
			}

			_createControl = createControl;
			_control = _createControl();
		}

		public override System.Drawing.Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return _control.Size;
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			if( logicalBounds == Rectangle.Empty )
			{
				_control.Visible = false;
			}
			else
			{
				_control.Visible = true;
				_control.Bounds = logicalBounds;
				_control.Parent = context.RibbonControl;
			}

			Rectangle sectionRect = context.GetSectionBounds( Section );

			Section.Paint( context, clip, sectionRect );
		}

		public override ToolStripItem CreateEquivalentToolStripItem()
		{
			Control control = _createControl();

			ToolStripControlHost host = new ToolStripControlHost( control );

			host.AutoSize = false;

			return host;
		}

		private CreateControl _createControl;
		private Control _control;
	}
}
