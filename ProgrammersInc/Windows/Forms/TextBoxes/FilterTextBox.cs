using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control de cuadro de texto al estilo Windows XP, que acepta
    /// personalización de comportamiento.
    /// </summary>
    [ToolboxBitmap(typeof(TextBox))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Representa un control de cuadro de texto al estilo Windows XP.")]
    public class FilterTextBox : XPTextBox
    {
        #region Variables Imlementation
        bool internalEditing;
        #endregion

        #region Contructors
        /// <summary>
        /// Crea una nueva instancia de la clase.
        /// </summary>
        public FilterTextBox()
        {
            this.forbidenChars = string.Empty;
            this.internalEditing = false;
            this.captleLettersOnly = false;
            this.acceptableChars = AcceptableCharacters.All;
        }
        #endregion

        #region Properties Implementation
        AcceptableCharacters acceptableChars;
        /// <summary>
        /// Obtiene o establece un valor que indicará el modo de comportamiento del control.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(AcceptableCharacters.All)]
        [Description("Obtiene o establece un valor que indicará el modo de comportamiento del control.")]
        public AcceptableCharacters AcceptableChars
        {
            get { return this.acceptableChars; }
            set { this.acceptableChars = value; }
        }

        bool captleLettersOnly;
        /// <summary>
        /// Obtiene o establece un valor que indica que sólo caracteres capitalizados podrán escribirse.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Obtiene o establece un valor que indica que sólo caracteres capitalizados podrán escribirse.")]
        public bool CaptleLettersOnly
        {
            get { return this.captleLettersOnly; }
            set { this.captleLettersOnly = value; }
        }

        string forbidenChars;
        /// <summary>
        /// Obtiene o establece los caracteres que no podrán estar en el texto del control.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Obtiene o establece los caracteres que no podrán estar en el texto del control.")]
        public string ForbidenChars
        {
            get { return this.forbidenChars; }
            set { this.forbidenChars = value; }
        }

        /// <summary>
        /// Obtiene o establece el texto asociado al control.
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (internalEditing)
                    base.Text = value;
                else
                    base.Text = RemoveForbidens(value);
            }
        }
        #endregion

        #region Methods Implementation
        #region Overrides
        /// <summary>
        /// Manejador del evento TextChanged.
        /// </summary>
        /// <param name="e"><see cref="System.EventArgs"/> que contiene los datos del evento.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            int selS = this.SelectionStart;
            this.internalEditing = true;
            this.Text = this.RemoveForbidens(this.Text, ref selS);
            this.internalEditing = false;
            this.SelectionStart = selS;
        }        
        #endregion

        #region Privates
        string RemoveForbidens(string st)
        {
            int i = st.Length;
            return RemoveForbidens(st, ref i);
        }

        string RemoveForbidens(string st, ref int selStart)
        {
            if (captleLettersOnly)
                st = st.ToUpper();

            for (int i = st.Length - 1; i >= 0; i--)
            {
                if (forbidenChars.IndexOf(st[i]) != -1)
                {
                    st = st.Remove(i, 1);
                    if (i < selStart)
                        selStart--;
                }
                else if (acceptableChars == AcceptableCharacters.DigitOnly && char.IsDigit(st[i]) != true)
                {
                    st = st.Remove(i, 1);
                    if (i < selStart)
                        selStart--;
                }
                else if (acceptableChars == AcceptableCharacters.LetterOnly && char.IsLetter(st[i]) != true)
                {
                    st = st.Remove(i, 1);
                    if (i < selStart)
                        selStart--;
                }
                else if (acceptableChars == AcceptableCharacters.LetterOrDigit && char.IsLetterOrDigit(st[i]) != true)
                {
                    st = st.Remove(i, 1);
                    if (i < selStart)
                        selStart--;
                }
            }

            return st;
        }
        #endregion
        #endregion
    }
}