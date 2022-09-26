/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ToolBarRibbonControl : RibbonControl
	{
		public ToolBarRibbonControl()
		{
			Renderer = new ToolBarGlossyRenderer( true, true );
		}

		public void SetEdges( bool topEdge, bool bottomEdge )
		{
			Renderer = new ToolBarGlossyRenderer( topEdge, bottomEdge );
		}

		protected override void SetHeight( int height )
		{
			if( Height == 0 )
			{
				Height = height;
			}
		}
	}
}
