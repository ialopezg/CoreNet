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

namespace ProgrammersInc.VectorGraphics.Styles
{
	public sealed class Style
	{
		internal Style()
		{
		}

		public string[] Classes
		{
			get
			{
				return _classes.ToArray();
			}
		}

		public bool Has( string name )
		{
			if( name == null )
			{
				throw new ArgumentNullException( "name" );
			}

			return _classes.Contains( name );
		}

		public string GetExtra( string key )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "key" );
			}

			string value;

			if( _extras.TryGetValue( key, out value ) )
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		public void Add( string name )
		{
			if( name == null )
			{
				throw new ArgumentNullException( "name" );
			}

			_classes.Add( name );
		}

		public void AddExtra( string key, string value )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "key" );
			}
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}

			_extras.Add( key, value );
		}

		public void MergeWith( Style style )
		{
			foreach( string s in style._classes )
			{
				if( !_classes.Contains( s ) )
				{
					_classes.Add( s );
				}
			}

			foreach( KeyValuePair<string, string> kvp in style._extras )
			{
				_extras.Add( kvp.Key, kvp.Value );
			}
		}

		private Utility.Collections.Set<string> _classes = new Utility.Collections.Set<string>();
		private Dictionary<string, string> _extras = new Dictionary<string, string>();
	}
}
