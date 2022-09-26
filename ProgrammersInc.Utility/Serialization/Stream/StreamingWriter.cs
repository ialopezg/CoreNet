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
	/// Used in conjunction with StreamingWriter to force writing a particular version of a type.
	/// </summary>
	public struct VersionWriteOverride
	{
		public VersionWriteOverride( Type type, short version )
		{
			this.Type = type;
			this.VersionToWrite = version;
		}

		public Type Type;
		public short VersionToWrite;
	}

	/// <summary>
	/// Helper class to write information to a stream.
	/// </summary>
	public class StreamingWriter
	{
		public StreamingWriter( Stream stream )
			: this( stream, DefaultIdentityPolicy.Instance )
		{
		}

		public StreamingWriter( Stream stream, TypeIdentityPolicy typeIdentityPolicy )
			: this( stream, typeIdentityPolicy, null ) 
		{
		}

		public StreamingWriter( Stream stream, TypeIdentityPolicy typeIdentityPolicy, VersionWriteOverride []versionWriteOverrides )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );
			if( !stream.CanWrite )
				throw new ArgumentException( "stream must support Writing" );
			if( typeIdentityPolicy == null )
			{
				_typeIdentityPolicy = DefaultIdentityPolicy.Instance;
			}
			else
			{
				_typeIdentityPolicy = typeIdentityPolicy;
			}

			if( versionWriteOverrides != null && versionWriteOverrides.Length > 0 )
			{
				_typeVersionOverrides = new Dictionary<Type,short>();
				foreach( VersionWriteOverride vwo in versionWriteOverrides )
				{
					_typeVersionOverrides[vwo.Type] = vwo.VersionToWrite;
				}
			}

			_stream = stream;
		}

		public static byte[]ToBytes<T>( T objectToWrite )
		{
			return ToBytes<T>( objectToWrite, null );
		}
		public static byte[] ToBytes<T>( T objectToWrite, TypeIdentityPolicy typeIdentityPolicy )
		{
			using( MemoryStream ms = new MemoryStream() )
			{
				StreamingWriter sw = new StreamingWriter( ms, typeIdentityPolicy );
				sw.Write( objectToWrite );
				return ms.ToArray();
			}
		}

		public void Write( byte value )
		{
			_stream.WriteByte( value );
		}

		public void Write( bool value )
		{
			WriteBytes( BitConverter.GetBytes( value ) );
		}

		public void Write( short value )
		{
			WriteBytes( BitConverter.GetBytes( value ) );
		}

		public void Write( int value  )
		{
			WriteBytes( BitConverter.GetBytes( value ) );
		}

		public void Write( long value )
		{
			WriteBytes( BitConverter.GetBytes( value ) );
		}

		public void Write( DateTime dt )
		{
			Write( dt.ToBinary() );
		}

		public void Write( TimeSpan span )
		{
			Write( span.Ticks );
		}

		/// <summary>
		/// Write out an objects base class contents
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		public void WriteObjectContents<T>( T value )
		{
			Type type = typeof( T );
			if( type.IsSealed )
				throw new InvalidOperationException( "Cannot write out base class as type cannot be derived from as it is sealed" );
			if( value == null )
				throw new ArgumentNullException( "value" );

			TypeHandlers.ScanForAutoRegisteringTypeHandlers( type.Assembly );

			TypeHandler handler = this.GetTypeHandler( type );

			if( handler.Version != TypeHandler.NoVersioning )
			{
				this.Write( handler.Version );
			}

			handler.WriteObject( this, value );
		}

		public void Write<T>( T value )
		{
			Write<T>( value, SerializationHint.Auto );
		}

		/// <summary>
		/// Write an object out to the stream.
		/// </summary>
		/// <param name="value"></param>
		public void Write<T>( T value, SerializationHint hint )
		{
			Type type = value == null ? typeof( T ) : value.GetType();

			if( hint == SerializationHint.Auto )
			{
				bool nullable = !type.IsValueType || TypeHandlers.IsTypeNullable( typeof( T ) );

				if( nullable )
				{
					if( value == null )
					{
						Write( TypeHandlers.NullableNoValue );
						return;
					}
					Write( TypeHandlers.NullableHasValue );
				}
			}
			
			bool isSealed = type.IsValueType || type.BaseType == typeof( object ) && type.IsSealed;
			if( !isSealed ) 
			{
				_typeIdentityPolicy.WriteIdentity( this, type );
			}

			TypeHandlers.ScanForAutoRegisteringTypeHandlers( type.Assembly );

			TypeHandler handler = this.GetTypeHandler( type );

			if( handler.Version != TypeHandler.NoVersioning )
			{
				this.Write( handler.Version );	
			}

			handler.WriteObject( this, value );
		}


		public void WriteBytes( byte[] bytes )
		{
			if( bytes.Length > 0 )
			{
				_stream.Write( bytes, 0, bytes.Length );
			}
		}

		internal void WriteString( string s )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( s );
			Write( bytes.Length );
			WriteBytes( bytes );
		}

		private TypeHandler GetTypeHandler( Type type )
		{
			TypeHandler handler;
			short version;
			if( _typeVersionOverrides == null || !_typeVersionOverrides.TryGetValue( type, out version ) )
			{
				if( !TypeHandlers.TryGetHandler( type, out handler ) )
					throw new ArgumentException( string.Format( "No type handler has been registered for type '{0}'", type.FullName ) );
			}
			else
			{
				if( !TypeHandlers.TryGetHandler( version, type, out handler ) )
					throw new ArgumentException( string.Format( "No type handler has been registered for type '{0}' and write version {1}", type.FullName, version ) );
			}
			return handler;
		}

		private Stream _stream;
		private TypeIdentityPolicy _typeIdentityPolicy;
		private Dictionary<Type, short> _typeVersionOverrides = null;
	}
}
