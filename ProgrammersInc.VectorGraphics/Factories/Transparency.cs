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
	public class Transparency
	{
		public void Apply( Primitives.Container container, double multiplier )
		{
			Visitor visitor = new Visitor( multiplier );

			container.Visit( visitor );
		}

		private sealed class Visitor : Primitives.Visitor
		{
			internal Visitor( double multiplier )
			{
				_multiplier = multiplier;
			}

			public override void VisitPath( Primitives.Path path )
			{
				base.VisitPath( path );

				PenVisitor penVisitor = new PenVisitor( _multiplier );
				BrushVisitor brushVisitor = new BrushVisitor( _multiplier );

				if( path.Pen != null )
				{
					path.Pen.Visit( penVisitor );
					path.Pen = penVisitor.NewPen;
				}
				if( path.Brush != null )
				{
					path.Brush.Visit( new Types.Rectangle( 0, 0, 0, 0 ), brushVisitor );
					path.Brush = brushVisitor.NewBrush;
				}
			}

			public override void VisitText( Primitives.Text text )
			{
				base.VisitText( text );

				text.Color = new Paint.Color( text.Color, text.Color.Alpha * _multiplier );
			}

			private double _multiplier;
		}

		#region PenVisitor

		private sealed class PenVisitor : Paint.Pens.PenVisitor
		{
			internal PenVisitor( double multiplier )
			{
				_multiplier = multiplier;
			}

			internal Paint.Pens.Pen NewPen
			{
				get
				{
					return _newPen;
				}
			}

			public override void VisitSolidPen( Paint.Pens.SolidPen solidPen )
			{
				_newPen = new Paint.Pens.SolidPen( new Paint.Color( solidPen.Color, solidPen.Color.Alpha * _multiplier ), solidPen.Width );
			}

			private double _multiplier;
			private Paint.Pens.Pen _newPen;
		}

		#endregion

		#region BrushVisitor

		private sealed class BrushVisitor : Paint.Brushes.BrushVisitor
		{
			internal BrushVisitor( double multiplier )
			{
				_multiplier = multiplier;
			}

			internal Paint.Brushes.Brush NewBrush
			{
				get
				{
					return _newBrush;
				}
			}

			public override void VisitSolidBrush( Types.Rectangle bounds, Paint.Brushes.SolidBrush solidBrush )
			{
				_newBrush = new Paint.Brushes.SolidBrush( new Paint.Color( solidBrush.Color, solidBrush.Color.Alpha * _multiplier ) );
			}

			public override void VisitLinearGradientBrush( Types.Rectangle bounds, Paint.Brushes.LinearGradientBrush linearGradientBrush )
			{
				List<KeyValuePair<double, Paint.Color>> intermediates = new List<KeyValuePair<double, Paint.Color>>();

				foreach( KeyValuePair<double, Paint.Color> kvp in linearGradientBrush.IntermediateColors )
				{
					intermediates.Add( new KeyValuePair<double, Paint.Color>( kvp.Key, new Paint.Color( kvp.Value, kvp.Value.Alpha * _multiplier ) ) );
				}

				_newBrush = new Paint.Brushes.LinearGradientBrush
					( new Paint.Color( linearGradientBrush.StartColor, linearGradientBrush.StartColor.Alpha * _multiplier )
					, new Paint.Color( linearGradientBrush.EndColor, linearGradientBrush.EndColor.Alpha * _multiplier )
					, linearGradientBrush.StartPoint, linearGradientBrush.EndPoint, intermediates.ToArray() );
			}

			public override void VisitRadialGradientBrush( Types.Rectangle bounds, Paint.Brushes.RadialGradientBrush radialGradientBrush )
			{
				_newBrush = new VectorGraphics.Paint.Brushes.RadialGradientBrush
					( new Paint.Color( radialGradientBrush.InnerColor, radialGradientBrush.InnerColor.Alpha * _multiplier )
					, new Paint.Color( radialGradientBrush.OuterColor, radialGradientBrush.OuterColor.Alpha * _multiplier )
					, radialGradientBrush.InnerPoint, radialGradientBrush.OuterPoint );
			}

			private double _multiplier;
			private Paint.Brushes.Brush _newBrush;
		}

		#endregion
	}
}
