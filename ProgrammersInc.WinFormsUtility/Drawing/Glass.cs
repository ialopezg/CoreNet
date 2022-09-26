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

namespace ProgrammersInc.WinFormsUtility.Drawing
{
	public sealed class Glass
	{
		public Glass()
		{
			if( !IsEnabled() )
			{
				throw new InvalidOperationException();
			}
		}

		public bool Ignore( Control owner )
		{
			Form f = owner.FindForm();

			if( f == null )
			{
				return false;
			}

			return f.WindowState == FormWindowState.Maximized;
		}

		public static bool IsEnabled()
		{
			if( Environment.OSVersion.Version.Major < 6 )
			{
				return false;
			}

			bool isGlassSupported = false;

			Utility.Win32.Dwmapi.DwmIsCompositionEnabled( ref isGlassSupported );

			return isGlassSupported;
		}

		public void ExtendGlassIntoClientArea( Form form, int top, int bottom, int left, int right )
		{
			if( !form.IsHandleCreated )
			{
				return;
			}

			Utility.Win32.Margins margins = new Utility.Win32.Margins();

			margins.Left = left;
			margins.Right = right;
			margins.Top = top;
			margins.Bottom = bottom;

			Utility.Win32.Dwmapi.DwmExtendFrameIntoClientArea( form.Handle, ref margins );
		}
	}
}
