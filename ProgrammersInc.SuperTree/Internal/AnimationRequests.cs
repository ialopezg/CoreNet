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
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.SuperTree.Internal
{
	internal sealed class AnimationRequests : IDisposable
	{
		internal AnimationRequests( ITreeEvents treeEvents )
		{
			_treeEvents = treeEvents;

			_timer.Interval = 100;
			_timer.Tick += new EventHandler( _timer_Tick );
		}

		#region IDisposable Members

		public void Dispose()
		{
			if( _timer != null )
			{
				_timer.Enabled = false;
				_timer.Tick -= new EventHandler( _timer_Tick );
				_timer.Dispose();
				_timer = null;
			}
		}

		#endregion

		internal void BeginAnimating()
		{
			++_allCount;

			UpdateTimer();
		}

		internal void EndAnimating()
		{
			Debug.Assert( _allCount > 0 );

			--_allCount;

			UpdateTimer();
		}

		internal void BeginAnimating( TreeNode treeNode, Rectangle subRect )
		{
			_toAdd.Add( new NodeAndSubRect( treeNode, subRect ) );
			
			UpdateTimer();
		}

		internal void EndAnimating( TreeNode treeNode )
		{
			_toRemove.Add( new NodeAndSubRect( treeNode, Rectangle.Empty ) );

			UpdateTimer();
		}

		private void DoAdd( NodeAndSubRect nodeAndSubRect )
		{
			CountAndSubRect countAndSubRect;

			if( !_nodeCounts.TryGetValue( nodeAndSubRect.TreeNode, out countAndSubRect ) )
			{
				countAndSubRect = new CountAndSubRect();
				countAndSubRect.SubRect = nodeAndSubRect.SubRect;

				_nodeCounts[nodeAndSubRect.TreeNode] = countAndSubRect;
			}

			++countAndSubRect.Count;

			countAndSubRect.SubRect = Rectangle.Union( countAndSubRect.SubRect, nodeAndSubRect.SubRect );

			UpdateTimer();
		}

		private void DoRemove( NodeAndSubRect nodeAndSubRect )
		{
			CountAndSubRect countAndSubRect;

			if( _nodeCounts.TryGetValue( nodeAndSubRect.TreeNode, out countAndSubRect ) )
			{
				--countAndSubRect.Count;

				if( countAndSubRect.Count <= 0 )
				{
					_nodeCounts.Remove( nodeAndSubRect.TreeNode );
				}
			}

			UpdateTimer();
		}

		private void UpdateTimer()
		{
			_timer.Enabled = (_allCount > 0 || _nodeCounts.Count > 0 || _toAdd.Count > 0 || _toRemove.Count > 0);
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			foreach( NodeAndSubRect nodeAndSubRect in _toAdd )
			{
				DoAdd( nodeAndSubRect );
			}
			_toAdd.Clear();

			bool needsUpdate = false;

			if( _allCount > 0 )
			{
				if( Invalidate != null )
				{
					Invalidate( this, EventArgs.Empty );
					needsUpdate = true;
				}
			}
			else
			{
				foreach( KeyValuePair<TreeNode, CountAndSubRect> kvp in _nodeCounts )
				{
					TreeNode treeNode = kvp.Key;
					CountAndSubRect countAndSubRect = kvp.Value;

					if( InvalidateTreeNode != null )
					{
						InvalidateTreeNode( this, new TreeNodeRectangleEventArgs( treeNode, countAndSubRect.SubRect ) );
						needsUpdate = true;
					}
				}
			}

			if( needsUpdate && Update != null )
			{
				Update( this, EventArgs.Empty );
			}

			foreach( NodeAndSubRect nodeAndSubRect in _toRemove )
			{
				DoRemove( nodeAndSubRect );
			}
			_toRemove.Clear();

			_treeEvents.UpdateAnimations();
		}

		private sealed class NodeAndSubRect
		{
			internal NodeAndSubRect( TreeNode treeNode, Rectangle subRect )
			{
				TreeNode = treeNode;
				SubRect = subRect;
			}

			internal TreeNode TreeNode;
			internal Rectangle SubRect;
		}

		private sealed class CountAndSubRect
		{
			internal int Count;
			internal Rectangle SubRect;
		}

		public event EventHandler Invalidate;
		public event TreeNodeRectangleEventHandler InvalidateTreeNode;
		public event EventHandler Update;

		private ITreeEvents _treeEvents;
		private int _allCount;
		private Dictionary<TreeNode, CountAndSubRect> _nodeCounts = new Dictionary<TreeNode, CountAndSubRect>();
		private Timer _timer = new Timer();
		private List<NodeAndSubRect> _toAdd = new List<NodeAndSubRect>(), _toRemove = new List<NodeAndSubRect>();
	}
}
