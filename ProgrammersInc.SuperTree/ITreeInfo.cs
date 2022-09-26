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
using System.Windows.Forms;

namespace ProgrammersInc.SuperTree
{
	public interface ITreeInfo
	{
		IDisposable SuspendUpdates();
		Graphics CreateGraphics();
		bool IsTreeFocused();
		bool IsUpdatesSuspended();
		bool IsMouseOverTree();
		List<Icon> Icons
		{
			get;
		}
		Size ViewportSize
		{
			get;
		}

		void BeginAnimating();
		void EndAnimating();
		void BeginAnimating( TreeNode treeNode, Rectangle subRect );
		void EndAnimating( TreeNode treeNode );

		bool IsSelected( TreeNode treeNode );
		bool IsExpanded( TreeNode treeNode );
		double ExpansionAnimationPosition( TreeNode treeNode );

		bool IsAnimating();

		void GetMouseOver( out TreeNode treeNode, out Point nodeRelative );
		TreeNode[] GetVisibleNodes();
		Size GetNodeSize( TreeNode treeNode );
	}
}
