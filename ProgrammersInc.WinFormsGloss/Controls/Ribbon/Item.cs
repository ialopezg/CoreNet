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
using System.Diagnostics;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public abstract class Item
	{
		public const int StandardImportance = 10;
		public const int BestLevelOfDetail = -2;
		public const int WorstLevelOfDetail = 20;

		protected Item()
		{
		}

		public Section Section
		{
			[DebuggerStepThrough]
			get
			{
				return _section;
			}
			internal set
			{
				_section = value;
			}
		}

		public int Importance
		{
			[DebuggerStepThrough]
			get
			{
				return _importance;
			}
			set
			{
				_importance = value;
			}
		}

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if( _visible == value )
				{
					return;
				}

				_visible = value;

				if( Section != null )
				{
					Section.NotifyItemChanged( this );
				}
			}
		}

		public virtual bool NeedsMouseOverUpdate
		{
			get
			{
				return false;
			}
		}

		public void PerformClick( Context context )
		{
			OnClick( context );
		}

		public abstract Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize );

		public abstract void Paint( Context context, Rectangle clip, Rectangle logicalBounds );

		public virtual bool HasStyle( string style )
		{
			return false;
		}

		public virtual SuperToolTipInfo GetTooltipInfo()
		{
			return null;
		}

		public virtual System.Windows.Forms.ToolStripItem CreateEquivalentToolStripItem()
		{
			return null;
		}

		protected virtual bool OnClick( Context context )
		{
			return false;
		}

		private Section _section;
		private int _importance = Item.StandardImportance;
		private bool _visible = true;
	}
}
