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
	public interface IData
	{
		int RowCount
		{
			get;
		}

		int ColumnCount
		{
			get;
		}

		double this[int r, int c]
		{
			get;
		}

		string GetExtra( string key, string fallback );
		string GetRowExtra( int r, string key, string fallback );
		string GetColumnExtra( int c, string key, string fallback );
	}
}
