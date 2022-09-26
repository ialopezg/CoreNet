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
using System.Reflection;

namespace ProgrammersInc.Utility.Serialization.Streaming
{
	#region SelfRegisteringClassAttribute
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	/// <summary>
	/// When a class supports self registration it should place this attribute on the class. 
	/// Note also that a corresponding private static method with the signiture "RegisterTypeHandler"
	/// should be declared.
	/// </summary>
	public class AutoRegisterTypeHandler: Attribute
	{
		public AutoRegisterTypeHandler()
		{
		}
	}
	#endregion
	/// <summary>
	/// Delegate that gets called when reading data into an object from a stream.
	/// </summary>
	/// <param name="reader">Reading stream</param>
	/// <param name="o">Option parameter, If non null then write object data into it, otherwise create it.
	/// </param>
	/// <returns>Needs to return the object of type handled if o is null.</returns>
	public delegate object ReadTypeFromStreamHandler( StreamingReader reader, object o );

	/// <summary>
	/// Writes an object into a stream.
	/// </summary>
	/// <param name="writer">Stream to write to</param>
	/// <param name="o">Object data</param>
	public delegate void WriteTypeToStreamHandler( StreamingWriter writer, object o );

	public class TypeHandler
	{
		/// <summary>
		/// Used as the parameter to <see cref="RegisterTypeHandler"/> where versioning isn't required.
		/// </summary>
		public const short NoVersioning = -1;

		public TypeHandler( Type type, short version, ReadTypeFromStreamHandler readObject, WriteTypeToStreamHandler writeObject )
		{
			if( readObject == null )
				throw new ArgumentNullException( "readObject" );
			if( writeObject == null && version == NoVersioning )
				throw new ArgumentNullException( "writeObject" );

			this.Version = version;
			this.ReadObject = readObject;
			this.WriteObject = writeObject;
			this.Type = type;
		}

		public readonly Type Type;
		public readonly short Version;
		public readonly ReadTypeFromStreamHandler ReadObject;
		public readonly WriteTypeToStreamHandler WriteObject;
	}

	public static class TypeHandlers
	{
		/// <summary>
		/// Registers a serialization type handler for a type.
		/// </summary>
		/// <typeparam name="T">Type thats going to be handled</typeparam>
		/// <param name="version">version number that this serialization process supports. TypeHandler.NoVersioning represents no versioning.</param>
		/// <param name="readObject"></param>
		/// <param name="writeObject"></param>
		public static void RegisterTypeHandler<T>( ReadTypeFromStreamHandler readObject, WriteTypeToStreamHandler writeObject )
		{
			RegisterTypeHandler<T>( TypeHandler.NoVersioning, readObject, writeObject );
		}

		/// <summary>
		/// Registers a serialization type handler for a type.
		/// </summary>
		/// <typeparam name="T">Type thats going to be handled</typeparam>
		/// <param name="version">version number that this serialization process supports. TypeHandler.NoVersioning represents no versioning.</param>
		/// <param name="readObject"></param>
		/// <param name="writeObject"></param>
		public static void RegisterTypeHandler<T>( short version, ReadTypeFromStreamHandler readObject, WriteTypeToStreamHandler writeObject )
		{
			TypeKey key = new TypeKey( version, typeof( T ) );

			if( readObject == null )
				throw new ArgumentNullException( "readObject" );
			if( _typeHandlers.ContainsKey( key ) )
				throw new Exception( string.Format( "'{0}' already has a type handler", key ) );

			TypeHandler typeHandler = new TypeHandler( typeof( T ), version, readObject, writeObject );

			TypeHandler latestVersion;
			if( !_latestVersionTypeHandlers.TryGetValue( key.Type, out latestVersion ) || 
				key.Version == TypeHandler.NoVersioning || 
				key.Version > latestVersion.Version )
			{
				_typeNameHandlers[key.Type.FullName] = typeHandler;
				_latestVersionTypeHandlers[key.Type] = typeHandler;
			}

			_typeHandlers[key] = typeHandler;
		}
		public static bool IsRegistered( Type type )
		{
			return _latestVersionTypeHandlers.ContainsKey( type );
		}
		public static bool TryGetHandler( Type type, out TypeHandler latestVersion )
		{
			return _latestVersionTypeHandlers.TryGetValue( type, out latestVersion );
		}
		public static bool TryGetHandler( string typeName, out TypeHandler latestVersion )
		{
			return _typeNameHandlers.TryGetValue( typeName, out latestVersion );
		}
		public static bool TryGetHandler( short version, Type type, out TypeHandler handler )
		{
			return _typeHandlers.TryGetValue( new TypeKey( version, type ), out handler );
		}
		public static bool IsTypeNullable( Type type )
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( System.Nullable<> );
		}

