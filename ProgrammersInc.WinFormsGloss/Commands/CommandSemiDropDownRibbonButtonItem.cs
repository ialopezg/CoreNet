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
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Commands
{
	public class CommandSemiDropDownRibbonButtonItem : Controls.Ribbon.SemiDropDownButtonItem, WinFormsUtility.Commands.ICommandControl
	{
		public CommandSemiDropDownRibbonButtonItem( string text, Icon icon16, Icon icon24 )
			: base( text, icon16, icon24 )
		{
		}

		public CommandSemiDropDownRibbonButtonItem( string text, Image image16, Image image24 )
			: base( text, image16, image24 )
		{
		}

		#region ICommandControl Members

		public WinFormsUtility.Commands.Command Command
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

		public WinFormsUtility.Commands.CommandControlSet CommandControlSet
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
			}
			else
			{
				Enabled = _command.IsEnabled();
			}
		}

		#endregion

		public override System.Windows.Forms.ToolStripItem CreateEquivalentToolStripItem()
		{
			string text = Text.Replace( "&", "&&" );

			WinFormsUtility.Commands.CommandToolStripMenuItem commandMenuItem = new WinFormsUtility.Commands.CommandToolStripMenuItem( text, _command );

			commandMenuItem.CommandControlSet = _commandControlSet;
			commandMenuItem.Image = Image16;

			return commandMenuItem;
		}

		protected override bool OnClick( Controls.Ribbon.Context context )
		{
			if( base.OnClick( context ) )
			{
				return true;
			}

			if( _command != null )
			{
				using( _commandControlSet.UsingCurrentInvocation() )
				{
					return _command.Invoke( Section.Ribbon );
				}
			}

			return false;
		}

		private WinFormsUtility.Commands.Command _command;
		private WinFormsUtility.Commands.CommandControlSet _commandControlSet;	}
}
