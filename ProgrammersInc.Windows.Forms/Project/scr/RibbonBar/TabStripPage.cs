using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Ficha seleccionable que puede contener una colección de controles.
    /// </summary>
    [ToolboxItem(false)]
    [Docking(DockingBehavior.Never)]
    [DesignerCategory("Code")]
    public class TabStripPage : RibbonPanel
    {
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.TabStripPage"/>.
        /// </summary>
        public TabStripPage() { }

        /// <summary>
        /// Trae este <see cref="ProgrammersInc.Windows.Forms.TabStripPage"/> al frente del switcher para ser activado.
        /// </summary>
        public void Activate()
        {
            TabPageSwitcher tabPageSwitcher = this.Parent as TabPageSwitcher;
            if (tabPageSwitcher != null)
            {
                tabPageSwitcher.SelectedTabStripPage = this;

                try
                {
                    int x0 = tabPageSwitcher.TabStrip.SelectedTab.Bounds.Location.X;
                    int xf = tabPageSwitcher.TabStrip.SelectedTab.Bounds.Right;
                    tabPageSwitcher.SelectedTabStripPage.LinePos(x0, xf, true);
                }
                catch { }
            }

        }
    }
}
