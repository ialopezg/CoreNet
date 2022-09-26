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
	public sealed class Rectangle
	{
		public Primitives.Path Create( Types.Rectangle rect )
		{
			return Create( rect.X, rect.Y, rect.Width, rect.Height );
		}

		public Primitives.Path Create( double x, double y, double width, double height )
		{
			Primitives.Path path = new Primitives.Path();

			path.Add( new Primitives.Path.Move( new Types.Point( x, y ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( x + width, y ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( x + width, y + height ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( x, y + height ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( x, y ) ) );
			path.Add( new Primitives.Path.Close() );

			return path;
		}
	}
}
