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

namespace ProgrammersInc.WinFormsGloss.Drawing
{
	public sealed class GlowUpdates<T>
	{
		public T[] Items
		{
			get
			{
				return _items.ToArray();
			}
		}

		public void NeedsUpdate( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			if( !_items.Contains( item ) )
			{
				_items.Add( item );
			}
		}

		public void DoneUpdate( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			_items.Remove( item );
		}

		public void DoneAll()
		{
			_items.Clear();
		}

		private List<T> _items = new List<T>();
	}
}
