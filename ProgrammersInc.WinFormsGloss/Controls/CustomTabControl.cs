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
using System.Drawing.Drawing2D;

namespace ProgrammersInc.WinFormsGloss.Controls
{
	public partial class CustomTabControl : UserControl
	{
		#region TabPage

		public class TabPage : UserControl
		{
			public bool CanClose
			{
				get
				{
					return _canClose;
				}
				set
				{
					if( _canClose == value )
					{
						return;
					}

					_canClose = value;

					if( _customTabControl != null )
					{
						_customTabControl.Invalidate();
					}
				}
			}

			internal CustomTabControl CustomTabControl
			{
				get
				{
					return _customTabControl;
				}
				set
				{
					_customTabControl = value;
				}
			}

			protected override void OnTextChanged( EventArgs e )
			{
				base.OnTextChanged( e );

				if( _customTabControl != null )
				{
					_customTabControl.Invalidate();
				}
			}

			protected internal virtual void OnRemoved( EventArgs e )
			{
				if( Removed != null )
				{
					Removed( this, e );
				}
			}

			protected internal virtual void OnSelected( EventArgs e )
			{
			}

			public event EventHandler Removed;

			private CustomTabControl _customTabControl;
			private bool _canClose = false;
		}

		#endregion
		#region TabPageCollection

		public sealed class TabPageCollection : IEnumerable<TabPage>
		{
			public delegate void TabRemovedHandler( object sender, TabRemovedEventArgs eventArgs );

			public class TabRemovedEventArgs : TabPageEventArgs
			{
				public TabRemovedEventArgs( TabPage tabRemoved, TabPage newSelectedTabPage )
					: base( tabRemoved )
				{
					this.NewSelectedTabPage = newSelectedTabPage;
				}

				public TabPage NewSelectedTabPage;
			}

			internal TabPageCollection( CustomTabControl customTabControl )
			{
				_customTabControl = customTabControl;
			}

			public int Count
			{
				get
				{
					return _tabPages.Count;
				}
			}

			public TabPage[] ToArray()
			{
				return _tabPages.ToArray();
			}

			public TabPage this[int index]
			{
				get
				{
					if( index < 0 || index >= _tabPages.Count )
					{
						throw new IndexOutOfRangeException();
					}

					return _tabPages[index];
				}
			}

			public void Add( TabPage tabPage )
			{
				tabPage.CustomTabControl = _customTabControl;

				_tabPages.Add( tabPage );

				if( _tabPages.Count == 1 )
				{
					_customTabControl.SelectedIndex = 0;
				}

				_customTabControl.SizeContentsPanel();
				_customTabControl.Invalidate();
			}

			public void Insert( int index, TabPage tabPage )
			{
				tabPage.CustomTabControl = _customTabControl;

				_tabPages.Insert( index, tabPage );

				if( index < _tabPages.Count )
				{
					++_customTabControl.SelectedIndex;
				}
				else if( _tabPages.Count == 1 )
				{
					_customTabControl.SelectedIndex = 0;
				}

				_customTabControl.SizeContentsPanel();
				_customTabControl.Invalidate();
			}

			public void Move( TabPage tabPage, int newIndex )
			{
				int index = _tabPages.IndexOf( tabPage );

				if( index < 0 )
				{
					throw new InvalidOperationException();
				}

				_tabPages.Insert( newIndex, tabPage );

				if( newIndex > index )
				{
					_tabPages.RemoveAt( index );
				}
				else
				{
					_tabPages.RemoveAt( index + 1 );
				}

				_customTabControl.SelectedIndex = -1;
				_customTabControl.SelectedIndex = newIndex;

				_customTabControl.SizeContentsPanel();
				_customTabControl.Invalidate();
			}

