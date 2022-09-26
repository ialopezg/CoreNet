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
	public sealed class LineChart
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
			if( data.ColumnCount < 2 )
			{
				throw new ArgumentException( "Line data must have at least two columns.", "data" );
			}

			Primitives.Container container = new Primitives.Container();

			Primitives.BoundsMarker bounds = new Primitives.BoundsMarker( new Types.Rectangle( 0, 0, settings.Width, settings.Height ) );

			bounds.Style.Add( "GraphContainer" );

			container.AddBack( bounds );

			Primitives.Container lines = new Primitives.Container();

			double minHorzData = double.MaxValue;
			double maxHorzData = double.MinValue;

			for( int i = 0; i < data.RowCount; ++i )
			{
				double v = data[i, 0];

				minHorzData = Math.Min( minHorzData, v );
				maxHorzData = Math.Max( maxHorzData, v );
			}

			double minVertData = double.MaxValue;
			double maxVertData = double.MinValue;

			for( int i = 0; i < data.RowCount; ++i )
			{
				for( int j = 1; j < data.ColumnCount; ++j )
				{
					double v = data[i, j];

					minVertData = Math.Min( minVertData, v );
					maxVertData = Math.Max( maxVertData, v );
				}
			}

			if( maxVertData == minVertData )
			{
				maxVertData = minVertData + 1;
			}

			double maxExtent = Math.Max( settings.Width, settings.Height);
			double textHeight = 2;
			double border = 20;
			Types.Rectangle visibleArea = new Types.Rectangle( border, border, settings.Width - border * 2, settings.Height - border * 2 - textHeight );
			double zero = visibleArea.Bottom + minVertData / (maxVertData - minVertData) * visibleArea.Height;

			for( int col = 1; col < data.ColumnCount; ++col )
			{
				Primitives.Container line = new Primitives.Container();

				List<Types.Point> points = new List<Types.Point>();
				List<Primitives.Path.Command> pathCommands = new List<Primitives.Path.Command>();

				pathCommands.Add( new Primitives.Path.Move( new Types.Point( visibleArea.X, zero ) ) );

				for( int row = 0; row < data.RowCount; ++row )
				{
					double y = data[row, col];
					double x = data[row, 0];

					x = visibleArea.X + (x - minHorzData) / (maxHorzData - minHorzData) * visibleArea.Width;
					y = visibleArea.Bottom - (y - minVertData) / (maxVertData - minVertData) * visibleArea.Height;

					Primitives.Path.Command pathCommand = new Primitives.Path.Line( new Types.Point( x, y ) );

					pathCommands.Add( pathCommand );
				}

				pathCommands.Add( new Primitives.Path.Line( new Types.Point( visibleArea.Right, zero ) ) );
				pathCommands.Add( new Primitives.Path.Close() );

				Primitives.Path path = new Primitives.Path( pathCommands.ToArray() );

				path.Style.Add( "GraphPiece" );
				path.Style.AddExtra( "RowIndex", (col - 1).ToString() );
				path.Style.AddExtra( "RowCount", (data.ColumnCount - 1).ToString() );

				line.AddBack( path );

				lines.AddBack( line );
			}

			container.AddBack( lines );

			double available = settings.Height - visibleArea.Bottom;

			for( int i = 0; i < data.RowCount; ++i )
			{
				string label = data.GetRowExtra( i, "LABEL", null );

				if( label != null )
				{
					double x = data[i, 0];

					x = visibleArea.X + (x - minHorzData) / (maxHorzData - minHorzData) * visibleArea.Width;

					Primitives.Text text = new Primitives.Text( label, new Types.Point( x, visibleArea.Bottom + available / 5 ), Primitives.Text.Position.TopCenter );

					text.FontSizePoints = textHeight / 2;
					text.Style.Add( "GraphText" );

					container.AddBack( text );
				}
			}

			return container;
		}
	}
}
