/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;

namespace ProgrammersInc.VectorGraphics.Renderers
{
	public class GdiPlusRenderer : Renderer, IDisposable
	{
		public enum MarkerHandling
		{
			Ignore,
			Display,
			Throw
		}

		public delegate Graphics CreateGraphics();

		public GdiPlusRenderer( CreateGraphics createGraphics, MarkerHandling markerHandling, double accuracy )
		{
			if( createGraphics == null )
			{
				throw new ArgumentNullException( "createGraphics" );
			}
			if( accuracy <= 0 )
			{
				throw new ArgumentException( "Accuracy must be greater than zero.", "accuracy" );
			}

			_createGraphics = createGraphics;
			_markerHandling = markerHandling;
			_accuracy = accuracy;
		}

		public void Render( Graphics g, Primitives.Container root, Types.Rectangle clip )
		{
			if( g == null )
			{
				throw new ArgumentNullException( "g" );
			}
			if( root == null )
			{
				throw new ArgumentNullException( "root" );
			}
			if( clip == null )
			{
				throw new ArgumentNullException( "clip" );
			}

			g.SmoothingMode = SmoothingMode.HighQuality;

			PrimitivesVisitor visitor = new PrimitivesVisitor( this, g, clip );

			root.Visit( visitor );
		}

		public void Render( Graphics g, Primitives.Container root )
		{
			if( g == null )
			{
				throw new ArgumentNullException( "g" );
			}
			if( root == null )
			{
				throw new ArgumentNullException( "root" );
			}

			g.SmoothingMode = SmoothingMode.HighQuality;

			PrimitivesVisitor visitor = new PrimitivesVisitor( this, g );

			root.Visit( visitor );
		}

		public MarkerHandling MarkerAction
		{
			get
			{
				return _markerHandling;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach( GraphicsPath gp in _mapPathToGraphicsPath.Values )
			{
				gp.Dispose();
			}
			foreach( Font font in _mapFontDetailsToFont.Values )
			{
				font.Dispose();
			}

			_mapPathToGraphicsPath.Clear();
			_mapFontDetailsToFont.Clear();
		}

		#endregion

		public override Primitives.Path FlattenPath( Primitives.Path source )
		{
			PathCommandVisitor visitor = new PathCommandVisitor();

			foreach( Primitives.Path.Command pathCommand in source.Commands )
			{
				pathCommand.Visit( visitor );
			}

			using( GraphicsPath gp = visitor.GetGraphicsPath() )
			{
				gp.Flatten();

				PointF lastPoint = PointF.Empty;

				Primitives.Path path = new Primitives.Path();

				path.Pen = source.Pen;
				path.Brush = source.Brush;

				for( int i = 0; i < gp.PointCount; ++i )
				{
					PointF point = gp.PathPoints[i];
					byte type = gp.PathTypes[i];
					PointF nextPoint = point;

					if( i < gp.PointCount - 1 && gp.PathTypes[i + 1] == 1 )
					{
						nextPoint = gp.PathPoints[i + 1];
					}

					switch( type )
					{
						case 0:
							{
								path.Add( new Primitives.Path.Move( new Types.Point( point.X, point.Y ) ) );
								break;
							}
						case 1:
							{
								bool first = (i == 0) || gp.PathTypes[i - 1] != 1;
								bool last = (i == gp.PointCount - 1) || gp.PathTypes[i + 1] != 1;

								if( first || last
									|| Math.Sqrt( Math.Pow( point.X - lastPoint.X, 2 ) + Math.Pow( point.Y - lastPoint.Y, 2 ) ) > _accuracy
									|| Math.Sqrt( Math.Pow( point.X - nextPoint.X, 2 ) + Math.Pow( point.Y - nextPoint.Y, 2 ) ) > _accuracy )
								{
									path.Add( new Primitives.Path.Line( new Types.Point( point.X, point.Y ) ) );
									lastPoint = point;
								}

								break;
							}
						case 129:
							{
								path.Add( new Primitives.Path.Line( new Types.Point( point.X, point.Y ) ) );
								path.Add( new Primitives.Path.Close() );
								break;
							}
						default:
							throw new InvalidOperationException();
					}
				}

				return path;
			}
		}

		public override void MeasureText( Primitives.Text text, out double width, out double height, out double baselineFromTop )
		{
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
			}

			Font font = CreateFont( text );

			height = font.SizeInPoints;

			Graphics g = _createGraphics();

			g.SmoothingMode = SmoothingMode.HighQuality;

			width = g.MeasureString( text.Value, font ).Width;

			double lineAscent = font.FontFamily.GetCellAscent( font.Style );
			double lineSpacing = font.FontFamily.GetLineSpacing( font.Style );

			height = font.GetHeight( g );

			baselineFromTop = height * lineAscent / lineSpacing;
		}

