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
	public abstract class Renderer
	{
		[Flags]
		public enum BackgroundStyle
		{
			Normal = 0,
			Pressed = 1,
			Checked = 2,
			Disabled = 4
		}

		protected Renderer()
		{
		}

		public abstract bool Layout
			( Graphics g, RibbonControl ribbonControl, Section[] sections, int levelOfDetail, int leftPos, int rightPos
			, out Dictionary<Section, Rectangle> sectionLogicalBounds, out Dictionary<Item, Rectangle> itemLogicalBounds, out int ribbonHeight, out int widthRequired );

		public abstract Rectangle GetVisualBounds( Graphics g, Section section, Rectangle logicalBounds );
		public abstract Rectangle GetVisualBounds( Graphics g, Item item, Rectangle logicalBounds );

		public abstract void PaintBackground( Context context, Rectangle clip );
		public abstract void PaintSection( Context context, Rectangle clip, Rectangle logicalBounds, Section section );
		public abstract void PaintItemBackground( Context context, Rectangle clip, Rectangle logicalBounds, Item item, BackgroundStyle backgroundStyle );

		public abstract void PaintProgressBar( Context context, Rectangle clip, Rectangle logicalBounds, int percentage );

		public abstract double GetFade( Context context, Item item );

		protected double GetFade( Context context, Item item, double fadeIn, double fadeOut )
		{
			double fadeInGlow = 0, fadeOutGlow = 0;

			if( context.History != null && context.History.MouseOverItem == item )
			{
				double fadeInTime = context.History.GetTimeOverItem( item ).Value;

				if( fadeInTime < fadeIn )
				{
					context.Updates.NeedsUpdate( item );
					fadeInGlow = fadeInTime / fadeIn;
				}
				else
				{
					fadeInGlow = 1;
				}
			}

			if( context.History != null )
			{
				double fadeOutTime = context.History.GetLastOverItem( item );

				if( fadeOutTime < fadeOut )
				{
					context.Updates.NeedsUpdate( item );
					fadeOutGlow = 1 - fadeOutTime / fadeOut;
				}
			}

			return Math.Min( Math.Max( 0, Math.Max( fadeInGlow, fadeOutGlow ) ), 1 );
		}

		protected double GetFade( Context context, Rectangle logicalBounds, Section section, double fadeIn, double fadeOut )
		{
			double fadeInGlow = 0, fadeOutGlow = 0;
			Rectangle visualBounds = GetVisualBounds( context.Graphics, section, logicalBounds );

			if( context.History.MouseOverSection == section )
			{
				double fadeInTime = context.History.GetTimeOverSection( section ).Value;

				if( fadeInTime < fadeIn )
				{
					context.Updates.NeedsUpdate( section );
					fadeInGlow = fadeInTime / fadeIn;
				}
				else
				{
					fadeInGlow = 1;
				}
			}

			double fadeOutTime = context.History.GetLastOverSection( section );

			if( fadeOutTime < fadeOut )
			{
				context.Updates.NeedsUpdate( section );
				fadeOutGlow = 1 - fadeOutTime / fadeOut;
			}

			return Math.Max( fadeInGlow, fadeOutGlow );
		}
	}
}
