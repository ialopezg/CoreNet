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

namespace ProgrammersInc.WinFormsUtility.Win32
{
	public static class BroadcastMessages
	{
		private static void OnPowerSuspend( EventArgs e )
		{
			if( !_suspended )
			{
				_suspended = true;

				if( _powerSuspend != null )
				{
					_powerSuspend( null, e );
				}
			}
		}

		private static void OnPowerResume( EventArgs e )
		{
			if( _suspended )
			{
				_suspended = false;

				if( _powerResume != null )
				{
					_powerResume( null, e );
				}
			}
		}

		private static void EnsureForm()
		{
			if( _form == null )
			{
				_form = new BroadcastMessageForm();
			}
		}

		public static event EventHandler PowerSuspend
		{
			add
			{
				EnsureForm();

				_powerSuspend += value;
			}
			remove
			{
				_powerSuspend -= value;
			}
		}

		public static event EventHandler PowerResume
		{
			add
			{
				EnsureForm();

				_powerResume += value;
			}
			remove
			{
				_powerResume -= value;
			}
		}

		#region BroadcastMessageForm

		private sealed class BroadcastMessageForm : Form
		{
			internal BroadcastMessageForm()
			{
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				Location = new System.Drawing.Point( -32000, -32000 );
				Name = "BroadcastMessageForm";
				Opacity = 0;
				ShowInTaskbar = false;
				StartPosition = System.Windows.Forms.FormStartPosition.Manual;

				CreateHandle();
			}

			protected override void WndProc( ref Message m )
			{
				base.WndProc( ref m );

				if( m.Msg == (int) Utility.Win32.Messages.WM_POWERBROADCAST )
				{
					switch( (int) m.WParam )
					{
						case PBT_APMSUSPEND:
						case PBT_APMSTANDBY:
							BroadcastMessages.OnPowerSuspend( EventArgs.Empty );
							break;
						case PBT_APMRESUMECRITICAL:
						case PBT_APMRESUMESUSPEND:
						case PBT_APMRESUMESTANDBY:
						case PBT_APMRESUMEAUTOMATIC:
							BroadcastMessages.OnPowerResume( EventArgs.Empty );
							break;
					}
				}
			}

			private const int PBT_APMSUSPEND = 0x0004;
			private const int PBT_APMSTANDBY = 0x0005;
			private const int PBT_APMRESUMECRITICAL = 0x0006;
			private const int PBT_APMRESUMESUSPEND = 0x0007;
			private const int PBT_APMRESUMESTANDBY = 0x0008;
			private const int PBT_APMRESUMEAUTOMATIC = 0x0012;
		}

		#endregion

		private static BroadcastMessageForm _form;
		private static event EventHandler _powerSuspend, _powerResume;
		private static bool _suspended;
	}
}
