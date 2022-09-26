/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	public partial class ImagePreviewControl : UserControl
	{
		public ImagePreviewControl()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw
				| ControlStyles.UserPaint
				, true );
		}

		public void SetImage( Image image )
		{
			_image = image;

			if( _locusEffect != null )
			{
				_locusEffect.Dispose();
				_locusEffect = null;
			}

			Invalidate();
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( _searchImage == null )
			{
				_searchImage = Image.FromStream( System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( "BinaryComponents.WinFormsGloss.Resources.Icons.Search24.png" ) );
			}

			if( _image != null )
			{
				e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

				Rectangle rect = ClientRectangle;

				if( _image.Height < _image.Width )
				{
					double ratio = (double) _image.Height / (double) _image.Width;
					int height = (int) (ClientRectangle.Height * ratio);

					rect = new Rectangle( 0, (ClientRectangle.Height - height) / 2, ClientRectangle.Width, height );
				}

				e.Graphics.DrawImage( _image, rect );
			}

			e.Graphics.DrawImage( _searchImage, new Rectangle( ClientRectangle.Width - 24, ClientRectangle.Height - 24, 24, 24 ) );
		}

		protected override void OnMouseHover( EventArgs e )
		{
			base.OnMouseHover( e );

			if( _image != null )
			{
				if( _locusEffect == null )
				{
					_locusEffect = new WinFormsUtility.Controls.LocusEffect( new ImageAnimation( _image ), _image.Size );
				}

				_locusEffect.Offset = new Point( ClientRectangle.Width / 2 - _image.Width / 2, ClientRectangle.Height / 2 - _image.Height / 2 );
				_locusEffect.ShowLocusEffect( this );
			}
		}

		#region class ImageAnimation

		private sealed class ImageAnimation : WinFormsUtility.Drawing.Animation
		{
			internal ImageAnimation( Image image )
			{
				_image = image;
			}

			public override void OnPaint( Graphics g, Rectangle drawingBounds, bool running, double seconds )
			{
				double prop = GetProportion( seconds );
				RectangleF rect;
				Point center = new Point( drawingBounds.X + drawingBounds.Width / 2, drawingBounds.Y + drawingBounds.Height / 2 );

				rect = new RectangleF
					( (float) (center.X - drawingBounds.Width * prop / 2)
					, (float) (center.Y - drawingBounds.Height * prop / 2)
					, (float) (drawingBounds.Width * prop)
					, (float) (drawingBounds.Height * prop) );

				g.DrawImage( _image, rect );
			}

			public override bool IsDone( double seconds )
			{
				return seconds > _showFor + _openSpeed * 2;
			}

			public override double GetSuggestedAlpha( double seconds )
			{
				return GetProportion( seconds );
			}

			private double GetProportion( double seconds )
			{
				double prop;

				if( seconds < _openSpeed )
				{
					prop = seconds / _openSpeed;
				}
				else if( seconds > _showFor + _openSpeed * 2 )
				{
					prop = 0;
				}
				else if( seconds > _showFor + _openSpeed )
				{
					prop = 1 - (seconds - _showFor - _openSpeed) / _openSpeed;
				}
				else
				{
					prop = 1;
				}

				return prop;
			}

			private const double _openSpeed = 0.5;
			private const double _showFor = 1.5;

			private Image _image;
		}

		#endregion

		private static Image _searchImage;

		private Image _image;
		private WinFormsUtility.Controls.LocusEffect _locusEffect;
	}
}
