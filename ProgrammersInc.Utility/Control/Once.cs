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
	public delegate void Action();

	public sealed class Once
	{
		public Once( Action action )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}

			_action = action;
		}

		public void Invoke()
		{
			if( !_actioned )
			{
				_actioned = true;
				_action();
			}
		}

		private bool _actioned;
		private Action _action;
	}
}
