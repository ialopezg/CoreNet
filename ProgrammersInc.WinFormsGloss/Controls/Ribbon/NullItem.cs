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
	public sealed class NullItem : Item
	{
		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return new Size( 0, suggestedSize.Height );
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
		}
	}
}
