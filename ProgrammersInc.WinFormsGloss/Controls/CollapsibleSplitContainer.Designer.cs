/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.WinFormsGloss.Controls
{
	partial class CollapsibleSplitContainer
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
			this._toggleBar = new System.Windows.Forms.Panel();
			this._splitter = new System.Windows.Forms.Panel();
			this._panel2 = new System.Windows.Forms.Panel();
			this._panel1 = new System.Windows.Forms.Panel();
			this._dragTimer = new System.Windows.Forms.Timer( this.components );
			this._updateTimer = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// _toggleBar
			// 
			this._toggleBar.Cursor = System.Windows.Forms.Cursors.Hand;
			this._toggleBar.Location = new System.Drawing.Point( 222, 226 );
			this._toggleBar.Name = "_toggleBar";
			this._toggleBar.Size = new System.Drawing.Size( 25, 62 );
			this._toggleBar.TabIndex = 7;
			// 
			// _splitter
			// 
			this._splitter.Location = new System.Drawing.Point( 233, 66 );
			this._splitter.Name = "_splitter";
			this._splitter.Size = new System.Drawing.Size( 10, 383 );
			this._splitter.TabIndex = 6;
			// 
			// _panel2
			// 
			this._panel2.Location = new System.Drawing.Point( 280, 66 );
			this._panel2.Name = "_panel2";
			this._panel2.Size = new System.Drawing.Size( 230, 383 );
			this._panel2.TabIndex = 5;
			// 
			// _panel1
			// 
			this._panel1.Location = new System.Drawing.Point( 61, 59 );
			this._panel1.Name = "_panel1";
			this._panel1.Size = new System.Drawing.Size( 155, 391 );
			this._panel1.TabIndex = 4;
			// 
			// _updateTimer
			// 
			this._updateTimer.Enabled = true;
			// 
			// CollapsibleSplitContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this._toggleBar );
			this.Controls.Add( this._splitter );
			this.Controls.Add( this._panel2 );
			this.Controls.Add( this._panel1 );
			this.Name = "CollapsibleSplitContainer";
			this.Size = new System.Drawing.Size( 571, 509 );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Panel _toggleBar;
		private System.Windows.Forms.Panel _splitter;
		private System.Windows.Forms.Panel _panel2;
		private System.Windows.Forms.Panel _panel1;
		private System.Windows.Forms.Timer _dragTimer;
		private System.Windows.Forms.Timer _updateTimer;
	}
}
