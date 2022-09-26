/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.WinFormsGloss.Controls
{
	partial class CustomTabControl
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
				if( _updateTimer != null )
				{
					_updateTimer.Tick += new System.EventHandler( this._updateTimer_Tick );
					_updateTimer.Dispose();
					_updateTimer = null;
				}

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
			this._contentsPanel = new System.Windows.Forms.Panel();
			this._updateTimer = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// _contentsPanel
			// 
			this._contentsPanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._contentsPanel.Location = new System.Drawing.Point( 123, 93 );
			this._contentsPanel.Name = "_contentsPanel";
			this._contentsPanel.Size = new System.Drawing.Size( 350, 334 );
			this._contentsPanel.TabIndex = 0;
			// 
			// _updateTimer
			// 
			this._updateTimer.Tick += new System.EventHandler( this._updateTimer_Tick );
			// 
			// CustomTabControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this._contentsPanel );
			this.Name = "CustomTabControl";
			this.Size = new System.Drawing.Size( 602, 494 );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Panel _contentsPanel;
		private System.Windows.Forms.Timer _updateTimer;
	}
}
