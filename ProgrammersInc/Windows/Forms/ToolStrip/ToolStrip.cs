using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Proporciona un contenedor para los objetos de barra de herramientas 
    /// de Windows personalizable.
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.ToolStrip))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Control que define un ToolStrip que podra ser personalidado.")]
    public class ToolStrip : System.Windows.Forms.ToolStrip
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase ProgrammersInc.Windows.Forms.Toolbar.
        /// </summary>
        public ToolStrip()
            : base() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase System.Windows.Forms.ToolStrip
        /// con la matriz especificada de <see cref="System.Windows.Forms.ToolStripItem"/>.
        /// </summary>
        /// <param name="items">Matriz de objetos <see cref="System.Windows.Forms.ToolStripItem"/>.</param>
        public ToolStrip(params System.Windows.Forms.ToolStripItem[] items)
            : base(items) { }
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
                    GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
                    break;
                case RendererType.Vista:
                    Renderer = new WindowsVistaRenderer();
                    GripStyle = ToolStripGripStyle.Hidden;
                    break;
            }
            Invalidate();
        }
        #endregion
    }
}
