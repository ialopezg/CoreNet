using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    /// <summary>
    /// Representa un menú contextual, con efectos especiales.
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.ContextMenuStrip))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Control que define un ToolStrip que podra ser personalizado.")]
    public class ContextualMenu : ContextMenuStrip
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ContextualMenu"/>.
        /// </summary>
        public ContextualMenu() : base() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ContextualMenu"/>.
        /// </summary>
        /// <param name="container">Un componente que implementa <see cref="System.ComponentModel.IContainer"/>, que es, el contenedor del control <see cref="System.Windows.Forms.ContextMenuStrip"/>.</param>
        public ContextualMenu(IContainer container) : base(container) { }
        #endregion

        #region Properties
        RendererType rendererType = RendererType.None;
        /// <summary>
        /// Obtiene o establece el tipo de renderizado a aplicarse al control actual.
        /// </summary>
        [Category("Appearance")]
        [Description("Obtiene o establece el tipo de renderizado a aplicarse al control actual.")]
        public RendererType RendererType
        {
            get { return rendererType; }
            set
            {
                if (rendererType != value)
                {
                    rendererType = value;
                    Refresh();
                }
            }
        }
        #endregion

        #region Methods Implementation
        /// <summary>
        /// Obliga al control a invalidar su área de cliente, y acto seguido, obliga
        /// a que vuelva a dibujarse el control y sus controles secundarios.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            switch (rendererType)
            {
                case RendererType.Office2007:
                    Renderer = new Office2007Renderer();
                    break;
                case RendererType.Vista:
                    Renderer = new WindowsVistaRenderer();
                    break;
            }
            Invalidate();
        }
        #endregion
    }
}
