using System;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Estructura que define un elemento del control <see cref="SummaryBox"/>.
    /// </summary>
    public struct SummaryBoxItem
    {
        /// <summary>
        /// El encabezado de este elemento.
        /// </summary>
        public string Header;
        /// <summary>
        /// La descripción de este elemento.
        /// </summary>
        public string Summary;
        /// <summary>
        /// Información relacionada a este elemento.
        /// </summary>
        public string Tag;

        /// <summary>
        /// Crea una instancia de la estructura.
        /// </summary>
        /// <param name="header">Encabezado del elemento.</param>
        /// <param name="summary">Sumario del elemento.</param>
        public SummaryBoxItem(string header, string summary) : this(header, summary, null) { }

        /// <summary>
        /// Crea una instancia de la estructura.
        /// </summary>
        /// <param name="header">Encabezado del elemento.</param>
        /// <param name="summary">Sumario del elemento.</param>
        /// <param name="tag">Tag del elemento.</param>
        public SummaryBoxItem(string header, string summary, string tag)
        {
            Header = header;
            Summary = summary;
            Tag = tag;
        }
    }
}