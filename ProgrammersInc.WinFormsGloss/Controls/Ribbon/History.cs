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

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public sealed class History
	{
		public Section MouseOverSection
		{
			get
			{
				return _sectionHistory.MouseOver;
			}
		}

		public Item MouseOverItem
		{
			get
			{
				return _itemHistory.MouseOver;
			}
		}

		public double? GetTimeOverSection( Section section )
		{
			return _sectionHistory.GetTimeOver( section );
		}

		public double? GetTimeOverItem( Item item )
		{
			return _itemHistory.GetTimeOver( item );
		}

		public double GetLastOverSection( Section section )
		{
			return _sectionHistory.GetLastOver( section );
		}

		public double GetLastOverItem( Item item )
		{
			return _itemHistory.GetLastOver( item );
		}

		public void Update( Section overSection, Item overItem )
		{
			_sectionHistory.Update( overSection );
			_itemHistory.Update( overItem );
		}

		private Drawing.GlowHistory<Section> _sectionHistory = new Drawing.GlowHistory<Section>( null );
		private Drawing.GlowHistory<Item> _itemHistory = new Drawing.GlowHistory<Item>( null );
	}
}
