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

namespace ProgrammersInc.VectorGraphics.Primitives
{
	public sealed class Text : VisualItem
	{
		public enum Position
		{
			TopLeft,			TopCenter,		TopRight,
			CenterLeft,		Center,				CenterRight,
			BaseLeft,			BaseCenter,		BaseRight,
			BottomLeft,		BottomCenter,	BottomRight
		}

		[Flags]
		public enum FontStyleFlags
		{
			Normal = 0,
			Bold = 1,
			Italic = 2,
			Underline = 4
		}

		public Text( string value, Types.Point point, Position alignment )
		{
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}
			if( point == null )
			{
				throw new ArgumentNullException( "point" );
			}

			_value = value;
			_point = point;
			_alignment = alignment;
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}

		public Types.Point Point
		{
			get
			{
				return _point;
			}
		}

		public Position Alignment
		{
			get
			{
				return _alignment;
			}
		}

		public Paint.Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_color = value;
			}
		}

		public string FontFamily
		{
			get
			{
				return _fontFamily;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_fontFamily = value;
			}
		}

		public FontStyleFlags FontStyle
		{
			get
			{
				return _fontStyle;
			}
			set
			{
				_fontStyle = value;
			}
		}

		public double FontSizePoints
		{
			get
			{
				return _fontSizePoints;
			}
			set
			{
				if( value < 0 )
				{
					throw new ArgumentException( "Font size must be positive.", "value" );
				}

				_fontSizePoints = value;
			}
		}

		public override VisualItem Copy()
		{
			Text text = new Text( _value, _point, _alignment );

			text.Color = Color;
			text.FontFamily = FontFamily;
			text.FontStyle = FontStyle;
			text.FontSizePoints = FontSizePoints;

			return text;
		}

		public override void Visit( Visitor visitor )
		{
			visitor.PreVisitVisualItem( this );
			visitor.VisitText( this );
			visitor.PostVisitVisualItem( this );
		}

		protected override Types.Rectangle CalculateBounds( Renderers.Renderer renderer )
		{
			double width, height, baselineFromTop, x, y;

			renderer.MeasureText( this, out width, out height, out baselineFromTop );

			switch( Alignment )
			{
				case Primitives.Text.Position.TopLeft:
				case Primitives.Text.Position.CenterLeft:
				case Primitives.Text.Position.BaseLeft:
				case Primitives.Text.Position.BottomLeft:
					x = _point.X;
					break;
				case Primitives.Text.Position.TopCenter:
				case Primitives.Text.Position.Center:
				case Primitives.Text.Position.BaseCenter:
				case Primitives.Text.Position.BottomCenter:
					x = _point.X - width / 2 - 1;
					break;
				case Primitives.Text.Position.TopRight:
				case Primitives.Text.Position.CenterRight:
				case Primitives.Text.Position.BaseRight:
				case Primitives.Text.Position.BottomRight:
					x = _point.X - width;
					break;
				default:
					throw new InvalidOperationException();
			}

			switch( Alignment )
			{
				case Primitives.Text.Position.TopLeft:
				case Primitives.Text.Position.TopCenter:
				case Primitives.Text.Position.TopRight:
					y = _point.Y;
					break;
				case Primitives.Text.Position.CenterLeft:
				case Primitives.Text.Position.Center:
				case Primitives.Text.Position.CenterRight:
					y = _point.Y - height / 2;
					break;
				case Primitives.Text.Position.BaseLeft:
				case Primitives.Text.Position.BaseCenter:
				case Primitives.Text.Position.BaseRight:
					y = _point.Y - baselineFromTop;
					break;
				case Primitives.Text.Position.BottomLeft:
				case Primitives.Text.Position.BottomCenter:
				case Primitives.Text.Position.BottomRight:
					y = _point.Y - height;
					break;
				default:
					throw new InvalidOperationException();
			}

			return new VectorGraphics.Types.Rectangle( x, y, width, height );
		}

		private string _value;
		private Types.Point _point;
		private Position _alignment;
		private Paint.Color _color = Paint.Color.Black;
		private string _fontFamily = "Arial";
		private FontStyleFlags _fontStyle;
		private double _fontSizePoints = 12;
	}
}
