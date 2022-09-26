using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Establece la region clippin hasta que la instancia se destruya.
    /// </summary>
    public class UseClipping : IDisposable
    {
        #region " Instance Fields "
        private Graphics _g;
        private Region _old;
        #endregion

        #region " Constructor && Destructor "
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UseClipping"/>.
        /// </summary>
        /// <param name="g">Graphics a implementarse.</param>
        /// <param name="path">Path Clipping.</param>
        public UseClipping(Graphics g, GraphicsPath path)
        {
            _g = g;
            _old = g.Clip;
            Region clip = _old.Clone();
            clip.Intersect(path);
            _g.Clip = clip;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UseClipping"/>.
        /// </summary>
        /// <param name="g">Graphics a implementarse.</param>
        /// <param name="region">Region Clipping.</param>
        public UseClipping(Graphics g, Region region)
        {
            _g = g;
            _old = g.Clip;
            Region clip = _old.Clone();
            clip.Intersect(region);
            _g.Clip = clip;
        }

        /// <summary>
        /// Revierte clipping a su configuracion original.
        /// </summary>
        public void Dispose()
        {
            _g.Clip = _old;
        }
        #endregion
    }

}
