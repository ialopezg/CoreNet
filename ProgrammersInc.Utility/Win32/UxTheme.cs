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
	public static class UxTheme
	{
		[DllImport( "uxtheme.dll" )]
		public static extern int SetWindowTheme( IntPtr hwnd, string pszSubAppName, string pszSubIdList );
	}
}
