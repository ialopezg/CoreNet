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

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public class CommandContextMenuFactory : Factories.ContextMenuFactory
	{
		public CommandContextMenuFactory( Utility.Assemblies.ManifestResources resources,
						string prefix, CommandControlSet commandControlSet )
			: this( resources, System.Reflection.Assembly.GetCallingAssembly(), prefix, commandControlSet )
		{
		}

		public CommandContextMenuFactory( Utility.Assemblies.ManifestResources resources,
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

		protected override System.Windows.Forms.MenuItem CreateSingleMenuItem( System.Xml.XmlNode node )
		{
			switch( node.Name )
			{
				case "MenuCommand":
					{
						CommandMenuItem menuItem = new CommandMenuItem();

						LoadMenuItem( node, menuItem );

						if( node.Attributes == null || node.Attributes["Command"] == null )
						{
							throw new XmlException( string.Format( "Missing 'Command' attribute in command menu item '{0}'.", node.Name ) );
						}

						menuItem.Command = CreateCommand( node.Attributes["Command"].Value );

						_commandControlSet.AddControl( menuItem );

						return menuItem;
					}
				default:
					return base.CreateSingleMenuItem( node );
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
