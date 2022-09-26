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
using System.Threading;

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public partial class ProcessingControl : UserControl
	{
		public ProcessingControl()
		{
			InitializeComponent();

			Visible = false;
		}

		public Drawing.Animation Animation
		{
			get
			{
				return _animationControl.Animation;
			}
			set
			{
				_animationControl.Animation = value;
			}
		}

		public bool ShowProgressBar
		{
			get
			{
				return _showProgressBar;
			}
			set
			{
				_showProgressBar = value;
				UpdateVisibility();
			}
		}

		public bool IsRunning
		{
			get
			{
				lock( _lock )
				{
					return _action != null;
				}
			}
		}

		public void UpdateVisibility()
		{
			Visible = _animationControl.IsRunning;

			_progressBar.Visible = _showProgressBar;
		}

		public void Start( Utility.Threading.Action action )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}

			if( _animationControl.Animation == null )
			{
				throw new InvalidOperationException( "No animation set." );
			}

			lock( _lock )
			{
				if( _action != null )
				{
					throw new InvalidOperationException( "Already processing." );
				}

				_action = action;

				OnProcessingBegin( EventArgs.Empty );

				ThreadStart ts = new ThreadStart( ThreadFunc );
				Thread thread = new Thread( ts );

				thread.Start();
			}
		}

		public void Cancel()
		{
			_progress.Cancel();
		}

		protected virtual void OnProcessingBegin( EventArgs e )
		{
			_animationControl.Start();

			UpdateVisibility();

			_timer.Enabled = true;

			if( ProcessingBegin != null )
			{
				ProcessingBegin( this, e );
			}
		}

		protected virtual void OnProcessingEndSuccess( EventArgs e )
		{
			_animationControl.Stop();

			UpdateVisibility();
			
			_timer.Enabled = false;

			if( ProcessingEndSuccess != null )
			{
				ProcessingEndSuccess( this, e );
			}
		}

		protected virtual void OnProcessingEndFailure( ExceptionEventArgs e )
		{
			_animationControl.Stop();

			UpdateVisibility();
			
			_timer.Enabled = false;

			if( ProcessingEndFailure != null )
			{
				ProcessingEndFailure( this, e );
			}
		}

		private void ThreadFunc()
		{
			try
			{
				_action( _progress );

				lock( _lock )
				{
					_action = null;
				}

				if( IsHandleCreated && !_progress.IsCancelled )
				{
					Invoke( new EndSuccessDelegate( EndSuccessCrossThread ) );
				}
			}
			catch( Exception e )
			{
				lock( _lock )
				{
					_action = null;
				}

				if( IsHandleCreated && !_progress.IsCancelled )
				{
					Invoke( new EndFailureDelegate( EndFailureCrossThread ), e );
				}
			}
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			_label.Text = _progress.Description;
			_progressBar.Value = _progress.Percentage;
		}

		#region Cross-thread operations

		private delegate void BeginDelegate();
		private void BeginCrossThread()
		{
			OnProcessingBegin( EventArgs.Empty );
		}

		private delegate void EndSuccessDelegate();
		private void EndSuccessCrossThread()
		{
			OnProcessingEndSuccess( EventArgs.Empty );
		}

		private delegate void EndFailureDelegate( Exception e );
		private void EndFailureCrossThread( Exception e )
		{
			OnProcessingEndFailure( new ExceptionEventArgs( e ) );
		}

		#endregion

		public event EventHandler ProcessingBegin, ProcessingEndSuccess;
		public event ExceptionEventHandler ProcessingEndFailure;

		private object _lock = new object();
		private Utility.Threading.Action _action;
		private Utility.Threading.Progress _progress = new Utility.Threading.Progress();
		private bool _showProgressBar = true;
	}

	public class ExceptionEventArgs : EventArgs
	{
		public ExceptionEventArgs( Exception exception )
		{
			if( exception == null )
			{
				throw new ArgumentNullException( "exception" );
			}

			_exception = exception;
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		private Exception _exception;
	}

	public delegate void ExceptionEventHandler( object sender, ExceptionEventArgs e );
}
