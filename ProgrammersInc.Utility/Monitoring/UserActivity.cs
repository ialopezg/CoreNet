/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.Utility.Monitoring
{
	/// <summary>
	/// Used to monitor when a user is active or not on the system.
	/// </summary>
	public static class UserActivity
	{

		public static int LastInputTimeMinutes
		{
			get
			{
				return LastInputTime / 60;
			}
		}
		/// <summary>
		/// Returns the time that the system has not been touched (keyboard / mouse).
		/// </summary>
		/// <returns>Number of seconds since the last user input (keyboard/mouse)</returns>
		public static int LastInputTime
		{
			get
			{
				int idleTime = 0;
				LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();

				lastInputInfo.cbSize = Marshal.SizeOf( lastInputInfo );
				lastInputInfo.dwTime = 0;

				int envTicks = Environment.TickCount;

				if( GetLastInputInfo( out lastInputInfo ) )
				{
					int lastInputTick = lastInputInfo.dwTime;

					idleTime = envTicks - lastInputTick;
				}

				return ((idleTime > 0) ? (idleTime / 1000) : idleTime);
			}
		}

		#region Implementation
		[DllImport( "user32.dll" )]
		private static extern bool GetLastInputInfo( out LASTINPUTINFO plii );
	
		[StructLayout( LayoutKind.Sequential )]
		private struct LASTINPUTINFO
		{
			public static readonly int SizeOf = Marshal.SizeOf( typeof( LASTINPUTINFO ) );

			[MarshalAs( UnmanagedType.U4 )]
			public int cbSize;
			[MarshalAs( UnmanagedType.U4 )]
			public int dwTime;
		}

		#endregion
	}
}
