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

namespace ProgrammersInc.VectorGraphics.Paint
{
	[DebuggerDisplay("Color(R={_r},G={_g},B={_b},A={_a})")]
	public sealed class Color
	{
		public delegate double Modify( double v );

		[DebuggerStepThrough]
		public Color( double r, double g, double b )
			: this( r, g, b, 1 )
		{
		}

		public Color( Color c, double a )
			: this( c.Red, c.Green, c.Blue, a )
		{
		}

		public Color( double r, double g, double b, double a )
		{
			if( r < 0 || r > 1 )
			{
				throw new ArgumentException( "Color component must be between 0 and 1.", "r" );
			}
			if( g < 0 || g > 1 )
			{
				throw new ArgumentException( "Color component must be between 0 and 1.", "r" );
			}
			if( b < 0 || b > 1 )
			{
				throw new ArgumentException( "Color component must be between 0 and 1.", "r" );
			}
			if( a < 0 || a > 1 )
			{
				throw new ArgumentException( "Color component must be between 0 and 1.", "r" );
			}

			_r = r;
			_g = g;
			_b = b;
			_a = a;
		}

		public double Red
		{
			[DebuggerStepThrough]
			get
			{
				return _r;
			}
		}

		public double Green
		{
			[DebuggerStepThrough]
			get
			{
				return _g;
			}
		}

		public double Blue
		{
			[DebuggerStepThrough]
			get
			{
				return _b;
			}
		}

		public double Alpha
		{
			[DebuggerStepThrough]
			get
			{
				return _a;
			}
		}

		public void GetHSL( out double h, out double s, out double l )
		{
			double varMin = Math.Min( _r, Math.Min( _g, _b ) );
			double varMax = Math.Max( _r, Math.Max( _g, _b ) );
			double delMax = varMax - varMin;

			l = (varMax + varMin) / 2;

			if( delMax == 0 )
			{
				h = 0;
				s = 0;
			}
			else
			{
				if( l < 0.5 )
				{
					s = delMax / (varMax + varMin);
				}
				else
				{
					s = delMax / (2 - varMax - varMin);
				}

				double delR = (((varMax - _r) / 6) + (delMax / 2)) / delMax;
				double delG = (((varMax - _g) / 6) + (delMax / 2)) / delMax;
				double delB = (((varMax - _b) / 6) + (delMax / 2)) / delMax;

				if( _r == varMax )
				{
					h = delB - delG;
				}
				else if( _g == varMax )
				{
					h = (1.0 / 3.0) + delR - delB;
				}
				else if( _b == varMax )
				{
					h = (2.0 / 3.0) + delG - delR;
				}
				else
				{
					throw new InvalidOperationException();
				}

				if( h < 0 )
				{
					h += 1;
				}
				if( h > 1 )
				{
					h -= 1;
				}
			}
		}

		public static Color Black
		{
			[DebuggerStepThrough]
			get
			{
				return new Color( 0, 0, 0 );
			}
		}

		public static Color White
		{
			[DebuggerStepThrough]
			get
			{
				return new Color( 1, 1, 1 );
			}
		}

		public static Color Transparent
		{
			[DebuggerStepThrough]
			get
			{
				return new Color( 0, 0, 0, 0 );
			}
		}

		public static Color Combine( Color c1, Color c2, double p )
		{
			if( c1 == null )
			{
				throw new ArgumentNullException( "c1" );
			}
			if( c2 == null )
			{
				throw new ArgumentNullException( "c2" );
			}

			double r = c1._r * p + c2._r * (1 - p);
			double g = c1._g * p + c2._g * (1 - p);
			double b = c1._b * p + c2._b * (1 - p);
			double a = c1._a * p + c2._a * (1 - p);

			r = Math.Min( Math.Max( r, 0 ), 1 );
			g = Math.Min( Math.Max( g, 0 ), 1 );
			b = Math.Min( Math.Max( b, 0 ), 1 );
			a = Math.Min( Math.Max( a, 0 ), 1 );

			return new Color( r, g, b, a );
		}

		public static Color ModifySaturation( Color c, Modify modify )
		{
			double h;
			double s;
			double l;

			c.GetHSL( out h, out s, out l );

			s = modify( s );

			s = Math.Min( Math.Max( 0, s ), 1 );

			return FromHSL( h, s, l );
		}

		public static Color ModifyHue( Color c, Modify modify )
		{
			double h;
			double s;
			double l;

			c.GetHSL( out h, out s, out l );

			h = modify( h );

			return FromHSL( h, s, l );
		}

		public static Color ModifyLight( Color c, Modify modify )
		{
			double h;
			double s;
			double l;

			c.GetHSL( out h, out s, out l );

			l = modify( l );

			l = Math.Min( Math.Max( 0, l ), 1 );

			return FromHSL( h, s, l );
		}

		public static Color ModifyHSL( Color c, Modify modifyH, Modify modifyS, Modify modifyL )
		{
			double h;
			double s;
			double l;

			c.GetHSL( out h, out s, out l );

			h = modifyH( h );
			s = modifyS( s );
			l = modifyL( l );

			s = Math.Min( Math.Max( 0, s ), 1 );
			l = Math.Min( Math.Max( 0, l ), 1 );

			return FromHSL( h, s, l );
		}

		public static Color FromHSL( double h, double s, double l )
		{
			double r = 0, g = 0, b = 0;
			double temp1, temp2;

			if( l == 0 )
			{
				r = g = b = 0;
			}
			else
			{
				if( s == 0 )
				{
					r = g = b = l;
				}
				else
				{
					temp2 = ((l <= 0.5) ? l * (1.0 + s) : l + s - (l * s));
					temp1 = 2.0 * l - temp2;

					double[] t3 = new double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
					double[] clr = new double[] { 0, 0, 0 };

					for( int i = 0; i < 3; i++ )
					{
						if( t3[i] < 0 )
						{
							t3[i] += 1.0;
						}
						if( t3[i] > 1 )
						{
							t3[i] -= 1.0;
						}

						if( 6.0 * t3[i] < 1.0 )
						{
							clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
						}
						else if( 2.0 * t3[i] < 1.0 )
						{
							clr[i] = temp2;
						}
						else if( 3.0 * t3[i] < 2.0 )
						{
							clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
						}
						else
						{
							clr[i] = temp1;
						}
					}

					r = clr[0];
					g = clr[1];
					b = clr[2];
				}
			}

			return new Color( r, g, b );
		}

		private double _r, _g, _b, _a;
	}
}
