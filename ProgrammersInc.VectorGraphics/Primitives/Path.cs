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
	public sealed class Path : VisualItem
	{
		#region Command

		public abstract class Command
		{
			public abstract class Visitor
			{
				public abstract void VisitMove( Move move );
				public abstract void VisitClose( Close close );
				public abstract void VisitLine( Line line );
				public abstract void VisitBezierCurve( BezierCurve curve );
				public abstract void VisitSmoothBezierCurve( SmoothBezierCurve smoothCurve );
				public abstract void VisitEllipticalArc( EllipticalArc ellipticalArc );
			}

			public abstract void Visit( Visitor visitor );
			public abstract double[] GetBoundsXCoordinates();
			public abstract double[] GetBoundsYCoordinates();
		}

		#endregion
		#region Move

		public sealed class Move : Command
		{
			public Move( Types.Point to )
			{
				if( to == null )
				{
					throw new ArgumentNullException( "to" );
				}

				_to = to;
			}

			public Types.Point To
			{
				get
				{
					return _to;
				}
			}

			public override void Visit( Visitor visitor )
			{
				visitor.VisitMove( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { _to.X };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { _to.Y };
			}

			private Types.Point _to;
		}

		#endregion
		#region Close

		public sealed class Close : Command
		{
			public override void Visit( Visitor visitor )
			{
				visitor.VisitClose( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { };
			}
		}

		#endregion
		#region Line

		public sealed class Line : Command
		{
			public Line( Types.Point to )
			{
				if( to == null )
				{
					throw new ArgumentNullException( "to" );
				}

				_to = to;
			}

			public Types.Point To
			{
				get
				{
					return _to;
				}
			}

			public override void Visit( Visitor visitor )
			{
				visitor.VisitLine( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { _to.X };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { _to.Y };
			}

			private Types.Point _to;
		}

		#endregion
		#region BezierCurve

		public sealed class BezierCurve : Command
		{
			public BezierCurve( Types.Point c1, Types.Point c2, Types.Point to )
			{
				if( c1 == null )
				{
					throw new ArgumentNullException( "c1" );
				}
				if( c2 == null )
				{
					throw new ArgumentNullException( "c2" );
				}
				if( to == null )
				{
					throw new ArgumentNullException( "to" );
				}

				_c1 = c1;
				_c2 = c2;
				_to = to;
			}

			public Types.Point C1
			{
				get
				{
					return _c1;
				}
			}

			public Types.Point C2
			{
				get
				{
					return _c2;
				}
			}

			public Types.Point To
			{
				get
				{
					return _to;
				}
			}

			public override void Visit( Visitor visitor )
			{
				visitor.VisitBezierCurve( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { _to.X, _c1.X, _c2.X };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { _to.Y, _c1.Y, _c2.Y };
			}

			private Types.Point _c1, _c2, _to;
		}

		#endregion
		#region SmoothBezierCurve

		public sealed class SmoothBezierCurve : Command
		{
			public SmoothBezierCurve( Types.Point c2, Types.Point to )
			{
				if( c2 == null )
				{
					throw new ArgumentNullException( "c2" );
				}
				if( to == null )
				{
					throw new ArgumentNullException( "to" );
				}

				_c2 = c2;
				_to = to;
			}

			public Types.Point C2
			{
				get
				{
					return _c2;
				}
			}

			public Types.Point To
			{
				get
				{
					return _to;
				}
			}

			public override void Visit( Visitor visitor )
			{
				visitor.VisitSmoothBezierCurve( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { _to.X, _c2.X };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { _to.Y, _c2.Y };
			}

			private Types.Point _c2, _to;
		}

		#endregion
		#region EllipticalArc

		public sealed class EllipticalArc : Command
		{
			public EllipticalArc( double rx, double ry, double xAxisRotation, bool largeArcFlag, bool sweepFlag, Types.Point to )
			{
				if( to == null )
				{
					throw new ArgumentNullException( "to" );
				}

				_rx = rx;
				_ry = ry;
				_xAxisRotation = xAxisRotation;
				_largeArcFlag = largeArcFlag;
				_sweepFlag = sweepFlag;
				_to = to;
			}

			public double RX
			{
				get
				{
					return _rx;
				}
			}

			public double RY
			{
				get
				{
					return _ry;
				}
			}

			public double XAxisRotation
			{
				get
				{
					return _xAxisRotation;
				}
			}

			public bool LargeArcFlag
			{
				get
				{
					return _largeArcFlag;
				}
			}

			public bool SweepFlag
			{
				get
				{
					return _sweepFlag;
				}
			}

			public Types.Point To
			{
				get
				{
					return _to;
				}
			}

			public override void Visit( Visitor visitor )
			{
				visitor.VisitEllipticalArc( this );
			}

			public override double[] GetBoundsXCoordinates()
			{
				return new double[] { _to.X, _to.X - _rx * 2, _to.X + _rx * 2 };
			}

			public override double[] GetBoundsYCoordinates()
			{
				return new double[] { _to.Y, _to.Y - _ry * 2, _to.Y + _ry * 2 };
			}

			private double _rx, _ry, _xAxisRotation;
			private bool _largeArcFlag, _sweepFlag;
			private Types.Point _to;
		}

		#endregion

		public Path()
			: this( new Command[] { } )
		{
		}

		public Path( Command[] commands )
		{
			if( commands == null )
			{
				throw new ArgumentNullException( "commands" );
			}

			foreach( Command command in commands )
			{
				if( command == null )
				{
					throw new ArgumentException( "Null command found in array.", "commands" );
				}

				_commands.Add( command );
			}
		}

		public Command[] Commands
		{
			get
			{
				return _commands.ToArray();
			}
		}

		public Paint.Pens.Pen Pen
		{
			get
			{
				return _pen;
			}
			set
			{
				_pen = value;
			}
		}

		public Paint.Brushes.Brush Brush
		{
			get
			{
				return _brush;
			}
			set
			{
				_brush = value;
			}
		}

		public void Add( Command command )
		{
			if( command == null )
			{
				throw new ArgumentNullException( "command" );
			}

			_commands.Add( command );
		}

		public override void Visit( Visitor visitor )
		{
			visitor.PreVisitVisualItem( this );
			visitor.VisitPath( this );
			visitor.PostVisitVisualItem( this );
		}

		public override VisualItem Copy()
		{
			//
			// Commands are readonly so we can safely copy references.

			Path path = new Path( _commands.ToArray() );

			path.Pen = Pen;
			path.Brush = Brush;

			return path;
		}

		protected override VectorGraphics.Types.Rectangle CalculateBounds( Renderers.Renderer renderer )
		{
			double minx = double.MaxValue, maxx = double.MinValue, miny = double.MaxValue, maxy = double.MinValue;

			foreach( Command command in Commands )
			{
				foreach( double x in command.GetBoundsXCoordinates() )
				{
					minx = Math.Min( minx, x );
					maxx = Math.Max( maxx, x );
				}
				foreach( double y in command.GetBoundsYCoordinates() )
				{
					miny = Math.Min( miny, y );
					maxy = Math.Max( maxy, y );
				}
			}

			if( minx == double.MaxValue )
			{
				minx = 0;
			}
			if( miny == double.MaxValue )
			{
				miny = 0;
			}

			double width = maxx - minx, height = maxy - miny;

			if( width < 0 )
			{
				width = 0;
			}
			if( height < 0 )
			{
				height = 0;
			}

			return new Types.Rectangle( minx, miny, width, height );
		}

		private List<Command> _commands = new List<Command>();
		private Paint.Pens.Pen _pen = new Paint.Pens.SolidPen( Paint.Color.Black, 1 );
		private Paint.Brushes.Brush _brush;
	}
}
