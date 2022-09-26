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
using System.Net;
using System.IO;

namespace ProgrammersInc.Utility.Net
{
	public sealed class StaticWebResponse : WebResponse
	{
		public StaticWebResponse( Exception exception )
		{
			if( exception == null )
			{
				throw new ArgumentNullException( "exception" );
			}

			_exception = exception;
		}

		public StaticWebResponse( string data, string contentType )
		{
			if( data == null )
			{
				throw new ArgumentNullException( "data" );
			}
			if( contentType == null )
			{
				throw new ArgumentNullException( "contentType" );
			}

			_contentType = contentType;

			_stream = new MemoryStream( Encoding.UTF8.GetBytes( data ) );
		}

		public override long ContentLength
		{
			get
			{
				if( _exception != null )
				{
					throw _exception;
				}

				return _stream.Length;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override string ContentType
		{
			get
			{
				if( _exception != null )
				{
					throw _exception;
				}

				return _contentType;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override void Close()
		{
		}

		public override Stream GetResponseStream()
		{
			if( _exception != null )
			{
				throw _exception;
			}

			return _stream;
		}

		private Exception _exception;
		private Stream _stream;
		private string _contentType;
	}
}
