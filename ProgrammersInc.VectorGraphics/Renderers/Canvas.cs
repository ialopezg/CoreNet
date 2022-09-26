/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.VectorGraphics.Renderers
{
	public partial class Canvas : UserControl
	{
		public Canvas()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw 
				| ControlStyles.UserPaint
				, true );

			_renderer = new Renderers.GdiPlusRenderer( CreateGraphics, Renderers.GdiPlusRenderer.MarkerHandling.Display, 5 );
		}

		public Renderers.GdiPlusRenderer Renderer
		{
			get
			{
				return _renderer;
			}
		}

		public Primitives.Container VectorGraphicsContainer
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;

				Invalidate();
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			if( _container != null )
			{
				_renderer.Render( e.Graphics, _container, new Types.Rectangle( e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height ) );
			}
		}

		private Primitives.Container _container;
		private Renderers.GdiPlusRenderer _renderer;
	}
}
