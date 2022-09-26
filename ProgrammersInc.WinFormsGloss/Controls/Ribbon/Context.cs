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
using System.Diagnostics;

namespace ProgrammersInc.WinFormsGloss.Controls.Ribbon
{
	public sealed class Context
	{
		public Context
			( Graphics g, RibbonControl ribbonControl, Renderer renderer, History history, Updates updates
			, Dictionary<Section, Rectangle> mapSectionToRectangle, Dictionary<Item, Rectangle> mapItemToRectangle )
		{
			_graphics = g;
			_renderer = renderer;
			_history = history;
			_updates = updates;
			_ribbonControl = ribbonControl;
			_mapSectionToRectangle = mapSectionToRectangle;
			_mapItemToRectangle = mapItemToRectangle;
		}

		public Graphics Graphics
		{
			[DebuggerStepThrough]
			get
			{
				return _graphics;
			}
		}

		public Renderer Renderer
		{
			[DebuggerStepThrough]
			get
			{
				return _renderer;
			}
		}

		public History History
		{
			[DebuggerStepThrough]
			get
			{
				return _history;
			}
		}

		public Updates Updates
		{
			[DebuggerStepThrough]
			get
			{
				return _updates;
			}
		}

		public RibbonControl  RibbonControl
		{
			[DebuggerStepThrough]
			get
			{
				return _ribbonControl;
			}
		}

		public Rectangle GetSectionBounds( Section section )
		{
			return _mapSectionToRectangle[section];
		}

		public Rectangle GetItemBounds( Item item )
		{
			return _mapItemToRectangle[item];
		}

		private Graphics _graphics;
		private Renderer _renderer;
		private History _history;
		private Updates _updates;
		private RibbonControl _ribbonControl;
		private Dictionary<Section, Rectangle> _mapSectionToRectangle = new Dictionary<Section, Rectangle>();
		private Dictionary<Item, Rectangle> _mapItemToRectangle = new Dictionary<Item, Rectangle>();
	}
}
