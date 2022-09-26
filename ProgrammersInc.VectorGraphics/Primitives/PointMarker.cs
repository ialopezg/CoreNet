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
	public sealed class PointMarker : VisualItem
	{
		public PointMarker( Types.Point point )
		{
			if( point == null )
			{
				throw new ArgumentNullException( "point" );
			}

			_point = point;
		}

		public Types.Point Point
		{
			get
			{
				return _point;
			}
		}

		public override void Visit( Visitor visitor )
		{
			visitor.PreVisitVisualItem( this );
			visitor.VisitPointMarker( this );
			visitor.PostVisitVisualItem( this );
		}

		public override VisualItem Copy()
		{
			return new PointMarker( _point );
		}

		protected override Types.Rectangle CalculateBounds( Renderers.Renderer renderer )
		{
			return new Types.Rectangle( _point.X, _point.Y, 0, 0 );
		}

		private Types.Point _point;
	}
}
