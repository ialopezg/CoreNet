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

namespace ProgrammersInc.VectorGraphics.Styles.Modifiers
{
	public abstract class PathModifier : Modifier
	{
		protected PathModifier()
		{
		}

		public override void Apply( Renderers.Renderer renderer, Primitives.VisualItem item )
		{
			Primitives.DelegateVisitor visitor = new Primitives.DelegateVisitor();

			visitor.VisitPathDelegate = delegate( Primitives.Path path )
			{
				Apply( renderer, path );
			};

			item.Visit( visitor );
		}

		protected abstract void Apply( Renderers.Renderer renderer, Primitives.Path path );
	}
}
