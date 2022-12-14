using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
  public partial class CommandButton : Button
  {
    enum eButtonState
    {
        Normal,
        MouseOver,
        Down
    }

    eButtonState m_State = eButtonState.Normal;

    Image imgArrow1 = null;
    Image imgArrow2 = null;

    // make sure the control is invalidated(repainted) when the text is changed
    public override string Text
    {
        get { return base.Text; }
        set
        {
            base.Text = value;
            this.Invalidate();
        }
    }

    Font m_smallFont;
    public Font SmallFont { get { return m_smallFont; } set { m_smallFont = value; } }

    bool m_autoHeight = true;
    public bool AutoHeight { get { return m_autoHeight; } set { m_autoHeight = value; if (m_autoHeight) this.Invalidate(); } }

    //--------------------------------------------------------------------------------
    public CommandButton()
    {
      InitializeComponent();
      base.Font = new Font("Arial", 11.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
      m_smallFont = new Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
    }

    //--------------------------------------------------------------------------------
    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      imgArrow1 = CommandButtonResources.GreenArrow1;
      imgArrow2 = CommandButtonResources.GreenArrow2;
    }

    //--------------------------------------------------------------------------------
    const int LEFT_MARGIN = 10;
    const int TOP_MARGIN = 10;
    const int ARROW_WIDTH = 19;

    string GetLargeText()
    {
      string[] lines = this.Text.Split(new char[] { '\n' });
      return lines[0];
    }

    string GetSmallText()
    {
      if (this.Text.IndexOf('\n') < 0)
        return "";
      
      string s = this.Text;
      string[] lines = s.Split(new char[] { '\n' });
      s = "";
      for (int i = 1; i < lines.Length; i++)
        s += lines[i] + "\n";
      return s.Trim(new char[] { '\n' });
    }

    SizeF GetLargeTextSizeF()
    {
      int x = LEFT_MARGIN + ARROW_WIDTH + 5;
      SizeF mzSize = new SizeF(this.Width - x - LEFT_MARGIN, 5000.0F);  // presume RIGHT_MARGIN = LEFT_MARGIN
      Graphics g = Graphics.FromHwnd(this.Handle);
      SizeF textSize = g.MeasureString(GetLargeText(), base.Font, mzSize);
      return textSize;
    }

    SizeF GetSmallTextSizeF()
    {
      string s = GetSmallText();
      if (s == "") return new SizeF(0, 0);
      int x = LEFT_MARGIN + ARROW_WIDTH + 8; // <- indent small text slightly more
      SizeF mzSize = new SizeF(this.Width - x - LEFT_MARGIN, 5000.0F);  // presume RIGHT_MARGIN = LEFT_MARGIN
      Graphics g = Graphics.FromHwnd(this.Handle);
      SizeF textSize = g.MeasureString(s, m_smallFont, mzSize);
      return textSize;
    }

    public int GetBestHeight()
    {
      //return 40;
      return (TOP_MARGIN * 2) + (int)GetSmallTextSizeF().Height + (int)GetLargeTextSizeF().Height;
    }

    //--------------------------------------------------------------------------------
    protected override void OnPaint(PaintEventArgs e)
    {
      e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

      LinearGradientBrush brush;
      LinearGradientMode mode = LinearGradientMode.Vertical;

      Rectangle newRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
      Color text_color = SystemColors.WindowText;

      Image img = imgArrow1;

      if (Enabled)
      {
        switch (m_State)
        {
          case eButtonState.Normal:
            e.Graphics.FillRectangle(Brushes.White, newRect);
            if (base.Focused)
              e.Graphics.DrawRectangle(new Pen(Color.SkyBlue, 1), newRect);
            else
              e.Graphics.DrawRectangle(new Pen(Color.White, 1), newRect);
            text_color = Color.DarkBlue;
            break;

          case eButtonState.MouseOver:
            brush = new LinearGradientBrush(newRect, Color.White, Color.WhiteSmoke, mode);
            e.Graphics.FillRectangle(brush, newRect);
            e.Graphics.DrawRectangle(new Pen(Color.Silver, 1), newRect);
            img = imgArrow2;
            text_color = Color.Blue;
            break;

          case eButtonState.Down:
            brush = new LinearGradientBrush(newRect, Color.WhiteSmoke, Color.White, mode);
            e.Graphics.FillRectangle(brush, newRect);
            e.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1), newRect);
            text_color = Color.DarkBlue;
            break;
        }
      }
      else
      {
        brush = new LinearGradientBrush(newRect, Color.WhiteSmoke, Color.Gainsboro, mode);
        e.Graphics.FillRectangle(brush, newRect);
        e.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1), newRect);
        text_color = Color.DarkBlue;
      }


      string largetext = this.GetLargeText();
      string smalltext = this.GetSmallText();

      SizeF szL = GetLargeTextSizeF();
      e.Graphics.DrawString(largetext, base.Font, new SolidBrush(text_color), new RectangleF(new PointF(LEFT_MARGIN + imgArrow1.Width + 5, TOP_MARGIN), szL));

      if (smalltext != "")
      {
        SizeF szS = GetSmallTextSizeF();
        e.Graphics.DrawString(smalltext, m_smallFont, new SolidBrush(text_color), new RectangleF(new PointF(LEFT_MARGIN + imgArrow1.Width + 8, TOP_MARGIN + (int)szL.Height), szS));
      }

      e.Graphics.DrawImage(img, new Point(LEFT_MARGIN, TOP_MARGIN + (int)(szL.Height/2) - (int)(img.Height/2)));
    }

    //--------------------------------------------------------------------------------
    protected override void OnMouseLeave(System.EventArgs e)
    {
      m_State = eButtonState.Normal;
      this.Invalidate();
      base.OnMouseLeave(e);
    }

    //--------------------------------------------------------------------------------
    protected override void OnMouseEnter(System.EventArgs e)
    {
      m_State = eButtonState.MouseOver;
      this.Invalidate();
      base.OnMouseEnter(e);
    }

    //--------------------------------------------------------------------------------
    protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
      m_State = eButtonState.MouseOver;
      this.Invalidate();
      base.OnMouseUp(e);
    }

    //--------------------------------------------------------------------------------
    protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      m_State = eButtonState.Down;
      this.Invalidate();
      base.OnMouseDown(e);
    }

    //--------------------------------------------------------------------------------
    protected override void OnSizeChanged(EventArgs e)
    {
      if (m_autoHeight)
      {
        int h = GetBestHeight();
        if (this.Height != h)
        {
          this.Height = h;
          return;
        }
      }
      base.OnSizeChanged(e);
    }

    //--------------------------------------------------------------------------------
  }
}
