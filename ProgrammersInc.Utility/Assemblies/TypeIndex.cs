using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.Utility.Assemblies
{
	public class TypeIndex
	{
		public TypeIndex()
		{
		}

		public void AddAssembly( System.Reflection.Assembly assembly )
		{
			Details details = new Details();

			details.Assembly = assembly;

			foreach( Type type in assembly.GetTypes() )
			{
				List<Type> types;

				if( details.TypeMap.ContainsKey( type.Name ) )
				{
					types = details.TypeMap[type.Name];
				}
				else
				{
					types = new List<Type>();
					details.TypeMap[type.Name] = types;
				}

				types.Add( type );
			}

			_details.Add( details );
		}

		public Type[] GetTypes( string name )
		{
			List<Type> types = new List<Type>();

			foreach( Details details in _details )
			{
				if( details.TypeMap.ContainsKey( name ) )
				{
					types.AddRange( details.TypeMap[name] );
				}
			}

			return types.ToArray();
		}

		public Type GetSingleType( string name )
		{
			Type[] types = GetTypes( name );

			if( types.Length == 0 )
			{
				throw new Exception( string.Format( "Type '{0}' not found in specified assemblies.", name ) );
			}
			else if( types.Length == 1 )
			{
				return types[0];
			}
			else
			{
				throw new Exception( string.Format( "Type '{0}' found multiple times in specified assemblies.", name ) );
			}
		}

		private class Details
		{
			internal System.Reflection.Assembly Assembly;
			internal Dictionary<string, List<Type>> TypeMap = new Dictionary<string,List<Type>>();
		}

		private List<Details> _details = new List<Details>();
	}
}
