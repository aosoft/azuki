// file: ColorScheme.cs
// brief: color set record
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-20
//=========================================================
using System.Collections.Generic;
using System.Drawing;

namespace Sgry.Azuki
{
	/// <summary>
	/// Pair of foreground/background colors.
	/// </summary>
	public class ColorPair
	{
		/// <summary>
		/// Foreground color.
		/// </summary>
		public Color Fore;

		/// <summary>
		/// Background color.
		/// </summary>
		public Color Back;
		
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ColorPair()
			: this( Color.Black, Color.White )
		{}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ColorPair( Color fore, Color back )
		{
			Fore = fore;
			Back = back;
		}
	}

	/// <summary>
	/// Color set used for drawing.
	/// </summary>
	public class ColorScheme
	{
		Dictionary< CharClass, ColorPair > _Colors = new Dictionary< CharClass, ColorPair >();

		/// <summary>
		/// Gets or sets color pair associated with given char-class.
		/// </summary>
		public ColorPair this[ CharClass klass ]
		{
			get{ return _Colors[klass]; }
			set
			{
				_Colors[klass] = value;
				if( klass == CharClass.Normal )
				{
					ForeColor = value.Fore;
					BackColor = value.Back;
				}
			}
		}

		/// <summary>
		/// Foreground color of normal text.
		/// </summary>
		public Color ForeColor;

		/// <summary>
		/// Background color of normal text.
		/// </summary>
		public Color BackColor;

		/// <summary>
		/// Color of selected text.
		/// </summary>
		public Color SelectionFore;

		/// <summary>
		/// Background color of selected text.
		/// </summary>
		public Color SelectionBack;

		/// <summary>
		/// Color of white-space chars.
		/// </summary>
		public Color WhiteSpaceColor;

		/// <summary>
		/// Color of EOL chars.
		/// </summary>
		public Color EolColor;

		/// <summary>
		/// Underline color of the line which the caret is on.
		/// </summary>
		public Color HighlightColor;

		/// <summary>
		/// Color of the line number text.
		/// </summary>
		public Color LineNumberFore;

		/// <summary>
		/// Background color of the line number text.
		/// </summary>
		public Color LineNumberBack;

		/// <summary>
		/// Gets default color scheme.
		/// </summary>
		public static ColorScheme Default
		{
			get
			{
				ColorScheme scheme = new ColorScheme();
				Color bgcolor = Color.FromArgb( 0xff, 0xfa, 0xf0 );
				Color azuki = Color.FromArgb( 0x92, 0x62, 0x57 ); // azuki iro
				Color shin_bashi = Color.FromArgb( 0x74, 0xa9, 0xd6 ); // shin-bashi iro (japanese)
				Color hana_asagi = Color.FromArgb( 0x1b, 0x77, 0x92 ); // hana-asagi iro (japanese)

				scheme[ CharClass.Normal ] = new ColorPair( Color.Black, bgcolor );
				scheme[ CharClass.Number ] = new ColorPair( Color.Black, bgcolor );
				scheme[ CharClass.String ] = new ColorPair( Color.Teal, bgcolor );
				scheme[ CharClass.Keyword ] = new ColorPair( Color.Blue, bgcolor );
				scheme[ CharClass.Comment ] = new ColorPair( Color.Green, bgcolor );

				scheme.SelectionFore = Color.White;
				scheme.SelectionBack = azuki;
				scheme.WhiteSpaceColor = Color.Silver;
				scheme.EolColor = shin_bashi;
				scheme.HighlightColor = azuki;
				scheme.LineNumberFore = hana_asagi;
				scheme.LineNumberBack = Color.FromArgb( 0xef, 0xef, 0xff );

				return scheme;
			}
		}

		/*/// <summary>
		/// Gets high contrast color scheme.
		/// </summary>
		public static ColorScheme HighContrast
		{
			get
			{
			}
		}*/
	}
}
