using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;

namespace ProgrammersInc.Windows.Forms
{
    internal class TabStripRenderer : ToolStripRenderer
    {
        private const int selOffset = 2;
        private ToolStripRenderer currentRenderer = null;

        #region Properties
        ToolStripRenderMode renderMode = ToolStripRenderMode.Custom;
        /// <summary>
        /// Obtiene o establece el modo para este Renderer.
        /// </summary>
        public ToolStripRenderMode RenderMode
        {
            get { return renderMode; }
            set
            {
                renderMode = value;
                switch (renderMode)
                {
                    case ToolStripRenderMode.Professional:
                        currentRenderer = new ToolStripProfessionalRenderer();
                        break;
                    case ToolStripRenderMode.System:
                        currentRenderer = new ToolStripSystemRenderer();
                        break;
                    default:
                        currentRenderer = null;
                        break;
                }
            }
        }

        bool mirrored = false;
        /// <summary>
        /// Obtiene o establece si el fondo del control se va a reflejar.
        /// </summary>
        /// <remarks>Use false for left and top positions, true for right and bottom</remarks>
        public bool Mirrored
        {
            get { return mirrored; }
            set { mirrored = value; }
        }

        bool useVisualStyles = Application.RenderWithVisualStyles;
        /// <summary>
        /// Obtiene o establece si los valores de los efectos visuales del SO seran aplicados al control.
        /// </summary>
        public bool UseVisualStyles
        {
            get { return useVisualStyles; }
            set
            {
                if (value && !Application.RenderWithVisualStyles)
                    return;
                useVisualStyles = value;
            }
        }
        #endregion

