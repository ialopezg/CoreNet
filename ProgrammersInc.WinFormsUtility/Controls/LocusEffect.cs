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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ProgrammersInc.WinFormsUtility.Controls
{
	[CLSCompliant( false )]
	public class LocusEffect : Win32.PerPixelAlphaForm
	{
		public LocusEffect( Drawing.Animation animation, Size size )
		{
			if( animation == null )
			{
				throw new ArgumentNullException( "animation" );
			}

			_animation = animation;

			Size = size;
		}

		public Point? Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;

				SetPosition( 0, 0 );
			}
		}

		public void ShowLocusEffect( Control owner )
		{
			if( owner == null )
			{
				throw new ArgumentNullException( "owner" );
			}

			_owner = owner;
			_ownerForm = owner.FindForm();

			if( _ownerForm == null )
			{
				return;
			}
			if( Visible )
			{
				return;
			}

			_owner.VisibleChanged += new EventHandler( _owner_VisibleChanged );
			_ownerForm.Activated += new EventHandler( _ownerForm_Activated );
			_ownerForm.Deactivate += new EventHandler( _ownerForm_Deactivate );

			SetPosition( 0, 0 );

			_start = DateTime.Now;

			if( _timer == null )
			{
				_timer = new Timer();
				_timer.Tick += new EventHandler( _timer_Tick );
				_timer.Enabled = true;
			}

			Draw();

			UpdateVisibility();
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			HideLocusEffect();
		}

		public void HideLocusEffect()
		{
			Hide();

			if( _timer != null )
			{
				_timer.Tick -= new EventHandler( _timer_Tick );
				_timer.Enabled = false;
				_timer.Dispose();
				_timer = null;
			}

			if( _ownerForm != null )
			{
				_ownerForm.Activated -= new EventHandler( _ownerForm_Activated );
				_ownerForm.Deactivate -= new EventHandler( _ownerForm_Deactivate );
				_ownerForm = null;
			}
			if( _owner != null )
			{
				_owner.VisibleChanged += new EventHandler( _owner_VisibleChanged );
				_owner = null;
			}
		}

		public void HintMoving( int xoff, int yoff )
		{
			SetPosition( xoff, yoff );
		}

		private bool ShouldBeVisible
		{
			get
			{
				return _ownerForm == Form.ActiveForm
					&& Form.ActiveForm != null
					&& _owner != null
					&& _owner.FindForm() == Form.ActiveForm
					&& _owner.Visible
					&& _owner.IsHandleCreated
					&& _owner.Size != Size.Empty;
			}
		}

		private void UpdateVisibility()
		{
			if( ShouldBeVisible )
			{
				if( !Visible )
				{
					Show( _ownerForm );
				}
			}
			else
			{
				if( Visible )
				{
					Hide();
				}
			}
		}

		private void _timer_Tick( object sender, EventArgs e )
		{
			SetPosition( 0, 0 );
			UpdateVisibility();

			if( ShouldBeVisible )
			{
				Draw();
			}

			double seconds = DateTime.Now.Subtract( _start ).TotalSeconds;

			if( _animation.IsDone( seconds ) )
			{
				HideLocusEffect();
			}
		}

		private void _ownerForm_Activated( object sender, EventArgs e )
		{
			UpdateVisibility();
		}

		private void _ownerForm_Deactivate( object sender, EventArgs e )
		{
			UpdateVisibility();
		}

		private void _owner_VisibleChanged( object sender, EventArgs e )
		{
			UpdateVisibility();
		}

		private void SetPosition( int xoff, int yoff )
		{
			if( _owner == null || _owner.IsDisposed )
			{
				return;
			}

			Point ownerScreen = _owner.PointToScreen( Point.Empty );

			if( _offset == null )
			{
				ownerScreen.Offset( _owner.Width / 2, _owner.Height / 2 );

				Location = new Point( ownerScreen.X - Width / 2, ownerScreen.Y - Height / 2 );
			}
			else
			{
				Utility.Win32.Common.RECT clientRect;

				Utility.Win32.User.GetWindowRect( _owner.Handle, out clientRect );

				if( ownerScreen != clientRect.Location )
				{
					Debug.Assert( true );
				}

				Location = new Point( ownerScreen.X + _offset.Value.X + xoff, ownerScreen.Y + _offset.Value.Y + yoff );
			}
		}

		private void Draw()
		{
			if( Width <= 0 || Height <= 0 || Width > 1000 || Height > 1000 )
			{
				return;
			}

			double seconds = DateTime.Now.Subtract( _start ).TotalSeconds;

			using( Bitmap bitmapArgb = new Bitmap( Width, Height, PixelFormat.Format32bppArgb ) )
			{
				using( Graphics g = Graphics.FromImage( bitmapArgb ) )
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					_animation.OnPaint( g, new Rectangle( 0, 0, Width, Height ), true, seconds );
				}

				double alpha = _animation.GetSuggestedAlpha( seconds );

				int a = Math.Max( 0, Math.Min( (int) (alpha * 255), 255 ) );

				SetBitmap( bitmapArgb, (byte) a );
			}
		}

		private Timer _timer;
		private Drawing.Animation _animation;
		private DateTime _start = DateTime.Now;
		private Control _owner;
		private Form _ownerForm;
		private Point? _offset;
	}
}
