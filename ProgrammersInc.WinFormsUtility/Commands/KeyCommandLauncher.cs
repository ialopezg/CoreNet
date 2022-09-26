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
	public sealed class KeyCommandLauncher
	{
		public KeyCommandLauncher()
		{
		}

		public KeyCommandLauncher( Control[] selectedControls )
		{
			_selectedControls = selectedControls;
		}

		public void Add( Keys keys, Command command )
		{
			if( command == null )
			{
				throw new ArgumentNullException( "command" );
			}

			List<Command> commands;

			if( !_mapKeysToCommands.TryGetValue( keys, out commands ) )
			{
				commands = new List<Command>();
				_mapKeysToCommands.Add( keys, commands );
			}

			commands.Add( command );

			List<Keys> keyList;

			if( !_mapCommandToKeys.TryGetValue( command, out keyList ) )
			{
				keyList = new List<Keys>();
				_mapCommandToKeys.Add( command, keyList );
			}

			keyList.Add( keys );
		}

		public bool ProcessCmdKey( Control owner, Keys keyData )
		{
			if( _selectedControls != null )
			{
				bool found = false;

				foreach( Control control in _selectedControls )
				{
					if( control.Focused )
					{
						found = true;
					}
				}

				if( !found )
				{
					return false;
				}
			}

			List<Command> commands;

			if( _mapKeysToCommands.TryGetValue( keyData, out commands ) )
			{
				foreach( Command command in commands )
				{
					if( command.IsEnabled() )
					{
						command.Invoke( owner );
						return true;
					}
				}
			}

			return false;
		}

		public string GetKeyText( Command command )
		{
			if( command == null )
			{
				throw new ArgumentNullException( "command" );
			}

			List<Keys> keyList;

			if( _mapCommandToKeys.TryGetValue( command, out keyList ) )
			{
				List<string> texts = new List<string>();

				foreach( Keys keys in keyList )
				{
					string text = string.Empty;

					if( (keys & Keys.Control) != 0 )
					{
						text += "Ctrl+";
					}
					if( (keys & Keys.Shift) != 0 )
					{
						text += "Shift+";
					}
					if( (keys & Keys.Alt) != 0 )
					{
						text += "Alt+";
					}

					Keys unmodified = keys & ~Keys.Modifiers;

					text += Enum.GetName( typeof( Keys ), unmodified );

					texts.Add( text );
				}

				return string.Join( "; ", texts.ToArray() );
			}
			else
			{
				return null;
			}
		}

		public void UpdateToolStrip( ToolStrip toolStrip )
		{
			foreach( ToolStripItem toolStripItem in toolStrip.Items )
			{
				CommandToolStripButton commandToolStripButton = toolStripItem as CommandToolStripButton;

				if( commandToolStripButton != null )
				{
					if( commandToolStripButton != null && commandToolStripButton.Command != null )
					{
						string keyText = GetKeyText( commandToolStripButton.Command );

						if( keyText != null )
						{
							commandToolStripButton.ToolTipText += string.Format( " ({0})", keyText );
						}
					}
				}
			}
		}

		public void UpdateMenu( MenuStrip menuStrip )
		{
			foreach( ToolStripMenuItem toolStripMenuItem in menuStrip.Items )
			{
				UpdateToolStripMenuItem( toolStripMenuItem );
			}
		}

		public void UpdateMenu( ContextMenuStrip menuStrip )
		{
			foreach( ToolStripItem toolStripItem in menuStrip.Items )
			{
				ToolStripMenuItem toolStripMenuItem = toolStripItem as ToolStripMenuItem;

				if( toolStripMenuItem != null )
				{
					UpdateToolStripMenuItem( toolStripMenuItem );
				}
			}
		}

		public void UpdateToolStripMenuItem( ToolStripMenuItem toolStripMenuItem )
		{
			CommandToolStripMenuItem commandMenuItem = toolStripMenuItem as CommandToolStripMenuItem;

			if( commandMenuItem != null && commandMenuItem.Command != null )
			{
				commandMenuItem.ShortcutKeyDisplayString = GetKeyText( commandMenuItem.Command );
			}

			foreach( ToolStripItem toolStripSubItem in toolStripMenuItem.DropDownItems )
			{
				ToolStripMenuItem toolStripMenuSubItem = toolStripSubItem as ToolStripMenuItem;

				if( toolStripMenuSubItem != null )
				{
					UpdateToolStripMenuItem( toolStripMenuSubItem );
				}
			}
		}

		private Control[] _selectedControls;
		private Dictionary<Keys, List<Command>> _mapKeysToCommands = new Dictionary<Keys, List<Command>>();
		private Dictionary<Command, List<Keys>> _mapCommandToKeys = new Dictionary<Command, List<Keys>>();
	}
}
