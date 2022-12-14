/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ProgrammersInc.Utility.Assemblies;
using ProgrammersInc.WinFormsUtility.Drawing;

namespace ProgrammersInc.SuperList.Sections
{
	public class GroupSection : RowSection
	{
		public GroupSection( ListControl listControl, RowIdentifier ri, HeaderSection headerSection, int position, int groupIndentWidth )
			: base( listControl, ri, headerSection, position )
		{
			Debug.Assert( ri is ListSection.GroupIdentifier );
			_groupIndentWidth = groupIndentWidth;
		}

		public override void Layout( GraphicsSettings gs, Size maximumSize )
		{
			SizeF size = gs.Graphics.MeasureString( Text, Font );
			if( size.Height < MinimumHeight )
			{
				size.Height = MinimumHeight;
			}
			Size = new Size( HeaderSection.Rectangle.Width, (int)size.Height + _margin + _separatorLineHeight );
		}

		public override void Paint( GraphicsSettings gs, Rectangle clipRect )
		{
			Rectangle rcText;
			GetDrawRectangles( out rcText, out _buttonRectangle );
			Rectangle rc = HostBasedRectangle;

			//
			// Fill indent area if any
			if( _groupIndentWidth > 0 )
			{
				Rectangle rcIndent = new Rectangle( rc.X, rc.Y, _groupIndentWidth, rc.Height );

				PaintIndentArea( gs.Graphics, rcIndent );
			}

			gs.Graphics.DrawIcon( DrawIcon, _buttonRectangle.X, _buttonRectangle.Y );

			GdiPlusEx.DrawString
					( gs.Graphics, Text, Font, (Host.FocusedSection == ListSection && IsSelected) ? SystemColors.HighlightText : ListControl.GroupSectionForeColor, rcText
					 , GdiPlusEx.Alignment.Left, ListControl.GroupSectionVerticalAlignment, GdiPlusEx.TextSplitting.SingleLineEllipsis, GdiPlusEx.Ampersands.Display );

			Rectangle rcLine = rc;
			rcLine.X += _groupIndentWidth;
			PaintSeparatorLine( gs.Graphics, rcLine );
		}

		public void GetDrawRectangles( out Rectangle textRectangle, out Rectangle buttonRectangle )
		{
			int spacing = 4;
			Rectangle rc = HostBasedRectangle;

			rc.X += _groupIndentWidth + spacing;
			rc.Height -= _margin + _separatorLineHeight;
			rc.Y += _margin;
			buttonRectangle = new Rectangle( rc.X, rc.Y, DrawIcon.Width, DrawIcon.Height );
			rc.X += DrawIcon.Width + spacing;
			textRectangle = new Rectangle( rc.X, rc.Y - 1, rc.Width, rc.Height );
			switch( ListControl.GroupSectionVerticalAlignment )
			{
				case GdiPlusEx.VAlignment.Bottom:
					buttonRectangle.Y = textRectangle.Bottom - DrawIcon.Height;
					break;
				case GdiPlusEx.VAlignment.Center:
					buttonRectangle.Y = (textRectangle.Bottom - textRectangle.Height / 2) - DrawIcon.Height / 2;
					break;
			}
		}

		private Icon DrawIcon
		{
			get
			{
				Icon drawIcon = null;
				switch( GroupState )
				{
					case ListSection.GroupState.Collapsed:
						drawIcon = _resources.ExpandIcon;
						break;
					case ListSection.GroupState.Expanded:
						drawIcon = _resources.CollapseIcon;
						break;
				}
				return drawIcon;
			}
		}

		public override bool MouseDoubleClick( Point pt )
		{
			GroupState = GroupState == ListSection.GroupState.Expanded ? ListSection.GroupState.Collapsed : ListSection.GroupState.Expanded;
			return true;
		}

		public override void MouseClick( MouseEventArgs e )
		{
			if( !_buttonRectangle.Contains( new Point( e.X, e.Y ) ) ) // stop selection if over + - button
			{
				base.MouseClick( e );
			}
		}

		public override void MouseDown( MouseEventArgs e )
		{
			if( _buttonRectangle.Contains( new Point( e.X, e.Y ) ) && e.Button == MouseButtons.Left )
			{
				GroupState = GroupState == ListSection.GroupState.Expanded ? ListSection.GroupState.Collapsed : ListSection.GroupState.Expanded;
			}
			else
			{
				base.MouseDown( e );
			}
		}


		protected override int IndentWidth
		{
			get
			{
				return _groupIndentWidth;
			}
		}

		protected virtual string Text
		{
			get
			{
				int descendentCount = RowIdentifier.Items.Length;
				return string.Format( "{0} ({1} {2})",
														 RowIdentifier.LastColumn.GroupItemAccessor( Item ),
														 descendentCount,
														 descendentCount == 1 ? ListControl.GroupSectionTextSingular : ListControl.GroupSectionTextPlural
						);
			}
		}

		protected virtual Font Font
		{
			get
			{
				return ListControl.GroupSectionFont == null ? Host.Font : ListControl.GroupSectionFont;
			}
		}

		protected override void PaintSeparatorLine( Graphics g, Rectangle rc )
		{
			using( Pen pen = new Pen( SeperatorColor, _separatorLineHeight ) )
			{
				g.DrawLine( pen, new Point( rc.Left, rc.Bottom - _separatorLineHeight + 1 ), new Point( rc.Right, rc.Bottom - _separatorLineHeight + 1 ) );
			}
		}

		protected ListSection.GroupState GroupState
		{
			get
			{
				return ListSection.GetGroupState( RowIdentifier );
			}
			set
			{
				ListSection.SetGroupState( RowIdentifier, value, true );
			}
		}

		#region Resources

		/// <summary>
		/// Class that shares out the insertion window resources
		/// </summary>
		private class Resources : IDisposable
		{
			public Resources()
			{
				_referencesCount++;
			}

			public void Dispose()
			{
				if( !_disposed )
				{
					if( --_referencesCount == 0 )
					{
						if( _collapseIcon != null )
						{
							_collapseIcon.Dispose();
							_collapseIcon = null;
						}
						if( _expandIcon != null )
						{
							_expandIcon.Dispose();
							_expandIcon = null;
						}
					}
					Debug.Assert( _referencesCount >= 0 );
					_disposed = true;
				}
			}

			public Icon CollapseIcon
			{
				get
				{
					if( _collapseIcon == null )
					{
						_collapseIcon = _resources.GetIcon( "CollapseGroup.ico" );
					}
					return _collapseIcon;
				}
			}

			public Icon ExpandIcon
			{
				get
				{
					if( _expandIcon == null )
					{
						_expandIcon = _resources.GetIcon( "ExpandGroup.ico" );
					}
					return _expandIcon;
				}
			}

			private bool _disposed = false;
			private static int _referencesCount = 0;
			private static Icon _expandIcon = null;
			private static Icon _collapseIcon = null;
			private ManifestResources _resources = new ManifestResources( "ProgrammersInc.SuperList.Resources" );
		}

		#endregion

		private const int _separatorLineHeight = 2;
		private const int _margin = 7;
		private Rectangle _buttonRectangle;
		private int _groupIndentWidth;
		private static Resources _resources = new Resources();
	}
}