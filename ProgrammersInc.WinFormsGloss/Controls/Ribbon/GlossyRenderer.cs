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
using System.Windows.Forms;
using System.Diagnostics;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class GlossyRenderer : Renderer
	{
		public GlossyRenderer()
		{
		}

		public override bool Layout
			( Graphics g, RibbonControl ribbonControl, Section[] sections, int levelOfDetail, int leftPos, int rightPos
			, out Dictionary<Section, Rectangle> sectionLogicalBounds, out Dictionary<Item, Rectangle> itemLogicalBounds, out int ribbonHeight, out int widthRequired )
		{
			foreach( Section section in sections )
			{
				section.PreLayout( ribbonControl, levelOfDetail );
			}

			sectionLogicalBounds = new Dictionary<Section, Rectangle>();
			itemLogicalBounds = new Dictionary<Item, Rectangle>();

			ribbonHeight = 0;

			foreach( Section section in sections )
			{
				if( ribbonControl.IsSectionVisible( section, levelOfDetail ) )
				{
					LayoutSection( g, section, ribbonControl, levelOfDetail, sectionLogicalBounds, itemLogicalBounds, ref leftPos, ref rightPos );
				}
				else
				{
					sectionLogicalBounds[section] = Rectangle.Empty;

					foreach( Item item in section.Items )
					{
						itemLogicalBounds[item] = Rectangle.Empty;
					}
				}
			}

			foreach( Rectangle rect in sectionLogicalBounds.Values )
			{
				ribbonHeight = Math.Max( ribbonHeight, rect.Height + SectionSep * 2 + 1 );
			}

			widthRequired = leftPos + rightPos + SectionSep * 2;

			return leftPos + rightPos < ribbonControl.ClientRectangle.Width - SectionSep * 2;
		}

		public override Rectangle GetVisualBounds( Graphics g, Section section, Rectangle logicalBounds )
		{
			return logicalBounds;
		}

		public override Rectangle GetVisualBounds( Graphics g, Item item, Rectangle logicalBounds )
		{
			return new Rectangle( logicalBounds.X - Glow, logicalBounds.Y - Glow, logicalBounds.Width + Glow * 2, logicalBounds.Height + Glow * 2 );
		}

		public override void PaintBackground( Context context, Rectangle clip )
		{
			if( context.RibbonControl.Glass == null || context.RibbonControl.Glass.Ignore( context.RibbonControl ) )
			{
				Color color;

				if( context.RibbonControl.FindForm() == Form.ActiveForm )
				{
					color = context.RibbonControl.ColorTable.PrimaryBackgroundColor;
				}
				else
				{
					color = context.RibbonControl.ColorTable.GrayPrimaryBackgroundColor;
				}

				using( Brush brush = new SolidBrush( color ) )
				{
					context.Graphics.FillRectangle( brush, new Rectangle( 0, 0, 5, 5 ) );
					context.Graphics.FillRectangle( brush, new Rectangle( context.RibbonControl.Width - 5, 0, 5, 5 ) );
					context.Graphics.FillRectangle( brush, new Rectangle( 0, context.RibbonControl.Height - 5, 5, 5 ) );
					context.Graphics.FillRectangle( brush, new Rectangle( context.RibbonControl.Width - 5, context.RibbonControl.Height - 5, 5, 5 ) );
				}

				VectorGraphics.Primitives.Container visualItem = CreateRibbonVisualItem( context );

				using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( context ) )
				{
					renderer.Render( context.Graphics, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip ) );
				}
			}
		}

		public override void PaintSection( Context context, Rectangle clip, Rectangle logicalBounds, Section section )
		{
			if( logicalBounds == Rectangle.Empty )
			{
				return;
			}

			VectorGraphics.Primitives.Container visualItem = CreateSectionVisualItem( context, logicalBounds, section );

			if( context.RibbonControl.Glass != null )
			{
				VectorGraphics.Factories.Transparency transparency = new VectorGraphics.Factories.Transparency();

				transparency.Apply( visualItem, 0.85 );
			}

			using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( context ) )
			{
				renderer.Render( context.Graphics, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip ) );
			}
		}

		public override void PaintItemBackground( Context context, Rectangle clip, Rectangle logicalBounds, Item item, BackgroundStyle backgroundStyle )
		{
			if( logicalBounds == Rectangle.Empty )
			{
				return;
			}

			VectorGraphics.Primitives.Container visualItem = CreateItemVisualItem( context, logicalBounds, item, backgroundStyle );

			using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( context ) )
			{
				renderer.Render( context.Graphics, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip ) );
			}
		}

		public override void PaintProgressBar( Context context, Rectangle clip, Rectangle logicalBounds, int percentage )
		{
			PaintItemBackground( context, clip, logicalBounds, null, BackgroundStyle.Normal );

			const int barSize = 10;

			int numBars = (int) ((logicalBounds.Width - 4) * percentage / 100.0 / barSize);

			using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( context ) )
			{
				for( int i = 0; i < numBars; ++i )
				{
					Rectangle barBounds = new Rectangle( logicalBounds.X + i * barSize + 3, logicalBounds.Y + 2, barSize - 2, logicalBounds.Height - 4 );
					VectorGraphics.Primitives.Container visualItem = CreateItemVisualItem( context, barBounds, null, 1, BackgroundStyle.Normal );

					renderer.Render( context.Graphics, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clip ) );
				}
			}
		}

		public override double GetFade( Context context, Item item )
		{
			return GetFade( context, item, FadeIn, FadeOut );
		}

		protected virtual void LayoutSection
			( Graphics g, Section section, RibbonControl ribbonControl, int levelOfDetail, Dictionary<Section, Rectangle> sectionLogicalBounds
			, Dictionary<Item, Rectangle> itemLogicalBounds, ref int leftPos, ref int rightPos )
		{
			int width = 0, height = 0;

			height += SystemFonts.DialogFont.Height + ItemSep;
			width = (int) WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, section.Title, SystemFonts.DialogFont, int.MaxValue ).Width + 1;

			int itemsWidth = 0;
			int topItemXPos = leftPos + SectionSep, bottomItemXPos = leftPos + SectionSep;
			Queue<KeyValuePair<int, int>> gaps = new Queue<KeyValuePair<int, int>>();
			List<int> itemLevelOfDetails = new List<int>();
			int zeroCount = 0, oneCount = 0, twoCount = 0;

			for( int i = 0; i < section.Items.Length; ++i )
			{
				Item item = section.Items[i];
				int itemLevelOfDetail = Math.Max( Math.Min( item.Importance - levelOfDetail, 2 ), 0 );

				switch( itemLevelOfDetail )
				{
					case 0:
						++zeroCount;
						break;
					case 1:
						++oneCount;
						break;
					case 2:
						++twoCount;
						break;
				}

				itemLevelOfDetails.Add( itemLevelOfDetail );
			}

			bool alreadyFixed = false;

			if( section.Items.Length == 2 && itemLevelOfDetails[0] == 0 && itemLevelOfDetails[1] == 0 )
			{
				itemLevelOfDetails[0] = 1;
				itemLevelOfDetails[1] = 1;
				zeroCount -= 2;
				oneCount += 2;
				alreadyFixed = true;
			}
			else if( oneCount == 0 && zeroCount == 1 )
			{
				for( int i = 0; i < itemLevelOfDetails.Count; ++i )
				{
					if( itemLevelOfDetails[i] == 0 )
					{
						itemLevelOfDetails[i] = 2;
						--zeroCount;
						++twoCount;
					}
				}
				alreadyFixed = true;
			}
			else if( section.Items.Length == 3 && itemLevelOfDetails[0] == 0 && itemLevelOfDetails[1] == 0 && itemLevelOfDetails[2] == 0 )
			{
				itemLevelOfDetails[2] = 2;
				--zeroCount;
				++twoCount;
				alreadyFixed = true;
			}

			if( zeroCount == 1 && oneCount == 1 )
			{
				for( int i = 0; i < itemLevelOfDetails.Count; ++i )
				{
					if( itemLevelOfDetails[i] == 1 )
					{
						itemLevelOfDetails[i] = 2;
						--oneCount;
						++twoCount;
					}
				}
				alreadyFixed = true;
			}

			if( twoCount == 0 && oneCount == 1 && zeroCount >= 4 && (zeroCount % 2) == 0 )
			{
				for( int i = 0; i < itemLevelOfDetails.Count; ++i )
				{
					if( itemLevelOfDetails[i] == 1 )
					{
						itemLevelOfDetails[i] = 2;
						--oneCount;
						++twoCount;
					}
				}
				alreadyFixed = true;
			}

			if( oneCount == 3 && zeroCount == 1 )
			{
				for( int i = 0; i < itemLevelOfDetails.Count; ++i )
				{
					if( itemLevelOfDetails[i] == 0 )
					{
						itemLevelOfDetails[i] = 1;
						--zeroCount;
						++oneCount;
					}
				}
				alreadyFixed = true;
			}

			for( int i = 0; i < section.Items.Length; ++i )
			{
				Item item = section.Items[i];
				int itemLevelOfDetail = itemLevelOfDetails[i];
				int itemXPos, itemYPos;
				Rectangle irect;
				bool nextIsFullHeight = false;
				Item nextItem;
				int nextItemLevelOfDetail;

				if( i == section.Items.Length - 1 )
				{
					nextItem = null;
					nextIsFullHeight = true;
					nextItemLevelOfDetail = -1;
				}
				else
				{
					nextItem = section.Items[i + 1];
					nextItemLevelOfDetail = itemLevelOfDetails[i + 1];

					Size nextItemSize = LayoutItem( ribbonControl, g, nextItem, nextItemLevelOfDetail );

					if( nextItemSize.Height > RowHeight )
					{
						nextIsFullHeight = true;
					}
				}

				if( nextIsFullHeight && !alreadyFixed && itemLevelOfDetail == 1 && topItemXPos == bottomItemXPos )
				{
					itemLevelOfDetail = 2;
				}

				Size itemSize = LayoutItem( ribbonControl, g, item, itemLevelOfDetail );
				bool gapped = false;

				if( itemLevelOfDetail == 0 && gaps.Count > 0 )
				{
					KeyValuePair<int, int> gap = gaps.Dequeue();

					itemXPos = gap.Key;
					itemYPos = gap.Value;
					gapped = true;
				}
				else if( itemSize.Height > RowHeight )
				{
					if( topItemXPos > bottomItemXPos + RowHeight )
					{
						gaps.Enqueue( new KeyValuePair<int, int>( bottomItemXPos, SectionSep + ItemSep * 2 + RowHeight ) );
					}
					else if( bottomItemXPos > topItemXPos + RowHeight )
					{
						gaps.Enqueue( new KeyValuePair<int, int>( topItemXPos, SectionSep + ItemSep ) );
					}

					itemXPos = Math.Max( topItemXPos, bottomItemXPos );
					topItemXPos = bottomItemXPos = itemXPos;
					itemYPos = SectionSep + ItemSep;
				}
				else
				{
					itemXPos = Math.Min( topItemXPos, bottomItemXPos );

					if( topItemXPos <= bottomItemXPos )
					{
						itemYPos = SectionSep + ItemSep;
					}
					else
					{
						itemYPos = SectionSep + ItemSep * 2 + RowHeight;
					}
				}

				itemXPos += ItemSep;

				irect = new Rectangle( itemXPos, itemYPos, itemSize.Width, itemSize.Height );

				if( !gapped )
				{
					if( itemSize.Height > RowHeight )
					{
						topItemXPos += itemSize.Width + ItemSep;
						bottomItemXPos += itemSize.Width + ItemSep;
					}
					else
					{
						if( topItemXPos <= bottomItemXPos )
						{
							topItemXPos += itemSize.Width + ItemSep;
						}
						else
						{
							bottomItemXPos += itemSize.Width + ItemSep;
						}
					}
				}

				itemLogicalBounds[item] = irect;
			}

			itemsWidth = Math.Max( topItemXPos - leftPos - SectionSep, bottomItemXPos - leftPos - SectionSep );

			height += RowHeight * 2 + 4 + ItemSep * 2;
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
					rightPos += SectionSep;
					rightPos += width;
					xp = ribbonControl.ClientRectangle.Width - rightPos;
					break;
				default:
					throw new InvalidOperationException();
			}

			Rectangle rect = new Rectangle( xp, SectionSep, width, height );

			sectionLogicalBounds[section] = rect;
		}

		protected virtual Size LayoutItem( RibbonControl ribbonControl, Graphics g, Item item, int itemLevelOfDetail )
		{
			Size maxItemSize;

			switch( itemLevelOfDetail )
			{
				case 2:
					maxItemSize = new Size( RowHeight * 2 + ItemSep, RowHeight * 2 + ItemSep );
					break;
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

		protected VectorGraphics.Paint.Brushes.Brush CreateSectionBrush( Context context, Section section, double glow, VectorGraphics.Types.Rectangle rect )
		{
			VectorGraphics.Factories.GlossyBrush glossyBrushFactory = CreateGlossyBrushFactory( context, section, glow / 2 );
			VectorGraphics.Paint.Color primaryColor = GetPrimaryBackgroundColor( context, section );
			VectorGraphics.Paint.Color lightener = GetLightenerColor( context, section, 0 );

			return glossyBrushFactory.Create( VectorGraphics.Paint.Color.Combine( primaryColor, lightener, 1 - glow / 3 ), rect.Top, rect.Bottom );
		}

		protected VectorGraphics.Renderers.GdiPlusRenderer CreateRenderer( Context context )
		{
			return new VectorGraphics.Renderers.GdiPlusRenderer( delegate
			{
				return context.Graphics;
			}, VectorGraphics.Renderers.GdiPlusRenderer.MarkerHandling.Throw, 5.0 );
		}

		protected VectorGraphics.Paint.Pens.Pen CreateSectionPen( Context context, Section section, double glow )
		{
			VectorGraphics.Paint.Color primaryColor = GetPrimaryBorderColor( context, section );
			VectorGraphics.Paint.Color lightner = GetLightenerColor( context, section, glow );

			return new VectorGraphics.Paint.Pens.SolidPen( VectorGraphics.Paint.Color.Combine( primaryColor, lightner, 0.7 - glow / 3 ), 1 );
		}

		private VectorGraphics.Primitives.Container CreateRibbonVisualItem( Context context )
		{
			Rectangle clientRect = new Rectangle
				( context.RibbonControl.ClientRectangle.X, context.RibbonControl.ClientRectangle.Y
				, context.RibbonControl.ClientRectangle.Width - 1, context.RibbonControl.ClientRectangle.Height - 1 );
			VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( clientRect );
			VectorGraphics.Factories.RoundedRectangle roundedRectangleFactory = new VectorGraphics.Factories.RoundedRectangle();

			VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

			VectorGraphics.Primitives.Path roundedRect = roundedRectangleFactory.Create( rect, 3 );

			roundedRect.Pen = CreateSectionPen( context, null, 0 );
			roundedRect.Brush = CreateSectionBrush( context, null, 0, rect );

			container.AddBack( roundedRect );

			VectorGraphics.Primitives.Path roundRectHighlight =
				CreateRoundRectHighlight( context, new VectorGraphics.Types.Rectangle( rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 ), 2 );

			container.AddBack( roundRectHighlight );

			return container;
		}

		private VectorGraphics.Primitives.Container CreateSectionVisualItem( Context context, Rectangle logicalBounds, Section section )
		{
			double glow = GetFade( context, logicalBounds, section, FadeIn, FadeOut );
			VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( logicalBounds );
			VectorGraphics.Factories.RoundedRectangle roundedRectangleFactory = new VectorGraphics.Factories.RoundedRectangle();

			VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

			VectorGraphics.Primitives.Path roundedRect = roundedRectangleFactory.Create( rect, 3 );

			roundedRect.Pen = CreateSectionPen( context, section, glow );
			roundedRect.Brush = CreateSectionBrush( context, section, glow, rect );

			container.AddBack( roundedRect );

			VectorGraphics.Primitives.Path roundRectHighlight =
				CreateRoundRectHighlight( context, new VectorGraphics.Types.Rectangle( rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 ), 2 );

			container.AddBack( roundRectHighlight );

			double textHeight = WinFormsUtility.Drawing.GdiPlusEx.MeasureString( context.Graphics, section.Title, SystemFonts.DialogFont, int.MaxValue ).Height;
			VectorGraphics.Primitives.Path titleBackground = roundedRectangleFactory.Create
				( new VectorGraphics.Types.Rectangle( rect.X, rect.Bottom - textHeight - 2, rect.Width, textHeight + 1 ), 2
				, VectorGraphics.Factories.RoundedRectangle.Corners.BottomLeft | VectorGraphics.Factories.RoundedRectangle.Corners.BottomRight );
			VectorGraphics.Paint.Color titleColor = GetPrimaryBackgroundColor( context, section );
			VectorGraphics.Paint.Color lightener = GetLightenerColor( context, section, glow );

			titleColor = VectorGraphics.Paint.Color.Combine( titleColor, lightener, 1 - glow / 7 );

			titleBackground.Pen = null;
			titleBackground.Brush = new VectorGraphics.Paint.Brushes.SolidBrush( titleColor );

			container.AddBack( titleBackground );

			VectorGraphics.Primitives.Text text = new VectorGraphics.Primitives.Text
				( section.Title, new VectorGraphics.Types.Point( rect.X + rect.Width / 2 + 1, rect.Bottom - 2 ), VectorGraphics.Primitives.Text.Position.BottomCenter );

			SetSectionFont( context, section, text );

			container.AddBack( text );

			return container;
		}

		private VectorGraphics.Primitives.Container CreateItemVisualItem( Context context, Rectangle logicalBounds, Item item, BackgroundStyle backgroundStyle )
		{
			double glow = item == null ? 0 : GetFade( context, item, FadeIn, FadeOut );

			if( backgroundStyle == BackgroundStyle.Disabled )
			{
				glow = 0;
			}

			return CreateItemVisualItem( context, logicalBounds, item, glow, backgroundStyle );
		}

		private VectorGraphics.Primitives.Container CreateItemVisualItem( Context context, Rectangle logicalBounds, Item item, double glow, BackgroundStyle backgroundStyle )
		{
			double overGlow = glow;

			if( logicalBounds.Height == RowHeight )
			{
				overGlow = 1;
			}

			VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( logicalBounds );
			VectorGraphics.Factories.RoundedRectangle roundedRectangleFactory = new VectorGraphics.Factories.RoundedRectangle();

			VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

			VectorGraphics.Primitives.Path roundedRect = roundedRectangleFactory.Create( rect, 3 );

			roundedRect.Pen = CreateItemPen( context, item, glow, overGlow, backgroundStyle );
			roundedRect.Brush = CreateItemBrush( context, item, glow, overGlow, rect, backgroundStyle );

			container.AddBack( roundedRect );

			VectorGraphics.Primitives.Path lightRoundedRect = roundedRectangleFactory.Create
				( new VectorGraphics.Types.Rectangle( rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 ), 2 );
			VectorGraphics.Paint.Color lightener = GetLightenerColor( context, null, glow );

			lightRoundedRect.Pen = new VectorGraphics.Paint.Pens.SolidPen( new VectorGraphics.Paint.Color( lightener, 0.4 * overGlow ), 1 );
			lightRoundedRect.Brush = null;

			container.AddBack( lightRoundedRect );

			if( glow > 0 )
			{
				VectorGraphics.Primitives.Path glowRoundedRect = roundedRectangleFactory.Create
					( new VectorGraphics.Types.Rectangle( rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2 ), 4 );
				VectorGraphics.Paint.Color glowEndColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowHighlightColor );

				glowEndColor = new VectorGraphics.Paint.Color( glowEndColor, overGlow );

				glowRoundedRect.Pen = new VectorGraphics.Paint.Pens.SolidPen
					( new VectorGraphics.Paint.Color( glowEndColor.Red, glowEndColor.Green, glowEndColor.Blue, glow / 2 ), 1 );
				glowRoundedRect.Brush = null;

				container.AddBack( glowRoundedRect );
			}

			return container;
		}

		private VectorGraphics.Paint.Color GetPrimaryBackgroundColor( Context context, Section section )
		{
			return VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.PrimaryBackgroundColor );
		}

		private VectorGraphics.Paint.Color GetPrimaryForegroundColor( Context context, Section section )
		{
			return VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.PrimaryColor );
		}

		private VectorGraphics.Paint.Color GetLightenerColor( Context context, Section section, double glow )
		{
			Color lightener = WinFormsUtility.Drawing.ColorUtil.Combine
				( context.RibbonControl.ColorTable.GlossyLightenerColor, context.RibbonControl.ColorTable.GlossyGlowLightenerColor, 1 - glow );
			
			return VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( lightener );
		}

		private VectorGraphics.Paint.Color GetPrimaryBorderColor( Context context, Section section )
		{
			return VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.PrimaryBorderColor );
		}

		private void SetSectionFont( Context context, Section section, VectorGraphics.Primitives.Text text )
		{
			VectorGraphics.Paint.Color titleColor = GetPrimaryBackgroundColor( context, section );
			VectorGraphics.Paint.Color textColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.TextColor );

			titleColor = VectorGraphics.Paint.Color.Combine( titleColor, textColor, 0.5 );

			text.FontFamily = SystemFonts.DialogFont.FontFamily.Name;
			text.FontSizePoints = SystemFonts.DialogFont.SizeInPoints;
			text.Color = titleColor;
		}

		private VectorGraphics.Factories.GlossyBrush CreateGlossyBrushFactory( Context context, double glow )
		{
			return new VectorGraphics.Factories.GlossyBrush( GetLightenerColor( context, null, glow ) );
		}

		private VectorGraphics.Factories.GlossyBrush CreateGlossyBrushFactory( Context context, Section section, double glow )
		{
			return new VectorGraphics.Factories.GlossyBrush( GetLightenerColor( context, section, glow ) );
		}

		private VectorGraphics.Paint.Pens.Pen CreateItemPen( Context context, Item item, double glow, double overGlow, BackgroundStyle backgroundStyle )
		{
			VectorGraphics.Paint.Color primaryColor = GetPrimaryForegroundColor( context, item == null ? null : item.Section );
			VectorGraphics.Paint.Color glowColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowColor );

			primaryColor = VectorGraphics.Paint.Color.Combine( primaryColor, VectorGraphics.Paint.Color.White, 0.7 );

			if( backgroundStyle == BackgroundStyle.Disabled )
			{
				primaryColor = VectorGraphics.Paint.Color.Combine( primaryColor, VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GrayBackgroundColor ), 0.2 );
			}
			else
			{
				if( (backgroundStyle & BackgroundStyle.Pressed) != 0 )
				{
					glowColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowDeepColor );
				}
			}

			primaryColor = new VectorGraphics.Paint.Color( primaryColor, overGlow );
			glowColor = new VectorGraphics.Paint.Color( glowColor, overGlow );

			return new VectorGraphics.Paint.Pens.SolidPen( VectorGraphics.Paint.Color.Combine( primaryColor, glowColor, 1 - glow ), 1 );
		}

		private VectorGraphics.Paint.Brushes.Brush CreateItemBrush( Context context, Item item, double glow, double overGlow, VectorGraphics.Types.Rectangle rect, BackgroundStyle backgroundStyle )
		{
			VectorGraphics.Factories.GlossyBrush glossyBrushFactory = CreateGlossyBrushFactory( context, glow );
			VectorGraphics.Paint.Color primaryColor = GetPrimaryBackgroundColor( context, item == null ? null : item.Section );
			VectorGraphics.Paint.Color glowStartColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowColor );
			VectorGraphics.Paint.Color glowEndColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowHighlightColor );

			if( backgroundStyle == BackgroundStyle.Disabled )
			{
				primaryColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GrayPrimaryBackgroundColor );
				glowEndColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GrayPrimaryBackgroundColor );
			}
			else
			{
				if( (backgroundStyle & BackgroundStyle.Pressed) != 0 )
				{
					glowStartColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowDeepColor );
					glowEndColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( context.RibbonControl.ColorTable.GlowDeepColor );
				}
			}

			VectorGraphics.Paint.Color lightener = GetLightenerColor( context, null, glow );
			VectorGraphics.Paint.Color primaryEnd = VectorGraphics.Paint.Color.Combine( primaryColor, lightener, 0.6 );

			primaryColor = new VectorGraphics.Paint.Color( primaryColor, overGlow );
			primaryEnd = new VectorGraphics.Paint.Color( primaryEnd, overGlow );
			glowStartColor = new VectorGraphics.Paint.Color( glowStartColor, overGlow );
			glowEndColor = new VectorGraphics.Paint.Color( glowEndColor, overGlow );

			return glossyBrushFactory.Create
				( VectorGraphics.Paint.Color.Combine( primaryColor, glowStartColor, 1 - glow )
				, VectorGraphics.Paint.Color.Combine( primaryEnd, glowEndColor, 1 - glow )
				, rect.Top, rect.Bottom );
		}

		private VectorGraphics.Primitives.Path CreateRoundRectHighlight( Context context, VectorGraphics.Types.Rectangle rect, double radius )
		{
			VectorGraphics.Primitives.Path path = new VectorGraphics.Primitives.Path();
			VectorGraphics.Paint.Color lightener = GetLightenerColor( context, null, 0 );

			path.Add( new VectorGraphics.Primitives.Path.Move( new VectorGraphics.Types.Point( rect.X, rect.Y + rect.Height - radius ) ) );
			path.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( rect.X, rect.Y + radius ) ) );
			path.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( rect.X + radius, rect.Y ) ) );
			path.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( rect.X + rect.Width - radius, rect.Y ) ) );

			path.Pen = new VectorGraphics.Paint.Pens.SolidPen( new VectorGraphics.Paint.Color( lightener, 0.6 ), 1 );

			return path;
		}

		protected const double FadeOut = 0.3;
		protected const double FadeIn = 0.1;
		protected const int Glow = 1;
		protected const int ItemSep = 4;
		protected const int SectionSep = 3;
		protected const int RowHeight = 23;
	}
}
