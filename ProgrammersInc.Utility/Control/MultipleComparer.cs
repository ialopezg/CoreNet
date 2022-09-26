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

namespace ProgrammersInc.Utility.Control
{
	public class MultipleComparer<T> : IComparer<T>
	{
		public MultipleComparer( params Comparison<T>[] comparers )
		{
			_comparers = comparers;
		}

		#region IComparer<T> Members

		public int Compare( T x, T y )
		{
			foreach( Comparison<T> comparer in _comparers )
			{
				int v = comparer( x, y );

				if( v != 0 )
				{
					return v;
				}
			}

			return 0;
		}

		#endregion

		private Comparison<T>[] _comparers;
	}
}
