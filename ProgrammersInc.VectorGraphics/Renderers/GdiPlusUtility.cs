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

namespace ProgrammersInc.VectorGraphics.Renderers.GdiPlusUtility
{
	public static class Convert
	{
		public static System.Drawing.Color Color( Paint.Color color )
		{
			int r = (int) (color.Red * 255);
			int g = (int) (color.Green * 255);
			int b = (int) (color.Blue * 255);
			int a = (int) (color.Alpha * 255);

			return System.Drawing.Color.FromArgb( a, r, g, b );
		}

		public static Paint.Color Color( System.Drawing.Color color )
		{
			double r = color.R / 255.0;
			double g = color.G / 255.0;
			double b = color.B / 255.0;
			double a = color.A / 255.0;

			return new VectorGraphics.Paint.Color( r, g, b, a );
		}

		public static Types.Rectangle Rectangle( System.Drawing.Rectangle rect )
		{
			return new VectorGraphics.Types.Rectangle( rect.X, rect.Y, rect.Width, rect.Height );
		}

		public static Types.Rectangle Rectangle( System.Drawing.RectangleF rect )
		{
			return new VectorGraphics.Types.Rectangle( rect.X, rect.Y, rect.Width, rect.Height );
		}

		public static System.Drawing.PointF Point( Types.Point p )
		{
			return new System.Drawing.PointF( (float) p.X, (float) p.Y );
		}
	}
}
