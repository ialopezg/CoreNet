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
	public class ListView : System.Windows.Forms.ListView
	{
		public IDisposable SuspendUpdate()
		{
			return new Updater( this );
		}

		public bool CanUpdate
		{
			get
			{
				return _updateDisabledCount == 0;
			}
		}

		public int ExpandingColumn
		{
			get
			{
				return _expandingColumn;
			}
			set
			{
				_expandingColumn = value;
			}
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			if( _expandingColumn >= 0 && _expandingColumn < Columns.Count )
			{
				int width = ClientRectangle.Width - 10;

				foreach( ColumnHeader column in Columns )
				{
					if( column.Index != _expandingColumn )
					{
						width -= column.Width;
					}
				}

				Columns[_expandingColumn].Width = width;
			}
		}

		protected virtual void OnUpdateEnabled()
		{
		}

		#region Updater

		private sealed class Updater : IDisposable
		{
			internal Updater( ListView listView )
			{
				_listView = listView;

				if( _listView._updateDisabledCount == 0 )
				{
					_listView.BeginUpdate();
				}

				_listView._updateDisabledCount++;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if( _listView != null )
				{
					_listView._updateDisabledCount--;

					if( _listView._updateDisabledCount == 0 )
					{
						_listView.OnUpdateEnabled();
						_listView.Invalidate();
						_listView.EndUpdate();
					}
					_listView = null;
				}
			}

			#endregion

			private ListView _listView;
		}

		#endregion

		private int _updateDisabledCount = 0;
		private int _expandingColumn = -1;
	}
}