		private Font CreateFont( Primitives.Text text )
		{
			Font font;
			FontDetails fd = new FontDetails( text);

			if( _mapFontDetailsToFont.TryGetValue( fd, out font ) )
			{
				return font;
			}

			FontStyle fontStyle = FontStyle.Regular;

			if( (text.FontStyle & Primitives.Text.FontStyleFlags.Bold) != 0 )
			{
				fontStyle |= FontStyle.Bold;
			}
			if( (text.FontStyle & Primitives.Text.FontStyleFlags.Italic) != 0 )
			{
				fontStyle |= FontStyle.Italic;
			}
			if( (text.FontStyle & Primitives.Text.FontStyleFlags.Underline) != 0 )
			{
				fontStyle |= FontStyle.Underline;
			}

			font = new Font( text.FontFamily, (float) text.FontSizePoints, fontStyle );

			_mapFontDetailsToFont.Add( fd, font );

			return font;
		}

		private GraphicsPath CreateGraphicsPath( Primitives.Path path )
		{
			GraphicsPath gp;

			if( _mapPathToGraphicsPath.TryGetValue( path, out gp ) )
			{
				return gp;
			}

			PathCommandVisitor visitor = new PathCommandVisitor();

			foreach( Primitives.Path.Command pathCommand in path.Commands )
			{
				pathCommand.Visit( visitor );
			}

			gp = visitor.GetGraphicsPath();

			_mapPathToGraphicsPath.Add( path, gp );

			return gp;
		}

		private Pen[] CreatePens( Paint.Pens.Pen pen )
		{
			if( pen == null )
			{
				return new Pen[] { };
			}

			PenVisitor visitor = new PenVisitor();

			pen.Visit( visitor );

			return visitor.GetPens();
		}

		private BrushStage[] CreateBrushes( Graphics graphics, Types.Rectangle bounds, Paint.Brushes.Brush brush )
		{
			if( brush == null )
			{
				return new BrushStage[] { };
			}

			BrushVisitor visitor = new BrushVisitor( graphics );

			brush.Visit( bounds, visitor );

			return visitor.GetBrushes();
		}

		#region PrimitivesVisitor

		private sealed class PrimitivesVisitor : Primitives.Visitor
		{
			internal PrimitivesVisitor( GdiPlusRenderer renderer, Graphics g, Types.Rectangle clip )
			{
				_renderer = renderer;
				_graphics = g;
				_clip = clip;
			}

			internal PrimitivesVisitor( GdiPlusRenderer renderer, Graphics g )
			{
				_renderer = renderer;
				_graphics = g;
			}

			public override void VisitBoundsMarker( Primitives.BoundsMarker boundsMarker )
			{
				if( !InClip( boundsMarker ) )
				{
					return;
				}

				switch( _renderer.MarkerAction )
				{
					case MarkerHandling.Display:
						using( Pen pen = CreateDebugPen() )
						{
							_graphics.DrawRectangle
								( pen
								, (float) boundsMarker.Rectangle.X
								, (float) boundsMarker.Rectangle.Y
								, (float) boundsMarker.Rectangle.Width
								, (float) boundsMarker.Rectangle.Height );
						}
						break;
					case MarkerHandling.Throw:
						throw new InvalidOperationException( "BoundsMarker found by renderer." );
				}
			}

			public override void VisitPointMarker( Primitives.PointMarker pointMarker )
			{
				if( !InClip( pointMarker ) )
				{
					return;
				}

				switch( _renderer.MarkerAction )
				{
					case MarkerHandling.Display:
						using( Pen pen = CreateDebugPen() )
						{
							_graphics.DrawEllipse( pen, (float) pointMarker.Point.X - 2.0f, (float) pointMarker.Point.Y - 2.0f, 4.0f, 4.0f );
						}
						break;
					case MarkerHandling.Throw:
						throw new InvalidOperationException( "PointMarker found by renderer." );
				}
			}

