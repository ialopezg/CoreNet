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

namespace ProgrammersInc.WinFormsUtility.Events
{
	public static class MenuLoop
	{
		public static bool InMenuLoop
		{
			get
			{
				return _inMenuLoop;
			}
		}

		public static void NotifyEnterMenuLoop()
		{
			_inMenuLoop = true;
		}

		public static void NotifyExitMenuLoop()
		{
			_inMenuLoop = false;
		}

		private static bool _inMenuLoop;
	}
}
