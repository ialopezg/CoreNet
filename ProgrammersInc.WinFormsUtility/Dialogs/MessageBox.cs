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

namespace ProgrammersInc.WinFormsUtility.Dialogs
{
	public static class MessageBox
	{
		public static void Show( IWin32Window owner, string message )
		{
			System.Windows.Forms.MessageBox.Show
					( owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
		}
	}
}
