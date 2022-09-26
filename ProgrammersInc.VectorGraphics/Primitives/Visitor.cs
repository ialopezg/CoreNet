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
	public abstract class Visitor
	{
		protected Visitor()
		{
		}

		public virtual void PreVisitVisualItem( VisualItem visualItem )
		{
		}

		public virtual void VisitBoundsMarker( BoundsMarker boundsMarker )
		{
		}

		public virtual void VisitContainer( Container container )
		{
		}

		public virtual bool VisitContainerPreChildren( Container container )
		{
			return true;
		}

		public virtual void VisitContainerPostChildren( Container container )
		{
		}

		public virtual void VisitPath( Path path )
		{
		}

		public virtual void VisitPointMarker( PointMarker pointMarker )
		{
		}

		public virtual void VisitText( Text text )
		{
		}

		public virtual void PostVisitVisualItem( VisualItem visualItem )
		{
		}
	}

	public sealed class DelegateVisitor : Visitor
	{
		public delegate void VisitItem<ItemType>( ItemType item );

		public VisitItem<BoundsMarker> VisitBoundsMarkerDelegate;
		public VisitItem<Container> VisitContainerDelegate;
		public VisitItem<Path> VisitPathDelegate;
		public VisitItem<PointMarker> VisitPointMarkerDelegate;
		public VisitItem<Text> VisitTextDelegate;

		public override void VisitBoundsMarker( BoundsMarker boundsMarker )
		{
			if( VisitBoundsMarkerDelegate != null )
			{
				VisitBoundsMarkerDelegate( boundsMarker );
			}
		}

		public override void VisitContainer( Container container )
		{
			if( VisitContainerDelegate != null )
			{
				VisitContainerDelegate( container );
			}
		}

		public override void VisitPath( Path path )
		{
			if( VisitPathDelegate != null )
			{
				VisitPathDelegate( path );
			}
		}

		public override void VisitPointMarker( PointMarker pointMarker )
		{
			if( VisitPointMarkerDelegate != null )
			{
				VisitPointMarkerDelegate( pointMarker );
			}
		}

		public override void VisitText( Text text )
		{
			if( VisitTextDelegate != null )
			{
				VisitTextDelegate( text );
			}
		}
	}
}
