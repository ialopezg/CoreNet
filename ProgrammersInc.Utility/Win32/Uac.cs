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
using System.Runtime.InteropServices;

namespace ProgrammersInc.Utility.Win32
{
	public static class Uac
	{
		public static bool IsSupported
		{
			get
			{
				return Environment.OSVersion.Version.Major >= 6;
			}
		}

		public class ShieldButton : Button
		{
			public ShieldButton()
			{
				FlatStyle = FlatStyle.System;
			}

			protected override void OnHandleCreated( EventArgs e )
			{
				base.OnHandleCreated( e );

				if( Uac.IsSupported )
				{
					SendMessage( new HandleRef( this, this.Handle ), BCM_SETSHIELD, IntPtr.Zero, new IntPtr( 1 ) );
				}
			}

			private const int BS_COMMANDLINK = 0x0000000E;
			private const uint BCM_SETNOTE = 0x00001609;
			private const uint BCM_SETSHIELD = 0x0000160C;

			[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
			private static extern IntPtr SendMessage( HandleRef hWnd, UInt32 Msg, IntPtr wParam, string lParam );

			[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
			private static extern IntPtr SendMessage( HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam );

			private string _noteText = string.Empty;
		}
	}
}
