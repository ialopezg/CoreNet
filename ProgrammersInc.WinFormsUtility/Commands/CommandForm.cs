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

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public class CommandForm : System.Windows.Forms.Form
	{
		protected CommandForm()
		{
			Font = SystemFonts.DialogFont;
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			Destroy();
		}

		protected void UpdateState()
		{
			if( _commandControlSet != null )
			{
				_commandControlSet.UpdateState();
			}
		}

		public CommandControlSet CommandControlSet
		{
			get
			{
				EnsureCommandControlSet();

				return _commandControlSet;
			}
		}

		protected void RegisterControl( ICommandControl control )
		{
			if( _commandControlSet == null )
			{
				_deferredControls.Add( control );
			}
			else
			{
				_commandControlSet.AddControl( control );
			}
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			EnsureCommandControlSet();

			_usingForm = _commandControlSet.UsingForm();

			_commandControlSet.UpdateState();
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

			Destroy();
		}

		private void Destroy()
		{
			if( _usingForm != null )
			{
				_usingForm.Dispose();
				_usingForm = null;
			}
			if( _commandControlSet != null )
			{
				_commandControlSet.Dispose();
				_commandControlSet = null;
			}

			_destroyed = true;
		}

		private void EnsureCommandControlSet()
		{
			if( _destroyed && !this.DesignMode )
			{
				throw new InvalidOperationException();
			}

			if( _commandControlSet == null )
			{
				CommandForm parent = Owner as CommandForm;

				_commandControlSet = new CommandControlSet( parent == null ? null : parent._commandControlSet, this );

				foreach( ICommandControl control in _deferredControls )
				{
					_commandControlSet.AddControl( control );
				}

				_deferredControls = new List<ICommandControl>();
			}
		}

		private CommandControlSet _commandControlSet;
		private List<ICommandControl> _deferredControls = new List<ICommandControl>();
		private IDisposable _usingForm;
		private bool _destroyed;
	}
}
