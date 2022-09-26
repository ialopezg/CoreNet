using System;
using System.Drawing;

namespace ProgrammersInc.Windows.Forms
{
    public class CustomGridColorTable
    {
        public Color ColumnHeaderStartColor
        {
            get { return Color.FromArgb(245, 249, 251); }
        }

        public Color ColumnHeaderMidColor1
        {
            get { return Color.FromArgb(234, 239, 245); }
        }

        public Color ColumnHeaderMidColor2
        {
            get { return Color.FromArgb(224, 231, 240); }
        }

        public Color ColumnHeaderEndColor
        {
            get { return Color.FromArgb(212, 220, 233); }
        }

        public Color ColumnHeaderActiveStartColor
        {
            get { return Color.FromArgb(248, 214, 152); }
        }

        public Color ColumnHeaderActiveMidColor1
        {
            get { return Color.FromArgb(246, 207, 131); }
        }

        public Color ColumnHeaderActiveMidColor2
        {
            get { return Color.FromArgb(244, 201, 117); }
        }

        public Color ColumnHeaderActiveEndColor
        {
            get { return Color.FromArgb(242, 195, 99); }
        }

        public Color GridColor
        {
            get { return Color.FromArgb(208, 215, 229); }
        }

        public Color DefaultCellColor
        {
            get { return Color.FromArgb(255, 255, 255); }
        }

        public Color ActiveCellColor
        {
            get { return Color.FromArgb(135, 169, 213); }
        }

        public Color ReadonlyCellColor
        {
            get { return Color.FromArgb(138, 138, 138); }
        }

        public Color ActiveBorderColor
        {
            get { return Color.FromArgb(255, 189, 105); }
        }
    }
}