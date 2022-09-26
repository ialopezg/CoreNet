/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using System.Xml;

namespace ProgrammersInc.WinFormsGloss.Drawing
{
	#region ColorTable

	public abstract class ColorTable
	{
		protected ColorTable()
		{
		}

		public Color[] Colors
		{
			get
			{
				return new Color[]
				{
					PrimaryColor,
					PrimaryBackgroundColor,
					PrimaryBrandingColor,
					GlowColor,
					GlowDeepColor,
					GrayForegroundColor,
					GrayBackgroundColor,
					TextColor,
					GrayTextColor,
					GlowTextColor,
					GrayPrimaryBackgroundColor,
					GlossyLightenerColor,
					GlossyGlowLightenerColor,
					PrimaryBorderColor
				};
			}
		}

		public abstract string Name
		{
			get;
		}

		public abstract string Description
		{
			get;
		}

		/// <summary>
		/// The primary foreground color.
		/// </summary>
		public abstract Color PrimaryColor
		{
			get;
		}

		/// <summary>
		/// The primary background color.
		/// </summary>
		public virtual Color PrimaryBackgroundColor
		{
			get
			{
				return WinFormsUtility.Drawing.ColorUtil.Combine( PrimaryColor, Color.White, 0.3 );
			}
		}

		/// <summary>
		/// The color used for glowing items.
		/// </summary>
		public abstract Color GlowColor
		{
			get;
		}

		/// <summary>
		/// The color of the highlight around a glowing item.
		/// </summary>
		public abstract Color GlowHighlightColor
		{
			get;
		}

		/// <summary>
		/// The color of a pressed item.
		/// </summary>
		public abstract Color GlowDeepColor
		{
			get;
		}

		/// <summary>
		/// The inactive foreground color.
		/// </summary>
		public abstract Color GrayForegroundColor
		{
			get;
		}

		/// <summary>
		/// The inactive background color.
		/// </summary>
		public abstract Color GrayBackgroundColor
		{
			get;
		}

		/// <summary>
		/// Normal text color.
		/// </summary>
		public abstract Color TextColor
		{
			get;
		}

		/// <summary>
		/// Inactive text color.
		/// </summary>
		public abstract Color GrayTextColor
		{
			get;
		}

		/// <summary>
		/// A color used for main branding.
		/// </summary>
		public abstract Color PrimaryBrandingColor
		{
			get;
		}

		/// <summary>
		/// Glowing text color.
		/// </summary>
		public virtual Color GlowTextColor
		{
			get
			{
				return TextColor;
			}
		}

		/// <summary>
		/// An inactive background color which has been merged with the primary color slightly.
		/// </summary>
		public virtual Color GrayPrimaryBackgroundColor
		{
			get
			{
				return WinFormsUtility.Drawing.ColorUtil.Combine( PrimaryColor, GrayBackgroundColor, 0.2 );
			}
		}

		/// <summary>
		/// The color used to make glossy brushes.
		/// </summary>
		public virtual Color GlossyLightenerColor
		{
			get
			{
				return Color.White;
			}
		}

		/// <summary>
		/// The color used to make glossy brushes, when glowing.
		/// </summary>
		public virtual Color GlossyGlowLightenerColor
		{
			get
			{
				return Color.White;
			}
		}

		/// <summary>
		/// The color used to make borders.
		/// </summary>
		public virtual Color PrimaryBorderColor
		{
			get
			{
				return GlossyLightenerColor;
			}
		}
	}

	#endregion

	#region WindowsThemeColorTable

	public class WindowsThemeColorTable : ColorTable
	{
		public override string Name
		{
			get
			{
				return "Windows Theme";
			}
		}

		public override string Description
		{
			get
			{
				return "Matching your Windows color scheme: blue, silver or olive.";
			}
		}

		public override Color PrimaryColor
		{
			get
			{
				if( _primaryColor == null )
				{
					Color color;

					if( VisualStyleRenderer.IsSupported )
					{
						VisualStyleRenderer vsr = new VisualStyleRenderer( VisualStyleElement.Window.Caption.Active );

						color = vsr.GetColor( ColorProperty.FillColorHint );
					}
					else
					{
						color = SystemColors.Control;
					}

					double h = color.GetHue() / 360, s = color.GetSaturation();

					_primaryColor = WinFormsUtility.Drawing.ColorUtil.FromHSL( h, s, 0.5 );
				}
				return _primaryColor.Value;
			}
		}

