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
using System.Drawing.Drawing2D;

namespace ProgrammersInc.WinFormsUtility.Controls
{
	public partial class AnimationControl : UserControl
	{
		public AnimationControl()
		{
			InitializeComponent();

			SetStyle
					( ControlStyles.OptimizedDoubleBuffer
					| ControlStyles.AllPaintingInWmPaint
					| ControlStyles.SupportsTransparentBackColor
					| ControlStyles.UserPaint
					, true );
		}

		public void Start()
		{
			_running = true;
			_start = DateTime.Now;
			UpdateTimer();

			OnInvalidating( EventArgs.Empty );
		}

		public void Stop()
		{
			_running = false;
			UpdateTimer();

			OnInvalidating( EventArgs.Empty );
			Invalidate();
		}

		public Drawing.Animation Animation
		{
			get
			{
				return _animation;
			}
			set
			{
				_animation = value;

				Invalidate();
			}
		}

		public bool IsRunning
		{
			get
			{
				return _running;
			}
		}

		public void DoPaint( Graphics g, Rectangle rect )
		{
			if( _animation != null )
			{
				double seconds = DateTime.Now.Subtract( _start ).TotalSeconds;

				using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( g ) )
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;

					_animation.OnPaint( g, rect, _running, seconds );
				}
			}
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			UpdateTimer();

			OnInvalidating( EventArgs.Empty );
		}

		protected virtual Rectangle GetDrawingRectangle()
		{
			return ClientRectangle;
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			Rectangle drawBounds = GetDrawingRectangle();

			DoPaint( e.Graphics, drawBounds );
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			if( _running )
			{
				StartTimer();
			}
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

			StopTimer();
		}

		protected virtual void OnInvalidating( EventArgs e )
		{
			if( Invalidating != null )
			{
				Invalidating( this, e );
			}
		}

		private void UpdateTimer()
		{
			bool want = _running && Visible;

			if( want && _updateTimer == null )
			{
				StartTimer();
			}
			else if( !want && _updateTimer != null )
			{
				StopTimer();
			}
		}

		private void StartTimer()
		{
			if( _updateTimer == null )
			{
				_updateTimer = new Timer();
				_updateTimer.Interval = 100;
				_updateTimer.Tick += new EventHandler( _updateTimer_Tick );
				_updateTimer.Start();
			}
		}

		private void StopTimer()
		{
			if( _updateTimer != null )
			{
				_updateTimer.Tick -= new EventHandler( _updateTimer_Tick );
				_updateTimer.Dispose();
				_updateTimer = null;
			}
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			OnInvalidating( EventArgs.Empty );
			Invalidate();
		}

		public event EventHandler Invalidating;

		private Timer _updateTimer;
		private bool _running;
		private Drawing.Animation _animation;
		private DateTime _start = DateTime.Now;
	}
}
