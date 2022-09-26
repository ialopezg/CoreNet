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
using System.Diagnostics;

namespace ProgrammersInc.Utility.Monitoring
{
	public class MouseHook: IDisposable
	{
		public delegate void MouseMoveHandler( System.Drawing.Point pt );

		public MouseHook( MouseMoveHandler handler )
		{
			if( handler == null )
				throw new ArgumentNullException( "handler" );

			lock( _lockObject )
			{
				_handlers.Add( handler );
				if( _handlers.Count == 1 )
				{
					HookUp();
				}
			}
			_handler = handler;
		}

		private static HookProc _hookHandler; // keep delegate around so the garbage collector doesn't clean up.

		private static void HookUp()
		{
			_hookHandler = new HookProc( MouseHookProc );
			_hHook = SetWindowsHookEx( WH_MOUSE_LL, _hookHandler,
																GetModuleHandle( Process.GetCurrentProcess().MainModule.ModuleName ), 0 ); 

			if( _hHook == 0 )
			{
				throw new Exception( "SetWindowsHookEx Failed" );
			}
		}
		private static void UnHook()
		{
			bool ret = UnhookWindowsHookEx( _hHook );
			if( ret == false )
			{
				throw new Exception( "UnhookWindowsHookEx Failed" );
			}
			_hHook = 0;
		}

		private delegate int HookProc( int nCode, IntPtr wParam, IntPtr lParam );

		private static int _hHook = 0;

		private const int WH_MOUSE_LL = 14;
	

		[StructLayout( LayoutKind.Sequential )]
		private class POINT
		{
			public int x;
			public int y;
		}

		//Declare the wrapper managed MouseHookStruct class.
		[StructLayout( LayoutKind.Sequential )]
		private class MouseHookStruct
		{
			public POINT pt;
			public int hwnd;
			public int wHitTestCode;
			public int dwExtraInfo;
		}

		//This is the Import for the SetWindowsHookEx function.
		//Use this function to install a thread-specific hook.
		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
		private static extern int SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hInstance, int threadId );

		//This is the Import for the UnhookWindowsHookEx function.
		//Call this function to uninstall the hook.
		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
		private static extern bool UnhookWindowsHookEx( int idHook );

		//This is the Import for the CallNextHookEx function.
		//Use this function to pass the hook information to the next hook procedure in chain.
		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
		private static extern int CallNextHookEx( int idHook, int nCode, IntPtr wParam, IntPtr lParam );
		[DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		private static extern IntPtr GetModuleHandle( string lpModuleName );
		private static int MouseHookProc( int nCode, IntPtr wParam, IntPtr lParam )
		{	
			//Marshall the data from the callback.
			MouseHookStruct mouseInfo = (MouseHookStruct)Marshal.PtrToStructure( lParam, typeof( MouseHookStruct ) );

			if( nCode < 0 )
			{
				return CallNextHookEx( _hHook, nCode, wParam, lParam );
			}
			else
			{
				System.Drawing.Point pt = new System.Drawing.Point( mouseInfo.pt.x, mouseInfo.pt.y );
				lock( _lockObject )
				{
					foreach( MouseMoveHandler handler in _handlers )
					{
						handler( pt );
					}
				}
				return CallNextHookEx( _hHook, nCode, wParam, lParam );
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			lock( _lockObject )
			{
				_handlers.Remove( _handler );
				if( _handlers.Count == 0 )
				{
					UnHook();
				}
			}
		}

		#endregion
		private MouseMoveHandler _handler;
		private static object _lockObject = new object();
		private static List<MouseMoveHandler> _handlers = new List<MouseMoveHandler>();
	}
}