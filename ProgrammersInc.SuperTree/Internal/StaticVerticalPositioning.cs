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
using System.Diagnostics;

namespace ProgrammersInc.SuperTree.Internal
{
	[Flags]
	internal enum Coordinates
	{
		None = 0,
		X = 1,
		Y = 2,
		Width = 4,
		Height = 8
	}

	internal sealed class StaticVerticalPositioning : VerticalPositioning
	{
		internal StaticVerticalPositioning( TreeNodeCollection nodes, IRenderer renderer, ITreeInfo treeInfo, ITreeEvents treeEvents )
			: base( nodes, renderer, treeInfo, treeEvents )
		{
		}

		internal override double ExpansionAnimationPosition( TreeNode treeNode )
		{
			return TreeInfo.IsExpanded( treeNode ) ? 1 : 0;
		}

		internal override bool IsAnimating()
		{
			return false;
		}

		internal override bool IsVisible( TreeNode treeNode )
		{
			if( _orderedNodes == null )
			{
				Calculate( null, Coordinates.None );
			}

			return _orderedNodes.Contains( treeNode );
		}

		internal override Rectangle GetNodeBounds( TreeNode treeNode, Coordinates required )
		{
			NodeBounds bounds;

			if( !_mapBounds.TryGetValue( treeNode, out bounds ) )
			{
				bounds = new NodeBounds();
				_mapBounds[treeNode] = bounds;
			}

			if( ((required & Coordinates.X) != 0 && bounds.X == null)
				|| ((required & Coordinates.Y) != 0 && bounds.Y == null)
				|| ((required & Coordinates.Width) != 0 && bounds.Width == null)
				|| ((required & Coordinates.Height) != 0 && bounds.Height == null) )
			{
				Calculate( treeNode, required );
			}

			Rectangle rect = new Rectangle( bounds.X ?? 0, bounds.Y ?? 0, bounds.Width ?? 0, bounds.Height ?? 0 );
			
			return rect;
		}

		internal override TreeNode[] GetNodesBetween( int top, int bottom )
		{
			if( _totalHeight == null )
			{
				Calculate( null, Coordinates.Y | Coordinates.Height );
			}
			if( top > bottom || bottom < 0 || top > _totalHeight || _orderedNodes.Count == 0 )
			{
				return new TreeNode[] { };
			}

			int first = _orderedNodes.BinarySearch( null, new PositionComparer( _mapBounds, top ) );
			int last = _orderedNodes.BinarySearch( null, new PositionComparer( _mapBounds, bottom ) );

			if( first < 0 )
			{
				first = ~first;
			}
			if( last < 0 )
			{
				last = ~last;
			}

			List<TreeNode> nodes = new List<TreeNode>();

			for( int i = first; i <= last; ++i )
			{
				if( i < _orderedNodes.Count )
				{
					nodes.Add( _orderedNodes[i] );
				}
			}

			return nodes.ToArray();
		}

		internal override TreeNode GetNodeAfter( TreeNode node )
		{
			if( _orderedNodes == null )
			{
				Calculate( null, Coordinates.None );
			}

			int index = _orderedNodes.IndexOf( node );

			if( index < _orderedNodes.Count - 1 )
			{
				return _orderedNodes[index + 1];
			}
			else
			{
				return null;
			}
		}

		internal override TreeNode GetNodeBefore( TreeNode node )
		{
			if( _orderedNodes == null )
			{
				Calculate( null, Coordinates.None );
			}

			int index = _orderedNodes.IndexOf( node );

			if( index > 0 )
			{
				return _orderedNodes[index - 1];
			}
			else
			{
				return null;
			}
		}

		internal override int GetTotalHeight()
		{
			if( _totalHeight == null )
			{
				Calculate( null, Coordinates.Y | Coordinates.Height );
			}

			return _totalHeight.Value;
		}

		internal override int GetMaxWidth()
		{
			if( _maxWidth == null )
			{
				Calculate( null, Coordinates.X | Coordinates.Width );
			}

			return _maxWidth.Value;
		}

		internal override void DirtyWidths()
		{
			_maxWidth = null;

			foreach( KeyValuePair<TreeNode, NodeBounds> kvp in _mapBounds )
			{
				kvp.Value.Width = null;
			}
		}

		internal override void SetAnimationMark( DateTime dateTime )
		{
		}

		internal StaticVerticalPositioning Copy()
		{
			StaticVerticalPositioning svp = new StaticVerticalPositioning( Nodes, Renderer, TreeInfo, TreeEvents );

			foreach( KeyValuePair<TreeNode, NodeBounds> kvp in _mapBounds )
			{
				svp._mapBounds[kvp.Key] = kvp.Value.Copy();
			}

			svp._maxWidth = _maxWidth;

			if( _orderedNodes != null )
			{
				svp._orderedNodes = new List<TreeNode>();
				svp._orderedNodes.AddRange( _orderedNodes );
			}

			svp._totalHeight = _totalHeight;

			return svp;
		}

		#region ITreeEvents Members

		public override void NodeUpdated( TreeNode treeNode )
		{
			using( Graphics g = TreeInfo.CreateGraphics() )
			{
				NodeBounds nb;

				if( _mapBounds.TryGetValue( treeNode, out nb ) )
				{
					bool needWidth = _maxWidth.HasValue;
					Size size = Renderer.MeasureTreeNode( g, TreeInfo, treeNode, needWidth, true );

					if( nb.Width == null || size.Width != nb.Width.Value )
					{
						if( needWidth )
						{
							nb.Width = size.Width;
						}
						else
						{
							nb.Width = null;
						}

						if( _maxWidth.HasValue && nb.X != null )
						{
							_maxWidth = Math.Max( _maxWidth.Value, nb.X.Value + nb.Width.Value + 4 );
						}
					}

					if( nb.Height != null && size.Height != nb.Height )
					{
						DirtyVerticals();
					}
				}
			}
		}

