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
	public class CommandToolStripFactory : Factories.ToolStripFactory
	{
		public CommandToolStripFactory( Utility.Assemblies.ManifestResources resources,
						string prefix, CommandControlSet commandControlSet )
			: this( resources, System.Reflection.Assembly.GetCallingAssembly(), prefix, commandControlSet )
		{
		}

		public CommandToolStripFactory( Utility.Assemblies.ManifestResources resources,
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

		public static void DestroyToolStrip( WinFormsUtility.Commands.CommandControlSet commandControlSet, ToolStrip toolStrip )
		{
			foreach( ToolStripItem toolStripItem in toolStrip.Items )
			{
				CommandToolStripButton commandToolStripButton = toolStripItem as CommandToolStripButton;

				if( commandToolStripButton != null )
				{
					commandControlSet.RemoveControl( commandToolStripButton );
					commandToolStripButton.Command = null;
				}
			}
		}

		protected override System.Windows.Forms.ToolStripItem CreateToolStripItem( System.Xml.XmlNode node )
		{
			switch( node.Name )
			{
				case "ToolStripCommand":
					{
						CommandToolStripButton toolStripButton = new CommandToolStripButton();

						LoadToolStripButton( node, toolStripButton );

						if( node.Attributes == null || node.Attributes["Command"] == null )
						{
							throw new XmlException( string.Format( "Missing 'Command' attribute in command toolstrip button '{0}'.", node.Name ) );
						}

						toolStripButton.Command = CreateCommand( node.Attributes["Command"].Value );

						_commandControlSet.AddControl( toolStripButton );

						return toolStripButton;
					}
				default:
					return base.CreateToolStripItem( node );
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
				throw new XmlException( string.Format( "Failed to create command '{0}'.", fullname ) );
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
