/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

namespace ProgrammersInc.WinFormsUtility.Dialogs
{
    partial class WizardForm
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
					this.components = new System.ComponentModel.Container();
					this._buttonPanel = new System.Windows.Forms.Panel();
					this._cancelButton = new System.Windows.Forms.Button();
					this._nextButton = new System.Windows.Forms.Button();
					this._backButton = new System.Windows.Forms.Button();
					this._titlePanel = new System.Windows.Forms.PictureBox();
					this._pageSummaryLabel = new System.Windows.Forms.Label();
					this._pageTitleLabel = new System.Windows.Forms.Label();
					this._pagePanel = new System.Windows.Forms.Panel();
                    this._nextDelayedAction = new ProgrammersInc.WinFormsUtility.Events.DelayedAction(this.components);
					this._buttonPanel.SuspendLayout();
					((System.ComponentModel.ISupportInitialize) (this._titlePanel)).BeginInit();
					this._titlePanel.SuspendLayout();
					this.SuspendLayout();
					// 
					// _buttonPanel
					// 
					this._buttonPanel.Controls.Add( this._cancelButton );
					this._buttonPanel.Controls.Add( this._nextButton );
					this._buttonPanel.Controls.Add( this._backButton );
					this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
					this._buttonPanel.Location = new System.Drawing.Point( 0, 262 );
					this._buttonPanel.Name = "_buttonPanel";
					this._buttonPanel.Size = new System.Drawing.Size( 469, 44 );
					this._buttonPanel.TabIndex = 0;
					// 
					// _cancelButton
					// 
					this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
					this._cancelButton.Location = new System.Drawing.Point( 382, 9 );
					this._cancelButton.Name = "_cancelButton";
					this._cancelButton.Size = new System.Drawing.Size( 75, 23 );
					this._cancelButton.TabIndex = 3;
					this._cancelButton.Text = "Cancel";
					this._cancelButton.UseVisualStyleBackColor = true;
					this._cancelButton.Click += new System.EventHandler( this._cancelButton_Click );
					// 
					// _nextButton
					// 
					this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this._nextButton.Location = new System.Drawing.Point( 288, 9 );
					this._nextButton.Name = "_nextButton";
					this._nextButton.Size = new System.Drawing.Size( 75, 23 );
					this._nextButton.TabIndex = 1;
					this._nextButton.Text = "Next >";
					this._nextButton.UseVisualStyleBackColor = true;
					this._nextButton.Click += new System.EventHandler( this._nextButton_Click );
					// 
					// _backButton
					// 
					this._backButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this._backButton.Location = new System.Drawing.Point( 207, 9 );
					this._backButton.Name = "_backButton";
					this._backButton.Size = new System.Drawing.Size( 75, 23 );
					this._backButton.TabIndex = 0;
					this._backButton.Text = "< Back";
					this._backButton.UseVisualStyleBackColor = true;
					this._backButton.Click += new System.EventHandler( this._backButton_Click );
					// 
					// _titlePanel
					// 
					this._titlePanel.BackColor = System.Drawing.SystemColors.Window;
					this._titlePanel.Controls.Add( this._pageSummaryLabel );
					this._titlePanel.Controls.Add( this._pageTitleLabel );
					this._titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
					this._titlePanel.Location = new System.Drawing.Point( 0, 0 );
					this._titlePanel.Name = "_titlePanel";
					this._titlePanel.Size = new System.Drawing.Size( 469, 61 );
					this._titlePanel.TabIndex = 1;
					this._titlePanel.TabStop = false;
					// 
					// _pageSummaryLabel
					// 
					this._pageSummaryLabel.BackColor = System.Drawing.Color.Transparent;
					this._pageSummaryLabel.Location = new System.Drawing.Point( 12, 27 );
					this._pageSummaryLabel.Name = "_pageSummaryLabel";
					this._pageSummaryLabel.Size = new System.Drawing.Size( 351, 31 );
					this._pageSummaryLabel.TabIndex = 1;
					this._pageSummaryLabel.Text = "<Page Summary>";
					// 
					// _pageTitleLabel
					// 
					this._pageTitleLabel.AutoSize = true;
					this._pageTitleLabel.BackColor = System.Drawing.Color.Transparent;
					this._pageTitleLabel.Location = new System.Drawing.Point( 12, 9 );
					this._pageTitleLabel.Name = "_pageTitleLabel";
					this._pageTitleLabel.Size = new System.Drawing.Size( 67, 13 );
					this._pageTitleLabel.TabIndex = 0;
					this._pageTitleLabel.Text = "<Page Title>";
					// 
					// _pagePanel
					// 
					this._pagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
					this._pagePanel.Location = new System.Drawing.Point( 0, 61 );
					this._pagePanel.Name = "_pagePanel";
					this._pagePanel.Padding = new System.Windows.Forms.Padding( 3 );
					this._pagePanel.Size = new System.Drawing.Size( 469, 201 );
					this._pagePanel.TabIndex = 2;
					// 
					// _nextDelayedAction
					// 
					this._nextDelayedAction.Apply += new System.EventHandler( this._nextDelayedAction_Apply );
					// 
					// WizardForm
					// 
					this.AcceptButton = this._nextButton;
					this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.CancelButton = this._cancelButton;
					this.ClientSize = new System.Drawing.Size( 469, 306 );
					this.Controls.Add( this._pagePanel );
					this.Controls.Add( this._titlePanel );
					this.Controls.Add( this._buttonPanel );
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
					this.MaximizeBox = false;
					this.MinimizeBox = false;
					this.Name = "WizardForm";
					this.ShowIcon = false;
					this.ShowInTaskbar = false;
					this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
					this.Text = "<Wizard Title>";
					this._buttonPanel.ResumeLayout( false );
					((System.ComponentModel.ISupportInitialize) (this._titlePanel)).EndInit();
					this._titlePanel.ResumeLayout( false );
					this._titlePanel.PerformLayout();
					this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.PictureBox _titlePanel;
        private System.Windows.Forms.Panel _pagePanel;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _nextButton;
        private System.Windows.Forms.Button _backButton;
        private System.Windows.Forms.Label _pageTitleLabel;
        private System.Windows.Forms.Label _pageSummaryLabel;
        private ProgrammersInc.WinFormsUtility.Events.DelayedAction _nextDelayedAction;
    }
}