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

namespace ProgrammersInc.WinFormsUtility.Factories
{
	public abstract class ControlFactory
	{
		protected ControlFactory( Utility.Assemblies.ManifestResources resources )
		{
			if( resources == null )
			{
				throw new ArgumentNullException( "resources" );
			}

			_resources = resources;
		}

		protected Utility.Assemblies.ManifestResources Resources
		{
			get
			{
				return _resources;
			}
		}

		private Utility.Assemblies.ManifestResources _resources;
	}
}
