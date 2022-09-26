/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.SuperTree
{
	partial class TreeControl
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._updateTimer = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// _updateTimer
			// 
			this._updateTimer.Interval = 1000;
			// 
			// TreeControl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Name = "TreeControl";
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Timer _updateTimer;
	}
}
