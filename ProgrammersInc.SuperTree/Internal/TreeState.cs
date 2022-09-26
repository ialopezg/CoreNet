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

namespace ProgrammersInc.SuperTree.Internal
{
	internal sealed class TreeState : ITreeEvents
	{
		internal TreeState( TreeNodeCollection nodes, ITreeEvents treeEvents )
		{
			_nodes = nodes;
			_treeEvents = treeEvents;
		}

		internal TreeNode SelectedTreeNode
		{
			get
			{
				return _selectedNode;
			}
		}

		internal bool IsExpanded( TreeNode treeNode )
		{
			return _mapExpansionState[treeNode];
		}

		#region ITreeEvents Members

		public void NodeUpdated( TreeNode treeNode )
		{
		}

		public void NodeInserted( TreeNode treeNode )
		{
			_mapExpansionState.Add( treeNode, false );
		}

		public void NodeDeleted( TreeNode treeNode )
		{
			_mapExpansionState.Remove( treeNode );
		}

		public void ToggleNodeExpansion( TreeNode treeNode )
		{
			_mapExpansionState[treeNode] = !_mapExpansionState[treeNode];

			if( _selectedNode != null && !_mapExpansionState[treeNode] )
			{
				if( IsParentOf( treeNode, _selectedNode ) )
				{
					_treeEvents.SelectNode( treeNode );
				}
			}
		}

		public void SelectNode( TreeNode treeNode )
		{
			if( _selectedNode == treeNode )
			{
				return;
			}

			if( _selectedNode != null )
			{
				_treeEvents.NodeUpdated( _selectedNode );
			}

			_selectedNode = treeNode;

			if( _selectedNode != null )
			{
				_treeEvents.NodeUpdated( _selectedNode );
			}
		}

		public void UpdateAnimations()
		{
		}

		#endregion

		private bool IsParentOf( TreeNode t1, TreeNode t2 )
		{
			if( t1 == t2 )
			{
				return false;
			}

			while( t2 != null )
			{
				if( t1 == t2 )
				{
					return true;
				}

				if( t2.ParentCollection == null )
				{
					t2 = null;
				}
				else
				{
					t2 = t2.ParentCollection.ParentNode;
				}
			}

			return false;
		}

		private TreeNodeCollection _nodes;
		private TreeNode _selectedNode;
		private Dictionary<TreeNode, bool> _mapExpansionState = new Dictionary<TreeNode, bool>();
		private ITreeEvents _treeEvents;
	}
}
