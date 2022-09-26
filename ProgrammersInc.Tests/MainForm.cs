/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//
// (c) 2006 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SuperTreeTest
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

            FillTreeControl(treeControl1);
            //FillTreeControl( _treeControl2 );
            //FillTreeControl( _treeControl3 );
            //FillTreeControl( _treeControl4 );
		}

		private void FillTreeControl( ProgrammersInc.SuperTree.TreeControl treeControl )
		{
            ProgrammersInc.Utility.Assemblies.ManifestResources res = new ProgrammersInc.Utility.Assemblies.ManifestResources("SuperTreeTest");

			using( treeControl.SuspendUpdates() )
			{
				treeControl.Icons.Add( res.GetIcon( "Resources.FeedTreeItem16.ico" ) );
				treeControl.Icons.Add( res.GetIcon( "Resources.CollapsedFolderTreeItem16.ico" ) );
				treeControl.Icons.Add( res.GetIcon( "Resources.ExpandedFolderTreeItem16.ico" ) );

				for( int x = 1; x <= 5; ++x )
				{
                    ProgrammersInc.SuperTree.TreeNode xtn = treeControl.RootNodes.Add();

					xtn.Text = string.Format( "Folder - X - {0}", x );
					xtn.ExpandedImageIndex = 2;
					xtn.CollapsedImageIndex = 1;

					for( int y = 1; y <= 5; ++y )
					{
                        ProgrammersInc.SuperTree.TreeNode ytn = xtn.ChildNodes.Add();

						ytn.Text = string.Format( "Folder - Y - {0}", y );
						ytn.ExpandedImageIndex = 2;
						ytn.CollapsedImageIndex = 1;

						for( int z = 1; z <= 5; ++z )
						{
                            ProgrammersInc.SuperTree.TreeNode ztn = ytn.ChildNodes.Add();

							ztn.Text = string.Format( "Subscription - Z - {0}", z );
							ztn.ExpandedImageIndex = 0;
							ztn.CollapsedImageIndex = 0;
						}
					}
				}
			}
		}
	}
}