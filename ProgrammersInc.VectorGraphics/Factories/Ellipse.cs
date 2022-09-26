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
	public sealed class Ellipse
	{
		public Primitives.Path Create( Types.Rectangle rect )
		{
			return Create( rect.X, rect.Y, rect.Width, rect.Height );
		}

		public Primitives.Path Create( double x, double y, double width, double height )
		{
			Primitives.Path path = new Primitives.Path();

			path.Add( new Primitives.Path.Move( new Types.Point( x + width / 2, y ) ) );
			path.Add( new Primitives.Path.EllipticalArc( width / 2, height / 2, 0, false, true, new Types.Point( x + width, y + height / 2 ) ) );
			path.Add( new Primitives.Path.EllipticalArc( width / 2, height / 2, 0, false, true, new Types.Point( x + width / 2, y + height ) ) );
			path.Add( new Primitives.Path.EllipticalArc( width / 2, height / 2, 0, false, true, new Types.Point( x, y + height / 2 ) ) );
			path.Add( new Primitives.Path.EllipticalArc( width / 2, height / 2, 0, false, true, new Types.Point( x + width / 2, y ) ) );

			path.Add( new Primitives.Path.Close() );

			return path;
		}
	}
}
