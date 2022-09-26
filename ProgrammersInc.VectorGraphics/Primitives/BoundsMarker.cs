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

namespace ProgrammersInc.VectorGraphics.Primitives
{
	public sealed class BoundsMarker : VisualItem
	{
		public BoundsMarker( Types.Rectangle rectangle )
		{
			_rectangle = rectangle;
		}

		public Types.Rectangle Rectangle
		{
			get
			{
				return _rectangle;
			}
		}

		public override void Visit( Visitor visitor )
		{
			visitor.PreVisitVisualItem( this );
			visitor.VisitBoundsMarker( this );
			visitor.PostVisitVisualItem( this );
		}

		public override VisualItem Copy()
		{
			return new BoundsMarker( _rectangle );
		}

		protected override Types.Rectangle CalculateBounds( Renderers.Renderer renderer )
		{
			return _rectangle;
		}

		private Types.Rectangle _rectangle;
	}
}
