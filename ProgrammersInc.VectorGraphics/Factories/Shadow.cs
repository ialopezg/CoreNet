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

namespace ProgrammersInc.VectorGraphics.Factories
{
	public abstract class Shadow
	{
		protected Shadow()
		{
		}

		public abstract Primitives.VisualItem Create( Primitives.Path source );

		public void Apply( Primitives.Container container )
		{
			List<Primitives.Path> sources = new List<Primitives.Path>();
			Primitives.DelegateVisitor visitor = new Primitives.DelegateVisitor();

			visitor.VisitPathDelegate = delegate( Primitives.Path path )
			{
				sources.Add( path );
			};

			container.Visit( visitor );

			foreach( Primitives.Path source in sources )
			{
				container.AddFront( Create( source ) );
			}
		}
	}
}
