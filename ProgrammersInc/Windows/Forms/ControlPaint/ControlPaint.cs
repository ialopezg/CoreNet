using System;
using System.Drawing;

namespace ProgrammersInc.Windows.Forms
{
    internal sealed class ControlPaint
    {
        /// <summary>
        /// Color del borde del control en estado normal.
        /// </summary>
        public static Color BorderColor
        {
            get { return Color.FromArgb(127, 157, 185); }
        }

        /// <summary>
        /// Color del borde del control cuando en estado desactivado.
        /// </summary>
        public static Color DisabledBorderColor
        {
            get { return Color.FromArgb(201, 199, 186); }
        }

        /// <summary>
        /// Color del borde de un boton en estado normal.
        /// </summary>
        public static Color ButtonBorderColor
        {
            get { return Color.FromArgb(28, 81, 128); }
        }

        /// <summary>
        /// Color del borde de un boton en estado dasactivado.
        /// </summary>
        public static Color DisabledButtonBorderColor
        {
            get { return Color.FromArgb(202, 200, 187); }
        }

        /// <summary>
        /// Color del fondo en estado desactidado.
        /// </summary>
        public static Color DisabledBackColor
        {
            get { return Color.FromArgb(236, 233, 216); }
        }

        /// <summary>
        /// Color de la fuente en estado desactivado.
        /// </summary>
        public static Color DisabledForeColor
        {
            get { return Color.FromArgb(161, 161, 146); }
        }

        /// <summary>
        /// Dibuja el borde de un control, en base a los valores de los parámetros dados.
        /// </summary>
        /// <param name="g">Objeto que se usará para hacer el dibujo.</param>
        /// <param name="x">Coordenada x de donde iniciará el área de dibujo.</param>
        /// <param name="y">Coordenada y de donde iniciará el área de dibujo.</param>
        /// <param name="width">Ancho del área de dibujo.</param>
        /// <param name="height">Alto del área de dibujo.</param>
        public static void DrawBorder(Graphics g, int x, int y, int width, int height)
        {
            g.DrawRectangle(new Pen(ControlPaint.BorderColor, 0), x, y,
                width, height);
        }
    }
}