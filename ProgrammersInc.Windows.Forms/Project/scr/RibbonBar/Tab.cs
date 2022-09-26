using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Tab is a specialized ToolStripButton with extra padding
    /// </summary>
    [DesignerCategory("code")]
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
    public class Tab : ToolStripButton {
        
        private TabStripPage tabStripPage;

        /// <summary>
        /// Variable que determina si el objeto actual esta habilitado.
        /// </summary>
        public bool b_on = false;
        /// <summary>
        /// Variable que determina si el objeto actual esta o no seleccionado.
        /// </summary>
        public bool b_selected = false;
        /// <summary>
        /// Variable que determina si el objeto actual está activo.
        /// </summary>
        public bool b_active = false;
        /// <summary>
        /// Variable que determina si el objeto soportorá efectos de desvanecimiento.
        /// </summary>
        public bool b_fading = true;
        /// <summary>
        /// Un valor que indica el nivel de opacidad del control.
        /// </summary>
        public int o_opacity = 180;
        /// <summary>
        /// Un valor que indica el nivel de opacidad del control.
        /// </summary>
        public int e_opacity = 40;
        /// <summary>
        /// Un valor que indica el nivel de opacidad del control.
        /// </summary>
        public int i_opacity;
        private Timer timer = new Timer();

        #region Contructors && Initialization
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>.
        /// </summary>
        public Tab() 
        {
            Initialize();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>,
        /// en base a los parámetros dados.
        /// </summary>
        /// <param name="text">Texto que se mostrará en el elemento.</param>
        public Tab(string text)
            : base(text, null, null)
        {
            Initialize();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>,
        /// en base a los parámetros dados.
        /// </summary>
        /// <param name="image">Imagen que se mostrará en el elemento.</param>
        public Tab(Image image)
            : base(null, image, null)
        {
            Initialize();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>,
        /// en base a los parámetros dados.
        /// </summary>
        /// <param name="text">Texto que se mostrará en el elemento.</param>
        /// <param name="image">Imagen que se mostrará en el elemento.</param>
        public Tab(string text, Image image)
            : base(text, image, null)
        {
            Initialize();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>,
        /// en base a los parámetros dados.
        /// </summary>
        /// <param name="text">Texto que se mostrará en el elemento.</param>
        /// <param name="image">Imagen que se mostrará en el elemento.</param>
        /// <param name="onClick">Un controlador de eventos que provoca el evento
        /// <see cref="System.Windows.Forms.ToolStripItem.Click"/>.</param>
        public Tab(string text, Image image, EventHandler onClick)
            : base(text, image, onClick)
        {
            Initialize();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.Tab"/>,
        /// en base a los parámetros dados.
        /// </summary>
        /// <param name="text">Texto que se mostrará en el elemento.</param>
        /// <param name="image">Imagen que se mostrará en el elemento.</param>
        /// <param name="onClick">Un controlador de eventos que provoca el evento
        /// <see cref="System.Windows.Forms.ToolStripItem.Click"/>.</param>
        /// <param name="name">Nombre de ProgrammersInc.Windows.Forms.Tab.</param>
        public Tab(string text, Image image, EventHandler onClick, string name)
            : base(text, image, onClick, name)
        {
            Initialize();
        }

        void Initialize()
       {
            this.AutoSize = false;
            this.Width = 60;
            CheckOnClick = true;
            this.ForeColor = Color.FromArgb(44, 90, 154);
            this.Font = new Font("Segoe UI", 9);
            this.Margin = new Padding(6, this.Margin.Top, this.Margin.Right, this.Margin.Bottom);
            i_opacity = o_opacity;
            timer.Interval = 1;
            timer.Tick += new EventHandler(timer_Tick);
        }
        #endregion

        /// <summary>
        /// Obtiene o establece si la propiedad CheckOnClick se mostrará la Explorador de Propiedades. 
        /// </summary>
        [DefaultValue(true)]
        public new bool CheckOnClick
        {
            get { return base.CheckOnClick; }
            set { base.CheckOnClick = value; }
        }

        /// <summary>
        /// Obtiene o establece el estilo a mostrarse en el elemento.
        /// </summary>
        protected override ToolStripItemDisplayStyle DefaultDisplayStyle {
            get {
                return ToolStripItemDisplayStyle.ImageAndText;
            }
        }
     
        /// <summary>
        /// Add extra internal spacing so we have enough room for our curve.
        /// </summary>
        protected override Padding DefaultPadding {
            get {
                return new Padding(35, 0, 6, 0);
            }
        }

        /// <summary>
        /// The associated TabStripPage - when Tab is clicked, this TabPage will be selected.
        /// </summary>
        [DefaultValue("null")]
        public TabStripPage TabStripPage {
            get {
                return tabStripPage;
            }
            set {
                tabStripPage = value;
            }
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.ToolStripItem.MouseEnter"/>.
        /// </summary>
        /// <param name="e"><see cref="System.EventArgs"/> que contiene los datos del evento.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            b_on = true; b_fading = true; b_selected = true;
            timer.Start();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.ToolStripItem.MouseLeave"/>.
        /// </summary>
        /// <param name="e"><see cref="System.EventArgs"/> que contiene los datos del evento.</param>
        protected override void OnMouseLeave(EventArgs e)
        {            
            b_on = false; b_fading = true; 
            timer.Start();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará en el elemento.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                Bitmap bmpdummy = new Bitmap(100,100);
                Graphics g = Graphics.FromImage(bmpdummy);
                float textwidth = g.MeasureString(this.Text, this.Font).Width;
                this.Width = Convert.ToInt16(textwidth) + 26;
            }
        }
        
        void timer_Tick(object sender, EventArgs e)
        {
            if (b_on)
            {
                if (i_opacity > e_opacity)
                {
                    i_opacity -= 20;
                    this.Invalidate();
                }
                else
                {
                    i_opacity = e_opacity;
                    this.Invalidate();
                    timer.Stop();
                }
            }
            if (!b_on)
            {
                if (i_opacity < o_opacity)
                {
                    i_opacity += 8;
                    this.Invalidate();
                }
                else
                {
                    i_opacity = o_opacity;
                    b_fading = false;
                    this.Invalidate();
                    b_selected = false;
                    timer.Stop();
                }

            }
        }
    }
}
