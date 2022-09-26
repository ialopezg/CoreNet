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
	[DebuggerDisplay( "Vector({_x},{_y})" )]
	public sealed class Vector
	{
		public Vector( double x, double y )
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

		public Vector( Point p )
		{
			_x = p.X;
			_y = p.Y;
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

		public Vector Normalize( double required )
		{
			double length = Math.Sqrt( _x * X + _y * _y );

			if( length == 0 )
			{
				return new Vector( 0, 0 );
			}
			else
			{
				return new Vector( _x * required / length, _y * required / length );
			}
		}

		public static Vector operator +( Vector v1, Vector v2 )
		{
			return new Vector( v1._x + v2._x, v1._y + v2._y );
		}

		public static Vector operator /( Vector v, double f )
		{
			if( f == 0 )
			{
				throw new ArgumentException( "Factor may not be zero.", "f" );
			}

			return new Vector( v._x / f, v._y / f );
		}

		private double _x, _y;
	}
}