			public override bool VisitContainerPreChildren( Primitives.Container container )
			{
				if( !InClip( container ) )
				{
					return false;
				}

				_transforms.Push( _graphics.Transform );

				_graphics.TranslateTransform( (float) container.Offset.X, (float) container.Offset.Y );

				return true;
			}

			public override void VisitContainerPostChildren( Primitives.Container container )
			{
				Matrix transform = _transforms.Pop();

				_graphics.Transform = transform;
			}

			public override void VisitPath( Primitives.Path path )
			{
				if( !InClip( path ) )
				{
					return;
				}

				Types.Rectangle bounds = path.GetBounds( _renderer );
				Pen[] pens = _renderer.CreatePens( path.Pen );
				BrushStage[] brushes = _renderer.CreateBrushes( _graphics, bounds, path.Brush );
				GraphicsPath gp = _renderer.CreateGraphicsPath( path );

				foreach( BrushStage brush in brushes )
				{
					_graphics.FillPath( brush.GetBrush(), gp );

					brush.Dispose();
				}
				foreach( Pen pen in pens )
				{
					_graphics.DrawPath( pen, gp );

					pen.Dispose();
				}
			}

			public override void VisitText( Primitives.Text text )
			{
				if( !InClip( text ) )
				{
					return;
				}

				Font font = _renderer.CreateFont( text );
				double width, height, baselineFromTop;

				_renderer.MeasureText( text, out width, out height, out baselineFromTop );

				double x, y;

				switch( text.Alignment )
				{
					case Primitives.Text.Position.TopLeft:
					case Primitives.Text.Position.CenterLeft:
					case Primitives.Text.Position.BaseLeft:
					case Primitives.Text.Position.BottomLeft:
						x = text.Point.X;
						break;
					case Primitives.Text.Position.TopCenter:
					case Primitives.Text.Position.Center:
					case Primitives.Text.Position.BaseCenter:
					case Primitives.Text.Position.BottomCenter:
						x = text.Point.X - width / 2 - 1;
						break;
					case Primitives.Text.Position.TopRight:
					case Primitives.Text.Position.CenterRight:
					case Primitives.Text.Position.BaseRight:
					case Primitives.Text.Position.BottomRight:
						x = text.Point.X - width;
						break;
					default:
						throw new InvalidOperationException();
				}

				switch( text.Alignment )
				{
					case Primitives.Text.Position.TopLeft:
					case Primitives.Text.Position.TopCenter:
					case Primitives.Text.Position.TopRight:
						y = text.Point.Y;
						break;
					case Primitives.Text.Position.CenterLeft:
					case Primitives.Text.Position.Center:
					case Primitives.Text.Position.CenterRight:
						y = text.Point.Y - height / 2;
						break;
					case Primitives.Text.Position.BaseLeft:
					case Primitives.Text.Position.BaseCenter:
					case Primitives.Text.Position.BaseRight:
						y = text.Point.Y - baselineFromTop;
						break;
					case Primitives.Text.Position.BottomLeft:
					case Primitives.Text.Position.BottomCenter:
					case Primitives.Text.Position.BottomRight:
						y = text.Point.Y - height;
						break;
					default:
						throw new InvalidOperationException();
				}

				using( StringFormat sf = new StringFormat( StringFormat.GenericTypographic ) )
				{
					sf.FormatFlags |= StringFormatFlags.NoClip;
					sf.FormatFlags |= StringFormatFlags.NoWrap;
					sf.Trimming = StringTrimming.EllipsisCharacter;

					using( Brush brush = new SolidBrush( GdiPlusUtility.Convert.Color( text.Color ) ) )
					{
						_graphics.DrawString( text.Value, font, brush, (float) x, (float) y );
					}
				}
			}

			private Pen CreateDebugPen()
			{
				Pen pen = new Pen( Color.Black, 1.0f );

				pen.DashStyle = DashStyle.Dash;

				return pen;
			}

			private bool InClip( Primitives.VisualItem item )
			{
				if( _clip == null )
				{
					return true;
				}

				PointF tl = new PointF( (float) _clip.Left, (float) _clip.Top );
				PointF br = new PointF( (float) _clip.Right, (float) _clip.Bottom );
				PointF[] pts = new PointF[] { tl, br };

				_graphics.TransformPoints( CoordinateSpace.World, CoordinateSpace.Device, pts );

				Types.Rectangle clip = new Types.Rectangle( pts[0].X, pts[0].Y, pts[1].X - pts[0].X, pts[1].Y - pts[0].Y );

				return Types.Rectangle.Overlap( item.GetBounds( _renderer ), clip );
			}

