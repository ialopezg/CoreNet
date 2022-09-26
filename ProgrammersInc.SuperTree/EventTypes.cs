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
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.SuperTree
{
	#region TreeNodeEvent

	public class TreeNodeEventArgs : EventArgs
	{
		public TreeNodeEventArgs( TreeNode treeNode )
		{
			if( treeNode == null )
			{
				throw new ArgumentNullException( "treeNode" );
			}

			_treeNode = treeNode;
		}

		public TreeNode Node
		{
			get
			{
				return _treeNode;
			}
		}

		private TreeNode _treeNode;
	}

	public delegate void TreeNodeEventHandler( object sender, TreeNodeEventArgs e );

	#endregion

	#region TreeNodeMouseEvent

	public class TreeNodeMouseEventArgs : TreeNodeEventArgs
	{
		public TreeNodeMouseEventArgs( TreeNode treeNode, MouseButtons buttons )
			: base( treeNode )
		{
			_buttons = buttons;
		}

		public MouseButtons Buttons
		{
			get
			{
				return _buttons;
			}
		}

		private MouseButtons _buttons;
	}

	public delegate void TreeNodeMouseEventHandler( object sender, TreeNodeMouseEventArgs e );

	#endregion


	#region TreeNodeRectangleEvent

	public class TreeNodeRectangleEventArgs : TreeNodeEventArgs
	{
		public TreeNodeRectangleEventArgs( TreeNode treeNode, Rectangle rect )
			: base( treeNode )
		{
			_rect = rect;
		}

		public Rectangle Rectangle
		{
			get
			{
				return _rect;
			}
		}

		private Rectangle _rect;
	}

	public delegate void TreeNodeRectangleEventHandler( object sender, TreeNodeRectangleEventArgs e );

	#endregion
}
