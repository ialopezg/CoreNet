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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public enum Alignment
	{
		Left,
		Right
	}

	public class Section
	{
		public Section()
		{
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_title = value;

				if( Ribbon != null )
				{
					Ribbon.NotifySectionChanged( this );
				}
			}
		}

		public int DisplayUntil
		{
			get
			{
				return _displayUntil;
			}
			set
			{
				if( value == _displayUntil )
				{
					return;
				}

				_displayUntil = value;

				if( Ribbon != null )
				{
					Ribbon.NotifySectionChanged( this );
				}
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

				if( Ribbon != null )
				{
					Ribbon.NotifyNeedsLayout();
				}
			}
		}

		public Alignment Alignment
		{
			get
			{
				return _alignment;
			}
			set
			{
				_alignment = value;

				if( Ribbon != null )
				{
					Ribbon.NotifyNeedsLayout();
				}
			}
		}

		public RibbonControl Ribbon
		{
			get
			{
				return _ribbon;
			}
			internal set
			{
				_ribbon = value;
			}
		}

		public Item[] Items
		{
			get
			{
				return _items.ToArray();
			}
		}

		public void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			context.Renderer.PaintSection( context, clip, logicalBounds, this );
		}

		public void AddItem( Item item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}
			if( item.Section != null )
			{
				throw new ArgumentException( "Item is already part of a section.", "item" );
			}

			_items.Add( item );
			item.Section = this;

			if( Ribbon != null )
			{
				Ribbon.NotifyNeedsLayout();
			}
		}

		public void ReplaceItem( Item oldItem, Item newItem )
		{
			if( oldItem == null )
			{
				throw new ArgumentNullException( "oldItem" );
			}
			if( newItem == null )
			{
				throw new ArgumentNullException( "newItem" );
			}
			if( newItem.Section != null )
			{
				throw new ArgumentException( "Item is already part of a section.", "newItem" );
			}

			int index = _items.IndexOf( oldItem );

			_items.RemoveAt( index );
			_items.Insert( index, newItem );

			newItem.Section = this;

			if( Ribbon != null )
			{
				Ribbon.NotifyNeedsLayout();
			}
		}

		public void NotifyItemChanged( Item item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}
			if( !_items.Contains( item ) )
			{
				throw new ArgumentException( "Section does not contain this item.", "item" );
			}

			if( Ribbon != null )
			{
				Ribbon.NotifySectionChanged( this );
			}
		}

		public virtual void PreLayout( RibbonControl ribbonControl, int levelOfDetail )
		{
		}

		private RibbonControl _ribbon;
		private string _title = string.Empty;
		private List<Item> _items = new List<Item>();
		private bool _visible = true;
		private int _displayUntil = Item.WorstLevelOfDetail + 1;
		private Alignment _alignment = Alignment.Left;
	}
}
