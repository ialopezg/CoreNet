using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control de barra de estado de Windows con efectos adicionales.
    /// </summary>
    [ToolboxBitmap(typeof(StatusStrip))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Representa un control de barra de estado de Windows con efectos adicionales.")]
    public class StatusStrip : System.Windows.Forms.StatusStrip
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.CustomStatusStrip"/>.
        /// </summary>
        public StatusStrip() :base() { }

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
                    GripStyle = ToolStripGripStyle.Hidden;
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