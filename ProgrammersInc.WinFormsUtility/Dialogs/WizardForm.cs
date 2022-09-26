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
	public partial class WizardForm : Form, IWizardContext
	{
		public WizardForm()
		{
			InitializeComponent();

			Font = SystemFonts.DialogFont;

			_pageTitleLabel.Font = new Font( _pageTitleLabel.Font, FontStyle.Bold );
			_cancelButton.DialogResult = DialogResult.None;
		}

		public bool Display( Control owner, WizardDescriptor descriptor )
		{
			if( descriptor == null )
			{
				throw new ArgumentNullException( "descriptor" );
			}

			_descriptor = descriptor;

			Text = _descriptor.Title;
			_titlePanel.Image = descriptor.TopImage;

			WizardPage firstPage = _descriptor.CreateInitialPage( this );

			SetPage( firstPage );

			_titlePanel.Paint += new PaintEventHandler( _titlePanel_Paint );

			ShowDialog( owner );

			_titlePanel.Paint -= new PaintEventHandler( _titlePanel_Paint );

			SetPage( null );

			foreach( WizardPage page in _history )
			{
				page.Dispose();
			}

			_descriptor = null;
			_history.Clear();

			return DialogResult == DialogResult.OK;
		}

		protected override void OnClosing( CancelEventArgs e )
		{
			base.OnClosing( e );

			if( DialogResult == DialogResult.None )
			{
				DoCancel( e );
			}
		}

		private void _titlePanel_Paint( object sender, PaintEventArgs e )
		{
			if( _descriptor != null )
			{
				Image image = _descriptor.TopImage;

				e.Graphics.DrawImage( image, new Rectangle( _titlePanel.Width - image.Width, 0, image.Width, image.Height ) );
			}
		}

		private void SetPage( WizardPage page )
		{
			_pagePanel.Controls.Clear();

			if( _currentPage != null )
			{
				_currentPage.OnPageClosed();
				_currentPage = null;
			}

			_currentPage = page;

			if( _currentPage != null )
			{
				_currentPage.OnPageOpened( this );
				_currentPage.Dock = DockStyle.Fill;
				_pagePanel.Controls.Add( _currentPage );
				_pageTitleLabel.Text = _currentPage.Title;
				_pageSummaryLabel.Text = _currentPage.Summary;
				_currentPage.Select();
			}
		}

		#region IWizardContext Members

		void IWizardContext.SetButtons( WizardButtons buttons )
		{
			if( ((buttons & WizardButtons.Next) != 0) && ((buttons & WizardButtons.Finish) != 0) )
			{
				throw new ArgumentException( "Cannot specifiy both Next and Finish.", "buttons" );
			}

			_backButton.Enabled = ((buttons & WizardButtons.Back) != 0) && (_history.Count != 0);
			_nextButton.Enabled = ((buttons & WizardButtons.Next) != 0) || ((buttons & WizardButtons.Finish) != 0);
			_cancelButton.Enabled = ((buttons & WizardButtons.Cancel) != 0);

			if( (buttons & WizardButtons.Finish) != 0 )
			{
				_nextButton.Text = _finishText;
			}
			if( (buttons & WizardButtons.Next) != 0 )
			{
				_nextButton.Text = _nextText;
			}
		}

		void IWizardContext.InvokeNext()
		{
			_nextDelayedAction.ApplyLater( 100 );
		}

		void IWizardContext.InvokeFinish()
		{
			_nextDelayedAction.ApplyLater( 100 );
		}

		void IWizardContext.ForceNext()
		{
			GoNext();
		}

		void IWizardContext.ForceFinish()
		{
			DialogResult = DialogResult.OK;
		}

		void IWizardContext.Close()
		{
			DialogResult = DialogResult.OK;
		}

		WizardDescriptor IWizardContext.Descriptor
		{
			get
			{
				return _descriptor;
			}
		}

		#endregion

		private void _backButton_Click( object sender, EventArgs e )
		{
			CancelEventArgs ce = new CancelEventArgs();

			_currentPage.OnNavigateBack( ce );

			if( ce.Cancel )
			{
				return;
			}

			if( _history.Count == 0 )
			{
				throw new InvalidOperationException();
			}

			WizardPage newPage = _history.Pop();

			SetPage( newPage );
		}

		private void _nextButton_Click( object sender, EventArgs e )
		{
			CancelEventArgs ce = new CancelEventArgs();

			if( _nextButton.Text == _finishText )
			{
				_currentPage.OnFinish( ce );

				if( ce.Cancel )
				{
					return;
				}

				DialogResult = DialogResult.OK;
			}
			else
			{
				_currentPage.OnNavigateNext( ce );

				if( ce.Cancel )
				{
					return;
				}

				GoNext();
			}
		}

		private void GoNext()
		{
			WizardPage newPage = _currentPage.CreateNextPage();

			if( newPage == null )
			{
				throw new InvalidOperationException();
			}
			if( _currentPage == null )
			{
				throw new InvalidOperationException();
			}

			_history.Push( _currentPage );
			SetPage( newPage );
		}

		private void _cancelButton_Click( object sender, EventArgs e )
		{
			CancelEventArgs ce = new CancelEventArgs();

			DoCancel( ce );
		}

		private void DoCancel( CancelEventArgs ce )
		{
			if( _currentPage != null )
			{
				_currentPage.OnCancel( ce );
			}

			if( ce.Cancel )
			{
				return;
			}

			_descriptor.OnCancel( this, ce );

			if( ce.Cancel )
			{
				return;
			}

			DialogResult = DialogResult.Cancel;
		}

		private void _nextDelayedAction_Apply( object sender, EventArgs e )
		{
			_nextButton.PerformClick();
		}

		private readonly string _finishText = "Finish";
		private readonly string _nextText = "Next >";

		private WizardDescriptor _descriptor;
		private WizardPage _currentPage;
		private Stack<WizardPage> _history = new Stack<WizardPage>();
	}

	[Flags]
	public enum WizardButtons
	{
		None = 0,
		Back = 1,
		Next = 2,
		Finish = 4,
		Cancel = 8
	}

	public interface IWizardContext
	{
		WizardDescriptor Descriptor
		{
			get;
		}
		void SetButtons( WizardButtons buttons );
		void InvokeNext();
		void InvokeFinish();
		void ForceNext();
		void ForceFinish();
		void Close();
	}

	public abstract class WizardDescriptor
	{
		protected WizardDescriptor()
		{
		}

		public abstract string Title
		{
			get;
		}

		public abstract Image TopImage
		{
			get;
		}

		public abstract WizardPage CreateInitialPage( IWizardContext context );

		public virtual void OnCancel( IWizardContext context, CancelEventArgs e )
		{
		}
	}

	public class WizardPage : UserControl
	{
		protected WizardPage()
		{
		}

		public virtual string Title
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public virtual string Summary
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public virtual WizardPage CreateNextPage()
		{
			throw new InvalidOperationException();
		}

		public virtual void OnPageOpened( IWizardContext context )
		{
			_context = context;
		}

		public virtual void OnPageClosed()
		{
			_context = null;
		}

		public virtual void OnNavigateBack( CancelEventArgs e )
		{
		}

		public virtual void OnNavigateNext( CancelEventArgs e )
		{
		}

		public virtual void OnFinish( CancelEventArgs e )
		{
		}

		public virtual void OnCancel( CancelEventArgs e )
		{
		}

		protected IWizardContext Context
		{
			get
			{
				return _context;
			}
		}

		private IWizardContext _context;
	}
}