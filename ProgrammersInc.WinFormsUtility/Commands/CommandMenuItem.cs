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

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public class CommandMenuItem : MenuItem, ICommandControl
	{
		public CommandMenuItem()
		{
		}

		public CommandMenuItem( string text, Command command )
		{
			if( text == null )
			{
				throw new ArgumentNullException( "text" );
			}
			if( command == null )
			{
				throw new ArgumentNullException( "command" );
			}

			Text = text;
			Command = command;
		}

		#region ICommandControl Members

		public Command Command
		{
			get
			{
				return _command;
			}
			set
			{
				_command = value;

				UpdateState();
			}
		}

		public CommandControlSet CommandControlSet
		{
			get
			{
				return _commandControlSet;
			}
			set
			{
				_commandControlSet = value;
			}
		}

		public void UpdateState()
		{
			if( _command == null )
			{
				Enabled = false;
				Checked = false;
			}
			else
			{
				Enabled = _command.IsEnabled();
				Checked = _command.IsChecked();
			}
		}

		#endregion

		protected override void OnClick( EventArgs e )
		{
			base.OnClick( e );

			if( _command != null )
			{
				using( _commandControlSet.UsingCurrentInvocation() )
				{
					_command.Invoke( _commandControlSet.Form );
				}
			}
		}

		private Command _command;
		private CommandControlSet _commandControlSet;
	}
}
