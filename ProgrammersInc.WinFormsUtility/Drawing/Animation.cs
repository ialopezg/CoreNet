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

namespace ProgrammersInc.WinFormsUtility.Drawing
{
	public abstract class Animation
	{
		protected Animation()
		{
		}

		public abstract void OnPaint( Graphics g, Rectangle drawingBounds, bool running, double seconds );

		public virtual bool IsDone( double seconds )
		{
			return false;
		}

		public virtual double GetSuggestedAlpha( double seconds )
		{
			return 1;
		}
	}
}
