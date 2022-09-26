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
	public sealed class BarChart
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

			Primitives.Container container = new Primitives.Container();

			Primitives.BoundsMarker bounds = new Primitives.BoundsMarker( new Types.Rectangle( 0, 0, settings.Width, settings.Height ) );

			bounds.Style.Add( "GraphContainer" );

			container.AddBack( bounds );

			double minVertData = double.MaxValue;
			double maxVertData = double.MinValue;

			for( int i = 0; i < data.RowCount; ++i )
			{
				for( int j = 0; j < data.ColumnCount; ++j )
				{
					double v = data[i, j];

					minVertData = Math.Min( minVertData, v );
					maxVertData = Math.Max( maxVertData, v );
				}
			}

			return container;
		}
	}
}
