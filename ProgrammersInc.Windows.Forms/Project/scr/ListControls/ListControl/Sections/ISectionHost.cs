using System;
using System.Drawing;

namespace ProgrammersInc.Windows.Forms
{
    public interface ISectionHost
    {
        #region Properties
        /// <summary>
        /// Devuelve la fuente para el Host.
        /// </summary>
        Font Font { get; }

        /// <summary>
        /// Devuelve <c>true</c> si se esta en una operación de arrastrar.
        /// </summary>
        bool IsInDragOperation { get; }

        /// <summary>
        /// Devuelve la sección sobre la cual está el puntero del Mouse (si hay alguna).
        /// </summary>
        Section SectionMouseOver { get;  }

        /// <summary>
        /// Devuelve cualquier objeto adjuntado (etiquetado) a este Host.
        /// </summary>
        object Tag { get; }

        /// <summary>
        /// Devuelve el color de texto para el Host.
        /// </summary>
        Color TextColor { get; }
        #endregion
    }
}