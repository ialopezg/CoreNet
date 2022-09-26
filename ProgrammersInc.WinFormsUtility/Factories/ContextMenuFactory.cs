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
using System.Xml;

namespace ProgrammersInc.WinFormsUtility.Factories
{
	public class ContextMenuFactory : ControlFactory
	{
		public ContextMenuFactory( Utility.Assemblies.ManifestResources resources )
			: base( resources )
		{
		}

		public ContextMenu CreateContextMenu( XmlDocument xmlDoc )
		{
			if( xmlDoc == null )
			{
				throw new ArgumentNullException( "xmlDoc" );
			}

			XmlNode contextMenuNode = xmlDoc.SelectSingleNode( "/ContextMenu" );

			if( contextMenuNode == null )
			{
				throw new XmlException( "Failed to find root ContextMenu node." );
			}

			ContextMenu contextMenu = new ContextMenu();

			foreach( XmlNode menuItemNode in contextMenuNode.ChildNodes )
			{
				contextMenu.MenuItems.Add( CreateItem( menuItemNode ) );
			}

			return contextMenu;
		}

		public MenuItem CreateItem( XmlNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			MenuItem menuItem = CreateSingleMenuItem( node );

			foreach( XmlNode childNode in node.ChildNodes )
			{
				menuItem.MenuItems.Add( CreateItem( childNode ) );
			}

			return menuItem;
		}

		protected virtual MenuItem CreateSingleMenuItem( XmlNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			switch( node.Name )
			{
				case "MenuItem":
					{
						MenuItem menuItem = new MenuItem();

						LoadMenuItem( node, menuItem );

						return menuItem;
					}
				case "Separator":
					{
						MenuItem menuItem = new MenuItem();

						menuItem.Text = "-";

						return menuItem;
					}
				default:
					throw new XmlException( string.Format( "Unknown menu item type '{0}'.", node.Name ) );
			}
		}

		protected void LoadMenuItem( XmlNode node, MenuItem menuItem )
		{
			if( node.Attributes == null || node.Attributes["Text"] == null )
			{
				throw new XmlException( string.Format( "Missing 'Text' attribute in menu item '{0}'.", node.Name ) );
			}
			if( node.Attributes != null && node.Attributes["Name"] != null )
			{
				menuItem.Name = node.Attributes["Name"].Value;
			}
			menuItem.Text = node.Attributes["Text"].Value;
		}
	}
}
