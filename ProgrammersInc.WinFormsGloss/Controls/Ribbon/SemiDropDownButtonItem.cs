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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class SemiDropDownButtonItem : ButtonItem
	{
		public SemiDropDownButtonItem( string text, Image image16, Image image24 )
			: base( text, image16, image24 )
		{
		}

		public SemiDropDownButtonItem( string text, Icon icon16, Icon icon24 )
			: base( text, icon16, icon24 )
		{
		}

		public ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return _contextMenuStrip;
			}
			set
			{
				_contextMenuStrip = value;
			}
		}

		public override bool NeedsMouseOverUpdate
		{
			get
			{
				return true;
			}
		}

		protected override bool OnClick( Context context )
		{
			Rectangle logicalBounds = context.GetItemBounds( this );
			ButtonSizeKind buttonSizeKind = IdentifyButton( logicalBounds.Size );

			if( buttonSizeKind != ButtonSizeKind.Single )
			{
				Rectangle majorBounds = GetMajorBounds( logicalBounds );
				Rectangle minorBounds = GetMinorBounds( logicalBounds );

				if( minorBounds.Contains( context.RibbonControl.PointToClient( Control.MousePosition ) ) )
				{
					if( _contextMenuStrip != null )
					{
						Screen buttonScreen = Screen.PrimaryScreen;
						Screen menuScreen = Screen.PrimaryScreen;
						Point buttonPosn = this.Section.Ribbon.PointToScreen( logicalBounds.Location );
						Rectangle buttonRect = new Rectangle( buttonPosn, logicalBounds.Size );

						foreach( Screen screen in Screen.AllScreens )
						{
							if( screen.Bounds.Contains( buttonRect ) )
							{
								buttonScreen = screen;
							}
						}

						int x = buttonRect.Left, y = buttonRect.Bottom;

						if( x + _contextMenuStrip.Bounds.Width > buttonScreen.WorkingArea.Right )
						{
							x = buttonScreen.WorkingArea.Right - _contextMenuStrip.Bounds.Width;
						}
						if( y + _contextMenuStrip.Bounds.Height > buttonScreen.WorkingArea.Bottom )
						{
							y = buttonScreen.WorkingArea.Bottom - _contextMenuStrip.Bounds.Height;
						}

						Point display = context.RibbonControl.PointToClient( new Point( x, y ) );

						_contextMenuStrip.Show( context.RibbonControl, display );

						return true;
					}
				}
			}

			return base.OnClick( context );
		}

		protected override void PaintImage( Context context, Rectangle logicalBounds, Image image, Rectangle imageRect, bool enabled )
		{
			ButtonSizeKind buttonSizeKind = IdentifyButton( logicalBounds.Size );

			if( buttonSizeKind == ButtonSizeKind.Big )
			{
				double glow = context.Renderer.GetFade( context, this );

				if( enabled )
				{
					int xoff = (int) (glow * 4);

					imageRect.Offset( -xoff, 0 );
				}
			}

			base.PaintImage( context, logicalBounds, image, imageRect, enabled );
		}

		protected override void PaintText( Context context, Rectangle logicalBounds, string text, Font font, Brush brush, RectangleF textRect, StringFormat sf, bool enabled )
		{
			ButtonSizeKind buttonSizeKind = IdentifyButton( logicalBounds.Size );

			if( buttonSizeKind == ButtonSizeKind.Big )
			{
				double glow = context.Renderer.GetFade( context, this );

				if( enabled )
				{
					float xoff = (float) (glow * 4);

					textRect.Offset( -xoff, 0 );
				}
			}

			base.PaintText( context, logicalBounds, text, font, brush, textRect, sf, enabled );
		}

		protected override void PaintBackground( Context context, Rectangle clip, Rectangle logicalBounds, Renderer.BackgroundStyle backgroundStyle )
		{
			ButtonSizeKind buttonSizeKind = IdentifyButton( logicalBounds.Size );

			if( buttonSizeKind != ButtonSizeKind.Single )
			{
				Rectangle majorBounds = GetMajorBounds( logicalBounds );
				Rectangle minorBounds = GetMinorBounds( logicalBounds );

				context.Renderer.PaintItemBackground( context, clip, minorBounds, this, backgroundStyle );

				Renderer.BackgroundStyle majorBackgroundStyle = backgroundStyle;

				if( minorBounds.Contains( context.RibbonControl.PointToClient( Control.MousePosition ) ) )
				{
					if( majorBackgroundStyle == Renderer.BackgroundStyle.Pressed )
					{
						majorBackgroundStyle = Renderer.BackgroundStyle.Normal;
					}
				}

				context.Renderer.PaintItemBackground( context, clip, majorBounds, this, majorBackgroundStyle );

				if( backgroundStyle != Renderer.BackgroundStyle.Disabled )
				{
					Rectangle arrowRect;
					Drawing.ColorTable colorTable = context.RibbonControl.ColorTable;

					switch( buttonSizeKind )
					{
						case ButtonSizeKind.Single:
						case ButtonSizeKind.Wide:
							arrowRect = new Rectangle( logicalBounds.Right - TriangleSize - 1, logicalBounds.Y + (logicalBounds.Height - TriangleSize) / 2 + 3, TriangleSize, TriangleSize / 2 );
							break;
						case ButtonSizeKind.Big:
							arrowRect = new Rectangle( logicalBounds.Right - TriangleSize - 1, logicalBounds.Top + logicalBounds.Height / 2, TriangleSize, TriangleSize / 2 );
							break;
						default:
							throw new InvalidOperationException();
					}

					using( Brush brush = new SolidBrush( colorTable.TextColor ) )
					{
						context.Graphics.FillPolygon( brush, new Point[]
							{ 
								new Point( arrowRect.Right, arrowRect.Top ),
								new Point( arrowRect.Left, arrowRect.Top ),
								new Point( arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom )
							} );
					}
				}
			}
			else
			{
				base.PaintBackground( context, clip, logicalBounds, backgroundStyle );
			}
		}

		private Rectangle GetMajorBounds( Rectangle logicalBounds )
		{
			return new Rectangle( logicalBounds.X, logicalBounds.Y, logicalBounds.Width - 10, logicalBounds.Height );
		}

		private Rectangle GetMinorBounds( Rectangle logicalBounds )
		{
			return new Rectangle( logicalBounds.Right - 10, logicalBounds.Y, 10, logicalBounds.Height );
		}

		protected const int TriangleSize = 8;

		private ContextMenuStrip _contextMenuStrip;
	}
}
