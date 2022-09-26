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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	[CLSCompliant( false )]
	public sealed class SuperToolTip : WinFormsUtility.Win32.PerPixelAlphaForm
	{
		public SuperToolTip( Drawing.ColorTable colorTable, SuperToolTipInfo info, Point p, bool balloon )
		{
			_colorTable = colorTable;
			_info = info;
			_balloon = balloon;

			using( Graphics g = CreateGraphics() )
			{
				Size size = GetSize( g, info );

				_width = size.Width;
				_height = size.Height;
			}

			Position( p );

			Setup();
		}

		public static Size GetSize( Graphics g, SuperToolTipInfo info )
		{
			int width, height;

			using( Font font = new Font( SystemFonts.DialogFont, FontStyle.Bold ) )
			{
				width = Math.Max( 200, WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, info.Title, font, int.MaxValue ).Width + 8 );
			}

			height = WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, info.Title, SystemFonts.DialogFont, width - _bodyIndent ).Height;
			height += WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, info.Description, SystemFonts.DialogFont, width - _bodyIndent ).Height;

			width += _border * 2;
			height += _border * 2 + _titleSep;

			return new Size( width, height );
		}

		public SuperToolTipInfo Info
		{
			get
			{
				return _info;
			}
		}

		public new double Opacity
		{
			get
			{
				return _opacity;
			}
			set
			{
				_opacity = value;

				Setup();
			}
		}

		public void Position( Point p )
		{
			Rectangle bounds = new Rectangle( p.X - _xoff, p.Y - _yoff, _width + 20 + _xoff, _height + 20 + _yoff );
			Rectangle screen = SystemInformation.WorkingArea;
			Point mousePos = Control.MousePosition;

			if( !screen.Contains( Control.MousePosition ) )
			{
				screen = SystemInformation.VirtualScreen;
			}

			if( bounds.Left < screen.Left )
			{
				bounds.X = screen.Left + 4;
			}
			if( bounds.Top < screen.Top )
			{
				bounds.Y = screen.Top + 4;
			}
			if( bounds.Right > screen.Right )
			{
				bounds.X = screen.Right - bounds.Width - 4;
			}
			if( bounds.Bottom > screen.Bottom )
			{
				bounds.Y = Math.Min( screen.Bottom, mousePos.Y ) - bounds.Height - 4;
			}

			Bounds = bounds;
		}

		public static void Render( Graphics g, Rectangle clip, SuperToolTipInfo info, int width, int height, bool balloon, Drawing.ColorTable colorTable )
		{
			VectorGraphics.Primitives.Container container;

			Render( g, clip, info, width, height, balloon, colorTable, out container );
		}

		private static void Render( Graphics g, Rectangle clip, SuperToolTipInfo info, int width, int height, bool balloon, Drawing.ColorTable colorTable, out VectorGraphics.Primitives.Container container )
		{
			VectorGraphics.Types.Rectangle clipRect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip );
			VectorGraphics.Renderers.GdiPlusRenderer renderer = new VectorGraphics.Renderers.GdiPlusRenderer
				( delegate
					{
						return g;
					}, VectorGraphics.Renderers.GdiPlusRenderer.MarkerHandling.Ignore, 5 );

			container = CreateContainer( renderer, width, height, balloon, colorTable );

			g.TranslateTransform( _xoff, _yoff );
			renderer.Render( g, container, clipRect );

			int titleHeight = WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, info.Title, SystemFonts.DialogFont, width - _bodyIndent ).Height;
			Rectangle rect = new Rectangle( 0, 0, width, height );
			Rectangle titleRect = new Rectangle( rect.X + _border, rect.Y + _border, rect.Width - _border * 2, titleHeight );
			Rectangle bodyRect = new Rectangle( rect.X + _border + _bodyIndent, rect.Y + titleHeight + _border + _titleSep, rect.Width - _border * 2 - _bodyIndent, rect.Height - titleHeight - _border * 2 - _titleSep );

			using( Font font = new Font( SystemFonts.DialogFont, FontStyle.Bold ) )
			{
				WinFormsUtility.Drawing.GdiPlusEx.DrawString
					( g, info.Title, font, colorTable.TextColor, titleRect
					, WinFormsUtility.Drawing.GdiPlusEx.TextSplitting.MultiLine, WinFormsUtility.Drawing.GdiPlusEx.Ampersands.Display );
			}

			WinFormsUtility.Drawing.GdiPlusEx.DrawString
				( g, info.Description, SystemFonts.DialogFont, colorTable.TextColor
				, bodyRect, WinFormsUtility.Drawing.GdiPlusEx.TextSplitting.MultiLine, WinFormsUtility.Drawing.GdiPlusEx.Ampersands.Display );
		}

		private void Setup()
		{
			BufferedGraphicsContext bgc = BufferedGraphicsManager.Current;

			bgc.MaximumBuffer = new Size( _width - 10, _height - 10 );

			using( BufferedGraphics bg = bgc.Allocate( CreateGraphics(), new Rectangle( _xoff + 7, _yoff + 3, _width - 10, _height - 10 ) ) )
			{
				Graphics g = bg.Graphics;
				VectorGraphics.Primitives.Container container;

				Render( g, new Rectangle( 0, 0, _width + 20, _height + 20 ), _info, _width, _height, _balloon, _colorTable, out container );

				Bitmap bitmapArgb = new Bitmap( _width + 30, _height + 30, PixelFormat.Format32bppArgb );

				using( Graphics ng = Graphics.FromImage( bitmapArgb ) )
				{
					VectorGraphics.Renderers.GdiPlusRenderer nrenderer = new VectorGraphics.Renderers.GdiPlusRenderer
						( delegate
							{
								return ng;
							}, VectorGraphics.Renderers.GdiPlusRenderer.MarkerHandling.Ignore, 5 );

					ng.TranslateTransform( _xoff, _yoff );
					nrenderer.Render( ng, container, new VectorGraphics.Types.Rectangle( 0, 0, _width + 30, _height + 30 ) );

					ng.TranslateTransform( _xoff + 4, _yoff + 4 );
					bg.Render( ng );
				}

				byte opacity = (byte) (_opacity * 255);

				SetBitmap( bitmapArgb, opacity );
			}
		}

		private static VectorGraphics.Primitives.Container CreateContainer( VectorGraphics.Renderers.Renderer renderer, int width, int height, bool balloon, Drawing.ColorTable colorTable )
		{
			VectorGraphics.Paint.Color primaryColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( colorTable.PrimaryColor );
			VectorGraphics.Paint.Color lightener = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( colorTable.GlossyLightenerColor );
			VectorGraphics.Paint.Color borderColor = VectorGraphics.Paint.Color.Combine( primaryColor, lightener, 0.5 );
			VectorGraphics.Paint.Color gradientStartColor = VectorGraphics.Paint.Color.Combine( primaryColor, lightener, 0.05 );
			VectorGraphics.Paint.Color gradientEndColor = VectorGraphics.Paint.Color.Combine( primaryColor, lightener, 0.2 );

			VectorGraphics.Factories.RoundedRectangle roundRectFactory = new VectorGraphics.Factories.RoundedRectangle();
			VectorGraphics.Factories.SoftShadow softShadowFactory = new VectorGraphics.Factories.SoftShadow
				( renderer, new VectorGraphics.Types.Point( 1, 1 ), 3, new VectorGraphics.Paint.Color( 0, 0, 0, 0.3 ) );

			VectorGraphics.Types.Rectangle mainRect = new VectorGraphics.Types.Rectangle( 0, 0, width, height );
			VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

			double radius = 3;
			VectorGraphics.Primitives.Path shape = roundRectFactory.Create( mainRect, radius );

			if( balloon )
			{
				shape = new VectorGraphics.Primitives.Path();

				shape.Add( new VectorGraphics.Primitives.Path.Move( new VectorGraphics.Types.Point( mainRect.X + radius, mainRect.Y ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X + radius * 2, mainRect.Y ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X - radius, mainRect.Y - radius * 7 ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X + radius * 9, mainRect.Y ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X + mainRect.Width - radius, mainRect.Y ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( mainRect.X + mainRect.Width, mainRect.Y + radius ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X + mainRect.Width, mainRect.Y + mainRect.Height - radius ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( mainRect.X + mainRect.Width - radius, mainRect.Y + mainRect.Height ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X + radius, mainRect.Y + mainRect.Height ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( mainRect.X, mainRect.Y + mainRect.Height - radius ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( mainRect.X, mainRect.Y + radius ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( mainRect.X + radius, mainRect.Y ) ) );
				shape.Add( new VectorGraphics.Primitives.Path.Close() );
			}
			else
			{
				shape = roundRectFactory.Create( mainRect, 3 );
			}

			shape.Pen = new VectorGraphics.Paint.Pens.SolidPen( borderColor, 1 );
			shape.Brush = new VectorGraphics.Paint.Brushes.LinearGradientBrush( gradientStartColor, gradientEndColor, mainRect.TopLeft, mainRect.BottomLeft );

			container.AddBack( shape );

			softShadowFactory.Apply( container );

			return container;
		}

		private const int _xoff = 10, _yoff = 20;
		private const int _border = 8;
		private const int _bodyIndent = 10;
		private const int _titleSep = 10;

		private int _width, _height;
		private SuperToolTipInfo _info;
		private Drawing.ColorTable _colorTable = new Drawing.WindowsThemeColorTable();
		private double _opacity = 1;
		private bool _balloon;
	}

	#region SuperToolTipInfo

	public sealed class SuperToolTipInfo
	{
		public SuperToolTipInfo( string title, string description )
		{
			if( title == null )
			{
				throw new ArgumentNullException( "title" );
			}
			if( description == null )
			{
				throw new ArgumentNullException( "description" );
			}

			_title = title;
			_description = description;
		}

		public string Title
		{
			get
			{
				return _title;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
		}

		public override int GetHashCode()
		{
			return _title.GetHashCode() ^ _description.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			SuperToolTipInfo info = obj as SuperToolTipInfo;

			if( info == null )
			{
				return false;
			}

			return Title == info.Title && Description == info.Description;
		}

		private string _title;
		private string _description;
	}

	#endregion
}
