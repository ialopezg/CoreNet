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

namespace ProgrammersInc.WinFormsUtility.Commands
{
	public sealed class UndoRedoStack
	{
		#region Commands

		public class UndoCommand : Command
		{
			public UndoCommand( UndoRedoStack undoRedoStack )
			{
				if( undoRedoStack == null )
				{
					throw new ArgumentNullException( "undoRedoStack" );
				}

				_undoRedoStack = undoRedoStack;
			}

			public override bool IsEnabled()
			{
				return _undoRedoStack.CanUndo;
			}

			protected override bool PerformInvoke( Control owner )
			{
				_undoRedoStack.Undo( owner );

				return true;
			}

			private UndoRedoStack _undoRedoStack;
		}

		public class RedoCommand : Command
		{
			public RedoCommand( UndoRedoStack undoRedoStack )
			{
				if( undoRedoStack == null )
				{
					throw new ArgumentNullException( "undoRedoStack" );
				}

				_undoRedoStack = undoRedoStack;
			}

			public override bool IsEnabled()
			{
				return _undoRedoStack.CanRedo;
			}

			protected override bool PerformInvoke( Control owner )
			{
				_undoRedoStack.Redo( owner );

				return true;
			}

			private UndoRedoStack _undoRedoStack;
		}

		#endregion

		public UndoRedoStack( int maxItems )
		{
			_maxItems = maxItems;
		}

		public bool CanUndo
		{
			get
			{
				return _position > 0;
			}
		}

		public bool CanRedo
		{
			get
			{
				return _position < _actions.Count;
			}
		}

		public UndoableAction UndoAction
		{
			get
			{
				if( CanUndo )
				{
					return _actions[_position - 1];
				}
				else
				{
					return null;
				}
			}
		}

		public UndoableAction RedoAction
		{
			get
			{
				if( CanRedo )
				{
					return _actions[_position];
				}
				else
				{
					return null;
				}
			}
		}

		public void AddAction( UndoableAction action )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}
			if( _changing.IsActive )
			{
				return;
			}

			if( _composites.Count > 0 )
			{
				CompositeUndoableAction composite = _composites.Peek();

				composite.AddAction( action );
			}
			else
			{
				_actions.RemoveRange( _position, _actions.Count - _position );
				_actions.Add( action );

				if( _actions.Count > _maxItems )
				{
					_actions.RemoveAt( 0 );
				}

				_position = _actions.Count;

				OnChanged( EventArgs.Empty );
			}
		}

		public void Undo( Control owner )
		{
			if( !CanUndo )
			{
				throw new InvalidOperationException();
			}

			using( _changing.Apply() )
			{
				while( CanUndo )
				{
					--_position;

					UndoableAction action = _actions[_position];

					if( action.Undo( owner ) )
					{
						break;
					}
				}

				OnChanged( EventArgs.Empty );
			}
		}

		public void Redo( Control owner )
		{
			if( !CanRedo )
			{
				throw new InvalidOperationException();
			}

			using( _changing.Apply() )
			{
				while( CanRedo )
				{
					UndoableAction action = _actions[_position];

					++_position;

					if( action.Redo( owner ) )
					{
						break;
					}
				}

				OnChanged( EventArgs.Empty );
			}
		}

		public IDisposable CreateComposite( string undoTitle, string redoTitle )
		{
			return new ComposeDisposer( this, new CompositeUndoableAction( undoTitle, redoTitle ) );
		}

		public IDisposable CreateComposite( CompositeUndoableAction action )
		{
			return new ComposeDisposer( this, action );
		}

		private void OnChanged( EventArgs e )
		{
			if( Changed != null )
			{
				Changed( this, e );
			}
		}

		private sealed class ComposeDisposer : IDisposable
		{
			internal ComposeDisposer( UndoRedoStack undoRedoStack, CompositeUndoableAction action )
			{
				_undoRedoStack = undoRedoStack;
				_undoRedoStack._composites.Push( action );
			}

			#region IDisposable Members

			public void Dispose()
			{
				if( _undoRedoStack != null )
				{
					CompositeUndoableAction action = _undoRedoStack._composites.Pop();

					_undoRedoStack.AddAction( action );

					_undoRedoStack = null;
				}
			}

			#endregion

			private UndoRedoStack _undoRedoStack;
		}

		public event EventHandler Changed;

		private int _position;
		private List<UndoableAction> _actions = new List<UndoableAction>();
		private Utility.Control.Flag _changing = new Utility.Control.Flag();
		private int _maxItems;
		private Stack<CompositeUndoableAction> _composites = new Stack<CompositeUndoableAction>();
	}
}
