using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms
{
    /// <summary>
    /// Establece el SmoothingMode = AntiAlias hasta que la instancia se destruya.
    /// </summary>
    public class UseAntiAlias : IDisposable
    {
        #region " Instance Fields "
        private Graphics g;
        private SmoothingMode old;
        #endregion

        #region " Constructors && Destructors "
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UseAntiAlias"/>.
        /// </summary>
        /// <param name="g">Objeto Graphics a implementarse.</param>
        public UseAntiAlias(Graphics g)
        {
            this.g = g;
            this.old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        /// <summary>
        /// Revierte el SmoothingMode a su configuracio original.
        /// </summary>
        public void Dispose()
        {
            g.SmoothingMode = old;
        }
        #endregion
    }
}