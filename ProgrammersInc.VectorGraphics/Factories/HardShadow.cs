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
	public sealed class HardShadow : Shadow
	{
		public HardShadow( Types.Point offset, Paint.Color color )
		{
			if( offset == null )
			{
				throw new ArgumentNullException( "offset" );
			}
			if( color == null )
			{
				throw new ArgumentNullException( "color" );
			}

			_offset = offset;
			_color = color;
		}

		public override Primitives.VisualItem Create( Primitives.Path source )
		{
			Primitives.Container container = new Primitives.Container( _offset );

			Primitives.Path shadow = (Primitives.Path) source.Copy();

			shadow.Pen = null;
			shadow.Brush = new Paint.Brushes.SolidBrush( _color );

			container.AddBack( shadow );

			return container;
		}

		private Types.Point _offset;
		private Paint.Color _color;
	}
}
