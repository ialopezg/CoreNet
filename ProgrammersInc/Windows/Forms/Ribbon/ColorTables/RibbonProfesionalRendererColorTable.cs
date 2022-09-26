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

namespace System.Windows.Forms
{
    public class RibbonProfesionalRendererColorTable
    {
        #region Fields

        public Color Arrow = FromHex("#678CBD");
        public Color ArrowLight = Color.FromArgb(130, Color.White);
        public Color ArrowDisabled = FromHex("#B7B7B7");
        public Color Text = FromHex("#15428B");

        public Color RibbonBackground = FromHex("#BFDBFF");
        public Color TabBorder = FromHex("#8DB2E3");
        public Color TabNorth = FromHex("#EBF3FE");
        public Color TabSouth = FromHex("#E1EAF6");
        public Color TabGlow = FromHex("#D1FBFF");
        public Color TabText = FromHex("#15428B");
        public Color TabActiveText = FromHex("#15428B");
        public Color TabContentNorth = FromHex("#C8D9ED");
        public Color TabContentSouth = FromHex("#E7F2FF");
        public Color TabSelectedGlow = FromHex("#E1D2A5");
        public Color PanelDarkBorder = Color.FromArgb(51, FromHex("#15428B"));
        public Color PanelLightBorder = Color.FromArgb(102, Color.White);
        public Color PanelTextBackground = FromHex("#C2D9F0");
        public Color PanelTextBackgroundSelected = FromHex("#C2D9F0");
        public Color PanelText = FromHex("#15428B");
        public Color PanelBackgroundSelected = Color.FromArgb(102, FromHex("#E8FFFD"));
        public Color PanelOverflowBackground = FromHex("#B9D1F0");
        public Color PanelOverflowBackgroundPressed = FromHex("#7699C8");
        public Color PanelOverflowBackgroundSelectedNorth = Color.FromArgb(100, Color.White);
        public Color PanelOverflowBackgroundSelectedSouth = Color.FromArgb(102, FromHex("#B8D7FD"));

        public Color ButtonBgOut = FromHex("#C1D5F1");
        public Color ButtonBgCenter = FromHex("#CFE0F7");
        public Color ButtonBorderOut = FromHex("#B9D0ED");
        public Color ButtonBorderIn = FromHex("#E3EDFB");
        public Color ButtonGlossyNorth = FromHex("#DEEBFE");
        public Color ButtonGlossySouth = FromHex("#CBDEF6");

        public Color ButtonDisabledBgOut = FromHex("#E0E4E8");
        public Color ButtonDisabledBgCenter = FromHex("#E8EBEF");
        public Color ButtonDisabledBorderOut = FromHex("#C5D1DE");
        public Color ButtonDisabledBorderIn = FromHex("#F1F3F5");
        public Color ButtonDisabledGlossyNorth = FromHex("#F0F3F6");
        public Color ButtonDisabledGlossySouth = FromHex("#EAEDF1");

        public Color ButtonSelectedBgOut = FromHex("#FFD646");
        public Color ButtonSelectedBgCenter = FromHex("#FFEAAC");
        public Color ButtonSelectedBorderOut = FromHex("#C2A978");
        public Color ButtonSelectedBorderIn = FromHex("#FFF2C7");
        public Color ButtonSelectedGlossyNorth = FromHex("#FFFDDB");
        public Color ButtonSelectedGlossySouth = FromHex("#FFE793");

        public Color ButtonPressedBgOut = FromHex("#F88F2C");
        public Color ButtonPressedBgCenter = FromHex("#FDF1B0");
        public Color ButtonPressedBorderOut = FromHex("#8E8165");
        public Color ButtonPressedBorderIn = FromHex("#F9C65A");
        public Color ButtonPressedGlossyNorth = FromHex("#FDD5A8");
        public Color ButtonPressedGlossySouth = FromHex("#FBB062");

        public Color ButtonCheckedBgOut = FromHex("#F9AA45");
        public Color ButtonCheckedBgCenter = FromHex("#FDEA9D");
        public Color ButtonCheckedBorderOut = FromHex("#8E8165");
        public Color ButtonCheckedBorderIn = FromHex("#F9C65A");
        public Color ButtonCheckedGlossyNorth = FromHex("#F8DBB7");
        public Color ButtonCheckedGlossySouth = FromHex("#FED18E");

        public Color ItemGroupOuterBorder = FromHex("#9EBAE1");
        public Color ItemGroupInnerBorder = Color.FromArgb(51, Color.White);
        public Color ItemGroupSeparatorLight = Color.FromArgb(64, Color.White);
        public Color ItemGroupSeparatorDark = Color.FromArgb(38, FromHex("#9EBAE1"));
        public Color ItemGroupBgNorth = FromHex("#CADCF0");
        public Color ItemGroupBgSouth = FromHex("#D0E1F7");
        public Color ItemGroupBgGlossy = FromHex("#BCD0E9");

        public Color ButtonListBorder = FromHex("#B9D0ED");
        public Color ButtonListBg = FromHex("#D4E6F8");
        public Color ButtonListBgSelected = FromHex("#ECF3FB");

        public Color DropDownBg = FromHex("#FAFAFA");
        public Color DropDownImageBg = FromHex("#E9EEEE");
        public Color DropDownImageSeparator = FromHex("#C5C5C5");
        public Color DropDownBorder = FromHex("#868686");
        public Color DropDownGripNorth = FromHex("#FFFFFF");
        public Color DropDownGripSouth = FromHex("#DFE9EF");
        public Color DropDownGripBorder = FromHex("#DDE7EE");
        public Color DropDownGripDark = FromHex("#5574A7");
        public Color DropDownGripLight = FromHex("#FFFFFF");

        public Color SeparatorLight = FromHex("#FAFBFD");
        public Color SeparatorDark = FromHex("#96B4DA");
        public Color SeparatorBg = FromHex("#DAE6EE");
        public Color SeparatorLine = FromHex("#C5C5C5");

        public Color TextBoxUnselectedBg = FromHex("#EAF2FB");
        public Color TextBoxBorder = FromHex("#ABC1DE");

        #endregion

        #region Methods

        protected static Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return Color.FromArgb(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }

        #endregion
    }
}
