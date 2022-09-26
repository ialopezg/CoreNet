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
	public class KeyCommandLauncherFactory
	{
		public KeyCommandLauncherFactory( Utility.Assemblies.ManifestResources resources,
						string prefix, CommandControlSet commandControlSet )
			: this( resources, System.Reflection.Assembly.GetCallingAssembly(), prefix, commandControlSet )
		{
		}

		public KeyCommandLauncherFactory( Utility.Assemblies.ManifestResources resources,
				System.Reflection.Assembly assembly, string prefix, CommandControlSet commandControlSet )
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
			if( resources == null )
			{
				throw new ArgumentNullException( "resources" );
			}

			_resources = resources;
			_assembly = assembly;
			_prefix = prefix;
			_commandControlSet = commandControlSet;
		}

		public void Create( XmlDocument xmlDoc, KeyCommandLauncher keyCommandLauncher )
		{
			foreach( XmlNode keyCommandNode in xmlDoc.SelectNodes( "/KeyCommands/KeyCommand" ) )
			{
				string keyText = keyCommandNode.Attributes["Key"].Value;
				string commandText = keyCommandNode.Attributes["Command"].Value;

				Command command = CreateCommand( commandText );
				Keys keys = Keys.None;

				string[] keyTextParts = keyText.Split( '+' );

				foreach( string keyTextPart in keyTextParts )
				{
					switch( keyTextPart )
					{
						case "Ctrl":
							keys |= Keys.Control;
							break;
						case "Shift":
							keys |= Keys.Shift;
							break;
						case "Alt":
							keys |= Keys.Alt;
							break;
						default:
							keys |= (Keys) Enum.Parse( typeof( Keys ), keyTextPart );
							break;
					}
				}

				keyCommandLauncher.Add( keys, command );
			}
		}

		protected virtual Command CreateCommand( string name )
		{
			string fullname = Prefix + name;

			Command command = (Command) _assembly.CreateInstance
					( fullname, false, System.Reflection.BindingFlags.CreateInstance, null
					,	CommandConstructorArguments, null, null );

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
		private Utility.Assemblies.ManifestResources _resources;
	}
}
