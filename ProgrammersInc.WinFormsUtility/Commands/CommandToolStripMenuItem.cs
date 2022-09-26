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

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public class CommandToolStripMenuItem : System.Windows.Forms.ToolStripMenuItem, ICommandControl
	{
		public CommandToolStripMenuItem()
		{
		}

		public CommandToolStripMenuItem( string text, Command command )
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
				if( _originalText == null )
				{
					_originalText = this.Text;
				}
				string text = _command.GetText( _originalText );
				if( text != null )
				{
					Text = text;
				}
			}
		}

		private string _originalText;

		#endregion

		protected override void OnClick( EventArgs e )
		{
			base.OnClick( e );

			if( _command != null )
			{
				using( this.CommandControlSet.UsingCurrentInvocation() )
				{
					_command.Invoke( this.CommandControlSet.Form );
				}
			}
		}

		private CommandControlSet _commandControlSet;
		private Command _command;
	}
}
