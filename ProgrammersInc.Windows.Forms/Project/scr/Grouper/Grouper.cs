using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Un control GroupBox personalizado con algunos efectos de dibujo.
    /// </summary>
    [ToolboxBitmap(typeof(Grouper), "ProgrammersInc.Resources.Grouper.bmp")]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [Description("Un control GroupBox personalizado con algunos efectos de dibujo.")]
    public class Grouper : UserControl
    {
        #region Enumerations
        /// <summary>
        /// Define el tipo de degrado a aplicarse sobre el control.
        /// </summary>
        public enum GroupBoxGradientMode
        {
            /// <summary>
            /// Sin degradado.
            /// </summary>
            None = 4,
            /// <summary>
            /// Especifica un degradado que va desde la esquina superior derecha 
            /// hacia la esquina inferior izquierda.
            /// </summary>
            BackwardDiagonal = 3,
            /// <summary>
            /// Especifica un degradado que va desde la esquina superior izquierda 
            /// hacia la esquina inferior derecha.
            /// </summary>
            ForwardDiagonal = 2,
            /// <summary>
            /// Especifica un degradado que va de derecha a izquierda.
            /// </summary>
            Horizontal = 0,
            /// <summary>
            /// Especifica un degradado que va de arriba hacia abajo.
            /// </summary>
            Vertical = 1
        }
        #endregion

        #region Variables Implementation
        Container components = null;
        #endregion

        #region Contructors, Initializers && Destructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="Grouper"/>.
        /// </summary>
        public Grouper()
        {
            InitializeStyles();
            InitializeGrouper();
        }

        void InitializeStyles()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        void InitializeGrouper()
        {
            components = new Container();
            DockPadding.All = 20;
            Name = "GroupBox";
            Size = new Size(368, 288);
        }

        /// <summary>
        /// Libera todos los recursos que se esten usando.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (components != null)
                    components.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region Properties
        Color backgroundColor = Color.White;
        /// <summary>
        /// Obtiene o establece el color de fondo del control.
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set 
            {
                backgroundColor = value;
                Refresh();
            }
        }

        Color backgroundGradientColor = Color.White;
        /// <summary>
        /// Obtiene o establece el color con el cual se combinará la propiedad BackgroundColor y
        /// dibujar un degradado en el control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        [Description("Color con el cual se combinará la propiedad BackgroundColor y dibujar un degradado en el control")]
        public Color BackgroundGradientColor
        { 
            get { return backgroundGradientColor; } 
            set 
            {
                backgroundGradientColor = value;
                this.Refresh(); 
            }
        }

        Color borderColor = Color.Black;
        /// <summary>
        /// Obtiene o establece el color del borde del control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        [Description("Color del borde del control.")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Refresh();
            }
        }

        float borderThickness = 1;
        /// <summary>
        /// Obtiene o establece el ancho del border del control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(float), "1")]
        [Description("Ancho del border del control.")]
        public float BorderThickness
        {
            get { return borderThickness; }
            set 
            {
                if (value > 3)
                    borderThickness = 3;
                else
                {
                    if (value < 1)
                        borderThickness = 1;
                    else
                        borderThickness = value;
                }
                Refresh();
            }
        }

        Color customGroupBoxColor = Color.White;
        /// <summary>
        /// Obtiene o establece el color que se usará para pintar el encabezado del control, si la propiedad
        /// PaintGroupBox es establecida a <c>true</c>.
        /// </summary>
        [Category("Appearance")]
        [Description("Color que se usará para pintar el encabezado del control, si la propiedad PaintGroupBox es establecida a true.")]
        public System.Drawing.Color CustomGroupBoxColor
        {
            get { return customGroupBoxColor; }
            set 
            {
                customGroupBoxColor = value;
                this.Refresh();
            }
        }

        GroupBoxGradientMode gradientMode = GroupBoxGradientMode.None;
        /// <summary>
        /// Obtiene o establece el tipo de relleno del color de degradado del control.
        /// </summary>
        [Category("Appearance")]
        [Description("Relleno de color del degradado del control.")]
        public GroupBoxGradientMode GradientMode
        { 
            get { return gradientMode; } 
            set 
            { 
                gradientMode = value;
                this.Refresh(); 
            }
        }

        Image image = null;
        /// <summary>
        /// Obtiene o establece la imagen (16x16) a mostrarse en el encabezado del control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(null)]
        [Description("Imagen (16x16) a mostrarse en el encabezado del control.")]
        public Image Image 
        { 
            get { return image; }
            set
            { 
                image = value;
                this.Refresh(); 
            } 
        }

        bool paintGroupBox = false;
        /// <summary>
        /// Obtiene o establece un valor indicando si el encabezado usará colores personalizados.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("This feature will paint the group title background to the CustomGroupBoxColor.")]
        public bool PaintGroupBox
        { 
            get { return paintGroupBox; } 
            set 
            {
                paintGroupBox = value;
                this.Refresh(); 
            }
        }

        int roundCorners = 10;
        /// <summary>
        /// Obtiene o establece el diámetro de doblez en las esquinas.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(10)]
        [Description("Diámetro de doblez en las esquinas.")]
        public int RoundCorners
        {
            get { return roundCorners; }
            set
            {
                if (value > 25)
                    roundCorners = 25;
                else
                {
                    if (value < 1)
                        roundCorners = 1;
                    else
                        roundCorners = value;
                }
                Refresh();
            }
        }

        bool shadow = false;
        /// <summary>
        /// Obtiene o establece si se aplicará el efecto sombra en el control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Indica si se aplicará el efecto sombra en el control.")]
        public bool Shadow
        {
            get { return shadow; }
            set 
            {
                shadow = value;
                Refresh();
            }
        }

        Color shadowColor = Color.DarkGray;
        /// <summary>
        /// Obtiene o establece el color de la sombra para el control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DarkGray")]
        [Description("Color de la sombra del control.")]
        public Color ShadowColor
        {
            get { return shadowColor; }
            set 
            {
                shadowColor = value;
                Refresh();
            }
        }

        int shadowThickness = 3;
        /// <summary>
        /// Obtiene o establece el ancho del borde para el efecto de sombra en el control.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(3)]
        [Description("Ancho del borde para el efecto de sombra en el control.")]
        public int ShadowThickness
        {
            get { return shadowThickness; }
            set 
            {
                if (value > 10)
                    shadowThickness = 10;
                else
                {
                    if (value < 1)
                        shadowThickness = 1;
                    else
                        shadowThickness = value;
                }

                Refresh();
            }
        }

        /// <summary>
        /// Obtiene o establece el texto asociado al control.
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("Texto asociado al control.")]
        public override string Text
        {
            get { return base.Text; }
            set 
            { 
                base.Text = value;
                Refresh();
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.Paint"/>.
        /// </summary>
        /// <param name="e">Una instancia <see cref="System.Windows.Forms.PaintEventArgs"/>
        /// conteniendo los datos del evento.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            PaintBack(e.Graphics);
            PaintGroupText(e.Graphics);
        }

        /// <summary>
        /// Provoca el evento <see cref="System.Windows.Forms.Control.Resize"/>.
        /// </summary>
        /// <param name="e">Una instancia <see cref="System.EventArgs"/>
        /// conteniendo los datos del evento.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Refresh();
        }
        #endregion

        #region Private Methods
        void PaintBack(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int arcWidth = roundCorners * 2;
            int arcHeight = roundCorners * 2;
            int arcX1 = 0;
            int arcX2 = (shadow) ? (Width - (arcWidth + 1)) - shadowThickness : Width - (arcWidth + 1);
            int arcY1 = 10;
            int arcY2 = (shadow) ? (Height - (arcHeight + 1)) - shadowThickness : Height - (arcHeight + 1);
            
            GraphicsPath path = new GraphicsPath();
            Brush borderBrush = new SolidBrush(borderColor);
            Pen borderPen = new Pen(borderBrush, borderThickness);
            LinearGradientBrush backgroundGradientBrush = null;
            Brush backgroundBrush = new SolidBrush(backgroundColor);
            SolidBrush shadowBrush = null;
            GraphicsPath shadowPath = null;

            if (shadow)
            {
                shadowBrush = new SolidBrush(shadowColor);
                shadowPath = new GraphicsPath();
                shadowPath.AddArc(arcX1 + shadowThickness, arcY1 + shadowThickness, arcWidth, arcHeight, 180, 90);
                shadowPath.AddArc(arcX2 + shadowThickness, arcY1 + shadowThickness, arcWidth, arcHeight, 270, 90);
                shadowPath.AddArc(arcX2 + shadowThickness, arcY2 + shadowThickness, arcWidth, arcHeight, 360, 90);
                shadowPath.AddArc(arcX1 + shadowThickness, arcY2 + shadowThickness, arcWidth, arcHeight, 90, 90);
                shadowPath.CloseAllFigures();

                g.FillPath(shadowBrush, shadowPath);
            }

            path.AddArc(arcX1, arcY1, arcWidth, arcHeight, 180, 90);
            path.AddArc(arcX2, arcY1, arcWidth, arcHeight, 270, 90); 
            path.AddArc(arcX2, arcY2, arcWidth, arcHeight, 360, 90); 
            path.AddArc(arcX1, arcY2, arcWidth, arcHeight, 90, 90);
            path.CloseAllFigures();

            if (gradientMode == GroupBoxGradientMode.None)
                g.FillPath(backgroundBrush, path);
            else
            {
                backgroundGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height),
                    backgroundColor, backgroundGradientColor, (LinearGradientMode)gradientMode);

                g.FillPath(backgroundGradientBrush, path);
            }

            g.DrawPath(borderPen, path);

            if (path != null)
                path.Dispose();
            if (borderBrush != null)
                borderBrush.Dispose();
            if (borderPen != null)
                borderPen.Dispose();
            if (backgroundGradientBrush != null)
                backgroundGradientBrush.Dispose();
            if (backgroundBrush != null)
                backgroundBrush.Dispose();
            if (shadowBrush != null)
                shadowBrush.Dispose();
            if (shadowPath != null)
                shadowPath.Dispose();
        }

        void PaintGroupText(Graphics g)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            SizeF StringSize = g.MeasureString(Text, Font);
            Size StringSize2 = StringSize.ToSize();
            if (image != null) { StringSize2.Width += 18; }
            int ArcWidth = this.RoundCorners;
            int ArcHeight = this.RoundCorners;
            int ArcX1 = 20;
            int ArcX2 = (StringSize2.Width + 34) - (ArcWidth + 1);
            int ArcY1 = 0;
            int ArcY2 = 24 - (ArcHeight + 1);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Brush BorderBrush = new SolidBrush(this.BorderColor);
            System.Drawing.Pen BorderPen = new Pen(BorderBrush, this.BorderThickness);
            System.Drawing.Drawing2D.LinearGradientBrush BackgroundGradientBrush = null;
            System.Drawing.Brush BackgroundBrush = (paintGroupBox) ? new SolidBrush(customGroupBoxColor) 
                : new SolidBrush(backgroundColor);
            System.Drawing.SolidBrush TextColorBrush = new SolidBrush(ForeColor);
            System.Drawing.SolidBrush ShadowBrush = null;
            System.Drawing.Drawing2D.GraphicsPath ShadowPath = null;

            if (shadow)
            {
                ShadowBrush = new SolidBrush(shadowColor);
                ShadowPath = new System.Drawing.Drawing2D.GraphicsPath();
                ShadowPath.AddArc(ArcX1 + (this.ShadowThickness - 1), ArcY1 + (this.ShadowThickness - 1), ArcWidth, ArcHeight, 180, 90);
                ShadowPath.AddArc(ArcX2 + (this.ShadowThickness - 1), ArcY1 + (this.ShadowThickness - 1), ArcWidth, ArcHeight, 270, 90);
                ShadowPath.AddArc(ArcX2 + (this.ShadowThickness - 1), ArcY2 + (this.ShadowThickness - 1), ArcWidth, ArcHeight, 360, 90);
                ShadowPath.AddArc(ArcX1 + (this.ShadowThickness - 1), ArcY2 + (this.ShadowThickness - 1), ArcWidth, ArcHeight, 90, 90);
                ShadowPath.CloseAllFigures();

                g.FillPath(ShadowBrush, ShadowPath);
            }

            path.AddArc(ArcX1, ArcY1, ArcWidth, ArcHeight, 180, 90);
            path.AddArc(ArcX2, ArcY1, ArcWidth, ArcHeight, 270, 90);
            path.AddArc(ArcX2, ArcY2, ArcWidth, ArcHeight, 360, 90);
            path.AddArc(ArcX1, ArcY2, ArcWidth, ArcHeight, 90, 90);
            path.CloseAllFigures();

            if (paintGroupBox)
                g.FillPath(BackgroundBrush, path);
            else
            {
                if (gradientMode == GroupBoxGradientMode.None)
                    g.FillPath(BackgroundBrush, path);
                else
                {
                    {
                        BackgroundGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height),
                            backgroundColor, backgroundGradientColor, (LinearGradientMode)gradientMode);

                        g.FillPath(BackgroundGradientBrush, path);
                    }
                }
            }

            g.DrawPath(BorderPen, path);

            int CustomStringWidth = (image != null) ? 44 : 28;
            g.DrawString(Text,Font, TextColorBrush, CustomStringWidth, 5);

            if (image != null)
                g.DrawImage(image, 28, 4, 16, 16);

            if (path != null) 
                path.Dispose();
            if (BorderBrush != null) 
                BorderBrush.Dispose();
            if (BorderPen != null) 
                BorderPen.Dispose();
            if (BackgroundGradientBrush != null)
                BackgroundGradientBrush.Dispose();
            if (BackgroundBrush != null) 
                BackgroundBrush.Dispose();
            if (TextColorBrush != null)
                TextColorBrush.Dispose();
            if (ShadowBrush != null)
                ShadowBrush.Dispose();
            if (ShadowPath != null)
                ShadowPath.Dispose();
        }
        #endregion
    }
}