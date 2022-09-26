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

namespace ProgrammersInc.SuperTree.Internal
{
	internal sealed class AnimatedVerticalPositioning : VerticalPositioning
	{
		internal AnimatedVerticalPositioning( TreeNodeCollection nodes, IRenderer renderer, ITreeInfo treeInfo, ITreeEvents treeEvents )
			: base( nodes, renderer, treeInfo, treeEvents )
		{
			_from = new StaticVerticalPositioning( nodes, renderer, treeInfo, treeEvents );
			_to = new StaticVerticalPositioning( nodes, renderer, treeInfo, treeEvents );
		}

		internal override double ExpansionAnimationPosition( TreeNode treeNode )
		{
			if( _treeNode == treeNode )
			{
				return TreeInfo.IsExpanded( treeNode ) ? Proportion : 1 - Proportion;
			}
			else
			{
				return TreeInfo.IsExpanded( treeNode ) ? 1 : 0;
			}
		}

		internal override bool IsVisible( TreeNode treeNode )
		{
			if( _animating )
			{
				return _from.IsVisible( treeNode ) || _to.IsVisible( treeNode );
			}
			else
			{
				return _from.IsVisible( treeNode );
			}
		}

		internal override int GetMaxWidth()
		{
			if( _animating )
			{
				return GetValue( _from.GetMaxWidth(), _to.GetMaxWidth() );
			}
			else
			{
				return _from.GetMaxWidth();
			}
		}

		internal override int GetTotalHeight()
		{
			if( _animating )
			{
				return GetValue( _from.GetTotalHeight(), _to.GetTotalHeight() );
			}
			else
			{
				return _from.GetTotalHeight();
			}
		}

		internal override bool IsAnimating()
		{
			return _animating;
		}

		internal override Rectangle GetNodeBounds( TreeNode treeNode, Coordinates required )
		{
			if( !_animating )
			{
				return GetBoundsHelper( _from, treeNode, required );
			}

			Rectangle from = GetBoundsHelper( _from, treeNode, required );
			Rectangle to = GetBoundsHelper( _to, treeNode, required );

			int x = Math.Max( from.X, to.X );
			int y = GetValue( from.Y, to.Y );
			int w = Math.Max( from.Width, to.Width );
			int h = Math.Max( from.Height, to.Height );

			if( _animating )
			{
				if( _expanding )
				{
					w = to.Width;
				}
				else
				{
					w = from.Width;
				}
			}

			return new Rectangle( x, y, w, h );
		}

		internal override TreeNode[] GetNodesBetween( int top, int bottom )
		{
			List<TreeNode> nodes = new List<TreeNode>();

			if( _animating )
			{
				bottom += (int) _distance;

				int extra = Math.Abs( GetValue( 0, _movement ) );
				int changeBottom = _changeTop + extra;
				Utility.Collections.Set<TreeNode> done = new Utility.Collections.Set<TreeNode>();
				StaticVerticalPositioning source;

				if( _expanding )
				{
					source = _to;
				}
				else
				{
					source = _from;
				}

				foreach( TreeNode tn in source.GetNodesBetween( top, _changeTop ) )
				{
					if( !done.Contains( tn ) )
					{
						nodes.Add( tn );
						done.Add( tn );
					}
				}
				foreach( TreeNode tn in source.GetNodesBetween( Math.Max( top, _changeTop ), Math.Min( bottom - extra, changeBottom ) ) )
				{
					if( !done.Contains( tn ) )
					{
						nodes.Add( tn );
						done.Add( tn );
					}
				}
				foreach( TreeNode tn in source.GetNodesBetween( changeBottom, bottom ) )
				{
					if( !done.Contains( tn ) )
					{
						nodes.Add( tn );
						done.Add( tn );
					}
				}
			}
			else
			{
				foreach( TreeNode tn in _from.GetNodesBetween( top, bottom ) )
				{
					nodes.Add( tn );
				}
			}

			return nodes.ToArray();
		}

		internal override void DirtyWidths()
		{
			_from.DirtyWidths();
			_to.DirtyWidths();
		}

		internal override void SetAnimationMark( DateTime dateTime )
		{
			_mark = dateTime;
		}

