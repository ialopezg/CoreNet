using System;
using System.Collections;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Extiende el comportamiento del control <see cref="Splitter"/>, en modo diseño.
    /// </summary>
    public class SplitterDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SplitterDesigner"/>.
        /// </summary>
        public SplitterDesigner() { }
        #endregion

        #region Methods
        /// <summary>
        /// Prepara las propiedades para el control.
        /// </summary>
        /// <param name="properties">Listado de propiedades admitidas.</param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            properties.Remove("IsCollapsed");
            properties.Remove("BorderStyle");
            properties.Remove("Size");
        }
        #endregion
    }
}