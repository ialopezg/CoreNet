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
	public abstract class MarkerReplacementModifier : Modifier
	{
		protected MarkerReplacementModifier()
		{
		}

		public override void Apply( Renderers.Renderer renderer, Primitives.VisualItem item )
		{
			Primitives.DelegateVisitor visitor = new Primitives.DelegateVisitor();

			visitor.VisitBoundsMarkerDelegate = delegate( Primitives.BoundsMarker marker )
			{
				Primitives.VisualItem to = CreateItem( marker );

				if( to != null )
				{
					to.Style.MergeWith( marker.Style );
				}

				marker.Parent.Replace( marker, to );
			};
			visitor.VisitPointMarkerDelegate = delegate( Primitives.PointMarker marker )
			{
				Primitives.VisualItem to = CreateItem( marker );

				if( to != null )
				{
					to.Style.MergeWith( marker.Style );
				}

				marker.Parent.Replace( marker, to );
			};

			item.Visit( visitor );
		}

		protected abstract Primitives.VisualItem CreateItem( Primitives.BoundsMarker marker );
		protected abstract Primitives.VisualItem CreateItem( Primitives.PointMarker marker );
	}
}
