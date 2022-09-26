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
using ProgrammersInc.WinFormsGloss.Controls.Ribbon;

namespace ProgrammersInc.WinFormsGloss.Commands
{
	public class CommandAnimatedRibbonButtonItem : CommandRibbonButtonItem
	{
		public CommandAnimatedRibbonButtonItem( string text, Icon icon16, Icon icon24 )
			: base( text, icon16, icon24 )
		{
		}

		public void StartAnimating( WinFormsUtility.Drawing.Animation animation )
		{
			_animation = animation;
			_start = DateTime.Now;
			StartTimer();
		}

		public void StopAnimating()
		{
			_animation = null;
			StopTimer();
		}

		public override void Paint( WinFormsGloss.Controls.Ribbon.Context context, Rectangle clip, Rectangle logicalBounds )
		{
			base.Paint( context, clip, logicalBounds );

			_updates = context.Updates;

			double seconds = DateTime.Now.Subtract( _start ).TotalSeconds;

			if( _animation != null )
			{
				_animation.OnPaint( context.Graphics, logicalBounds, true, seconds );
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

		private WinFormsUtility.Drawing.Animation _animation;
		private DateTime _start;
		private Timer _updateTimer;
		private Updates _updates;
	}
}
