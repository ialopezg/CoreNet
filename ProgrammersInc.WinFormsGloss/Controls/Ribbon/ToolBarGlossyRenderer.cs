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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ToolBarGlossyRenderer : GlossyRenderer
	{
		public ToolBarGlossyRenderer( bool topEdge, bool bottomEdge )
		{
			_topEdge = topEdge;
			_bottomEdge = bottomEdge;
		}

		public override void PaintSection( Context context, Rectangle clip, Rectangle logicalBounds, Section section )
		{
		}

		protected override void LayoutSection
			( Graphics g, Section section, RibbonControl ribbonControl, int levelOfDetail, Dictionary<Section, Rectangle> sectionLogicalBounds
			, Dictionary<Item, Rectangle> itemLogicalBounds, ref int leftPos, ref int rightPos )
		{
			int width = 0, height = 0;

			width = (int) WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, section.Title, SystemFonts.DialogFont, int.MaxValue ).Width + 1;

			int itemsWidth = 0;
			int itemXPos = 0;
			List<int> itemLevelOfDetails = new List<int>();

			for( int i = 0; i < section.Items.Length; ++i )
			{
				Item item = section.Items[i];
				int itemLevelOfDetail = Math.Max( Math.Min( item.Importance - levelOfDetail, 2 ), 0 );

				itemLevelOfDetails.Add( itemLevelOfDetail );
			}

			for( int i = 0; i < section.Items.Length; ++i )
			{
				Item item = section.Items[i];
				int itemLevelOfDetail = itemLevelOfDetails[i];
				Rectangle irect;

				if( item.Visible )
				{
					Size itemSize = LayoutItem( ribbonControl, g, item, itemLevelOfDetail );

					irect = new Rectangle( itemXPos, SectionSep, itemSize.Width, itemSize.Height );

					itemXPos += itemSize.Width + ItemSep;
				}
				else
				{
					irect = Rectangle.Empty;
				}

				itemLogicalBounds[item] = irect;
			}

			itemsWidth = itemXPos - SectionSep;

			height += RowHeight - 1;
			width = Math.Max( width, itemsWidth );
			width += ItemSep;

			int xp;

			switch( section.Alignment )
			{
				case Alignment.Left:
					leftPos += SectionSep;
					xp = leftPos;
					leftPos += width;
					break;
				case Alignment.Right:
					rightPos += width;
					xp = ribbonControl.ClientRectangle.Width + 2 - rightPos;
					rightPos += SectionSep;
					break;
				default:
					throw new InvalidOperationException();
			}

			for( int i = 0; i < section.Items.Length; ++i )
			{
				Item item = section.Items[i];
				Rectangle irect = itemLogicalBounds[item];

				if( item.Visible )
				{
					irect.Offset( xp, 0 );
				}

				itemLogicalBounds[item] = irect;
			}

			Rectangle rect = new Rectangle( xp, SectionSep, width, height );

			sectionLogicalBounds[section] = rect;
		}

		protected override Size LayoutItem( RibbonControl ribbonControl, Graphics g, Item item, int itemLevelOfDetail )
		{
			Size maxItemSize;

			switch( itemLevelOfDetail )
			{
				case 2:
				case 1:
					maxItemSize = new Size( RowHeight * 3 + ItemSep * 2, RowHeight );
					break;
				case 0:
					maxItemSize = new Size( RowHeight, RowHeight );
					break;
				default:
					throw new InvalidOperationException();
			}

			return item.GetLogicalSize( ribbonControl, g, maxItemSize );
		}

		public override void PaintBackground( Context context, Rectangle clip )
		{
			VectorGraphics.Primitives.Container visualItem = CreateRibbonVisualItem( context );

			using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( context ) )
			{
				renderer.Render( context.Graphics, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip ) );
			}
		}

		private VectorGraphics.Primitives.Container CreateRibbonVisualItem( Context context )
		{
			int topOff = _topEdge ? 0 : 1;
			int heightOff = _bottomEdge ? 0 : 1;

			Rectangle clientRect = new Rectangle
				( context.RibbonControl.ClientRectangle.X - 1, context.RibbonControl.ClientRectangle.Y - topOff
				, context.RibbonControl.ClientRectangle.Width + 2, context.RibbonControl.ClientRectangle.Height + heightOff - 1 );
			VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clientRect );
			VectorGraphics.Factories.Rectangle rectangleFactory = new VectorGraphics.Factories.Rectangle();

			VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

			VectorGraphics.Primitives.Path backRect = rectangleFactory.Create( rect );

			backRect.Pen = CreateSectionPen( context, null, 0 );
			backRect.Brush = CreateSectionBrush( context, null, 0, rect );

			container.AddBack( backRect );

			return container;
		}

		private bool _topEdge, _bottomEdge;
	}
}
