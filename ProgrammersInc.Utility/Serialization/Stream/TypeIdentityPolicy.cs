/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.Utility.Serialization.Streaming
{
	/// <summary>
	/// Used to by the streaming classes to write out type information so objects can
	/// be instantiated when read in from stream.
	/// </summary>
	public abstract class TypeIdentityPolicy
	{
		public abstract void WriteIdentity( StreamingWriter writer, Type type );
		public abstract Type ReadIdentity( StreamingReader reader );
	}

	/// <summary>
	/// Generic identity policy allowing different type to be used for saving out identity.
	/// </summary>
	/// <typeparam name="T">Identity type</typeparam>
	public class MappedIdentityPolicy<T>: TypeIdentityPolicy
	{
		public MappedIdentityPolicy()
		{
			System.Diagnostics.Debug.Assert( TypeHandlers.IsRegistered( typeof( T ) ), 
				string.Format( "You will need to register type '{0}' before using it as an identity", typeof(T).FullName  ) );
		}
		/// <summary>
		/// Identity value mapping to Type used for Register method.
		/// </summary>
		public struct IdentityMapping
		{
			public IdentityMapping( T identity, Type type )
			{
				Identity = identity;
				Type = type;
			}
			public T Identity;
			public Type Type;
		}

		/// <summary>
		/// Registers a mapping between an identity value and a Type.
		/// </summary>
		/// <param name="mappedItems"></param>
		public void Register( params IdentityMapping[] mappedItems )
		{
			foreach( IdentityMapping item in mappedItems )
			{
				System.Diagnostics.Debug.Assert( !_typeToIdentity.ContainsKey( item.Type ), string.Format( "Type '{0}' is already mapped", item.Type.FullName ) );
				System.Diagnostics.Debug.Assert( !_identityToType.ContainsKey( item.Identity ), string.Format( "Identity '{0}' is already being used", item.Identity ) );

				_typeToIdentity[item.Type] = item.Identity;
				_identityToType[item.Identity] = item.Type;
			}
		}
		public override void WriteIdentity( StreamingWriter writer, Type type )
		{
			System.Diagnostics.Debug.Assert( _typeToIdentity.ContainsKey( type ) ); // Make sure you registered an identity mapping for this type...
			writer.Write<T>( _typeToIdentity[type] );
		}

		public override Type ReadIdentity( StreamingReader reader )
		{
			T identity = reader.Read<T>();
			System.Diagnostics.Debug.Assert( _identityToType.ContainsKey( identity ) ); // Make sure you registered an identity mapping for this type...
			return _identityToType[identity];
		}
		private Dictionary<Type, T> _typeToIdentity = new Dictionary<Type, T>();
		private Dictionary<T, Type> _identityToType = new Dictionary<T, Type>();
	}


	/// <summary>
	/// Default policy for writing out identities is their full names (assembly name included)
	/// </summary>
	internal sealed class DefaultIdentityPolicy: TypeIdentityPolicy
	{
		public static DefaultIdentityPolicy Instance = new DefaultIdentityPolicy();

		public override void WriteIdentity( StreamingWriter writer, Type type )
		{
			writer.WriteString( type.FullName );
		}

		public override Type ReadIdentity( StreamingReader reader  )
		{
			string typeName = reader.ReadString();

			TypeHandler th;
			if( !TypeHandlers.TryGetHandler( typeName, out th ) )
			{
				throw new Exception( string.Format( "Couldn't not find type handler for '{0}'", typeName ) );
			}
			
			return th.Type;
		}

		private DefaultIdentityPolicy()
		{
		}
	}
}
