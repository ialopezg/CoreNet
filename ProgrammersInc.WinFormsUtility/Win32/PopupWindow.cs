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

namespace ProgrammersInc.WinFormsUtility.Win32
{
	[CLSCompliant( false )]
	public class PopupWindow : Form
	{
		protected PopupWindow()
		{
			InitializeComponent();
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = new CreateParams();

				createParams.X = Location.X;
				createParams.Y = Location.Y;
				createParams.Height = Size.Height;
				createParams.Width = Size.Width;

				createParams.ClassStyle = unchecked( (int) ClassStyle );
				createParams.Parent = IntPtr.Zero;
				createParams.Style = unchecked( (int) WindowStyles );
				createParams.ExStyle = unchecked( (int) WindowStylesEx );

				return createParams;
			}
		}

		protected virtual Utility.Win32.ClassStyle ClassStyle
		{
			get
			{
				return 0;
			}
		}

		protected virtual Utility.Win32.WindowStyles WindowStyles
		{
			get
			{
				return Utility.Win32.WindowStyles.WS_POPUP;
			}
		}

		protected virtual Utility.Win32.WindowStylesEx WindowStylesEx
		{
			get
			{
				return
						Utility.Win32.WindowStylesEx.WS_EX_TOOLWINDOW |
						Utility.Win32.WindowStylesEx.WS_EX_NOACTIVATE;
			}
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			if( Visible )
			{
				IntPtr HWND_TOPMOST = new IntPtr( -1 );

				Utility.Win32.User.SetWindowPos
					( Handle, HWND_TOPMOST, 0, 0, 0, 0
					, Utility.Win32.SetWindowPosOptions.SWP_NOSIZE | Utility.Win32.SetWindowPosOptions.SWP_NOMOVE
					| Utility.Win32.SetWindowPosOptions.SWP_NOACTIVATE | Utility.Win32.SetWindowPosOptions.SWP_NOREDRAW );
			}
		}

		protected override bool ShowWithoutActivation
		{
			get
			{
				return true;
			}
		}

		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case (int) Utility.Win32.Messages.WM_LBUTTONDOWN:
					OnClick( EventArgs.Empty );
					break;
			}

			if( !this.IsDisposed )
			{
				base.WndProc( ref m );
			}
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// PopupWindow
			// 
			this.ClientSize = new System.Drawing.Size( 292, 271 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "PopupWindow";
			this.ShowInTaskbar = false;
			this.ResumeLayout( false );

		}
	}
}
