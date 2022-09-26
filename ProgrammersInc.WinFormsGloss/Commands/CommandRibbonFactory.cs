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
using System.Xml;

namespace ProgrammersInc.WinFormsGloss.Commands
{
	public class CommandRibbonFactory : Factories.RibbonFactory
	{
		public CommandRibbonFactory( Utility.Assemblies.ManifestResources resources,
				string prefix, WinFormsUtility.Commands.CommandControlSet commandControlSet )
			: this( resources, System.Reflection.Assembly.GetCallingAssembly(), prefix, commandControlSet )
		{
		}

		public CommandRibbonFactory( Utility.Assemblies.ManifestResources resources,
				System.Reflection.Assembly assembly, string prefix, WinFormsUtility.Commands.CommandControlSet commandControlSet )
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

		public static void DestroyRibbon( WinFormsUtility.Commands.CommandControlSet commandControlSet, Controls.Ribbon.RibbonControl ribbonControl )
		{
			foreach( Controls.Ribbon.Section section in ribbonControl.Sections )
			{
				foreach( Controls.Ribbon.Item item in section.Items )
				{
					CommandRibbonButtonItem commandRibbonButtonItem = item as CommandRibbonButtonItem;

					if( commandRibbonButtonItem != null )
					{
						commandControlSet.RemoveControl( commandRibbonButtonItem );
						commandRibbonButtonItem.Command = null;
					}
				}
			}

			while( ribbonControl.Sections.Length > 0 )
			{
				ribbonControl.RemoveSection( ribbonControl.Sections[0] );
			}
		}

		protected override Controls.Ribbon.Item CreateSingleItem( System.Xml.XmlNode node, Dictionary<string, Controls.Ribbon.Section> mapTagToSection, Dictionary<string, Controls.Ribbon.Item> mapTagToItem )
		{
			switch( node.Name )
			{
				case "RibbonCommand":
				case "SemiDropDownRibbonCommand":
				case "AnimatedRibbonCommand":
					{
						string text = null;
						Icon icon16 = null, icon24 = null;

						if( node.Attributes["Text"] != null )
						{
							text = node.Attributes["Text"].Value;
						}
						if( node.Attributes["Icon"] != null )
						{
							string name = node.Attributes["Icon"].Value;

							icon16 = Resources.GetIcon( string.Format( name, 16 ) );
							icon24 = Resources.GetIcon( string.Format( name, 24 ) );
						}

						Controls.Ribbon.ButtonItem item;
						WinFormsUtility.Commands.Command command;
						WinFormsUtility.Commands.ICommandControl commandControl;

						if( node.Attributes == null || node.Attributes["Command"] == null )
						{
							throw new XmlException( string.Format( "Missing 'Command' attribute in command button item '{0}'.", node.Name ) );
						}

						command = CreateCommand( node.Attributes["Command"].Value );

						switch( node.Name )
						{
							case "RibbonCommand":
								{
									CommandRibbonButtonItem commandItem = new CommandRibbonButtonItem( text, icon16, icon24 );

									commandItem.Command = command;
									item = commandItem;
									commandControl = commandItem;
									break;
								}
							case "AnimatedRibbonCommand":
								{
									CommandAnimatedRibbonButtonItem commandItem = new CommandAnimatedRibbonButtonItem( text, icon16, icon24 );

									commandItem.Command = command;
									item = commandItem;
									commandControl = commandItem;
									break;
								}
							case "SemiDropDownRibbonCommand":
								{
									CommandSemiDropDownRibbonButtonItem commandItem = new CommandSemiDropDownRibbonButtonItem( text, icon16, icon24 );

									commandItem.Command = command;
									item = commandItem;
									commandControl = commandItem;
									break;
								}
							default:
								throw new InvalidOperationException();
						}

						if( node.Attributes != null && node.Attributes["Tag"] != null )
						{
							string tagName = node.Attributes["Tag"].Value;

							mapTagToItem[tagName] = item;
						}

						if( node.Attributes["Importance"] != null )
						{
							item.Importance = int.Parse( node.Attributes["Importance"].Value );
						}
						if( node.Attributes["TooltipTitle"] != null )
						{
							item.TooltipTitle = node.Attributes["TooltipTitle"].Value;
						}
						if( node.Attributes["TooltipDescription"] != null )
						{
							item.TooltipDescription = node.Attributes["TooltipDescription"].Value;
						}

						_commandControlSet.AddControl( commandControl );

						return item;
					}
				default:
					return base.CreateSingleItem( node, mapTagToSection, mapTagToItem );
			}
		}

		protected virtual WinFormsUtility.Commands.Command CreateCommand( string name )
		{
			string fullname = Prefix + name;

			WinFormsUtility.Commands.Command command = (WinFormsUtility.Commands.Command) _assembly.CreateInstance
				( fullname, false, System.Reflection.BindingFlags.CreateInstance, null
				, CommandConstructorArguments, null, null );

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
		private WinFormsUtility.Commands.CommandControlSet _commandControlSet;
	}
}
