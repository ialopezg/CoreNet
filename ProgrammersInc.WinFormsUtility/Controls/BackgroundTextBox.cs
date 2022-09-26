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

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public partial class BackgroundTextBox : TextBox
	{
		public BackgroundTextBox()
		{
			InitializeComponent();
			
			SetStyle( ControlStyles.UserPaint, true );
		}

		public string BackgroundText
		{
			get
			{
				return _backgroundText;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_backgroundText = value;
			}
		}

		public Icon BackgroundIcon
		{
			get
			{
				return _backgroundIcon;
			}
			set
			{
				_backgroundIcon = value;
			}
		}

		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );

			UpdateVisual();
		}

		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			UpdateVisual();
		}

		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );

			UpdateVisual();
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( !Focused )
			{
				using( Font font = new Font( Font, FontStyle.Italic ) )
				{
					Rectangle textRect = ClientRectangle;

					textRect.Y += 1;
					textRect.Height -= 1;

					if( _backgroundIcon != null )
					{
						Rectangle iconRect = new Rectangle( textRect.X, textRect.Y, 16, 16 );

						e.Graphics.DrawIconUnstretched( _backgroundIcon, iconRect );

						textRect.X += 16;
						textRect.Width -= 16;
					}

					e.Graphics.DrawString( _backgroundText, font, SystemBrushes.ControlDark, textRect );
				}
			}
		}

		private void UpdateVisual()
		{
			if( _updating.IsActive )
			{
				return;
			}

			using( _updating.Apply() )
			{
				bool bg = (Text == string.Empty && !Focused);

				if( _withBackground != bg )
				{
					_withBackground = bg;

					if( bg )
					{
						SetStyle( ControlStyles.UserPaint, true );
					}
					else
					{
						SetStyle( ControlStyles.UserPaint, false );
						Font = Control.DefaultFont;
						Font = SystemFonts.DialogFont;
					}

					Invalidate();
				}
			}
		}

		private string _backgroundText = string.Empty;
		private Icon _backgroundIcon;
		private Utility.Control.Flag _updating = new Utility.Control.Flag();
		private bool _withBackground = true;
	}
}
