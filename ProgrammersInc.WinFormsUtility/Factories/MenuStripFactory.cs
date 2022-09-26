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
using ProgrammersInc.WinFormsUtility.Commands;

namespace ProgrammersInc.WinFormsUtility.Factories
{
	public class MenuStripFactory : ControlFactory
	{
		public MenuStripFactory( Utility.Assemblies.ManifestResources resources )
			: base( resources )
		{
		}

		public MenuStrip CreateMenuStrip( XmlDocument xmlDoc )
		{
			Dictionary<string, ToolStripMenuItem> mapTagToMenuItem;

			return CreateMenuStrip( xmlDoc, out mapTagToMenuItem );
		}

		public MenuStrip CreateMenuStrip( XmlDocument xmlDoc, out Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			if( xmlDoc == null )
			{
				throw new ArgumentNullException( "xmlDoc" );
			}

			mapTagToMenuItem = new Dictionary<string, ToolStripMenuItem>();

			XmlNode menuStripNode = xmlDoc.SelectSingleNode( "/MenuStrip" );

			if( menuStripNode == null )
			{
				throw new XmlException( "Failed to find root MenuStrip node." );
			}

			MenuStrip menuStrip = new MenuStrip();

			foreach( XmlNode menuItemNode in menuStripNode.ChildNodes )
			{
				menuStrip.Items.Add( CreateItem( menuItemNode, mapTagToMenuItem ) );
			}

			return menuStrip;
		}

		public ContextMenuStrip CreateContextMenuStrip( XmlDocument xmlDoc )
		{
			Dictionary<string, ToolStripMenuItem> mapTagToMenuItem;

			return CreateContextMenuStrip( xmlDoc, out mapTagToMenuItem );
		}

		public ContextMenuStrip CreateContextMenuStrip( XmlDocument xmlDoc, out Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			if( xmlDoc == null )
			{
				throw new ArgumentNullException( "xmlDoc" );
			}

			mapTagToMenuItem = new Dictionary<string, ToolStripMenuItem>();

			XmlNode menuStripNode = xmlDoc.SelectSingleNode( "/ContextMenuStrip" );

			if( menuStripNode == null )
			{
				throw new XmlException( "Failed to find root ContextMenuStrip node." );
			}

			ContextMenuStrip menuStrip = new ContextMenuStrip();

			foreach( XmlNode menuItemNode in menuStripNode.ChildNodes )
			{
				menuStrip.Items.Add( CreateItem( menuItemNode, mapTagToMenuItem ) );
			}

			return menuStrip;
		}

		public ToolStripItem CreateItem( XmlNode node )
		{
			Dictionary<string, ToolStripMenuItem> mapTagToMenuItem = new Dictionary<string, ToolStripMenuItem>();

			return CreateItem( node, mapTagToMenuItem );
		}

		public ToolStripItem CreateItem( XmlNode node, Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			if( node is XmlDocument )
			{
				node = node.LastChild;
			}

			ToolStripItem item = CreateSingleMenuItem( node, mapTagToMenuItem );
			ToolStripMenuItem menuItem = item as ToolStripMenuItem;

			if( menuItem != null )
			{
				foreach( XmlNode childNode in node.ChildNodes )
				{
					menuItem.DropDownItems.Add( CreateItem( childNode, mapTagToMenuItem ) );
				}
			}

			return item;
		}

		private class CommandToolStripMenuContainer : System.Windows.Forms.ToolStripMenuItem
		{
			protected override void OnDropDownOpened( EventArgs e )
			{
				base.OnDropDownOpened( e );

				foreach( ToolStripItem item in this.DropDownItems )
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

		protected virtual ToolStripItem CreateSingleMenuItem( XmlNode node, Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			switch( node.Name )
			{
				case "MenuItem":
					{
						CommandToolStripMenuContainer menuItem = new CommandToolStripMenuContainer();

						LoadMenuItem( node, menuItem, mapTagToMenuItem );

						return menuItem;
					}
				case "Separator":
					{
						ToolStripSeparator separator = new ToolStripSeparator();

						return separator;
					}
				default:
					throw new XmlException( string.Format( "Unknown menu item type '{0}'.", node.Name ) );
			}
		}

		protected void LoadMenuItem( XmlNode node, ToolStripMenuItem menuItem, Dictionary<string, ToolStripMenuItem> mapTagToMenuItem )
		{
			if( node.Attributes == null || node.Attributes["Text"] == null )
			{
				throw new XmlException( string.Format( "Missing 'Text' attribute in menu item '{0}'.", node.Name ) );
			}

			menuItem.Text = node.Attributes["Text"].Value;

			if( node.Attributes != null && node.Attributes["Bold"] != null && node.Attributes["Bold"].Value == "true" )
			{
				menuItem.Font = new System.Drawing.Font( menuItem.Font, System.Drawing.FontStyle.Bold );
			}

			if( node.Attributes != null && node.Attributes["Name"] != null )
			{
				menuItem.Name = node.Attributes["Name"].Value;
			}

			if( node.Attributes != null && node.Attributes["Image"] != null )
			{
				menuItem.Image = Resources.GetIcon( node.Attributes["Image"].Value ).ToBitmap();
			}

			if( node.Attributes != null && node.Attributes["Tag"] != null )
			{
				string tagName = node.Attributes["Tag"].Value;

				menuItem.Tag = tagName;
				mapTagToMenuItem[tagName] = menuItem;
			}
		}
	}
}
