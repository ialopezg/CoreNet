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

namespace ProgrammersInc.VectorGraphics.Primitives
{
	public sealed class Container : VisualItem, IEnumerable<VisualItem>
	{
		public Container()
			: this( new Types.Point( 0, 0 ), null )
		{
		}

		public Container( Types.Point offset )
			: this( offset, null )
		{
		}

		public Container( Types.Point offset, Path clip )
		{
			if( offset == null )
			{
				throw new ArgumentNullException( "offset" );
			}

			_offset = offset;
			_clip = clip;
		}

		public Types.Point Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_offset = value;

				DirtyBounds();
			}
		}

		public void AddBack( VisualItem item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}
			if( item.Parent != null )
			{
				throw new ArgumentException( "Item is already parented.", "item" );
			}

			item.Parent = this;
			_items.Add( item );
			DirtyBounds();
		}

		public void AddFront( VisualItem item )
		{
			if( item == null )
			{
				throw new ArgumentNullException( "item" );
			}
			if( item.Parent != null )
			{
				throw new ArgumentException( "Item is already parented.", "item" );
			}

			item.Parent = this;
			_items.Insert( 0, item );
			DirtyBounds();
		}

		public void Replace( VisualItem from, VisualItem to )
		{
			if( from == null )
			{
				throw new ArgumentNullException( "from" );
			}

			int index = _items.IndexOf( from );

			if( index < 0 )
			{
				throw new ArgumentException( "Item not found.", "from" );
			}

			_items.RemoveAt( index );

			if( to != null )
			{
				_items.Insert( index, to );
			}
		}

		public override void Visit( Visitor visitor )
		{
			visitor.PreVisitVisualItem( this );
			visitor.VisitContainer( this );

			if( visitor.VisitContainerPreChildren( this ) )
			{
				foreach( VisualItem child in this )
				{
					child.Visit( visitor );
				}
				visitor.VisitContainerPostChildren( this );
			}

			visitor.PostVisitVisualItem( this );
		}

		public override VisualItem Copy()
		{
			Container container = new Container( _offset, _clip );

			foreach( VisualItem child in this )
			{
				container.AddBack( child.Copy() );
			}

			return container;
		}

		protected override Types.Rectangle CalculateBounds( Renderers.Renderer renderer )
		{
			List<Types.Rectangle> childBoundsList = new List<Types.Rectangle>();

			foreach( VisualItem child in this )
			{
				Types.Rectangle childBounds = child.GetBounds( renderer );

				childBounds = Types.Rectangle.Offset( childBounds, Offset );

				childBoundsList.Add( childBounds );
			}

			Types.Rectangle bounds = Types.Rectangle.Union( childBoundsList.ToArray() );

			return bounds;
		}

		#region IEnumerable<VisualItem> Members

		public IEnumerator<VisualItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		private Types.Point _offset;
		private Path _clip;
		private List<VisualItem> _items = new List<VisualItem>();
	}
}
