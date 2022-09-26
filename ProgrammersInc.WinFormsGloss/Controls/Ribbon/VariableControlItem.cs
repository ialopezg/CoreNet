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
	public class VariableControlItem : Item
	{
		public delegate Control CreateControl();
		public delegate Size CalculateSize( RibbonControl ribbonControl );

		public VariableControlItem( CreateControl createControl, CalculateSize calculateSize, int yOffset )
		{
			if( createControl == null )
			{
				throw new ArgumentNullException( "createControl" );
			}

			_createControl = createControl;
			_calculateSize = calculateSize;
			_control = _createControl();
			_yOffset = yOffset;
		}

		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return _calculateSize( ribbonControl );
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
				_control.Bounds = new Rectangle( logicalBounds.X, logicalBounds.Y + _yOffset, logicalBounds.Width, logicalBounds.Height );
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

		public Control Control
		{
			get
			{
				return _control;
			}
		}

		private CreateControl _createControl;
		private CalculateSize _calculateSize;
		private Control _control;
		private int _yOffset;
	}
}
