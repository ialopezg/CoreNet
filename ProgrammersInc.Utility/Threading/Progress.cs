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

namespace ProgrammersInc.Utility.Threading
{
	public delegate void Action( Progress progress );

	public sealed class Progress
	{
		public Progress()
		{
			_ranges.Push( new Range( 0, 100, string.Empty ) );
		}

		public static Progress Empty
		{
			get
			{
				return new Progress();
			}
		}

		public int Percentage
		{
			get
			{
				lock( _lock )
				{
					return _percentage;
				}
			}
		}

		public string Description
		{
			get
			{
				lock( _lock )
				{
					return _ranges.Peek().Description;
				}
			}
		}

		public bool IsCancelled
		{
			get
			{
				lock( _lock )
				{
					return _cancel;
				}
			}
		}

		public void CheckCancel()
		{
			lock( _lock )
			{
				if( _cancel )
				{
					throw new Exception( "Operation was cancelled." );
				}
			}
		}

		public void Cancel()
		{
			lock( _lock )
			{
				_cancel = true;
			}
		}

		public IDisposable GetRange( int start, int end, string description )
		{
			lock( _lock )
			{
				PushRange( start, end, description );

				_percentage = (int) _ranges.Peek().Start;

				return new RangeDisposable( this );
			}
		}

		public IDisposable GetRangeFromCount( int index, int total, string description )
		{
			if( index < 0 )
			{
				throw new ArgumentOutOfRangeException( "index" );
			}
			if( total < 1 )
			{
				throw new ArgumentOutOfRangeException( "total" );
			}
			if( index >= total )
			{
				throw new ArgumentOutOfRangeException( "index" );
			}

			double dIndex = index, dTotal = total;

			return GetRange( (int) (dIndex / dTotal * 100), (int) ((dIndex + 1) / dTotal * 100), description );
		}

		private void PushRange( int start, int end, string description )
		{
			lock( _lock )
			{
				Range range = _ranges.Peek();

				double diff = range.End - range.Start;

				_ranges.Push( new Range( range.Start + diff * start / 100.0, range.Start + diff * end / 100.0, description ) );
			}
		}

		private void PopRange()
		{
			lock( _lock )
			{
				_ranges.Pop();
			}
		}

		#region RangeDisposable

		private sealed class RangeDisposable : IDisposable
		{
			internal RangeDisposable( Progress progress )
			{
				_progress = progress;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if( _progress != null )
				{
					_progress.PopRange();
					_progress = null;
				}
			}

			#endregion

			private Progress _progress;
		}

		#endregion

		#region Range

		private sealed class Range
		{
			internal Range( double start, double end, string description )
			{
				Start = start;
				End = end;
				Description = description;
			}

			internal double Start, End;
			internal string Description;
		}

		#endregion

		private object _lock = new object();
		private int _percentage;
		private Stack<Range> _ranges = new Stack<Range>();
		private bool _cancel;
	}
}
