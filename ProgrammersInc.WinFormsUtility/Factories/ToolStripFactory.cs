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
	public class ToolStripFactory : ControlFactory
	{
		public ToolStripFactory( Utility.Assemblies.ManifestResources resources )
			: base( resources )
		{
		}

		public ToolStrip CreateToolStrip( XmlDocument xmlDoc )
		{
			if( xmlDoc == null )
			{
				throw new ArgumentNullException( "xmlDoc" );
			}

			XmlNode toolStripNode = xmlDoc.SelectSingleNode( "/ToolStrip" );

			if( toolStripNode == null )
			{
				throw new XmlException( "Failed to find root ToolStrip node." );
			}

			ToolStrip toolStrip = new ToolStrip();

			foreach( XmlNode toolStripItemNode in toolStripNode.ChildNodes )
			{
				toolStrip.Items.Add( CreateToolStripItem( toolStripItemNode ) );
			}

			return toolStrip;
		}

		protected virtual ToolStripItem CreateToolStripItem( XmlNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			switch( node.Name )
			{
				case "ToolStripSeparator":
					{
						ToolStripSeparator toolStripSeparator = new ToolStripSeparator();

						LoadToolStripItem( node, toolStripSeparator );

						return toolStripSeparator;
					}
				case "ToolStripButton":
					{
						ToolStripButton toolStripButton = new ToolStripButton();

						LoadToolStripButton( node, toolStripButton );

						return toolStripButton;
					}
				case "ToolStripComboBox":
					{
						ToolStripComboBox toolStripComboBox = new ToolStripComboBox();

						LoadToolStripItem( node, toolStripComboBox );

						return toolStripComboBox;
					}
				default:
					throw new XmlException( string.Format( "Unknown toolstrip item type '{0}'.", node.Name ) );
			}
		}

		protected void LoadToolStripItem( XmlNode node, ToolStripItem toolStripItem )
		{
			if( node.Attributes != null && node.Attributes["Name"] != null )
			{
				toolStripItem.Name = node.Attributes["Name"].Value;
			}
		}

		protected void LoadToolStripButton( XmlNode node, ToolStripButton toolStripButton )
		{
			LoadToolStripItem( node, toolStripButton );

			if( node.Attributes != null && node.Attributes["Image"] != null )
			{
				toolStripButton.Image = Resources.GetIcon( node.Attributes["Image"].Value ).ToBitmap();
			}

			if( node.Attributes != null && node.Attributes["Text"] != null )
			{
				toolStripButton.Text = node.Attributes["Text"].Value;
			}

			if( node.Attributes != null && node.Attributes["ToolTipText"] != null )
			{
				toolStripButton.ToolTipText = node.Attributes["ToolTipText"].Value;
			}
		}
	}
}
