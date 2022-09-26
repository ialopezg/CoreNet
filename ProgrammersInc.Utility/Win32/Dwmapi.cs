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
using System.Runtime.InteropServices;

namespace ProgrammersInc.Utility.Win32
{
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public struct DWM_BLURBEHIND
	{
		public int dwFlags;
		public bool fEnable;
		public System.IntPtr hRgnBlur;
		public bool fTransitionOnMaximized;
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct Margins
	{
		public int Left, Right, Top, Bottom;
	}

	public sealed class Dwmapi
	{
		[DllImport( "dwmapi.dll" )]
		public static extern void DwmIsCompositionEnabled( ref bool pfEnabled );

		[DllImport( "dwmapi.dll" )]
		public static extern void DwmExtendFrameIntoClientArea( IntPtr hWnd, ref Margins pMargins );

		[DllImport( "dwmapi" )]
		public static extern int DwmEnableBlurBehindWindow( IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind );
	}
}
