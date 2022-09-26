/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.WinFormsUtility.Dialogs
{
	partial class OptionsBox
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
			this._description = new System.Windows.Forms.Label();
			this._okButton = new System.Windows.Forms.Button();
			this._pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize) (this._pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _description
			// 
			this._description.AutoSize = true;
			this._description.Location = new System.Drawing.Point( 50, 12 );
			this._description.MaximumSize = new System.Drawing.Size( 250, 0 );
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size( 104, 13 );
			this._description.TabIndex = 0;
			this._description.Text = "Description text here";
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._okButton.Location = new System.Drawing.Point( 233, 62 );
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size( 75, 23 );
			this._okButton.TabIndex = 100;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler( this._okButton_Click );
			// 
			// _pictureBox
			// 
			this._pictureBox.Location = new System.Drawing.Point( 12, 12 );
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size( 32, 32 );
			this._pictureBox.TabIndex = 101;
			this._pictureBox.TabStop = false;
			// 
			// OptionsBox
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._okButton;
			this.ClientSize = new System.Drawing.Size( 320, 97 );
			this.ControlBox = false;
			this.Controls.Add( this._pictureBox );
			this.Controls.Add( this._okButton );
			this.Controls.Add( this._description );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsBox";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "OptionsBox";
			((System.ComponentModel.ISupportInitialize) (this._pictureBox)).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _description;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.PictureBox _pictureBox;
	}
}