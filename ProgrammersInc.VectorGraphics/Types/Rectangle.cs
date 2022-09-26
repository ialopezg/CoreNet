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
	[DebuggerDisplay( "Rectangle({_x},{_y},{_width},{_height})" )]
	public sealed class Rectangle
	{
		public Rectangle( double x, double y, double width, double height )
		{
			if( double.IsNaN( x ) )
			{
				throw new ArgumentException( "Not a number.", "x" );
			}
			if( double.IsNaN( y ) )
			{
				throw new ArgumentException( "Not a number.", "y" );
			}
			if( double.IsNaN( width ) )
			{
				throw new ArgumentException( "Not a number.", "width" );
			}
			if( double.IsNaN( height ) )
			{
				throw new ArgumentException( "Not a number.", "height" );
			}

			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		public double X
		{
			[DebuggerStepThrough]
			get
			{
				return _x;
			}
		}

		public double Y
		{
			[DebuggerStepThrough]
			get
			{
				return _y;
			}
		}

		public double Width
		{
			[DebuggerStepThrough]
			get
			{
				return _width;
			}
		}

		public double Height
		{
			[DebuggerStepThrough]
			get
			{
				return _height;
			}
		}

		public double Left
		{
			[DebuggerStepThrough]
			get
			{
				return _x;
			}
		}

		public double Right
		{
			[DebuggerStepThrough]
			get
			{
				return _x + _width;
			}
		}

		public double Top
		{
			[DebuggerStepThrough]
			get
			{
				return _y;
			}
		}

		public double Bottom
		{
			[DebuggerStepThrough]
			get
			{
				return _y + _height;
			}
		}

		public Point TopLeft
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x, _y );
			}
		}

		public Point TopRight
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x + _width, _y );
			}
		}

		public Point BottomLeft
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x, _y + _height );
			}
		}

		public Point BottomRight
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x + _width, _y + _height );
			}
		}

		public Point Center
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x + _width / 2, _y + _height / 2 );
			}
		}

		public Point CenterLeft
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x, _y + _height / 2 );
			}
		}

		public Point CenterRight
		{
			[DebuggerStepThrough]
			get
			{
				return new Point( _x + _width, _y + _height / 2 );
			}
		}

		public static Rectangle Union( params Rectangle[] rects )
		{
			Rectangle union = new Rectangle( 0, 0, 0, 0 );

			foreach( Rectangle rect in rects )
			{
				if( union.Width == 0 || union.Height == 0 )
				{
					union = rect;
				}
				else if( rect.Width != 0 && rect.Height != 0 )
				{
					double x = Math.Min( union.X, rect.X);
					double y = Math.Min( union.Y, rect.Y );
					double right = Math.Max( union.Right, rect.Right );
					double bottom = Math.Max( union.Bottom, rect.Bottom );

					union = new Rectangle( x, y, right - x, bottom - y );
				}
			}

			return union;
		}

		public static bool Overlap( Rectangle r1, Rectangle r2 )
		{
			if( r1 == null )
			{
				throw new ArgumentNullException( "r1" );
			}
			if( r2 == null )
			{
				throw new ArgumentNullException( "r2" );
			}

			if( r1.Right < r2.Left || r2.Right < r1.Left )
			{
				return false;
			}
			if( r1.Bottom < r2.Top || r2.Bottom < r1.Top )
			{
				return false;
			}

			return true;
		}

		public static Rectangle Offset( Rectangle rect, Point offset )
		{
			return new Rectangle( rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height );
		}

		public static Rectangle Shrink( Rectangle rect, double by )
		{
			return new Rectangle( rect.X + by, rect.Y + by, rect.Width - by * 2, rect.Height - by * 2 );
		}

		public static Rectangle Expand( Rectangle rect, double by )
		{
			return new Rectangle( rect.X - by, rect.Y - by, rect.Width + by * 2, rect.Height + by * 2 );
		}

		private double _x, _y, _width, _height;	}
}
