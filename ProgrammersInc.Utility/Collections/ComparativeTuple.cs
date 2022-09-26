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

namespace ProgrammersInc.Utility.Collections
{
	public sealed class ComparativeTuple
	{
		public ComparativeTuple( object[] values )
		{
			_values = values;
		}

		public override int GetHashCode()
		{
			int hc = 0;

			foreach( object obj in _values )
			{
				if( obj != null )
				{
					hc ^= obj.GetHashCode();
				}
			}

			return hc;
		}

		public override bool Equals( object obj )
		{
			ComparativeTuple ct = obj as ComparativeTuple;

			if( ct == null )
			{
				return false;
			}

			if( _values.Length != ct._values.Length )
			{
				return false;
			}

			for( int i = 0; i < _values.Length; ++i )
			{
				if( !object.Equals( _values[i], ct._values[i] ) )
				{
					return false;
				}
			}

			return true;
		}

		private object[] _values;
	}
}
