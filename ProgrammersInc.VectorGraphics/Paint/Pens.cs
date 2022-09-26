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

namespace ProgrammersInc.VectorGraphics.Paint.Pens
{
	public abstract class PenVisitor
	{
		public abstract void VisitSolidPen( SolidPen solidPen );
	}

	public abstract class Pen
	{
		protected Pen()
		{
		}

		public abstract void Visit( PenVisitor visitor );
	}

	public class SolidPen : Pen
	{
		public SolidPen( Color color, double width )
		{
			if( color == null )
			{
				throw new ArgumentNullException( "color" );
			}
			if( width < 0 )
			{
				throw new ArgumentException( "Width must be non-negative.", "width" );
			}

			_color = color;
			_width = width;
		}

		public Color Color
		{
			get
			{
				return _color;
			}
		}

		public double Width
		{
			get
			{
				return _width;
			}
		}

		public override void Visit( PenVisitor visitor )
		{
			visitor.VisitSolidPen( this );
		}

		private Color _color;
		private double _width;
	}
}
