/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public partial class ToolbarAnimationControl : AnimationControl
	{
		public ToolbarAnimationControl()
		{
			InitializeComponent();
		}

		public override Size GetPreferredSize( Size proposedSize )
		{
			return new Size( 24, 20 );
		}

		protected override Rectangle GetDrawingRectangle()
		{
			return new Rectangle( 0, 0, 19, 19 );
		}
	}
}

