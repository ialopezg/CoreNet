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
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ProgrammersInc.SuperTree.Renderers
{
	public class StandardRenderer : IRenderer
	{
		#region IRenderer Members

		public void Setup()
		{
		}

		public void Setdown()
		{
		}

		public void PreRender( ITreeInfo treeInfo, ITreeEvents treeEvents )
		{
		}

		public void RenderBackground( Graphics g, Rectangle clip )
		{
			g.Clear( SystemColors.Window );
		}

		public int MeasureIndent( Graphics g, ITreeInfo treeInfo, TreeNode node )
		{
			return node.Depth * _indent;
		}

		public Size MeasureTreeNode( Graphics g, ITreeInfo treeInfo, TreeNode treeNode, bool needsWidth, bool needsHeight )
		{
			Size textSize = WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, treeNode.Text, treeNode.Font, int.MaxValue );
			Size ecSize = GetGlyphSize( g, treeNode.IsExpanded );

			int width = (int) textSize.Width + _leftSep + _ecSep + ecSize.Width + _rightSep;
			int height = (int) textSize.Height;

			if( treeNode.Icon != null )
			{
				width += _imageSep + treeNode.Icon.Size.Width;
				height = Math.Max( height, treeNode.Icon.Size.Height );
			}

			height += _verticalSep;

			return new Size( width, height );
		}

		public void RenderTreeNode( Graphics g, ITreeInfo treeInfo, TreeNode treeNode, Rectangle nodeRectangle, Rectangle clip )
		{
			bool isLast = (treeNode.Index == treeNode.ParentCollection.Count - 1);
			Size ecSize = GetGlyphSize( g, treeNode.IsExpanded );
			Point ecCenter = new Point( nodeRectangle.X + _leftSep + ecSize.Width / 2, nodeRectangle.Y + (nodeRectangle.Height - ecSize.Height) / 2 + ecSize.Height / 2 );

			using( Brush brush = new HatchBrush( HatchStyle.Percent50, SystemColors.Window, SystemColors.GrayText ) )
			using( Pen pen = new Pen( brush ) )
			{
				g.DrawLine( pen, ecCenter, new Point( ecCenter.X + 12, ecCenter.Y ) );
				g.DrawLine( pen, ecCenter, new Point( ecCenter.X, nodeRectangle.Y ) );

				if( !isLast )
				{
					g.DrawLine( pen, ecCenter, new Point( ecCenter.X, nodeRectangle.Bottom ) );
				}
			}

			int textX = nodeRectangle.X + ecSize.Width + _leftSep + _ecSep;

			if( treeNode.Icon != null )
			{
				Icon image = treeNode.Icon;

				g.DrawIconUnstretched( image, new Rectangle( textX, nodeRectangle.Y + (nodeRectangle.Height - image.Height) / 2, image.Width, image.Height ) );

				textX += image.Width + _imageSep;
			}

			if( treeInfo.IsSelected( treeNode ) )
			{
				Brush brush = treeInfo.IsTreeFocused() ? SystemBrushes.Highlight : SystemBrushes.Control;

				g.FillRectangle( brush, textX, nodeRectangle.Y, nodeRectangle.Right - textX + 2, nodeRectangle.Height - 1 );

				if( treeInfo.IsTreeFocused() )
				{
					using( Brush hatchBrush = new HatchBrush( HatchStyle.Percent50, SystemColors.Highlight ) )
					using( Pen pen = new Pen( hatchBrush ) )
					{
						g.DrawRectangle( pen, textX, nodeRectangle.Y, nodeRectangle.Right - textX + 2, nodeRectangle.Height - 1 );
					}
				}
			}

			WinFormsUtility.Drawing.GdiPlusEx.DrawString
				( g, treeNode.Text, treeNode.Font, SystemColors.ControlText
				, new Rectangle( textX + 2, nodeRectangle.Y + _verticalSep, int.MaxValue, int.MaxValue )
				, WinFormsUtility.Drawing.GdiPlusEx.TextSplitting.SingleLineEllipsis, WinFormsUtility.Drawing.GdiPlusEx.Ampersands.Display );

			if( treeNode.ChildNodes.Count > 0 )
			{
				DrawGlyph( g, new Point( nodeRectangle.X + _leftSep, nodeRectangle.Y + (nodeRectangle.Height - ecSize.Height) / 2 ), treeNode.IsExpanded );
			}
		}

		public void RenderTreeNodeRow( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Rectangle rowRectangle )
		{
			Size ecSize = GetGlyphSize( g, treeNode.IsExpanded );
			TreeNode parent = treeNode.ParentCollection.ParentNode;

			using( Brush brush = new HatchBrush( HatchStyle.Percent50, SystemColors.Window, SystemColors.GrayText ) )
			using( Pen pen = new Pen( brush ) )
			{
				while( parent != null )
				{
					bool isLast = (parent.Index == parent.ParentCollection.Count - 1);

					if( !isLast )
					{
						g.DrawLine
							( pen
							, new Point( parent.Depth * _indent + _leftSep + ecSize.Width / 2, rowRectangle.Y )
							, new Point( parent.Depth * _indent + _leftSep + ecSize.Width / 2, rowRectangle.Bottom ) );
					}

					parent = parent.ParentCollection.ParentNode;
				}
			}
		}

		public void ProcessClick( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Point p, ITreeInfo treeInfo, ITreeEvents treeEvents )
		{
			Size ecSize = GetGlyphSize( g, treeNode.IsExpanded );
			Rectangle ecBounds = new Rectangle( nodeRectangle.X + _leftSep, nodeRectangle.Y + (nodeRectangle.Height - ecSize.Height) / 2, ecSize.Width, ecSize.Height );

			if( ecBounds.Contains( p ) && treeNode.ChildNodes.Count > 0 )
			{
				if( !treeInfo.IsAnimating() )
				{
					treeEvents.ToggleNodeExpansion( treeNode );
				}
			}
			else
			{
				treeEvents.SelectNode( treeNode );
			}
		}

		public void ProcessDoubleClick( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Point p, ITreeInfo treeInfo, ITreeEvents treeEvents )
		{
			Size ecSize = GetGlyphSize( g, treeNode.IsExpanded );
			Rectangle ecBounds = new Rectangle( nodeRectangle.X + _leftSep, nodeRectangle.Y + (nodeRectangle.Height - ecSize.Height) / 2, ecSize.Width, ecSize.Height );

			if( !(ecBounds.Contains( p ) && treeNode.ChildNodes.Count > 0) )
			{
				if( treeNode.ChildNodes.Count > 0 )
				{
					treeEvents.ToggleNodeExpansion( treeNode );
				}
				else
				{
					treeEvents.SelectNode( treeNode );
				}
			}
		}

		#endregion

		private Size GetGlyphSize( Graphics g, bool expanded )
		{
			if( VisualStyleRenderer.IsSupported )
			{
				VisualStyleElement vse = expanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed;
				VisualStyleRenderer vsr = new VisualStyleRenderer( vse );
				Size ecSize = vsr.GetPartSize( g, ThemeSizeType.Draw );

				return ecSize;
			}
			else
			{
				return new Size( 9, 9 );
			}
		}

		private void DrawGlyph( Graphics g, Point p, bool expanded )
		{
			if( VisualStyleRenderer.IsSupported )
			{
				VisualStyleElement vse = expanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed;
				VisualStyleRenderer vsr = new VisualStyleRenderer( vse );
				Size ecSize = vsr.GetPartSize( g, ThemeSizeType.Draw );

				vsr.DrawBackground( g, new Rectangle( p.X, p.Y, ecSize.Width, ecSize.Height ) );
			}
			else
			{
				g.FillRectangle( SystemBrushes.Window, new Rectangle( p.X, p.Y, 8, 8 ) );

				using( Pen pen = new Pen( Color.FromArgb( 128, 128, 128 ) ))
				{
					g.DrawRectangle( pen, new Rectangle( p.X, p.Y, 8, 8 ) );
				}

				using( Pen pen = new Pen( Color.Black ) )
				{
					g.DrawLine( pen, new Point( p.X + 2, p.Y + 4 ), new Point( p.X + 6, p.Y + 4 ) );

					if( !expanded )
					{
						g.DrawLine( pen, new Point( p.X + 4, p.Y + 2 ), new Point( p.X + 4, p.Y + 6 ) );
					}
				}
			}
		}

		private const int _indent = 16;
		private const int _verticalSep = 1;
		private const int _leftSep = 4;
		private const int _rightSep = 4;
		private const int _ecSep = 11;
		private const int _imageSep = 2;
	}
}