		public static void ScanForAutoRegisteringTypeHandlers( System.Reflection.Assembly assembly )
		{
			if( _assembliesInspected.ContainsKey( assembly ) )
				return ;

			foreach( Type type in assembly.GetTypes() )
			{
				if( !_latestVersionTypeHandlers.ContainsKey( type ) && 
					type.GetCustomAttributes( typeof( AutoRegisterTypeHandler ), false ).Length == 1 )
				{
					MethodInfo mi = type.GetMethod( "RegisterTypeHandler", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
					if( mi == null )
					{
						throw new SerializationException( string.Format("Class '{0}' has SelfRegisteringClassAttribute declared but no corresponding static method 'RegisterTypeHandler' could be found.", type.FullName ) );
					}
					mi.Invoke(  null, null );
				}
			}
			_assembliesInspected[assembly] = true;
		}

		public static bool NullableHasValue = true;
		public static bool NullableNoValue = false;


		static TypeHandlers()
		{
			_assembliesInspected[typeof( string ).Assembly] = true; // dont bother inspecting any types from mscorlib.

			//
			//	Register stock handlers. Note although we already have specific handlers
			//	for some of these types in their respective streaming readers / writers we 
			//	need them here so that we can make use of the automatic Nullable<T> handling.

			#region List<string>
			RegisterTypeHandler <List<string>>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					List<string> list = o == null ? new List<string>() : (List<string>)o;
					int count = reader.ReadInt();
					list.Capacity = count;
					for( int i = 0; i < count; i++ )
					{
						string vi = reader.Read<string>( SerializationHint.NonNullable );
						list.Add( vi );
					}
					return list;
				},
				delegate( StreamingWriter writer, object o )
				{
					List<string> list = (List<string>)o;
					writer.Write( list.Count );
					foreach( string s in list )
					{
						writer.Write( s );
					}
				}
				);
			#endregion

			#region Byte
			RegisterTypeHandler<byte>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadByte();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write((byte)o);
				}
			);
			#endregion

			#region Bool
			RegisterTypeHandler<bool>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadBool();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (bool)o );
				}
			);
			#endregion

			#region Int16
			RegisterTypeHandler<Int16>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadShort();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (short)o );
				}
			);
			#endregion

			#region Int
			RegisterTypeHandler<int>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadInt();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (int)o );
				}
			);
			#endregion

			#region Long
			RegisterTypeHandler<long>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadLong();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (long)o );
				}
			);
			#endregion

			#region DateTime
			RegisterTypeHandler<DateTime>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadDateTime();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (DateTime)o );
				}
			);
			#endregion

			#region TimeSpan
			RegisterTypeHandler<TimeSpan>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadTimeSpan();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( (TimeSpan)o );
				}
			);
			#endregion

			#region String
			RegisterTypeHandler<string>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					return reader.ReadString();
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.WriteString( (string)o );
				}
			);
			#endregion

			#region Uri
			RegisterTypeHandler<Uri>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					string uriValue = reader.Read<string>();
					return new Uri( uriValue );
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.Write( ((Uri)o).AbsoluteUri );
				}
			);
			#endregion

			#region Guid
			RegisterTypeHandler<Guid>( TypeHandler.NoVersioning,
				delegate( StreamingReader reader, object o )
				{
					System.Diagnostics.Debug.Assert( o == null );
					byte []bytes = reader.ReadBytes( 16 );
					return new Guid( bytes );
				}
				,
				delegate( StreamingWriter writer, object o )
				{
					writer.WriteBytes( ((Guid)o).ToByteArray() );
				}
			);
			#endregion
		}

		/// <summary>
		/// Used as a key to identify a type handler by its type name and version.
		/// </summary>
		private struct TypeKey
		{
			public TypeKey( short version, Type type )
			{
				this.Version = version;
				this.Type = type;
			}
			public override string ToString()
			{
				return string.Format( "{0}, version {1}", Type.FullName, this.Version );
			}
			public override int GetHashCode()
			{
				return Type.GetHashCode();
			}
			public  short Version;
			public Type Type;
		}


		private static Dictionary<string, TypeHandler> _typeNameHandlers = new Dictionary<string, TypeHandler>();
		private static Dictionary<Type, TypeHandler> _latestVersionTypeHandlers = new Dictionary<Type, TypeHandler>();
		private static Dictionary<TypeKey, TypeHandler> _typeHandlers = new Dictionary<TypeKey, TypeHandler>();
		private static Dictionary<System.Reflection.Assembly,bool> _assembliesInspected = new Dictionary<System.Reflection.Assembly,bool>();
	}
}
