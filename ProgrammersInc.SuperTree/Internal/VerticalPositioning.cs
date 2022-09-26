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
	internal abstract class VerticalPositioning : ITreeEvents
	{
		protected VerticalPositioning( TreeNodeCollection nodes, IRenderer renderer, ITreeInfo treeInfo, ITreeEvents treeEvents )
		{
			_nodes = nodes;
			_renderer = renderer;
			_treeInfo = treeInfo;
			_treeEvents = treeEvents;
		}

		internal abstract double ExpansionAnimationPosition( TreeNode treeNode );

		internal abstract bool IsAnimating();

		internal abstract Rectangle GetNodeBounds( TreeNode treeNode, Coordinates required );

		internal abstract bool IsVisible( TreeNode treeNode );

		internal abstract TreeNode[] GetNodesBetween( int top, int bottom );

		internal abstract TreeNode GetNodeBefore( TreeNode node );
		internal abstract TreeNode GetNodeAfter( TreeNode node );

		internal abstract int GetTotalHeight();
		internal abstract int GetMaxWidth();

		internal abstract void DirtyWidths();
		internal abstract void SetAnimationMark( DateTime dateTime );

		#region ITreeEvents Members

		public abstract void NodeUpdated( TreeNode treeNode );

		public abstract void NodeInserted( TreeNode treeNode );

		public abstract void NodeDeleted( TreeNode treeNode );

		public abstract void ToggleNodeExpansion( TreeNode treeNode );

		public abstract void SelectNode( TreeNode treeNode );

		public abstract void UpdateAnimations();

		#endregion

		protected TreeNodeCollection Nodes
		{
			[DebuggerStepThrough]
			get
			{
				return _nodes;
			}
		}

		protected IRenderer Renderer
		{
			[DebuggerStepThrough]
			get
			{
				return _renderer;
			}
		}

		protected ITreeInfo TreeInfo
		{
			[DebuggerStepThrough]
			get
			{
				return _treeInfo;
			}
		}

		protected ITreeEvents TreeEvents
		{
			[DebuggerStepThrough]
			get
			{
				return _treeEvents;
			}
		}

		private TreeNodeCollection _nodes;
		private IRenderer _renderer;
		private ITreeInfo _treeInfo;
		private ITreeEvents _treeEvents;
	}
}
