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
using System.Diagnostics;

namespace ProgrammersInc.VectorGraphics.Types
{
	[DebuggerDisplay( "Point({_x},{_y})" )]
	public sealed class Point
	{
		public Point( double x, double y )
		{
			if( double.IsNaN( x ) )
			{
				throw new ArgumentException( "Not a number.", "x" );
			}
			if( double.IsNaN( y ) )
			{
				throw new ArgumentException( "Not a number.", "y" );
			}

			_x = x;
			_y = y;
		}

		public double X
		{
			get
			{
				return _x;
			}
		}

		public double Y
		{
			get
			{
				return _y;
			}
		}

		public static Point operator +( Point p, Vector v )
		{
			return new Point( p.X + v.X, p.Y + v.Y );
		}

		public static Vector operator -( Point p1, Point p2 )
		{
			return new Vector( p1.X - p2.X, p1.Y - p2.Y );
		}

		private double _x, _y;
	}
}
