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
	public sealed class Flag
	{
		public bool IsActive
		{
			get
			{
				lock( _lock )
				{
					return _count > 0;
				}
			}
		}

		public IDisposable Apply()
		{
			int count;

			lock( _lock )
			{
				++_count;

				count = _count;
			}

			if( count == 1 && Set != null )
			{
				Set( this, EventArgs.Empty );
			}

			return new Disposer( this );
		}

		private sealed class Disposer : IDisposable
		{
			internal Disposer( Flag flag )
			{
				_flag = flag;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if( _flag != null )
				{
					int count;

					lock( _flag._lock )
					{
						--_flag._count;

						count = _flag._count;
					}

					if( count == 0 && _flag.Reset != null )
					{
						_flag.Reset( _flag, EventArgs.Empty );
					}

					_flag = null;
				}
			}

			#endregion

			private Flag _flag;
		}

		public event EventHandler Set;
		public event EventHandler Reset;

		private int _count;
		private object _lock = new object();
	}
}
