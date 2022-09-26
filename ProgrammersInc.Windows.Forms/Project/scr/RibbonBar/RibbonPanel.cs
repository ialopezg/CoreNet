using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Clase que emula las fichas de menú de selección de Microsoft Office 2007.
    /// </summary>
    public class RibbonPanel : Panel
    {
        #region Variables Implementation
        int X0;
        int XF;
        int Y0;
        int YF;
        int T = 3;
        int i_Zero = 180;
        int i_Sweep = 90;
        int X; int Y;
        GraphicsPath path;
        int D = -1;
        int R0 = 215;
        int G0 = 227;
        int B0 = 242;
        int i_mode = 0; //0 Entering, 1 Leaving
        int i_fR = 1; int i_fG = 1; int i_fB = 1;

        Timer timer1 = new Timer();

        int activex0 = 0;
        int activexf = 0;
        bool activestate = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.RibbonPanel"/>.
        /// </summary>
        public RibbonPanel()
        {
            this.Padding = new Padding(0, 3, 0, 0);
            timer1.Interval = 1;
            timer1.Tick += new EventHandler(timer1_Tick);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        #endregion

        #region Properties
        Color baseColor = Color.FromArgb(215, 227, 242);
        /// <summary>
        /// Obtiene o establece el color base para el controls.
        /// </summary>
        public Color BaseColor
        {
            get { return baseColor; }
            set 
            {
                baseColor = value;
                R0 = baseColor.R;
                B0 = baseColor.B;
                G0 = baseColor.G;
            }
        }

        Color baseColorOn = Color.FromArgb(215, 227, 242);
        /// <summary>
        /// Obtiene o establece el color base del control cuando esta activado.
        /// </summary>
        public Color BaseColorOn
        {
            get { return baseColorOn; }
            set
            {
                baseColorOn = value;
                R0 = baseColor.R;
                B0 = baseColor.B;
                G0 = baseColor.G;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto asociado al control.
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                this.Refresh();
            }
        }

        int speed = 8;
        /// <summary>
        /// Obtiene o establece el intervalo de tiempo que se usara para la
        /// animación del control.
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        int opacity = 255;
        /// <summary>
        /// Obtiene o establece un valor que representa el nivel de opacidad del control.
        /// </summary>
        public int Opacity
        {
            get { return opacity; }
            set
            {
                if (value < 256 | value > -1)
                    opacity = value;
            }

        }
        #endregion

        #region Overrides Methods
        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.Paint"/>.
        /// </summary>
        /// <param name="e"><see cref="System.Windows.Forms.PaintEventArgs"/> que contiene los
        /// datos de eventos.</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            X0 = 0; XF = this.Width + X0 - 3;
            Y0 = 0; YF = this.Height + Y0 - 3;
            T = 6;
            Point P0 = new Point(X0, Y0 - 1);
            Point PF = new Point(X0, Y0 + YF + 8);
            Pen b1 = new Pen(Color.FromArgb(opacity, R0 - 18, G0 - 17, B0 - 19));
            Pen b2 = Pens.Black;
            try
            {
                b2 = new Pen(Color.FromArgb(opacity, R0 - 74, G0 - 49, B0 - 15));
            }
            catch
            {
                b2 = new Pen(Color.FromArgb(opacity, R0 - 22, G0 - 11, B0));
            }
            Pen b22 = new Pen(Color.FromArgb(opacity, R0 + 23, G0 + 21, B0 + 13));
            Pen b3 = new Pen(Color.FromArgb(opacity, R0 + 14, G0 + 9, B0 + 3));
            Pen b4 = new Pen(Color.FromArgb(opacity, R0 - 8, G0 - 4, B0 - 2));
            Pen b5 = new Pen(Color.FromArgb(opacity, R0+4, G0+3, B0+3));
            Pen b6 = new Pen(Color.FromArgb(opacity, R0 - 16, G0 - 11, B0 - 5));
            Pen b8 = new Pen(Color.FromArgb(opacity, R0 + 12, G0 + 17, B0 + 13));
            Pen b7 = new Pen(Color.FromArgb(opacity, R0 - 22, G0 - 10, B0));

            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            Brush B4 = b4.Brush;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            X = X0; Y = Y0; i_Zero = 180; D = 0;

            DrawArc();
            e.Graphics.FillPath(b5.Brush, path);
            Rectangle rect = e.ClipRectangle;
            //LinearGradientBrush brocha = new LinearGradientBrush(rect, b6.Color, b8.Color,LinearGradientMode.Vertical);
            LinearGradientBrush brocha = new LinearGradientBrush(P0, PF, b6.Color, b8.Color);
            DrawArc2(17, YF + 7);
            e.Graphics.FillPath(brocha, path);
            D--;
            DrawFHalfArc();
            e.Graphics.DrawPath(b2, path);
            DrawSHalfArc();
            e.Graphics.DrawPath(b22, path);

            if (activestate)
            {
                e.Graphics.DrawLine(b5,new Point(activex0+1,0),new Point(activexf-9,0));
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.MouseEnter"/>.
        /// </summary>
        /// <param name="e"><see cref="System.Windows.Forms.PaintEventArgs"/> que contiene los
        /// datos de eventos.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            Point P_EX = Cursor.Position;
            P_EX = this.PointToClient(P_EX);
            if (P_EX.X > 0 | P_EX.X < this.Width | P_EX.Y > 0 | P_EX.Y < this.Height)
            {
                i_mode = 0;
                timer1.Start();
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.MouseLeave"/>.
        /// </summary>
        /// <param name="e"><see cref="System.Windows.Forms.PaintEventArgs"/> que contiene los
        /// datos de eventos.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            Point P_EX = Cursor.Position;
            P_EX = this.PointToClient(P_EX);
            if (P_EX.X < 0 | P_EX.X >= this.Width | P_EX.Y < 0 | P_EX.Y >= this.Height)
            {
                i_mode = 1;
                timer1.Start();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.Resize"/>.
        /// </summary>
        /// <param name="e"><see cref="System.Windows.Forms.PaintEventArgs"/> que contiene los
        /// datos de eventos.</param>
        protected override void OnResize(EventArgs e)
        {
            this.Refresh();
            base.OnResize(e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Dibuja un arco elíptico en el control.
        /// </summary>
        public void DrawArc()
        {
            X = X0 - 2; Y = Y0 - 1; i_Zero = 180; D++;
            path = new GraphicsPath();
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; X += XF;
            path.AddArc(X - D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y += YF;
            path.AddArc(X - D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; X -= XF;
            path.AddArc(X + D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y -= YF;
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep);
        }

        /// <summary>
        /// Dibuja un arco elíptico en el control.
        /// </summary>
        public void DrawFHalfArc()
        {
            X = X0 - 2; Y = Y0 - 1; i_Zero = 180; D++;
            path = new GraphicsPath();
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; X += XF - 1;
            path.AddArc(X - D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y += YF;
            path.AddArc(X - D, Y - D, T, T, i_Zero, i_Sweep);

        }

        /// <summary>
        /// Dibuja un arco elíptico en el control.
        /// </summary>
        public void DrawSHalfArc()
        {
            X = X0 - 3; Y = Y0 - 1; i_Zero = 180; D++;
            path = new GraphicsPath();
            i_Zero += 90; X += XF;
            path.AddArc(X - D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y += YF - 1;
            path.AddArc(X - D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; X -= XF - 1;
            path.AddArc(X + D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y -= YF - 1;
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep);

        }

        /// <summary>
        /// Dibuja un arco elíptico en el control.
        /// </summary>
        public void DrawArc2(int OF_Y, int SW_Y)
        {
            X = X0 - 1; Y = Y0 + OF_Y; i_Zero = 180;
            path = new GraphicsPath();
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; X += XF - 1;
            path.AddArc(X - D, Y + D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y += SW_Y - 20;
            path.AddArc(X - D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; X -= XF - 1;
            path.AddArc(X + D, Y - D, T, T, i_Zero, i_Sweep); i_Zero += 90; Y -= SW_Y - 20;
            path.AddArc(X + D, Y + D, T, T, i_Zero, i_Sweep);
        }

        /// <summary>
        /// Establece el punto actual a las coordenadas dadas.
        /// </summary>
        /// <param name="x0">Punto inicial.</param>
        /// <param name="xf">Punto final.</param>
        /// <param name="state">Estado del punto.</param>
        public void LinePos(int x0, int xf, bool state)
        {
            activex0 = x0; activexf = xf; activestate = state;
            this.Refresh();
        }
        #endregion

        #region Methods Event Objects Handlers
        void timer1_Tick(object sender, EventArgs e)
        {
            #region Entering
            if (i_mode == 0)
            {
                if (Math.Abs(baseColorOn.R - R0) > speed)
                    i_fR = speed;
                else
                    i_fR = 1;
                if (Math.Abs(baseColorOn.G - G0) > speed)
                    i_fG = speed;
                else
                    i_fG = 1;
                if (Math.Abs(baseColorOn.B - B0) > speed)
                    i_fB = speed;
                else
                    i_fB = 1;

                if (baseColorOn.R < R0)
                    R0 -= i_fR;
                else if (baseColorOn.R > R0)
                    R0 += i_fR;

                if (baseColorOn.G < G0)
                    G0 -= i_fG;
                else if (baseColorOn.G > G0)
                    G0 += i_fG;

                if (baseColorOn.B < B0)
                    B0 -= i_fB;
                else if (baseColorOn.B > B0)
                    B0 += i_fB;

                if (baseColorOn == Color.FromArgb(R0, G0, B0))
                    timer1.Stop();
                else
                    this.Refresh();
            }
            #endregion

            #region Leaving
            if (i_mode == 1)
            {
                if (Math.Abs(baseColor.R - R0) < speed)
                    i_fR = 1;
                else
                    i_fR = speed;
                if (Math.Abs(baseColor.G - G0) < speed)
                    i_fG = 1;
                else
                    i_fG = speed;
                if (Math.Abs(baseColor.B - B0) < speed)
                    i_fB = 1;
                else
                    i_fB = speed;

                if (baseColor.R < R0)
                    R0 -= i_fR;
                else if (baseColor.R > R0)
                    R0 += i_fR;

                if (baseColor.G < G0)
                    G0 -= i_fG;
                else if (baseColor.G > G0)
                    G0 += i_fG;

                if (baseColor.B < B0)
                    B0 -= i_fB;
                else if (baseColor.B > B0)
                    B0 += i_fB;

                if (baseColor == Color.FromArgb(R0, G0, B0))
                    timer1.Stop();
                else
                    this.Refresh();

            }
            #endregion
        }
        #endregion
    }
}