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
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class AnimationItem : Item
	{
		public AnimationItem( WinFormsUtility.Drawing.Animation animation )
		{
			_animation = animation;
		}

		public bool IsAnimating
		{
			get
			{
				return _animating;
			}
		}

		public void Start()
		{
			_animating = true;
			_start = DateTime.Now;
			StartTimer();
		}

		public void Stop()
		{
			_animating = false;
			StopTimer();
		}

		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			return new Size( suggestedSize.Height, suggestedSize.Height );
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			_updates = context.Updates;

			double seconds = DateTime.Now.Subtract( _start ).TotalSeconds;

			_animation.OnPaint( context.Graphics, logicalBounds, _animating, seconds );
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

			if( _updates != null )
			{
				_updates.NeedsUpdate( this );
			}
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			if( _updates != null )
			{
				_updates.NeedsUpdate( this );
			}
		}

		private bool _animating;
		private WinFormsUtility.Drawing.Animation _animation;
		private DateTime _start;
		private Timer _updateTimer;
		private Updates _updates;
	}
}
