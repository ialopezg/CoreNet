using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ProgrammersInc.Utility.Connectivity
{
	public class ConnectivityState
	{
		public static bool IsConnected
		{
			get
			{
				return IsLanAlive || IsWanAlive;
			}
		}
		public static bool IsLanAlive
		{
			get
			{
				return IsNetworkAlive( ref NETWORK_ALIVE_LAN );
			}
		}
		public static bool IsWanAlive
		{
			get
			{
				return IsNetworkAlive( ref NETWORK_ALIVE_WAN );
			}
		}
		public static bool IsDestinationAlive( string Destination )
		{
			return (IsDestinationReachable( Destination, IntPtr.Zero ));
		}
		[DllImport( "sensapi.dll" )]
		private extern static bool IsNetworkAlive( ref int flags );
		[DllImport( "sensapi.dll" )]

		private extern static bool IsDestinationReachable( string dest, IntPtr ptr );
		private static int NETWORK_ALIVE_LAN = 0x00000001;
		private static int NETWORK_ALIVE_WAN = 0x00000002;
	}
}
