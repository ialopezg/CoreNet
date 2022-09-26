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

namespace SuperTreeTest
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("fdfsdsf");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("sdfsdfsdf");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("fsdfsdfsd");
            ProgrammersInc.WinFormsGloss.Drawing.WindowsThemeColorTable windowsThemeColorTable2 = new ProgrammersInc.WinFormsGloss.Drawing.WindowsThemeColorTable();
            this.listView1 = new ProgrammersInc.WinFormsUtility.Controls.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.treeControl1 = new ProgrammersInc.WinFormsGloss.Controls.TreeControl();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.ExpandingColumn = -1;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listView1.Location = new System.Drawing.Point(328, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(457, 173);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 167;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 163;
            // 
            // treeControl1
            // 
            this.treeControl1.Animate = false;
            this.treeControl1.AutoScroll = true;
            this.treeControl1.AutoScrollMinSize = new System.Drawing.Size(0, 8);
            this.treeControl1.BackColor = System.Drawing.SystemColors.Window;
            this.treeControl1.ColorTable = windowsThemeColorTable2;
            this.treeControl1.Location = new System.Drawing.Point(12, 12);
            this.treeControl1.Name = "treeControl1";
            this.treeControl1.SelectedNode = null;
            this.treeControl1.Size = new System.Drawing.Size(260, 445);
            this.treeControl1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 469);
            this.Controls.Add(this.treeControl1);
            this.Controls.Add(this.listView1);
            this.Name = "MainForm";
            this.Text = "SuperTreeTest";
            this.ResumeLayout(false);

		}

		#endregion

        private ProgrammersInc.WinFormsUtility.Controls.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ProgrammersInc.WinFormsGloss.Controls.TreeControl treeControl1;

        
	}
}

