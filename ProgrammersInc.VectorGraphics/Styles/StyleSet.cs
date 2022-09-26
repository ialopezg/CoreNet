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

namespace ProgrammersInc.VectorGraphics.Styles
{
	public abstract class StyleSet
	{
		protected StyleSet()
		{
		}

		public void Apply( Renderers.Renderer renderer, Primitives.Container container )
		{
			foreach( KeyValuePair<string, Modifier> kvp in _modifiers )
			{
				Lookup lookup = new Lookup( container );

				string style = kvp.Key;
				Modifier modifier = kvp.Value;

				Primitives.VisualItem[] items = lookup.GetVisualItems( style );

				foreach( Primitives.VisualItem item in items )
				{
					modifier.Apply( renderer, item );
				}
			}
		}

		protected void AddModifier( string styleClass, Modifier modifier )
		{
			if( styleClass == null )
			{
				throw new ArgumentNullException( "styleClass" );
			}
			if( modifier == null )
			{
				throw new ArgumentNullException( "modifier" );
			}

			_modifiers.Add( new KeyValuePair<string, Modifier>( styleClass, modifier ) );
		}

		private List<KeyValuePair<string, Modifier>> _modifiers = new List<KeyValuePair<string, Modifier>>();
	}
}
