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
	public sealed class Updates
	{
		public Section[] Sections
		{
			get
			{
				return _sections.ToArray();
			}
		}

		public Item[] Items
		{
			get
			{
				return _items.ToArray();
			}
		}

		public void NeedsUpdate( Section section )
		{
			if( section == null )
			{
				throw new ArgumentNullException( "section" );
			}

			if( !_sections.Contains( section ) )
			{
				_sections.Add( section );

				foreach( Item item in section.Items )
				{
					_items.Remove( item );
				}
			}
		}

		public void NeedsUpdate( Item item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			if( item.Section == null )
			{
				if( !_items.Contains( item ) )
				{
					_items.Add( item );
				}
			}
			else
			{
				if( !_sections.Contains( item.Section ) )
				{
					if( !_items.Contains( item ) )
					{
						_items.Add( item );
					}
				}
			}
		}

		public void DoneUpdate( Section section )
		{
			if( section == null )
			{
				throw new ArgumentNullException( "section" );
			}

			_sections.Remove( section );
		}

		public void DoneUpdate( Item item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			_items.Remove( item );
		}

		private List<Section> _sections = new List<Section>();
		private List<Item> _items = new List<Item>();
	}
}
