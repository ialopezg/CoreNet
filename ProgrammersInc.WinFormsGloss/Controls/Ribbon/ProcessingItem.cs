/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class ProcessingItem : AnimationItem
	{
		public ProcessingItem( WinFormsUtility.Drawing.Animation animation )
			: base( animation )
		{
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

		public void Start( Utility.Threading.Action action, Control invoker )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}
			if( invoker == null )
			{
				throw new ArgumentNullException( "invoker" );
			}

			lock( _lock )
			{
				if( _action != null )
				{
					throw new InvalidOperationException( "Already processing." );
				}

				_action = action;
				_invoker = invoker;
				_progress = new Utility.Threading.Progress();

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
			Start();

			if( ProcessingBegin != null )
			{
				ProcessingBegin( this, e );
			}
		}

		protected virtual void OnProcessingEndSuccess( EventArgs e )
		{
			Stop();

			if( ProcessingEndSuccess != null )
			{
				ProcessingEndSuccess( this, e );
			}
		}

		protected virtual void OnProcessingEndFailure( WinFormsUtility.Controls.ExceptionEventArgs e )
		{
			Stop();

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

				if( _invoker.IsHandleCreated )
				{
					_invoker.Invoke( new EndSuccessDelegate( EndSuccessCrossThread ) );
				}
			}
			catch( Exception e )
			{
				lock( _lock )
				{
					_action = null;
				}

				if( _invoker.IsHandleCreated )
				{
					_invoker.Invoke( new EndFailureDelegate( EndFailureCrossThread ), e );
				}
			}
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
			OnProcessingEndFailure( new WinFormsUtility.Controls.ExceptionEventArgs( e ) );
		}

		#endregion

		public event EventHandler ProcessingBegin, ProcessingEndSuccess;
		public event WinFormsUtility.Controls.ExceptionEventHandler ProcessingEndFailure;

		private object _lock = new object();
		private Utility.Threading.Action _action;
		private Utility.Threading.Progress _progress = new Utility.Threading.Progress();
		private Control _invoker;
	}
}
