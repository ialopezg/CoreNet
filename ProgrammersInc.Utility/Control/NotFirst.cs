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
	public sealed class NotFirst
	{
		public NotFirst( Action action )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}

			_action = action;
		}

		public void Invoke()
		{
			if( !_notfirst )
			{
				_notfirst = true;
			}
			else
			{
				_action();
			}
		}

		private bool _notfirst;
		private Action _action;	}
}
