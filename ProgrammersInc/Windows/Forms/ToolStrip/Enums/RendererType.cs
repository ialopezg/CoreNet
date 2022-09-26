using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Define los tipos de render que podrían soportar los objetos del tipo
    /// <see cref="ProgrammersInc.Windows.Forms.ContextualMenu"/>, 
    /// <see cref="ProgrammersInc.Windows.Forms.CustomToolStrip"/>.
    /// </summary>
	public enum RendererType
	{
        /// <summary>
        /// Sin render.
        /// </summary>
        None = 0,
        /// <summary>
        /// Render al estilo Microsof Office 2007.
        /// </summary>
        Office2007,
        /// <summary>
        /// Paleta de colores de Windows Vista.
        /// </summary>
        Vista
	};
}
