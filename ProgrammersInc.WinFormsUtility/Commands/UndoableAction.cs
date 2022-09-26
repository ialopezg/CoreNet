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
	public abstract class UndoableAction
	{
		protected UndoableAction()
		{
		}

		public abstract string UndoTitle
		{
			get;
		}

		public abstract string RedoTitle
		{
			get;
		}

		public abstract bool Undo( Control owner );
		public abstract bool Redo( Control owner );
	}

	public class CompositeUndoableAction : UndoableAction
	{
		public CompositeUndoableAction( string undoTitle, string redoTitle )
		{
			if( undoTitle == null )
			{
				throw new ArgumentNullException( "undoTitle" );
			}
			if( redoTitle == null )
			{
				throw new ArgumentNullException( "redoTitle" );
			}

			_undoTitle = undoTitle;
			_redoTitle = redoTitle;
		}

		public override string UndoTitle
		{
			get
			{
				return _undoTitle;
			}
		}

		public override string RedoTitle
		{
			get
			{
				return _redoTitle;
			}
		}

		public override bool Undo( Control owner )
		{
			bool success = false;

			UndoableAction[] actions = _actions.ToArray();

			Array.Reverse( actions );

			foreach( UndoableAction action in actions )
			{
				success |= action.Undo( owner );
			}

			return success;
		}

		public override bool Redo( Control owner )
		{
			bool success = false;

			foreach( UndoableAction action in _actions )
			{
				success |= action.Redo( owner );
			}

			return success;
		}

		public void AddAction( UndoableAction action )
		{
			if( action == null )
			{
				throw new ArgumentNullException( "action" );
			}

			_actions.Add( action );
		}

		private string _undoTitle, _redoTitle;
		private List<UndoableAction> _actions = new List<UndoableAction>();
	}
}
