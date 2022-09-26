using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control que muestra una colección jerárquica de elementos con etiquetas
    /// y que opcionalmente pueden contener una imagen.
    /// </summary>
    [DesignTimeVisible(true)]
    [ToolboxItem(typeof(System.Windows.Forms.TreeView))]
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.ResizeRedraw 
                | ControlStyles.Selectable, true);
        }
    }
}
