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
	public class TextFontModifier : TextModifier
	{
		public TextFontModifier( string fontFamily, double? fontSizeInPoints, Primitives.Text.FontStyleFlags? fontStyleFlags )
		{
			_fontFamily = fontFamily;
			_fontSizeInPoints = fontSizeInPoints;
			_fontStyleFlags = fontStyleFlags;
		}

		protected override void Apply( Renderers.Renderer renderer, Primitives.Text text )
		{
			if( _fontFamily != null )
			{
				text.FontFamily = _fontFamily;
			}

			if( _fontSizeInPoints != null )
			{
				text.FontSizePoints = _fontSizeInPoints.Value;
			}

			if( _fontStyleFlags != null )
			{
				text.FontStyle = _fontStyleFlags.Value;
			}
		}

		private string _fontFamily;
		private double? _fontSizeInPoints;
		private Primitives.Text.FontStyleFlags? _fontStyleFlags;
	}
}