		internal override TreeNode GetNodeAfter( TreeNode node )
		{
			return _from.GetNodeAfter( node );
		}

		internal override TreeNode GetNodeBefore( TreeNode node )
		{
			return _from.GetNodeBefore( node );
		}

		public override void NodeUpdated( TreeNode treeNode )
		{
			_from.NodeUpdated( treeNode );
			_to.NodeUpdated( treeNode );
		}

		public override void NodeDeleted( TreeNode treeNode )
		{
			if( _animating )
			{
				_to.NodeDeleted( treeNode );
			}
			else if( TreeInfo.IsUpdatesSuspended() )
			{
				_from.NodeDeleted( treeNode );
				_to.NodeDeleted( treeNode );
			}
			else
			{
				_start = DateTime.Now;
				_treeNode = treeNode;
				_expanding = false;
				_animating = true;

				_to.NodeDeleted( treeNode );

				TreeInfo.BeginAnimating();
			}
		}

		public override void NodeInserted( TreeNode treeNode )
		{
			if( _animating )
			{
				_to.NodeInserted( treeNode );
			}
			else if( TreeInfo.IsUpdatesSuspended() )
			{
				_from.NodeInserted( treeNode );
				_to.NodeInserted( treeNode );
			}
			else
			{
				_start = DateTime.Now;
				_treeNode = treeNode;
				_expanding = true;
				_animating = true;

				_to.NodeInserted( treeNode );

				TreeInfo.BeginAnimating();
			}
		}

		public override void SelectNode( TreeNode treeNode )
		{
			_from.SelectNode( treeNode );
			_to.SelectNode( treeNode );
		}

		public override void ToggleNodeExpansion( TreeNode treeNode )
		{
			if( _animating )
			{
				_to.ToggleNodeExpansion( treeNode );
			}
			else if( TreeInfo.IsUpdatesSuspended() )
			{
				_from.ToggleNodeExpansion( treeNode );
				_to.ToggleNodeExpansion( treeNode );
			}
			else
			{
				_changeTop = GetNodeBounds( treeNode, Coordinates.Y | Coordinates.Height ).Bottom;

				_start = DateTime.Now;
				_treeNode = treeNode;
				_expanding = treeNode.IsExpanded;

				_to.ToggleNodeExpansion( treeNode );

				if( !_animating )
				{
					_animating = true;
					TreeInfo.BeginAnimating();
				}

				_distance = Math.Abs( _from.GetTotalHeight() - _to.GetTotalHeight() );
				_movement = _to.GetTotalHeight() - _from.GetTotalHeight();
			}
		}

		public override void UpdateAnimations()
		{
			if( _treeNode != null && Proportion >= 1 )
			{
				if( _animating )
				{
					TreeInfo.EndAnimating();
					_animating = false;
				}

				_from = _to.Copy();
				_treeNode = null;
			}
		}

		private int GetValue( int from, int to )
		{
			if( from == to )
			{
				return from;
			}

			if( _expanding )
			{
				return (int) (to - _movement * (1 - Proportion));
			}
			else
			{
				return (int) (from + _movement * Proportion);
			}
		}

		private double Proportion
		{
			get
			{
				double time = 0.2;
				double secs = _mark.Subtract( _start ).TotalSeconds;

				double prop = secs / time;

				if( prop < 0 )
				{
					prop = 0;
				}
				else if( prop > 1 )
				{
					prop = 1;
				}

				return prop;
			}
		}

		private Rectangle GetBoundsHelper( StaticVerticalPositioning vp, TreeNode tn, Coordinates required )
		{
			while( tn != null )
			{
				if( vp.IsVisible( tn ) )
				{
					return vp.GetNodeBounds( tn, required );
				}

				if( tn.ParentCollection == null )
				{
					break;
				}
				tn = tn.ParentCollection.ParentNode;
			}

			return Rectangle.Empty;
		}

		private StaticVerticalPositioning _from, _to;
		private DateTime _start;
		private TreeNode _treeNode;
		private bool _expanding, _animating;
		private double _distance;
		private int _movement, _changeTop;
		private DateTime _mark;
	}
}
