namespace ProgrammersInc.Windows.Forms
{
    partial class SerialNumberBox
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
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();
            this.Box5 = new ProgrammersInc.Windows.Forms.FilterTextBox();
            this.Box4 = new ProgrammersInc.Windows.Forms.FilterTextBox();
            this.Box3 = new ProgrammersInc.Windows.Forms.FilterTextBox();
            this.Box2 = new ProgrammersInc.Windows.Forms.FilterTextBox();
            this.Box1 = new ProgrammersInc.Windows.Forms.FilterTextBox();
            this.SuspendLayout();
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(52, 2);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(10, 13);
            this.lbl1.TabIndex = 1;
            this.lbl1.Text = "-";
            this.lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Location = new System.Drawing.Point(112, 2);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(10, 13);
            this.lbl2.TabIndex = 13;
            this.lbl2.Text = "-";
            this.lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl3
            // 
            this.lbl3.AutoSize = true;
            this.lbl3.Location = new System.Drawing.Point(172, 2);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(10, 13);
            this.lbl3.TabIndex = 14;
            this.lbl3.Text = "-";
            this.lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl4
            // 
            this.lbl4.AutoSize = true;
            this.lbl4.Location = new System.Drawing.Point(232, 2);
            this.lbl4.Name = "lbl4";
            this.lbl4.Size = new System.Drawing.Size(10, 13);
            this.lbl4.TabIndex = 15;
            this.lbl4.Text = "-";
            this.lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Box5
            // 
            this.Box5.AllowSpace = false;
            this.Box5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Box5.Location = new System.Drawing.Point(240, 0);
            this.Box5.Name = "Box5";
            this.Box5.Size = new System.Drawing.Size(53, 18);
            this.Box5.TabIndex = 12;
            this.Box5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Box5.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.Box5.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // Box4
            // 
            this.Box4.AllowSpace = false;
            this.Box4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Box4.Location = new System.Drawing.Point(180, 0);
            this.Box4.Name = "Box4";
            this.Box4.Size = new System.Drawing.Size(53, 18);
            this.Box4.TabIndex = 11;
            this.Box4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Box4.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.Box4.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // Box3
            // 
            this.Box3.AllowSpace = false;
            this.Box3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Box3.Location = new System.Drawing.Point(120, 0);
            this.Box3.Name = "Box3";
            this.Box3.Size = new System.Drawing.Size(53, 18);
            this.Box3.TabIndex = 10;
            this.Box3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Box3.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.Box3.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // Box2
            // 
            this.Box2.AllowSpace = false;
            this.Box2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Box2.Location = new System.Drawing.Point(60, 0);
            this.Box2.Name = "Box2";
            this.Box2.Size = new System.Drawing.Size(53, 18);
            this.Box2.TabIndex = 9;
            this.Box2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Box2.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.Box2.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // Box1
            // 
            this.Box1.AllowSpace = false;
            this.Box1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Box1.Location = new System.Drawing.Point(0, 0);
            this.Box1.Name = "Box1";
            this.Box1.Size = new System.Drawing.Size(53, 18);
            this.Box1.TabIndex = 8;
            this.Box1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Box1.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            this.Box1.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // SerialNumberBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Box5);
            this.Controls.Add(this.Box4);
            this.Controls.Add(this.Box3);
            this.Controls.Add(this.Box2);
            this.Controls.Add(this.Box1);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.lbl4);
            this.Name = "SerialNumberBox";
            this.Size = new System.Drawing.Size(293, 18);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgrammersInc.Windows.Forms.FilterTextBox Box1;
        private ProgrammersInc.Windows.Forms.FilterTextBox Box2;
        private ProgrammersInc.Windows.Forms.FilterTextBox Box3;
        private ProgrammersInc.Windows.Forms.FilterTextBox Box4;
        private ProgrammersInc.Windows.Forms.FilterTextBox Box5;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.Label lbl4;
    }
}
