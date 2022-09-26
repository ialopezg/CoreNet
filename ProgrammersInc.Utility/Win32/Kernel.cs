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
	[CLSCompliant( false )]
	public static class Kernel
	{
		[DllImport( "kernel32.dll" )]
		public static extern bool SetProcessWorkingSetSize( IntPtr proc, int min, int max );

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr CreateEvent( IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName );

		[DllImport( "kernel32.dll", SetLastError = true )]
		public static extern IntPtr OpenEvent( uint dwDesiredAccess, bool bInheritHandle, string lpName );

		[DllImport( "kernel32", SetLastError = true )]
		public static extern Int32 WaitForSingleObject( IntPtr handle, Int32 milliseconds );

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr GlobalAlloc( uint uFlags, UIntPtr dwBytes );

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr GlobalFree( IntPtr hMem );

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr GlobalLock( IntPtr hMem );

		[DllImport( "kernel32.dll" )]
		public static extern bool GlobalUnlock( IntPtr hMem );
	}
}
