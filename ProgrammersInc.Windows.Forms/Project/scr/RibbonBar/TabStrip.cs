using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Control que simula las fichas de Microsoft Office 2007.
    /// </summary>
    [ToolboxItem(typeof(TabStripToolboxItem))]
    public partial class TabStrip : ToolStrip
    {
        Font boldFont = new Font(SystemFonts.MenuFont, FontStyle.Bold);
        private const int EXTRA_PADDING = 0;      
        
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.TabStrip"/>.
        /// </summary>
        public TabStrip()
        {
            Renderer = new TabStripProfessionalRenderer();
            this.Padding = new Padding(60, 3, 30, 0);
            this.AutoSize = false;
            this.Size = new Size(this.Width, 26);
            this.BackColor = Color.FromArgb(191, 219, 255);
            this.GripStyle = ToolStripGripStyle.Hidden;

            this.ShowItemToolTips = false;
        }

        /// <summary>
        /// Crea un <see cref="ProgrammersInc.Windows.Forms.Tab"/> predeterminado con el texto, la
        /// imagen y el controlador de eventos especificados en una nueva instancia de
        /// <see cref="ProgrammersInc.Windows.Forms.Tab"/>.
        /// </summary>
        /// <param name="text">El texto que se utilizará para el <see cref="ProgrammersInc.Windows.Forms.Tab"/>.
        /// Si el parámetro text es un guión (-), este método crea un System.Windows.Forms.ToolStripSeparator.</param>
        /// <param name="image">System.Drawing.Image que se va a mostrar en <see cref="ProgrammersInc.Windows.Forms.Tab"/>.</param>
        /// <param name="onClick">Un controlador de eventos que provoca el evento <see cref="System.Windows.Forms.Control.Click"/>
        /// cuando se hace clic en <see cref="ProgrammersInc.Windows.Forms.Tab"/>.</param>
        /// <returns>System.Windows.Forms.ToolStripButton.#ctor(System.String,System.Drawing.Image,System.EventHandler)
        /// o System.Windows.Forms.ToolStripSeparator si el parámetro text es un guión
        /// (-).</returns>
        protected override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
        {
            return new Tab(text, image, onClick);
        }

        /// <summary>
        /// Obtiene el espaciado interno, en píxeles, del contenido de un <see cref="ProgrammersInc.Windows.Forms.Tab"/>.
        /// </summary>
        protected override Padding DefaultPadding
        {
            get
            {
                Padding padding = base.DefaultPadding;
                padding.Top += EXTRA_PADDING;
                padding.Bottom += EXTRA_PADDING;

                return padding;
            }
        }

        Tab currentSelection;
        /// <summary>
        /// Obtiene o establece el elemento actualmente seleccionado.
        /// </summary>
        public Tab SelectedTab
        {
            get { return currentSelection; }
            set
            {
                if (currentSelection != value)
                {
                    currentSelection = value;

                    if (currentSelection != null)
                    {
                        PerformLayout();
                        if (currentSelection.TabStripPage != null)
                        {
                            currentSelection.TabStripPage.Activate();
                        }
                    }
                }

            }
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Tab currentTab = Items[i] as Tab;
                SuspendLayout();
                if (currentTab != null)
                {
                    if (currentTab != e.ClickedItem)
                    {
                        currentTab.Checked = false;
                        currentTab.Font = this.Font;
                        currentTab.b_active = false;
                    }
                    else
                    {
                        // currentTab.Font = boldFont;
                        currentTab.b_active = true;
                    }
                }
                ResumeLayout();
            }
            SelectedTab = e.ClickedItem as Tab;

            base.OnItemClicked(e);

        }

        protected override void SetDisplayedItems()
        {
            base.SetDisplayedItems();
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                if (DisplayedItems[i] == SelectedTab)
                {
                    DisplayedItems.Add(SelectedTab);
                    break;
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                Size size = base.DefaultSize;
                // size.Height += EXTRA_PADDING*2;
                return size;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = Size.Empty;
            proposedSize -= this.Padding.Size;

            foreach (ToolStripItem item in this.Items)
            {
                preferredSize = LayoutUtils.UnionSizes(preferredSize, item.GetPreferredSize(proposedSize) + item.Padding.Size);
            }
            return preferredSize + this.Padding.Size;
        }

        private int tabOverlap = 0;
        [DefaultValue(10)]
        public int TabOverlap
        {
            get { return tabOverlap; }
            set
            {
                if (tabOverlap != value)
                {
                   
                    tabOverlap = value;
                    // call perform layout so we 
                    PerformLayout();
                }
            }
        }



    }
}
