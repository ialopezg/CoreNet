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

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public class EnumComboBox : ComboBox
	{
		public void SetValues( object[] values, string[] descriptions )
		{
			_values = values;

			if( descriptions.Length != _values.Length )
			{
				throw new InvalidOperationException( "Incorrect number of descriptions for enum type." );
			}

			for( int i = 0; i < _values.Length; ++i )
			{
				Items.Add( descriptions[i] );
			}

			SelectedIndex = 0;
		}

		public void SetValue( object value )
		{
			int index = Array.IndexOf( _values, value );

			if( index >= 0 && index < Items.Count )
			{
				SelectedIndex = index;
			}
			else
			{
				throw new ArgumentOutOfRangeException( "value" );
			}
		}

		public object GetValue()
		{
			int index = SelectedIndex;

			if( index >= 0 && index < _values.Length )
			{
				return _values[index];
			}
			else
			{
				return null;
			}
		}

		private object[] _values;
	}
}
