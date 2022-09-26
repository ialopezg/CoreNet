/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsUtility.Factories
{
	public sealed class ImageListBuilder
	{
		public ImageListBuilder( Utility.Assemblies.ManifestResources resources )
		{
			if( resources == null )
			{
				throw new ArgumentNullException( "resources" );
			}

			_resources = resources;
			_imageList = new ImageList();

			_imageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		public ImageList ImageList
		{
			get
			{
				return _imageList;
			}
		}

		public int GetImageIndex( string resource )
		{
			if( resource == null )
			{
				throw new ArgumentNullException( "resource" );
			}

			int index;

			if( !_indexMap.TryGetValue( resource, out index ) )
			{
				Icon icon = _resources.GetIcon( resource );

				index = _imageList.Images.Count;
				_imageList.Images.Add( icon );

				_indexMap[resource] = index;
				_iconMap[index] = icon;
			}

			return index;
		}

		public Icon GetIcon( int index )
		{
			return _iconMap[index];
		}

		public int GetImageIndex( IconBuilder builder, params string[] keys )
		{
			if( builder == null )
			{
				throw new ArgumentNullException( "builder" );
			}
			if( keys == null )
			{
				throw new ArgumentNullException( "keys" );
			}

			string compositeKey = string.Join( ",", keys );

			int index;

			if( !_indexMap.TryGetValue( compositeKey, out index ) )
			{
				Image image = builder.GetImage( keys );
				Icon icon = builder.GetIcon( keys );

				index = _imageList.Images.Count;
				_imageList.Images.Add( image );

				_indexMap[compositeKey] = index;
				_iconMap[index] = icon;
				_imageMap[index] = image;
			}

			return index;
		}

		public void UpdateIconList( List<Icon> icons )
		{
			if( icons.Count < _iconMap.Count )
			{
				for( int i = icons.Count; i < _iconMap.Count; ++i )
				{
					icons.Add( _iconMap[i] );
				}
			}
		}

		private Utility.Assemblies.ManifestResources _resources;
		private ImageList _imageList;
		private Dictionary<string, int> _indexMap = new Dictionary<string, int>();
		private Dictionary<int, Icon> _iconMap = new Dictionary<int, Icon>();
		private Dictionary<int, Image> _imageMap = new Dictionary<int, Image>();
	}
}
