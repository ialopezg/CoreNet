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
	public abstract class TextModifier : Modifier
	{
		protected TextModifier()
		{
		}

		public override void Apply( Renderers.Renderer renderer, Primitives.VisualItem item )
		{
			Primitives.DelegateVisitor visitor = new Primitives.DelegateVisitor();

			visitor.VisitTextDelegate = delegate( Primitives.Text text )
			{
				Apply( renderer, text );
			};

			item.Visit( visitor );
		}

		protected abstract void Apply( Renderers.Renderer renderer, Primitives.Text text );
	}
}
