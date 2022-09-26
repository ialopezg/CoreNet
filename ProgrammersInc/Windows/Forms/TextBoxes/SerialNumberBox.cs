using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control para números de serie de 25 caracteres.
    /// </summary>
    [ToolboxBitmap(typeof(TextBox))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Representa un control para números de serie de 25 caracteres.")]
    public partial class SerialNumberBox : UserControl
    {
        FilterTextBox[] boxes = new FilterTextBox[5];

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase.
        /// </summary>
        public SerialNumberBox()
        {
            InitializeComponent();

            this.boxes[0] = this.Box1;
            this.boxes[1] = this.Box2;
            this.boxes[2] = this.Box3;
            this.boxes[3] = this.Box4;
            this.boxes[4] = this.Box5;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene o establece un valor que indicará el modo de comportamiento del control.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(AcceptableCharacters.All)]
        [Description("Obtiene o establece un valor que indicará el modo de comportamiento del control.")]
        public AcceptableCharacters AcceptableChars
        {
            get { return this.boxes[0].AcceptableChars; }
            set
            {
                foreach (FilterTextBox box in this.boxes)
                    box.AcceptableChars = value;
            }
        }

        /// <summary>
        /// Obtiene o establece un valor que indica que sólo caracteres capitalizados podrán escribirse.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Obtiene o establece un valor que indica que sólo caracteres capitalizados podrán escribirse.")]
        public bool CaptleLettersOnly
        {
            get { return this.boxes[0].CaptleLettersOnly; }
            set
            {
                foreach (FilterTextBox box in this.boxes)
                    box.CaptleLettersOnly = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los caracteres que no podrán estar en el texto del control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Obtiene o establece los caracteres que no podrán estar en el texto del control.")]
        public string ForbidenChars
        {
            get { return this.boxes[0].ForbidenChars; }
            set
            {
                foreach (FilterTextBox box in this.boxes)
                    box.ForbidenChars = value;
            }
        }

        /// <summary>
        /// Obtiene o establece un valor indicando que el texto del control es de sólo lectura.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Obtiene o establece un valor indicando que el texto del control es de sólo lectura.")]
        public bool ReadOnly
        {
            get { return this.boxes[0].ReadOnly; }
            set
            {
                foreach (FilterTextBox box in this.boxes)
                    box.ReadOnly = value;
            }
        }

        /// <summary>
        /// Obtiene o establece un valor indicando si la dirección del texto.
        /// </summary>
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set
            {
                base.RightToLeft = value;
                if (base.RightToLeft == RightToLeft.Yes)
                {
                    this.Box5.TabIndex = 0;
                    this.lbl4.TabIndex = 1;
                    this.Box4.TabIndex = 2;
                    this.lbl4.TabIndex = 3;
                    this.Box3.TabIndex = 4;
                    this.lbl4.TabIndex = 5;
                    this.Box2.TabIndex = 6;
                    this.lbl4.TabIndex = 7;
                    this.Box1.TabIndex = 8;
                }
                else
                {
                    this.Box5.TabIndex = 8;
                    this.lbl1.TabIndex = 7;
                    this.Box4.TabIndex = 6;
                    this.lbl1.TabIndex = 5;
                    this.Box3.TabIndex = 4;
                    this.lbl1.TabIndex = 3;
                    this.Box2.TabIndex = 2;
                    this.lbl1.TabIndex = 1;
                    this.Box1.TabIndex = 0;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el texto asociado al control.
        /// </summary>
        [Browsable(true)]
        public override string Text
        {
            get
            {
                if (base.RightToLeft == RightToLeft.Yes)
                    return Box5.Text + Box4.Text + Box3.Text + Box2.Text + Box1.Text;
                else
                    return Box1.Text + Box2.Text + Box3.Text + Box4.Text + Box5.Text;
            }
            set
            {
                this.ClearControls();
                if (value.Length > 25)
                    value = value.Substring(0, 25);

                int len;
                for (int i = 0; i < value.Length && i < 25; i += 5)
                {
                    len = (i + 5) > value.Length ? value.Length - i : 5;
                    if (base.RightToLeft == RightToLeft.No)
                        boxes[i / 5].Text = value.Substring(i, len);
                    else
                        boxes[4 - (i / 5)].Text = value.Substring(i, len);
                }
            }
        }
        #endregion

        #region Methods Implementation
        #region Protected Override Methods
        /// <summary>
        /// Provoce el evento OnResize.
        /// </summary>
        /// <param name="e"><see cref="System.EventArgs"/> que contiene los datos del evetno.</param>
        protected override void OnResize(EventArgs e)
        {
            if (this.Size.Height > 18 || this.Size.Width > 293)
                this.Size = new Size(293, 18);
        }
        #endregion

        #region Private Methods
        void ClearControls()
        {
            foreach (FilterTextBox box in this.boxes)
                box.Text = string.Empty;
        }
        #endregion

        #region Event Handler Methods
        void TextBoxEnter(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            box.SelectAll();
        }

        void TextBoxTextChanged(object sender, EventArgs e)
        {
            FilterTextBox ft = (FilterTextBox)sender;
            if (ft.Text.Length == 5)
                this.SelectNextControl(ft, true, true, false, false);
        }
        #endregion
        #endregion
    }
}
