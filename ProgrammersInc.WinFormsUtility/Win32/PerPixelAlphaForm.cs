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

namespace ProgrammersInc.WinFormsUtility.Win32
{
	[CLSCompliant(false)]
	public class PerPixelAlphaForm : PopupWindow
	{
		public PerPixelAlphaForm()
		{
			FormBorderStyle = FormBorderStyle.None;
		}

		protected void SetBitmap( Bitmap bitmap )
		{
			SetBitmap( bitmap, 255 );
		}

		protected void SetBitmap( Bitmap bitmap, byte opacity )
		{
			if( !IsHandleCreated )
			{
				return;
			}
			if( bitmap.PixelFormat != PixelFormat.Format32bppArgb )
			{
				throw new ApplicationException( "The bitmap must be 32ppp with alpha-channel." );
			}

			IntPtr screenDc = Utility.Win32.User.GetDC( IntPtr.Zero );
			IntPtr memDc = Utility.Win32.Gdi.CreateCompatibleDC( screenDc );
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr oldBitmap = IntPtr.Zero;

			try
			{
				hBitmap = bitmap.GetHbitmap( Color.FromArgb( 0 ) );  // grab a GDI handle from this GDI+ bitmap
				oldBitmap = Utility.Win32.Gdi.SelectObject( memDc, hBitmap );

				Utility.Win32.Common.SIZE size = new Utility.Win32.Common.SIZE( bitmap.Width, bitmap.Height );
				Utility.Win32.Common.POINT pointSource = new Utility.Win32.Common.POINT( 0, 0 );
				Utility.Win32.Common.POINT topPos = new Utility.Win32.Common.POINT( Left, Top );
				Utility.Win32.BLENDFUNCTION blend = new Utility.Win32.BLENDFUNCTION();

				blend.BlendOp = AC_SRC_OVER;
				blend.BlendFlags = 0;
				blend.SourceConstantAlpha = opacity;
				blend.AlphaFormat = AC_SRC_ALPHA;

				Utility.Win32.User.UpdateLayeredWindow( Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA );
			}
			finally
			{
				Utility.Win32.Gdi.ReleaseDC( IntPtr.Zero, screenDc );

				if( hBitmap != IntPtr.Zero )
				{
					Utility.Win32.Gdi.SelectObject( memDc, oldBitmap );
					Utility.Win32.Gdi.DeleteObject( hBitmap );
				}
				Utility.Win32.Gdi.DeleteDC( memDc );
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				cp.ExStyle |= (int) Utility.Win32.WindowStylesEx.WS_EX_LAYERED;

				return cp;
			}
		}

		private const Int32 ULW_COLORKEY = 0x00000001;
		private const Int32 ULW_ALPHA = 0x00000002;
		private const Int32 ULW_OPAQUE = 0x00000004;

		private const byte AC_SRC_OVER = 0x00;
		private const byte AC_SRC_ALPHA = 0x01;
	}
}
