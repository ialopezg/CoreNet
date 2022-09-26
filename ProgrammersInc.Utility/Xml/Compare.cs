using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ProgrammersInc.Utility
{
	public static class Xml
	{
		public static bool Compare( XmlDocument first, XmlDocument second )
		{
			if( first == null )
			{
				throw new ArgumentNullException( "first" );
			}
			if( second == null )
			{
				throw new ArgumentNullException( "second" );
			}

			return Compare( first.DocumentElement, second.DocumentElement );
		}

		public static bool Compare( XmlNode first, XmlNode second )
		{
			if( first == null )
			{
				throw new ArgumentNullException( "first" );
			}
			if( second == null )
			{
				throw new ArgumentNullException( "second" );
			}

			if( first.Name != second.Name )
			{
				return false;
			}

			int firstAttributeCount = first.Attributes == null ? 0 : first.Attributes.Count;
			int secondAttributeCount = second.Attributes == null ? 0 : second.Attributes.Count;

			if( firstAttributeCount != secondAttributeCount )
			{
				return false;
			}
			for( int i = 0; i < firstAttributeCount; ++i )
			{
				if( first.Attributes[i].Value != second.Attributes[i].Value )
				{
					return false;
				}
			}

			if( first.ChildNodes.Count != second.ChildNodes.Count )
			{
				return false;
			}
			for( int i = 0; i < first.ChildNodes.Count; ++i )
			{
				if( !Compare( first.ChildNodes[i], second.ChildNodes[i] ) )
				{
					return false;
				}
			}

			return true;
		}
	}
}