        #region Methods Implementation
        #region Protected Override
        protected override void Initialize(ToolStrip ts)
        {
            base.Initialize(ts);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Color c = SystemColors.AppWorkspace;
            if (UseVisualStyles)
            {
                VisualStyleRenderer rndr = new VisualStyleRenderer(VisualStyleElement.Tab.Pane.Normal);
                c = rndr.GetColor(ColorProperty.BorderColorHint);
            }

            using (Pen p = new Pen(c))
            using (Pen p2 = new Pen(e.BackColor))
            {
                Rectangle r = e.ToolStrip.Bounds;
                int x1 = (mirrored) ? 0 : r.Width - 1 - e.ToolStrip.Padding.Horizontal;
                int y1 = (mirrored) ? 0 : r.Height - 1;
                if (e.ToolStrip.Orientation == Orientation.Horizontal)
                    e.Graphics.DrawLine(p, 0, y1, r.Width, y1);
                else
                {
                    e.Graphics.DrawLine(p, x1, 0, x1, r.Height);
                    if (!Mirrored)
                        for (int i = x1 + 1; i < r.Width; i++)
                            e.Graphics.DrawLine(p2, i, 0, i, r.Height);
                }
                foreach (ToolStripItem x in e.ToolStrip.Items)
                {
                    if (x.IsOnOverflow) continue;
                    TabStripButton btn = x as TabStripButton;
                    if (btn == null) continue;
                    Rectangle rc = btn.Bounds;
                    int x2 = (mirrored) ? rc.Left : rc.Right;
                    int y2 = (mirrored) ? rc.Top : rc.Bottom - 1;
                    int addXY = (Mirrored) ? 0 : 1;
                    if (e.ToolStrip.Orientation == Orientation.Horizontal)
                    {
                        e.Graphics.DrawLine(p, rc.Left, y2, rc.Right, y2);
                        if (btn.Checked) e.Graphics.DrawLine(p2, rc.Left + 2 - addXY, y2, rc.Right - 2 - addXY, y2);
                    }
                    else
                    {
                        e.Graphics.DrawLine(p, x2, rc.Top, x2, rc.Bottom);
                        if (btn.Checked) e.Graphics.DrawLine(p2, x2, rc.Top + 2 - addXY, x2, rc.Bottom - 2 - addXY);
                    }
                }
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawToolStripBackground(e);
            else
                base.OnRenderToolStripBackground(e);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            TabbedStrip tabs = e.ToolStrip as TabbedStrip;
            TabStripButton tab = e.Item as TabStripButton;
            if (tabs == null || tab == null)
            {
                if (currentRenderer != null)
                    currentRenderer.DrawButtonBackground(e);
                else
                    base.OnRenderButtonBackground(e);
                return;
            }

            bool selected = tab.Checked;
            bool hovered = tab.Selected;
            int top = 0;
            int left = 0;
            int width = tab.Bounds.Width - 1;
            int height = tab.Bounds.Height - 1;
            Rectangle drawBorder;


            if (UseVisualStyles)
            {
                if (tabs.Orientation == Orientation.Horizontal)
                {
                    if (!selected)
                    {
                        top = selOffset;
                        height -= (selOffset - 1);
                    }
                    else
                        top = 1;
                    drawBorder = new Rectangle(0, 0, width, height);
                }
                else
                {
                    if (!selected)
                    {
                        left = selOffset;
                        width -= (selOffset - 1);
                    }
                    else
                        left = 1;
                    drawBorder = new Rectangle(0, 0, height, width);
                }
                using (Bitmap b = new Bitmap(drawBorder.Width, drawBorder.Height))
                {
                    VisualStyleElement el = VisualStyleElement.Tab.TabItem.Normal;
                    if (selected)
                        el = VisualStyleElement.Tab.TabItem.Pressed;
                    if (hovered)
                        el = VisualStyleElement.Tab.TabItem.Hot;
                    if (!tab.Enabled)
                        el = VisualStyleElement.Tab.TabItem.Disabled;

                    if (!selected || hovered) drawBorder.Width++; else drawBorder.Height++;

                    using (Graphics gr = Graphics.FromImage(b))
                    {
                        VisualStyleRenderer rndr = new VisualStyleRenderer(el);
                        rndr.DrawBackground(gr, drawBorder);

                        if (tabs.Orientation == Orientation.Vertical)
                        {
                            if (Mirrored)
                                b.RotateFlip(RotateFlipType.Rotate270FlipXY);
                            else
                                b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                        else
                        {
                            if (mirrored)
                                b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }
                        if (mirrored)
                        {
                            left = tab.Bounds.Width - b.Width - left;
                            top = tab.Bounds.Height - b.Height - top;
                        }
                        g.DrawImage(b, left, top);
                    }
                }
            }
            else
            {
                if (tabs.Orientation == Orientation.Horizontal)
                {
                    if (!selected)
                    {
                        top = selOffset;
                        height -= (selOffset - 1);
                    }
                    else
                        top = 1;
                    if (mirrored)
                    {
                        left = 1;
                        top = 0;
                    }
                    else
                        top++;
                    width--;
                }
                else
                {
                    if (!selected)
                    {
                        left = selOffset;
                        width--;
                    }
                    else
                        left = 1;
                    if (mirrored)
                    {
                        left = 0;
                        top = 1;
                    }
                }
                height--;
                drawBorder = new Rectangle(left, top, width, height);

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (mirrored && tabs.Orientation == Orientation.Horizontal)
                    {
                        gp.AddLine(drawBorder.Left, drawBorder.Top, drawBorder.Left, drawBorder.Bottom - 2);
                        gp.AddArc(drawBorder.Left, drawBorder.Bottom - 3, 2, 2, 90, 90);
                        gp.AddLine(drawBorder.Left + 2, drawBorder.Bottom, drawBorder.Right - 2, drawBorder.Bottom);
                        gp.AddArc(drawBorder.Right - 2, drawBorder.Bottom - 3, 2, 2, 0, 90);
                        gp.AddLine(drawBorder.Right, drawBorder.Bottom - 2, drawBorder.Right, drawBorder.Top);
                    }
                    else if (!mirrored && tabs.Orientation == Orientation.Horizontal)
                    {
                        gp.AddLine(drawBorder.Left, drawBorder.Bottom, drawBorder.Left, drawBorder.Top + 2);
                        gp.AddArc(drawBorder.Left, drawBorder.Top + 1, 2, 2, 180, 90);
                        gp.AddLine(drawBorder.Left + 2, drawBorder.Top, drawBorder.Right - 2, drawBorder.Top);
                        gp.AddArc(drawBorder.Right - 2, drawBorder.Top + 1, 2, 2, 270, 90);
                        gp.AddLine(drawBorder.Right, drawBorder.Top + 2, drawBorder.Right, drawBorder.Bottom);
                    }
                    else if (mirrored && tabs.Orientation == Orientation.Vertical)
                    {
                        gp.AddLine(drawBorder.Left, drawBorder.Top, drawBorder.Right - 2, drawBorder.Top);
                        gp.AddArc(drawBorder.Right - 2, drawBorder.Top + 1, 2, 2, 270, 90);
                        gp.AddLine(drawBorder.Right, drawBorder.Top + 2, drawBorder.Right, drawBorder.Bottom - 2);
                        gp.AddArc(drawBorder.Right - 2, drawBorder.Bottom - 3, 2, 2, 0, 90);
                        gp.AddLine(drawBorder.Right - 2, drawBorder.Bottom, drawBorder.Left, drawBorder.Bottom);
                    }
                    else
                    {
                        gp.AddLine(drawBorder.Right, drawBorder.Top, drawBorder.Left + 2, drawBorder.Top);
                        gp.AddArc(drawBorder.Left, drawBorder.Top + 1, 2, 2, 180, 90);
                        gp.AddLine(drawBorder.Left, drawBorder.Top + 2, drawBorder.Left, drawBorder.Bottom - 2);
                        gp.AddArc(drawBorder.Left, drawBorder.Bottom - 3, 2, 2, 90, 90);
                        gp.AddLine(drawBorder.Left + 2, drawBorder.Bottom, drawBorder.Right, drawBorder.Bottom);
                    }

                    if (selected || hovered)
                    {
                        Color fill = (hovered) ? Color.WhiteSmoke : Color.White;
                        if (renderMode == ToolStripRenderMode.Professional)
                        {
                            fill = (hovered) ? ProfessionalColors.ButtonCheckedGradientBegin : ProfessionalColors.ButtonCheckedGradientEnd;
                            using (LinearGradientBrush br = new LinearGradientBrush(tab.ContentRectangle, fill, ProfessionalColors.ButtonCheckedGradientMiddle, LinearGradientMode.Vertical))
                                g.FillPath(br, gp);
                        }
                        else
                            using (SolidBrush br = new SolidBrush(fill))
                                g.FillPath(br, gp);
                    }
                    using (Pen p = new Pen((selected) ? System.Windows.Forms.ControlPaint.Dark(SystemColors.AppWorkspace) : SystemColors.AppWorkspace))
                        g.DrawPath(p, gp);
                }
            }

        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = e.ImageRectangle;
            TabStripButton btn = e.Item as TabStripButton;
            if (btn != null)
            {
                int delta = ((mirrored) ? -1 : 1) * ((btn.Checked) ? 1 : selOffset);
                if (e.ToolStrip.Orientation == Orientation.Horizontal)
                    rc.Offset((mirrored) ? 2 : 1, delta + ((mirrored) ? 1 : 0));
                else
                    rc.Offset(delta + 2, 0);
            }
            ToolStripItemImageRenderEventArgs x =
                new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, e.Image, rc);
            if (currentRenderer != null)
                currentRenderer.DrawItemImage(x);
            else
                base.OnRenderItemImage(x);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            Rectangle rc = e.TextRectangle;
            TabStripButton btn = e.Item as TabStripButton;
            Color c = e.TextColor;
            Font f = e.TextFont;
            if (btn != null)
            {
                int delta = ((mirrored) ? -1 : 1) * ((btn.Checked) ? 1 : selOffset);
                if (e.ToolStrip.Orientation == Orientation.Horizontal)
                    rc.Offset((mirrored) ? 2 : 1, delta + ((mirrored) ? 1 : -1));
                else
                    rc.Offset(delta + 2, 0);
                if (btn.Selected)
                    c = btn.HotTextColor;
                else if (btn.Checked)
                    c = btn.SelectedTextColor;
                if (btn.Checked)
                    f = btn.SelectedFont;
            }
            ToolStripItemTextRenderEventArgs x =
                new ToolStripItemTextRenderEventArgs(e.Graphics, e.Item, e.Text, rc, c, f, e.TextFormat);
            x.TextDirection = e.TextDirection;
            if (currentRenderer != null)
                currentRenderer.DrawItemText(x);
            else
                base.OnRenderItemText(x);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawArrow(e);
            else
                base.OnRenderArrow(e);
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawDropDownButtonBackground(e);
            else
                base.OnRenderDropDownButtonBackground(e);
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawGrip(e);
            else
                base.OnRenderGrip(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawImageMargin(e);
            else
                base.OnRenderImageMargin(e);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawItemBackground(e);
            else
                base.OnRenderItemBackground(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawItemCheck(e);
            else
                base.OnRenderItemCheck(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawLabelBackground(e);
            else
                base.OnRenderLabelBackground(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawMenuItemBackground(e);
            else
                base.OnRenderMenuItemBackground(e);
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawOverflowButtonBackground(e);
            else
                base.OnRenderOverflowButtonBackground(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawSeparator(e);
            else
                base.OnRenderSeparator(e);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawSplitButton(e);
            else
                base.OnRenderSplitButtonBackground(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawStatusStripSizingGrip(e);
            else
                base.OnRenderStatusStripSizingGrip(e);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawToolStripContentPanelBackground(e);
            else
                base.OnRenderToolStripContentPanelBackground(e);
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawToolStripPanelBackground(e);
            else
                base.OnRenderToolStripPanelBackground(e);
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
        {
            if (currentRenderer != null)
                currentRenderer.DrawToolStripStatusLabelBackground(e);
            else
                base.OnRenderToolStripStatusLabelBackground(e);
        }
        #endregion
        #endregion
    }
}