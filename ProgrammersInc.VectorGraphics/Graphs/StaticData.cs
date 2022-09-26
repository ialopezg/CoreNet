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

namespace ProgrammersInc.VectorGraphics.Graphs
{
	public class StaticData : IData
	{
		public StaticData( int rowCount, int columnCount )
		{
			_data = new double[rowCount, columnCount];
			_rowExtras = new Dictionary<string, string>[rowCount];
			_columnExtras = new Dictionary<string, string>[columnCount];

			for( int r = 0; r < rowCount; ++r )
			{
				_rowExtras[r] = new Dictionary<string, string>();
			}
			for( int c = 0; c < columnCount; ++c )
			{
				_columnExtras[c] = new Dictionary<string, string>();
			}
		}

		#region IData Members

		public int RowCount
		{
			get
			{
				return _data.GetLength( 0 );
			}
		}

		public int ColumnCount
		{
			get
			{
				return _data.GetLength( 1 );
			}
		}

		public double this[int r, int c]
		{
			get
			{
				return _data[r, c];
			}
			set
			{
				_data[r, c] = value;
			}
		}

		public string GetExtra( string key, string fallback )
		{
			string v;

			if( _extras.TryGetValue( key, out v ) )
			{
				return v;
			}
			else
			{
				return fallback;
			}
		}

		public string GetRowExtra( int row, string key, string fallback )
		{
			string v;

			if( _rowExtras[row].TryGetValue( key, out v ) )
			{
				return v;
			}
			else
			{
				return fallback;
			}
		}

		public string GetColumnExtra( int column, string key, string fallback )
		{
			string v;

			if( _columnExtras[column].TryGetValue( key, out v ) )
			{
				return v;
			}
			else
			{
				return fallback;
			}
		}

		#endregion

		public void SetExtra( string key, string value )
		{
			_extras.Add( key, value );
		}

		public void SetRowExtra( int row, string key, string value )
		{
			_rowExtras[row].Add( key, value );
		}

		public void SetColumnExtra( int column, string key, string value )
		{
			_columnExtras[column].Add( key, value );
		}

		private Dictionary<string, string> _extras = new Dictionary<string, string>();
		private Dictionary<string, string>[] _rowExtras, _columnExtras;
		private double[,] _data;
	}
}
