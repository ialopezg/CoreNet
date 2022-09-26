/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//
// (c) 2006 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryComponents.VectorGraphics.TestHarness.StyleSets
{
	public sealed class BlueAndWhite : Styles.StyleSet
	{
		public BlueAndWhite()
		{
			AddModifier( "GraphContainer", new Styles.Modifiers.RectangleMarkerReplacementModifier() );
			AddModifier( "GraphContainer", new Styles.Modifiers.PathPaintModifier( null, new Paint.Brushes.SolidBrush( _backColor ) ) );
			AddModifier( "GraphPiece", new GraphPiecePaintModifier() );
			AddModifier( "GraphPiece", new Styles.Modifiers.SoftShadowModifier( new Types.Point( 1, 1 ), 5, new Paint.Color( 0, 0, 0, 0.1 ) ) );
		}

		private sealed class GraphPiecePaintModifier : Styles.Modifiers.PathModifier
		{
			protected override void Apply( Renderers.Renderer renderer, Primitives.Path path )
			{
				double index = int.Parse( path.Style.GetExtra( "RowIndex" ) );
				double count = int.Parse( path.Style.GetExtra( "RowCount" ) );

				Paint.Color color = Paint.Color.Combine( _baseColor, Paint.Color.White, 1 - index / (count * 1.5) );

				path.Pen = new Paint.Pens.SolidPen( Paint.Color.White, 2 );
				path.Brush = new Paint.Brushes.SolidBrush( color );
			}
		}

		private static Paint.Color _backColor = new Paint.Color( 0.875, 0.890, 0.925 );
		private static Paint.Color _baseColor = new Paint.Color( 0.294, 0.420, 0.580 );
	}
}
