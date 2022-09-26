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

namespace ProgrammersInc.Utility.Serialization
{
	public class SerializationException : Exception
	{
		public SerializationException( string message )
			: base( message )
		{
		}
		public SerializationException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public class DeserializationException : SerializationException
	{
		public DeserializationException( string message, Exception innerException, int lineNumber, int linePosition )
			: base( message, innerException )
		{
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}
		public DeserializationException( string message, int lineNumber, int linePosition )
			: this( message, null, lineNumber, linePosition )
		{
		}

		public readonly int LineNumber;
		public readonly int LinePosition;
	}
}
