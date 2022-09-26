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

namespace ProgrammersInc.SuperTree
{
	public interface IRenderer
	{
		void Setup();
		void Setdown();

		void PreRender( ITreeInfo treeInfo, ITreeEvents treeEvents );
		int MeasureIndent( Graphics g, ITreeInfo treeControl, TreeNode node );
		Size MeasureTreeNode( Graphics g, ITreeInfo treeInfo, TreeNode treeNode, bool needsWidth, bool needsHeight );

		void RenderBackground( Graphics g, Rectangle clip );
		void RenderTreeNode( Graphics g, ITreeInfo treeInfo, TreeNode treeNode, Rectangle nodeRectangle, Rectangle clip );
		void RenderTreeNodeRow( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Rectangle rowRectangle );

		void ProcessClick( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Point p, ITreeInfo treeInfo, ITreeEvents treeEvents );
		void ProcessDoubleClick( Graphics g, TreeNode treeNode, Rectangle nodeRectangle, Point p, ITreeInfo treeInfo, ITreeEvents treeEvents );
	}
}
