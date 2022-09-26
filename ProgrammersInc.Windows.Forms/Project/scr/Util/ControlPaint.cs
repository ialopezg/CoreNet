using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProgrammersInc.Windows.Forms
{
    internal sealed class ControlPaint
    {
        public static Color BorderColor
        {
            get { return Color.FromArgb(127, 157, 185); }
        }

        public static Color DisabledBorderColor
        {
            get { return Color.FromArgb(201, 199, 186); }
        }

        public static Color ButtonBorderColor
        {
            get { return Color.FromArgb(28, 81, 128); }
        }

        public static Color DisabledButtonBorderColor
        {
            get { return Color.FromArgb(202, 200, 187); }
        }

        public static Color DisabledBackColor
        {
            get { return Color.FromArgb(236, 233, 216); }
        }

        public static Color DisabledForeColor
        {
            get { return Color.FromArgb(161, 161, 146); }
        }

        public static void DrawBorder(Graphics g, int x, int y, int width, int height)
        {
            g.DrawRectangle(new Pen(ControlPaint.BorderColor, 0), x, y,
                width, height);
        }
    }
}
