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

namespace ProgrammersInc.SuperTree
{
	public interface ITreeEvents
	{
		void NodeUpdated( TreeNode treeNode );
		void NodeInserted( TreeNode treeNode );
		void NodeDeleted( TreeNode treeNode );

		void ToggleNodeExpansion( TreeNode treeNode );
		void SelectNode( TreeNode treeNode );

		void UpdateAnimations();
	}
}
