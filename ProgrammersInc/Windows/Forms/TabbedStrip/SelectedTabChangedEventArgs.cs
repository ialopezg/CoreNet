using System;

namespace ProgrammersInc.Windows.Forms
{
    public class SelectedTabChangedEventArgs : EventArgs
    {
        public readonly TabStripButton SelectedTab;

        public SelectedTabChangedEventArgs(TabStripButton tab)
        {
            SelectedTab = tab;
        }
    }
}