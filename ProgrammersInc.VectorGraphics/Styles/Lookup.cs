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

namespace ProgrammersInc.VectorGraphics.Styles
{
	public sealed class Lookup
	{
		public Lookup( Primitives.Container container )
		{
			if( container == null )
			{
				throw new ArgumentNullException( "container" );
			}

			Visitor visitor = new Visitor( _mapStyleToItems );

			container.Visit( visitor );
		}

		public Primitives.VisualItem[] GetVisualItems( string styleClass )
		{
			List<Primitives.VisualItem> list;

			if( _mapStyleToItems.TryGetValue( styleClass, out list ) )
			{
				return list.ToArray();
			}
			else
			{
				return new Primitives.VisualItem[] { };
			}
		}

		#region Visitor

		private sealed class Visitor : Primitives.Visitor
		{
			internal Visitor( Dictionary<string, List<Primitives.VisualItem>> mapStyleToItems )
			{
				_mapStyleToItems = mapStyleToItems;
			}

			public override void PreVisitVisualItem( Primitives.VisualItem visualItem )
			{
				foreach( string c in visualItem.Style.Classes )
				{
					List<Primitives.VisualItem> items;

					if( !_mapStyleToItems.TryGetValue( c, out items ) )
					{
						items = new List<Primitives.VisualItem>();
						_mapStyleToItems.Add( c, items );
					}

					items.Add( visualItem );
				}
			}

			private Dictionary<string, List<Primitives.VisualItem>> _mapStyleToItems;
		}

		#endregion

		private Dictionary<string, List<Primitives.VisualItem>> _mapStyleToItems = new Dictionary<string, List<Primitives.VisualItem>>();
	}
}
