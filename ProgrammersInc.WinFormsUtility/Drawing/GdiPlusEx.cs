using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using ProgrammersInc.Utility.Win32;
using ProgrammersInc.Utility.Win32.Common;

namespace ProgrammersInc.WinFormsUtility.Drawing
{
    /// <summary>
    /// Clase para realizar operaciones de dibujo de textos.
    /// </summary>
	public static class GdiPlusEx
	{
        #region Methods
        /// <summary>
        /// Dibuja el texto especificado con la fuente, color y tipo de despliegue dados.
        /// </summary>
        /// <param name="g">Dispositivo de dibujo.</param>
        /// <param name="text">Texto a dibujarse.</param>
        /// <param name="font">Fuente del texto.</param>
        /// <param name="color">Color del texto.</param>
        /// <param name="rect">Area de contenido del texto.</param>
        /// <param name="textSplitting">Formato de visualización.</param>
        /// <param name="ampersands">Comportamiento del caracter ampersand.</param>
        public static void DrawString(Graphics g, string text, Font font, Color color, Rectangle rect, TextSplitting textSplitting, Ampersands ampersands)
        {
            DrawString(g, text, font, color, rect, Alignment.Left, VAlignment.Top, textSplitting, ampersands);
        }

        public static void DrawString(Graphics g, string text, Font font, Color color, Rectangle rect, Alignment alignment, VAlignment valignment, TextSplitting textSplitting, Ampersands ampersands)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");

            if (ampersands == Ampersands.Display)
                text = text.Replace("&", "&&");

            float[] txValues = g.Transform.Elements;
            IntPtr hClipRgn = g.Clip.GetHrgn(g);
            IntPtr hDC = g.GetHdc();

            Gdi.SelectClipRgn(hDC, hClipRgn);

            int oldGraphicsMode = Gdi.SetGraphicsMode(hDC, 2);
            XFORM oldXForm = new XFORM();

            Gdi.GetWorldTransform(hDC, ref oldXForm);

            XFORM newXForm = new XFORM();

            newXForm.eM11 = txValues[0];
            newXForm.eM12 = txValues[1];
            newXForm.eM21 = txValues[2];
            newXForm.eM22 = txValues[3];
            newXForm.eDx = txValues[4];
            newXForm.eDy = txValues[5];

            Gdi.SetWorldTransform(hDC, ref newXForm);

