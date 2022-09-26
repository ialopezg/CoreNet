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
	public class RectangleMarkerReplacementModifier : MarkerReplacementModifier
	{
		protected override Primitives.VisualItem CreateItem( Primitives.BoundsMarker marker )
		{
			Primitives.Path rect = new Primitives.Path();

			rect.Add( new Primitives.Path.Move( marker.Rectangle.TopLeft ) );
			rect.Add( new Primitives.Path.Line( marker.Rectangle.TopRight ) );
			rect.Add( new Primitives.Path.Line( marker.Rectangle.BottomRight ) );
			rect.Add( new Primitives.Path.Line( marker.Rectangle.BottomLeft ) );
			rect.Add( new Primitives.Path.Line( marker.Rectangle.TopLeft ) );
			rect.Add( new Primitives.Path.Close() );

			return rect;
		}

		protected override Primitives.VisualItem CreateItem( Primitives.PointMarker marker )
		{
			return null;
		}
	}
}
