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
	public enum EventResult
	{
		Done,
		Defer
	}

	public interface IEvent
	{
		EventResult Invoke();
	}

	public partial class EventQueue : Component
	{
		public EventQueue()
		{
			InitializeComponent();
		}

		public EventQueue( IContainer container )
		{
			container.Add( this );

			InitializeComponent();
		}

		public void Add( IEvent ev )
		{
			if( ev == null )
			{
				throw new ArgumentNullException( "ev" );
			}

			lock( _lock )
			{
				if( !_events.Contains( ev ) )
				{
					_events.Enqueue( ev );
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return _timer.Enabled;
			}
			set
			{
				_timer.Enabled = value;
			}
		}

		public void Flush()
		{
			for( ; ; )
			{
				if( !FlushOne() )
				{
					return;
				}
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			if( _timer != null )
			{
				_timer.Tick -= new System.EventHandler( _timer_Tick );
				_timer.Dispose();
				_timer = null;
			}

			_events.Clear();

			base.Dispose( disposing );
		}

		private bool FlushOne()
		{
			if( _activeFlag.IsActive )
			{
				throw new InvalidOperationException( "Cannot flush the event queue from an event handler." );
			}

			using( _activeFlag.Apply() )
			{
				IEvent ev = null;

				lock( _lock )
				{
					if( _events.Count > 0 )
					{
						ev = _events.Peek();
					}
				}

				if( ev == null )
				{
					return false;
				}
				else
				{
					EventResult er = ev.Invoke();

					switch( er )
					{
						case EventResult.Done:
							lock( _lock )
							{
								Queue<IEvent> newEvents = new Queue<IEvent>();

								while( _events.Count > 0 )
								{
									IEvent ne = _events.Dequeue();

									if( ne != ev )
									{
										newEvents.Enqueue( ne );
									}
								}

								_events = newEvents;
							}
							break;
						case EventResult.Defer:
							break;
						default:
							throw new InvalidOperationException();
					}
				}
			}

			return true;
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			if( !_activeFlag.IsActive )
			{
				FlushOne();
			}
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._timer = new System.Windows.Forms.Timer( this.components );
			// 
			// _timer
			// 
			this._timer.Interval = 200;
			this._timer.Tick += new System.EventHandler( this._timer_Tick );

		}

		#endregion

		private object _lock = new object();
		private Queue<IEvent> _events = new Queue<IEvent>();
		private Utility.Control.Flag _activeFlag = new Utility.Control.Flag();
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Timer _timer;
	}
}