			private GdiPlusRenderer _renderer;
			private Graphics _graphics;
			private Stack<Matrix> _transforms = new Stack<Matrix>();
			private Types.Rectangle _clip;
		}

		#endregion

		#region PathCommandVisitor

		private sealed class PathCommandVisitor : Primitives.Path.Command.Visitor
		{
			public override void VisitMove( Primitives.Path.Move move )
			{
				_current = new GraphicsPath( FillMode.Winding );

				_x = _cy = move.To.X;
				_y = _cy = move.To.Y;
			}

			public override void VisitClose( Primitives.Path.Close close )
			{
				_current.CloseFigure();
				_x = _cy = 0;
				_y = _cy = 0;
			}

			public override void VisitLine( Primitives.Path.Line line )
			{
				_current.AddLine( (float) _x, (float) _y, (float) line.To.X, (float) line.To.Y );
				_x = _cx = line.To.X;
				_y = _cy = line.To.Y;
			}

			public override void VisitBezierCurve( Primitives.Path.BezierCurve curve )
			{
				_current.AddBezier
					( (float) _x, (float) _y
					, (float) curve.C1.X, (float) curve.C1.Y
					, (float) curve.C2.X, (float) curve.C2.Y
					, (float) curve.To.X, (float) curve.To.Y );

				_x = curve.To.X;
				_y = curve.To.Y;
				_cx = curve.C2.X;
				_cy = curve.C2.Y;
			}

			public override void VisitSmoothBezierCurve( Primitives.Path.SmoothBezierCurve smoothCurve )
			{
				_current.AddBezier
					( (float) _x, (float) _y
					, (float) _cx, (float) _cy
					, (float) smoothCurve.C2.X, (float) smoothCurve.C2.Y
					, (float) smoothCurve.To.X, (float) smoothCurve.To.Y );

				_x = smoothCurve.To.X;
				_y = smoothCurve.To.Y;
				_cx = smoothCurve.C2.X;
				_cy = smoothCurve.C2.Y;
			}

			public override void VisitEllipticalArc( Primitives.Path.EllipticalArc ellipticalArc )
			{
				double x1 = _x;
				double y1 = _y;
				double rx = ellipticalArc.RX * 0.999;
				double ry = ellipticalArc.RY * 0.999;
				double xrot = ellipticalArc.XAxisRotation;
				bool fa = ellipticalArc.LargeArcFlag;
				bool fs = ellipticalArc.SweepFlag;
				double ex = ellipticalArc.To.X;
				double ey = ellipticalArc.To.Y;
				double cosxrot = Math.Cos( xrot );
				double sinxrot = Math.Sin( xrot );
				double mx = (x1 - ex) / 2;
				double my = (y1 - ey) / 2;
				double xd = cosxrot * mx + sinxrot * my;
				double yd = cosxrot * my - sinxrot * mx;

				if( xd != 0 || yd != 0 )
				{
					double preSqrtM = (rx * rx * ry * ry) - (rx * rx * yd * yd) - (ry * ry * xd * xd);
					double preSqrtD = (rx * rx * yd * yd) + (ry * ry * xd * xd);
					double preSqrt = Math.Abs( preSqrtM / preSqrtD );
					double scale = Math.Sqrt( preSqrt );

					if( fa == fs )
					{
						scale = -scale;
					}

					double cxd = scale * rx * yd / ry;
					double cyd = -scale * ry * xd / rx;

					double ccx = cosxrot * cxd - sinxrot * cyd + (x1 + ex) / 2;
					double ccy = sinxrot * cxd + cosxrot * cyd + (y1 + ey) / 2;

					double start = Angle( 1, 0, (xd - cxd) / rx, (yd - cyd) / ry );
					double sweep = Angle( (xd - cxd) / rx, (yd - cyd) / ry, (-xd - cxd) / rx, (-yd - cyd) / ry );

					start = 180 * start / Math.PI;
					sweep = 180 * sweep / Math.PI;

					if( !fs && sweep > 0 )
					{
						sweep -= 360;
					}
					else if( fs && sweep < 0 )
					{
						sweep += 360;
					}

					_current.AddArc( (float) (ccx - rx), (float) (ccy - ry), (float) (rx * 2), (float) (ry * 2), (float) start, (float) sweep );
				}

				_x = ellipticalArc.To.X;
				_y = ellipticalArc.To.Y;
			}

