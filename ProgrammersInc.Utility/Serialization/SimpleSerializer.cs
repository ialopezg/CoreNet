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
using System.Xml;
using System.Reflection;

namespace ProgrammersInc.Utility.Serialization
{
	public sealed class SimpleSerializer
	{
		public SimpleSerializer()
		{
			RegisterConverter( new StringConverter() );
			RegisterConverter( new EnumConverter() );
			RegisterConverter( new BooleanConverter() );
			RegisterConverter( new IntegerConverter() );
			RegisterConverter( new DecimalConverter() );
			RegisterConverter( new GuidConverter() );
		}

		public void Write( XmlTextWriter tw, object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException( "obj" );
			}

			string typeName = obj.GetType().Name;

			tw.WriteStartElement( typeName );

			WriteTypeContents( tw, obj );

			tw.WriteEndElement();
		}

		public void Read( XmlReader tr, object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException( "obj" );
			}

			string typeName = obj.GetType().Name;

			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load( tr );

			XmlNode node = xmlDoc.SelectSingleNode( typeName );

			if( node != null )
			{
				ReadTypeContents( node, obj );
			}
		}

		public void RegisterConverter( Converter converter )
		{
			if( converter == null )
			{
				throw new ArgumentNullException( "converter" );
			}

			_converters.Add( converter );
		}

		private void ReadTypeContents( XmlNode node, object obj )
		{
			foreach( FieldInfo fieldInfo in GetFields( obj ) )
			{
				XmlNode fieldNode = node.SelectSingleNode( fieldInfo.Name );

				if( fieldNode != null )
				{
					try
					{
						object fieldValue = fieldInfo.GetValue( obj );
						object newValue = ReadValue( fieldNode, fieldInfo.Name, fieldInfo.FieldType, fieldValue );

						fieldInfo.SetValue( obj, newValue );
					}
					catch
					{
					}
				}
			}
			foreach( PropertyInfo propertyInfo in GetProperties( obj ) )
			{
				XmlNode propertyNode = node.SelectSingleNode( propertyInfo.Name );

				if( propertyNode != null )
				{
					try
					{
						object propertyValue = propertyInfo.GetValue( obj, new object[] { } );
						object newValue = ReadValue( propertyNode, propertyInfo.Name, propertyInfo.PropertyType, propertyValue );

						if( propertyInfo.CanWrite )
						{
							propertyInfo.SetValue( obj, newValue, new object[] { } );
						}
					}
					catch
					{
					}
				}
			}
		}

		private object ReadValue( XmlNode node, string name, Type type, object existing )
		{
			Converter converter = FindConverter( type );

			if( converter != null )
			{
				return converter.Read( type, node.InnerText, existing );
			}
			else
			{
				if( type.IsClass && type.IsSealed )
				{
					ReadTypeContents( node, existing );
				}
			}

			return existing;
		}

		private void WriteTypeContents( XmlTextWriter tw, object obj )
		{
			foreach( FieldInfo fieldInfo in GetFields( obj ) )
			{
				object fieldValue = fieldInfo.GetValue( obj );

				WriteValue( tw, fieldInfo.Name, fieldInfo.FieldType, fieldValue );
			}
			foreach( PropertyInfo propertyInfo in GetProperties( obj ) )
			{
				object propertyValue = propertyInfo.GetValue( obj, new object[] { } );

				WriteValue( tw, propertyInfo.Name, propertyInfo.PropertyType, propertyValue );
			}
		}

		private void WriteValue( XmlTextWriter tw, string name, Type type, object obj )
		{
			Converter converter = FindConverter( type );

			if( converter != null )
			{
				tw.WriteStartElement( name );
				tw.WriteValue( converter.Write( obj ) );
				tw.WriteEndElement();
			}
			else
			{
				if( type.IsClass && type.IsSealed )
				{
					tw.WriteStartElement( name );
					WriteTypeContents( tw, obj );
					tw.WriteEndElement();
				}
				else
				{
					throw new InvalidOperationException( string.Format( "Cannot handle type '{0}'.", type.Name ) );
				}
			}
		}

		private Converter FindConverter( Type type )
		{
			foreach( Converter c in _converters )
			{
				if( c.Type.IsAssignableFrom( type ) )
				{
					return c;
				}
			}

			return null;
		}

		private FieldInfo[] GetFields( object obj )
		{
			return obj.GetType().GetFields( BindingFlags.Instance | BindingFlags.Public );
		}

		private PropertyInfo[] GetProperties( object obj )
		{
			return Array.FindAll<PropertyInfo>( obj.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ), delegate( PropertyInfo pi )
			{
				return pi.CanRead;
			} );
		}

		#region Converter

		public abstract class Converter
		{
			protected Converter( Type type )
			{
				if( type == null )
				{
					throw new ArgumentNullException( "type" );
				}

				_type = type;
			}

			public Type Type
			{
				get
				{
					return _type;
				}
			}

			public abstract string Write( object obj );
			public abstract object Read( Type type, string rep, object existing );

			private Type _type;
		}

		#endregion

		#region StringConverter

		public sealed class StringConverter : Converter
		{
			public StringConverter()
				: base( typeof( string ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					return "(null)";
				}
				else
				{
					return (string) obj;
				}
			}

			public override object Read( Type type, string rep, object existing )
			{
				if( rep == "(null)" )
				{
					return null;
				}
				else
				{
					return rep;
				}
			}
		}

		#endregion
		#region EnumConverter

		public sealed class EnumConverter : Converter
		{
			public EnumConverter()
				: base( typeof( Enum ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					throw new InvalidOperationException( "Cannot handle null enums." );
				}

				return Enum.GetName( obj.GetType(), obj );
			}

			public override object Read( Type type, string rep, object existing )
			{
				return Enum.Parse( type, rep );
			}
		}

		#endregion
		#region BooleanConverter

		public sealed class BooleanConverter : Converter
		{
			public BooleanConverter()
				: base( typeof( bool ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					throw new InvalidOperationException( "Cannot handle null bools." );
				}

				bool b = (bool) obj;

				return b ? "true" : "false";
			}

			public override object Read( Type type, string rep, object existing )
			{
				if( rep == "true" )
				{
					return true;
				}
				else if( rep == "false" )
				{
					return false;
				}
				else
				{
					throw new InvalidOperationException( "Unknown boolean value." );
				}
			}
		}

		#endregion
		#region IntegerConverter

		public sealed class IntegerConverter : Converter
		{
			public IntegerConverter()
				: base( typeof( int ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					throw new InvalidOperationException( "Cannot handle null ints." );
				}

				int i = (int) obj;

				return i.ToString( System.Globalization.CultureInfo.InvariantCulture );
			}

			public override object Read( Type type, string rep, object existing )
			{
				return int.Parse( rep, System.Globalization.CultureInfo.InvariantCulture );
			}
		}

		#endregion
		#region DecimalConverter

		public sealed class DecimalConverter : Converter
		{
			public DecimalConverter()
				: base( typeof( decimal ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					throw new InvalidOperationException( "Cannot handle null decimals." );
				}

				decimal d = (decimal) obj;

				return d.ToString( System.Globalization.CultureInfo.InvariantCulture );
			}

			public override object Read( Type type, string rep, object existing )
			{
				return decimal.Parse( rep, System.Globalization.CultureInfo.InvariantCulture );
			}
		}

		#endregion
		#region GuidConverter

		public sealed class GuidConverter : Converter
		{
			public GuidConverter()
				: base( typeof( Guid ) )
			{
			}

			public override string Write( object obj )
			{
				if( obj == null )
				{
					throw new InvalidOperationException( "Cannot handle null GUIDs." );
				}

				return obj.ToString();
			}

			public override object Read( Type type, string rep, object existing )
			{
				return new Guid( rep );
			}
		}

		#endregion

		private List<Converter> _converters = new List<Converter>();
	}
}
