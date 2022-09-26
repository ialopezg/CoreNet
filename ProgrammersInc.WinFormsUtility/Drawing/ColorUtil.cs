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
using System.Drawing;

namespace ProgrammersInc.WinFormsUtility.Drawing
{
	public static class ColorUtil
	{
		public static Color Combine( Color c1, Color c2, double proportion )
		{
			double iprop = 1 - proportion;
			int r = (int) (c1.R * proportion + c2.R * iprop);
			int g = (int) (c1.G * proportion + c2.G * iprop);
			int b = (int) (c1.B * proportion + c2.B * iprop);

			r = Math.Min( Math.Max( 0, r ), 255 );
			g = Math.Min( Math.Max( 0, g ), 255 );
			b = Math.Min( Math.Max( 0, b ), 255 );

			return Color.FromArgb( r, g, b );
		}

		public static Color ModifySaturation( Color c, double change )
		{
			double h = c.GetHue() / 360.0;
			double s = c.GetSaturation();
			double l = c.GetBrightness();

			s *= change;

			s = Math.Min( Math.Max( 0, s ), 1 );

			return FromHSL( h, s, l );
		}

		public static Color ModifyHue( Color c, double change )
		{
			double h = c.GetHue() / 360.0;
			double s = c.GetSaturation();
			double l = c.GetBrightness();

			h += change;

			return FromHSL( h, s, l );
		}

		public static Color ModifyLight( Color c, double change )
		{
			double h = c.GetHue() / 360.0;
			double s = c.GetSaturation();
			double l = c.GetBrightness();

			l *= change;

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

			return Color.FromArgb( (int) (255 * r), (int) (255 * g), (int) (255 * b) );
		}
	}
}