			private static double Angle( double ux, double uy, double vx, double vy )
			{
				double lu = Math.Sqrt( ux * ux + uy * uy );
				double lv = Math.Sqrt( vx * vx + vy * vy );
				double a = Math.Acos( (ux * vx + uy * vy) / (lu * lv) );

				a *= Math.Sign( ux * vy - uy * vx );

				return a;
			}

			internal GraphicsPath GetGraphicsPath()
			{
				return _current;
			}

			private GraphicsPath _current = new GraphicsPath( FillMode.Winding );
			private double _x = 0, _y = 0, _cx = 0, _cy = 0;
		}

		#endregion

		#region PenVisitor

		private sealed class PenVisitor : Paint.Pens.PenVisitor
		{
			public override void VisitSolidPen( Paint.Pens.SolidPen solidPen )
			{
				Pen pen = new Pen( GdiPlusUtility.Convert.Color( solidPen.Color ), (float) solidPen.Width );

				pen.LineJoin = LineJoin.Round;

				_pens.Add( pen );
			}

			public Pen[] GetPens()
			{
				return _pens.ToArray();
			}

			private List<Pen> _pens = new List<Pen>();
		}

		#endregion

		#region BrushVisitor

		private sealed class BrushVisitor : Paint.Brushes.BrushVisitor
		{
			internal BrushVisitor( Graphics graphics )
			{
				_graphics = graphics;
			}

			public override void VisitSolidBrush( Types.Rectangle bounds, Paint.Brushes.SolidBrush solidBrush )
			{
				Brush brush = new SolidBrush( GdiPlusUtility.Convert.Color( solidBrush.Color ) );

				_brushes.Add( new StandardBrushStage( brush ) );
			}