			public void Remove( TabPage tabPage )
			{
				if( _tabPages.IndexOf( tabPage ) == -1 )
				{
					throw new InvalidOperationException();
				}

				_tabPages.Remove( tabPage );
				_customTabControl.SelectedIndex = -1;

				tabPage.OnRemoved( EventArgs.Empty );

				TabPage newSelection = null;

				if( _customTabControl != null )
				{
					if( _customTabControl.TabPages.Count > 0 )
					{
						if( _customTabControl.SelectedIndex >= _customTabControl.TabPages.Count )
						{
							newSelection = _customTabControl.TabPages[_customTabControl.TabPages.Count - 1];
						}
						else
						{
							if( _customTabControl.SelectedIndex != -1 )
							{
								newSelection = _customTabControl.TabPages[_customTabControl.SelectedIndex];
							}
						}
					}
				}

				TabRemovedEventArgs removedEvent = new TabRemovedEventArgs( tabPage, newSelection );

				OnTabPageRemoved( removedEvent );

				if( removedEvent.NewSelectedTabPage != null )
				{
					int i = _customTabControl.TabPages.IndexOf( removedEvent.NewSelectedTabPage );

					_customTabControl.SelectedIndex = -1;
					_customTabControl.SelectedIndex = i;
				}

				_customTabControl.SizeContentsPanel();
				_customTabControl.Invalidate();
			}

			public int IndexOf( TabPage tabPage )
			{
				return _tabPages.IndexOf( tabPage );
			}

			private void OnTabPageRemoved( TabRemovedEventArgs eventArgs )
			{
				if( TabPageRemoved != null )
				{
					TabPageRemoved( this, eventArgs );
				}
			}

			#region IEnumerable<TabPage> Members

			IEnumerator<TabPage> IEnumerable<TabPage>.GetEnumerator()
			{
				return _tabPages.GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _tabPages.GetEnumerator();
			}

			#endregion

			public event TabRemovedHandler TabPageRemoved;

			private CustomTabControl _customTabControl;
			private List<TabPage> _tabPages = new List<TabPage>();
		}

		#endregion
		#region TabPageEventArgs

		public class TabPageEventArgs : EventArgs
		{
			public TabPageEventArgs( TabPage tabPage )
			{
				_tabPage = tabPage;
			}

			public TabPage TabPage
			{
				get
				{
					return _tabPage;
				}
			}

			private TabPage _tabPage;
		}

		public delegate void TabPageEventHandler( object sender, TabPageEventArgs e );

		#endregion

		public CustomTabControl()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.ResizeRedraw
				, true );

			_contentsPanel.BackColor = SystemColors.Window;

			_tabPageCollection = new TabPageCollection( this );

			SizeContentsPanel();

