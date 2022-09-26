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
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Factories
{
	public class RibbonFactory : WinFormsUtility.Factories.ControlFactory
	{
		public RibbonFactory( Utility.Assemblies.ManifestResources resources )
			: base( resources )
		{
		}

		public void FillRibbon( XmlDocument xmlDoc, Controls.Ribbon.RibbonControl ribbon )
		{
			Dictionary<string, Controls.Ribbon.Section> mapTagToSection = new Dictionary<string, Controls.Ribbon.Section>();
			Dictionary<string, Controls.Ribbon.Item> mapTagToItem = new Dictionary<string, Controls.Ribbon.Item>();

			FillRibbon( xmlDoc, ribbon, out mapTagToSection, out mapTagToItem );
		}

		public void FillRibbon( XmlDocument xmlDoc, Controls.Ribbon.RibbonControl ribbon,
			out Dictionary<string, Controls.Ribbon.Section> mapTagToSection, out Dictionary<string, Controls.Ribbon.Item> mapTagToItem )
		{
			if( xmlDoc == null )
			{
				throw new ArgumentNullException( "xmlDoc" );
			}

			mapTagToSection = new Dictionary<string, Controls.Ribbon.Section>();
			mapTagToItem = new Dictionary<string, Controls.Ribbon.Item>();

			XmlNode ribbonNode = xmlDoc.SelectSingleNode( "/Ribbon" );

			if( ribbonNode == null )
			{
				throw new XmlException( "Failed to find root Ribbon node." );
			}

			foreach( XmlNode sectionNode in ribbonNode.ChildNodes )
			{
				ribbon.AddSection( CreateSection( sectionNode, mapTagToSection, mapTagToItem ) );
			}
		}

		public Controls.Ribbon.Section CreateSection( XmlNode node, Dictionary<string, Controls.Ribbon.Section> mapTagToSection, Dictionary<string, Controls.Ribbon.Item> mapTagToItem )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			Controls.Ribbon.Section section = new Controls.Ribbon.Section();

			if( node.Attributes["Title"] != null )
			{
				section.Title = node.Attributes["Title"].Value;
			}
			if( node.Attributes != null && node.Attributes["Tag"] != null )
			{
				string tagName = node.Attributes["Tag"].Value;

				mapTagToSection[tagName] = section;
			}
			if( node.Attributes["DisplayUntil"] != null )
			{
				section.DisplayUntil = int.Parse( node.Attributes["DisplayUntil"].Value );
			}
			if( node.Attributes["Alignment"] != null )
			{
				section.Alignment = (Controls.Ribbon.Alignment) Enum.Parse( typeof( Controls.Ribbon.Alignment ), node.Attributes["Alignment"].Value );
			}

			foreach( XmlNode childNode in node.ChildNodes )
			{
				if( childNode.Name == "#comment" )
				{
					continue;
				}

				section.AddItem( CreateSingleItem( childNode, mapTagToSection, mapTagToItem ) );
			}

			return section;
		}

		protected virtual Controls.Ribbon.Item CreateSingleItem( XmlNode node, Dictionary<string, Controls.Ribbon.Section> mapTagToSection, Dictionary<string, Controls.Ribbon.Item> mapTagToItem )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			switch( node.Name )
			{
				case "DropDownButton":
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

						Controls.Ribbon.DropDownButtonItem item = new Controls.Ribbon.DropDownButtonItem( text, icon16, icon24 );

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

						return item;
					}
				case "ProgressBar":
					{
						Controls.Ribbon.ProgressBarItem item = new Controls.Ribbon.ProgressBarItem();

						if( node.Attributes != null && node.Attributes["Tag"] != null )
						{
							string tagName = node.Attributes["Tag"].Value;

							mapTagToItem[tagName] = item;
						}

						return item;
					}
				case "ToolBarVerticalSeparator":
					{
						return new Controls.Ribbon.ToolBarVerticalSeparator();
					}
				case "Null":
					{
						Controls.Ribbon.NullItem item = new Controls.Ribbon.NullItem();

						if( node.Attributes != null && node.Attributes["Tag"] != null )
						{
							string tagName = node.Attributes["Tag"].Value;

							mapTagToItem[tagName] = item;
						}

						return item;
					}
				default:
					throw new InvalidOperationException( string.Format( "Unknown item type '{0}'.", node.Name ) );
			}
		}
	}
}
