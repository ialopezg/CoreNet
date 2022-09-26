using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un botón Tab para un control <see cref="TabbedStrip"/>.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class TabStripButton : ToolStripButton
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.
        /// </summary>
        public TabStripButton()
            : base()
        {
            InitButton();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>
        /// que muestra la imagen especificada.
        /// </summary>
        /// <param name="image">Imagen que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        public TabStripButton(Image image)
            : base(image)
        {
            InitButton();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>
        /// que muestra el texto especificado.
        /// </summary>
        /// <param name="text">Texto que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        public TabStripButton(string text)
            : base(text) 
        {
            InitButton(); 
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>
        /// que muestra la imagen y el texto especificados.
        /// </summary>
        /// <param name="text">Texto que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        /// <param name="image">Imagen que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        public TabStripButton(string text, Image image) 
            : base(text, image)
        { 
            InitButton(); 
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>
        /// que muestra la imagen y el texto especificados, y que provoca el evento <see cref="System.Windows.Forms.ToolStripItem.Click"/>.
        /// </summary>
        /// <param name="Text">Texto que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        /// <param name="Image">Imagen que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        /// <param name="Handler">Un controlador de eventos que provoca el evento <see cref="System.Windows.Forms.ToolStripItem.Click"/>.</param>
        public TabStripButton(string Text, Image Image, EventHandler Handler)
            : base(Text, Image, Handler)
        {
            InitButton(); 
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>
        /// con el nombre especificado que muestra la imagen y el texto especificados,
        /// y que provoca el evento <see cref="System.Windows.Forms.ToolStripItem.Click"/>.
        /// </summary>
        /// <param name="Text">Texto que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        /// <param name="Image">Imagen que se va a mostrar en el <see cref="ProgrammersInc.Windows.Forms.ToolStripButton"/>.</param>
        /// <param name="Handler">Un controlador de eventos que provoca el evento <see cref="System.Windows.Forms.ToolStripItem.Click"/>.</param>
        /// <param name="name">Nombre de <see cref="System.Windows.Forms.ToolStripButton"/>.</param>
        public TabStripButton(string Text, Image Image, EventHandler Handler, string name)
            : base(Text, Image, Handler, name)
        {
            InitButton(); 
        }
        #endregion

        #region Methods Implementation
        #region Protected Overrride
        protected override void OnOwnerChanged(EventArgs e)
        {
            if (Owner != null && !(Owner is TabbedStrip))
                throw new Exception("Cannot add TabStripButton to " + Owner.GetType().Name);
            base.OnOwnerChanged(e);
        }
        #endregion

        #region Public Override
        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size sz = base.GetPreferredSize(constrainingSize);
            if (this.Owner != null && this.Owner.Orientation == Orientation.Vertical)
            {
                sz.Width += 3;
                sz.Height += 10;
            }
            return sz;
        }
        #endregion

        #region Private
        void InitButton()
        {
            selectedFont = this.Font;
        }
        #endregion
        #endregion

        #region Properties
        protected override Padding DefaultMargin
        {
            get
            {
                return new Padding(0);
            }
        }

        [Browsable(false)]
        public new Padding Margin
        {
            get { return base.Margin; }
            set { }
        }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { }
        }

        Color hotTextColor = Control.DefaultForeColor;
        [Category("Appearance")]
        [Description("El color del TabButton cuando este activo.")]
        public Color HotTextColor
        {
            get { return hotTextColor; }
            set { hotTextColor = value; }
        }

        Color selectedTextColor = Control.DefaultForeColor;
        [Category("Appearance")]
        [Description("Color del texto cuando TabButton este seleccionado.")]
        public Color SelectedTextColor
        {
            get { return selectedTextColor; }
            set { selectedTextColor = value; }
        }

        private Font selectedFont;
        [Category("Appearance")]
        [Description("Fuente usada cuando el TabButton esta seleccionado.")]
        public Font SelectedFont
        {
            get { return (selectedFont == null) ? this.Font : selectedFont; }
            set { selectedFont = value; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public new bool Checked
        {
            get { return IsSelected; }
            set { }
        }

        /// <summary>
        /// Obtiene o establece si el control TabButton actualmente está seleccionado.
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                TabbedStrip owner = Owner as TabbedStrip;
                if (owner != null)
                    return (this == owner.SelectedTab);
                return false;
            }
            set
            {
                if (value == false) return;
                TabbedStrip owner = Owner as TabbedStrip;
                if (owner == null) return;
                owner.SelectedTab = this;
            }
        }
        #endregion
    }
}