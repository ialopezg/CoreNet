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
using System.Diagnostics;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public sealed class CommandControlSet : IDisposable
	{
		public CommandControlSet( CommandControlSet chain, Form form )
		{
			_chain = chain;
			_form = form;
		}

		public Form Form
		{
			get
			{
				return _form;
			}
		}

		public CommandControlSet ChainParent
		{
			get
			{
				return _chain;
			}
		}

		public void AddControl( ICommandControl control )
		{
			if( control == null )
			{
				throw new ArgumentNullException( "control" );
			}
			if( control.CommandControlSet != null )
			{
				throw new InvalidOperationException( "Control is already attached to a command control set." );
			}

			_controls.Add( control );
			control.CommandControlSet = this;
		}
		
		public void RemoveControl( ICommandControl control )
		{
			if( control == null )
			{
				throw new ArgumentNullException( "control" );
			}
			if( control.CommandControlSet != this )
			{
				throw new InvalidOperationException( "Control is not attached to this command control set." );
			}

			_controls.Remove( control );
			control.CommandControlSet = this;
		}

		public void UpdateState()
		{
			foreach( ICommandControl control in _controls )
			{
				control.UpdateState();
			}
		}

		public IDisposable UsingCurrentInvocation()
		{
			_invocationStack.Push( this );

			return new StackPopper( _invocationStack );
		}

		public IDisposable UsingForm()
		{
			_formStack.Push( this );

			return new StackPopper( _formStack );
		}

		public static CommandControlSet CurrentInvocationContext
		{
			get
			{
				if( _invocationStack.Count == 0 )
				{
					return null;
				}
				else
				{
					return _invocationStack.Peek();
				}
			}
		}

		public static CommandControlSet CurrentFormContext
		{
			get
			{
				if( _formStack.Count == 0 )
				{
					return null;
				}
				else
				{
					return _formStack.Peek();
				}
			}
		}

		public static T FormInvocationInstance<T>()
			where T : CommandForm
		{
			return FormInstance<T>( CurrentInvocationContext );
		}

		public static T FormInstance<T>()
				where T : CommandForm
		{
			return FormInstance<T>( CurrentFormContext );
		}

		private static T FormInstance<T>( CommandControlSet ccs )
				where T : CommandForm
		{
			while( ccs != null )
			{
				T form = ccs.Form as T;

				if( form != null )
				{
					return form;
				}

				ccs = ccs.ChainParent;
			}

			return null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if( _chain != null )
			{
				_chain.UpdateState();
				_chain = null;
			}
		}

		#endregion

		#region StackPopper

		internal sealed class StackPopper : IDisposable
		{
			internal StackPopper( Stack<CommandControlSet> stack )
			{
				_stack = stack;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if( _stack != null )
				{
					_stack.Pop();
					_stack = null;
				}
			}

			#endregion

			private Stack<CommandControlSet> _stack;
		}

		#endregion

		private CommandControlSet _chain;
		private List<ICommandControl> _controls = new List<ICommandControl>();
		private Form _form;

		private static Stack<CommandControlSet> _invocationStack = new Stack<CommandControlSet>();
		private static Stack<CommandControlSet> _formStack = new Stack<CommandControlSet>();
	}
}