			public override void VisitLinearGradientBrush( Types.Rectangle bounds, Paint.Brushes.LinearGradientBrush linearGradientBrush )
			{
				double maxClipRectSize = Math.Sqrt( bounds.Width * bounds.Width + bounds.Height * bounds.Height );
				double maxLinearVectorSize = Math.Sqrt
					( Math.Pow( linearGradientBrush.StartPoint.X - linearGradientBrush.EndPoint.X, 2 )
					+ Math.Pow( linearGradientBrush.StartPoint.Y - linearGradientBrush.EndPoint.Y, 2 ) );
				double maxSepSize = Math.Sqrt
					( Math.Pow( linearGradientBrush.StartPoint.X - bounds.X, 2 )
					+ Math.Pow( linearGradientBrush.StartPoint.Y - bounds.Y, 2 ) );
				double extent = maxClipRectSize + maxLinearVectorSize + maxSepSize;

				if( extent == 0 )
				{
					return;
				}

				PointF linearVec = new PointF
					( (float) (linearGradientBrush.EndPoint.X - linearGradientBrush.StartPoint.X)
					, (float) (linearGradientBrush.EndPoint.Y - linearGradientBrush.StartPoint.Y) );
				PointF vec = new PointF
					( (float) ((linearGradientBrush.EndPoint.X - linearGradientBrush.StartPoint.X) * extent / maxLinearVectorSize)
					, (float) ((linearGradientBrush.EndPoint.Y - linearGradientBrush.StartPoint.Y) * extent / maxLinearVectorSize) );
				PointF ivec = new PointF
					( (float) (-(linearGradientBrush.EndPoint.Y - linearGradientBrush.StartPoint.Y) * extent / maxLinearVectorSize)
					, (float) ((linearGradientBrush.EndPoint.X - linearGradientBrush.StartPoint.X) * extent / maxLinearVectorSize) );
				PointF initialPoint = new PointF( (float) linearGradientBrush.StartPoint.X, (float) linearGradientBrush.StartPoint.Y );
				PointF finalPoint = new PointF( (float) linearGradientBrush.EndPoint.X, (float) linearGradientBrush.EndPoint.Y );

				if( linearGradientBrush.Render == Paint.Brushes.LinearGradientBrush.RenderHint.Normal )
				{
					using( GraphicsPath gp = new GraphicsPath() )
					{
						gp.StartFigure();
						gp.AddPolygon(
							new PointF[] {
				      initialPoint,
				      PointDiff( initialPoint, ivec ),
				      PointDiff( PointDiff( initialPoint, ivec ), vec ),
				      PointDiff( initialPoint, vec),
				      PointAdd( PointDiff( initialPoint, vec), ivec ),
				      PointAdd( initialPoint, ivec ),
				      initialPoint
				    }
						);
						gp.CloseFigure();

						BrushStage brush = new ClippedStandardBrushStage( _graphics, new SolidBrush( GdiPlusUtility.Convert.Color( linearGradientBrush.StartColor ) ), new Region( gp ) );

						_brushes.Add( brush );
					}
				}

				List<KeyValuePair<double, Paint.Color>> steps = new List<KeyValuePair<double, Paint.Color>>();

				steps.Add( new KeyValuePair<double, Paint.Color>( 0, linearGradientBrush.StartColor ) );
				steps.AddRange( linearGradientBrush.IntermediateColors );
				steps.Add( new KeyValuePair<double, Paint.Color>( 1, linearGradientBrush.EndColor ) );

				if( maxLinearVectorSize > 0 )
				{
					for( int i = 0; i < steps.Count - 1; ++i )
					{
						double startProportion = steps[i].Key;
						double endProportion = steps[i + 1].Key;

						if( Math.Abs( startProportion - endProportion ) < 0.001 )
						{
							continue;
						}

						Paint.Color startColor = steps[i].Value;
						Paint.Color endColor = steps[i + 1].Value;

						PointF stepLinearVec = PointMul( linearVec, endProportion - startProportion );
						PointF stepInitialPoint = PointAdd( initialPoint, PointMul( linearVec, startProportion ) );
						PointF stepFinalPoint = PointAdd( initialPoint, PointMul( linearVec, endProportion ) );

						using( GraphicsPath gp = new GraphicsPath() )
						{
							gp.StartFigure();
							gp.AddPolygon(
								new PointF[] {
				      stepInitialPoint,
				      PointDiff( stepInitialPoint, ivec ),
				      PointAdd( PointDiff( stepInitialPoint, ivec ), stepLinearVec ),
				      PointAdd( stepInitialPoint, stepLinearVec ),
				      PointAdd( PointAdd( stepInitialPoint, stepLinearVec ), ivec ),
				      PointAdd( stepInitialPoint, ivec ),
				      stepInitialPoint
				    }
							);
							gp.CloseFigure();

							LinearGradientBrush linearBrush = new LinearGradientBrush
								( stepInitialPoint, stepFinalPoint, GdiPlusUtility.Convert.Color( startColor ), GdiPlusUtility.Convert.Color( endColor ) );

							linearBrush.WrapMode = WrapMode.TileFlipXY;

							BrushStage brush = new ClippedStandardBrushStage( _graphics, linearBrush, new Region( gp ) );

							_brushes.Add( brush );
						}
					}
				}

				if( linearGradientBrush.Render == Paint.Brushes.LinearGradientBrush.RenderHint.Normal )
				{
					using( GraphicsPath gp = new GraphicsPath() )
					{
						gp.StartFigure();
						gp.AddPolygon(
							new PointF[] {
				      finalPoint,
				      PointAdd( finalPoint, ivec ),
				      PointAdd( PointAdd( finalPoint, vec ), ivec ),
				      PointAdd( finalPoint, vec ),
				      PointAdd( PointDiff( finalPoint, ivec ), vec ),
				      PointDiff( finalPoint, ivec ),
				      finalPoint
				    }
						);
						gp.CloseFigure();

						BrushStage brush = new ClippedStandardBrushStage( _graphics, new SolidBrush( GdiPlusUtility.Convert.Color( linearGradientBrush.EndColor ) ), new Region( gp ) );

						_brushes.Add( brush );
					}
				}
			}

