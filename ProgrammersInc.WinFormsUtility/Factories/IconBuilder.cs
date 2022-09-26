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
using System.Drawing;

namespace ProgrammersInc.WinFormsUtility.Factories
{
	public abstract class IconBuilder : IDisposable
	{
		protected IconBuilder()
		{
		}

		public Icon GetIcon( params string[] keys )
		{
			if( keys == null )
			{
				throw new ArgumentNullException( "keys" );
			}
			if( keys.Length < 1 )
			{
				throw new ArgumentException( "Must supply at least one icon key.", "keys" );
			}

			string compositeKey = string.Join( ",", keys );

			if( !_icons.ContainsKey( compositeKey ) )
			{
				Build( keys );
			}

			return _icons[compositeKey];
		}

		public Image GetImage( params string[] keys )
		{
			if( keys == null )
			{
				throw new ArgumentNullException( "keys" );
			}
			if( keys.Length < 1 )
			{
				throw new ArgumentException( "Must supply at least one icon key.", "keys" );
			}

			string compositeKey = string.Join( ",", keys );

			if( !_images.ContainsKey( compositeKey ) )
			{
				Build( keys );
			}

			return _images[compositeKey];
		}

		private void Build( params string[] keys )
		{
			string compositeKey = string.Join( ",", keys );
			Icon icon = LoadIcon( keys[0] );

			Bitmap bitmap = new Bitmap( icon.Width, icon.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

			using( Graphics g = Graphics.FromImage( bitmap ) )
			{
				foreach( string key in keys )
				{
					icon = LoadIcon( key );

					g.DrawIcon( icon, 0, 0 );
				}
			}

			icon = Icon.FromHandle( bitmap.GetHicon() );

			_icons.Add( compositeKey, icon );
			_images.Add( compositeKey, bitmap );
		}

		#region IDisposable Members

		public void Dispose()
		{
			if( _icons != null )
			{
				foreach( Icon icon in _icons.Values )
				{
					Utility.Win32.User.DestroyIcon( icon.Handle );
				}
				_icons = null;
			}
		}

		#endregion

		protected abstract Icon LoadIcon( string key );

		private Dictionary<string, Icon> _icons = new Dictionary<string, Icon>();
		private Dictionary<string, Image> _images = new Dictionary<string, Image>();
	}
}
