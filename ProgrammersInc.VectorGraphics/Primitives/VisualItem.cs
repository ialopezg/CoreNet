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
	public abstract class VisualItem
	{
		protected VisualItem()
		{
			_style = new Styles.Style();
		}

		public Container Parent
		{
			get
			{
				return _parent;
			}
			internal set
			{
				_parent = value;
			}
		}

		public Styles.Style Style
		{
			get
			{
				return _style;
			}
		}

		public Types.Rectangle GetBounds( Renderers.Renderer renderer )
		{
			if( _bounds == null )
			{
				_bounds = CalculateBounds( renderer );

				if( _bounds == null )
				{
					throw new InvalidOperationException();
				}
			}
			return _bounds;
		}

		public abstract void Visit( Visitor visitor );

		public abstract VisualItem Copy();

		protected abstract Types.Rectangle CalculateBounds( Renderers.Renderer renderer );

		internal void DirtyBounds()
		{
			_bounds = null;

			if( Parent != null )
			{
				Parent.DirtyBounds();
			}
		}

		private Styles.Style _style;
		private Container _parent;
		private Types.Rectangle _bounds;
	}
}
