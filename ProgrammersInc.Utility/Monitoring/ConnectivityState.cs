/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ProgrammersInc.Utility.Monitoring
{
	public static class ConnectivityState
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

		public class ConnectionStateChangedEventArgs: EventArgs
		{
			internal ConnectionStateChangedEventArgs( bool isConnected )
			{
				IsConnected = isConnected;
			}
			public readonly bool IsConnected;
		}

		public delegate void ConnectStateChangedHandler( ConnectionStateChangedEventArgs eventArgs );

		public static event ConnectStateChangedHandler ConnectionStateChanged
		{
			add
			{
				lock( _lockObject )
				{
					_connectionStateChanged += value;
					if( _connectTimer == null )
					{
						_connectTimer = new Timer( new TimerCallback( ConnectionCheckHandler ), null, 0, _timerInterval );
						_lastConnectedState = IsConnected;
					}
				}
			}
			remove
			{
				lock( _lockObject )
				{
					_connectionStateChanged -= value;
					if( ( _connectionStateChanged == null || _connectionStateChanged.GetInvocationList().Length == 0 ) && _connectTimer != null )
					{
						_connectTimer.Dispose();
						_connectTimer = null;
						_lastConnectedState = false;
					}
				}
			}
		}

		private static void ConnectionCheckHandler( object state )
		{
			lock( _lockObject )
			{
				if( _lastConnectedState != IsConnected )
				{
					_connectionStateChanged( new ConnectionStateChangedEventArgs( IsConnected ) );
					_lastConnectedState = IsConnected;
				}
			}
		}

		[DllImport( "sensapi.dll" )]
		private extern static bool IsNetworkAlive( ref int flags );
		[DllImport( "sensapi.dll" )]
		private extern static bool IsDestinationReachable( string dest, IntPtr ptr );

		private static int NETWORK_ALIVE_LAN = 0x00000001;
		private static int NETWORK_ALIVE_WAN = 0x00000002;

		private const int _timerInterval = 5000;

		private static Timer _connectTimer = null;

		private static object _lockObject = new object();
		private static event ConnectStateChangedHandler _connectionStateChanged;
		private static bool _lastConnectedState = false;
	}
}
