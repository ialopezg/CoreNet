using System;
using System.Drawing;
using System.Drawing.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Establece el <see cref="TextRenderingHint.ClearTypeGridFit"/> hasta que la instancia se destruya.
    /// </summary>
    public class UseClearTypeGridFit : IDisposable
    {
        #region " Instance Fields "
        private Graphics g;
        private TextRenderingHint old;
        #endregion

        #region " Constructors && Destructors "
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UseClearTypeGridFit"/>.
        /// </summary>
        /// <param name="g">Objeto Graphics a implementarse.</param>
        public UseClearTypeGridFit(Graphics g)
        {
            this.g = g;
            old = g.TextRenderingHint;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        }

        /// <summary>
        /// Revierte el TextRenderingHint a su configuración original.
        /// </summary>
        public void Dispose()
        {
            g.TextRenderingHint = old;
        }
        #endregion
    }
}