		public override Color GlowColor
		{
			get
			{
				if( _glowColor == null )
				{
					Color color;

					if( VisualStyleRenderer.IsSupported )
					{
						color = VisualStyleInformation.ControlHighlightHot;
					}
					else
					{
						color = SystemColors.Highlight;
					}

					double h = color.GetHue() / 360, s = color.GetSaturation();

					_glowColor = WinFormsUtility.Drawing.ColorUtil.FromHSL( h, s, 0.66 );
				}
				return _glowColor.Value;
			}
		}

		public override Color GlowHighlightColor
		{
			get
			{
				if( _glowHighlightColor == null )
				{
					Color color;

					if( VisualStyleRenderer.IsSupported )
					{
						color = VisualStyleInformation.ControlHighlightHot;
						color = WinFormsUtility.Drawing.ColorUtil.ModifyHue( color, 0.02 );

						double h = color.GetHue() / 360, s = color.GetSaturation();

						_glowHighlightColor = WinFormsUtility.Drawing.ColorUtil.FromHSL( h, s, 0.6 );
					}
					else
					{
						color = SystemColors.Window;

						double h = color.GetHue() / 360, s = color.GetSaturation();

						_glowHighlightColor = WinFormsUtility.Drawing.ColorUtil.FromHSL( h, s, 0.85 );
					}
				}

				return _glowHighlightColor.Value;
			}
		}

		public override Color GlowDeepColor
		{
			get
			{
				return WinFormsUtility.Drawing.ColorUtil.ModifyHue( VisualStyleInformation.ControlHighlightHot, -0.02 );
			}
		}

		public override Color GrayForegroundColor
		{
			get
			{
				return SystemColors.GrayText;
			}
		}

		public override Color GrayBackgroundColor
		{
			get
			{
				return SystemColors.ControlLight;
			}
		}

		public override Color TextColor
		{
			get
			{
				return SystemColors.ControlText;
			}
		}

		public override Color GrayTextColor
		{
			get
			{
				return SystemColors.GrayText;
			}
		}

		public override Color PrimaryBrandingColor
		{
			get
			{
				if( _primaryBrandingColor == null )
				{
					Color color;

					if( VisualStyleRenderer.IsSupported )
					{
						VisualStyleRenderer vsr = new VisualStyleRenderer( VisualStyleElement.Window.Caption.Active );

						color = vsr.GetColor( ColorProperty.FillColorHint );
					}
					else
					{
						color = SystemColors.Highlight;
					}

					double h = color.GetHue() / 360, s = color.GetSaturation();

					_primaryBrandingColor = WinFormsUtility.Drawing.ColorUtil.FromHSL( h, s, 0.6 );
				}
				return _primaryBrandingColor.Value;
			}
		}

		private Color? _primaryColor;
		private Color? _glowColor;
		private Color? _glowHighlightColor;
		private Color? _primaryBrandingColor;
	}

	#endregion

	#region WindowBackgroundColorTable

	public class WindowBackgroundColorTable : ColorTable
	{
		public WindowBackgroundColorTable( ColorTable basedOn )
		{
			_basedOn = basedOn;
		}

		public override string Name
		{
			get
			{
				return string.Format( "Windows Background (based on {0})", _basedOn.Name );
			}
		}

		public override string Description
		{
			get
			{
				return string.Format( "Windows Background (based on {0})", _basedOn.Description );
			}
		}

		public override Color PrimaryColor
		{
			get
			{
				return SystemColors.ControlDark;
			}
		}

		public override Color PrimaryBackgroundColor
		{
			get
			{
				return SystemColors.Control;
			}
		}

		public override Color TextColor
		{
			get
			{
				return SystemColors.WindowText;
			}
		}

		public override Color GrayTextColor
		{
			get
			{
				return SystemColors.GrayText;
			}
		}

