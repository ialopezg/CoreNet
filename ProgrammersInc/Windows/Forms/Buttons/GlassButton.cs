using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;


namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un botón al estilo Windows Vista.
    /// </summary>
    [ToolboxBitmap(typeof(GlassButton))]
    [ToolboxItem(true)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [Description("Control GlassButton al estilo LongHorn.")]
    public class GlassButton : Button
    {
        #region Fields
        Timer timer;
        #endregion

        #region " Constructors "
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProgrammersInc.Windows.Forms.GlassButton"/>.
        /// </summary>
        public GlassButton()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = animationLength / framesCount;
            base.BackColor = Color.Transparent;
            BackColor = Color.Black;
            ForeColor = Color.White;
            OuterBorderColor = Color.White;
            InnerBorderColor = Color.Black;
            ShineColor = Color.White;
            GlowColor = Color.FromArgb(-7488001);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, false);
        }
        #endregion
        
        #region " Destructors "
        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        #region " Fields and Properties "
        private Color backColor;
        /// <summary>
        /// Obtiene o establece el color de fondo del control.
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public virtual new Color BackColor
        {
            get { return backColor; }
            set
            {
                if (!backColor.Equals(value))
                {
                    backColor = value;
                    UseVisualStyleBackColor = false;
                    CreateFrames();
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el color del texto en el control.
        /// </summary>
        [DefaultValue(typeof(Color), "White")]
        public virtual new Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        private Color innerBorderColor;
        /// <summary>
        /// Obtiene o establece el color del borde interio del control.
        /// </summary>
        [DefaultValue(typeof(Color), "Black"), Category("Appearance"), Description("The inner border color of the control.")]
        public virtual Color InnerBorderColor
        {
            get { return innerBorderColor; }
            set
            {
                if (innerBorderColor != value)
                {
                    innerBorderColor = value;
                    CreateFrames();
                    if (IsHandleCreated)
                        Invalidate();
                    OnInnerBorderColorChanged(EventArgs.Empty);
                }
            }
        }

        private Color outerBorderColor;
        /// <summary>
        /// Obtiene o establece el color del borde exterior del control.
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("Appearance"), Description("The outer border color of the control.")]
        public virtual Color OuterBorderColor
        {
            get { return outerBorderColor; }
            set
            {
                if (outerBorderColor != value)
                {
                    outerBorderColor = value;
                    CreateFrames();
                    if (IsHandleCreated)
                        Invalidate();
                    OnOuterBorderColorChanged(EventArgs.Empty);
                }
            }
        }

        private Color shineColor;
        /// <summary>
        /// Obtiene o establece el color de brillo del control.
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("Appearance"), Description("The shine color of the control.")]
        public virtual Color ShineColor
        {
            get { return shineColor; }
            set
            {
                if (shineColor != value)
                {
                    shineColor = value;
                    CreateFrames();
                    if (IsHandleCreated)
                        Invalidate();
                    OnShineColorChanged(EventArgs.Empty);
                }
            }
        }

        private Color glowColor;
        /// <summary>
        /// Obtiene o establece el color de brillo del control.
        /// </summary>
        [DefaultValue(typeof(Color), "255,141,189,255"), Category("Appearance"), Description("The glow color of the control.")]
        public virtual Color GlowColor
        {
            get { return glowColor; }
            set
            {
                if (glowColor != value)
                {
                    glowColor = value;
                    CreateFrames();
                    if (IsHandleCreated)
                        Invalidate();
                    OnGlowColorChanged(EventArgs.Empty);
                }
            }
        }

        private bool isHovered;
        private bool isFocused;
        private bool isFocusedByKey;
        private bool isKeyDown;
        private bool isMouseDown;
        private bool isPressed { get { return isKeyDown || (isMouseDown && isHovered); } }

        /// <summary>
        /// Obtiene el estado del control botón.
        /// </summary>
        [Browsable(false)]
        public PushButtonState State
        {
            get
            {
                if (!Enabled)
                    return PushButtonState.Disabled;
                if (isPressed)
                    return PushButtonState.Pressed;
                if (isHovered)
                    return PushButtonState.Hot;
                if (isFocused || IsDefault)
                    return PushButtonState.Default;
                
                return PushButtonState.Normal;
            }
        }
        #endregion

        #region " Events "
        /// <summary>
        /// Ocurre cuando el valor de la propiedad <see cref="P:GlassButton.InnerBorderColor"/> ha cambiado de valor.
        /// </summary>
        [Description("Evento que se dispara cuando el valor de la propiedad InnerBorderColor ha cambiado de valor."), Category("Property Changed")]
        public event EventHandler InnerBorderColorChanged;

        /// <summary>
        /// Dispara el evento <see cref="E:GlassButton.InnerBorderColorChanged" />.
        /// </summary>
        /// <param name="e">Una estructura <see cref="T:System.EventArgs"/> que contiene los datos del evento.</param>
        protected virtual void OnInnerBorderColorChanged(EventArgs e)
        {
            if (InnerBorderColorChanged != null)
                InnerBorderColorChanged(this, e);
        }

        /// <summary>
        /// Ocurre cuando el valor de la propiedad <see cref="P:GlassButton.OuterBorderColor"/> ha cambiado de valor.
        /// </summary>
        [Description("Evento que se dispara cuando el valor de la propiedad OuterBorderColor ha cambiado de valor."), Category("Property Changed")]
        public event EventHandler OuterBorderColorChanged;

        /// <summary>
        /// Dispra el evento <see cref="E:GlassButton.OuterBorderColorChanged"/>.
        /// </summary>
        /// <param name="e">Una estructura <see cref="T:System.EventArgs"/> que contiene los datos del evento.</param>
        protected virtual void OnOuterBorderColorChanged(EventArgs e)
        {
            if (OuterBorderColorChanged != null)
                OuterBorderColorChanged(this, e);
        }

        /// <summary>
        /// Ocurre cuando el valor de la propiedad <see cref="P:GlassButton.ShineColor" /> ha cambiado de valor.
        /// </summary>
        [Description("Evento que se dispara cuando el valor de la propiedad ShineColor ha cambiado de valor."), Category("Property Changed")]
        public event EventHandler ShineColorChanged;

        /// <summary>
        /// Dispra el evento <see cref="E:GlassButton.ShineColorChanged"/>.
        /// </summary>
        /// <param name="e">Una estructura <see cref="T:System.EventArgs" /> que contiene los datos del evento.</param>
        protected virtual void OnShineColorChanged(EventArgs e)
        {
            if (ShineColorChanged != null)
                ShineColorChanged(this, e);
        }

        /// <summary>
        /// Ocurre cuando el valor de la propiedad <see cref="P:GlassButton.GlowColor" /> ha cambiado de valor.
        /// </summary>
        [Description("Evento que se dispara cuando el valor de la propiedad GlowColor  ha cambiado de valor."), Category("Property Changed")]
        public event EventHandler GlowColorChanged;

        /// <summary>
        /// Dispra el evento <see cref="E:GlassButton.GlowColorChanged" />.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> que contiene los datos del evento.</param>
        protected virtual void OnGlowColorChanged(EventArgs e)
        {
            if (GlowColorChanged != null)
                InnerBorderColorChanged(this, e);
        }
        #endregion

        #region " Overrided Methods "
        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.SizeChanged"/>.
        /// </summary>
        /// <param name="e">Una estructura <see cref="T:System.EventArgs"/> que contien los datos del evento.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            CreateFrames();
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.Click"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="System.EventArgs" /> conteniendo los datos del evento.</param>
        protected override void OnClick(EventArgs e)
        {
            isKeyDown = isMouseDown = false;
            base.OnClick(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.Enter"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.EventArgs" /> conteniendo los datos del evento.</param>
        protected override void OnEnter(EventArgs e)
        {
            isFocused = isFocusedByKey = true;
            base.OnEnter(e);
        }

        /// <summary>
        /// Dispara el evento  <see cref="E:System.Windows.Forms.Control.Leave"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.EventArgs" /> conteniendo los datos del evento.</param>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            isFocused = isFocusedByKey = isKeyDown = isMouseDown = false;
            Invalidate();
        }

        /// <summary>
        /// Dispara el evento <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" />.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.KeyEventArgs" /> conteniendo los datos del evento.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                isKeyDown = true;
                Invalidate();
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.KeyEventArgs" /> conteniendo los datos del evento.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (isKeyDown && e.KeyCode == Keys.Space)
            {
                isKeyDown = false;
                Invalidate();
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.MouseDown"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.MouseEventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!isMouseDown && e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                isFocusedByKey = false;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.MouseUp"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.MouseEventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.MouseEventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.None)
            {
                if (!ClientRectangle.Contains(e.X, e.Y))
                {
                    if (isHovered)
                    {
                        isHovered = false;
                        Invalidate();
                    }
                }
                else if (!isHovered)
                {
                    isHovered = true;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.MouseEnter" />.
        /// </summary>
        /// <param name="e">La instancia <see cref="System.EventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            FadeIn();
            Invalidate();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Dispara el evento <see cref="E:System.Windows.Forms.Control.MouseLeave"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="System.EventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            FadeOut();
            Invalidate();
            base.OnMouseLeave(e);
        }
        #endregion

        #region " Painting "
        /// <summary>
        /// Dispara el evento <see cref="M:System.Windows.Forms.ButtonBase.OnPaint(System.Windows.Forms.PaintEventArgs)"/>.
        /// </summary>
        /// <param name="e">La instancia <see cref="T:System.Windows.Forms.PaintEventArgs"/> conteniendo los datos del evento.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawButtonBackgroundFromBuffer(e.Graphics);
            DrawForegroundFromButton(e);
            DrawButtonForeground(e.Graphics);
        }

        void DrawButtonBackgroundFromBuffer(Graphics graphics)
        {
            int frame;
            if (!Enabled)
                frame = FRAME_DISABLED;
            else if (isPressed)
                frame = FRAME_PRESSED;
            else if (!isAnimating && currentFrame == 0)
                frame = FRAME_NORMAL;
            else
            {
                if (!HasAnimationFrames)
                    CreateFrames(true);
                frame = FRAME_ANIMATED + currentFrame;
            }
            if (frames == null)
                CreateFrames();
            graphics.DrawImage(frames[frame], Point.Empty);
        }

        /// <summary>
        /// Crea la imagen de fondo que se asociará al control.
        /// </summary>
        /// <param name="pressed">Indica si el botón está presionado.</param>
        /// <param name="hovered">Indica si el botón está Hovered.</param>
        /// <param name="animating">Indica si el botón está animado.</param>
        /// <param name="enabled">Indica si el botón está Enabled.</param>
        /// <param name="glowOpacity">Indica si el botón está opaco.</param>
        /// <returns></returns>
        public Image CreateBackgroundFrame(bool pressed, bool hovered,
            bool animating, bool enabled, float glowOpacity)
        {
            Rectangle rect = ClientRectangle;
            if (rect.Width <= 0)
                rect.Width = 1;
            if (rect.Height <= 0)
                rect.Height = 1;
            Image img = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                g.Clear(Color.Transparent);
                DrawButtonBackground(g, rect, pressed, hovered, animating, enabled,
                    outerBorderColor, backColor, glowColor, shineColor, innerBorderColor,
                    glowOpacity);
            }
            return img;
        }

        static void DrawButtonBackground(Graphics g, Rectangle rectangle,
            bool pressed, bool hovered, bool animating, bool enabled,
            Color outerBorderColor, Color backColor, Color glowColor, Color shineColor,
            Color innerBorderColor, float glowOpacity)
        {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            #region " white border "
            Rectangle rect = rectangle;
            rect.Width--;
            rect.Height--;
            using (GraphicsPath bw = CreateRoundRectangle(rect, 4))
            {
                using (Pen p = new Pen(outerBorderColor))
                {
                    g.DrawPath(p, bw);
                }
            }
            #endregion

            rect.X++;
            rect.Y++;
            rect.Width -= 2;
            rect.Height -= 2;
            Rectangle rect2 = rect;
            rect2.Height >>= 1;

            #region " content "
            using (GraphicsPath bb = CreateRoundRectangle(rect, 2))
            {
                int opacity = pressed ? 0xcc : 0x7f;
                using (Brush br = new SolidBrush(Color.FromArgb(opacity, backColor)))
                {
                    g.FillPath(br, bb);
                }
            }
            #endregion

            #region " glow "
            if ((hovered || animating) && !pressed)
            {
                using (GraphicsPath clip = CreateRoundRectangle(rect, 2))
                {
                    g.SetClip(clip, CombineMode.Intersect);
                    using (GraphicsPath brad = CreateBottomRadialPath(rect))
                    {
                        using (PathGradientBrush pgr = new PathGradientBrush(brad))
                        {
                            unchecked
                            {
                                int opacity = (int)(0xB2 * glowOpacity + .5f);
                                RectangleF bounds = brad.GetBounds();
                                pgr.CenterPoint = new PointF((bounds.Left + bounds.Right) / 2f, (bounds.Top + bounds.Bottom) / 2f);
                                pgr.CenterColor = Color.FromArgb(opacity, glowColor);
                                pgr.SurroundColors = new Color[] { Color.FromArgb(0, glowColor) };
                            }
                            g.FillPath(pgr, brad);
                        }
                    }
                    g.ResetClip();
                }
            }
            #endregion

            #region " shine "
            if (rect2.Width > 0 && rect2.Height > 0)
            {
                rect2.Height++;
                using (GraphicsPath bh = CreateTopRoundRectangle(rect2, 2))
                {
                    rect2.Height++;
                    int opacity = 0x99;
                    if (pressed | !enabled)
                    {
                        opacity = (int)(.4f * opacity + .5f);
                    }
                    using (LinearGradientBrush br = new LinearGradientBrush(rect2, Color.FromArgb(opacity, shineColor), Color.FromArgb(opacity / 3, shineColor), LinearGradientMode.Vertical))
                    {
                        g.FillPath(br, bh);
                    }
                }
                rect2.Height -= 2;
            }
            #endregion

            #region " black border "
            using (GraphicsPath bb = CreateRoundRectangle(rect, 3))
            {
                using (Pen p = new Pen(innerBorderColor))
                {
                    g.DrawPath(p, bb);
                }
            }
            #endregion

            g.SmoothingMode = sm;
        }

        void DrawButtonForeground(Graphics g)
        {
            if (Focused && ShowFocusCues)
            {
                Rectangle rect = ClientRectangle;
                rect.Inflate(-4, -4);
                System.Windows.Forms.ControlPaint.DrawFocusRectangle(g, rect);
            }
        }

        Button imageButton;
        private void DrawForegroundFromButton(PaintEventArgs e)
        {
            if (imageButton == null)
            {
                imageButton = new Button();
                imageButton.Parent = new TransparentControl();
                imageButton.BackColor = Color.Transparent;
                imageButton.FlatAppearance.BorderSize = 0;
                imageButton.FlatStyle = FlatStyle.Flat;
            }
            imageButton.AutoEllipsis = AutoEllipsis;
            if (Enabled)
                imageButton.ForeColor = ForeColor;
            else
                imageButton.ForeColor = Color.FromArgb((3 * ForeColor.R + backColor.R) >> 2,
                    (3 * ForeColor.G + backColor.G) >> 2,
                    (3 * ForeColor.B + backColor.B) >> 2);
            imageButton.Font = Font;
            imageButton.RightToLeft = RightToLeft;
            imageButton.Image = Image;
            imageButton.ImageAlign = ImageAlign;
            imageButton.ImageIndex = ImageIndex;
            imageButton.ImageKey = ImageKey;
            imageButton.ImageList = ImageList;
            imageButton.Padding = Padding;
            imageButton.Size = Size;
            imageButton.Text = Text;
            imageButton.TextAlign = TextAlign;
            imageButton.TextImageRelation = TextImageRelation;
            imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering;
            imageButton.UseMnemonic = UseMnemonic;
            InvokePaint(imageButton, e);
        }

        class TransparentControl : Control
        {
            protected override void OnPaintBackground(PaintEventArgs pevent) { }
            protected override void OnPaint(PaintEventArgs e) { }
        }

        static GraphicsPath CreateRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;
            path.AddArc(l, t, d, d, 180, 90);
            path.AddLine(l + radius, t, l + w - radius, t);
            path.AddArc(l + w - d, t, d, d, 270, 90);
            path.AddLine(l + w, t + radius, l + w, t + h - radius);
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90);
            path.AddLine(l + w - radius, t + h, l + radius, t + h);
            path.AddArc(l, t + h - d, d, d, 90, 90);
            path.AddLine(l, t + h - radius, l, t + radius);
            path.CloseFigure();
            return path;
        }

        static GraphicsPath CreateTopRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;
            path.AddArc(l, t, d, d, 180, 90);
            path.AddLine(l + radius, t, l + w - radius, t);
            path.AddArc(l + w - d, t, d, d, 270, 90);
            path.AddLine(l + w, t + radius, l + w, t + h);
            path.AddLine(l + w, t + h, l, t + h);
            path.AddLine(l, t + h, l, t + radius);
            path.CloseFigure();
            return path;
        }

        static GraphicsPath CreateBottomRadialPath(Rectangle rectangle)
        {
            GraphicsPath path = new GraphicsPath();
            RectangleF rect = rectangle;
            rect.X -= rect.Width * .35f;
            rect.Y -= rect.Height * .15f;
            rect.Width *= 1.7f;
            rect.Height *= 2.3f;
            path.AddEllipse(rect);
            path.CloseFigure();
            return path;
        }
        #endregion

        #region " Unused Properties & Events "
        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new FlatButtonAppearance FlatAppearance
        {
            get { return base.FlatAppearance; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new FlatStyle FlatStyle
        {
            get { return base.FlatStyle; }
            set { base.FlatStyle = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool UseVisualStyleBackColor
        {
            get { return base.UseVisualStyleBackColor; }
            set { base.UseVisualStyleBackColor = value; }
        }
        #endregion

        #region " Animation Support "
        private List<Image> frames;

        private const int FRAME_DISABLED = 0;
        private const int FRAME_PRESSED = 1;
        private const int FRAME_NORMAL = 2;
        private const int FRAME_ANIMATED = 3;

        bool HasAnimationFrames
        {
            get { return frames != null && frames.Count > FRAME_ANIMATED; }
        }

        void CreateFrames()
        {
            CreateFrames(false);
        }

        void CreateFrames(bool withAnimationFrames)
        {
            DestroyFrames();
            if (!IsHandleCreated)
                return;
            if (frames == null)
                frames = new List<Image>();
            frames.Add(CreateBackgroundFrame(false, false, false, false, 0));
            frames.Add(CreateBackgroundFrame(true, true, false, true, 0));
            frames.Add(CreateBackgroundFrame(false, false, false, true, 0));
            if (!withAnimationFrames)
                return;
            for (int i = 0; i < framesCount; i++)
                frames.Add(CreateBackgroundFrame(false, true, true, true, (float)i / (framesCount - 1F)));
        }

        void DestroyFrames()
        {
            if (frames != null)
            {
                while (frames.Count > 0)
                {
                    frames[frames.Count - 1].Dispose();
                    frames.RemoveAt(frames.Count - 1);
                }
            }
        }

        const int animationLength = 300;
        const int framesCount = 10;
        int currentFrame;
        int direction;

        bool isAnimating
        {
            get { return direction != 0; }
        }

        void FadeIn()
        {
            direction = 1;
            timer.Enabled = true;
        }

        void FadeOut()
        {
            direction = -1;
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!timer.Enabled)
                return;
            Refresh();
            currentFrame += direction;
            if (currentFrame == -1)
            {
                currentFrame = 0;
                timer.Enabled = false;
                direction = 0;
                return;
            }
            if (currentFrame == framesCount)
            {
                currentFrame = framesCount - 1;
                timer.Enabled = false;
                direction = 0;
            }
        }
        #endregion
    }
}
