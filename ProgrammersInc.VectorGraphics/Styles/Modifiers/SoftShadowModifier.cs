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

namespace ProgrammersInc.VectorGraphics.Styles.Modifiers
{
	public class SoftShadowModifier : PathModifier
	{
		public SoftShadowModifier( Types.Point offset, double extent, Paint.Color color )
		{
			if( offset == null )
			{
				throw new ArgumentNullException( "offset" );
			}
			if( extent <= 0 )
			{
				throw new ArgumentException( "Extent must be greater than zero.", "extent" );
			}
			if( color == null )
			{
				throw new ArgumentNullException( "color" );
			}

			_offset = offset;
			_extent = extent;
			_color = color;
		}

		protected override void Apply( Renderers.Renderer renderer, Primitives.Path path )
		{
			Factories.SoftShadow softShadow = new Factories.SoftShadow( renderer, _offset, _extent, _color );

			Primitives.VisualItem shadow = softShadow.Create( path );

			path.Parent.AddFront( shadow );
		}

		private Types.Point _offset;
		private double _extent;
		private Paint.Color _color;
	}
}
