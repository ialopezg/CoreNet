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

namespace ProgrammersInc.VectorGraphics.Renderers
{
	public abstract class Renderer
	{
		protected Renderer()
		{
		}

		public abstract Primitives.Path FlattenPath( Primitives.Path path );
		public abstract void MeasureText( Primitives.Text text, out double width, out double height, out double baselineFromTop );
	}
}
