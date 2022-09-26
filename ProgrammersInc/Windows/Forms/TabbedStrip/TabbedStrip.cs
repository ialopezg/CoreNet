using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Proporciona un contenedor para los objetos de barra de herramientas 
    /// de Windows personalizable al estilo páginas de contenido.
    /// </summary>
    [ToolboxBitmap(typeof(ToolStrip))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Proporciona un contenedor para los objetos de barra de herramientas de Windows personalizable al estilo páginas de contenido.")]
    public class TabbedStrip : ToolStrip
    {
        private TabStripRenderer renderer = new TabStripRenderer();
        DesignerVerb insPage = null;

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.TabbedStrip"/>
        /// </summary>
        public TabbedStrip()
            : base()
        {
            InitControl();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.TabbedStrip"/>
        /// con la matriz específicada de <see cref="ProgrammersInc.Windows.Forms.TabStripButton"/>.
        /// </summary>
        /// <param name="buttons">Matríz de objetos <see cref="ProgrammersInc.Windows.Forms.TabStripButton"/>.</param>
        public TabbedStrip(params TabStripButton[] buttons)
            : base(buttons)
        {
            InitControl();
        }
        #endregion

        #region Properties
        public override ISite Site
        {
            get
            {
                ISite site = base.Site;
                if (site != null && site.DesignMode)
                {
                    IContainer comp = site.Container;
                    if (comp != null)
                    {
                        IDesignerHost host = comp as IDesignerHost;
                        if (host != null)
                        {
                            IDesigner designer = host.GetDesigner(site.Component);
                            if (designer != null && !designer.Verbs.Contains(insPage))
                                designer.Verbs.Add(insPage);
                        }
                    }
                }
                return site;
            }
            set
            {
                base.Site = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el Render personalizado para el TabStrip. a operación establecer no tiene efecto.
        /// </summary>
        public new ToolStripRenderer Renderer
        {
            get { return renderer; }
            set { base.Renderer = renderer; }
        }

        /// <summary>
        /// Obtiene o establece el estilo de diseño para un control TabStrip.
        /// </summary>
        public new ToolStripLayoutStyle LayoutStyle
        {
            get { return base.LayoutStyle; }
            set
            {
                switch (value)
                {
                    case ToolStripLayoutStyle.StackWithOverflow:
                    case ToolStripLayoutStyle.HorizontalStackWithOverflow:
                    case ToolStripLayoutStyle.VerticalStackWithOverflow:
                        base.LayoutStyle = ToolStripLayoutStyle.StackWithOverflow;
                        break;
                    case ToolStripLayoutStyle.Table:
                        base.LayoutStyle = ToolStripLayoutStyle.Table;
                        break;
                    case ToolStripLayoutStyle.Flow:
                        base.LayoutStyle = ToolStripLayoutStyle.Flow;
                        break;
                    default:
                        base.LayoutStyle = ToolStripLayoutStyle.StackWithOverflow;
                        break;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el modo del Render para el control.
        /// </summary>
        [Obsolete("Use RenderStyle instead")]
        [Browsable(false)]
        public new ToolStripRenderMode RenderMode
        {
            get { return base.RenderMode; }
            set { RenderStyle = value; }
        }

        /// <summary>
        /// Obtiene o establece el estilo del Render para el control.
        /// </summary>
        [Category("Appearance")]
        [Description("Obtiene o establece el estilo del Render para el control.")]
        public ToolStripRenderMode RenderStyle
        {
            get { return renderer.RenderMode; }
            set
            {
                renderer.RenderMode = value;
                this.Invalidate();
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return Padding.Empty;
            }
        }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return DefaultPadding; }
            set { }
        }

        /// <summary>
        /// Obtiene o establece si el control usará los estilos vivuales del SO para el dibujado de los elementos.
        /// </summary>
        [Category("Appearance")]
        [Description("Específica si el control usará los estilos vivuales del SO para el dibujado de los elementos.")]
        public bool UseVisualStyles
        {
            get { return renderer.UseVisualStyles; }
            set
            {
                renderer.UseVisualStyles = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Obtiene o establece si los TabButtons podrían ser dibujados volteados.
        /// </summary>
        [Category("Appearance")]
        [Description("Específica si los TabButtons podrían ser dibujados volteados.)")]
        public bool FlipButtons
        {
            get { return renderer.Mirrored; }
            set
            {
                renderer.Mirrored = value;
                this.Invalidate();
            }
        }

        protected TabStripButton selectedTab;
        /// <summary>
        /// Obtiene o establece el elemento <see cref="TabStripButton"/> actualmente seleccionado.
        /// </summary>
        public TabStripButton SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if (value == null)
                    return;
                if (selectedTab == value)
                    return;
                if (value.Owner != this)
                    throw new ArgumentException("Cannot select TabButtons that do not belong to this TabStrip");
                OnItemClicked(new ToolStripItemClickedEventArgs(value));
            }
        }
        #endregion

        #region Methos Implementation
        #region Protected
        protected void InitControl()
        {
            base.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            base.Renderer = renderer;
            renderer.RenderMode = this.RenderStyle;
            insPage = new DesignerVerb("Insertar un elemento Tab", new EventHandler(OnInsertPageClicked));
        }

        protected void OnInsertPageClicked(object sender, EventArgs e)
        {
            ISite site = base.Site;
            if (site != null && site.DesignMode)
            {
                IContainer container = site.Container;
                if (container != null)
                {
                    TabStripButton btn = new TabStripButton();
                    container.Add(btn);
                    btn.Text = btn.Name;
                }
            }
        }
        #endregion

        #region Protected Override
        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);
            if (e.Item is TabStripButton)
                SelectedTab = (TabStripButton)e.Item;
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            TabStripButton clickedBtn = e.ClickedItem as TabStripButton;
            if (clickedBtn != null)
            {
                this.SuspendLayout();
                selectedTab = clickedBtn;
                this.ResumeLayout();
                OnTabSelected(clickedBtn);
            }
            base.OnItemClicked(e);
        }

        protected void OnTabSelected(TabStripButton tab)
        {
            this.Invalidate();
            if (SelectedTabChanged != null)
                SelectedTabChanged(this, new SelectedTabChangedEventArgs(tab));
        }
        #endregion
        #endregion

        #region Events
        public event EventHandler<SelectedTabChangedEventArgs> SelectedTabChanged;
        #endregion
    }
}