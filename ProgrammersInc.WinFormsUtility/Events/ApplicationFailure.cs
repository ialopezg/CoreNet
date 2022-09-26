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
using System.Xml;
using System.IO;

namespace ProgrammersInc.WinFormsUtility.Events
{
	public static class ApplicationFailure
	{
		public interface IReportContext
		{
			void OnException( Exception e );
			string GenerateFilename();
			string[] GetFilenames();
		}

		public delegate void Send( string report );

		public static void Hook( IReportContext context )
		{
			if( context == null )
			{
				throw new ArgumentNullException( "context" );
			}

			_context = context;

			System.Windows.Forms.Application.ThreadException +=
					new System.Threading.ThreadExceptionEventHandler( Application_ThreadException );
			AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
		}

		public static void LogFailure( Exception e, string extra )
		{
			if( e == null )
			{
				try
				{
					throw new InvalidOperationException();
				}
				catch( Exception fake )
				{
					e = fake;
				}
			}

			ProcessException( e, extra, false );
		}

		public static bool HaveUnsentReports
		{
			get
			{
				return _context != null && _context.GetFilenames().Length > 0;
			}
		}

		private static void ProcessException( Exception e, string extra, bool abort )
		{
			string filename = _context.GenerateFilename();

			using( XmlTextWriter tw = new XmlTextWriter( filename, Encoding.UTF8 ) )
			{
				CreateErrorReport( tw, e, extra );
			}

			if( abort )
			{
				_context.OnException( e );

				System.Windows.Forms.Application.Exit();
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}

		private static void Application_ThreadException( object sender, System.Threading.ThreadExceptionEventArgs e )
		{
			try
			{
				System.Windows.Forms.Application.ThreadException -=
						new System.Threading.ThreadExceptionEventHandler( Application_ThreadException );
			}
			catch
			{
			}

			ProcessException( e.Exception, null, true );
		}

		private static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
		{
			try
			{
				AppDomain.CurrentDomain.UnhandledException -=
						new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
			}
			catch
			{
			}

			ProcessException( (Exception) e.ExceptionObject, null, true );
		}

		private static void CreateErrorReport( XmlTextWriter tw, Exception exception, string extra )
		{
			string callstack = Callstack( exception );
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] callstackBytes = new System.Text.UnicodeEncoding().GetBytes( callstack );
			byte[] callstackMD5Bytes = md5.ComputeHash( callstackBytes );
			string callstackMD5 = DumpBytes( callstackMD5Bytes );

			tw.Formatting = Formatting.Indented;

			tw.WriteStartDocument();
			tw.WriteStartElement( "CrashReport" );

			tw.WriteStartElement( "Product" );
			tw.WriteString( Application.ProductName );
			tw.WriteEndElement();

			tw.WriteStartElement( "Version" );
			tw.WriteString( Application.ProductVersion );
			tw.WriteEndElement();

			tw.WriteStartElement( "StackHash" );
			tw.WriteString( callstackMD5 );
			tw.WriteEndElement();

			tw.WriteStartElement( "Time" );
			tw.WriteString( DateTime.UtcNow.ToString( System.Globalization.CultureInfo.InvariantCulture ) );
			tw.WriteEndElement();

			tw.WriteStartElement( "Stack" );
			tw.WriteString( callstack );
			tw.WriteEndElement();

			if( !string.IsNullOrEmpty( extra ) )
			{
				tw.WriteStartElement( "Extra" );
				tw.WriteString( extra );
				tw.WriteEndElement();
			}

			tw.WriteEndElement();
		}

		private static string DumpBytes( byte[] bytes )
		{
			StringBuilder sb = new StringBuilder();

			foreach( byte b in bytes )
			{
				int low = b & 0xf;
				int high = (b & 0xf0) >> 4;

				sb.Append( "0123456789abcdef"[low] );
				sb.Append( "0123456789abcdef"[high] );
			}

			return sb.ToString();
		}

		private static string Callstack( Exception exception )
		{
			StringBuilder sb = new StringBuilder();

			while( exception != null )
			{
				sb.Append( exception.ToString() );
				sb.Append( Environment.NewLine );

				exception = exception.InnerException;
			}

			sb.Append( Environment.NewLine + Environment.NewLine );

			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();

			sb.Append( st.ToString() );
			sb.Append( Environment.NewLine );

			return sb.ToString();
		}

		private static IReportContext _context;
	}
}
