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

namespace ProgrammersInc.Utility.File
{
	public sealed class History
	{
		public History( int numVersions )
		{
			if( numVersions <= 0 )
			{
				throw new ArgumentException( "Number of versions must be greater than zero.", "numVersions" );
			}

			_numVersions = numVersions;
		}

		public void AccessFile( string filename )
		{
			for( int n = _numVersions; n > 1; --n )
			{
				string older = GetFilename( filename, n );
				string newer = GetFilename( filename, n - 1 );

				RenameFile( newer, older );
			}

			CopyFile( filename, GetFilename( filename, 1 ) );
		}

		private string GetFilename( string baseName, int n )
		{
			if( n == 0 )
			{
				return baseName;
			}
			else
			{
				return string.Format( "{0}.{1:000}", baseName, n );
			}
		}

		private void DeleteFile( string filename )
		{
			if( System.IO.File.Exists( filename ) )
			{
				try
				{
					System.IO.File.Delete( filename );
				}
				catch
				{
				}
			}
		}

		private void RenameFile( string oldName, string newName )
		{
			DeleteFile( newName );

			if( System.IO.File.Exists( oldName ) )
			{
				System.IO.File.Move( oldName, newName );
			}
		}

		private void CopyFile( string from, string to )
		{
			DeleteFile( to );

			if( System.IO.File.Exists( from ) )
			{
				try
				{
					System.IO.File.Copy( from, to );
				}
				catch
				{
				}
			}
		}

		private int _numVersions;
	}
}
