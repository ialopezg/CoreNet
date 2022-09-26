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
using System.Diagnostics;
using System.Drawing;

namespace ProgrammersInc.SuperTree
{
	public sealed class TreeNode
	{
		internal TreeNode( ITreeEvents treeEvents, ITreeInfo treeInfo )
		{
			_treeEvents = treeEvents;
			_treeInfo = treeInfo;
			_childNodes = new TreeNodeCollection( this, treeEvents, treeInfo );
		}

		public string Text
		{
			[DebuggerStepThrough]
			get
			{
				return _text;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}
				if( _text == value )
				{
					return;
				}

				_text = value;

				_treeEvents.NodeUpdated( this );
			}
		}

		public object Tag
		{
			[DebuggerStepThrough]
			get
			{
				return _tag;
			}
			[DebuggerStepThrough]
			set
			{
				_tag = value;

				_treeEvents.NodeUpdated( this );
			}
		}

		public int ExpandedImageIndex
		{
			[DebuggerStepThrough]
			get
			{
				return _expandedImageIndex;
			}
			set
			{
				_expandedImageIndex = value;

				_treeEvents.NodeUpdated( this );
			}
		}

		public int CollapsedImageIndex
		{
			[DebuggerStepThrough]
			get
			{
				return _collapsedImageIndex;
			}
			set
			{
				_collapsedImageIndex = value;

				_treeEvents.NodeUpdated( this );
			}
		}

		public TreeNodeCollection ChildNodes
		{
			[DebuggerStepThrough]
			get
			{
				return _childNodes;
			}
		}

		public TreeNodeCollection ParentCollection
		{
			[DebuggerStepThrough]
			get
			{
				return _parentCollection;
			}
			[DebuggerStepThrough]
			internal set
			{
				_parentCollection = value;
			}
		}

		public int Index
		{
			get
			{
				if( _index == null )
				{
					_index = _parentCollection.IndexOf( this );
				}

				return _index.Value;
			}
		}

		public int Depth
		{
			get
			{
				if( _depth == null )
				{
					int d = 0;

					TreeNode tn = this.ParentCollection.ParentNode;

					while( tn != null )
					{
						++d;
						tn = tn.ParentCollection.ParentNode;
					}

					_depth = d;
				}

				return _depth.Value;
			}
		}

		public bool IsExpanded
		{
			get
			{
				return _treeInfo.IsExpanded( this );
			}
		}

		public Icon Icon
		{
			get
			{
				if( IsExpanded )
				{
					if( _expandedImageIndex == -1 )
					{
						return null;
					}
					else
					{
						return _treeInfo.Icons[_expandedImageIndex];
					}
				}
				else
				{
					if( _collapsedImageIndex == -1 )
					{
						return null;
					}
					else
					{
						return _treeInfo.Icons[_collapsedImageIndex];
					}
				}
			}
		}

		public Font Font
		{
			get
			{
				return _font;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				if( _font.Equals( value ) )
				{
					return;
				}

				_font = value;
				_treeEvents.NodeUpdated( this );
			}
		}

		internal void DirtyIndex()
		{
			_index = null;
			_depth = null;
		}

		private TreeNodeCollection _parentCollection;
		private ITreeInfo _treeInfo;
		private ITreeEvents _treeEvents;
		private string _text = string.Empty;
		private TreeNodeCollection _childNodes;
		private object _tag;
		private int? _index;
		private int? _depth;
		private int _expandedImageIndex = -1, _collapsedImageIndex = -1;
		private Font _font = SystemFonts.DialogFont;
	}

	public sealed class TreeNodeCollection : IEnumerable<TreeNode>
	{
		internal TreeNodeCollection( TreeNode parentNode, ITreeEvents treeEvents, ITreeInfo treeInfo )
		{
			_parentNode = parentNode;
			_treeEvents = treeEvents;
			_treeInfo = treeInfo;
		}

		public int Count
		{
			[DebuggerStepThrough]
			get
			{
				return _nodes.Count;
			}
		}

		public TreeNode this[int index]
		{
			[DebuggerStepThrough]
			get
			{
				return _nodes[index];
			}
		}

		public TreeNode ParentNode
		{
			[DebuggerStepThrough]
			get
			{
				return _parentNode;
			}
		}

		public TreeNode Add()
		{
			TreeNode treeNode = new TreeNode( _treeEvents, _treeInfo );

			treeNode.ParentCollection = this;
			_nodes.Add( treeNode );

			foreach( TreeNode node in _nodes )
			{
				node.DirtyIndex();
			}

			_treeEvents.NodeInserted( treeNode );

			return treeNode;
		}

		public TreeNode Insert( int index )
		{
			if( index > _nodes.Count )
			{
				throw new ArgumentOutOfRangeException( "index" );
			}

			TreeNode treeNode = new TreeNode( _treeEvents, _treeInfo );

			treeNode.ParentCollection = this;
			_nodes.Insert( index, treeNode );

			foreach( TreeNode node in _nodes )
			{
				node.DirtyIndex();
			}

			_treeEvents.NodeInserted( treeNode );

			return treeNode;
		}

		public void Move( TreeNode node, int newIndex )
		{
			if( node == null )
			{
				throw new ArgumentNullException( "node" );
			}
			if( newIndex > _nodes.Count )
			{
				throw new ArgumentOutOfRangeException( "index" );
			}

			using( _treeInfo.SuspendUpdates() )
			{
				_nodes.Remove( node );

				_treeEvents.NodeDeleted( node );

				if( newIndex > _nodes.Count )
				{
					_nodes.Add( node );
				}
				else
				{
					_nodes.Insert( newIndex, node );
				}

				foreach( TreeNode child in _nodes )
				{
					child.DirtyIndex();
				}

				_treeEvents.NodeInserted( node );
			}
		}

		public void Remove( TreeNode treeNode )
		{
			if( treeNode == null )
			{
				throw new ArgumentNullException( "treeNode" );
			}
			if( !_nodes.Contains( treeNode ) )
			{
				throw new ArgumentException( "Node is not a member of this collection.", "treeNode" );
			}

			foreach( TreeNode node in _nodes )
			{
				node.DirtyIndex();
			}

			_nodes.Remove( treeNode );
			_treeEvents.NodeDeleted( treeNode );
			treeNode.ParentCollection = null;
		}

		public void Clear()
		{
			using( _treeInfo.SuspendUpdates() )
			{
				List<TreeNode> nodes = new List<TreeNode>( _nodes );

				foreach( TreeNode child in nodes )
				{
					Remove( child );
				}
			}
		}

		internal int IndexOf( TreeNode treeNode )
		{
			return _nodes.IndexOf( treeNode );
		}

		#region IEnumerable<TreeNode> Members

		IEnumerator<TreeNode> IEnumerable<TreeNode>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		#endregion

		private TreeNode _parentNode;
		private ITreeEvents _treeEvents;
		private ITreeInfo _treeInfo;
		private List<TreeNode> _nodes = new List<TreeNode>();
	}
}
