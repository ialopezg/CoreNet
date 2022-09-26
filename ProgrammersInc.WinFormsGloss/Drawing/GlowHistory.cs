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
	public sealed class GlowHistory<T>
	{
		public GlowHistory( T def )
		{
			_def = def;
		}

		public T MouseOver
		{
			get
			{
				return _firstOver == null ? _def : _firstOver.Value.Key;
			}
		}

		public double? GetTimeOver( T t )
		{
			if( t == null )
			{
				throw new ArgumentNullException( "t" );
			}

			if( _firstOver == null )
			{
				return null;
			}
			else if( object.Equals( _firstOver.Value.Key, t ) )
			{
				return DateTime.Now.Subtract( _firstOver.Value.Value ).TotalSeconds;
			}
			else
			{
				return null;
			}
		}

		public double GetLastOver( T t )
		{
			if( t == null )
			{
				throw new ArgumentNullException( "t" );
			}

			DateTime d;

			if( _lastOver.TryGetValue( t, out d ) )
			{
				return DateTime.Now.Subtract( d ).TotalSeconds;
			}
			else
			{
				return double.MaxValue;
			}
		}

		public void Update( T over )
		{
			if( _firstOver == null || !object.Equals( _firstOver.Value.Key, over ) )
			{
				if( _firstOver != null )
				{
					T old = _firstOver.Value.Key;

					_lastOver[old] = DateTime.Now;
				}

				if( over != null )
				{
					_firstOver = new KeyValuePair<T, DateTime>( over, DateTime.Now );
				}
			}
			if( over == null )
			{
				_firstOver = null;
			}
		}

		private T _def;
		private Dictionary<T, DateTime> _lastOver = new Dictionary<T, DateTime>();
		private KeyValuePair<T, DateTime>? _firstOver;
	}
}
