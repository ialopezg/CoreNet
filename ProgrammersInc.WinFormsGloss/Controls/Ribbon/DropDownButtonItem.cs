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
using System.Windows.Forms;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public class DropDownButtonItem : ButtonItem
	{
		public DropDownButtonItem( string text, Icon icon16, Icon icon24 )
			: base( text, icon16, icon24 )
		{
			Enabled = false;
		}

		public ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return _contextMenuStrip;
			}
			set
			{
				_contextMenuStrip = value;
				Enabled = (_contextMenuStrip != null);
			}
		}

		public WinFormsUtility.Commands.CommandControlSet CommandControlSet
		{
			get
			{
				return _commandControlSet;
			}
			set
			{
				_commandControlSet = value;
			}
		}

		public override Size GetLogicalSize( RibbonControl ribbonControl, Graphics g, Size suggestedSize )
		{
			Size size = base.GetLogicalSize( ribbonControl, g, suggestedSize );
			ButtonSizeKind buttonSizeKind = base.IdentifyButton( size );

			switch( buttonSizeKind )
			{
				case ButtonSizeKind.Single:
				case ButtonSizeKind.Wide:
					return new Size( size.Width + TriangleSize, size.Height );
				case ButtonSizeKind.Big:
					return size;
				default:
					throw new InvalidOperationException();
			}
		}

		protected override bool OnClick( Context context )
		{
			if( _contextMenuStrip != null )
			{
				SuperToolTipManager.CloseToolTip();

				Rectangle itemBounds = context.GetItemBounds( this );

				if( CommandControlSet != null )
				{
					CommandControlSet.UpdateState();
				}

				Point buttonPosn = this.Section.Ribbon.PointToScreen( itemBounds.Location );
				Rectangle buttonRect = new Rectangle( buttonPosn, itemBounds.Size );
				Point screen = WinFormsUtility.Controls.ControlUtils.FixForScreen( buttonRect );

				Point display = context.RibbonControl.PointToClient( screen );

				_contextMenuStrip.Show( context.RibbonControl, display );

				return true;
			}
			else
			{
				return false;
			}
		}

		public override void Paint( Context context, Rectangle clip, Rectangle logicalBounds )
		{
			base.Paint( context, clip, logicalBounds );

			ButtonSizeKind buttonSizeKind = base.IdentifyButton( logicalBounds.Size );
			Rectangle arrowRect;
			Drawing.ColorTable colorTable = context.RibbonControl.ColorTable;

			switch( buttonSizeKind )
			{
				case ButtonSizeKind.Single:
				case ButtonSizeKind.Wide:
					arrowRect = new Rectangle( logicalBounds.Right - TriangleSize - 3, logicalBounds.Y + (logicalBounds.Height - TriangleSize) / 2 + 3, TriangleSize, TriangleSize / 2 );
					break;
				case ButtonSizeKind.Big:
					arrowRect = new Rectangle( logicalBounds.Left + (logicalBounds.Width - TriangleSize)/2, logicalBounds.Bottom - TriangleSize / 2, TriangleSize, TriangleSize / 2 );
					break;
				default:
					throw new InvalidOperationException();
			}

			using( Brush brush = new SolidBrush( colorTable.TextColor ) )
			{
				context.Graphics.FillPolygon( brush, new Point[]
					{ 
						new Point( arrowRect.Right, arrowRect.Top ),
						new Point( arrowRect.Left, arrowRect.Top ),
						new Point( arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom )
					} );
			}
		}

		public override SuperToolTipInfo GetTooltipInfo()
		{
			if( _contextMenuStrip != null && _contextMenuStrip.Visible )
			{
				return null;
			}
			else
			{
				return base.GetTooltipInfo();
			}
		}

		protected const int TriangleSize = 10;

		private ContextMenuStrip _contextMenuStrip;
		private WinFormsUtility.Commands.CommandControlSet _commandControlSet;
	}
}