		public override Color GlowTextColor
		{
			get
			{
				return _basedOn.GlowTextColor;
			}
		}

		public override Color GrayBackgroundColor
		{
			get
			{
				return SystemColors.Control;
			}
		}

		public override Color GlowHighlightColor
		{
			get
			{
				return _basedOn.GlowHighlightColor;
			}
		}

		public override Color GlowColor
		{
			get
			{
				return _basedOn.GlowColor;
			}
		}

		public override Color GlowDeepColor
		{
			get
			{
				return _basedOn.GlowDeepColor;
			}
		}

		public override Color PrimaryBrandingColor
		{
			get
			{
				return _basedOn.PrimaryBrandingColor;
			}
		}

		public override Color GrayForegroundColor
		{
			get
			{
				return SystemColors.Control;
			}
		}

		private ColorTable _basedOn;
	}

	#endregion

	#region XmlColorTable

	public class XmlColorTable : ColorTable
	{
		public XmlColorTable( Utility.Assemblies.ManifestResources res, string path )
		{
			XmlDocument xmlDoc = res.GetXmlDocument( path );

			XmlNode ctNode = xmlDoc.SelectSingleNode( "ColorTable" );

			_name = ctNode.Attributes["Name"].Value;
			_description = ctNode.Attributes["Description"].Value;

			foreach( XmlNode cNode in ctNode.ChildNodes )
			{
				string name = cNode.Name;
				string value = cNode.InnerText;

				if( value.Length != 7 || value[0] != '#' )
				{
					throw new InvalidOperationException( string.Format( "Invalid color '{0}' in '{1}'.", value, name ) );
				}

				int v = int.Parse( value.Substring( 1 ), System.Globalization.NumberStyles.AllowHexSpecifier );

				unchecked
				{
					Color color = Color.FromArgb( (int) 0xff000000 | v );

					_colors.Add( name, color );
				}
			}
		}

		public override string Name
		{
			get
			{
				return _name;
			}
		}

		public override string Description
		{
			get
			{
				return _description;
			}
		}

		public override Color PrimaryColor
		{
			get
			{
				return _colors["PrimaryColor"];
			}
		}

		public override Color GlowColor
		{
			get
			{
				return _colors["GlowColor"];
			}
		}

		public override Color GlowHighlightColor
		{
			get
			{
				return _colors["GlowHighlightColor"];
			}
		}

		public override Color GlowDeepColor
		{
			get
			{
				return _colors["GlowDeepColor"];
			}
		}

		public override Color GrayForegroundColor
		{
			get
			{
				return _colors["GrayForegroundColor"];
			}
		}

		public override Color GrayBackgroundColor
		{
			get
			{
				return _colors["GrayBackgroundColor"];
			}
		}

		public override Color TextColor
		{
			get
			{
				return _colors["TextColor"];
			}
		}

		public override Color GrayTextColor
		{
			get
			{
				return _colors["GrayTextColor"];
			}
		}

		public override Color PrimaryBrandingColor
		{
			get
			{
				return _colors["PrimaryBrandingColor"];
			}
		}

		public override Color GlowTextColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "GlowTextColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.GlowTextColor;
				}
			}
		}

		public override Color PrimaryBackgroundColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "PrimaryBackgroundColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.PrimaryBackgroundColor;
				}
			}
		}

		public override Color GlossyGlowLightenerColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "GlossyGlowLightenerColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.GlossyGlowLightenerColor;
				}
			}
		}

		public override Color GlossyLightenerColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "GlossyLightenerColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.GlossyLightenerColor;
				}
			}
		}

		public override Color GrayPrimaryBackgroundColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "GrayPrimaryBackgroundColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.GrayPrimaryBackgroundColor;
				}
			}
		}

		public override Color PrimaryBorderColor
		{
			get
			{
				Color c;

				if( _colors.TryGetValue( "PrimaryBorderColor", out c ) )
				{
					return c;
				}
				else
				{
					return base.PrimaryBorderColor;
				}
			}
		}

		private string _name, _description;
		private Dictionary<string, Color> _colors = new Dictionary<string, Color>();
	}

	#endregion
}
