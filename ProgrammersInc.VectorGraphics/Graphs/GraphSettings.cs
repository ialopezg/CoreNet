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

namespace ProgrammersInc.VectorGraphics.Graphs
{
	public abstract class GraphSettings
	{
		protected GraphSettings()
		{
		}

		public double Width
		{
			get
			{
				return _width;
			}
			set
			{
				if( value <= 0 )
				{
					throw new ArgumentException( "Width must be greater than zero.", "value" );
				}

				_width = value;
			}
		}

		public double Height
		{
			get
			{
				return _height;
			}
			set
			{
				if( value <= 0 )
				{
					throw new ArgumentException( "Height must be greater than zero.", "value" );
				}

				_height = value;
			}
		}

		private double _width = 100, _height = 100;
	}
}
