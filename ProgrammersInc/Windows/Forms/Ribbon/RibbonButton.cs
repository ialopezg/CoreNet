/*
 
 2008 José Manuel Menéndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms
{
    [Designer(typeof(RibbonButtonDesigner))]
    public class RibbonButton : RibbonItem, IContainsRibbonComponents
    {
        #region Fields

        private const int arrowWidth = 5;
        private RibbonButtonStyle _style;
        private Rectangle _dropDownBounds;
        private Rectangle _buttonFaceBounds;
        private RibbonItemCollection _dropDownItems;
        private bool _dropDownPressed;
        private bool _dropDownSelected;
        private Image _smallImage; 
        private Size _dropDownArrowSize;
        private Padding _dropDownMargin;
        private bool _dropDownVisible;
        private RibbonDropDown _dropDown;
        private Rectangle _imageBounds;
        private Rectangle _textBounds;
        private bool _dropDownResizable;

       #endregion

        #region Events

        /// <summary>
        /// Occurs when the dropdown is about to be displayed
        /// </summary>
        public event EventHandler DropDownShowing; 

        #endregion

        #region Ctors

        public RibbonButton(string text)
            : this()
        {
            Text = text;
        }

        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="image">Image of the button (32 x 32 suggested)</param>
        /// <param name="smallImage">Image of the button when in medium of compact mode (16 x 16 suggested)</param>
        /// <param name="style">Style of the button</param>
        /// <param name="text">Text of the button</param>
        public RibbonButton()
        {
            _dropDownItems = new RibbonItemCollection();
            _dropDownArrowSize = new Size(5, 3);
            _dropDownMargin = new Padding(6);
            Image = CreateImage(32);
            SmallImage = CreateImage(16);
        }

        private Image CreateImage(int size)
        {
            Bitmap bmp = new Bitmap(size, size);

            //using (Graphics g = Graphics.FromImage(bmp))
            //{
            //    g.Clear(Color.FromArgb(50, Color.Navy));
            //}

            return bmp;
        }

        #endregion

        #region Props

        /// <summary>
        /// Gets or sets a value indicating if the DropDown should be resizable
        /// </summary>
        [DefaultValue(false)]
        [Description("Makes the DropDown resizable with a grip on the corner")]
        public bool DropDownResizable
        {
            get { return _dropDownResizable; }
            set { _dropDownResizable = value; }
        }

        /// <summary>
        /// Gets the bounds where the <see cref="Image"/> or <see cref="SmallImage"/> will be drawn.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ImageBounds
        {
            get { return _imageBounds; }
        }

        /// <summary>
        /// Gets the bounds where the <see cref="Text"/> of the button will be drawn
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle TextBounds
        {
            get { return _textBounds; }
        }


        /// <summary>
        /// Gets if the DropDown is currently visible
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DropDownVisible
        {
            get { return _dropDownVisible; }
        }

        /// <summary>
        /// Gets or sets the size of the dropdown arrow
        /// </summary>
        public Size DropDownArrowSize
        {
            get { return _dropDownArrowSize; }
            set { _dropDownArrowSize = value; if (Owner != null) Owner.OnRegionsChanged(); }
        }

        /// <summary>
        /// Gets the style of the button
        /// </summary>
        public RibbonButtonStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;

                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        /// Gets the collection of items shown on the dropdown pop-up when Style allows it
        /// </summary>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Content)]
        public RibbonItemCollection DropDownItems
        {
            get
            {
                return _dropDownItems;
            }
        }

        /// <summary>
        /// Gets the bounds of the button face
        /// </summary>
        /// <remarks>When Style is different from SplitDropDown and SplitBottomDropDown, ButtonFaceBounds is the same area than DropDownBounds</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Rectangle ButtonFaceBounds
        {
            get
            {
                return _buttonFaceBounds;
            }
        }

        /// <summary>
        /// Gets the bounds of the dropdown button
        /// </summary>
        /// <remarks>When Style is different from SplitDropDown and SplitBottomDropDown, ButtonFaceBounds is the same area than DropDownBounds</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Rectangle DropDownBounds
        {
            get
            {
                return _dropDownBounds;
            }
        }

        /// <summary>
        /// Gets if the dropdown part of the button is selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DropDownSelected
        {
            get
            {
                return _dropDownSelected;
            }
        }

        /// <summary>
        /// Gets if the dropdown part of the button is pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DropDownPressed
        {
            get
            {
                return _dropDownPressed;
            }
        }

        /// <summary>
        /// Gets or sets the image of the button when in compact of medium size mode
        /// </summary>
        [DefaultValue(null)]
        public Image SmallImage
        {
            get
            {
                return _smallImage;
            }
            set 
            {
                _smallImage = value;

                if (Owner != null) Owner.OnRegionsChanged();
            }
        } 

        #endregion

        #region Methods

        internal override void SetPressed(bool pressed)
        {
            base.SetPressed(pressed);
        }

        internal override void SetOwner(Ribbon owner)
        {
            base.SetOwner(owner);

            if (_dropDownItems != null) _dropDownItems.SetOwner(owner);
        }

        internal override void SetOwnerPanel(RibbonPanel ownerPanel)
        {
            base.SetOwnerPanel(ownerPanel);

            if (_dropDownItems != null) _dropDownItems.SetOwnerPanel(ownerPanel);
        }

        internal override void SetOwnerTab(RibbonTab ownerTab)
        {
            base.SetOwnerTab(ownerTab);

            if (_dropDownItems != null) _dropDownItems.SetOwnerTab(ownerTab);
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Owner == null) return;
            RibbonElementSizeMode theSize = GetNearestSize(e.Mode);

            Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(Owner, e.Graphics, e.Clip, this));

            Rectangle img = Rectangle.Empty;

            if (theSize == RibbonElementSizeMode.Large)
            {
                if (Image != null)
                {
                    img = new Rectangle(
                    Bounds.Left + ((Bounds.Width - Image.Width) / 2),
                    Bounds.Top + Owner.ItemMargin.Top,
                    Image.Width,
                    Image.Height);

                    Owner.Renderer.OnRenderRibbonItemImage(
                        new RibbonItemBoundsEventArgs(Owner, e.Graphics, e.Clip, this, img));
                }
            }
            else
            {
                

                if (SmallImage != null)
                {
                    img = new Rectangle(
                    Bounds.Left + Owner.ItemMargin.Left,
                    Bounds.Top + ((Bounds.Height - SmallImage.Height) / 2),
                    SmallImage.Width,
                    SmallImage.Height);

                    Owner.Renderer.OnRenderRibbonItemImage(
                        new RibbonItemBoundsEventArgs(Owner, e.Graphics, e.Clip, this, img));
                }
            }

            Rectangle t = Rectangle.Empty;
            int imgw = SmallImage == null ? 16 : SmallImage.Width;
            int imgh = Image == null ? 32 : Image.Height;

            if (theSize == RibbonElementSizeMode.Large)
            {
                t = Rectangle.FromLTRB(
                    Bounds.Left + Owner.ItemMargin.Left,
                    Bounds.Top + Owner.ItemMargin.Top + imgh + Owner.ItemMargin.Vertical,
                    Bounds.Right - Owner.ItemMargin.Right,
                    Bounds.Bottom - Owner.ItemMargin.Bottom);

                if(SizeMode != RibbonElementSizeMode.Compact)
                Owner.Renderer.OnRenderRibbonItemText(
                    new RibbonItemBoundsEventArgs(Owner, e.Graphics, e.Clip, this, t));
            }
            else
            {
                int ddw = Style != RibbonButtonStyle.Normal ? _dropDownMargin.Horizontal : 0;
                t = Rectangle.FromLTRB(
                    Bounds.Left + imgw + Owner.ItemMargin.Horizontal + Owner.ItemMargin.Left,
                    Bounds.Top + Owner.ItemMargin.Top,
                    Bounds.Right - ddw,
                    Bounds.Bottom - Owner.ItemMargin.Bottom);

                if (SizeMode != RibbonElementSizeMode.Compact)
                Owner.Renderer.OnRenderRibbonItemText(
                    new RibbonItemBoundsEventArgs(Owner, e.Graphics, e.Clip, this, t));
            }



        }

        /// <summary>
        /// Sets the bounds of the button
        /// </summary>
        /// <param name="bounds"></param>
        public override void SetBounds(System.Drawing.Rectangle bounds)
        {
            base.SetBounds(bounds);

            RibbonElementSizeMode theSize = GetNearestSize(SizeMode);

            if (theSize == RibbonElementSizeMode.Large)
            {
                if (Image != null)
                {
                    _imageBounds = new Rectangle(
                    Bounds.Left + ((Bounds.Width - Image.Width) / 2),
                    Bounds.Top + Owner.ItemMargin.Top,
                    Image.Width,
                    Image.Height);
                }
                else
                {
                    _imageBounds = new Rectangle(ContentBounds.Location, new Size(32, 32));
                }
            }
            else
            {
                if (SmallImage != null)
                {
                    _imageBounds = new Rectangle(
                        Bounds.Left + Owner.ItemMargin.Left,
                        Bounds.Top + ((Bounds.Height - SmallImage.Height) / 2),
                        SmallImage.Width,
                        SmallImage.Height);
                }
                else
                {
                    _imageBounds = new Rectangle(ContentBounds.Location, new Size(16, 16));
                }
            }

            int imgw = _imageBounds.Width;
            int imgh = _imageBounds.Height;

            if (theSize == RibbonElementSizeMode.Large)
            {
                _textBounds = Rectangle.FromLTRB(
                    Bounds.Left + Owner.ItemMargin.Left,
                    Bounds.Top + Owner.ItemMargin.Top + imgh + Owner.ItemMargin.Vertical,
                    Bounds.Right - Owner.ItemMargin.Right,
                    Bounds.Bottom - Owner.ItemMargin.Bottom);
            }
            else
            {
                int ddw = Style != RibbonButtonStyle.Normal ? _dropDownMargin.Horizontal : 0;
                _textBounds = Rectangle.FromLTRB(
                    Bounds.Left + imgw + Owner.ItemMargin.Horizontal + Owner.ItemMargin.Left,
                    Bounds.Top + Owner.ItemMargin.Top,
                    Bounds.Right - ddw,
                    Bounds.Bottom - Owner.ItemMargin.Bottom);

            }


            if (Style == RibbonButtonStyle.SplitDropDown)
            {
                Rectangle sideBounds = Rectangle.FromLTRB(
                    bounds.Right - _dropDownMargin.Horizontal - 2, 
                    bounds.Top, bounds.Right, bounds.Bottom);

                switch (SizeMode)
                {
                    case RibbonElementSizeMode.Large:
                    case RibbonElementSizeMode.Overflow:
                        _dropDownBounds = Rectangle.FromLTRB(bounds.Left,
                            bounds.Top + Image.Height + Owner.ItemMargin.Vertical,
                            bounds.Right, bounds.Bottom);
                        _buttonFaceBounds = Rectangle.FromLTRB(bounds.Left,
                            bounds.Top, bounds.Right, _dropDownBounds.Top);
                        break;
                    case RibbonElementSizeMode.DropDown:
                    case RibbonElementSizeMode.Medium:
                    case RibbonElementSizeMode.Compact:
                        _dropDownBounds = sideBounds;
                        _buttonFaceBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top,
                            _dropDownBounds.Left, bounds.Bottom);
                        break;
                }
            }
        }

        public static Size MeasureStringLargeSize(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Size sz = g.MeasureString(text, font).ToSize();
            string[] words = text.Split(' ');
            string longestWord = string.Empty;
            int width = sz.Width;

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > longestWord.Length)
                {
                    longestWord = words[i];
                }
            }

            if (words.Length > 1)
            {
                width = Math.Max(sz.Width / 2, g.MeasureString(longestWord, font).ToSize().Width) + 1;
            }
            else
            {
                return g.MeasureString(text, font).ToSize();
            }

            Size rs = g.MeasureString(text, font, width).ToSize();

            return new Size(rs.Width, rs.Height);
        }

        /// <summary>
        /// Measures the size of the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            RibbonElementSizeMode theSize = GetNearestSize(e.SizeMode);
            int widthSum = Owner.ItemMargin.Horizontal;
            int heightSum = Owner.ItemMargin.Vertical;
            int largeHeight = OwnerPanel.ContentBounds.Height - Owner.ItemPadding.Vertical;// -Owner.ItemMargin.Vertical; //58;

            Size simg = SmallImage != null ? SmallImage.Size : Size.Empty;
            Size img = Image != null ? Image.Size : Size.Empty;
            Size sz = Size.Empty;

            switch (theSize)
            {
                case RibbonElementSizeMode.Large:
                case RibbonElementSizeMode.Overflow:
                    sz = MeasureStringLargeSize(e.Graphics, Text, Owner.Font);
                    if (!string.IsNullOrEmpty(Text))
                    {
                        widthSum += Math.Max(sz.Width + 1, img.Width);
                        heightSum = largeHeight;
                    }
                    else
                    {
                        widthSum += img.Width;
                        heightSum += img.Height;
                    }
                    
                    break;
                case RibbonElementSizeMode.DropDown:
                case RibbonElementSizeMode.Medium:
                    sz = e.Graphics.MeasureString(Text, Owner.Font).ToSize();
                    if(!string.IsNullOrEmpty(Text)) widthSum += sz.Width + 1;
                    widthSum += simg.Width + Owner.ItemMargin.Horizontal;
                    heightSum += Math.Max(sz.Height, simg.Height);
                    break;
                case RibbonElementSizeMode.Compact:
                    widthSum += simg.Width;
                    heightSum += simg.Height;
                    break;
                default:
                    throw new ApplicationException("SizeMode not supported: " + e.SizeMode.ToString());
            }

            if (theSize == RibbonElementSizeMode.DropDown)
            {
                heightSum += 2;
            }

            if (Style == RibbonButtonStyle.DropDown)
            {
                widthSum += arrowWidth + Owner.ItemMargin.Right;
            }
            else if (Style == RibbonButtonStyle.SplitDropDown)
            {
                widthSum += arrowWidth + Owner.ItemMargin.Horizontal;
            }

            SetLastMeasuredSize(new Size(widthSum, heightSum));

            return LastMeasuredSize;
        }

        /// <summary>
        /// Sets the value of the DropDownPressed property
        /// </summary>
        /// <param name="pressed">Value that indicates if the dropdown button is pressed</param>
        internal void SetDropDownPressed(bool pressed)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the DropDownSelected property
        /// </summary>
        /// <param name="selected">Value that indicates if the dropdown part of the button is selected</param>
        internal void SetDropDownSelected(bool selected)
        {
            //Dont use, an overflow occours
            throw new Exception();
        }

        /// <summary>
        /// Shows the drop down items of the button, as if the dropdown part has been clicked
        /// </summary>
        public void ShowDropDown()
        {
            if (Style == RibbonButtonStyle.Normal || DropDownItems.Count == 0)
            {
                CloseDropDown();
                return;
            }

            if (Style == RibbonButtonStyle.DropDown)
            {
                SetPressed(true);
            }
            else
            {
                _dropDownPressed = true;
            }

            OnDropDownShowing(EventArgs.Empty);

            _dropDown = new RibbonDropDown(this, DropDownItems, Owner);
            _dropDown.Closed += new EventHandler(_dropDown_Closed);
            _dropDown.ShowSizingGrip = DropDownResizable;

            RibbonDropDown canvasdd = Canvas as RibbonDropDown;
            

            Point location = Point.Empty;
            if (canvasdd != null)
            {
                location = Canvas.PointToScreen(new Point(Bounds.Right, Bounds.Top));
                _dropDown.PreviousDropDown = Canvas as RibbonDropDown;
                canvasdd.NextDropDown = _dropDown;
            }
            else
            {
                location = Canvas.PointToScreen(new Point(Bounds.Left, Bounds.Bottom));
            }

            SetDropDownVisible(true);
            _dropDown.SelectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            _dropDown.Show(location);
            
            
        }

        private void _dropDown_Closed(object sender, EventArgs e)
        {
            SetPressed(false);

            _dropDownPressed = false;

            SetDropDownVisible(false);

            SetSelected(false);

            RedrawItem();
        }

        private void dropDown_FormClosed(object sender, FormClosedEventArgs e)
        {
            _dropDownPressed = false;
            SetDropDownVisible(false);
            RedrawItem();
        }

        /// <summary>
        /// Ignores deactivation of canvas if it is a volatile window
        /// </summary>
        private void IgnoreDeactivation()
        {
            if (Canvas is RibbonPanelPopup)
            {
                (Canvas as RibbonPanelPopup).IgnoreNextClickDeactivation();
            }

            if (Canvas is RibbonDropDown)
            {
                (Canvas as RibbonDropDown).IgnoreNextClickDeactivation();
            }
        }

        /// <summary>
        /// Closes the DropDown if opened
        /// </summary>
        public void CloseDropDown()
        {
            if (_dropDown != null)
            {
                _dropDown.Close();
            }

            SetDropDownVisible(false);
        }

        /// <summary>
        /// Overriden. Informs the button text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("RibbonButton: {0}", Text);
        }

        /// <summary>
        /// Sets the value of DropDownVisible
        /// </summary>
        /// <param name="visible"></param>
        internal void SetDropDownVisible(bool visible)
        {
            _dropDownVisible = visible;
        }

        /// <summary>
        /// Raises the DropDownShowing event
        /// </summary>
        /// <param name="e"></param>
        public void OnDropDownShowing(EventArgs e)
        {
            if (DropDownShowing != null)
            {
                DropDownShowing(this, e);
            }
        }

        #endregion

        #region Overrides

        internal override void SetSizeMode(RibbonElementSizeMode sizeMode)
        {
            

            if (sizeMode == RibbonElementSizeMode.Overflow)
            {
                base.SetSizeMode(RibbonElementSizeMode.Large);
            }
            else
            {
                base.SetSizeMode(sizeMode);
            }
        }

        internal override void SetSelected(bool selected)
        {
            base.SetSelected(selected);

            SetPressed(false);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (!Enabled) return;

            if ((DropDownSelected || Style == RibbonButtonStyle.DropDown) && DropDownItems.Count > 0)
            {
                _dropDownPressed = true;
                ShowDropDown();
            }

            base.OnMouseDown(e);
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!Enabled) return;

            //Detect mouse on dropdwon
            if (Style == RibbonButtonStyle.SplitDropDown)
            {
                bool lastState = _dropDownSelected;

                if (DropDownBounds.Contains(e.X, e.Y))
                {
                    _dropDownSelected = true;
                }
                else
                {
                    _dropDownSelected = false;
                }

                if (lastState != _dropDownSelected) 
                    RedrawItem();

                lastState = _dropDownSelected;
            }

            base.OnMouseMove(e);
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _dropDownSelected = false;
            
        }

        #endregion

        #region IContainsRibbonItems Members

        public IEnumerable<RibbonItem> GetItems()
        {
            return DropDownItems;
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<Component> GetAllChildComponents()
        {
            return DropDownItems.ToArray();
        }

        #endregion
    }
}
