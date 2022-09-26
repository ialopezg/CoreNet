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

namespace ProgrammersInc.VectorGraphics.Factories
{
	public sealed class RoundedRectangle
	{
		[Flags]
		public enum Corners
		{
			TopLeft = 1,
			TopRight = 2,
			BottomRight = 4,
			BottomLeft = 8,
			All = TopLeft | TopRight | BottomRight | BottomLeft
		}

		public Primitives.Path Create( Types.Rectangle rect, double radius )
		{
			return Create( rect.X, rect.Y, rect.Width, rect.Height, radius, Corners.All );
		}
		
		public Primitives.Path Create( Types.Rectangle rect, double radius, Corners corners )
		{
			return Create( rect.X, rect.Y, rect.Width, rect.Height, radius, corners );
		}
		
		public Primitives.Path Create( double x, double y, double width, double height, double radius )
		{
			return Create( x, y, width, height, radius, Corners.All );
		}

		public Primitives.Path Create( double x, double y, double width, double height, double radius, Corners corners )
		{
			bool topLeft = (corners & Corners.TopLeft) != 0;
			bool topRight = (corners & Corners.TopRight) != 0;
			bool bottomLeft = (corners & Corners.BottomLeft) != 0;
			bool bottomRight = (corners & Corners.BottomRight) != 0;

			Primitives.Path path = new Primitives.Path();

			path.Add( new Primitives.Path.Move( new Types.Point( x + (topLeft ? radius : 0), y ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( x + width - (topRight ? radius : 0), y ) ) );

			if( topRight )
			{
				path.Add( new Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new Types.Point( x + width, y + radius ) ) );
			}
			
			path.Add( new Primitives.Path.Line( new Types.Point( x + width, y + height - (bottomRight ? radius : 0) ) ) );

			if( bottomRight )
			{
				path.Add( new Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new Types.Point( x + width - radius, y + height ) ) );
			}

			path.Add( new Primitives.Path.Line( new Types.Point( x + (bottomLeft ? radius : 0), y + height ) ) );

			if( bottomLeft )
			{
				path.Add( new Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new Types.Point( x, y + height - radius ) ) );
			}

			path.Add( new Primitives.Path.Line( new Types.Point( x, y + (topLeft ? radius : 0) ) ) );

			if( topLeft )
			{
				path.Add( new Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new Types.Point( x + radius, y ) ) );
			}

			path.Add( new Primitives.Path.Close() );

			return path;
		}
	}
}