			public override void VisitRadialGradientBrush( Types.Rectangle bounds, Paint.Brushes.RadialGradientBrush radialGradientBrush )
			{
				double dx = radialGradientBrush.InnerPoint.X - radialGradientBrush.OuterPoint.X;
				double dy = radialGradientBrush.InnerPoint.Y - radialGradientBrush.OuterPoint.Y;

				float radius = (float) Math.Sqrt( dx * dx + dy * dy );

				if( radius == 0 )
				{
					return;
				}

				using( GraphicsPath gp = new GraphicsPath() )
				{
					gp.StartFigure();
					gp.AddEllipse( (float) radialGradientBrush.InnerPoint.X - radius, (float) radialGradientBrush.InnerPoint.Y - radius, radius * 2, radius * 2 );

					Region around = new Region();

					around.MakeInfinite();
					around.Xor( gp );

					SolidBrush b = new SolidBrush( GdiPlusUtility.Convert.Color( radialGradientBrush.OuterColor ) );

					_brushes.Add( new ClippedStandardBrushStage( _graphics, b, around ) );

					PathGradientBrush radialBrush = new PathGradientBrush( gp );

					radialBrush.CenterColor = GdiPlusUtility.Convert.Color( radialGradientBrush.InnerColor );
					radialBrush.CenterPoint = GdiPlusUtility.Convert.Point( radialGradientBrush.InnerPoint );
					radialBrush.SurroundColors = new Color[] { GdiPlusUtility.Convert.Color( radialGradientBrush.OuterColor ) };
					radialBrush.WrapMode = WrapMode.Clamp;

					_brushes.Add( new StandardBrushStage( radialBrush ) );
				}
			}

			public BrushStage[] GetBrushes()
			{
				return _brushes.ToArray();
			}

			private static PointF PointDiff( PointF p1, PointF p2 )
			{
				return new PointF( p1.X - p2.X, p1.Y - p2.Y );
			}

			private static PointF PointAdd( PointF p1, PointF p2 )
			{
				return new PointF( p1.X + p2.X, p1.Y + p2.Y );
			}

			private static PointF PointMul( PointF p, double f )
			{
				return new PointF( (float) (p.X * f), (float) (p.Y * f) );
			}

			private Graphics _graphics;
			private List<BrushStage> _brushes = new List<BrushStage>();
		}

		#endregion

		#region BrushStages

		private abstract class BrushStage : IDisposable
		{
			protected BrushStage()
			{
			}

			public abstract Brush GetBrush();

			protected virtual void Post()
			{
			}

			#region IDisposable Members

			public void Dispose()
			{
				Post();
			}

			#endregion
		}

		private class StandardBrushStage : BrushStage
		{
			internal StandardBrushStage( Brush brush )
			{
				_brush = brush;
			}

			public override Brush GetBrush()
			{
				return _brush;
			}

			protected override void Post()
			{
				if( _brush != null )
				{
					_brush.Dispose();
					_brush = null;
				}
			}

			private Brush _brush;
		}

		private sealed class ClippedStandardBrushStage : StandardBrushStage
		{
			internal ClippedStandardBrushStage( Graphics graphics, Brush brush, Region region )
				: base( brush )
			{
				_graphics = graphics;
				_region = region;
			}

			public override Brush GetBrush()
			{
				_graphics.SetClip( _region, CombineMode.Intersect );

				return base.GetBrush();
			}

			protected override void Post()
			{
				base.Post();

				if( _region != null )
				{
					_region.Dispose();
					_region = null;
				}
					
				_graphics.ResetClip();
			}

			private Graphics _graphics;
			private Region _region;
		}

		#endregion

		#region FontDetails

		private sealed class FontDetails
		{
			internal FontDetails( Primitives.Text text )
			{
				_fontFamily = text.FontFamily;
				_fontStyle = text.FontStyle;
				_fontSizePoints = text.FontSizePoints;
			}

			public override int GetHashCode()
			{
				return _fontFamily.GetHashCode() ^ _fontStyle.GetHashCode() ^ _fontSizePoints.GetHashCode();
			}

			public override bool Equals( object obj )
			{
				FontDetails fd = obj as FontDetails;

				if( fd == null )
				{
					return false;
				}
				else
				{
					return _fontFamily == fd._fontFamily && _fontStyle == fd._fontStyle && _fontSizePoints == fd._fontSizePoints;
				}
			}

			private string _fontFamily = "Arial";
			private Primitives.Text.FontStyleFlags _fontStyle;
			private double _fontSizePoints = 12;
		}

		#endregion

		private CreateGraphics _createGraphics;
		private MarkerHandling _markerHandling;
		private double _accuracy;
		private Dictionary<Primitives.Path, GraphicsPath> _mapPathToGraphicsPath = new Dictionary<Primitives.Path, GraphicsPath>();
		private Dictionary<FontDetails, Font> _mapFontDetailsToFont = new Dictionary<FontDetails, Font>();
	}
}
