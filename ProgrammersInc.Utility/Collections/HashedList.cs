using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ProgrammersInc.Utility.Collections
{
	[DebuggerDisplay("Count={_list.Count}")]
	public sealed class HashedList<T> : IEnumerable<T>, ICollection<T>
	{
		public HashedList()
		{
		}

		public HashedList( IEnumerable<T> items )
		{
			_list.AddRange( items );
			_positions = null;
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		public void AddRange( IEnumerable<T> items )
		{
			foreach( T item in items )
			{
				Add( item );
			}
		}

		public void Sort( Comparison<T> comparison )
		{
			_list.Sort( comparison );
			_positions = new Dictionary<T, int>();

			for( int i = 0; i < _list.Count; ++i )
			{
				_positions[_list[i]] = i;
			}
		}

		#region ICollection<T> Members

		public void Add( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}
			if( Contains( item ) )
			{
				throw new ArgumentException( "List already contains this item.", "item" );
			}

			_list.Add( item );
			_positions.Add( item, _list.Count - 1 );
		}

		public void Clear()
		{
			_list = new List<T>();
			_positions = new Dictionary<T, int>();
		}

		public bool Contains( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			EnsurePositions();

			return _positions.ContainsKey( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			_list.CopyTo( array, arrayIndex );
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			if( !Contains( item ) )
			{
				return false;
			}

			int position = _positions[item];

			_list.RemoveAt( position );

			_positions = null;

			return true;
		}

		#endregion

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
		}

		public int BinarySearch( T item, IComparer<T> comparer )
		{
			return _list.BinarySearch( item, comparer );
		}

		public int IndexOf( T item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			EnsurePositions();

			int pos;

			if( !_positions.TryGetValue( item, out pos ) )
			{
				pos = -1;
			}

			return pos;
		}

		private void EnsurePositions()
		{
			if( _positions != null )
			{
				return;
			}

			_positions = new Dictionary<T, int>();

			for( int i = 0; i < _list.Count; ++i )
			{
				_positions.Add( _list[i], i );
			}
		}

		private List<T> _list = new List<T>();
		private Dictionary<T, int> _positions = new Dictionary<T,int>();
	}
}
