using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un control divisor que le permite al usuario cambiar el tamaño de los controles acoplados.
    /// </summary>
    [Description("Representa un control divisor que le permite al usuario cambiar el tamaño de los controles acoplados.")]
    [DesignTimeVisibleAttribute(true)]
    [ToolboxBitmap(typeof(System.Windows.Forms.Splitter))]
    [DesignerAttribute(typeof(SplitterDesigner))]
    public class Splitter : System.Windows.Forms.Splitter
    {
        #region Fields
        /// <summary>
        /// Variable requerida por el diseñador.
        /// </summary>
        IContainer components = null;
        Form parentForm;
        SplitterState currentState;
        Rectangle rr;
        bool hot;
        Color hotColor = CalculateColor(SystemColors.Highlight, SystemColors.Window, 70);
        int controlWidth;
        int controlHeight;
        Timer animationTimer;
        int parentFormWidth;
        int parentFormHeight;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Splitter"/>.
        /// </summary>
        public Splitter()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            animationTimer = new Timer();
            animationTimer.Interval = animationDelay;
            animationTimer.Tick += new EventHandler(animationTimerTick);
        }
        #endregion

        #region Properties
        int animationDelay = 20;
        /// <summary>
        /// Obtiene o establece un valor inidicando el retardo en milisegundos entre los cambios de la animación.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(20)]
        [Description("El retardo en milisegundos entre los cambios de la animación.")]
        public int AnimationDelay
        {
            get { return animationDelay; }
            set
            {
                if (value < 20)
                    animationDelay = 20;
                else
                    animationDelay = value;
            }
        }

        int animationStep = 20;
        /// <summary>
        /// Obtiene o establece un valor indicando la cantidad de pixeles que se moverá en cada paso de la animación.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(20)]
        [Description("La cantidad de pixeles que se moverá en cada paso de la animación.")]
        public int AnimationStep
        {
            get { return animationStep; }
            set
            {
                if (value < 20)
                    animationStep = 20;
                else
                    animationStep = value;
            }
        }

        Border3DStyle borderStyle = Border3DStyle.Flat;
        /// <summary>
        /// Obtiene o establece el estilo del borde en 3D al pintar el control.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(Border3DStyle.Flat)]
        [Description("El estilo del borde en modo 3D del control.")]
        public Border3DStyle BorderStyle3D
        {
            get { return borderStyle; }
            set
            {
                borderStyle = value;
                Invalidate();
            }
        }

        Control controlToHide;
        /// <summary>
        /// Obtiene o establece el control que será colapsado por este control divisor.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(null)]
        [Description("Obtiene o establece el control que será colapsado por este control divisor.")]
        public Control ControlToHide
        {
            get { return controlToHide; }
            set { controlToHide = value; }
        }

        bool expandParentForm = false;
        /// <summary>
        /// Obtiene o establece un valor que determina si el formulario padre será expandido o colapsado.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(false)]
        [Description("Determina si el formulario padre será expandido o colapsado.")]
        public bool ExpandParentForm
        {
            get { return expandParentForm; }
            set { expandParentForm = value; }
        }

        /// <summary>
        /// Obtiene o establece el estado inicial del control.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(false)]
        [Description("Determina el estado inicial del control.")]
        public bool IsCollapsed
        {
            get
            {
                if (controlToHide != null)
                    return !controlToHide.Visible;
                else
                    return true;
            }
        }

        bool useAnimations = true;
        /// <summary>
        /// Obtiene o establece un valor determinando si el control usará animaciones a la hora de colapsarse o contraerse.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(true)]
        [Description("Determinando si el control usará animaciones a la hora de colapsarse o contraerse.")]
        public bool UseAnimations
        {
            get { return useAnimations; }
            set { useAnimations = value; }
        }

        ProgressBarStyle visualStyle = ProgressBarStyle.XP;
        /// <summary>
        /// Obtiene o establece el estilo visual como el control será dibujado.
        /// </summary>
        [Bindable(true)]
        [Category("Collapsing Options")]
        [DefaultValue(ProgressBarStyle.XP)]
        [Description("El estilo visual como el control será dibujado.")]
        public ProgressBarStyle VisualStyle
        {
            get { return visualStyle; }
            set
            {
                visualStyle = value;
                Invalidate();
            }
        }
        #endregion

        #region Methods
        void animationTimerTick(object sender, System.EventArgs e)
        {
            switch (currentState)
            {
                case SplitterState.Collapsing:
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    {
                        if (controlToHide.Width > animationStep)
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Width -= animationStep;
                            controlToHide.Width -= animationStep;
                        }
                        else
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Width = parentFormWidth;
                            controlToHide.Visible = false;
                            animationTimer.Enabled = false;
                            controlToHide.Width = controlWidth;
                            currentState = SplitterState.Collapsed;
                            Invalidate();
                        }
                    }
                    else
                    {
                        if (controlToHide.Height > animationStep)
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Height -= animationStep;
                            controlToHide.Height -= animationStep;
                        }
                        else
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Height = parentFormHeight;
                            controlToHide.Visible = false;
                            animationTimer.Enabled = false;
                            controlToHide.Height = controlHeight;
                            currentState = SplitterState.Collapsed;
                            Invalidate();
                        }
                    }
                    break;
                case SplitterState.Expanding:
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    {
                        if (controlToHide.Width < (controlWidth - animationStep))
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Width += animationStep;
                            controlToHide.Width += animationStep;
                        }
                        else
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Width = parentFormWidth;
                            controlToHide.Width = controlWidth;
                            controlToHide.Visible = true;
                            animationTimer.Enabled = false;
                            currentState = SplitterState.Expanded;
                            Invalidate();
                        }
                    }
                    else
                    {
                        if (controlToHide.Height < (controlHeight - animationStep))
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Height += animationStep;
                            controlToHide.Height += animationStep;
                        }
                        else
                        {
                            if (expandParentForm && parentForm.WindowState != FormWindowState.Maximized && parentForm != null)
                                parentForm.Height = parentFormHeight;
                            controlToHide.Height = controlHeight;
                            controlToHide.Visible = true;
                            animationTimer.Enabled = false;
                            currentState = SplitterState.Expanded;
                            Invalidate();
                        }
                    }
                    break;
            }
        }

        Point[] ArrowPointArray(int x, int y)
        {
            Point[] point = new Point[3];

            if (controlToHide != null)
            {
                if ((Dock == DockStyle.Right && controlToHide.Visible) || (Dock == DockStyle.Left && !controlToHide.Visible))
                {
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 3, y + 3);
                    point[2] = new Point(x, y + 6);
                }
                else if ((Dock == DockStyle.Right && !controlToHide.Visible) || (Dock == DockStyle.Left && controlToHide.Visible))
                {
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x, y + 3);
                    point[2] = new Point(x + 3, y + 6);
                }
                else if ((Dock == DockStyle.Top && controlToHide.Visible) || (Dock == DockStyle.Bottom && !controlToHide.Visible))
                {
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x + 6, y + 4);
                    point[2] = new Point(x, y + 4);
                }
                else if ((Dock == DockStyle.Top && !controlToHide.Visible) || (Dock == DockStyle.Bottom && controlToHide.Visible))
                {
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 6, y);
                    point[2] = new Point(x + 3, y + 3);
                }
            }

            return point;
        }

        static Color CalculateColor(Color front, Color back, int alpha)
        {
            Color frontColor = Color.FromArgb(255, front);
            Color backColor = Color.FromArgb(255, back);

            float frontRed = frontColor.R;
            float frontGreen = frontColor.G;
            float frontBlue = frontColor.B;
            float backRed = backColor.R;
            float backGreen = backColor.G;
            float backBlue = backColor.B;

            float fRed = frontRed * alpha / 255 + backRed * ((float)(255 - alpha) / 255);
            byte newRed = (byte)fRed;
            float fGreen = frontGreen * alpha / 255 + backGreen * ((float)(255 - alpha) / 255);
            byte newGreen = (byte)fGreen;
            float fBlue = frontBlue * alpha / 255 + backBlue * ((float)(255 - alpha) / 255);
            byte newBlue = (byte)fBlue;

            return Color.FromArgb(255, newRed, newGreen, newBlue);
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.EnabledChanged"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.Click"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (controlToHide != null && hot && currentState != SplitterState.Collapsing && currentState != SplitterState.Expanding)
                ToggleSplitter();
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.HandleCreated"/>.
        /// </summary>
        /// <param name="e">datos del evento.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            parentForm = FindForm();

            if (controlToHide != null)
            {
                if (controlToHide.Visible)
                    currentState = SplitterState.Expanded;
                else
                    currentState = SplitterState.Collapsed;
            }
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.MouseDown"/>.
        /// </summary>
        /// <param name="e">datos del evento.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.controlToHide != null)
                if (!this.hot && this.controlToHide.Visible)
                    base.OnMouseDown(e);
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.MouseLeave"/>.
        /// </summary>
        /// <param name="e">datos del evento.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hot = false;
            Invalidate();
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.MouseMove"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.X >= rr.X && e.X <= rr.X + rr.Width && e.Y >= rr.Y && e.Y <= rr.Y + rr.Height)
            {
                if (!hot)
                {
                    hot = true;
                    Cursor = Cursors.Hand;
                    Invalidate();
                }
            }
            else
            {
                if (hot)
                {
                    hot = false;
                    Invalidate();
                }

                Cursor = Cursors.Default;

                if (controlToHide != null)
                {
                    if (!controlToHide.Visible)
                        Cursor = Cursors.Default;
                    else
                    {
                        if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                            this.Cursor = Cursors.VSplit;
                        else
                            this.Cursor = Cursors.HSplit;
                    }
                }
            }
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.Paint"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle r = this.ClientRectangle;

            g.FillRectangle(new SolidBrush(BackColor), r);

            if (Dock == DockStyle.Left || Dock == DockStyle.Right)
            {
                rr = new Rectangle(r.X, (int)(r.Y + ((r.Height - 115) / 2)), 8, 115);
                Width = 8;

                if (hot)
                    g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));
                else
                    g.FillRectangle(new SolidBrush(BackColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));

                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y, rr.X + rr.Width - 2, rr.Y);
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y + rr.Height, rr.X + rr.Width - 2, rr.Y + rr.Height);

                if (Enabled)
                {
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + 3));
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + rr.Height - 9));
                }

                int x = rr.X + 3;
                int y = rr.Y + 14;

                switch(visualStyle)
				{
					case ProgressBarStyle.Mozilla:

						for(int i=0; i < 30; i++)
						{
                            g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y + (i * 3), x + 1, y + 1 + (i * 3));
                            g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
                            if (hot)
                                g.DrawLine(new Pen(hotColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
                            else
                                g.DrawLine(new Pen(this.BackColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
						}
						break;

					case ProgressBarStyle.DoubleDots:
                        for (int i = 0; i < 30; i++)
                        {
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x, y + 1 + (i * 3), 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x - 1, y + (i * 3), 1, 1);
                            i++;
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 2, y + 1 + (i * 3), 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + 1, y + (i * 3), 1, 1);
                        }
						break;
					case ProgressBarStyle.Win9x:
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 2, y);
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 90);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x + 2, y, x + 2, y + 90);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 90, x + 2, y + 90);
                        break;
					case ProgressBarStyle.XP:

                        for (int i = 0; i < 18; i++)
                        {
                            g.DrawRectangle(new Pen(SystemColors.ControlLight), x, y + (i * 5), 2, 2);
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1, y + 1 + (i * 5), 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x, y + (i * 5), 1, 1);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x, y + (i * 5) + 1);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x + 1, y + (i * 5));
                        }
						break;
					case ProgressBarStyle.Lines:
                        for (int i = 0; i < 44; i++)
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 2), x + 2, y + (i * 2));
						break;
				}

                if (this.borderStyle != Border3DStyle.Flat)
                {
                    System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Left);
                    System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Right);
                }
            }
            else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
            {
                rr = new Rectangle((int)r.X + ((r.Width - 115) / 2), r.Y, 115, 8);
                this.Height = 8;

                if (hot)
                    g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));
                else
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));

                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X, rr.Y + 1, rr.X, rr.Y + rr.Height - 2);
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + rr.Width, rr.Y + 1, rr.X + rr.Width, rr.Y + rr.Height - 2);

                if (this.Enabled)
                {
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 3, rr.Y + 2));
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + rr.Width - 9, rr.Y + 2));
                }

                int x = rr.X + 14;
                int y = rr.Y + 3;

                switch (visualStyle)
                {
                    case ProgressBarStyle.Mozilla:
                        for (int i = 0; i < 30; i++)
                        {
                            g.DrawLine(new Pen(SystemColors.ControlLightLight), x + (i * 3), y, x + 1 + (i * 3), y + 1);
                            g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1 + (i * 3), y + 1, x + 2 + (i * 3), y + 2);
                            if (hot)
                                g.DrawLine(new Pen(hotColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
                            else
                                g.DrawLine(new Pen(this.BackColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
                        }
                        break;
                    case ProgressBarStyle.DoubleDots:
                        for (int i = 0; i < 30; i++)
                        {
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y, 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y - 1, 1, 1);
                            i++;
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y + 2, 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y + 1, 1, 1);
                        }
                        break;
                    case ProgressBarStyle.Win9x:
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 2);
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 88, y);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 2, x + 88, y + 2);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x + 88, y, x + 88, y + 2);
                        break;
                    case ProgressBarStyle.XP:
                        for (int i = 0; i < 18; i++)
                        {
                            g.DrawRectangle(new Pen(SystemColors.ControlLight), x + (i * 5), y, 2, 2);
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 5), y + 1, 1, 1);
                            g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x + (i * 5), y, 1, 1);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5) + 1, y);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5), y + 1);
                        }
                        break;
                    case ProgressBarStyle.Lines:
                        for (int i = 0; i < 44; i++)
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 2), y, x + (i * 2), y + 2);
                        break;
                }

                if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Top);
                    System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Bottom);
                }
            }
            else
                throw new Exception("The Collapsible Splitter control cannot have the Filled or None Dockstyle property");

            g.Dispose();
        }

        /// <summary>
        /// Provoca el evento <see cref="Control.Resize"/>.
        /// </summary>
        /// <param name="e">Datos del evento.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        void ToggleSplitter()
        {
            if (currentState == SplitterState.Collapsing || currentState == SplitterState.Expanding)
                return;

            controlWidth = controlToHide.Width;
            controlHeight = controlToHide.Height;

            if (controlToHide.Visible)
            {
                if (useAnimations)
                {
                    currentState = SplitterState.Collapsing;

                    if (parentForm != null)
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                            parentFormWidth = parentForm.Width - controlWidth;
                        else
                            parentFormHeight = parentForm.Height - controlHeight;
                    }

                    animationTimer.Enabled = true;
                }
                else
                {
                    currentState = SplitterState.Collapsed;
                    controlToHide.Visible = false;
                    if (expandParentForm && parentForm != null)
                    {
                        if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                            parentForm.Width -= controlToHide.Width;
                        else
                            parentForm.Height -= controlToHide.Height;
                    }
                }
            }
            else
            {
                if (useAnimations)
                {
                    currentState = SplitterState.Expanding;

                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    {
                        if (parentForm != null)
                            parentFormWidth = parentForm.Width + controlWidth;
                        controlToHide.Width = 0;
                    }
                    else
                    {
                        if (parentForm != null)
                            parentFormHeight = parentForm.Height + controlHeight;
                        controlToHide.Height = 0;
                    }
                    controlToHide.Visible = true;
                    this.animationTimer.Enabled = true;
                }
                else
                {
                    currentState = SplitterState.Expanded;
                    controlToHide.Visible = true;
                    if (expandParentForm && parentForm != null)
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                            parentForm.Width += controlToHide.Width;
                        else
                            parentForm.Height += controlToHide.Height;
                    }
                }
            }
        }
        #endregion
    }
}