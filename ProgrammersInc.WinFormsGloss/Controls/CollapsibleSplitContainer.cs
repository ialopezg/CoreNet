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
	public partial class CollapsibleSplitContainer : UserControl
	{
		public enum Edge
		{
			Top,
			Bottom,
			Left,
			Right
		}

		#region Watch

		public class Watch : WinFormsUtility.ControlPreferences.Watch
		{
			public Watch( Controls.CollapsibleSplitContainer splitContainer, string id )
				: base( splitContainer, id )
			{
				_splitContainer = splitContainer;
			}

			protected override void Read()
			{
				string splitterDistance = ControlPreferences.GetValue( Name, "SplitterDistance" );

				if( splitterDistance != null )
				{
					try
					{
						_splitContainer.SplitterDistance = int.Parse( splitterDistance, WinFormsUtility.ControlPreferences.Culture );
					}
					catch
					{
					}
				}

				string panel1Collapsed = ControlPreferences.GetValue( Name, "Panel1Collapsed" );

				if( panel1Collapsed != null )
				{
					try
					{
						_splitContainer.Panel1Collapsed = bool.Parse( panel1Collapsed );
					}
					catch
					{
					}
				}
			}

			protected override void Write()
			{
				ControlPreferences.SetValue( Name, "SplitterDistance", _splitContainer.SplitterDistance.ToString( WinFormsUtility.ControlPreferences.Culture ) );
				ControlPreferences.SetValue( Name, "Panel1Collapsed", _splitContainer.Panel1Collapsed.ToString( WinFormsUtility.ControlPreferences.Culture ) );
			}

			protected override void OnVisibleSet()
			{
			}

			protected override void OnVisibleUnset()
			{
			}

			private Controls.CollapsibleSplitContainer _splitContainer;
		}

		#endregion

		public CollapsibleSplitContainer()
		{
			InitializeComponent();

			SetStyle
				( ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.ContainerControl
				| ControlStyles.ResizeRedraw
				, true );

			LayoutPanels();
		}

		public Edge CollapseSide
		{
			get
			{
				return _collapseSide;
			}
			set
			{
				if( value == _collapseSide )
				{
					return;
				}

				_collapseSide = value;

				LayoutPanels();
				_toggleBar.Invalidate();
			}
		}

		public Panel Panel1
		{
			get
			{
				return _panel1;
			}
		}

		public Panel Panel2
		{
			get
			{
				return _panel2;
			}
		}

		public int SplitterDistance
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;

				ConstrainWidth();
				LayoutPanels();
			}
		}

		public int Panel1MinSize
		{
			get
			{
				return _minWidth;
			}
			set
			{
				_minWidth = value;

				ConstrainWidth();
				LayoutPanels();
			}
		}

		public int Panel2MinSize
		{
			get
			{
				return _minExtra;
			}
			set
			{
				_minExtra = value;

				ConstrainWidth();
				LayoutPanels();
			}
		}

		public bool Panel1Collapsed
		{
			get
			{
				return _collapsed;
			}
			set
			{
				if( _collapsed == value )
				{
					return;
				}

				_collapsed = value;

				LayoutPanels();
				OnCollapsedExpanded( EventArgs.Empty );
				_toggleBar.Invalidate();
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
				UpdateBackColor();
				Invalidate();
			}
		}

		public Form OwnerForm
		{
			get
			{
				return _ownerForm;
			}
			set
			{
				DetachParentEvents();

				_ownerForm = value;

				AttachParentEvents();
			}
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			_toggleBar.Click += new System.EventHandler( _toggleBar_Click );
			_toggleBar.Paint += new System.Windows.Forms.PaintEventHandler( _toggleBar_Paint );
			_splitter.MouseDown += new System.Windows.Forms.MouseEventHandler( _splitter_MouseDown );
			_splitter.Paint += new System.Windows.Forms.PaintEventHandler( _splitter_Paint );
			_dragTimer.Tick += new System.EventHandler( _dragTimer_Tick );
			_updateTimer.Tick += new System.EventHandler( _updateTimer_Tick );
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			base.OnHandleDestroyed( e );

			_toggleBar.Click -= new System.EventHandler( _toggleBar_Click );
			_toggleBar.Paint -= new System.Windows.Forms.PaintEventHandler( _toggleBar_Paint );
			_splitter.MouseDown -= new System.Windows.Forms.MouseEventHandler( _splitter_MouseDown );
			_splitter.Paint -= new System.Windows.Forms.PaintEventHandler( _splitter_Paint );
			_dragTimer.Tick -= new System.EventHandler( _dragTimer_Tick );
			_updateTimer.Tick -= new System.EventHandler( _updateTimer_Tick );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			LayoutPanels();
		}

		protected virtual void OnPaintSplitter( PaintEventArgs e )
		{
			if( PaintSplitter != null )
			{
				PaintSplitter( _splitter, e );
			}
		}

		protected virtual void OnPaintToggler( PaintEventArgs e )
		{
			if( PaintToggler != null )
			{
				PaintToggler( _toggleBar, e );
			}
		}

		protected virtual void OnCollapsedExpanded( EventArgs e )
		{
			if( CollapsedExpanded != null )
			{
				CollapsedExpanded( this, e );
			}
		}

		private void DetachParentEvents()
		{
			if( _ownerForm != null )
			{
				_ownerForm.BackColorChanged -= new EventHandler( _ownerForm_BackColorChanged );

				_ownerForm = null;
			}
		}

		private void AttachParentEvents()
		{
			if( _ownerForm != null )
			{
				_ownerForm.BackColorChanged += new EventHandler( _ownerForm_BackColorChanged );
			}
		}

		private void _ownerForm_BackColorChanged( object sender, EventArgs e )
		{
			UpdateBackColor();
		}

		private void UpdateBackColor()
		{
			if( _ownerForm == null )
			{
				_splitter.BackColor = _colorTable.PrimaryBackgroundColor;
				_toggleBar.BackColor = _colorTable.PrimaryBackgroundColor;
			}
			else
			{
				_splitter.BackColor = _ownerForm.BackColor;
				_toggleBar.BackColor = _ownerForm.BackColor;
			}
		}

		private void UpdateSplitter()
		{
			if( _collapsed )
			{
				if( Controls.Contains( _splitter ) )
				{
					Controls.Remove( _splitter );
				}

				_panel1.Visible = false;
			}
			else
			{
				if( !Controls.Contains( _splitter ) )
				{
					Controls.Add( _splitter );
				}

				_panel1.Visible = true;
			}
		}

		private void LayoutPanels()
		{
			int width = _collapsed ? 0 : _width;
			int sideInset = _collapsed ? 0 : _inset;
			int splitWidth = _collapsed ? 0 : _splitWidth;

			switch( _collapseSide )
			{
				case Edge.Top:
					{
						_panel1.Bounds = new Rectangle( _inset, _inset, ClientRectangle.Width - _inset * 2, width );
						_panel2.Bounds = new Rectangle( _inset, _inset + sideInset * 2 + width + splitWidth, ClientRectangle.Width - _inset * 2, ClientRectangle.Height - (_inset * 2 + sideInset * 2 + width + splitWidth) );
						_splitter.Bounds = new Rectangle( _inset, width + _inset * 2, ClientRectangle.Width - _inset * 2, _splitWidth );
						_splitter.Cursor = Cursors.HSplit;

						if( _collapsed )
						{
							_toggleBar.Bounds = new Rectangle( _inset + (ClientRectangle.Width - _inset * 2) / 2 - TogglerHeight / 2, ClientRectangle.Top, TogglerHeight, TogglerWidth );
						}
						else
						{
							_toggleBar.Bounds = new Rectangle( _inset + (ClientRectangle.Width - _inset * 2) / 2 - TogglerHeight / 2, width + sideInset + _inset + splitWidth / 2 - TogglerWidth / 2, TogglerHeight, TogglerWidth );
						}
						break;
					}
				case Edge.Bottom:
					{
						_panel1.Bounds = new Rectangle( _inset, ClientRectangle.Height - width - _inset, ClientRectangle.Width - _inset * 2, width );
						_panel2.Bounds = new Rectangle( _inset, _inset, ClientRectangle.Width - _inset * 2, ClientRectangle.Height - width - sideInset * 2 - _inset * 2 - splitWidth );
						_splitter.Bounds = new Rectangle( _inset, ClientRectangle.Height - (width + _inset * 2 + splitWidth), ClientRectangle.Width - _inset * 2, _splitWidth );
						_splitter.Cursor = Cursors.HSplit;

						if( _collapsed )
						{
							_toggleBar.Bounds = new Rectangle( _inset + (ClientRectangle.Width - _inset * 2) / 2 - TogglerHeight / 2, ClientRectangle.Bottom - TogglerWidth, TogglerHeight, TogglerWidth );
						}
						else
						{
							_toggleBar.Bounds = new Rectangle( _inset + (ClientRectangle.Width - _inset * 2) / 2 - TogglerHeight / 2, ClientRectangle.Height - (width + _inset * 2 + splitWidth) + _splitWidth / 2 - TogglerWidth / 2, TogglerHeight, TogglerWidth );
						}
						break;
					}
				case Edge.Left:
					{
						_panel1.Bounds = new Rectangle( _inset, _inset, width, ClientRectangle.Height - _inset * 2 );
						_panel2.Bounds = new Rectangle( _inset + sideInset * 2 + width + splitWidth, _inset, ClientRectangle.Width - (_inset * 2 + sideInset * 2 + width + splitWidth), ClientRectangle.Height - _inset * 2 );
						_splitter.Bounds = new Rectangle( width + _inset * 2, _inset, _splitWidth, ClientRectangle.Height - _inset * 2 );
						_splitter.Cursor = Cursors.VSplit;

						if( _collapsed )
						{
							_toggleBar.Bounds = new Rectangle( ClientRectangle.Left - 1, _inset + (ClientRectangle.Height - _inset * 2) / 2 - TogglerHeight / 2, TogglerWidth, TogglerHeight );
						}
						else
						{
							_toggleBar.Bounds = new Rectangle( width + sideInset + _inset + splitWidth / 2 - TogglerWidth / 2, _inset + (ClientRectangle.Height - _inset * 2) / 2 - TogglerHeight / 2, TogglerWidth, TogglerHeight );
						}
						break;
					}
				case Edge.Right:
					{
						_panel1.Bounds = new Rectangle( ClientRectangle.Width - width - _inset, _inset, width, ClientRectangle.Height - _inset * 2 );
						_panel2.Bounds = new Rectangle( _inset, _inset, ClientRectangle.Width - width - sideInset * 2 - _inset * 2 - splitWidth, ClientRectangle.Height - _inset * 2 );
						_splitter.Bounds = new Rectangle( ClientRectangle.Width - (width + _inset * 2 + splitWidth), _inset, _splitWidth, ClientRectangle.Height - _inset * 2 );
						_splitter.Cursor = Cursors.VSplit;

						if( _collapsed )
						{
							_toggleBar.Bounds = new Rectangle( ClientRectangle.Right - TogglerWidth, _inset + (ClientRectangle.Height - _inset * 2) / 2 - TogglerHeight / 2, TogglerWidth, TogglerHeight );
						}
						else
						{
							_toggleBar.Bounds = new Rectangle( ClientRectangle.Width - (width + _inset * 2 + splitWidth) + _splitWidth / 2 - TogglerWidth / 2, _inset + (ClientRectangle.Height - _inset * 2) / 2 - TogglerHeight / 2, TogglerWidth, TogglerHeight );
						}
						break;
					}
				default:
					throw new InvalidOperationException();
			}

			UpdateSplitter();
		}

		private void _toggleBar_Click( object sender, EventArgs e )
		{
			_collapsed = !_collapsed;

			LayoutPanels();
			OnCollapsedExpanded( EventArgs.Empty );
			_toggleBar.Invalidate();
		}

		private void _splitter_MouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				_startWidth = _width;
				_dragStart = Control.MousePosition;
				_dragTimer.Start();
			}
		}

		private void _dragTimer_Tick( object sender, EventArgs e )
		{
			if( Control.MouseButtons != MouseButtons.Left )
			{
				_dragTimer.Stop();
				return;
			}

			int offset;

			switch( _collapseSide )
			{
				case Edge.Top:
					{
						offset = Control.MousePosition.Y - _dragStart.Y;
						break;
					}
				case Edge.Bottom:
					{
						offset = _dragStart.Y - Control.MousePosition.Y;
						break;
					}
				case Edge.Left:
					{
						offset = Control.MousePosition.X - _dragStart.X;
						break;
					}
				case Edge.Right:
					{
						offset = _dragStart.X - Control.MousePosition.X;
						break;
					}
				default:
					throw new InvalidOperationException();
			}

			_width = _startWidth + offset;

			ConstrainWidth();

			LayoutPanels();
		}

		private void ConstrainWidth()
		{
			_width = Math.Min( Math.Max( _width, _minWidth ), ClientRectangle.Width - _inset * 4 - _splitWidth - _minExtra );
		}

		private void _splitter_Paint( object sender, PaintEventArgs e )
		{
			OnPaintSplitter( e );
		}

		private void _toggleBar_Paint( object sender, PaintEventArgs e )
		{
			OnPaintToggler( e );

			Edge pointTowards;
			int edge = _collapsed ? 2 : 1;

			switch( _collapseSide )
			{
				case Edge.Top:
					if( _collapsed )
					{
						pointTowards = Edge.Bottom;
					}
					else
					{
						pointTowards = Edge.Top;
					}
					break;
				case Edge.Bottom:
					if( _collapsed )
					{
						pointTowards = Edge.Top;
					}
					else
					{
						pointTowards = Edge.Bottom;
					}
					break;
				case Edge.Left:
					if( _collapsed )
					{
						pointTowards = Edge.Right;
					}
					else
					{
						pointTowards = Edge.Left;
					}
					break;
				case Edge.Right:
					if( _collapsed )
					{
						pointTowards = Edge.Left;
					}
					else
					{
						pointTowards = Edge.Right;
					}
					break;
				default:
					throw new InvalidOperationException();
			}

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			Color arrowColor = _colorTable.TextColor;

			if( _overToggler )
			{
				ProfessionalColorTable pct = new ProfessionalColorTable();

				if( _collapsed )
				{
					using( Brush brush = new SolidBrush( _colorTable.GlowColor ) )
					{
						e.Graphics.FillRectangle( brush, ClientRectangle );
					}
				}
				else
				{
					arrowColor = _colorTable.GlowColor;
				}
			}

			using( Brush brush = new SolidBrush( arrowColor ) )
			{
				switch( pointTowards )
				{
					case Edge.Top:
						{
							e.Graphics.FillPolygon( brush, new Point[]
							{
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + edge * 2, TogglerWidth - edge - 1 ),
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth * 2 - edge * 2, TogglerWidth - edge - 1 ),
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth, edge - 1 )
							} );
							break;
						}
					case Edge.Bottom:
						{
							e.Graphics.FillPolygon( brush, new Point[]
							{
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + edge * 2, edge ),
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth * 2 - edge * 2, edge ),
								new Point( (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth, TogglerWidth - edge )
							} );
							break;
						}
					case Edge.Left:
						{
							e.Graphics.FillPolygon( brush, new Point[]
							{
								new Point( TogglerWidth - edge - 1, (TogglerHeight - TogglerWidth * 2) / 2 + edge * 2 ),
								new Point( TogglerWidth - edge - 1, (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth * 2 - edge * 2 ),
								new Point( edge - 1, (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth )
							} );
							break;
						}
					case Edge.Right:
						{
							e.Graphics.FillPolygon( brush, new Point[]
							{
								new Point( edge, (TogglerHeight - TogglerWidth * 2) / 2 + edge * 2 ),
								new Point( edge, (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth * 2 - edge * 2 ),
								new Point( TogglerWidth - edge, (TogglerHeight - TogglerWidth * 2) / 2 + TogglerWidth )
							} );
							break;
						}
					default:
						throw new InvalidOperationException();
				}
			}

			if( _collapsed )
			{
				e.Graphics.DrawRectangle( SystemPens.ControlText, new Rectangle( 0, 0, _toggleBar.ClientRectangle.Width - 1, _toggleBar.ClientRectangle.Height - 1 ) );
			}
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			bool overToggler = _toggleBar.ClientRectangle.Contains( _toggleBar.PointToClient( Control.MousePosition ) );

			if( FindForm() != Form.ActiveForm )
			{
				overToggler = false;
			}

			if( overToggler != _overToggler )
			{
				_overToggler = overToggler;
				_toggleBar.Invalidate();
			}
		}

		private int TogglerWidth
		{
			get
			{
				return 8;
			}
		}

		private int TogglerHeight
		{
			get
			{
				return 35;
			}
		}

		public event PaintEventHandler PaintSplitter;
		public event PaintEventHandler PaintToggler;
		public event EventHandler CollapsedExpanded;

		private int _inset = 0, _splitWidth = 8;
		private Edge _collapseSide = Edge.Left;
		private int _width = 100, _startWidth, _minWidth = 80, _minExtra = 80;
		private bool _collapsed = false;
		private Point _dragStart;
		private bool _overToggler;
		private Drawing.ColorTable _colorTable = new Drawing.WindowsThemeColorTable();
		private Form _ownerForm;
	}
}
