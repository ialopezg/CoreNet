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

namespace ProgrammersInc.Utility.Serialization.Streaming
{
	/// <summary>
	/// Hints for optimising the serialization process
	/// </summary>
	public enum SerializationHint { 

		/// <summary>
		/// Serialization process will take care of nullability testing.
		/// Note, for nullable types there is an extra byte thats saved out
		/// for nullable flag.
		/// </summary>
		Auto,

		/// <summary>
		/// Indicates that the object will never be null.
		/// </summary>
		NonNullable = 2 
	}; 

	/// <summary>
	/// Helper class to read information from a stream.
	/// </summary>
	public class StreamingReader
	{
		public StreamingReader( Stream stream )
			: this( stream, DefaultIdentityPolicy.Instance )
		{
		}

		public StreamingReader( Stream stream, TypeIdentityPolicy typeIdentityPolicy )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );
			if( !stream.CanRead )
				throw new ArgumentException( "stream must support reading" );
			if( typeIdentityPolicy == null )
			{
				_typeIdentityPolicy = DefaultIdentityPolicy.Instance;
			}
			else
			{
				_typeIdentityPolicy = typeIdentityPolicy;
			}

			_stream = stream;
		}
		public byte ReadByte()
		{
			return (byte)_stream.ReadByte();
		}

		public short ReadShort()
		{
			return BitConverter.ToInt16( this.ReadBytes( sizeof( short ) ), 0 );
		}


		public bool ReadBool()
		{
			return BitConverter.ToBoolean( this.ReadBytes( sizeof( bool ) ), 0 );
		}

		public int ReadInt()
		{
			return BitConverter.ToInt32( this.ReadBytes( sizeof( int ) ), 0 );
		}

		public long ReadLong()
		{
			return BitConverter.ToInt64( this.ReadBytes( sizeof( long ) ), 0 );
		}

		public DateTime ReadDateTime()
		{
			long dtVal = ReadLong();
			return DateTime.FromBinary( dtVal );
		}

		public TimeSpan ReadTimeSpan()
		{
			long ticks = ReadLong();
			return new TimeSpan( ticks );
		}

		/// <summary>
		/// Returns true if in the course of reading serialised objects one or more
		/// had older versioned serialised data compared to the object being
		/// loaded into.
		/// </summary>
		public bool OlderVersionObjectLoaded
		{
			get
			{
				return _olderVersionObjectLoaded;
			}
		}

		public byte[] ReadBytes( int numberOfBytes )
		{
			byte[] buffer = new byte[numberOfBytes];
			if( numberOfBytes == 0 )
				return buffer;

			int bytesRead = 0;
			do
			{
				int bytesJustRead = _stream.Read( buffer, bytesRead, numberOfBytes - bytesRead );
				if( bytesJustRead == 0 )
				{
					throw new Exception( "Invalid stream" );
				}
				bytesRead += bytesJustRead;
			}while( bytesRead != numberOfBytes );
			return buffer;
		}

		public T Read<T>()
		{
			return Read<T>( SerializationHint.Auto );
		}

		public T Read<T>( SerializationHint hint )
		{
			//
			// Get the real type this object.
			Type type = typeof( T );

			if( hint == SerializationHint.Auto )
			{
				bool genericNullable = TypeHandlers.IsTypeNullable( typeof( T ) );
				bool nullable = !type.IsValueType || genericNullable;
				if( nullable )
				{
					if( ReadBool() == TypeHandlers.NullableNoValue )
					{
						return default( T );
					}
					else if( genericNullable )
					{
						type = type.GetGenericArguments()[0];
					}
				}
			}
			TypeHandlers.ScanForAutoRegisteringTypeHandlers( type.Assembly );
			if( type.IsGenericType )
			{
				foreach( Type paramType in type.GetGenericArguments() )
				{
					TypeHandlers.ScanForAutoRegisteringTypeHandlers( paramType.Assembly );
				}
			}

			bool isSealed = type.IsValueType || type.BaseType == typeof( object ) && type.IsSealed;
			if( !isSealed )
			{
				type = _typeIdentityPolicy.ReadIdentity( this );
			}

			TypeHandler handler = GetVersionedTypeHandler( type );

			return (T)handler.ReadObject( this, null );
		}

		public void ReadIntoObjectContents<T>( object o )
		{
			if( o == null )
				throw new ArgumentNullException( "o" );

			Type type = typeof( T );
			GetVersionedTypeHandler( type ).ReadObject( this, o );
		}

		private TypeHandler GetVersionedTypeHandler( Type type )
		{
			TypeHandler handler;
			if( !TypeHandlers.TryGetHandler( type, out handler ) )
			{
				TypeHandlers.ScanForAutoRegisteringTypeHandlers( type.Assembly );
				if( !TypeHandlers.TryGetHandler( type, out handler ) )
				{
					throw new ArgumentException( string.Format( "No type handler has been registered for type '{0}'", type.FullName ) );
				}
			}

			if( handler.Version != TypeHandler.NoVersioning )
			{
				short version = this.ReadShort();
				if( handler.Version != version )
				{
					if( !TypeHandlers.TryGetHandler( version, type, out handler ) )
					{
						throw new ArgumentException( string.Format( "No versioned type handler has been registered for type '{0}'", type.FullName ) );
					}
					_olderVersionObjectLoaded = true;
				}
			}
			return handler;
		}

		internal string ReadString()
		{
			int stringSize = ReadInt();
			byte[] bytes = ReadBytes( stringSize );
			return Encoding.UTF8.GetString( bytes, 0, stringSize );
		}

		private bool _olderVersionObjectLoaded = false;
		private Stream _stream;
		private TypeIdentityPolicy _typeIdentityPolicy;
	}
}
