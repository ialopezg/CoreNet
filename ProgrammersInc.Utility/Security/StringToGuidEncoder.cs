/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Security.Cryptography;
using System.Text;


namespace ProgrammersInc.Utility.Security
{
	/// <summary>
	/// Class for encoding strings.
	/// </summary>
	public class StringToGuidEncoder: IDisposable
	{
		public StringToGuidEncoder()
		{
		}

		/// <summary>
		/// Used for one off encoding.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static Guid OneOffEncode( string text )
		{
			using( MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider() )
			{
				return Encode( text, provider );
			}
		}

		/// <summary>
		/// Used for multiple encodings.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public Guid Encode( string text )
		{
			return Encode( text, _provider );
		}

		private static Guid Encode( string text, MD5CryptoServiceProvider provider )
		{
			byte[] byteRepresentation = UnicodeEncoding.UTF8.GetBytes( text.Trim() );
			byte[] hashedTextInBytes = hashedTextInBytes = provider.ComputeHash( byteRepresentation );
			return new Guid( hashedTextInBytes );
		}

		public void Dispose()
		{
			((IDisposable)_provider).Dispose();
		}

		private MD5CryptoServiceProvider _provider = new MD5CryptoServiceProvider();
	}
}
