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

namespace ProgrammersInc.VectorGraphics.Styles.Modifiers
{
	public class PathPaintModifier : PathModifier
	{
		public PathPaintModifier( Paint.Pens.Pen pen, Paint.Brushes.Brush brush )
		{
			_pen = pen;
			_brush = brush;
		}

		protected override void Apply( Renderers.Renderer renderer, Primitives.Path path )
		{
			path.Pen = _pen;
			path.Brush = _brush;
		}

		private Paint.Pens.Pen _pen;
		private Paint.Brushes.Brush _brush;
	}
}
