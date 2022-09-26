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
using System.Windows.Forms;
using System.Drawing;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	public static class SuperToolTipManager
	{
		static SuperToolTipManager()
		{
			_timer.Interval = 500;
			_timer.Tick += new EventHandler( _timer_Tick );
			_timer.Enabled = true;
		}

		public static void ShowToolTip( Drawing.ColorTable colorTable, SuperToolTipInfo info, Control owner, Point p )
		{
			ShowToolTip( colorTable, info, owner, p, false );
		}

		public static void ShowToolTip( Drawing.ColorTable colorTable, SuperToolTipInfo info, Control owner, Point p, bool balloon )
		{
			if( _suppressCount != 0 || WinFormsUtility.Events.MenuLoop.InMenuLoop )
			{
				return;
			}

			if( _existing != null )
			{
				if( object.Equals( _existing.Info, info ) )
				{
					return;
				}
				else
				{
					_existing.Close();
					_existing = null;
				}
			}

			_mousePoint = Control.MousePosition;

			_existing = new SuperToolTip( colorTable, info, p, balloon );

			_existing.Show( owner );
		}

		public static void CloseToolTip()
		{
			if( _existing != null )
			{
				_existing.Close();
				_existing = null;
			}
		}

		public static void SuppressToolTips()
		{
			++_suppressCount;
			CloseToolTip();
		}

		public static void AllowToolTips()
		{
			--_suppressCount;
		}

		private static void _timer_Tick( object sender, EventArgs e )
		{
			if( _mousePoint != Control.MousePosition )
			{
				CloseToolTip();
			}
		}

		private static Timer _timer = new Timer();
		private static SuperToolTip _existing;
		private static Point _mousePoint;
		private static int _suppressCount;
	}
}