			_updateTimer.Start();
		}

		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				if( _selectedIndex == value )
				{
					return;
				}
				if( value >= TabPages.Count )
				{
					throw new ArgumentException();
				}

				_contentsPanel.SuspendLayout();

				List<Control> toRemove = new List<Control>();

				foreach( Control child in _contentsPanel.Controls )
				{
					toRemove.Add( child );
				}

				_selectedIndex = value;

				Control content = null;

				if( _selectedIndex >= 0 && _selectedIndex < TabPages.Count )
				{
					content = TabPages[_selectedIndex];
				}

				if( content != null )
				{
					content.Bounds = _contentsPanel.ClientRectangle;
					content.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
					content.Bounds = _contentsPanel.ClientRectangle;

					_contentsPanel.Controls.Add( content );

					foreach( Control child in _contentsPanel.Controls )
					{
						RecurseSetVisible( child, true );
					}
				}

				foreach( Control child in toRemove )
				{
					RecurseSetVisible( child, false );
					_contentsPanel.Controls.Remove( child );
				}

				_contentsPanel.ResumeLayout();

				Invalidate();

				OnSelectedIndexChanged( EventArgs.Empty );
			}
		}

		public TabPage SelectedTab
		{
			get
			{
				if( SelectedIndex < 0 )
				{
					return null;
				}

				return TabPages[SelectedIndex];
			}
			set
			{
				if( value == null )
				{
					return;
				}

				int index = TabPages.IndexOf( value );

				if( index < 0 || index >= TabPages.Count )
				{
					throw new ArgumentException();
				}

				SelectedIndex = index;
			}
		}

		public TabPageCollection TabPages
		{
			get
			{
				return _tabPageCollection;
			}
		}

		public string CloseButtonToolTip
		{
			get
			{
				return _closeButtonToolTip;
			}
			set
			{
				_closeButtonToolTip = value;
			}
		}

		public Drawing.ColorTable ColorTable
		{
			get
			{
				return _colorTable;
			}
			set
			{
				_colorTable = value;
				Invalidate();
			}
		}

		protected virtual void OnSelectedIndexChanged( EventArgs e )
		{
			if( SelectedTab != null )
			{
				SelectedTab.OnSelected( EventArgs.Empty );
			}

			if( SelectedIndexChanged != null )
			{
				SelectedIndexChanged( this, e );
			}
		}

		protected virtual void OnCloseButtonClick( TabPageEventArgs e )
		{
			if( CloseButtonClick != null )
			{
				CloseButtonClick( this, e );
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			GetTabBounds( e.Graphics );

			if( TabPages.Count > 1 )
			{
				for( int i = TabPages.Count - 1; i >= 0; --i )
				{
					if( i != SelectedIndex )
					{
						using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( e.Graphics ) )
						{
							DrawTabPage( e.Graphics, i );
						}
					}
				}
			}

			Color borderColor = WinFormsUtility.Drawing.ColorUtil.Combine( _colorTable.PrimaryColor, Color.White, 0.7 );

			e.Graphics.SmoothingMode = SmoothingMode.None;

			if( TabPages.Count > 1 )
			{
				using( Pen pen = new Pen( borderColor ) )
				{
					e.Graphics.DrawRectangle( pen, 0, _tabsHeight - 1, Width - 1, Height - _tabsHeight );
				}

				if( SelectedIndex >= 0 && SelectedIndex < TabPages.Count )
				{
					using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( e.Graphics ) )
					{
						DrawTabPage( e.Graphics, SelectedIndex );
					}
				}
			}
			else
			{
				using( Pen pen = new Pen( borderColor ) )
				{
					e.Graphics.DrawRectangle( pen, 0, 1, Width - 1, Height - 2 );
				}
			}

			if( TabPages.Count > 1 )
			{
				DrawCloseBox( e.Graphics );
			}
		}

		protected override void OnMouseClick( MouseEventArgs e )
		{
			base.OnMouseClick( e );

			if( e.Button == MouseButtons.Left )
			{
				int overTabCloseButton = GetOverTabCloseButton( e.Location );

				if( overTabCloseButton >= 0 )
				{
					OnCloseButtonClick( new TabPageEventArgs( _tabPageCollection[overTabCloseButton] ) );
					return;
				}

				for( int i = 0; i < _tabBounds.Length && i < TabPages.Count; ++i )
				{
					if( _tabBounds[i].Contains( e.Location ) )
					{
						SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void SizeContentsPanel()
		{
			if( TabPages.Count <= 1 )
			{
				_contentsPanel.Bounds = new Rectangle( _contentMargin, _contentMargin + 1,
					Width - _contentMargin * 2, Height - _contentMargin * 2 - 1 );
			}
			else
			{
				_contentsPanel.Bounds = new Rectangle( _contentMargin, _tabsHeight + _contentMargin - 1,
					Width - _contentMargin * 2, Height - _tabsHeight - _contentMargin * 2 + 1 );
			}
		}

		private void RecurseSetVisible( Control c, bool visible )
		{
			if( visible )
			{
				c.Visible = true;
			}

			foreach( Control child in c.Controls )
			{
				RecurseSetVisible( child, visible );
			}

			if( !visible )
			{
				c.Visible = false;
			}
		}

		private void DrawCloseBox( Graphics g )
		{
			g.SmoothingMode = SmoothingMode.HighQuality;

			Point mousePosition = PointToClient( Control.MousePosition );
			int overTabPage = GetOverTabPage( mousePosition );

			if( overTabPage < 0 || !_tabPageCollection[overTabPage].CanClose )
			{
				return;
			}

			_tabCloseUpdates.DoneUpdate( overTabPage );

			Rectangle bounds = GetCloseButtonBounds( _tabPageCollection[overTabPage] );

			g.SmoothingMode = SmoothingMode.AntiAlias;

			if( _tabCloseHistory.MouseOver >= 0 )
			{
				using( Brush brush = new SolidBrush( _colorTable.GlowHighlightColor ) )
				{
					WinFormsUtility.Drawing.GdiPlusEx.FillRoundRect( g, brush, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1, 2 );
				}

				using( Pen outlinePen = new Pen( SystemColors.ControlText, -1f ) )
				{
					WinFormsUtility.Drawing.GdiPlusEx.DrawRoundRect( g, outlinePen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1, 2 );
				}

				WinFormsUtility.Drawing.GdiPlusEx.DrawRoundRect( g, SystemPens.Window, bounds.X - 1, bounds.Y - 1, bounds.Width + 1, bounds.Height + 1, 3 );
			}

			Pen crossPen;

			if( _tabCloseHistory.MouseOver >= 0 )
			{
				crossPen = new Pen( SystemColors.ControlText, 2f );
			}
			else
			{
				crossPen = new Pen( WinFormsUtility.Drawing.ColorUtil.Combine( SystemColors.ControlDarkDark, Color.Black, 0.5 ), 2f );
			}

			using( crossPen )
			{
				g.DrawLine( crossPen, bounds.X + 2, bounds.Y + 2, bounds.Right - 3, bounds.Bottom - 3 );
				g.DrawLine( crossPen, bounds.X + 2, bounds.Bottom - 3, bounds.Right - 3, bounds.Y + 2 );
			}
		}

		private void DrawTabPage( Graphics g, int i )
		{
			_tabPageUpdates.DoneUpdate( i );

			double tabPageGlow = GetTabPageFade( i, _fadeIn, _fadeOut );
			double tabCloseGlow = GetTabCloseFade( i, _fadeIn, _fadeOut );
			TabPage tabPage = TabPages[i];
			Rectangle bounds = _tabBounds[i];
			Rectangle boundsTop = new Rectangle( bounds.X, bounds.Y, bounds.Width + 8, bounds.Height * 2 / 5 );
			Rectangle boundsBottom = new Rectangle( bounds.X, bounds.Y + bounds.Height * 2 / 5, bounds.Width + 8, bounds.Height - bounds.Height * 2 / 5 );

			int closeWidth = 0;

			if( tabPage.CanClose )
			{
				closeWidth = 17;
			}

			Rectangle textBounds = new Rectangle( bounds.X + _tabSlope + _curveRadius, bounds.Y + 4,
				bounds.Width - _tabSlope - _curveRadius - closeWidth, bounds.Height - 1 );

			using( GraphicsPath gp = new GraphicsPath() )
			{
				gp.AddLine( bounds.X - 1, bounds.Bottom + 1, bounds.X + _tabSlope - _curveRadius, bounds.Y + _curveRadius );
				gp.AddCurve( new Point[]
						{
							new Point( bounds.X + _tabSlope - _curveRadius, bounds.Y + _curveRadius ),
							new Point( bounds.X + _tabSlope, bounds.Y + _curveRadiusSemi ),
							new Point( bounds.X + _tabSlope + _curveRadius, bounds.Y )
						} );
				gp.AddLine( bounds.X + _tabSlope + _curveRadius, bounds.Y, bounds.Right - _curveRadius + _overlap - _tabSeparator, bounds.Y );
				gp.AddCurve( new Point[]
						{
							new Point( bounds.Right - _curveRadius + _overlap - _tabSeparatorSemi, bounds.Y ),
							new Point( bounds.Right - _curveRadiusSemi + _overlap - _tabSeparatorSemi, bounds.Y + _curveRadiusSemi ),
							new Point( bounds.Right + _overlap - _tabSeparatorSemi, bounds.Y + _curveRadius )
						} );
				gp.AddLine( bounds.Right + _overlap - _tabSeparatorSemi, bounds.Y + _curveRadius, bounds.Right + _overlap - _tabSeparatorSemi, bounds.Bottom + 1 );

				g.SmoothingMode = SmoothingMode.HighQuality;

				Color primaryColor = WinFormsUtility.Drawing.ColorUtil.Combine( _colorTable.PrimaryColor, Color.White, 0.3 );
				Color borderColor = WinFormsUtility.Drawing.ColorUtil.Combine( _colorTable.PrimaryColor, Color.White, 0.7 );

				Color gradientTopBegin = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.4 );
				Color gradientTopEnd = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.7 );
				Color gradientBottomBegin = primaryColor;
				Color gradientBottomEnd = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.2 );
				Color glowStartColor = _colorTable.GlowColor;
				Color glowEndColor = WinFormsUtility.Drawing.ColorUtil.Combine( _colorTable.GlowHighlightColor, Color.White, 0.3 );

				Color glowGradientTopBegin = WinFormsUtility.Drawing.ColorUtil.Combine( glowStartColor, Color.White, 0.4 );
				Color glowGradientTopEnd = WinFormsUtility.Drawing.ColorUtil.Combine( glowStartColor, Color.White, 0.7 );
				Color glowGradientBottomBegin = glowStartColor;
				Color glowGradientBottomEnd = gradientBottomEnd;

				if( i == SelectedIndex )
				{
					primaryColor = WinFormsUtility.Drawing.ColorUtil.Combine( _colorTable.PrimaryColor, Color.White, 0.15 );
					gradientTopBegin = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.4 );
					gradientTopEnd = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.7 );
					gradientBottomBegin = primaryColor;
					gradientBottomEnd = WinFormsUtility.Drawing.ColorUtil.Combine( primaryColor, Color.White, 0.2 );
				}

				gradientTopBegin = WinFormsUtility.Drawing.ColorUtil.Combine( gradientTopBegin, glowGradientTopBegin, 1 - tabPageGlow );
				gradientTopEnd = WinFormsUtility.Drawing.ColorUtil.Combine( gradientTopEnd, glowGradientTopEnd, 1 - tabPageGlow );
				gradientBottomBegin = WinFormsUtility.Drawing.ColorUtil.Combine( gradientBottomBegin, glowGradientBottomBegin, 1 - tabPageGlow );
				gradientBottomEnd = WinFormsUtility.Drawing.ColorUtil.Combine( gradientBottomEnd, glowGradientBottomEnd, 1 - tabPageGlow );

				Region clip = new Region( TabClipBounds );

				Rectangle lbt = new Rectangle( boundsTop.Left, boundsTop.Top - 1, boundsTop.Width, boundsTop.Height + 2 );

				if( i == SelectedIndex )
				{
					using( WinFormsUtility.Drawing.GdiPlusEx.SaveState( g ) )
					{
						g.TranslateTransform( _shadow + 1, _shadow + 1 );

						using( Brush b = new SolidBrush( Color.FromArgb( 100 / _shadow, 0, 0, 0 ) ) )
						{
							for( int j = 0; j < _shadow; ++j )
							{
								g.TranslateTransform( -1, -1 );

								g.FillPath( b, gp );
							}
						}
					}
				}

				clip.Intersect( boundsTop );

				g.Clip = clip;

				using( Brush b = new LinearGradientBrush( lbt, gradientTopBegin, gradientTopEnd, LinearGradientMode.Vertical ) )
				{
					g.FillPath( b, gp );
				}

				clip = new Region( TabClipBounds );

				clip.Intersect( boundsBottom );

				g.Clip = clip;

				Rectangle lbb = new Rectangle( boundsBottom.Left, boundsBottom.Top - 1, boundsBottom.Width, boundsBottom.Height + 2 );

				using( Brush b = new LinearGradientBrush( lbb, gradientBottomBegin, gradientBottomEnd, LinearGradientMode.Vertical ) )
				{
					g.FillPath( b, gp );
				}

				g.Clip = new Region( TabClipBounds );

				using( Font font = new Font( SystemFonts.DialogFont, i == SelectedIndex ? FontStyle.Bold : FontStyle.Regular ) )
				{
					Color textColor = SystemColors.ControlText;

					WinFormsUtility.Drawing.GdiPlusEx.DrawString
						( g, tabPage.Text, font, textColor, textBounds, WinFormsUtility.Drawing.GdiPlusEx.TextSplitting.SingleLineEllipsis, WinFormsUtility.Drawing.GdiPlusEx.Ampersands.Display );

					using( Pen pen = new Pen( borderColor ) )
					{
						g.DrawPath( pen, gp );
					}
				}

				if( tabPage.CanClose )
				{
					Point mousePosition = PointToClient( Control.MousePosition );
					int overTabPage = GetOverTabPage( mousePosition );

					if( overTabPage != i )
					{
						Rectangle closeBounds = GetCloseButtonBounds( tabPage );

						using( Pen crossPen = new Pen( SystemColors.ControlDark, 2f ) )
						{
							g.DrawLine( crossPen, closeBounds.X + 2, closeBounds.Y + 2, closeBounds.Right - 3, closeBounds.Bottom - 3 );
							g.DrawLine( crossPen, closeBounds.X + 2, closeBounds.Bottom - 3, closeBounds.Right - 3, closeBounds.Y + 2 );
						}
					}

					g.ResetClip();
				}
			}
		}

		private double GetTabPageFade( int tab, double fadeIn, double fadeOut )
		{
			double fadeInGlow = 0, fadeOutGlow = 0;

			if( _tabPageHistory.MouseOver == tab )
			{
				double fadeInTime = _tabPageHistory.GetTimeOver( tab ).Value;

				if( fadeInTime < fadeIn )
				{
					_tabPageUpdates.NeedsUpdate( tab );
					fadeInGlow = fadeInTime / fadeIn;
				}
				else
				{
					fadeInGlow = 1;
				}
			}

			double fadeOutTime = _tabPageHistory.GetLastOver( tab );

			if( fadeOutTime < fadeOut )
			{
				_tabPageUpdates.NeedsUpdate( tab );
				fadeOutGlow = 1 - fadeOutTime / fadeOut;
			}

			return Math.Max( fadeInGlow, fadeOutGlow );
		}

		private double GetTabCloseFade( int tab, double fadeIn, double fadeOut )
		{
			double fadeInGlow = 0, fadeOutGlow = 0;

			if( _tabCloseHistory.MouseOver == tab )
			{
				double fadeInTime = _tabCloseHistory.GetTimeOver( tab ).Value;

				if( fadeInTime < fadeIn )
				{
					_tabCloseUpdates.NeedsUpdate( tab );
					fadeInGlow = fadeInTime / fadeIn;
				}
				else
				{
					fadeInGlow = 1;
				}
			}

			double fadeOutTime = _tabCloseHistory.GetLastOver( tab );

			if( fadeOutTime < fadeOut )
			{
				_tabCloseUpdates.NeedsUpdate( tab );
				fadeOutGlow = 1 - fadeOutTime / fadeOut;
			}

			return Math.Max( fadeInGlow, fadeOutGlow );
		}

		private void GetTabBounds( Graphics g )
		{
			int xpos = 0;

			_tabBounds = new Rectangle[TabPages.Count];

			using( Font font = new Font( SystemFonts.DialogFont, FontStyle.Bold ) )
			{
				for( int i = 0; i < TabPages.Count; ++i )
				{
					TabPage tabPage = TabPages[i];
					int desiredWidth = (int) g.MeasureString( tabPage.Text, font ).Width;

					if( tabPage.CanClose )
					{
						desiredWidth += 20;
					}

					int width = Math.Min( MaxTabWidth, desiredWidth ) + _tabSeparator + _tabSlope;

					_tabBounds[i] = new Rectangle( xpos, 0, width, _tabsHeight );

					xpos += width;
				}
			}
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			UpdateAndInvalidate();

			Point mousePos = Control.MousePosition;
			bool showToolTip = false;

			if( mousePos == _lastMousePosition )
			{
				if( DateTime.Now.Subtract( _lastMouseWhen ).TotalSeconds > 1 )
				{
					showToolTip = true;
				}
			}
			else
			{
				_lastMousePosition = mousePos;
				_lastMouseWhen = DateTime.Now;
			}

			if( showToolTip && _tabCloseHistory.MouseOver != -1 )
			{
				Rectangle closeButtonRect = GetCloseButtonBounds( _tabPageCollection[_tabCloseHistory.MouseOver] );

				SuperToolTipInfo tooltipInfo = new SuperToolTipInfo( "Close", _closeButtonToolTip );
				Point p = new Point( closeButtonRect.Right, closeButtonRect.Bottom + 16 );

				p = PointToScreen( p );

				SuperToolTipManager.ShowToolTip( new Drawing.WindowBackgroundColorTable( ColorTable ), tooltipInfo, this, p );
			}
		}

		private void UpdateAndInvalidate()
		{
			Point mousePosition = PointToClient( Control.MousePosition );
			int overTabCloseButton = GetOverTabCloseButton( mousePosition );
			int overTabPage = GetOverTabPage( mousePosition );

			if( overTabPage != _tabPageHistory.MouseOver )
			{
				if( overTabPage != -1 )
				{
					_tabPageUpdates.NeedsUpdate( overTabPage );
				}
				if( _tabPageHistory.MouseOver != -1 )
				{
					_tabPageUpdates.NeedsUpdate( _tabPageHistory.MouseOver );
				}
			}
			if( overTabCloseButton != _tabCloseHistory.MouseOver )
			{
				if( overTabCloseButton != -1 )
				{
					_tabCloseUpdates.NeedsUpdate( overTabCloseButton );
				}
				if( _tabCloseHistory.MouseOver != -1 )
				{
					_tabCloseUpdates.NeedsUpdate( _tabCloseHistory.MouseOver );
				}
			}

			_tabPageHistory.Update( overTabPage );
			_tabCloseHistory.Update( overTabCloseButton );

			RunUpdates();
		}

		private void RunUpdates()
		{
			foreach( int i in _tabPageUpdates.Items )
			{
				if( i < 0 | i >= _tabBounds.Length )
				{
					continue;
				}

				Rectangle rect = _tabBounds[i];

				rect.Inflate( 4, 4 );

				Invalidate( rect );
			}
			foreach( int i in _tabCloseUpdates.Items )
			{
				if( i < 0 | i >= _tabBounds.Length )
				{
					continue;
				}

				Rectangle rect = _tabBounds[i];

				rect.Inflate( 4, 4 );

				Invalidate( rect );
			}
		}

		private int GetOverTabPage( Point mousePosition )
		{
			if( FindForm() != Form.ActiveForm )
			{
				return -1;
			}
			if( _tabBounds == null )
			{
				return -1;
			}

			int overTabPage = -1;

			for( int i = 0; i < _tabPageCollection.Count; ++i )
			{
				if( i >= 0 && i < _tabBounds.Length )
				{
					Rectangle r = _tabBounds[i];

					if( r.Contains( mousePosition ) )
					{
						overTabPage = i;
						break;
					}
				}
			}

			return overTabPage;
		}

		private int GetOverTabCloseButton( Point mousePosition )
		{
			if( FindForm() != Form.ActiveForm )
			{
				return -1;
			}

			int overTabCloseButton = -1;

			for( int i = 0; i < _tabPageCollection.Count; ++i )
			{
				Rectangle r = GetCloseButtonBounds( _tabPageCollection[i] );

				if( r.Contains( mousePosition ) )
				{
					overTabCloseButton = i;
					break;
				}
			}

			return overTabCloseButton;
		}

		private Rectangle GetCloseButtonBounds( TabPage tabPage )
		{
			if( tabPage.CanClose && _tabBounds != null )
			{
				int index = _tabPageCollection.IndexOf( tabPage );

				if( index >= 0 && index < _tabBounds.Length )
				{
					Rectangle tr = _tabBounds[index];

					return new Rectangle( tr.Right - 15, tr.Top + 3, 15, 15 );
				}
			}

			return Rectangle.Empty;
		}

		private Rectangle TabClipBounds
		{
			get
			{
				return new Rectangle( 0, 0, Width, _tabsHeight );
			}
		}

		private int MaxTabWidth
		{
			get
			{
				int available = Width - 8;
				int count = TabPages.Count;

				if( count == 0 )
				{
					count = 1;
				}

				available -= (_tabSeparator + _tabSlope) * count;

				if( available < 20 )
				{
					available = 20;
				}

				return available / count;
			}
		}

		public event EventHandler SelectedIndexChanged;
		public event TabPageEventHandler CloseButtonClick;

		private const int _tabSeparator = 10;
		private const int _tabSeparatorSemi = 4;
		private const int _tabSlope = 10;
		private const int _curveRadius = 3;
		private const int _curveRadiusSemi = 1;
		private const int _overlap = 6;
		private const int _contentMargin = 1;
		private const int _shadow = 3;
		private const double _fadeOut = 0.3;
		private const double _fadeIn = 0.1;

		private TabPageCollection _tabPageCollection;
		private int _selectedIndex = -1;
		private int _tabsHeight = SystemFonts.DialogFont.Height + 8;
		private Rectangle[] _tabBounds;
		private Drawing.GlowHistory<int> _tabPageHistory = new Drawing.GlowHistory<int>( -1 );
		private Drawing.GlowHistory<int> _tabCloseHistory = new Drawing.GlowHistory<int>( -1 );
		private Drawing.GlowUpdates<int> _tabPageUpdates = new Drawing.GlowUpdates<int>();
		private Drawing.GlowUpdates<int> _tabCloseUpdates = new Drawing.GlowUpdates<int>();
		private Point _lastMousePosition;
		private DateTime _lastMouseWhen;
		private string _closeButtonToolTip = "Close Tab";
		private Drawing.ColorTable _colorTable = new Drawing.WindowsThemeColorTable();
	}
}
