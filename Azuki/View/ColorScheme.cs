// file: ColorScheme.cs
// brief: color set record
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-06-22
//=========================================================
using System.Drawing;

namespace Sgry.Azuki
{
	/// <summary>
	/// Color set used for drawing.
	/// </summary>
	public struct ColorScheme
	{
		/// <summary>Color of normal text.</summary>
		public Color ForeColor;

		/// <summary>Background color of normal text.</summary>
		public Color BackColor;

		/// <summary>Color of number token.</summary>
		public Color NumberForeColor;

		/// <summary>Background color of number token.</summary>
		public Color NumberBackColor;

		/// <summary>Color of keyword.</summary>
		public Color KeywordForeColor;

		/// <summary>Background color of keyword.</summary>
		public Color KeywordBackColor;

		/// <summary>Color of comment.</summary>
		public Color CommentForeColor;

		/// <summary>Background color of comment.</summary>
		public Color CommentBackColor;

		/// <summary>Color of string.</summary>
		public Color StringForeColor;

		/// <summary>Background color of string.</summary>
		public Color StringBackColor;

		/// <summary>Color of selected text.</summary>
		public Color SelectionForeColor;

		/// <summary>Background color of selected text.</summary>
		public Color SelectionBackColor;

		/// <summary>Color of white-space chars.</summary>
		public Color WhiteSpaceColor;

		/// <summary>Color of EOL chars.</summary>
		public Color EolColor;

		/// <summary>Underline color of the line which the caret is on.</summary>
		public Color HighlightColor;

		/// <summary>Color of the line number text.</summary>
		public Color LineNumberColor;

		/// <summary>Background color of the line number area.</summary>
		public Color LineNumberBackColor;

		/// <summary>
		/// Gets default color scheme.
		/// </summary>
		public static ColorScheme Default
		{
			get
			{
				ColorScheme scheme = new ColorScheme();
				
				scheme.ForeColor = Color.Black;
				scheme.BackColor = Color.FromArgb( 0xff, 0xfa, 0xf0 );
				scheme.NumberForeColor = scheme.ForeColor;
scheme.NumberForeColor = Color.Red;
				scheme.NumberBackColor = scheme.BackColor;
				scheme.KeywordForeColor = Color.Blue;
				scheme.KeywordBackColor = scheme.BackColor;
				scheme.CommentForeColor = Color.Green;
				scheme.CommentBackColor = scheme.BackColor;
				scheme.StringForeColor = Color.Teal;
				scheme.StringBackColor = scheme.BackColor;
				scheme.SelectionForeColor = Color.White;
				scheme.SelectionBackColor = Color.FromArgb( 0x92, 0x62, 0x57 ); // azuki iro (japanese)
				scheme.WhiteSpaceColor = Color.Silver;
				scheme.EolColor = Color.FromArgb( 0x74, 0xa9, 0xd6 ); // shin-bashi iro (japanese)
				scheme.HighlightColor = Color.FromArgb( 0x92, 0x62, 0x57 ); // azuki iro (japanese)
				scheme.LineNumberColor = Color.FromArgb( 0x1b, 0x77, 0x92 ); // hana-asagi iro (japanese)
				scheme.LineNumberBackColor = Color.FromArgb( 0xef, 0xef, 0xff );

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
				ColorScheme scheme = new ColorScheme();
				
				scheme.ForeColor = Color.White;
				scheme.BackColor = Color.Black;
				scheme.KeywordForeColor = Color.Blue;
				scheme.KeywordBackColor = Color.White;
				scheme.SelectionForeColor = Color.White;
				scheme.SelectionBackColor = Color.FromArgb( 0x92, 0x62, 0x57 ); // color or azuki
				scheme.WhiteSpaceColor = Color.Silver;
				scheme.EolColor = Color.FromArgb( 0x74, 0xa9, 0xd6 ); // shin-bashi iro (japanese)
				scheme.HighlightColor = Color.FromArgb( 0x92, 0x62, 0x57 );
				scheme.LineNumberColor = Color.FromArgb( 0x1b, 0x77, 0x92 ); // hana-asagi color
				scheme.LineNumberBackColor = Color.FromArgb( 0xef, 0xef, 0xff );

				return scheme;
			}
		}*/
	}
}
