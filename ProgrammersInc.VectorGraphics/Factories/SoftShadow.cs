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
	//
	// TODO:
	// http://www.codeproject.com/useritems/FuzzyDropShadows.asp
	//
	public sealed class SoftShadow : Shadow
	{
		public SoftShadow( Renderers.Renderer renderer, Types.Point offset, double extent, Paint.Color color )
		{
			if( renderer == null )
			{
				throw new ArgumentNullException( "renderer" );
			}
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

			_renderer = renderer;
			_offset = offset;
			_extent = extent;
			_color = color;
		}

		public override Primitives.VisualItem Create( Primitives.Path source )
		{
			Primitives.Container container = new Primitives.Container( _offset );
			PathCommandVisitor visitor = new PathCommandVisitor( this );

			source = _renderer.FlattenPath( source );

			foreach( Primitives.Path.Command command in source.Commands )
			{
				command.Visit( visitor );
			}

			foreach( Primitives.Path path in visitor.Paths )
			{
				container.AddFront( path );
			}

			return container;
		}

		private sealed class PathCommandVisitor : Primitives.Path.Command.Visitor
		{
			internal PathCommandVisitor( SoftShadow softShadow )
			{
				_softShadow = softShadow;
				_innerPath.Pen = null;
				_innerPath.Brush = new Paint.Brushes.SolidBrush( _softShadow._color );
			}

			internal Primitives.Path[] Paths
			{
				get
				{
					Primitives.Path[] paths = new Primitives.Path[_parts.Count + 1];

					_parts.CopyTo( paths, 0 );
					paths[paths.Length - 1] = _innerPath;

					return paths;
				}
			}

			public override void VisitMove( Primitives.Path.Move move )
			{
				_pos = move.To;
				_estartpos = _epos = null;
				_startpos = _lastpos = _pos;
				_startev = null;

				_innerPath.Add( new Primitives.Path.Move( move.To ) );
			}

			public override void VisitClose( Primitives.Path.Close close )
			{
				Types.Vector lv = _startpos - _lastpos;
				Types.Vector ev = new Types.Vector( lv.Y, -lv.X ).Normalize( _softShadow._extent );

				if( _estartpos != null )
				{
					CreateFiller( ev );
				}

				Primitives.Path part = new VectorGraphics.Primitives.Path();

				part.Add( new Primitives.Path.Move( _lastpos + ev ) );
				part.Add( new Primitives.Path.Line( _startpos + ev ) );
				part.Add( new Primitives.Path.Line( _startpos ) );
				part.Add( new Primitives.Path.Line( _lastpos ) );
				part.Add( new Primitives.Path.Line( _lastpos + ev ) );
				part.Add( new Primitives.Path.Close() );

				part.Pen = null;

				Types.Point gstart = _startpos, gend = _startpos + ev;

				part.Brush = new Paint.Brushes.LinearGradientBrush
					( _softShadow._color, Paint.Color.Transparent, gstart, gend, Paint.Brushes.LinearGradientBrush.RenderHint.NoClip );

				_parts.Add( part );

				if( _estartpos == null )
				{
					_estartpos = _pos + ev;
				}

				_lastpos = _pos = _startpos;
				_epos = _pos + ev;

				_innerPath.Add( new Primitives.Path.Line( _startpos ) );

				CreateFiller( _startev );

				_innerPath.Add( new Primitives.Path.Close() );
			}

			public override void VisitLine( Primitives.Path.Line line )
			{
				Types.Vector lv = line.To - _lastpos;
				Types.Vector ev = new Types.Vector( lv.Y, -lv.X ).Normalize( _softShadow._extent );

				if( _epos != null )
				{
					CreateFiller( ev );
				}

				Primitives.Path part = new VectorGraphics.Primitives.Path();

				part.Add( new Primitives.Path.Move( _lastpos + ev ) );
				part.Add( new Primitives.Path.Line( line.To + ev ) );
				part.Add( new Primitives.Path.Line( line.To ) );
				part.Add( new Primitives.Path.Line( _lastpos ) );
				part.Add( new Primitives.Path.Line( _lastpos + ev ) );
				part.Add( new Primitives.Path.Close() );

				part.Pen = null;

				Types.Point gstart = line.To, gend = line.To + ev;

				part.Brush = new Paint.Brushes.LinearGradientBrush
					( _softShadow._color, Paint.Color.Transparent, gstart, gend, Paint.Brushes.LinearGradientBrush.RenderHint.NoClip );

				_parts.Add( part );

				if( _estartpos == null )
				{
					_estartpos = _pos + ev;
				}
				if( _startev == null )
				{
					_startev = ev;
				}

				_lastpos = _pos = line.To;
				_epos = _pos + ev;

				_innerPath.Add( new Primitives.Path.Line( line.To ) );
			}

			public override void VisitEllipticalArc( Primitives.Path.EllipticalArc ellipticalArc )
			{
				_lastpos = ellipticalArc.To;
			}

			public override void VisitBezierCurve( Primitives.Path.BezierCurve curve )
			{
				_lastpos = curve.To;
			}

			public override void VisitSmoothBezierCurve( Primitives.Path.SmoothBezierCurve smoothCurve )
			{
				_lastpos = smoothCurve.To;
			}

			private void CreateFiller( Types.Vector ev2 )
			{
				Types.Vector ev1 = _epos - _pos;
				Types.Vector lv1 = _lastpos - _pos;
				Types.Vector lv2 = (_lastpos + ev2) - _epos;

				Primitives.Path part = new VectorGraphics.Primitives.Path();

				part.Add( new Primitives.Path.Move( _lastpos ) );

				part.Add( new Primitives.Path.Line( _lastpos + ev2 ) );
				part.Add( new Primitives.Path.EllipticalArc( _softShadow._extent, _softShadow._extent, 0, false, false, _epos ) );

				part.Add( new Primitives.Path.Line( _lastpos ) );

				part.Add( new Primitives.Path.Close() );

				part.Pen = null;

				Types.Point gstart = _pos + (_lastpos - _pos) / 2, gend = gstart + (ev1 + ev2).Normalize( _softShadow._extent * 2/3 );

				part.Brush = new Paint.Brushes.RadialGradientBrush
					( _softShadow._color, Paint.Color.Transparent, gstart, gend );

				_parts.Add( part );

				_innerPath.Add( new Primitives.Path.Line( _lastpos ) );
			}

			private SoftShadow _softShadow;
			private Types.Point _pos, _epos, _lastpos, _estartpos, _startpos;
			private List<Primitives.Path> _parts = new List<Primitives.Path>();
			private Primitives.Path _innerPath = new Primitives.Path();
			private Types.Vector _startev;
		}

		private Renderers.Renderer _renderer;
		private Types.Point _offset;
		private Paint.Color _color;
		private double _extent;
	}
}
