using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Clase que expone métodos auxiliares de dibujo.
    /// </summary>
    internal static class GraphicsTools
    {
        /// <summary>
        /// Crea un rectángulo con las esquinas redondeadas en el rectángulo y radio
        /// especificados.
        /// </summary>
        /// <param name="rectangle">Rectángulo base.</param>
        /// <param name="radius">Radio de rendondeado para las esquinas.</param>
        /// <returns>El rectángulo dado con las esquinas redondeadas como un
        /// objeto <see cref="System.Drawing.Drawing2D.GraphicsPath"/>.</returns>
        public static GraphicsPath CreateRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            path.AddArc(l, t, d, d, 180, 90); // topleft
            path.AddLine(l + radius, t, l + w - radius, t); // top
            path.AddArc(l + w - d, t, d, d, 270, 90); // topright
            path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
            path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
            path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
            path.AddLine(l, t + h - radius, l, t + radius); // left
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Crea un rectágulo con las esquinas superiores redondeadas.
        /// </summary>
        /// <param name="rectangle">Rectángulo base.</param>
        /// <param name="radius">Radio para las esquinas a redondear.</param>
        /// <returns>Un rectágulo con la esquinas superiores redondeadas como un
        /// objeto <see cref="System.Drawing.Drawing2D.GraphicsPath"/>.</returns>
        public static GraphicsPath CreateTopRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            path.AddArc(l, t, d, d, 180, 90); // topleft
            path.AddLine(l + radius, t, l + w - radius, t); // top
            path.AddArc(l + w - d, t, d, d, 270, 90); // topright
            path.AddLine(l + w, t + radius, l + w, t + h); // right
            path.AddLine(l + w, t + h, l, t + h); // bottom
            path.AddLine(l, t + h, l, t + radius); // left
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Crea un rectágulo con las esquinas inferiores redondeadas.
        /// </summary>
        /// <param name="rectangle">Rectángulo base.</param>
        /// <param name="radius">Radio para las esquinas a redondear.</param>
        /// <returns>Un rectágulo con la esquinas inferiores redondeadas como un
        /// objeto <see cref="System.Drawing.Drawing2D.GraphicsPath"/>.</returns>
        public static GraphicsPath CreateBottomRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            path.AddLine(l + radius, t, l + w - radius, t); // top
            path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
            path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
            path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
            path.AddLine(l, t + h - radius, l, t + radius); // left
            path.CloseFigure();

            return path;
        }
    }
}