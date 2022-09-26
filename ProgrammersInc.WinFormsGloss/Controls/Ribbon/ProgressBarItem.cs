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
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ProgressBarItem : Item
	{
		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return new Size( 106, suggestedSize.Height );
		}

		public int Percentage
		{
			get
			{
				return _percentage;
			}
			set
			{
				if( _percentage == value )
				{
					return;
				}

				_percentage = value;

				if( Section != null )
				{
					Section.NotifyItemChanged( this );
				}
			}
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			if( logicalBounds == Rectangle.Empty )
			{
				return;
			}

			context.Renderer.PaintProgressBar( context, clip, logicalBounds, _percentage );
		}

		private int _percentage;
	}
}
