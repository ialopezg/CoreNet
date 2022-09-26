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
using System.Diagnostics;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public partial class RibbonControl : ContainerControl
	{
		public RibbonControl()
		{
			SetStyle
				( ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.ContainerControl
				, true );
			SetStyle
				( ControlStyles.Selectable
				, false );

			_updateTimer = new Timer();
			_updateTimer.Enabled = true;
			_updateTimer.Tick += new System.EventHandler( _updateTimer_Tick );
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			if( _updateTimer != null )
			{
				_updateTimer.Tick -= new System.EventHandler( _updateTimer_Tick );
				_updateTimer.Dispose();
				_updateTimer = null;
			}
		}

		public Renderer Renderer
		{
			get
			{
				return _renderer;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}

				_renderer = value;
				NotifyNeedsLayout();
				Invalidate( true );
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

		public WinFormsUtility.Drawing.Glass Glass
		{
			get
			{
				return _glass;
			}
			set
			{
				_glass = value;
			}
		}

		public Section[] Sections
		{
			get
			{
				return _sections.ToArray();
			}
		}

		public int LevelOfDetail
		{
			get
			{
				return _levelOfDetail;
			}
		}

		public bool ShowToolTips
		{
			get
			{
				return _showToolTips;
			}
			set
			{
				_showToolTips = value;
			}
		}

		public int MaximumWidth
		{
			get
			{
				if( _needsLayout )
				{
					ReLayout( false );
					_needsLayout = false;
				}

				List<LayoutDetails> listLayoutDetails;
				Utility.Collections.ComparativeTuple visiblesKey = GetSectionsVisibleKey();

				if( !_mapLevelOfDetails.TryGetValue( visiblesKey, out listLayoutDetails ) )
				{
					return 0;
				}
				else
				{
					return listLayoutDetails[listLayoutDetails.Count - 1].MaxWidth;
				}
			}
		}

		public void AddSection( Section section )
		{
			if( section == null )
			{
				throw new ArgumentNullException( "section" );
			}
			if( section.Ribbon != null )
			{
				throw new ArgumentException( "Section is already part of a ribbon.", "section" );
			}

			_sections.Add( section );
			section.Ribbon = this;

			NotifySectionChanged( section );
		}

		public void RemoveSection( Section section )
		{
			if( section == null )
			{
				throw new ArgumentNullException( "section" );
			}
			if( !_sections.Contains( section ) )
			{
				throw new ArgumentException( "Ribbon does not contain this section.", "section" );
			}

			_sections.Remove( section );
			section.Ribbon = null;

			NotifyNeedsLayout();
			Invalidate( true );
		}

		public void NotifySectionChanged( Section section )
		{
			if( section == null )
			{
				throw new ArgumentNullException( "section" );
			}
			if( !_sections.Contains( section ) )
			{
				throw new ArgumentException( "Ribbon does not contain this section.", "section" );
			}

			if( !IsHandleCreated )
			{
				return;
			}

			NotifyNeedsLayout();

			using( Graphics g = CreateGraphics() )
			{
				Rectangle rect = GetSectionBounds( g, section );

				Invalidate( rect, true );
			}
		}

		public void NotifyNeedsLayout()
		{
			_mapLevelOfDetails.Clear();
			_needsLayout = true;
			Invalidate();
		}

		public bool IsSectionVisible( Section section, int levelOfDetail )
		{
			if( !section.Visible )
			{
				return false;
			}
			if( levelOfDetail > section.DisplayUntil )
			{
				return false;
			}

			return true;
		}

		public void SetMargins( int left, int right )
		{
			if( _leftOffset == left && _rightOffset == right )
			{
				return;
			}

			_leftOffset = left;
			_rightOffset = right;

			NotifyNeedsLayout();
		}

		public void GetUnusedBounds( out int left, out int right )
		{
			left = 0;
			right = Width;

			foreach( Section section in _sections )
			{
				Rectangle rect = _mapSectionToRectangle[section];

				if( section.Alignment == Alignment.Left )
				{
					left = Math.Max( left, rect.Right );
				}
				else
				{
					right = Math.Min( right, rect.Left );
				}
			}
		}

		public void UpdateKeys( WinFormsUtility.Commands.KeyCommandLauncher keyCommandLauncher )
		{
			foreach( Controls.Ribbon.Section section in Sections )
			{
				foreach( Controls.Ribbon.Item item in section.Items )
				{
					Commands.CommandRibbonButtonItem buttonItem = item as Commands.CommandRibbonButtonItem;
					Commands.CommandSemiDropDownRibbonButtonItem semiButtonItem = item as Commands.CommandSemiDropDownRibbonButtonItem;

					if( buttonItem != null && buttonItem.Command != null )
					{
						string keyText = keyCommandLauncher.GetKeyText( buttonItem.Command );

						if( keyText != null )
						{
							buttonItem.TooltipDescription += string.Format( " ({0})", keyText );
						}
					}

					if( semiButtonItem != null && semiButtonItem.Command != null )
					{
						string keyText = keyCommandLauncher.GetKeyText( semiButtonItem.Command );

						if( keyText != null )
						{
							semiButtonItem.TooltipDescription += string.Format( " ({0})", keyText );
						}
					}
				}
			}
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			_mapLevelOfDetails.Clear();
			ReLayout( true );
		}

		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			bool hasRightAligned = false;

			foreach( Section section in _sections )
			{
				hasRightAligned |= (section.Alignment == Alignment.Right);
			}

			if( hasRightAligned )
			{
				_mapLevelOfDetails.Clear();
				ReLayout( true );
			}
			else
			{
				int change = Math.Abs( _oldWidth - ClientRectangle.Width ) + 16;

				Invalidate( new Rectangle( ClientRectangle.Width - change, 0, change, ClientRectangle.Height ) );

				_oldWidth = ClientRectangle.Width;

				ReLayout( false );
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			if( _needsLayout )
			{
				ReLayout( false );
				_needsLayout = false;
			}

			DoPaint( e.Graphics, e.ClipRectangle, true );

			base.OnPaint( e );
		}

		protected override void OnPaintBackground( PaintEventArgs e )
		{
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			UpdateAndInvalidate();
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );
			this.PerformOnMouseDown();
		}

		protected void PerformOnMouseDown()
		{
			Section mouseOverSection;
			Item mouseOverItem;

			FindMouseOvers( out mouseOverSection, out mouseOverItem );

			_mouseDownItem = mouseOverItem;

			if( mouseOverItem != null )
			{
				_updates.NeedsUpdate( mouseOverItem );
			}
			if( _history.MouseOverItem != null )
			{
				_updates.NeedsUpdate( _history.MouseOverItem );
			}

			RunUpdates();
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			Section mouseOverSection;
			Item mouseOverItem;

			FindMouseOvers( out mouseOverSection, out mouseOverItem );

			if( mouseOverItem != null )
			{
				_updates.NeedsUpdate( mouseOverItem );
			}
			if( _history.MouseOverItem != null )
			{
				_updates.NeedsUpdate( _history.MouseOverItem );
			}

			if( _mouseDownItem == mouseOverItem && _mouseDownItem != null && e.Button == MouseButtons.Left )
			{
				SuperToolTipManager.CloseToolTip();

				Context context = new Context( null, this, _renderer, _history, _updates, _mapSectionToRectangle, _mapItemToRectangle );

				mouseOverItem.PerformClick( context );
				_mouseDownItem = null;
				_lastMouseWhen = DateTime.Now;
			}

			RunUpdates();
		}

		protected virtual void SetHeight( int height )
		{
			Height = height;
		}

		private void DoPaint( Graphics g, Rectangle clip, bool markDone )
		{
			if( _mapItemToRectangle.Count == 0 || _mapSectionToRectangle.Count == 0 )
			{
				ReLayout( false );
			}

			Context context = new Context( g, this, _renderer, _history, _updates, _mapSectionToRectangle, _mapItemToRectangle );

			_renderer.PaintBackground( context, clip );

			foreach( Section section in _sections )
			{
				Rectangle sectionRect = GetSectionBounds( g, section );
				Rectangle sectionVisualRect = _renderer.GetVisualBounds( g, section, sectionRect );

				if( sectionVisualRect.IntersectsWith( clip ) || sectionVisualRect == Rectangle.Empty )
				{
					if( markDone )
					{
						_updates.DoneUpdate( section );
					}

					section.Paint( context, clip, sectionRect );

					foreach( Item item in section.Items )
					{
						Rectangle itemRect = GetItemBounds( g, item );
						Rectangle itemVisualRect = _renderer.GetVisualBounds( g, item, itemRect );

						if( itemVisualRect.IntersectsWith( clip ) )
						{
							if( markDone )
							{
								_updates.DoneUpdate( item );
							}

							item.Paint( context, clip, itemRect );
						}
					}
				}
			}
		}

		private void ShowTooltip()
		{
			if( !ShowToolTips )
			{
				return;
			}

			Section mouseOverSection;
			Item mouseOverItem;

			FindMouseOvers( out mouseOverSection, out mouseOverItem );

			if( mouseOverItem != null && mouseOverSection != null )
			{
				Rectangle itemRect = _mapItemToRectangle[mouseOverItem];
				Rectangle sectionRect = _mapSectionToRectangle[mouseOverSection];
				Point p = new Point( itemRect.X, sectionRect.Bottom );

				p = PointToScreen( p );

				SuperToolTipInfo tooltipInfo = mouseOverItem.GetTooltipInfo();

				if( tooltipInfo != null )
				{
					SuperToolTipManager.ShowToolTip( _colorTable, tooltipInfo, this, p );
				}
			}
		}

		private Utility.Collections.ComparativeTuple GetSectionsVisibleKey()
		{
			object[] visibles = new object[_sections.Count];

			for( int i = 0; i < _sections.Count; ++i )
			{
				visibles[i] = _sections[i].Visible;
			}

			return new Utility.Collections.ComparativeTuple( visibles );
		}

		private void ReLayout( bool forceInvalidate )
		{
			if( _layingOutFlag.IsActive )
			{
				return;
			}
			if( !IsHandleCreated )
			{
				return;
			}

			using( _layingOutFlag.Apply() )
			{
				int oldLevelOfDetail = _levelOfDetail;

				List<LayoutDetails> listLayoutDetails;
				Utility.Collections.ComparativeTuple visiblesKey = GetSectionsVisibleKey();

				if( !_mapLevelOfDetails.TryGetValue( visiblesKey, out listLayoutDetails ) )
				{
					using( Graphics g = CreateGraphics() )
					{
						listLayoutDetails = new List<LayoutDetails>();

						for( int lod = Item.BestLevelOfDetail; lod <= Item.WorstLevelOfDetail; ++lod )
						{
							LayoutDetails layoutDetails = new LayoutDetails();

							int widthRequired;
							int ribbonHeight;
							bool success = _renderer.Layout
								( g, this, _sections.ToArray(), lod, _leftOffset, _rightOffset
								, out layoutDetails.MapSectionToRectangle, out layoutDetails.MapItemToRectangle, out ribbonHeight, out widthRequired );

							layoutDetails.MaxWidth = widthRequired;
							layoutDetails.LevelOfDetail = lod;
							layoutDetails.RibbonHeight = ribbonHeight;

							listLayoutDetails.Add( layoutDetails );
						}
					}

					listLayoutDetails.Reverse();

					_mapLevelOfDetails.Add( visiblesKey, listLayoutDetails );
				}

				int newRibbonHeight = 0;

				foreach( LayoutDetails layoutDetails in listLayoutDetails )
				{
					if( layoutDetails.MaxWidth > ClientRectangle.Width )
					{
						break;
					}

					newRibbonHeight = layoutDetails.RibbonHeight;
					_mapItemToRectangle = layoutDetails.MapItemToRectangle;
					_mapSectionToRectangle = layoutDetails.MapSectionToRectangle;
					_levelOfDetail = layoutDetails.LevelOfDetail;
				}

				if( forceInvalidate || _levelOfDetail != oldLevelOfDetail )
				{
					Invalidate( true );
				}

				if( Height != newRibbonHeight )
				{
					SetHeight( newRibbonHeight );
				}
			}
		}

		private Rectangle GetSectionBounds( Graphics g, Section section )
		{
			Rectangle rect = Rectangle.Empty;

			_mapSectionToRectangle.TryGetValue( section, out rect );

			return rect;
		}

		private Rectangle GetItemBounds( Graphics g, Item item )
		{
			return _mapItemToRectangle[item];
		}

		private void _updateTimer_Tick( object sender, EventArgs e )
		{
			UpdateAndInvalidate();

			Point mousePos = Control.MousePosition;

			if( Visible && mousePos == _lastMousePosition )
			{
				if( DateTime.Now.Subtract( _lastMouseWhen ).TotalMilliseconds > SystemInformation.MouseHoverTime )
				{
					if( FindForm() == Form.ActiveForm && Form.ActiveForm != null )
					{
						ShowTooltip();
					}
				}
			}
			else
			{
				_lastMousePosition = mousePos;
				_lastMouseWhen = DateTime.Now;
			}
		}

		private void FindMouseOvers( out Section mouseOverSection, out Item mouseOverItem )
		{
			mouseOverSection = null;
			mouseOverItem = null;

			if( WinFormsUtility.Events.MenuLoop.InMenuLoop )
			{
				return;
			}
			if( FindForm() != Form.ActiveForm )
			{
				return;
			}
			if( Height == 0 )
			{
				return;
			}

			Point mousePos = PointToClient( Control.MousePosition );

			using( Graphics g = CreateGraphics() )
			{
				foreach( Section section in _sections )
				{
					Rectangle sectionRect = GetSectionBounds( g, section );

					if( sectionRect.Contains( mousePos ) )
					{
						mouseOverSection = section;

						foreach( Item item in section.Items )
						{
							Rectangle itemRect = GetItemBounds( g, item );

							if( itemRect.Contains( mousePos ) )
							{
								mouseOverItem = item;
								break;
							}
						}

						break;
					}
				}
			}
		}

		private void UpdateAndInvalidate()
		{
			Section mouseOverSection;
			Item mouseOverItem;

			FindMouseOvers( out mouseOverSection, out mouseOverItem );

			if( _history.MouseOverSection != mouseOverSection )
			{
				if( mouseOverSection != null )
				{
					_updates.NeedsUpdate( mouseOverSection );
				}
				if( _history.MouseOverSection != null )
				{
					_updates.NeedsUpdate( _history.MouseOverSection );
				}
			}

			bool addedMouseOverItem = false;

			if( _history.MouseOverItem != mouseOverItem )
			{
				if( mouseOverItem != null )
				{
					_updates.NeedsUpdate( mouseOverItem );
					addedMouseOverItem = true;
				}
				if( _history.MouseOverItem != null )
				{
					_updates.NeedsUpdate( _history.MouseOverItem );
				}
			}

			if( !addedMouseOverItem && mouseOverItem != null && mouseOverItem.NeedsMouseOverUpdate )
			{
				_updates.NeedsUpdate( mouseOverItem );
			}

			_history.Update( mouseOverSection, mouseOverItem );

			RunUpdates();
		}

		private void RunUpdates()
		{
			if( IsDisposed )
			{
				return;
			}

			using( Graphics g = CreateGraphics() )
			{
				foreach( Section section in _updates.Sections )
				{
					Rectangle sectionRect = GetSectionBounds( g, section );
					Rectangle sectionVisualRect = _renderer.GetVisualBounds( g, section, sectionRect );

					sectionVisualRect.Inflate( 1, 1 );

					InvalidateRect( sectionVisualRect );
				}
				foreach( Item item in _updates.Items )
				{
					Rectangle itemRect = GetItemBounds( g, item );
					Rectangle itemVisualRect = _renderer.GetVisualBounds( g, item, itemRect );

					itemVisualRect.Inflate( 1, 1 );

					InvalidateRect( itemVisualRect );
				}
			}
		}

		private void InvalidateRect( Rectangle rect )
		{
			Invalidate( rect, true );

			foreach( Control child in this.Controls )
			{
				if( child.Bounds.IntersectsWith( rect ) )
				{
					child.Refresh();
				}
			}
		}

		[DebuggerDisplay( "LayoutDetails(MaxWidth={MaxWidth},LevelOfDetail={LevelOfDetail}, RibbonHeight={RibbonHeight})" )]
		private sealed class LayoutDetails
		{
			internal int MaxWidth, LevelOfDetail, RibbonHeight;
			internal Dictionary<Section, Rectangle> MapSectionToRectangle = new Dictionary<Section, Rectangle>();
			internal Dictionary<Item, Rectangle> MapItemToRectangle = new Dictionary<Item, Rectangle>();
		}

		private List<Section> _sections = new List<Section>();
		private Dictionary<Section, Rectangle> _mapSectionToRectangle = new Dictionary<Section, Rectangle>();
		private Dictionary<Item, Rectangle> _mapItemToRectangle = new Dictionary<Item, Rectangle>();
		private Renderer _renderer = new GlossyRenderer();
		private History _history = new History();
		private Updates _updates = new Updates();
		private Item _mouseDownItem;
		private int _levelOfDetail = Item.BestLevelOfDetail;
		private Point _lastMousePosition;
		private DateTime _lastMouseWhen;
		private Dictionary<Utility.Collections.ComparativeTuple, List<LayoutDetails>> _mapLevelOfDetails
			= new Dictionary<Utility.Collections.ComparativeTuple, List<LayoutDetails>>();
		private int _oldWidth = 0;
		private Utility.Control.Flag _layingOutFlag = new Utility.Control.Flag();
		private Drawing.ColorTable _colorTable = new Drawing.WindowsThemeColorTable();
		private int _leftOffset, _rightOffset;
		private WinFormsUtility.Drawing.Glass _glass;
		private bool _needsLayout;
		private bool _showToolTips = true;
		private Timer _updateTimer;
	}
}
