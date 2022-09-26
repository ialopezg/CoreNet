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
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	public class TreeControl : SuperTree.TreeControl
	{
		public TreeControl()
		{
			ColorTable = new Drawing.WindowsThemeColorTable();
		}

		public Drawing.ColorTable ColorTable
		{
			get
			{
				return _colorTable;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_colorTable = value;

				Renderer = new GlowRenderer( this );
			}
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			SuperToolTipManager.CloseToolTip();

			base.OnMouseDown( e );
		}

		protected override void OnContextMenuStripChanged( EventArgs e )
		{
			base.OnContextMenuStripChanged( e );

			if( _oldContextMenuStrip != null )
			{
				_oldContextMenuStrip.Opened -= new EventHandler( ContextMenuStrip_Opened );
				_oldContextMenuStrip.Closed -= new ToolStripDropDownClosedEventHandler( ContextMenuStrip_Closed );
			}

			if( ContextMenuStrip != null )
			{
				ContextMenuStrip.Opened += new EventHandler( ContextMenuStrip_Opened );
				ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler( ContextMenuStrip_Closed );
			}

			_oldContextMenuStrip = ContextMenuStrip;
		}

		private void ContextMenuStrip_Opened( object sender, EventArgs e )
		{
			WinFormsUtility.Events.MenuLoop.NotifyEnterMenuLoop();

			_contextMenuOpen = true;
		}

		private void ContextMenuStrip_Closed( object sender, ToolStripDropDownClosedEventArgs e )
		{
			WinFormsUtility.Events.MenuLoop.NotifyExitMenuLoop();

			_contextMenuOpen = false;
		}

		#region Renderer

		public class GlowRenderer : SuperTree.IRenderer
		{
			public GlowRenderer( TreeControl treeControl )
			{
				if( treeControl == null )
				{
					throw new ArgumentNullException( "treeControl" );
				}

				_treeControl = treeControl;
				_colorTable = _treeControl.ColorTable;
			}

			#region IRenderer Members

			public void Setup()
			{
				if( _updateTimer == null )
				{
					_updateTimer = new Timer();
					_updateTimer.Interval = 50;
					_updateTimer.Enabled = true;

					_updateTimer.Tick += new EventHandler( _updateTimer_Tick );
				}
			}

			public void Setdown()
			{
				if( _updateTimer != null )
				{
					_updateTimer.Tick -= new EventHandler( _updateTimer_Tick );
					_updateTimer.Enabled = false;
					_updateTimer.Dispose();
					_updateTimer = null;
				}
			}

			public void PreRender( SuperTree.ITreeInfo treeInfo, SuperTree.ITreeEvents treeEvents )
			{
				_treeInfo = treeInfo;
				_treeEvents = treeEvents;
			}

			public void RenderBackground( Graphics g, Rectangle clip )
			{
				g.Clear( SystemColors.Window );
			}

			public int MeasureIndent( Graphics g, SuperTree.ITreeInfo treeInfo, SuperTree.TreeNode node )
			{
				return node.Depth * _indent;
			}

			public Size MeasureTreeNode( Graphics g, SuperTree.ITreeInfo treeInfo, SuperTree.TreeNode treeNode, bool needsWidth, bool needsHeight )
			{
				string text = treeNode.Text.Replace( "&", "&&" );

				Size textSize = WinFormsUtility.Drawing.GdiPlusEx.MeasureString( g, text, treeNode.Font, int.MaxValue );
				Size ecSize;

				if( VisualStyleRenderer.IsSupported )
				{
					VisualStyleElement vse = VisualStyleElement.TreeView.Glyph.Opened;
					VisualStyleRenderer vsr = new VisualStyleRenderer( vse );

					ecSize = vsr.GetPartSize( g, ThemeSizeType.Draw );
				}
				else
				{
					ecSize = new Size( 9, 9 );
				}

				int width = (int) textSize.Width + _leftSep + _ecSep + ecSize.Width + _rightSep + 16;
				int height = (int) textSize.Height;

				if( treeNode.Icon != null )
				{
					width += _imageSep + treeNode.Icon.Size.Width;
					height = Math.Max( height, treeNode.Icon.Size.Height );
				}

				height += _verticalSep;

				return new Size( width, height );
			}

			public void RenderTreeNode( Graphics g, SuperTree.ITreeInfo treeInfo, SuperTree.TreeNode treeNode, Rectangle nodeRectangle, Rectangle clip )
			{
				UnsetAnimateGlow( treeNode );
				UnsetAnimateMark( treeNode );

				bool isLast = (treeNode.Index == treeNode.ParentCollection.Count - 1);
				Point ecCenter = new Point( nodeRectangle.X + _leftSep + _ecSize / 2, nodeRectangle.Y + (nodeRectangle.Height - _ecSize) / 2 + _ecSize / 2 );

				int textX = nodeRectangle.X + _ecSize + _leftSep + _ecSep;

				bool isSelected = treeInfo.IsSelected( treeNode );

				Rectangle bgRect = new Rectangle( nodeRectangle.X + _ecSize + _ecSep, nodeRectangle.Y, nodeRectangle.Width - _ecSize - 8, nodeRectangle.Height - 1 );
				SuperTree.TreeNode mouseOver;
				Point nodePosition;

				treeInfo.GetMouseOver( out mouseOver, out nodePosition );

				_nodeUpdates.DoneUpdate( treeNode );

				double nodeGlow = GetNodeFade( treeNode );

				if( isSelected || nodeGlow > 0 )
				{
					RenderNode( g, treeInfo, treeNode, bgRect, isSelected, nodeGlow );
				}

				if( treeNode.Icon != null )
				{
					Image image = GetImage( treeNode.Icon );
					Rectangle imageRect = new Rectangle( textX, nodeRectangle.Y + 1, image.Width, image.Height );

					if( clip.IntersectsWith( imageRect ) )
					{
						g.DrawImageUnscaled( image, imageRect );
					}

					textX += image.Width + _imageSep;
				}

				Color textColor;
				Rectangle textRect = new Rectangle( textX + 2, nodeRectangle.Y + _verticalSep, nodeRectangle.Right - textX - 5, nodeRectangle.Height );

				if( isSelected )
				{
					if( _treeControl.Focused )
					{
						textColor = _colorTable.GlowTextColor;
					}
					else
					{
						textColor = _colorTable.TextColor;
					}
				}
				else
				{
					textColor = SystemColors.ControlText;
				}

				if( clip.IntersectsWith( textRect ) )
				{
					WinFormsUtility.Drawing.GdiPlusEx.DrawString
						( g, treeNode.Text, treeNode.Font, textColor
						, textRect
						, WinFormsUtility.Drawing.GdiPlusEx.TextSplitting.SingleLineEllipsis, WinFormsUtility.Drawing.GdiPlusEx.Ampersands.Display );
				}

				Rectangle ecRect = new Rectangle( nodeRectangle.X, nodeRectangle.Y, 10, 10 );

				if( clip.IntersectsWith( ecRect ) )
				{
					if( treeNode.ChildNodes.Count > 0 )
					{
						RenderExpansionMark( g, treeInfo, treeNode, nodeRectangle );
					}
				}
			}

			public void RenderTreeNodeRow( Graphics g, SuperTree.TreeNode treeNode, Rectangle nodeRectangle, Rectangle rowRectangle )
			{
			}

			public void ProcessClick( Graphics g, SuperTree.TreeNode treeNode, Rectangle nodeRectangle, Point p, SuperTree.ITreeInfo treeInfo, SuperTree.ITreeEvents treeEvents )
			{
				if( IsOverExpandCollapseMark( treeNode, nodeRectangle, p ) )
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

			public void ProcessDoubleClick( Graphics g, SuperTree.TreeNode treeNode, Rectangle nodeRectangle, Point p, SuperTree.ITreeInfo treeInfo, SuperTree.ITreeEvents treeEvents )
			{
				if( !IsOverExpandCollapseMark( treeNode, nodeRectangle, p ) )
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

			private Image GetImage( Icon icon )
			{
				Image image;

				if( !_mapIconToImage.TryGetValue( icon, out image ) )
				{
					image = icon.ToBitmap();
					_mapIconToImage.Add( icon, image );
				}

				return image;
			}

			private VectorGraphics.Primitives.Container CreateNodeItem( VectorGraphics.Renderers.Renderer renderer, Rectangle nodeRect, bool isSelected, double glow )
			{
				VectorGraphics.Paint.Color glowHighlightColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlowHighlightColor );
				VectorGraphics.Paint.Color glossyGlowLightenerColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlossyGlowLightenerColor );
				VectorGraphics.Paint.Color glowColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlowColor );
				VectorGraphics.Paint.Color white = VectorGraphics.Paint.Color.White;

				VectorGraphics.Factories.RoundedRectangle roundedRectangleFactory = new VectorGraphics.Factories.RoundedRectangle();
				VectorGraphics.Factories.GlossyBrush glossyBrushFactory = new VectorGraphics.Factories.GlossyBrush( glossyGlowLightenerColor );

				VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

				VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( nodeRect );

				VectorGraphics.Primitives.Path roundedRect = roundedRectangleFactory.Create( rect, 3 );

				container.AddBack( roundedRect );

				if( isSelected )
				{
					VectorGraphics.Paint.Color borderColor = glowColor;
					VectorGraphics.Paint.Color glowStartColor = glowColor;
					VectorGraphics.Paint.Color glowEndColor = glowHighlightColor;

					if( !_treeControl.Focused )
					{
						borderColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GrayForegroundColor );
						glowStartColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GrayForegroundColor );
						glowEndColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GrayBackgroundColor );
					}

					roundedRect.Pen = new VectorGraphics.Paint.Pens.SolidPen( borderColor, 1 );
					roundedRect.Brush = glossyBrushFactory.Create( glowStartColor, glowEndColor, nodeRect.Top, nodeRect.Bottom );

					rect = VectorGraphics.Types.Rectangle.Shrink( rect, 1 );

					container.AddBack( CreateRoundRectHighlight( rect, 2 ) );
				}
				else if( glow > 0 )
				{
					VectorGraphics.Paint.Color borderColor = glowHighlightColor;
					VectorGraphics.Paint.Color glowStartColor = glowColor;
					VectorGraphics.Paint.Color glowEndColor = glowHighlightColor;

					borderColor = VectorGraphics.Paint.Color.Combine( borderColor, white, 0.7 );
					glowStartColor = VectorGraphics.Paint.Color.Combine( glowStartColor, white, 0.4 );
					glowEndColor = VectorGraphics.Paint.Color.Combine( glowEndColor, white, 0.4 );

					borderColor = new VectorGraphics.Paint.Color( borderColor, glow );
					glowStartColor = new VectorGraphics.Paint.Color( glowStartColor, glow );
					glowEndColor = new VectorGraphics.Paint.Color( glowEndColor, glow );

					roundedRect.Pen = new VectorGraphics.Paint.Pens.SolidPen( borderColor, 1 );
					roundedRect.Brush = glossyBrushFactory.Create( glowStartColor, glowEndColor, nodeRect.Top, nodeRect.Bottom );
				}

				return container;
			}

			private VectorGraphics.Primitives.Path CreateRoundRectHighlight( VectorGraphics.Types.Rectangle rect, double radius )
			{
				VectorGraphics.Primitives.Path path = new VectorGraphics.Primitives.Path();
				VectorGraphics.Paint.Color glossyGlowLightenerColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlossyGlowLightenerColor );

				path.Add( new VectorGraphics.Primitives.Path.Move( new VectorGraphics.Types.Point( rect.X, rect.Y + rect.Height - radius ) ) );
				path.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( rect.X, rect.Y + radius ) ) );
				path.Add( new VectorGraphics.Primitives.Path.EllipticalArc( radius, radius, 0, false, true, new VectorGraphics.Types.Point( rect.X + radius, rect.Y ) ) );
				path.Add( new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( rect.X + rect.Width - radius, rect.Y ) ) );

				path.Pen = new VectorGraphics.Paint.Pens.SolidPen( new VectorGraphics.Paint.Color( glossyGlowLightenerColor, 0.6 ), 1 );

				return path;
			}

			private VectorGraphics.Primitives.Container CreateExpandCollapseItem( VectorGraphics.Renderers.Renderer renderer, double borderGlow, double glow, bool over )
			{
				VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

				VectorGraphics.Primitives.Path arrow = new VectorGraphics.Primitives.Path( new VectorGraphics.Primitives.Path.Command[]
					{
						new VectorGraphics.Primitives.Path.Move( new VectorGraphics.Types.Point( -_ecSize / 4, -_ecSize / 2 ) ),
						new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( _ecSize / 3, 0 ) ),
						new VectorGraphics.Primitives.Path.Line( new VectorGraphics.Types.Point( -_ecSize / 4, _ecSize / 2 ) ),
						new VectorGraphics.Primitives.Path.Close()
					} );

				VectorGraphics.Paint.Color greyColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GrayTextColor );
				VectorGraphics.Paint.Color glowDeepColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlowDeepColor );
				VectorGraphics.Paint.Color glowColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( _colorTable.GlowColor );
				VectorGraphics.Paint.Color bgColor = VectorGraphics.Renderers.GdiPlusUtility.Convert.Color( SystemColors.Window );

				glowColor = VectorGraphics.Paint.Color.Combine( glowColor, bgColor, 0.7 );

				VectorGraphics.Paint.Color borderColor = VectorGraphics.Paint.Color.Combine( glowDeepColor, greyColor, glow );

				arrow.Pen = new VectorGraphics.Paint.Pens.SolidPen( new VectorGraphics.Paint.Color( borderColor, borderGlow ), 1 );

				container.AddBack( arrow );

				if( glow > 0 )
				{
					arrow.Brush = new VectorGraphics.Paint.Brushes.SolidBrush( new VectorGraphics.Paint.Color( glowColor, glow ) );

					VectorGraphics.Factories.SoftShadow shadow = new VectorGraphics.Factories.SoftShadow
						( renderer, new VectorGraphics.Types.Point( 0, 0 ), 3
						, new VectorGraphics.Paint.Color( glowColor, glow ) );

					shadow.Apply( container );
				}

				return container;
			}

			private VectorGraphics.Renderers.GdiPlusRenderer CreateRenderer( Graphics g )
			{
				return new VectorGraphics.Renderers.GdiPlusRenderer( delegate
				{
					return g;
				}, VectorGraphics.Renderers.GdiPlusRenderer.MarkerHandling.Ignore, 1 );
			}

			private void RenderRowBackground( Graphics g, SuperTree.TreeNode treeNode, Rectangle bounds )
			{
				VectorGraphics.Primitives.Container visualItem = CreateRowVisualItem( bounds, 1 );

				using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( g ) )
				{
					renderer.Render( g, visualItem, VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( bounds ) );
				}
			}

			private VectorGraphics.Primitives.Container CreateRowVisualItem( Rectangle bounds, double glow )
			{
				VectorGraphics.Types.Rectangle rect = VectorGraphics.Renderers.GdiPlusUtility.Convert.Rectangle( bounds );
				VectorGraphics.Factories.RoundedRectangle roundedRectangleFactory = new VectorGraphics.Factories.RoundedRectangle();

				VectorGraphics.Primitives.Container container = new VectorGraphics.Primitives.Container();

				VectorGraphics.Primitives.Path roundedRect = roundedRectangleFactory.Create( rect, 3 );

				container.AddBack( roundedRect );

				return container;
			}

			private bool IsOverExpandCollapseMark( SuperTree.TreeNode treeNode, Rectangle nodeRectangle, Point p )
			{
				if( treeNode.ChildNodes.Count == 0 )
				{
					return false;
				}

				Rectangle ecBounds = new Rectangle( nodeRectangle.X + _leftSep, nodeRectangle.Y + (nodeRectangle.Height - _ecSize) / 2, _ecSize, _ecSize );

				return ecBounds.Contains( p ) && treeNode.ChildNodes.Count > 0;
			}

			private void RenderNode( Graphics g, SuperTree.ITreeInfo treeInfo, SuperTree.TreeNode treeNode, Rectangle nodeBounds, bool isSelected, double glow )
			{
				using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( g ) )
				{
					g.SmoothingMode = SmoothingMode.HighQuality;

					using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( g ) )
					{
						VectorGraphics.Primitives.Container visualItem = CreateNodeItem( renderer, nodeBounds, isSelected, glow );

						renderer.Render( g, visualItem );
					}
				}
			}

			private void RenderExpansionMark( Graphics g, SuperTree.ITreeInfo treeInfo, SuperTree.TreeNode treeNode, Rectangle nodeBounds )
			{
				double p = treeInfo.ExpansionAnimationPosition( treeNode );
				Point ecCenter = new Point( nodeBounds.X + _leftSep + _ecSize / 2 + 1, nodeBounds.Y + (nodeBounds.Height - _ecSize) / 2 + _ecSize / 2 - 1 );

				Debug.Assert( p >= 0 && p <= 1 );

				using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( g ) )
				{
					g.SmoothingMode = SmoothingMode.HighQuality;

					g.TranslateTransform( ecCenter.X, ecCenter.Y );
					g.RotateTransform( (float) (45 * p) );

					SuperTree.TreeNode mouseOver;
					Point nodePosition;
					bool over = false;

					_treeInfo.GetMouseOver( out mouseOver, out nodePosition );

					_markUpdates.DoneUpdate( treeNode );

					double borderGlow = GetOverallFade();
					double glow = GetMarkFade( treeNode );

					if( borderGlow > 0 || glow > 0 )
					{
						using( VectorGraphics.Renderers.GdiPlusRenderer renderer = CreateRenderer( g ) )
						{
							VectorGraphics.Primitives.Container visualItem = CreateExpandCollapseItem( renderer, borderGlow, glow, over );

							renderer.Render( g, visualItem );
						}
					}
				}
			}

			private void _updateTimer_Tick( object sender, EventArgs e )
			{
				if( _treeInfo == null )
				{
					return;
				}

				//
				// Overall tree animation.

				if( (_treeInfo.IsMouseOverTree() && _treeHistory.MouseOver == null)
					|| (!_treeInfo.IsMouseOverTree() && _treeHistory.MouseOver != null) )
				{
					_treeUpdates.NeedsUpdate( _treeInfo );
				}

				_treeHistory.Update( _treeInfo.IsMouseOverTree() ? _treeInfo : null );

				if( _treeUpdates.Items.Length > 0 )
				{
					if( !_animating )
					{
						foreach( SuperTree.TreeNode folderNode in _folderNodes )
						{
							_treeInfo.EndAnimating( folderNode );
						}

						_folderNodes = GetVisibleFolderNodes();

						foreach( SuperTree.TreeNode folderNode in _folderNodes )
						{
							_treeInfo.BeginAnimating( folderNode, new Rectangle( 4, 0, 10, 19 ) );
						}
						_animating = true;
					}
					_treeUpdates.DoneAll();
				}
				else
				{
					if( _animating )
					{
						foreach( SuperTree.TreeNode folderNode in _folderNodes )
						{
							_treeInfo.EndAnimating( folderNode );
						}
						_folderNodes.Clear();
						_animating = false;
					}
				}

				//
				// Expand/collapse marks and node animations.

				SuperTree.TreeNode mouseOver;
				SuperTree.TreeNode ecMouseOver = null, nodeMouseOver = null;
				Point nodePosition;

				_treeInfo.GetMouseOver( out mouseOver, out nodePosition );

				if( _treeControl._contextMenuOpen || _treeInfo.IsAnimating() )
				{
					mouseOver = null;
				}

				if( mouseOver != null )
				{
					if( IsOverExpandCollapseMark( mouseOver, new Rectangle( Point.Empty, MeasureTreeNode( _treeInfo.CreateGraphics(), _treeInfo, mouseOver, true, true ) ), nodePosition ) )
					{
						ecMouseOver = mouseOver;
					}

					nodeMouseOver = mouseOver;
				}

				if( ecMouseOver != _markHistory.MouseOver )
				{
					if( _markHistory.MouseOver != null )
					{
						_markUpdates.NeedsUpdate( _markHistory.MouseOver );
					}

					_markHistory.Update( ecMouseOver );

					if( _markHistory.MouseOver != null )
					{
						_markUpdates.NeedsUpdate( _markHistory.MouseOver );
					}
				}
				if( nodeMouseOver != _nodeHistory.MouseOver )
				{
					if( _nodeHistory.MouseOver != null )
					{
						_nodeUpdates.NeedsUpdate( _nodeHistory.MouseOver );
					}

					_nodeHistory.Update( nodeMouseOver );

					if( _nodeHistory.MouseOver != null )
					{
						_nodeUpdates.NeedsUpdate( _nodeHistory.MouseOver );
					}
				}

				foreach( SuperTree.TreeNode tn in _markUpdates.Items )
				{
					SetAnimateMark( tn );
				}
				foreach( SuperTree.TreeNode tn in _nodeUpdates.Items )
				{
					SetAnimateGlow( tn );
				}
			}

			private List<SuperTree.TreeNode> GetVisibleFolderNodes()
			{
				List<SuperTree.TreeNode> nodes = new List<SuperTree.TreeNode>();

				foreach( SuperTree.TreeNode node in _treeInfo.GetVisibleNodes() )
				{
					if( node.ChildNodes.Count > 0 )
					{
						nodes.Add( node );
					}
				}

				return nodes;
			}

			private void SetAnimateGlow( SuperTree.TreeNode treeNode )
			{
				if( !_animatingGlows.Contains( treeNode ) )
				{
					_animatingGlows.Add( treeNode );
					_treeInfo.BeginAnimating( treeNode, new Rectangle( Point.Empty, _treeInfo.GetNodeSize( treeNode ) ) );
				}
			}

			private void UnsetAnimateGlow( SuperTree.TreeNode treeNode )
			{
				if( _animatingGlows.Contains( treeNode ) )
				{
					_animatingGlows.Remove( treeNode );
					_treeInfo.EndAnimating( treeNode );
				}
			}

			private void SetAnimateMark( SuperTree.TreeNode treeNode )
			{
				if( !_animatingMarks.Contains( treeNode ) )
				{
					_animatingMarks.Add( treeNode );
					_treeInfo.BeginAnimating( treeNode, new Rectangle( _leftSep - 3, 0, 14, 19 ) );
				}
			}

			private void UnsetAnimateMark( SuperTree.TreeNode treeNode )
			{
				if( _animatingMarks.Contains( treeNode ) )
				{
					_animatingMarks.Remove( treeNode );
					_treeInfo.EndAnimating( treeNode );
				}
			}

			private double GetOverallFade()
			{
				double fadeInGlow = 0, fadeOutGlow = 0;

				if( _treeHistory.MouseOver == _treeInfo )
				{
					double fadeInTime = _treeHistory.GetTimeOver( _treeInfo ) ?? 0;

					if( fadeInTime < _overallFadeIn )
					{
						_treeUpdates.NeedsUpdate( _treeInfo );
						fadeInGlow = fadeInTime / _overallFadeIn;
					}
					else
					{
						fadeInGlow = 1;
					}
				}

				double fadeOutTime = _treeHistory.GetLastOver( _treeInfo );

				if( fadeOutTime < _overallFadeOut )
				{
					_treeUpdates.NeedsUpdate( _treeInfo );
					fadeOutGlow = 1 - fadeOutTime / _overallFadeOut;
				}

				double f = Math.Max( fadeInGlow, fadeOutGlow );

				f = Math.Min( Math.Max( f, 0 ), 1 );

				return f;
			}

			private double GetMarkFade( SuperTree.TreeNode treeNode )
			{
				double fadeInGlow = 0, fadeOutGlow = 0;

				if( _markHistory.MouseOver == treeNode )
				{
					double fadeInTime = _markHistory.GetTimeOver( treeNode ) ?? 0;

					if( fadeInTime < _markFadeIn )
					{
						_markUpdates.NeedsUpdate( treeNode );
						fadeInGlow = fadeInTime / _markFadeIn;
					}
					else
					{
						fadeInGlow = 1;
					}
				}

				double fadeOutTime = _markHistory.GetLastOver( treeNode );

				if( fadeOutTime < _markFadeOut )
				{
					_markUpdates.NeedsUpdate( treeNode );
					fadeOutGlow = 1 - fadeOutTime / _markFadeOut;
				}

				double f = Math.Max( fadeInGlow, fadeOutGlow );

				f = Math.Min( Math.Max( f, 0 ), 1 );

				return f;
			}

			private double GetNodeFade( SuperTree.TreeNode treeNode )
			{
				double fadeInGlow = 0, fadeOutGlow = 0;

				if( _nodeHistory.MouseOver == treeNode )
				{
					double fadeInTime = _nodeHistory.GetTimeOver( treeNode ) ?? 0;

					if( fadeInTime < _nodeFadeIn )
					{
						_nodeUpdates.NeedsUpdate( treeNode );
						fadeInGlow = fadeInTime / _nodeFadeIn;
					}
					else
					{
						fadeInGlow = 1;
					}
				}

				double fadeOutTime = _nodeHistory.GetLastOver( treeNode );

				if( fadeOutTime < _nodeFadeOut )
				{
					_nodeUpdates.NeedsUpdate( treeNode );
					fadeOutGlow = 1 - fadeOutTime / _nodeFadeOut;
				}

				double f = Math.Max( fadeInGlow, fadeOutGlow );

				f = Math.Min( Math.Max( f, 0 ), 1 );

				return f;
			}

			private const int _indent = 16;
			private const int _verticalSep = 3;
			private const int _leftSep = 3;
			private const int _ecSep = 4;
			private const int _imageSep = 2;
			private const int _ecSize = 10;
			private const int _rightSep = 4;

			private const double _overallFadeIn = 0.2;
			private const double _overallFadeOut = 0.7;
			private const double _markFadeIn = 0.2;
			private const double _markFadeOut = 0.6;
			private const double _nodeFadeIn = 0.0;
			private const double _nodeFadeOut = 0.2;

			private TreeControl _treeControl;
			private Timer _updateTimer;
			private SuperTree.ITreeInfo _treeInfo;
			private SuperTree.ITreeEvents _treeEvents;
			private WinFormsGloss.Drawing.GlowHistory<SuperTree.ITreeInfo> _treeHistory = new WinFormsGloss.Drawing.GlowHistory<SuperTree.ITreeInfo>( null );
			private WinFormsGloss.Drawing.GlowUpdates<SuperTree.ITreeInfo> _treeUpdates = new WinFormsGloss.Drawing.GlowUpdates<SuperTree.ITreeInfo>();
			private WinFormsGloss.Drawing.GlowHistory<SuperTree.TreeNode> _nodeHistory = new WinFormsGloss.Drawing.GlowHistory<SuperTree.TreeNode>( null );
			private WinFormsGloss.Drawing.GlowUpdates<SuperTree.TreeNode> _nodeUpdates = new WinFormsGloss.Drawing.GlowUpdates<SuperTree.TreeNode>();
			private WinFormsGloss.Drawing.GlowHistory<SuperTree.TreeNode> _markHistory = new WinFormsGloss.Drawing.GlowHistory<SuperTree.TreeNode>( null );
			private WinFormsGloss.Drawing.GlowUpdates<SuperTree.TreeNode> _markUpdates = new WinFormsGloss.Drawing.GlowUpdates<SuperTree.TreeNode>();
			private bool _animating;
			private Drawing.ColorTable _colorTable;
			private Utility.Collections.Set<SuperTree.TreeNode> _animatingGlows = new Utility.Collections.Set<SuperTree.TreeNode>();
			private Utility.Collections.Set<SuperTree.TreeNode> _animatingMarks = new Utility.Collections.Set<SuperTree.TreeNode>();
			private List<SuperTree.TreeNode> _folderNodes = new List<SuperTree.TreeNode>();
			private Dictionary<Icon, Image> _mapIconToImage = new Dictionary<Icon, Image>();
		}

		#endregion

		private Drawing.ColorTable _colorTable;
		private ContextMenuStrip _oldContextMenuStrip;
		private bool _contextMenuOpen;
	}
}
