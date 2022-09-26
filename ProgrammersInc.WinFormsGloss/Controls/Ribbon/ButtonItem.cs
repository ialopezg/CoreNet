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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ButtonItem : Item
	{
		public ButtonItem( string text, Image image16, Image image24 )
		{
			if( text == null || text == string.Empty )
			{
				if( image16 == null || image24 == null )
				{
					throw new ArgumentException( "Text and icon cannot both be empty.", "text" );
				}
			}

			_text = text;
			_tooltipTitle = _text;
			_image16 = image16;
			_image24 = image24;
		}

		public ButtonItem( string text, Icon icon16, Icon icon24 )
			: this( text, icon16 == null ? null : icon16.ToBitmap(), icon24 == null ? null : icon24.ToBitmap() )
		{
		}

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if( _enabled == value )
				{
					return;
				}

				_enabled = value;

				if( Section != null )
				{
					Section.NotifyItemChanged( this );
				}
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
		}

		public Image Image16
		{
			get
			{
				return _image16;
			}
		}

		public string TooltipTitle
		{
			get
			{
				return _tooltipTitle;
			}
			set
			{
				_tooltipTitle = value;
			}
		}

		public string TooltipDescription
		{
			get
			{
				return _tooltipDescription;
			}
			set
			{
				_tooltipDescription = value;
			}
		}

		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return suggestedSize;
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			if( logicalBounds == Rectangle.Empty )
			{
				return;
			}
			
			EnsureDisabledIcons( context.RibbonControl.ColorTable );

			Renderer.BackgroundStyle backgroundStyle = Renderer.BackgroundStyle.Normal;

			if( !Enabled )
			{
				backgroundStyle |= Renderer.BackgroundStyle.Disabled;
			}
			else if( context.History.MouseOverItem == this )
			{
				if( logicalBounds.Contains( context.RibbonControl.PointToClient( Control.MousePosition ) ) )
				{
					if( Control.MouseButtons == MouseButtons.Left )
					{
						backgroundStyle |= Renderer.BackgroundStyle.Pressed;
					}
				}
			}

			RectangleF textRect;
			Rectangle imageRect;
			StringAlignment lineAlignment = StringAlignment.Far, alignment = StringAlignment.Center;
			Image image = null, imageDisabled = null;
			ButtonSizeKind buttonSizeKind = IdentifyButton( logicalBounds.Size );

			PaintBackground( context, clip, logicalBounds, backgroundStyle );

			switch( buttonSizeKind )
			{
				case ButtonSizeKind.Single:
					{
						imageRect = new Rectangle( logicalBounds.X + 4, logicalBounds.Y + (logicalBounds.Height - 16) / 2 + 1, 16, 16 );
						textRect = RectangleF.Empty;
						image = _image16;
						imageDisabled = _imageDisabled16;
						break;
					}
				case ButtonSizeKind.Wide:
					{
						if( _image16 == null )
						{
							imageRect = Rectangle.Empty;
							textRect = new RectangleF( logicalBounds.X + 4, logicalBounds.Y + 1, logicalBounds.Width - 6, logicalBounds.Height - 1 );
						}
						else
						{
							imageRect = new Rectangle( logicalBounds.X + 3, logicalBounds.Y + (logicalBounds.Height - 16) / 2 + 1, 16, 16 );
							textRect = new RectangleF( logicalBounds.X + 21, logicalBounds.Y + 1, logicalBounds.Width - 18, logicalBounds.Height - 1 );
						}
						alignment = StringAlignment.Near;
						lineAlignment = StringAlignment.Center;
						image = _image16;
						imageDisabled = _imageDisabled16;
						break;
					}
				case ButtonSizeKind.Big:
					{
						imageRect = new Rectangle( logicalBounds.X + (logicalBounds.Width - 24) / 2, logicalBounds.Y + 3, 24, 24 );
						textRect = new RectangleF( logicalBounds.X + 1, logicalBounds.Y + 13, logicalBounds.Width - 2, logicalBounds.Height - 14 );
						image = _image24;
						imageDisabled = _imageDisabled24;
						break;
					}
				default:
					throw new InvalidOperationException();
			}

			int xpos = logicalBounds.X + Border;

			if( image != null )
			{
				context.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

				if( Enabled )
				{
					PaintImage( context, logicalBounds, image, imageRect, true );
				}
				else
				{
					PaintImage( context, logicalBounds, imageDisabled, imageRect, false );
				}
			}

			if( _text != null && _text != string.Empty && textRect != RectangleF.Empty )
			{
				Drawing.ColorTable colorTable = context.RibbonControl.ColorTable;
				double glow = context.Renderer.GetFade( context, this );

				Color color;

				if( Enabled )
				{
					color = WinFormsUtility.Drawing.ColorUtil.Combine( colorTable.GlowTextColor, colorTable.TextColor, glow );
				}
				else
				{
					color = colorTable.GrayTextColor;
				}

				using( Brush brush = new SolidBrush( color ) )
				using( StringFormat sf = new StringFormat( StringFormat.GenericTypographic ) )
				{
					sf.Alignment = alignment;
					sf.LineAlignment = lineAlignment;

					using( Font font = CreateFont() )
					{
						PaintText( context, logicalBounds, _text, font, brush, textRect, sf, Enabled );
					}
				}
			}
		}

		public override SuperToolTipInfo GetTooltipInfo()
		{
			return new SuperToolTipInfo( _tooltipTitle, _tooltipDescription );
		}

		protected virtual void PaintText( Context context, Rectangle logicalBounds, string text, Font font, Brush brush, RectangleF textRect, StringFormat sf, bool enabled )
		{
			context.Graphics.DrawString( _text, font, brush, textRect, sf );
		}

		protected virtual void PaintImage( Context context, Rectangle logicalBounds, Image image, Rectangle imageRect, bool enabled )
		{
			context.Graphics.DrawImage( image, imageRect );
		}

		protected virtual void PaintBackground( Context context, Rectangle clip, Rectangle logicalBounds, Renderer.BackgroundStyle backgroundStyle )
		{
			context.Renderer.PaintItemBackground( context, clip, logicalBounds, this, backgroundStyle );
		}

		protected virtual ButtonSizeKind IdentifyButton( Size logicalSize )
		{
			if( logicalSize.Width > CutOff && logicalSize.Height > CutOff )
			{
				return ButtonSizeKind.Big;
			}
			else if( logicalSize.Width > CutOff )
			{
				return ButtonSizeKind.Wide;
			}
			else
			{
				return ButtonSizeKind.Single;
			}
		}

		protected enum ButtonSizeKind
		{
			Single,
			Wide,
			Big
		}

		private Font CreateFont()
		{
			return new Font( SystemFonts.DialogFont.FontFamily, SystemFonts.DialogFont.Size - 1 );
		}

		private void EnsureDisabledIcons( Drawing.ColorTable colorTable )
		{
			Color color = WinFormsUtility.Drawing.ColorUtil.Combine( colorTable.GrayTextColor, colorTable.PrimaryColor, 0.2 );

			if( _greyColor == color )
			{
				return;
			}

			if( _image16 != null )
			{
				_imageDisabled16 = WinFormsUtility.Drawing.GdiPlusEx.MakeDisabledImage( _image16, color );
			}

			if( _image24 != null )
			{
				_imageDisabled24 = WinFormsUtility.Drawing.GdiPlusEx.MakeDisabledImage( _image24, color );
			}

			_greyColor = color;
		}

		protected const int Border = 3;
		protected const int IconTextSeparation = 3;
		protected const int CutOff = 33;

		private string _text;
		private Image _image16, _image24, _imageDisabled16, _imageDisabled24;
		private bool _enabled = true;
		private string _tooltipTitle, _tooltipDescription = string.Empty;
		private Color _greyColor = Color.Red;
	}
}
