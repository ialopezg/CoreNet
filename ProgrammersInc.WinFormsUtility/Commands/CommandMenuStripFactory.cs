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
using System.Xml;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public class CommandMenuStripFactory : Factories.MenuStripFactory
	{
		public CommandMenuStripFactory( Utility.Assemblies.ManifestResources resources,
				string prefix, CommandControlSet commandControlSet )
			: this( resources, System.Reflection.Assembly.GetCallingAssembly(), prefix, commandControlSet )
		{
		}

		public CommandMenuStripFactory( Utility.Assemblies.ManifestResources resources,
				System.Reflection.Assembly assembly, string prefix, CommandControlSet commandControlSet )
			: base( resources )
		{
			if( assembly == null )
			{
				throw new ArgumentNullException( "assembly" );
			}
			if( prefix == null )
			{
				throw new ArgumentNullException( "prefix" );
			}
			if( commandControlSet == null )
			{
				throw new ArgumentNullException( "commandControlSet" );
			}

			_assembly = assembly;
			_prefix = prefix;
			_commandControlSet = commandControlSet;
		}

		public static void DestroyContextMenuStrip( WinFormsUtility.Commands.CommandControlSet commandControlSet, ContextMenuStrip menuStrip )
		{
			foreach( ToolStripItem toolStripItem in menuStrip.Items )
			{
				CommandToolStripMenuItem commandToolStripMenuItem = toolStripItem as CommandToolStripMenuItem;

				if( commandToolStripMenuItem != null )
				{
					commandControlSet.RemoveControl( commandToolStripMenuItem );
					commandToolStripMenuItem.Command = null;
				}
			}
		}

		private class CommandToolStripMenuContainer : System.Windows.Forms.ToolStripMenuItem
		{
			protected override void OnDropDownOpened( EventArgs e )
			{
				base.OnDropDownOpened( e );

				foreach( ToolStripMenuItem item in this.DropDownItems )
				{
					CommandToolStripMenuItem commandItem = item as CommandToolStripMenuItem;

					if( commandItem != null )
					{
						commandItem.UpdateState();
						commandItem.CommandControlSet.UpdateState();
					}
				}
			}
		}

		protected override System.Windows.Forms.ToolStripItem CreateSingleMenuItem( System.Xml.XmlNode node, Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			switch( node.Name )
			{
				case "MenuCommand":
					{
						CommandToolStripMenuItem menuItem = new CommandToolStripMenuItem();

						LoadMenuItem( node, menuItem, mapTagToMenuItem );

						if( node.Attributes == null || node.Attributes["Command"] == null )
						{
							throw new ApplicationException( string.Format( "Missing 'Command' attribute in command menu item '{0}'.", node.Name ) );
						}
						menuItem.Command = CreateCommand( node.Attributes["Command"].Value );

						_commandControlSet.AddControl( menuItem );

						return menuItem;
					}
				default:
					return base.CreateSingleMenuItem( node, mapTagToMenuItem );
			}
		}

		protected virtual Command CreateCommand( string name )
		{
			string fullname = Prefix + name;

			Command command = (Command) _assembly.CreateInstance
				( fullname, false, System.Reflection.BindingFlags.CreateInstance, null,
				CommandConstructorArguments, null, null );

			if( command == null )
			{
				throw new ApplicationException( string.Format( "Failed to create command '{0}'.", fullname ) );
			}

			return command;
		}

		protected virtual object[] CommandConstructorArguments
		{
			get
			{
				return new object[] { };
			}
		}

		protected string Prefix
		{
			get
			{
				return _prefix;
			}
		}

		private System.Reflection.Assembly _assembly;
		private string _prefix;
		private CommandControlSet _commandControlSet;
	}
}
