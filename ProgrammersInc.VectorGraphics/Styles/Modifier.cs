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

namespace ProgrammersInc.VectorGraphics.Styles
{
	public abstract class Modifier
	{
		protected Modifier()
		{
		}

		public abstract void Apply( Renderers.Renderer renderer, Primitives.VisualItem item );
	}
}
