using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ProgrammersInc.Utility.Security
{
	public class StringEncode
	{
		public StringEncode( string salt )
		{
			_salt = salt;
		}

		public string EncryptString( string value )
		{
			if( string.IsNullOrEmpty( value ) )
			{
				return string.Empty;
			}

			TripleDESCryptoServiceProvider crypto = GetProvider();
			byte[] valueBytes = Encoding.Unicode.GetBytes( value );
			byte[] valueEncrypted = crypto.CreateEncryptor().TransformFinalBlock( valueBytes, 0, valueBytes.Length );

			return Convert.ToBase64String( valueEncrypted );
		}

		public string DecryptString( string encryptedString )
		{
			if( string.IsNullOrEmpty( encryptedString ) )
			{
				return string.Empty;
			}

			TripleDESCryptoServiceProvider crypto = GetProvider();
			byte[] encryptedBytes = Convert.FromBase64String( encryptedString );
			byte[] decryptedBytes = crypto.CreateDecryptor().TransformFinalBlock( encryptedBytes, 0, encryptedBytes.Length );

			return Encoding.Unicode.GetString( decryptedBytes );
		}

		private TripleDESCryptoServiceProvider GetProvider()
		{
			TripleDESCryptoServiceProvider crypto = new TripleDESCryptoServiceProvider();

			string salt = _salt;
			byte[] bytes = Encoding.Unicode.GetBytes( salt );

			MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();

			crypto.Key = csp.ComputeHash( bytes );
			crypto.Mode = CipherMode.ECB;

			return crypto;
		}

		private string _salt;
	}
}
