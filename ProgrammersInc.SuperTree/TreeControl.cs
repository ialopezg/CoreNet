/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProgrammersInc.SuperTree
{
	public partial class TreeControl : UserControl, ITreeEvents, ITreeInfo
	{
		public TreeControl()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.ResizeRedraw
				| ControlStyles.Selectable
				, true );

			_updatesSuspended.Reset += new EventHandler( _updatesSuspended_Reset );
			_rootNodes = new TreeNodeCollection( null, this, this );
			_treeState = new Internal.TreeState( _rootNodes, this );

			Renderer = new Renderers.StandardRenderer();
		}

		public IRenderer Renderer
		{
			get
			{
				return _renderer;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				if( _renderer != null )
				{
					_renderer.Setdown();
				}

				_renderer = value;

				if( IsHandleCreated )
				{
					_renderer.Setup();
				}

				RecreateVerticalPositioning();
			}
		}

		public bool Animate
		{
			get
			{
				return _animate;
			}
			set
			{
				if( _animate == value )
				{
					return;
				}

				_animate = value;

				RecreateVerticalPositioning();
			}
		}

		public TreeNodeCollection RootNodes
		{
			[DebuggerStepThrough]
			get
			{
				return _rootNodes;
			}
		}

		public List<Icon> Icons
		{
			get
			{
				return _icons;
			}
		}

		public TreeNode SelectedNode
		{
			get
			{
				return _treeState.SelectedTreeNode;
			}
			set
			{
				if( value == null )
				{
					return;
				}
				if( value == SelectedNode )
				{
					return;
				}

				((ITreeEvents)this).SelectNode( value );

				EnsureNodeVisible( value );

				if( !_updatesSuspended.IsActive )
				{
					Update();
				}
			}
		}

		public IDisposable SuspendUpdates()
		{
			return _updatesSuspended.Apply();
		}

		public void EnsureNodeVisible( TreeNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			using( SuspendUpdates() )
			{
				TreeNode parent = node.ParentCollection.ParentNode;

				while( parent != null )
				{
					if( !parent.IsExpanded )
					{
						((ITreeEvents) this).ToggleNodeExpansion( parent );
					}

					parent = parent.ParentCollection.ParentNode;
				}
			}

			Rectangle bounds = _verticalPositioning.GetNodeBounds( node, Internal.Coordinates.Y | Internal.Coordinates.Height );

			bounds.Offset( AutoScrollPosition );

			if( bounds.Top < ClientRectangle.Top )
			{
				AutoScrollPosition = new Point( -AutoScrollPosition.X, -AutoScrollPosition.Y - (ClientRectangle.Top - bounds.Top) - 4 );
			}
			else if( bounds.Bottom > ClientRectangle.Bottom )
			{
				AutoScrollPosition = new Point( -AutoScrollPosition.X, -AutoScrollPosition.Y + (bounds.Bottom - ClientRectangle.Bottom) + 4 );
			}
		}
		
		public void MoveSelectionOne()
		{
			TreeNode node = SelectedNode;

			if( node == null )
			{
				return;
			}

			TreeNode next = _verticalPositioning.GetNodeAfter( node );

			if( next != null )
			{
				SelectedNode = next;
			}
		}

		public void ExpandNode( TreeNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			if( !_treeState.IsExpanded( node ) )
			{
				((ITreeEvents) this).ToggleNodeExpansion( node );
			}
		}

		public void CollapseNode( TreeNode node )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}

			if( _treeState.IsExpanded( node ) )
			{
				((ITreeEvents) this).ToggleNodeExpansion( node );
			}
		}

		public void ExpandAllNodes()
		{
			RecurseExpandAll( RootNodes );
		}

		public TreeNode GetNodeAt( Point p )
		{
			p = new Point( p.X - AutoScrollPosition.X, p.Y - AutoScrollPosition.Y );

			foreach( TreeNode treeNode in _verticalPositioning.GetNodesBetween( p.Y, p.Y ) )
			{
				if( _verticalPositioning.GetNodeBounds( treeNode, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height ).Contains( p ) )
				{
					return treeNode;
				}
			}

			return null;
		}

		public void RecalculateWidths()
		{
			_verticalPositioning.DirtyWidths();
			
			Invalidate();

			_needsInvalidate.Clear();
		}

		public void InvalidateNode( TreeNode node )
		{
			Rectangle nodeBounds = _verticalPositioning.GetNodeBounds( node, Internal.Coordinates.Y | Internal.Coordinates.Height );

			nodeBounds.Offset( 0, AutoScrollPosition.Y );

			Rectangle rect = new Rectangle( 0, nodeBounds.Y, Math.Max( Width, AutoScrollMinSize.Width ), nodeBounds.Height );

			Invalidate( rect );
		}

		#region Event launchers

		protected virtual void OnAfterSelect( TreeNodeEventArgs e )
		{
			if( AfterSelect != null )
			{
				AfterSelect( this, e );
			}
		}

		protected virtual void OnNodeMouseClick( TreeNodeMouseEventArgs e )
		{
			if( NodeMouseClick != null )
			{
				NodeMouseClick( this, e );
			}
		}

		protected virtual void OnNodeMouseDoubleClick( TreeNodeMouseEventArgs e )
		{
			if( NodeMouseDoubleClick != null )
			{
				NodeMouseDoubleClick( this, e );
			}
		}

		protected virtual void OnNodeMouseHover( TreeNodeEventArgs e )
		{
			if( NodeMouseHover != null )
			{
				NodeMouseHover( this, e );
			}
		}

		protected virtual void OnNodeDrag( TreeNodeMouseEventArgs e )
		{
			if( NodeDrag != null )
			{
				NodeDrag( this, e );
			}
		}

		#endregion

		#region Event handlers

		protected override void OnPaintBackground( PaintEventArgs e )
		{
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			_renderer.PreRender( this, this );
			_verticalPositioning.SetAnimationMark( DateTime.Now );

			Rectangle clip = e.ClipRectangle;
			int ypos = AutoScrollPosition.Y;

			clip.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );

			using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( e.Graphics ) )
			{
				e.Graphics.TranslateTransform( AutoScrollPosition.X, AutoScrollPosition.Y );
				e.Graphics.RenderingOrigin = new Point( AutoScrollPosition.X, AutoScrollPosition.Y );

				_renderer.RenderBackground( e.Graphics, clip );

				int clipTop = clip.Top, clipBottom = clip.Bottom;

				if( clipTop < ClientRectangle.Top - AutoScrollPosition.Y )
				{
					clipTop = ClientRectangle.Top - AutoScrollPosition.Y;
				}
				if( clipBottom > ClientRectangle.Bottom - AutoScrollPosition.Y )
				{
					clipBottom = ClientRectangle.Bottom - AutoScrollPosition.Y;
				}

				TreeNode[] nodes = _verticalPositioning.GetNodesBetween( clipTop, clipBottom );

				if( nodes.Length > 0 )
				{
					_verticalPositioning.GetNodeBounds
						 ( nodes[nodes.Length - 1], Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );
				}

				foreach( TreeNode treeNode in nodes )
				{
					Rectangle nodeBounds = _verticalPositioning.GetNodeBounds
						( treeNode, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );

					if( nodeBounds.Bottom > ypos )
					{
						Rectangle rowBounds = new Rectangle( 0, nodeBounds.Y, Math.Max( AutoScrollMinSize.Width, ClientRectangle.Width ), nodeBounds.Height );

						if( _verticalPositioning.IsAnimating() )
						{
							Rectangle newClip = new Rectangle( 0, Math.Max( ypos, nodeBounds.Y ), Math.Max( AutoScrollMinSize.Width, ClientRectangle.Width ), ClientRectangle.Height );

							e.Graphics.Clip = new Region( newClip );
						}

						_renderer.RenderTreeNodeRow( e.Graphics, treeNode, nodeBounds, rowBounds );
						_renderer.RenderTreeNode( e.Graphics, this, treeNode, nodeBounds, clip );
					}

					ypos = Math.Max( ypos, nodeBounds.Bottom );
				}
			}

			Size required = new Size( _verticalPositioning.GetMaxWidth(), _verticalPositioning.GetTotalHeight() );

			if( required != AutoScrollMinSize )
			{
				AutoScrollMinSize = required;
			}
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			ApplyUpdate();
		}

		protected override void OnMouseDoubleClick( MouseEventArgs e )
		{
			base.OnMouseDoubleClick( e );

			int x = e.X - AutoScrollPosition.X;
			int y = e.Y - AutoScrollPosition.Y;

			TreeNode[] nodes = _verticalPositioning.GetNodesBetween( y, y );

			if( nodes.Length == 1 )
			{
				TreeNode treeNode = nodes[0];
				Rectangle nodeBounds = _verticalPositioning.GetNodeBounds
					( treeNode, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );
				Rectangle rowBounds = new Rectangle( 0, nodeBounds.Y, AutoScrollMinSize.Width, nodeBounds.Height );

				if( rowBounds.Contains( x, y ) )
				{
					_renderer.ProcessDoubleClick( CreateGraphics(), treeNode, nodeBounds, new Point( x, y ), this, this );

					OnNodeMouseDoubleClick( new TreeNodeMouseEventArgs( treeNode, e.Button ) );
				}
			}

			_gotDragClick = false;
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			_gotDragClick = (e.Button == MouseButtons.Left);

			_mouseDownPosition = e.Location;

			int x = e.X - AutoScrollPosition.X;
			int y = e.Y - AutoScrollPosition.Y;

			TreeNode[] nodes = _verticalPositioning.GetNodesBetween( y, y );

			if( nodes.Length == 1 )
			{
				TreeNode treeNode = nodes[0];
				Rectangle nodeBounds = _verticalPositioning.GetNodeBounds
					( treeNode, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );
				Rectangle rowBounds = new Rectangle( 0, nodeBounds.Y, AutoScrollMinSize.Width, nodeBounds.Height );

				if( rowBounds.Contains( x, y ) )
				{
					_renderer.ProcessClick( CreateGraphics(), treeNode, nodeBounds, new Point( x, y ), this, this );

					OnNodeMouseClick( new TreeNodeMouseEventArgs( treeNode, e.Button ) );
				}
			}
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			Rectangle allowed = new Rectangle
				( _mouseDownPosition.X - SystemInformation.DoubleClickSize.Width / 2, _mouseDownPosition.Y - SystemInformation.DoubleClickSize.Height / 2
				, SystemInformation.DoubleClickSize.Width, SystemInformation.DoubleClickSize.Height );
			TreeNode node = GetNodeAt( _mouseDownPosition );

			if( _gotDragClick && node != null && e.Button != MouseButtons.None && !allowed.Contains( e.Location ) )
			{
				_gotDragClick = false;
				OnNodeDrag( new TreeNodeMouseEventArgs( node, e.Button ) );
			}
		}

		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );

			ApplyUpdate();
		}

		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			ApplyUpdate();
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			_renderer.Setup();

			_timer = new Timer();
			_timer.Interval = 200;
			_timer.Enabled = true;
			_timer.Tick += new EventHandler( _timer_Tick );

			if( _updateTimer != null )
			{
				_updateTimer.Enabled = true;
				_updateTimer.Tick += new System.EventHandler( this._updateTimer_Tick );
			}

			_animationRequests = new Internal.AnimationRequests( this );
			_animationRequests.Invalidate += new EventHandler( _animationRequests_Invalidate );
			_animationRequests.InvalidateTreeNode += new TreeNodeRectangleEventHandler( _animationRequests_InvalidateTreeNode );
			_animationRequests.Update += new EventHandler( _animationRequests_Update );
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

			_renderer.Setdown();

			if( _timer != null )
			{
				_timer.Enabled = false;
				_timer.Tick -= new EventHandler( _timer_Tick );
				_timer.Dispose();
				_timer = null;
			}

			if( _updateTimer != null )
			{
				_updateTimer.Tick -= new System.EventHandler( this._updateTimer_Tick );
				_updateTimer.Dispose();
				_updateTimer = null;
			}

			if( _animationRequests != null )
			{
				_animationRequests.Invalidate -= new EventHandler( _animationRequests_Invalidate );
				_animationRequests.InvalidateTreeNode -= new TreeNodeRectangleEventHandler( _animationRequests_InvalidateTreeNode );
				_animationRequests.Update -= new EventHandler( _animationRequests_Update );
				_animationRequests.Dispose();
				_animationRequests = null;
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			TreeNode node = SelectedNode;

			switch( keyData )
			{
				case Keys.Multiply:
					{
						if( node != null )
						{
							if( !node.IsExpanded )
							{
								((ITreeEvents) this).ToggleNodeExpansion( node );
							}

							RecurseExpandAll( node.ChildNodes );
						}
						return true;
					}
				case Keys.Subtract:
					{
						if( node != null )
						{
							if( node.IsExpanded )
							{
								((ITreeEvents) this).ToggleNodeExpansion( node );
							}
						}
						return true;
					}
				case Keys.Up:
					{
						if( node != null )
						{
							TreeNode newNode = _verticalPositioning.GetNodeBefore( node );

							if( newNode != null )
							{
								SelectedNode = newNode;
							}
						}
						return true;
					}
				case Keys.Down:
					{
						if( node != null )
						{
							TreeNode newNode = _verticalPositioning.GetNodeAfter( node );

							if( newNode != null )
							{
								SelectedNode = newNode;
							}
						}
						return true;
					}
				case Keys.Left:
					{
						while( node != null && (node.ChildNodes.Count == 0 || !_treeState.IsExpanded( node )) )
						{
							node = node.ParentCollection.ParentNode;
						}

						if( node != null )
						{
							CollapseNode( node );
						}
						return true;
					}
				case Keys.Right:
					{
						if( node != null )
						{
							ExpandNode( node );
						}

						return true;
					}
				case Keys.Up | Keys.Control:
					{
						AutoScrollPosition = new Point( -AutoScrollPosition.X, -AutoScrollPosition.Y - 16 );
						return true;
					}
				case Keys.Down | Keys.Control:
					{
						AutoScrollPosition = new Point( -AutoScrollPosition.X, -AutoScrollPosition.Y + 16 );
						return true;
					}
				case Keys.Left | Keys.Control:
					{
						AutoScrollPosition = new Point( -AutoScrollPosition.X - 16, -AutoScrollPosition.Y );
						return true;
					}
				case Keys.Right | Keys.Control:
					{
						AutoScrollPosition = new Point( -AutoScrollPosition.X + 16, -AutoScrollPosition.Y );
						return true;
					}
				default:
					{
						return base.ProcessCmdKey( ref msg, keyData );
					}
			}
		}

		#endregion

		#region ITreeEvents Members

		void ITreeEvents.NodeUpdated( TreeNode treeNode )
		{
			_treeState.NodeUpdated( treeNode );
			_verticalPositioning.NodeUpdated( treeNode );

			ApplyUpdate();
		}

		void ITreeEvents.NodeInserted( TreeNode treeNode )
		{
			_treeState.NodeInserted( treeNode );
			_verticalPositioning.NodeInserted( treeNode );

			ApplyUpdate();
		}

		void ITreeEvents.NodeDeleted( TreeNode treeNode )
		{
			_animationRequests.EndAnimating( treeNode );
			_treeState.NodeDeleted( treeNode );
			_verticalPositioning.NodeDeleted( treeNode );

			ApplyUpdate();
		}

		void ITreeEvents.ToggleNodeExpansion( TreeNode treeNode )
		{
			_treeState.ToggleNodeExpansion( treeNode );
			_verticalPositioning.ToggleNodeExpansion( treeNode );

			ApplyUpdate();
		}

		void ITreeEvents.SelectNode( TreeNode treeNode )
		{
			if( treeNode == SelectedNode )
			{
				return;
			}

			_treeState.SelectNode( treeNode );
			_verticalPositioning.SelectNode( treeNode );

			OnAfterSelect( new TreeNodeEventArgs( treeNode ) );
		}

		void ITreeEvents.UpdateAnimations()
		{
			_treeState.UpdateAnimations();
			_verticalPositioning.UpdateAnimations();
		}

		#endregion

		#region ITreeInfo Members

		Size ITreeInfo.ViewportSize
		{
			get
			{
				return ClientSize;
			}
		}

		Size ITreeInfo.GetNodeSize( TreeNode treeNode )
		{
			return _verticalPositioning.GetNodeBounds( treeNode, Internal.Coordinates.Width | Internal.Coordinates.Height ).Size;
		}

		bool ITreeInfo.IsTreeFocused()
		{
			return Focused;
		}

		bool ITreeInfo.IsUpdatesSuspended()
		{
			if( _updatesSuspended.IsActive )
			{
				return true;
			}

			if( _animationRequests == null )
			{
				return true;
			}

			return false;
		}

		bool ITreeInfo.IsAnimating()
		{
			return _verticalPositioning.IsAnimating();
		}

		void ITreeInfo.BeginAnimating()
		{
			_animationRequests.BeginAnimating();
		}

		void ITreeInfo.EndAnimating()
		{
			_animationRequests.EndAnimating();

			ApplyUpdate();
		}

		void ITreeInfo.BeginAnimating( TreeNode treeNode, Rectangle subRect )
		{
			_animationRequests.BeginAnimating( treeNode, subRect );
		}

		void ITreeInfo.EndAnimating( TreeNode treeNode )
		{
			_animationRequests.EndAnimating( treeNode );
		}

		bool ITreeInfo.IsSelected( TreeNode treeNode )
		{
			return _treeState.SelectedTreeNode == treeNode;
		}

		bool ITreeInfo.IsExpanded( TreeNode treeNode )
		{
			return _treeState.IsExpanded( treeNode );
		}

		double ITreeInfo.ExpansionAnimationPosition( TreeNode treeNode )
		{
			return _verticalPositioning.ExpansionAnimationPosition( treeNode );
		}

		bool ITreeInfo.IsMouseOverTree()
		{
			if( WinFormsUtility.Events.MenuLoop.InMenuLoop )
			{
				return false;
			}

			Form ownerForm = FindForm();

			if( ownerForm == null )
			{
				return false;
			}
			if( Form.ActiveForm != ownerForm )
			{
				return false;
			}

			return new Rectangle( 0, 0, Width, Height ).Contains( PointToClient( Control.MousePosition ) );
		}

		void ITreeInfo.GetMouseOver( out TreeNode treeNode, out Point nodeRelative )
		{
			if( !((ITreeInfo) this).IsMouseOverTree() )
			{
				treeNode = null;
				nodeRelative = Point.Empty;
				return;
			}

			Point p = PointToClient( Control.MousePosition );
			int x = p.X - AutoScrollPosition.X;

			if( x < 0 || x > ClientRectangle.Width )
			{
				treeNode = null;
				nodeRelative = Point.Empty;
				return;
			}

			int y = p.Y - AutoScrollPosition.Y;

			foreach( TreeNode tn in _verticalPositioning.GetNodesBetween( y, y ) )
			{
				Rectangle nodeBounds = _verticalPositioning.GetNodeBounds( tn, Internal.Coordinates.X | Internal.Coordinates.Y );

				treeNode = tn;
				nodeRelative = new Point( x - nodeBounds.X, y - nodeBounds.Y );

				return;
			}

			treeNode = null;
			nodeRelative = Point.Empty;
		}

		TreeNode[] ITreeInfo.GetVisibleNodes()
		{
			List<TreeNode> nodes = new List<TreeNode>();
			Utility.Collections.Set<TreeNode> disallowParents = new Utility.Collections.Set<TreeNode>();
			int top = ClientRectangle.Top, bottom = ClientRectangle.Bottom;

			top -= AutoScrollPosition.Y;
			bottom -= AutoScrollPosition.Y;

			foreach( TreeNode treeNode in _verticalPositioning.GetNodesBetween( top, bottom ) )
			{
				TreeNode parent = treeNode.ParentCollection.ParentNode;

				if( parent != null && disallowParents.Contains( parent ) )
				{
					if( !disallowParents.Contains( treeNode ) )
					{
						disallowParents.Add( treeNode );
					}
				}
				else
				{
					nodes.Add( treeNode );

					if( !_treeState.IsExpanded( treeNode ) && !disallowParents.Contains( treeNode ) )
					{
						disallowParents.Add( treeNode );
					}
				}
			}

			return nodes.ToArray();
		}

		#endregion

		private void _updatesSuspended_Reset( object sender, EventArgs e )
		{
			foreach( TreeNode node in _needsInvalidate )
			{
				InvalidateNode( node );
			}

			_needsInvalidate.Clear();
		}

		private void RecurseExpandAll( TreeNodeCollection nodes )
		{
			foreach( TreeNode child in nodes )
			{
				if( !_treeState.IsExpanded( child ) )
				{
					((ITreeEvents) this).ToggleNodeExpansion( child );
				}

				RecurseExpandAll( child.ChildNodes );
			}
		}

		private void ApplyUpdate()
		{
			if( _updatesSuspended.IsActive )
			{
				_updateTimer.Enabled = true;
				return;
			}

			if( _updateTimer != null )
			{
				_updateTimer.Enabled = false;
			}

			Invalidate();
		}

		private void SetNeedsUpdate()
		{
			_updateTimer.Enabled = true;
		}

		private void RecreateVerticalPositioning()
		{
			if( _animate )
			{
				_verticalPositioning = new Internal.AnimatedVerticalPositioning( _rootNodes, _renderer, this, this );
			}
			else
			{
				_verticalPositioning = new Internal.StaticVerticalPositioning( _rootNodes, _renderer, this, this );
			}

			ApplyUpdate();
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			ApplyUpdate();
		}

		private void _animationRequests_InvalidateTreeNode( object sender, TreeNodeRectangleEventArgs e )
		{
			Rectangle nodeBounds = _verticalPositioning.GetNodeBounds
				( e.Node, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );
			Rectangle subRect = new Rectangle( nodeBounds.X + e.Rectangle.X, nodeBounds.Y + e.Rectangle.Y, e.Rectangle.Width, e.Rectangle.Height );

			nodeBounds = new Rectangle( 0, nodeBounds.Y, AutoScrollMinSize.Width, nodeBounds.Height );
			nodeBounds.Intersect( subRect );

			nodeBounds.Offset( AutoScrollPosition.X, AutoScrollPosition.Y );

			Invalidate( nodeBounds );
		}

		private void _animationRequests_Invalidate( object sender, EventArgs e )
		{
			Invalidate();
			Update();
		}

		private void _animationRequests_Update( object sender, EventArgs e )
		{
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			Point newMousePosition = PointToClient( Control.MousePosition );

			Rectangle clientRect = new Rectangle( -AutoScrollPosition.X, -AutoScrollPosition.Y, ClientRectangle.Width, ClientRectangle.Height );
			TreeNode node = GetNodeAt( newMousePosition );

			newMousePosition.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );

			Rectangle allowedRect = new Rectangle
				( newMousePosition.X - SystemInformation.MouseHoverSize.Width / 2, newMousePosition.Y - SystemInformation.MouseHoverSize.Height / 2
				, SystemInformation.MouseHoverSize.Width, SystemInformation.MouseHoverSize.Height );

			if( node == _hoverNode && allowedRect.Contains( newMousePosition ) && clientRect.Contains( newMousePosition ) )
			{
				if( DateTime.Now.Subtract( _lastMouseTime ).TotalMilliseconds > SystemInformation.MouseHoverTime && Form.ActiveForm == FindForm() )
				{
					if( node != null )
					{
						Rectangle nodeBounds = _verticalPositioning.GetNodeBounds
							( node, Internal.Coordinates.X | Internal.Coordinates.Y | Internal.Coordinates.Width | Internal.Coordinates.Height );

						nodeBounds = new Rectangle( nodeBounds.X + 20, nodeBounds.Y, nodeBounds.Width - 20, nodeBounds.Height );

						if( !_hovering && nodeBounds.Contains( newMousePosition ) )
						{
							OnNodeMouseHover( new TreeNodeEventArgs( node ) );
							_hovering = true;
						}
					}
				}
			}
			else
			{
				_lastMousePosition = newMousePosition;
				_lastMouseTime = DateTime.Now;
				_hoverNode = node;
				_hovering = false;
			}
		}

		public event TreeNodeEventHandler AfterSelect;
		public event TreeNodeMouseEventHandler NodeMouseClick;
		public event TreeNodeMouseEventHandler NodeMouseDoubleClick;
		public event TreeNodeEventHandler NodeMouseHover;
		public event TreeNodeMouseEventHandler NodeDrag;

		private TreeNodeCollection _rootNodes;
		private Internal.TreeState _treeState;
		private Internal.VerticalPositioning _verticalPositioning;
		private IRenderer _renderer;
		private Utility.Control.Flag _updatesSuspended = new Utility.Control.Flag();
		private bool _animate = false;
		private Internal.AnimationRequests _animationRequests;
		private List<Icon> _icons = new List<Icon>();
		private Timer _timer;
		private Point _lastMousePosition;
		private DateTime _lastMouseTime;
		private Point _mouseDownPosition;
		private TreeNode _hoverNode;
		private bool _hovering;
		private Utility.Collections.Set<TreeNode> _needsInvalidate = new Utility.Collections.Set<TreeNode>();
		private bool _gotDragClick;
	}
}
