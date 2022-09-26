/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProgrammersInc.WinFormsUtility.Events
{
	public delegate void Action();

	public partial class DelayedAction : Component
	{
		public DelayedAction()
		{
			InitializeComponent();
		}

		public DelayedAction( IContainer container )
		{
			container.Add( this );

			InitializeComponent();
		}

		public void ApplyImmediate()
		{
			_timer.Stop();

			OnApply( EventArgs.Empty );
		}

		public void ApplyLater( int milliseconds )
		{
			ApplyLater( milliseconds, null );
		}

		public void ApplyLater( int milliseconds, Action action )
		{
			_action = action;

			if( milliseconds <= 0 )
			{
				ApplyImmediate();
				return;
			}

			_timer.Interval = milliseconds;

			_start = DateTime.Now;
			_timer.Start();
		}

		protected virtual void OnApply( EventArgs e )
		{
			if( _action != null )
			{
				_action();
				_action = null;
			}

			if( Apply != null )
			{
				Apply( this, e );
			}
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			double ms = DateTime.Now.Subtract( _start ).TotalMilliseconds;

			if( ms < _timer.Interval )
			{
				return;
			}

			_timer.Stop();

			OnApply( EventArgs.Empty );
		}

		public event EventHandler Apply;

		private DateTime _start;
		private Action _action;
	}
}
