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
using ProgrammersInc.Utility.Win32;
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public static class ControlUtils
	{
		/// <summary>
        /// Devuelve <c>true</c> si alguna parte del base del cliente es visible.
		/// </summary>
		/// <param name="control">Control a evaluar.</param>
		/// <param name="rectangleToCheck">Rectángulo cliente a evaluar.</param>
		public static bool IsClientRectangleVisible( Control control, Rectangle rectangleToCheck )
		{
			if( !control.IsHandleCreated )
			{
				return false;
			}

			Utility.Win32.Common.RECT rcClip, rcClient = new Utility.Win32.Common.RECT( rectangleToCheck );

			using( Graphics grfx = control.CreateGraphics() )
			{
				IntPtr hdc = IntPtr.Zero;

				try
				{
					hdc = grfx.GetHdc();

					RegionValue result = (RegionValue) Gdi.GetClipBox( hdc, out rcClip );

					return result != RegionValue.NULLREGION;
				}
				finally
				{
					if( hdc != IntPtr.Zero )
					{
						grfx.ReleaseHdc( hdc );
					}
				}
			}
		}

		public static Point FixForScreen( Rectangle rect )
		{
			Screen buttonScreen = Screen.PrimaryScreen;
			Screen menuScreen = Screen.PrimaryScreen;

			foreach( Screen screen in Screen.AllScreens )
			{
				if( screen.Bounds.Contains( rect ) )
				{
					buttonScreen = screen;
				}
			}

			int x = rect.Left, y = rect.Bottom;

			if( x + rect.Width > buttonScreen.WorkingArea.Right )
			{
				x = buttonScreen.WorkingArea.Right - rect.Width;
			}
			if( y + rect.Height > buttonScreen.WorkingArea.Bottom )
			{
				y = buttonScreen.WorkingArea.Bottom - rect.Height;
			}

			return new Point( x, y );
		}
	}
}
