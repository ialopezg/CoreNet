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
	public static class QuestionBox
	{
		public static bool AskOkCancel( IWin32Window owner, string message )
		{
			System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show
					( owner, message, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

			return dr == DialogResult.OK;
		}

		public static bool AskYesNo( IWin32Window owner, string message )
		{
			System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show
					( owner, message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation );

			return dr == DialogResult.Yes;
		}
	}
}
