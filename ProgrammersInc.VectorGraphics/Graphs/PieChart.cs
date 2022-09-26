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

namespace ProgrammersInc.VectorGraphics.Graphs
{
	public sealed class PieChart
	{
		public sealed class Settings : GraphSettings
		{
		}

		public Primitives.Container Create( IData data, Settings settings )
		{
			if( data == null )
			{
				throw new ArgumentNullException( "data" );
			}
			if( settings == null )
			{
				throw new ArgumentNullException( "settings" );
			}
			if( data.ColumnCount != 1 )
			{
				throw new ArgumentException( "Pie data must have exactly one column.", "data" );
			}

			double cx = settings.Width / 2, cy = settings.Height / 2;
			double radius = Math.Min( cx * 0.8, cy * 0.8);
			double total = 0;

			for( int i = 0; i < data.RowCount; ++i )
			{
				double v = data[i, 0];

				if( v < 0 )
				{
					v = 0;
				}

				total += v;
			}

			Primitives.Container container = new Primitives.Container();

			Primitives.BoundsMarker bounds = new Primitives.BoundsMarker( new Types.Rectangle( 0, 0, settings.Width, settings.Height ) );

			bounds.Style.Add( "GraphContainer" );

			container.AddBack( bounds );

			Primitives.Container pie = new Primitives.Container( new Types.Point( cx, cy ) );

			double angle = 0;

			if( total > 0 )
			{
				for( int r = 0; r < data.RowCount; ++r )
				{
					double v = data[r, 0];

					if( v < 0 )
					{
						v = 0;
					}

					double sweep = 2 * Math.PI * v / total;
					bool exploded = bool.Parse( data.GetRowExtra( r, "Exploded", "false" ) );

					pie.AddBack( CreateSlice( r, data.RowCount, angle, sweep, radius, exploded ? radius * 0.15 : 0 ) );

					angle += sweep;
				}
			}

			container.AddBack( pie );

			return container;
		}

		private Primitives.Path CreateSlice( int index, int count, double start, double sweep, double radius, double move )
		{
			Primitives.Path path = new Primitives.Path();

			double cx = Math.Sin( start + sweep / 2 ) * move, cy = -Math.Cos( start + sweep / 2 ) * move;

			double sx = Math.Sin( start ) * radius, sy = -Math.Cos( start ) * radius;
			double ex = Math.Sin( start + sweep ) * radius, ey = -Math.Cos( start + sweep ) * radius;

			path.Add( new Primitives.Path.Move( new Types.Point( cx, cy ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( cx + sx, cy + sy ) ) );
			path.Add( new Primitives.Path.EllipticalArc( radius, radius, 0, (sweep > Math.PI), true, new Types.Point( cx + ex, cy + ey ) ) );
			path.Add( new Primitives.Path.Line( new Types.Point( cx, cy ) ) );
			path.Add( new Primitives.Path.Close() );

			path.Style.Add( "GraphPiece" );
			path.Style.AddExtra( "RowIndex", index.ToString() );
			path.Style.AddExtra( "RowCount", count.ToString() );

			return path;
		}
	}
}
