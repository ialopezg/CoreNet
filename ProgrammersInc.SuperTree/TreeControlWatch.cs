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
	public class TreeControlWatch : WinFormsUtility.ControlPreferences.Watch
	{
		public TreeControlWatch( TreeControl treeControl, string id )
			: base( treeControl, id )
		{
			_treeControl = treeControl;
		}

		protected override void OnRegistered()
		{
			base.OnRegistered();

			_treeControl.AfterSelect += new TreeNodeEventHandler( _treeControl_AfterSelect );
		}

		protected override void OnUnregistered()
		{
			base.OnUnregistered();

			_treeControl.AfterSelect -= new TreeNodeEventHandler( _treeControl_AfterSelect );
		}

		protected override void Read()
		{
			string selectedPath = ControlPreferences.GetValue( Name, "Selection" );

			using( _treeControl.SuspendUpdates() )
			{
				ReadNodes( string.Empty, _treeControl.RootNodes, selectedPath );
			}
		}

		protected override void Write()
		{
			WriteNodes( string.Empty, _treeControl.RootNodes );
		}

		protected virtual string GetNodeText( TreeNode node )
		{
			return node.Text;
		}

		private void ReadNodes( string path, TreeNodeCollection nodes, string selectedPath )
		{
			foreach( TreeNode node in nodes )
			{
				string childPath = path + "/" + GetNodeText( node ).Replace( '/', '_' );
				bool expand = ControlPreferences.GetValue( Name, childPath ) == "Expanded";
				bool collapse = ControlPreferences.GetValue( Name, childPath ) == "Collapsed";
				bool select = (childPath == selectedPath);

				if( expand )
				{
					_treeControl.ExpandNode( node );

					ReadNodes( childPath, node.ChildNodes, selectedPath );
				}
				else if( collapse )
				{
					_treeControl.CollapseNode( node );
				}
				if( select )
				{
					_treeControl.SelectedNode = node;
				}
			}
		}

		private void WriteNodes( string path, TreeNodeCollection nodes )
		{
			foreach( TreeNode node in nodes )
			{
				string childPath = path + "/" + GetNodeText( node ).Replace( '/', '_' );

				if( node == _treeControl.SelectedNode )
				{
					ControlPreferences.SetValue( Name, "Selection", childPath );
				}

				if( node.ChildNodes.Count > 0 )
				{
					if( node.IsExpanded )
					{
						ControlPreferences.SetValue( Name, childPath, "Expanded" );

						WriteNodes( childPath, node.ChildNodes );
					}
					else
					{
						ControlPreferences.SetValue( Name, childPath, "Collapsed" );
					}
				}
			}
		}

		private void _treeControl_AfterSelect( object sender, TreeNodeEventArgs e )
		{
			string path = string.Empty;
			TreeNode node = _treeControl.SelectedNode;

			while( node != null )
			{
				path = "/" + GetNodeText( node ).Replace( '/', '_' ) + path;
				node = node.ParentCollection.ParentNode;
			}

			ControlPreferences.SetValue( Name, "Selection", path );
		}

		private TreeControl _treeControl;
	}
}
