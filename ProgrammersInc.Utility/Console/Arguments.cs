using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProgrammersInc.Utility.Console
{
	public static class Argument
	{
		#region NameAttribute

		public sealed class NameAttribute : Attribute
		{
			public NameAttribute( string value )
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_value = value;
			}

			public string Value
			{
				get
				{
					return _value;
				}
			}

			private string _value;
		}

		#endregion
		#region HelpAttribute

		public sealed class HelpAttribute : Attribute
		{
			public HelpAttribute( string value )
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_value = value;
			}

			public string Value
			{
				get
				{
					return _value;
				}
			}

			private string _value;
		}

		#endregion
		#region DefaultAttribute

		public sealed class DefaultAttribute : Attribute
		{
			public DefaultAttribute( string value )
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_value = value;
			}

			public string Value
			{
				get
				{
					return _value;
				}
			}

			private string _value;
		}

		#endregion
		#region MandatoryAttribute

		public sealed class MandatoryAttribute : Attribute
		{
			public MandatoryAttribute()
			{
				_value = true;
			}

			public MandatoryAttribute( bool value )
			{
				_value = value;
			}

			public bool Value
			{
				get
				{
					return _value;
				}
			}

			private bool _value;
		}

		#endregion
	}

	public class ArgumentReader<T>
		where T : new()
	{

		public ArgumentReader( string[] args )
		{
			if( args == null )
			{
				throw new ArgumentNullException( "args" );
			}

			_args = args;

			MemberInfo[] members = typeof( T ).FindMembers(MemberTypes.Field,
                BindingFlags.Public | BindingFlags.Instance, null, null );

			_allowedSwitches = new Dictionary<string, ArgumentDetails>();

			Type[] allowedFieldTypes = new Type[]
			{
				typeof( bool ),
				typeof( string ),
				typeof( List<string> )
			};

			foreach( System.Reflection.FieldInfo fieldInfo in members )
			{
				string name = fieldInfo.Name;
				ArgumentDetails argumentDetails = new ArgumentDetails();

				argumentDetails.FieldInfo = fieldInfo;

				Argument.NameAttribute nameAttribute = GetAttribute<Argument.NameAttribute>( fieldInfo );
				Argument.HelpAttribute helpAttribute = GetAttribute<Argument.HelpAttribute>( fieldInfo );
				Argument.DefaultAttribute defaultAttribute = GetAttribute<Argument.DefaultAttribute>( fieldInfo );
				Argument.MandatoryAttribute mandatoryAttribute = GetAttribute<Argument.MandatoryAttribute>( fieldInfo );

				if( nameAttribute != null )
				{
					name = nameAttribute.Value;
				}
				if( helpAttribute != null )
				{
					argumentDetails.Help = helpAttribute.Value;
				}
				if( defaultAttribute != null )
				{
					argumentDetails.DefaultValue = defaultAttribute.Value;
				}
				if( mandatoryAttribute != null )
				{
					argumentDetails.Mandatory = mandatoryAttribute.Value;
				}

				if( fieldInfo.FieldType == typeof( bool ) )
				{
					argumentDetails.NeedsArgument = false;
				}
				else
				{
					argumentDetails.NeedsArgument = true;
				}

				if( argumentDetails.DefaultValue != null && argumentDetails.Mandatory )
				{
					throw new ArgumentException( string.Format( "The argument '{0}' cannot be mandatory and have a default value.", name ) );
				}
				if( _allowedSwitches.ContainsKey( name ) )
				{
					throw new ArgumentException( string.Format( "Argument '{0}' defined multiple times in '{1}'.",
						name, typeof( T ).Name ) );
				}
				if( Array.IndexOf( allowedFieldTypes, fieldInfo.FieldType ) < 0 )
				{
					throw new ArgumentException( string.Format( "Field '{0}' must be of type bool, string, or List<string>.", name ) );
				}

				_allowedSwitches[name] = argumentDetails;
			}
		}

		public T ReadArguments()
		{
			T target = new T();
			return ReadArguments( target ) ? target : default( T );
		}

		public bool ReadArguments( T target )
		{
			int argumentPos = 0;

			foreach( KeyValuePair<string, ArgumentDetails> kvp in _allowedSwitches )
			{
				System.Reflection.FieldInfo fieldInfo = kvp.Value.FieldInfo;

				if( fieldInfo.FieldType.Equals( typeof( List<string> ) ) )
				{
					if( fieldInfo.GetValue( target ) == null )
					{
						fieldInfo.SetValue( target, new List<string>() );
					}
				}
			}

			while( argumentPos < _args.Length )
			{
				string name = _args[argumentPos];

				if( name == "/?" )
				{
					DisplayHelp( HelpString );

					return false;
				}

				if( name[0] == '/' )
				{
					name = name.Substring( 1 );

					if( _allowedSwitches.ContainsKey( name ) )
					{
						ArgumentDetails memberDetails = _allowedSwitches[name];

						if( memberDetails.Found )
						{
							HandleError( target, string.Format( "Argument '{0}' already defined.", name ) );
						}

						++argumentPos;

						if( memberDetails.NeedsArgument )
						{
							if( argumentPos >= _args.Length )
							{
								HandleError( target, string.Format( "Argument '{0}' is not followed by a value.", name ) );
							}

							if( memberDetails.FieldInfo.FieldType.Equals( typeof( List<string> ) ) )
							{
								List<string> list = (List<string>) memberDetails.FieldInfo.GetValue( target );

								list.Add( _args[argumentPos] );
							}
							else if( memberDetails.FieldInfo.FieldType.Equals( typeof( string ) ) )
							{
								memberDetails.FieldInfo.SetValue( target, _args[argumentPos] );
							}

							++argumentPos;
						}
						else
						{
							memberDetails.FieldInfo.SetValue( target, true );
						}

						memberDetails.Found = true;
					}
					else
					{
						++argumentPos;
						HandleUnknownArgument( target, name );
					}
				}
				else
				{
					++argumentPos;
					HandleUnknownArgument( target, name );
				}
			}

			foreach( KeyValuePair<string, ArgumentDetails> kvp in _allowedSwitches )
			{
				if( kvp.Value.Mandatory )
				{
					FieldInfo fieldInfo = kvp.Value.FieldInfo;

					if( fieldInfo.FieldType.Equals( typeof( List<string> ) ) )
					{
						List<string> list = (List<string>) fieldInfo.GetValue( target );

						if( list.Count == 0 )
						{
							HandleError( target, string.Format( "You must specify at least one '{0}' argument.", kvp.Key ) );
						}
					}
					else if( fieldInfo.FieldType.Equals( typeof( string ) ) )
					{
						if( fieldInfo.GetValue( target ) == null )
						{
							HandleError( target, string.Format( "You must specify the '{0}' argument.", kvp.Key ) );
						}
					}
				}
			}

			return true;
		}

		public string HelpString
		{
			get
			{
				string help = "Allowed arguments:" +Environment.NewLine + Environment.NewLine;

				foreach( KeyValuePair<string, ArgumentDetails> kvp in _allowedSwitches )
				{
					string defaultText = string.Empty;

					if( kvp.Value.DefaultValue != null )
					{
						defaultText = string.Format( " (default '{0}')", kvp.Value.DefaultValue );
					}

					help += string.Format( "/{0}\t{1}{2}{3}", kvp.Key, kvp.Value.Help, defaultText, Environment.NewLine );
				}

				return help;
			}
		}

		public virtual void DisplayHelp( string help )
		{
            System.Console.WriteLine(help);
		}

        public virtual void HandleError(T returnValue, string error)
        {
            throw new ArgumentException(string.Format("{0}{1}{1}{2}", error, Environment.NewLine, HelpString));
        }

        public virtual void HandleUnknownArgument(T returnValue, string value)
        {
            HandleError(returnValue, string.Format("Unknown argument '{0}'", value));
        }

        private TAttribute GetAttribute<TAttribute>(MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            TAttribute value = null;

            foreach (TAttribute attr in memberInfo.GetCustomAttributes(typeof(TAttribute), false))
            {
                if (value != null)
                {
                    throw new InvalidOperationException(string.Format("Attribute '{0}' found multiple times on type '{1}'.",
                        typeof(TAttribute).Name, typeof(T).Name));
                }

                value = attr;
            }

            return value;
        }

		private sealed class ArgumentDetails
		{
			internal FieldInfo FieldInfo;
			internal string Help = string.Empty;
			internal string DefaultValue = null;
			internal bool Mandatory = false;
			internal bool Found;
			internal bool NeedsArgument;
		}

		private string[] _args;
		private Dictionary<string, ArgumentDetails> _allowedSwitches;
	}
}
