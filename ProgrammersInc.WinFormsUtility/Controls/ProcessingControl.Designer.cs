/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.WinFormsUtility.Controls
{
	partial class ProcessingControl
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
            this._animationControl = new ProgrammersInc.WinFormsUtility.Controls.AnimationControl();
			this._label = new System.Windows.Forms.Label();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._timer = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// _animationControl
			// 
			this._animationControl.Animation = null;
			this._animationControl.BackColor = System.Drawing.Color.Transparent;
			this._animationControl.Location = new System.Drawing.Point( 3, 3 );
			this._animationControl.Name = "_animationControl";
			this._animationControl.Size = new System.Drawing.Size( 24, 24 );
			this._animationControl.TabIndex = 0;
			// 
			// _label
			// 
			this._label.AutoEllipsis = true;
			this._label.AutoSize = true;
			this._label.Location = new System.Drawing.Point( 33, 9 );
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size( 0, 13 );
			this._label.TabIndex = 1;
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point( 3, 32 );
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size( 287, 15 );
			this._progressBar.TabIndex = 2;
			// 
			// _timer
			// 
			this._timer.Interval = 200;
			this._timer.Tick += new System.EventHandler( this._timer_Tick );
			// 
			// ProcessingControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this._progressBar );
			this.Controls.Add( this._label );
			this.Controls.Add( this._animationControl );
			this.Name = "ProcessingControl";
			this.Size = new System.Drawing.Size( 293, 50 );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private ProgrammersInc.WinFormsUtility.Controls.AnimationControl _animationControl;
		private System.Windows.Forms.Label _label;
		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Timer _timer;
	}
}
