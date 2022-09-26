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
	public static class PasswordHash
	{
		public static string HashText( string saltAsString, string textToHash )
		{
			byte[] byteRepresentation = UnicodeEncoding.UTF8.GetBytes( textToHash + saltAsString );
			byte[] hashedTextInBytes = null;
			MD5CryptoServiceProvider myMD5 = new MD5CryptoServiceProvider();
			hashedTextInBytes = myMD5.ComputeHash( byteRepresentation );
			return Convert.ToBase64String( hashedTextInBytes );
		}
		public static Guid HashTextToGuid( string saltAsString, string textToHash )
		{
			byte[] byteRepresentation = UnicodeEncoding.UTF8.GetBytes( textToHash + saltAsString );
			byte[] hashedTextInBytes = null;
			MD5CryptoServiceProvider myMD5 = new MD5CryptoServiceProvider();
			hashedTextInBytes = myMD5.ComputeHash( byteRepresentation );
			return new Guid( hashedTextInBytes );
		}
	}
}
