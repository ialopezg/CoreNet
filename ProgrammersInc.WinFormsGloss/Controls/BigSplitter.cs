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
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	public class BigSplitter : SplitContainer
	{
		public BigSplitter()
		{
			this.Panel1.SizeChanged += new EventHandler( Panel_SizeChanged );
			this.Panel2.SizeChanged += new EventHandler( Panel_SizeChanged );
		}

		public Drawing.ColorTable ColorTable
		{
			get
			{
				return _colorTable;
			}
			set
			{
				_colorTable = value;

				Invalidate();
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			e.Graphics.Clear( BackColor );

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			Rectangle splitRect = this.SplitterRectangle;
			int size = splitRect.Height / 4;

			if( size > 2 )
			{
				using( Brush brush = new SolidBrush( ColorTable.GrayForegroundColor ) )
				{
					float centerX = splitRect.Left + splitRect.Width / 2;
					float centerY = splitRect.Top + splitRect.Height / 2;

					using( Pen pen = new Pen( brush ) )
					{
						e.Graphics.DrawLine( pen, centerX - 20, centerY - 1, splitRect.Left, centerY - 1 );
						e.Graphics.DrawLine( pen, centerX + 20, centerY - 1, splitRect.Right, centerY - 1 );
						e.Graphics.DrawLine( pen, centerX - 20, centerY + 1, splitRect.Left, centerY + 1 );
						e.Graphics.DrawLine( pen, centerX + 20, centerY + 1, splitRect.Right, centerY + 1 );
					}

					float txsize = 8, tysize = size;

					e.Graphics.FillPolygon( brush, new PointF[] { new PointF( centerX - txsize, centerY - 1 ), new PointF( centerX, centerY - tysize ), new PointF( centerX + txsize, centerY - 1 ) } );
					e.Graphics.FillPolygon( brush, new PointF[] { new PointF( centerX - txsize, centerY + 1 ), new PointF( centerX, centerY + tysize ), new PointF( centerX + txsize, centerY + 1 ) } );
				}
			}
		}

		private void Panel_SizeChanged( object sender, EventArgs e )
		{
			Invalidate( SplitterRectangle );
		}

		private Drawing.ColorTable _colorTable = new Drawing.WindowsThemeColorTable();
	}
}
