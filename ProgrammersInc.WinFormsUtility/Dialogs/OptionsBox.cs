/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsUtility.Dialogs
{
	public partial class OptionsBox : Form
	{
		public class Option
		{
			public Option( string id, string optionText )
			{
				this.Id = id;
				this.Description = optionText;
			}

			public readonly string Id;
			public readonly string Description;
		}

		public static Option Show( IWin32Window owner, string choicesDescription, Option[] options )
		{
			OptionsBox ob = new OptionsBox( choicesDescription, options );

			ob.Visible = false;

			ob.ShowDialog( owner );

			return ob._selectedOption;
		}

		private OptionsBox( string choicesDescription, Option[] options )
		{
			InitializeComponent();

			Text = Application.ProductName;

			int descriptionHeight = _description.Height;

			_description.Text = choicesDescription;
			_pictureBox.Image = SystemIcons.Question.ToBitmap();

			Point pt = new Point( _description.Left + 2, _description.Bottom + 20 );
			int diff = _description.Height - descriptionHeight + _okButton.Height;
			int tabIndex = 1;

			foreach( Option option in options )
			{
				RadioButton rb = new RadioButton();

				rb.AutoSize = true;
				rb.Location = pt;
				rb.Name = "radioButton" + tabIndex.ToString();
				rb.Tag = option;
				rb.TabIndex = tabIndex++;
				rb.TabStop = true;
				rb.Text = option.Description;
				rb.UseVisualStyleBackColor = true;
				Controls.Add( rb );
				rb.Visible = true;
				pt.Y += rb.Size.Height + 2;
				diff += rb.Size.Height + 2;
			}
			
			Size = new Size( Size.Width, Size.Height + diff );
		}

		private void _okButton_Click( object sender, EventArgs e )
		{
			foreach( Control c in this.Controls )
			{
				RadioButton rb = c as RadioButton;

				if( rb != null && rb.Checked )
				{
					_selectedOption = (Option) rb.Tag;
					break;
				}
			}
			this.Close();
		}

		private Option _selectedOption;
	}
}