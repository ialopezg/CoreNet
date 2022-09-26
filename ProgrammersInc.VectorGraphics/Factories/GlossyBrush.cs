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
	public sealed class GlossyBrush
	{
		public GlossyBrush()
		{
		}

		public GlossyBrush( Paint.Color lightener )
		{
			if( lightener == null )
			{
				throw new ArgumentNullException( "lightener" );
			}

			_lightener = lightener;
		}

		public Paint.Brushes.Brush Create( Paint.Color color, double top, double bottom )
		{
			return Create( color, Paint.Color.Combine( color, _lightener, 0.2 ), top, bottom );
		}

		public Paint.Brushes.Brush Create( Paint.Color color, double top, double bottom, double proportion )
		{
			return Create( color, Paint.Color.Combine( color, _lightener, 0.2 ), top, bottom, proportion );
		}

		public Paint.Brushes.Brush Create( Paint.Color color, Paint.Color lighterColor, double top, double bottom )
		{
			return Create( color, lighterColor, top, bottom, 0.3 );
		}

		public Paint.Brushes.Brush Create( Paint.Color color, Paint.Color lighterColor, double top, double bottom, double proportion )
		{
			Paint.Color white = new Paint.Color( _lightener, color.Alpha );
			Paint.Color topStart = Paint.Color.Combine( color, white, 0.4 );
			Paint.Color topEnd = Paint.Color.Combine( color, white, 0.7 );
			Paint.Color bottomStart = color;
			Paint.Color bottomEnd = lighterColor;

			return new Paint.Brushes.LinearGradientBrush
				( topStart, bottomEnd
				, new Types.Point( 0, top ), new Types.Point( 0, bottom )
				, Paint.Brushes.LinearGradientBrush.RenderHint.NoClip
				, new KeyValuePair<double, Paint.Color>[] { new KeyValuePair<double, Paint.Color>( proportion, topEnd ), new KeyValuePair<double, Paint.Color>( proportion, bottomStart ) } );
		}

		private Paint.Color _lightener = Paint.Color.White;
	}
}
