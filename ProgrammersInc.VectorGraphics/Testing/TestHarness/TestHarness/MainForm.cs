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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BinaryComponents.VectorGraphics.TestHarness
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				, true );

			_renderer = new Renderers.GdiPlusRenderer( CreateGraphics, Renderers.GdiPlusRenderer.MarkerHandling.Display, 5 );

			Create();
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( keyData == Keys.F5 )
			{
				Create();
				Invalidate();
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		private void Create()
		{
			Factories.Rectangle rectangleFactory = new Factories.Rectangle();
			Factories.RoundedRectangle roundedRectangleFactory = new Factories.RoundedRectangle();
			Factories.SoftShadow softShadowFactory = new Factories.SoftShadow( _renderer, new Types.Point( 2, 2 ), 4, new Paint.Color( 0, 0, 0, 0.4 ) );
			Factories.GlossyBrush glossyBrush = new Factories.GlossyBrush();

			Random random = new Random();
			Styles.StyleSet style = new StyleSets.BlueAndWhite();

			_container = new Primitives.Container();

			//
			// Create pie chart.

			int pieParts = 5;
			Graphs.StaticData pieChartData = new Graphs.StaticData( pieParts, 1 );

			for( int i = 0; i < pieParts; ++i )
			{
			  pieChartData[i, 0] = random.NextDouble();
			}

			pieChartData.SetRowExtra( 2, "Exploded", "true" );

			Graphs.PieChart.Settings pieChartSettings = new Graphs.PieChart.Settings();

			pieChartSettings.Width = 200;
			pieChartSettings.Height = 200;

			Primitives.Container pieChartContainer = new Primitives.Container( new Types.Point( 10, 10 ) );

			Graphs.PieChart pieChart = new Graphs.PieChart();

			pieChartContainer.AddBack( pieChart.Create( pieChartData, pieChartSettings ) );

			style.Apply( _renderer, pieChartContainer );

			_container.AddBack( pieChartContainer );

			//
			// Create bar chart.

			int barGroups = 5;
			int barParts = 3;
			Graphs.StaticData barChartData = new Graphs.StaticData( barGroups, barParts );

			for( int i = 0; i < barGroups; ++i )
			{
			  for( int j = 0; j < barParts; ++j )
			  {
			    barChartData[i, j] = random.NextDouble();
			  }
			}

			Graphs.BarChart.Settings barChartSettings = new Graphs.BarChart.Settings();

			barChartSettings.Width = 200;
			barChartSettings.Height = 200;

			Primitives.Container barChartContainer = new Primitives.Container( new Types.Point( 220, 10 ) );

			Graphs.BarChart barChart = new Graphs.BarChart();

			barChartContainer.AddBack( barChart.Create( barChartData, barChartSettings ) );

			style.Apply( _renderer, barChartContainer );

			_container.AddBack( barChartContainer );

			//
			// Create line chart.

			int lines = 2;
			int columns = 10;
			Graphs.StaticData lineChartData = new Graphs.StaticData( columns, lines + 1 );

			for( int i = 0; i < columns; ++i )
			{
				lineChartData[i, 0] = i;
				lineChartData.SetRowExtra( i, "LABEL", i.ToString() );
			}
			for( int i = 1; i < lines + 1; ++i )
			{
				for( int j = 0; j < columns; ++j )
				{
					lineChartData[j, i] = random.NextDouble() - 0.5;
				}
			}

			Graphs.LineChart.Settings lineChartSettings = new Graphs.LineChart.Settings();

			lineChartSettings.Width = 200;
			lineChartSettings.Height = 200;

			Primitives.Container lineChartContainer = new Primitives.Container( new Types.Point( 440, 10 ) );

			Graphs.LineChart lineChart = new Graphs.LineChart();

			lineChartContainer.AddBack( lineChart.Create( lineChartData, lineChartSettings ) );

			style.Apply( _renderer, lineChartContainer );

			_container.AddBack( lineChartContainer );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			_renderer.Render( e.Graphics, _container, new Types.Rectangle( e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height ) );
		}

		private Renderers.GdiPlusRenderer _renderer;
		private Primitives.Container _container;
	}
}