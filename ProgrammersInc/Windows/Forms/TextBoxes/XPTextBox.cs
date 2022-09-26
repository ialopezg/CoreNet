using System;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control de texto de Windows XP.
    /// </summary>
    [ToolboxBitmap(typeof(TextBox))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Representa un control de texto de Windows XP.")]
    public class XPTextBox : TextBox
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase XPTextBox.
        /// </summary>
        public XPTextBox()
 {  }
        #endregion

        #region Properties
        bool allowSpace;
        /// <summary>
        /// Devuelve o establece un valor indicando si el control aceptará como entrada
        /// el caracter de espaciado.
        /// </summary>
        public bool AllowSpace
        {
            set { this.allowSpace = value; }
            get { return this.allowSpace; }
        }

        TextboxStyles behavior = TextboxStyles.None;
        /// <summary>
        /// Devuelve o establece el tipo de comportamiento del control.
        /// </summary>
        [DefaultValue(TextboxStyles.None)]
        public TextboxStyles Behavior
        {
            get { return behavior; }
            set
            {
                behavior = value;
                if (value == TextboxStyles.Numeric)
                {
                    if (string.IsNullOrEmpty(this.Text))
                        this.Text = "0";
                    this.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
                }
            }
        }

        /// <summary>
        /// Devuelve el valor entero que representa el texto del control.
        /// </summary>
        public int IntValue
        {
            get { return Int32.Parse(this.Text); }
        }

        /// <summary>
        /// Deveulve el valor decimal que representa el texto del control.
        /// </summary>
        public decimal DecimalValue
        {
            get { return Decimal.Parse(this.Text); }
        }
        #endregion

        #region Methods Implementation
        #region Overrides Methods
        /// <summary>
        /// Recupera todos los mensajes de la ventana enviados al SO.
        /// </summary>
        /// <param name="m">Mensaje a procesar.</param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0085)  // WM_NCPAINT
            {
                Graphics g = Graphics.FromHdc(GetWindowDC(this.Handle));
                ControlPaint.DrawBorder(g, 0, 0, this.Width - 1, this.Height - 1);
                this.DrawInnerBorder(g);
                g.Dispose();
            }
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.KeyPress"/>.
        /// </summary>
        /// <param name="e"><see cref="System.Windows.Forms.KeyPressEventArgs"/> que contiene los datos
        /// del evento.</param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (behavior == TextboxStyles.Numeric)
            {
                NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
                string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
                string groupSeparator = numberFormatInfo.NumberGroupSeparator;
                string negativeSign = numberFormatInfo.NegativeSign;

                string keyInput = e.KeyChar.ToString();

                if (Char.IsDigit(e.KeyChar))
                {
                }
                else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
                    keyInput.Equals(negativeSign))
                {
                    // Decimal separator is OK
                }
                else if (e.KeyChar == '\b')
                {
                    // Backspace key is OK
                }
                //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                //    {
                //     // Let the edit control handle control and alt key combinations
                //    }
                else if (this.allowSpace && e.KeyChar == ' ')
                {

                }
                else
                {
                    // Consume this invalid key and beep
                    e.Handled = true;
                    //    MessageBeep();
                }

            }
        }
        #endregion

        #region Privates Methods
        void DrawInnerBorder(Graphics g)
        {
            g.DrawRectangle(new Pen(this.BackColor, 0), 1, 1,
                this.Width - 3, this.Height - 3);
        }
        #endregion
        #endregion
    }
}