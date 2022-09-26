using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    public class CustomGrid : DataGridView
    {
        #region Constructors
        public CustomGrid()
        {
            BackgroundColor = Color.WhiteSmoke;
            DefaultCellStyle.SelectionForeColor = Color.Black;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            RowHeadersWidth = 25;
            this.Paint += new PaintEventHandler(CustomGridPaint);
            this.CellPainting += new DataGridViewCellPaintingEventHandler(CustomGridCellPainting);
        }
        #endregion

        #region Methods Implementation
        #region Protected Override
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            BeginEdit(false);
        }
        #endregion

        #region Protected
        protected void DrawCell(DataGridViewCellPaintingEventArgs e)
        {
            Rectangle r1 = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
        }

        protected void DrawColumnHeader(DataGridViewCellPaintingEventArgs e)
        {
            int h = e.CellBounds.Height;
            int w = e.CellBounds.Width;
            int h1 = Convert.ToInt32(h * 0.4);
            CustomGridColorTable ct = new CustomGridColorTable();
            Rectangle r1 = new Rectangle(e.CellBounds.X, e.CellBounds.Y, w, h1);
            Rectangle r2 = new Rectangle(e.CellBounds.X, h1, w, h -h1 + 1);
            LinearGradientBrush lb1 = new LinearGradientBrush(r1, ct.ColumnHeaderStartColor, ct.ColumnHeaderMidColor1, LinearGradientMode.Vertical);
            LinearGradientBrush lb2 = new LinearGradientBrush(r2, ct.ColumnHeaderMidColor2, ct.ColumnHeaderEndColor, LinearGradientMode.Vertical);
            Pen p = new Pen(ct.GridColor, 1);
            StringFormat frmt = new StringFormat();
            frmt.Alignment = StringAlignment.Center;
            frmt.FormatFlags = StringFormatFlags.DisplayFormatControl;
            frmt.LineAlignment = StringAlignment.Center;
            e.Graphics.FillRectangle(lb1, r1);
            e.Graphics.FillRectangle(lb2, r2);
            e.Graphics.DrawRectangle(p, e.CellBounds);
        }
        #endregion

        #region Private
        void CustomGridCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Handled = true;
            if (e.RowIndex < 0)
                DrawColumnHeader(e);
            else
                DrawCell(e);

            e.PaintContent(e.CellBounds);
        }

        void CustomGridPaint(object sender, PaintEventArgs e)
        {
            int h = RowTemplate.Height;
            int rh = ColumnHeadersHeight + 1;
            int lh = GetRowDisplayRectangle(RowCount, false).Bottom;
            Rectangle r;
            CustomGridColorTable ct = new CustomGridColorTable();
            if (lh < Width)
            {
                for (int i = 0; lh + rh < Height; i += h)
                {
                    r = new Rectangle(0, i, Width, h);
                    e.Graphics.DrawRectangle(new Pen(ct.GridColor), r);
                }
            }
        }
        #endregion
        #endregion
    }
}