            try
            {
                IntPtr hFont = font.ToHfont();
                IntPtr hOldFont = Gdi.SelectObject(hDC, hFont);

                try
                {
                    RECT r = new RECT(rect);
                    User.DrawTextFlags uFormat;

                    switch (textSplitting)
                    {
                        case TextSplitting.SingleLineEllipsis:
                            uFormat = User.DrawTextFlags.DT_WORD_ELLIPSIS | User.DrawTextFlags.DT_END_ELLIPSIS;
                            break;
                        case TextSplitting.MultiLine:
                            uFormat = User.DrawTextFlags.DT_WORDBREAK;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    switch (alignment)
                    {
                        case Alignment.Left:
                            break;
                        case Alignment.Center:
                            uFormat = User.DrawTextFlags.DT_CENTER;
                            break;
                        case Alignment.Right:
                            uFormat = User.DrawTextFlags.DT_RIGHT;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    switch (valignment)
                    {
                        case VAlignment.Top:
                            break;
                        case VAlignment.Bottom:
                            uFormat |= User.DrawTextFlags.DT_BOTTOM | User.DrawTextFlags.DT_SINGLELINE;
                            break;
                        case VAlignment.Center:
                            uFormat |= User.DrawTextFlags.DT_VCENTER | User.DrawTextFlags.DT_SINGLELINE;
                            break;
                    }

                    uint bgr = (uint)((color.B << 16) | (color.G << 8) | (color.R));
                    uint oldColor = Gdi.SetTextColor(hDC, bgr);

                    try
                    {
                        BackgroundMode oldBackgroundMode = Gdi.SetBkMode(hDC, BackgroundMode.TRANSPARENT);

                        try
                        {
                            User.DrawText(hDC, text, text.Length, ref r, uFormat);
                        }
                        finally
                        {
                            Gdi.SetBkMode(hDC, oldBackgroundMode);
                        }
                    }
                    finally
                    {
                        Gdi.SetTextColor(hDC, oldColor);
                    }
                }
                finally
                {
                    Gdi.SelectObject(hDC, hOldFont);
                    Gdi.DeleteObject(hFont);
                }
            }
            finally
            {
                if (oldGraphicsMode == 1)
                {
                    oldXForm.eM11 = 1;
                    oldXForm.eM12 = 0;
                    oldXForm.eM21 = 0;
                    oldXForm.eM22 = 1;
                    oldXForm.eDx = 0;
                    oldXForm.eDx = 0;
                }

                Gdi.SetWorldTransform(hDC, ref oldXForm);
                Gdi.SetGraphicsMode(hDC, oldGraphicsMode);

                g.ReleaseHdc(hDC);

                if (hClipRgn != IntPtr.Zero)
                    g.Clip.ReleaseHrgn(hClipRgn);
            }
        }

        public static Size MeasureString(Graphics g, string text, Font font, int width)
        {
            Size size;
            TextDetails td = new TextDetails(text, font, width);

            if (_mapTextSizes.TryGetValue(td, out size))
                return size;

            IntPtr hDC = g.GetHdc();

            try
            {
                IntPtr hFont = font.ToHfont();

                try
                {
                    IntPtr hOldFont = Gdi.SelectObject(hDC, hFont);

                    try
                    {
                        Rectangle rect = new Rectangle(0, 0, width, 0);
                        RECT r = new RECT(rect);
                        User.DrawTextFlags uFormat = User.DrawTextFlags.DT_WORDBREAK | User.DrawTextFlags.DT_CALCRECT;

                        User.DrawText(hDC, text, text.Length, ref r, uFormat);

                        size = new Size(r.Right, r.Bottom);

                        _mapTextSizes[td] = size;

                        return size;
                    }
                    finally
                    {
                        Gdi.SelectObject(hDC, hOldFont);
                    }
                }
                finally
                {
                    Gdi.DeleteObject(hFont);
                }
            }
            finally
            {
                g.ReleaseHdc(hDC);
            }
        }

        public static void DrawRoundRect(Graphics g, Pen p, Rectangle rect, int radius)
        {
            DrawRoundRect(g, p, rect.X, rect.Y, rect.Width, rect.Height, radius);
        }

        public static void DrawRoundRect(Graphics g, Pen p, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            radius = Math.Min(radius, height / 2 - 1);
            radius = Math.Min(radius, width / 2 - 1);

            if (radius <= 0)
            {
                g.DrawRectangle(p, x, y, width, height);
                return;
            }

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(x + radius, y, x + width - (radius * 2), y);
                gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);
                gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2));
                gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
                gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height);
                gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
                gp.AddLine(x, y + height - (radius * 2), x, y + radius);
                gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                gp.CloseFigure();

                g.DrawPath(p, gp);
            }
        }

        public static void FillRoundRect(Graphics g, Brush b, Rectangle rect, int radius)
        {
            FillRoundRect(g, b, rect.X, rect.Y, rect.Width, rect.Height, radius);
        }

        public static void FillRoundRect(Graphics g, Brush b, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            radius = Math.Min(radius, height / 2);
            radius = Math.Min(radius, width / 2);

            if (radius == 0)
            {
                g.FillRectangle(b, x, y, width, height);
                return;
            }

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(x + radius, y, x + width - (radius * 2), y);
                gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);
                gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2));
                gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
                gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height);
                gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
                gp.AddLine(x, y + height - (radius * 2), x, y + radius);
                gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                gp.CloseFigure();

                g.FillPath(b, gp);
            }
        }

        public static IDisposable SaveState(Graphics g)
        {
            return new GraphicsStateDisposer(g);
        }

        /// <summary>
        /// Dibuja la imagen especificada en escala de grises.
        /// </summary>
        /// <param name="source">Imagen a dibujarse.</param>
        /// <returns>La imagen dibuja en escala de grises.</returns>
        public static Image MakeDisabledImage(Image source)
        {
            return MakeDisabledImage(source, SystemColors.GrayText);
        }

        public static Image MakeDisabledImage(Image source, Color greyColor)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            float w = (greyColor.R + greyColor.G + greyColor.B) / 765.0f;
            float r = greyColor.R / 255.0f / 2;
            float g = greyColor.G / 255.0f / 2;
            float b = greyColor.B / 255.0f / 2;

            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
				{
					new float[] { w, w, w, 0, 0 },
					new float[] { w, w, w, 0, 0 },
					new float[] { w, w, w, 0, 0 },
					new float[] { 0, 0, 0, 1, 0 },
					new float[] { r, g, b, 0, 1 }
				});

            ImageAttributes imageAttributes = new ImageAttributes();

            imageAttributes.SetColorMatrix(colorMatrix);

            Image disabled = (Image)source.Clone();

            using (Graphics graphics = Graphics.FromImage(disabled))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, disabled.Width, disabled.Width), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return disabled;
        }
        #endregion

        #region Enumerations
        #region Alignment
        /// <summary>
        /// Alineación del texto de la columna dentro del ColumnHeader.
        /// </summary>
        public enum Alignment
        {
            /// <summary>
            /// Alineación a la izquierda.
            /// </summary>
            Left,
            /// <summary>
            /// Alineación al centro.
            /// </summary>
            Center,
            /// <summary>
            /// Alineación a la derecha.
            /// </summary>
            Right
        }
        #endregion

        #region VAlignment
        /// <summary>
        /// Alineación vertical del texto.
        /// </summary>
        public enum VAlignment
        {
            /// <summary>
            /// Alineación vertical superior.
            /// </summary>
            Top,
            /// <summary>
            /// Alineación vertical al centro.
            /// </summary>
            Center,
            /// <summary>
            /// Alineación vertical inferior.
            /// </summary>
            Bottom
        }
        #endregion

        #region TextSplitting
        /// <summary>
        /// Forma en que se mostrará el texto.
        /// </summary>
        public enum TextSplitting
        {
            /// <summary>
            /// Linea simple.
            /// </summary>
            SingleLineEllipsis,
            /// <summary>
            /// Multilínea
            /// </summary>
            MultiLine
        }
        #endregion

        #region Ampersands
        /// <summary>
        /// Define el comportamiento del caracter ampersand.
        /// </summary>
        public enum Ampersands
        {
            /// <summary>
            /// Mostrarlo.
            /// </summary>
            Display,
            /// <summary>
            /// Crear acceso directo.
            /// </summary>
            MakeShortcut
        }
        #endregion
        #endregion

        #region Classes
        #region GraphicsStateDisposer
        sealed class GraphicsStateDisposer : IDisposable
        {
            #region Constructors
            internal GraphicsStateDisposer(Graphics g)
            {
                this.g = g;
                this.state = g.Save();
            }
            #endregion

            #region Merthods
            public void Dispose()
			{
				if( g != null )
				{
					g.Restore( state );
					g = null;
					state = null;
				}
			}
			#endregion

            #region Fields
            Graphics g;
			GraphicsState state;
            #endregion
        }
		#endregion

        #region TextDetails
        sealed class TextDetails
        {
            #region Constructors
            internal TextDetails(string text, Font font, int width)
            {
                this.text = text;
                this.font = font;
                this.width = width;
            }
            #endregion

            #region Methods
            public override int GetHashCode()
            {
                return text.GetHashCode() ^ font.GetHashCode() ^ width;
            }

            public override bool Equals(object obj)
            {
                TextDetails td = obj as TextDetails;

                if (td == null)
                {
                    return false;
                }

                return text == td.text && font.Equals(td.font) && width == td.width;
            }
            #endregion

            #region Fields
            string text;
            Font font;
            int width;
            #endregion
        }
        #endregion
        #endregion

        #region Fields
        static Dictionary<TextDetails, Size> _mapTextSizes = new Dictionary<TextDetails, Size>();
        #endregion
    }
}
