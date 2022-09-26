namespace ProgrammersInc.Windows.Forms
{
    partial class SummaryBox
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.YScroll = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // YScroll
            // 
            this.YScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.YScroll.Location = new System.Drawing.Point(264, 0);
            this.YScroll.Name = "YScroll";
            this.YScroll.Size = new System.Drawing.Size(16, 264);
            this.YScroll.TabIndex = 2;
            this.YScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.YScroll_Scroll);
            // 
            // SummaryBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.YScroll);
            this.Name = "SummaryBox";
            this.Size = new System.Drawing.Size(280, 264);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SummaryBox_MouseDown);
            this.Resize += new System.EventHandler(this.SummaryBox_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.VScrollBar YScroll;
    }
}
