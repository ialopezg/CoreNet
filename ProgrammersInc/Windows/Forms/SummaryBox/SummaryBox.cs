using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
	/// <summary>
	/// Clase que define un control listado con items personalizados.
	/// </summary>
    [ToolboxBitmap(typeof(SummaryBox))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Control lista con elementos personalizadaos.")]
	public partial class SummaryBox : UserControl
    {
        #region Enums
        /// <summary>
		/// Define los estados soportados por los items del control.
		/// </summary>
        public enum ItemStates
        {
        	/// <summary>
        	/// El item puede ser calculado.
        	/// </summary>
            CanCalculate = 0,
            /// <summary>
            /// El item no puede ser calculado.
            /// </summary>
            CannotCalculate = 1
        }

        /// <summary>
        /// Modos de seleccion de items soportados por el control
        /// </summary>
        public enum SelectionModes
        {
        	/// <summary>
        	/// Sin seleccion.
        	/// </summary>
            NoSelection = 0,
            /// <summary>
            /// Seleccion simple.
            /// </summary>
            SingleSelection = 1
        }
        #endregion

        #region Fields
        ItemStates ItemStatus;
        int[] RealItemPosition;
        int[] RealItemHeight;
        private int MaxY;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia del control <see cref="SummaryBox"/>.
        /// </summary>
        public SummaryBox()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            items = new SummaryBoxItemCollection(this);

            alternatingColors = true;
            alternatingColor = Color.Gainsboro;
            selectedIndex = -1;
        }
        #endregion

        #region Properties
        private Color alternatingColor;
        /// <summary>
        /// Obtiene o establece el color alterno cuando se ha establecido a <c>true</c>
        /// la propiedad <see cref="SummaryBox.AlternatingColor"/>.
        /// </summary>
        [DefaultValue("Color.Gainsboro")]
        [Description("Obtiene o establece el color que se usará en el alternado de colores entre los elementos del control.")]
        public Color AlternatingColor
        {
            get { return alternatingColor; }
            set
            {
                alternatingColor = value;
                Invalidate();
            }
        }

        private bool alternatingColors;
        /// <summary>
        /// Obtiene o establece si el control pintara los elementos alternando dos colores.
        /// </summary>
        [DefaultValue(true)]
        [Description("Obtiene o establece si el control pintara los elementos alternando dos colores.")]
        public bool AlternatingColors
        {
            get { return alternatingColors; }
            set
            {
                alternatingColors = value;
                Invalidate();
            }
        }

        SummaryBoxItemCollection items;
        /// <summary>
        /// Devuelve los elementos que se mostrarán en el control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Devuelve los elementos que se mostrarán en el control.")]
        public SummaryBoxItemCollection Items
        {
            get { return items; }
        }

        static Color selectionBackgroundBorderColor = Color.FromArgb(10, 36, 106);
        /// <summary>
        /// Obtiene o establece el color del borde del fondo de elemento seleccionado.
        /// </summary>
        public Color SelectionBackgroundBorderColor
        {
            get { return selectionBackgroundBorderColor; }
            set
            {
                selectionBackgroundBorderColor = value;
                Invalidate();
            }
        }

        static Color selectionBackgroundColor = Color.FromArgb(188, 195, 214);
        /// <summary>
        /// Obtiene o establece el color de fondo del elemento seleccionado.
        /// </summary>
        public Color SelectionBackgroundColor
        {
            get { return selectionBackgroundColor; }
            set
            {
                selectionBackgroundColor = value;
                this.Invalidate();
            }
        }

        SelectionModes SelectionState = SelectionModes.SingleSelection;
        /// <summary>
        /// Obtiene o establece el modo de seleccion de los elementos del control.
        /// </summary>
        public SelectionModes SelectionMode
        {
            get { return SelectionState; }
            set
            {
                SelectionState = value;
                selectedIndex = -1;
                Invalidate();
            }
        }

        int ItemVerticalSpacing = 5;
        /// <summary>
        /// Obtiene o establece el ancho del espaciado vertical entre los elementos del control.
        /// </summary>
        public int VerticalPadding
        {
            get { return ItemVerticalSpacing; }
            set
            {
                ItemVerticalSpacing = value;
                Invalidate();
            }
        }
        #endregion

        int selectedIndex;
        /// <summary>
        /// Obtiene el numero de indice del elemento seleccionado.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        #region Methods
        /// <summary>
        /// Agrega un item al control.
        /// </summary>
        /// <param name="item">Un elemento <see cref="SummaryBoxItem"/> a agregarse al control.</param>
        public void AddItem(SummaryBoxItem item)
        {
            items.Add(item);
            CalculateItems();
            Invalidate();
        }

        /// <summary>
        /// Agrega un item al control.
        /// </summary>
        /// <param name="header">Encabezado del item.</param>
        /// <param name="summary">Sumario para el item.</param>
        public void AddItem(string header, string summary)
        {
            AddItem(header, summary, string.Empty);
        }

        /// <summary>
        /// Agrega un item al control.
        /// </summary>
        /// <param name="header">Encabezado del item.</param>
        /// <param name="summary">Sumario para el item.</param>
        /// <param name="tag">Tag del item.</param>
        public void AddItem(string header, string summary, string tag)
        {
            AddItem(new SummaryBoxItem(header, summary, tag));
        }

        internal void CalculateItems()
        {
            try
            {
                int W = ClientSize.Width - this.YScroll.Width;
                if (ItemStatus != ItemStates.CanCalculate)
                    return;

                if (this.Visible)
                {
                    if (this.items.Count == 0)
                    {
                        RealItemPosition = null;
                        RealItemHeight = null;
                        Enabled = false;
                    }
                    else
                    {
                        Enabled = true;
                        int ItemY = 0;
                        RealItemPosition = new int[items.Count];
                        RealItemHeight = new int[items.Count];
                        for (int i = 0; i < items.Count; i++)
                        {
                            StringFormat f = new StringFormat(StringFormatFlags.FitBlackBox);
                            SummaryBoxItem obj = (SummaryBoxItem)this.items[i];
                            SizeF ItemContentSize = this.CreateGraphics().MeasureString(obj.Summary, this.Font, W - 5, f);
                            SizeF ItemHeaderSize = this.CreateGraphics().MeasureString(obj.Header, new Font(Font.Name, Font.Size, FontStyle.Bold), W - 5, f);
                            RealItemHeight[i] = Convert.ToInt32(ItemContentSize.Height + ItemHeaderSize.Height);
                            RealItemPosition[i] = ItemY;
                            ItemY += RealItemHeight[i] + ItemVerticalSpacing;
                        }
                        MaxY = (ItemY - Height) + (ItemVerticalSpacing * 2);
                        if (MaxY < 0)
                        {
                            MaxY = 0;
                            YScroll.Enabled = false;
                        }
                        else
                        {
                            YScroll.Enabled = true;
                            YScroll.Minimum = 0;
                            YScroll.Maximum = MaxY;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        #endregion

        /// <summary>
        /// Incializa la edicion de los elementos del control.
        /// </summary>
        public void BeginEdit()
        {
            ItemStatus = ItemStates.CannotCalculate;
        }

        /// <summary>
        /// Elimina un item del control.
        /// </summary>
        /// <param name="Index">Indice del item a eliminar.</param>
        public void RemoveItem(int Index)
        {
            if (items.Count == 0)
                return;

            this.items.RemoveAt(Index);
        }

        /// <summary>
        /// Finaliza la edicion de los elementos del control.
        /// </summary>
        public void EndEdit()
        {
            ItemStatus = ItemStates.CanCalculate;
        }

        /// <summary>
        /// Borra todos los elementos del control.
        /// </summary>
        public void ItemsClear()
        {
            this.items.Clear();
            this.Invalidate();
        }

        /// <summary>
        /// Obtiene el numero total de elementos en el control.
        /// </summary>
        /// <returns>El numero total de elementos.</returns>
        public int ItemCount()
        {
            return this.items.Count;
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.Paint"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                Bitmap BackBuffer = new Bitmap(ClientSize.Width - this.YScroll.Width, ClientSize.Height);
                Graphics DrawingArea = Graphics.FromImage(BackBuffer);

                DrawingArea.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                DrawingArea.Clear(SystemColors.Window);
                if (this.items.Count > 0)
                {
                    for (int i = 0; i < this.items.Count; i++)
                    {
                        DrawListItem(i, DrawingArea);
                    }
                }

                e.Graphics.DrawImageUnscaled(BackBuffer, 0, 0);
            }
            catch { }
        }

        private Brush SelectionBackground = new SolidBrush(selectionBackgroundColor);
        private Pen SelectionBackgroundBorder = new Pen(selectionBackgroundBorderColor);
        private void DrawListItem(int i, Graphics g)
        {
            try
            {
                SummaryBoxItem obj = (SummaryBoxItem)this.items[i];
                Brush BackgroundColor; Brush TextColor;
                FontStyle fs = new FontStyle();
                StringFormat f = new StringFormat(StringFormatFlags.FitBlackBox);
                int W = ClientSize.Width - this.YScroll.Width;
                RectangleF lrBG = new RectangleF(0, RealItemPosition[i] - YScroll.Value, W, RealItemHeight[i]);
                RectangleF lrSelectedBG = new RectangleF(1, (RealItemPosition[i] - YScroll.Value) + 1, (W) - 2, (RealItemHeight[i] - 2));
                Rectangle lrSelectedBorderBG = new Rectangle(0, (RealItemPosition[i] - YScroll.Value), (W) - 1, (RealItemHeight[i] - 1));
                SizeF ItemHeaderSize = g.MeasureString(obj.Header, new Font(Font.Name, Font.Size, FontStyle.Bold), W - 5, f);
                RectangleF lr = new RectangleF(0, (RealItemPosition[i] + ItemHeaderSize.Height) - YScroll.Value, W - 5, RealItemHeight[i] - ItemHeaderSize.Height);
                RectangleF lrHeader = new RectangleF(0, (RealItemPosition[i]) - YScroll.Value, W - 5, ItemHeaderSize.Height);

                if (this.SelectedIndex == i)
                {
                    BackgroundColor = SystemBrushes.Highlight;
                    TextColor = SystemBrushes.WindowText;
                    fs = FontStyle.Regular;
                }
                else
                {
                    TextColor = SystemBrushes.WindowText;
                    fs = FontStyle.Regular;

                    if (alternatingColors)
                        if ((i % 2) == 0)
                            BackgroundColor = new SolidBrush(AlternatingColor);
                        else
                            BackgroundColor = SystemBrushes.Window;
                    else
                        BackgroundColor = SystemBrushes.Window;
                }

                if (this.SelectedIndex == i)
                {
                    g.FillRectangle(SelectionBackground, lrSelectedBG);
                    g.DrawRectangle(SelectionBackgroundBorder, lrSelectedBorderBG);
                }
                else
                    g.FillRectangle(BackgroundColor, lrBG);

                g.DrawString(obj.Summary, new Font(Font.Name, Font.Size, fs), TextColor, lr, f);
                g.DrawString(obj.Header, new Font(Font.Name, Font.Size, FontStyle.Bold), TextColor, lrHeader, f);

                BackgroundColor = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void YScroll_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private void SummaryBox_Resize(object sender, EventArgs e)
        {
            CalculateItems();
        }

        private void SummaryBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (SelectionState == SelectionModes.SingleSelection)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (this.items.Count > 0)
                        {
                            for (int i = 0; i < this.items.Count; i++)
                            {
                                if (e.Y + YScroll.Value >= RealItemPosition[i] & e.Y + YScroll.Value <= RealItemPosition[i] + RealItemHeight[i])
                                    if (SelectedIndex == i)
                                    {
                                        selectedIndex = -1;
                                        this.Invalidate();
                                        return;
                                    }
                                    else
                                    {
                                        selectedIndex = i;
                                        this.Invalidate();
                                        return;
                                    }
                            }
                        }
                        if (this.SelectedIndex > -1)
                        {
                            selectedIndex = -1;
                            this.Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}
