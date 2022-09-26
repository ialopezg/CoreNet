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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ExtrasSection : Section
	{
		public ExtrasSection()
		{
			Title = "...";

			AddItem( new ExtrasButtonItem() );
		}

		public override void PreLayout( RibbonControl ribbonControl, int levelOfDetail )
		{
			base.PreLayout( ribbonControl, levelOfDetail );

			bool haveRemovedSections = false;

			foreach( Section section in ribbonControl.Sections )
			{
				if( section == this )
				{
					continue;
				}

				if( levelOfDetail > section.DisplayUntil )
				{
					haveRemovedSections = true;
				}
			}

			Visible = haveRemovedSections;
		}

		private sealed class ExtrasButtonItem : Item
		{
			public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
			{
				return new Size( 8, suggestedSize.Height );
			}

			public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
			{
				Renderer.BackgroundStyle backgroundStyle = Renderer.BackgroundStyle.Normal;

				if( context.History.MouseOverItem == this )
				{
					if( logicalBounds.Contains( context.RibbonControl.PointToClient( Control.MousePosition ) ) )
					{
						if( Control.MouseButtons == MouseButtons.Left )
						{
							backgroundStyle |= Renderer.BackgroundStyle.Pressed;
						}
					}
				}

				context.Renderer.PaintItemBackground( context, clip, logicalBounds, this, backgroundStyle );
			}

			protected override bool OnClick( Context context )
			{
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				Section lastSection = null;

				foreach( Section section in context.RibbonControl.Sections )
				{
					if( context.RibbonControl.LevelOfDetail <= section.DisplayUntil )
					{
						continue;
					}

					foreach( Item item in section.Items )
					{
						System.Windows.Forms.ToolStripItem menuItem = item.CreateEquivalentToolStripItem();

						if( menuItem != null )
						{
							if( lastSection != null && lastSection != section )
							{
								contextMenu.Items.Add( new ToolStripSeparator() );
							}

							contextMenu.Items.Add( menuItem );
							lastSection = section;
						}
					}
				}

				Rectangle itemRect = context.GetItemBounds( this );

				contextMenu.Show( context.RibbonControl, new Point( itemRect.X, itemRect.Bottom ) );

				return true;
			}
		}
	}
}