		public override void NodeInserted( TreeNode treeNode )
		{
			DirtyVerticals();
		}

		public override void NodeDeleted( TreeNode treeNode )
		{
			DirtyVerticals();
		}

		public override void ToggleNodeExpansion( TreeNode treeNode )
		{
			DirtyVerticals();
		}

		public override void SelectNode( TreeNode treeNode )
		{
		}

		public override void UpdateAnimations()
		{
		}

		#endregion

		private void DirtyVerticals()
		{
			_maxWidth = null;
			_totalHeight = null;
			_orderedNodes = null;
			_mapBounds.Clear();
		}

		private void Calculate( TreeNode stop, Coordinates required )
		{
			int ypos = 4;

			if( stop == null )
			{
				_orderedNodes = new List<TreeNode>();
			}

			if( (required & Coordinates.X) != 0 && (required & Coordinates.Width) != 0 )
			{
				_maxWidth = 0;
			}

			using( Graphics g = TreeInfo.CreateGraphics() )
			{
				CalculateCollection( g, Nodes, stop, required, ref ypos );
			}

			if( stop == null && (required & Coordinates.Y) != 0 && (required & Coordinates.Height) != 0 )
			{
				_totalHeight = ypos + 4;
			}
		}

		private bool CalculateCollection( Graphics g, TreeNodeCollection nodes, TreeNode stop, Coordinates required, ref int ypos )
		{
			foreach( TreeNode node in nodes )
			{
				NodeBounds nb;

				if( !_mapBounds.TryGetValue( node, out nb ) )
				{
					nb = new NodeBounds();
					_mapBounds[node] = nb;
				}

				if( (required & Coordinates.X) != 0 )
				{
					if( nb.X == null )
					{
						nb.X = Renderer.MeasureIndent( g, TreeInfo, node );
					}
				}
				if( (required & Coordinates.Y) != 0 )
				{
					if( nb.Y == null )
					{
						nb.Y = ypos;
					}
				}
				if( (required & Coordinates.Width) != 0 || (required & Coordinates.Height) != 0 )
				{
					Size size = Renderer.MeasureTreeNode( g, TreeInfo, node, (required & Coordinates.Width) != 0, (required & Coordinates.Height) != 0);

					if( nb.Width == null && (required & Coordinates.Width) != 0 )
					{
						nb.Width = size.Width;
					}
					if( nb.Height == null && (required & Coordinates.Height) != 0 )
					{
						nb.Height = size.Height;
					}

					if( (required & Coordinates.Height) != 0 )
					{
						Debug.Assert( size.Height > 0 );

						ypos += size.Height;
					}
				}

				if( (required & Coordinates.X) != 0 && (required & Coordinates.Width) != 0 )
				{
					_maxWidth = Math.Max( _maxWidth.Value, nb.X.Value + nb.Width.Value + 4 );
				}

				if( node == stop )
				{
					return false;
				}
				else
				{
					if( stop == null )
					{
						_orderedNodes.Add( node );
					}
				}

				if( TreeInfo.IsExpanded( node ) )
				{
					if( !CalculateCollection( g, node.ChildNodes, stop, required, ref ypos ) )
					{
						return false;
					}
				}
			}

			return true;
		}

		#region TopComparer

		private sealed class PositionComparer : IComparer<TreeNode>
		{
			internal PositionComparer( Dictionary<TreeNode, NodeBounds> mapBounds, int pos )
			{
				_mapBounds = mapBounds;
				_pos = pos;
			}

			#region IComparer<TreeNode> Members

			public int Compare( TreeNode x, TreeNode y )
			{
				NodeBounds bounds;
				int rev = 1;

				if( x == null && y == null )
				{
					throw new InvalidOperationException();
				}
				else if( x != null && y != null )
				{
					throw new InvalidOperationException();
				}
				else if( x != null )
				{
					bounds = _mapBounds[x];
				}
				else if( y != null )
				{
					bounds = _mapBounds[y];
					rev = -1;
				}
				else
				{
					throw new InvalidOperationException();
				}

				if( _pos < bounds.Y.Value )
				{
					return rev;
				}
				else if( _pos > bounds.Y.Value + bounds.Height.Value )
				{
					return -rev;
				}
				else
				{
					return 0;
				}
			}

			#endregion

			private Dictionary<TreeNode, NodeBounds> _mapBounds;
			private int _pos;
		}

		#endregion

		#region NodeBounds

		private sealed class NodeBounds
		{
			internal int? X;
			internal int? Y;
			internal int? Width;
			internal int? Height;

			internal NodeBounds Copy()
			{
				NodeBounds nb = new NodeBounds();

				nb.X = X;
				nb.Y = Y;
				nb.Width = Width;
				nb.Height = Height;

				return nb;
			}
		}

		#endregion

		private Dictionary<TreeNode, NodeBounds> _mapBounds = new Dictionary<TreeNode, NodeBounds>();
		private int? _totalHeight;
		private int? _maxWidth;
		private List<TreeNode> _orderedNodes;
	}
}
