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
using Microsoft.Win32;

namespace ProgrammersInc.Utility.Registry
{
	[CLSCompliant( false )]
	public abstract class RegistrySet
	{
		protected RegistrySet()
		{
		}

		protected void AddValue( RegistryValue value )
		{
			_registryValues.Add( value );
		}

		public bool IsCorrect()
		{
			foreach( RegistryValue rv in _registryValues )
			{
				if( !rv.IsCorrect() )
				{
					return false;
				}
			}

			return true;
		}

		public bool TryApply( out string result )
		{
			try
			{
				foreach( RegistryValue rv in _registryValues )
				{
					rv.Apply();
				}

				result = "Success";
				return true;
			}
			catch( Exception e )
			{
				result = e.Message;
				return false;
			}
		}

		private List<RegistryValue> _registryValues = new List<RegistryValue>();
	}

	#region RegistryValue

	[CLSCompliant( false )]
	public sealed class RegistryValue
	{
		public RegistryValue( string path, string key, string value )
			: this( path, key, RegistryValueKind.String )
		{
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}

			_stringValue = value;
		}

		public RegistryValue( string path, string key, uint value )
			: this( path, key, RegistryValueKind.DWord )
		{
			_wordValue = value;
		}

		public RegistryKey TryGetRegistryKey()
		{
			RegistryKey rk = GetRoot();

			for( int i = 1; i < _path.Length; ++i )
			{
				rk = rk.OpenSubKey( _path[i], false );

				if( rk == null )
				{
					return null;
				}
			}

			return rk;
		}

		public void Apply()
		{
			RegistryKey rk = GetRoot();

			for( int i = 1; i < _path.Length; ++i )
			{
				RegistryKey newRk = rk.OpenSubKey( _path[i], true );

				if( newRk == null )
				{
					newRk = rk.CreateSubKey( _path[i] );
				}

				rk = newRk;
			}

			switch( _valueType )
			{
				case RegistryValueKind.DWord:
					rk.SetValue( _key, _wordValue );
					break;
				case RegistryValueKind.String:
					rk.SetValue( _key, _stringValue );
					break;
				default:
					break;
			}
		}

		public bool IsCorrect()
		{
			RegistryKey rk = TryGetRegistryKey();

			if( rk == null )
			{
				return false;
			}

			if( rk.GetValueKind( _key ) != _valueType )
			{
				return false;
			}

			object value = rk.GetValue( _key );

			switch( _valueType )
			{
				case RegistryValueKind.DWord:
					uint uvalue = (uint) value;

					if( _wordValue != uvalue )
					{
						return false;
					}
					break;
				case RegistryValueKind.String:
					string svalue = (string) value;

					if( _stringValue != svalue )
					{
						return false;
					}
					break;
				default:
					return false;
			}

			return true;
		}

		private RegistryKey GetRoot()
		{
			switch( _path[0] )
			{
				case "HKEY_CLASSES_ROOT":
					return Microsoft.Win32.Registry.ClassesRoot;
				case "HKEY_CURRENT_USER":
					return Microsoft.Win32.Registry.CurrentUser;
				case "HKEY_LOCAL_MACHINE":
					return Microsoft.Win32.Registry.LocalMachine;
				case "HKEY_USERS":
					return Microsoft.Win32.Registry.Users;
				case "HKEY_CURRENT_CONFIG":
					return Microsoft.Win32.Registry.CurrentConfig;
				default:
					throw new InvalidOperationException();
			}
		}

		private RegistryValue( string path, string key, RegistryValueKind valueType )
		{
			if( path == null )
			{
				throw new ArgumentNullException( "path" );
			}
			if( key == null )
			{
				throw new ArgumentNullException( "key" );
			}

			_path = path.Split( '\\' );

			if( _path.Length < 1 )
			{
				throw new ArgumentException( "path" );
			}

			_key = key;
			_valueType = valueType;
		}

		private string[] _path;
		private string _key;
		private RegistryValueKind _valueType;
		private string _stringValue;
		private uint _wordValue;
	}

	#endregion
}
