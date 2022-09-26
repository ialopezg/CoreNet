/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.Utility.Monitoring
{
	/// <summary>
	/// Logs messages to a specified file
	/// </summary>
	public static class Log
	{
		public static string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				_fileName = value;
			}
		}

		public static bool IsEnabled
		{
			get
			{
				return _fileName != null;
			}
		}
		
		public static bool SendOutputToConsole
		{
			get
			{
				return _sendOutputToConsole;
			}
			set
			{
				_sendOutputToConsole = value;
			}
		}
		public static void WriteLine( string text )
		{
			if( _fileName != null )
			{
				if( _sendOutputToConsole )
				{
					System.Console.WriteLine( text );
				}

				int retryCount = 0;
				while( true )
				{
					try
					{
						using( FileStream stream = new FileStream( _fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None ) )
						{
							stream.Seek( 0, SeekOrigin.End );
							using( StreamWriter sw = new StreamWriter( stream ) )
							{
								sw.WriteLine( text );
								break;
							}
						}
					}
					catch( Exception )
					{
						System.Threading.Thread.Sleep( _random.Next( 1000 ) );
						if( ++retryCount > 5 )
						{
							throw;
						}
					}
				}
			}
		}

		public static void WriteLine( object o )
		{
			WriteLine( o.ToString() );
		}

		public static void WriteLine( string format, params object[] objs )
		{
			string text = string.Format( format, objs );
			WriteLine( text );
		}

		private static Random _random = new Random();
		private static bool _sendOutputToConsole = true;
		private static string _fileName = null;

	}
}
