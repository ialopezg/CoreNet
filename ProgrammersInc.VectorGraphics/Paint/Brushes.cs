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

namespace ProgrammersInc.VectorGraphics.Paint.Brushes
{
	#region BrushVisitor

	public abstract class BrushVisitor
	{
		public abstract void VisitSolidBrush( Types.Rectangle bounds, SolidBrush solidBrush );
		public abstract void VisitLinearGradientBrush( Types.Rectangle bounds, LinearGradientBrush linearGradientBrush );
		public abstract void VisitRadialGradientBrush( Types.Rectangle bounds, RadialGradientBrush radialGradientBrush );
	}

	#endregion

	#region Brush

	public abstract class Brush
	{
		protected Brush()
		{
		}

		public abstract void Visit( Types.Rectangle bounds, BrushVisitor visitor );
	}

	#endregion

	#region SolidBrush

	public sealed class SolidBrush : Brush
	{
		public SolidBrush( Color color )
		{
			if( color == null )
			{
				throw new ArgumentNullException( "color" );
			}

			_color = color;
		}

		public Color Color
		{
			get
			{
				return _color;
			}
		}

		public override void Visit( Types.Rectangle bounds, BrushVisitor visitor )
		{
			visitor.VisitSolidBrush( bounds, this );
		}

		private Color _color;
	}

	#endregion

	#region LinearGradientBrush

	public sealed class LinearGradientBrush : Brush
	{
		public enum RenderHint
		{
			Normal,
			NoClip
		}

		public LinearGradientBrush( Color startColor, Color endColor, Types.Point startPoint, Types.Point endPoint, params KeyValuePair<double, Color>[] intermediateColors )
			: this( startColor, endColor, startPoint, endPoint, RenderHint.Normal, intermediateColors )
		{
		}

		public LinearGradientBrush( Color startColor, Color endColor, Types.Point startPoint, Types.Point endPoint, RenderHint renderHint, params KeyValuePair<double, Color>[] intermediateColors )
		{
			if( startColor == null )
			{
				throw new ArgumentNullException( "startColor" );
			}
			if( endColor == null )
			{
				throw new ArgumentNullException( "endColor" );
			}
			if( startPoint == null )
			{
				throw new ArgumentNullException( "startPoint" );
			}
			if( endPoint == null )
			{
				throw new ArgumentNullException( "endPoint" );
			}
			if( intermediateColors == null )
			{
				throw new ArgumentNullException( "intermediateColors" );
			}

			foreach( KeyValuePair<double, Color> kvp in intermediateColors )
			{
				if( kvp.Key < 0 || kvp.Key > 1 )
				{
					throw new ArgumentException( "Intermediate color position out-of-range.", "intermediateColors" );
				}
			}

			_startColor = startColor;
			_endColor = endColor;
			_startPoint = startPoint;
			_endPoint = endPoint;
			_renderHint = renderHint;
			_intermediateColors = intermediateColors;
		}

		public Color StartColor
		{
			get
			{
				return _startColor;
			}
		}

		public Color EndColor
		{
			get
			{
				return _endColor;
			}
		}

		public Types.Point StartPoint
		{
			get
			{
				return _startPoint;
			}
		}

		public Types.Point EndPoint
		{
			get
			{
				return _endPoint;
			}
		}

		public RenderHint Render
		{
			get
			{
				return _renderHint;
			}
		}

		public KeyValuePair<double, Color>[] IntermediateColors
		{
			get
			{
				return _intermediateColors;
			}
		}

		public override void Visit( Types.Rectangle bounds, BrushVisitor visitor )
		{
			visitor.VisitLinearGradientBrush( bounds, this );
		}

		private Color _startColor, _endColor;
		private Types.Point _startPoint, _endPoint;
		private RenderHint _renderHint;
		private KeyValuePair<double, Color>[] _intermediateColors;
	}

	#endregion

	#region RadialGradientBrush

	public sealed class RadialGradientBrush : Brush
	{
		public RadialGradientBrush( Color innerColor, Color outerColor, Types.Point innerPoint, Types.Point outerPoint )
		{
			if( innerColor == null )
			{
				throw new ArgumentNullException( "innerColor" );
			}
			if( outerColor == null )
			{
				throw new ArgumentNullException( "outerColor" );
			}
			if( innerPoint == null )
			{
				throw new ArgumentNullException( "innerPoint" );
			}
			if( outerPoint == null )
			{
				throw new ArgumentNullException( "outerPoint" );
			}

			_innerColor = innerColor;
			_outerColor = outerColor;
			_innerPoint = innerPoint;
			_outerPoint = outerPoint;
		}

		public Color InnerColor
		{
			get
			{
				return _innerColor;
			}
		}

		public Color OuterColor
		{
			get
			{
				return _outerColor;
			}
		}

		public Types.Point InnerPoint
		{
			get
			{
				return _innerPoint;
			}
		}

		public Types.Point OuterPoint
		{
			get
			{
				return _outerPoint;
			}
		}

		public override void Visit( Types.Rectangle bounds, BrushVisitor visitor )
		{
			visitor.VisitRadialGradientBrush( bounds, this );
		}

		private Color _innerColor, _outerColor;
		private Types.Point _innerPoint, _outerPoint;
	}

	#endregion
}
