// 2008-10-26
using System;
using System.Windows.Forms;

namespace Sgry.Ann
{
	static partial class Actions
	{
		/// <summary>
		/// Set editing mode to C/C++ mode.
		/// </summary>
		public static AnnAction SetToCppMode
			= delegate( AppLogic app )
		{
			app.MainForm.Azuki.Highlighter = Azuki.HighlighterFactory.CppHighlighter;
			app.MainForm.Azuki.AutoIndentHook = Azuki.AutoIndentLogic.CHook;
		};

		/// <summary>
		/// Set editing mode to C# mode.
		/// </summary>
		public static AnnAction SetToCSharpMode
			= delegate( AppLogic app )
		{
			app.MainForm.Azuki.Highlighter = Azuki.HighlighterFactory.CppHighlighter;
			app.MainForm.Azuki.AutoIndentHook = Azuki.AutoIndentLogic.CHook;
		};

		/// <summary>
		/// Set editing mode to Java mode.
		/// </summary>
		public static AnnAction SetToJavaMode
			= delegate( AppLogic app )
		{
			app.MainForm.Azuki.Highlighter = Azuki.HighlighterFactory.JavaHighlighter;
			app.MainForm.Azuki.AutoIndentHook = Azuki.AutoIndentLogic.CHook;
		};

		/// <summary>
		/// Set editing mode to Ruby mode.
		/// </summary>
		public static AnnAction SetToRubyMode
			= delegate( AppLogic app )
		{
			app.MainForm.Azuki.Highlighter = Azuki.HighlighterFactory.RubyHighlighter;
			app.MainForm.Azuki.AutoIndentHook = Azuki.AutoIndentLogic.GenericHook;
		};

		/// <summary>
		/// Set editing mode to XML mode.
		/// </summary>
		public static AnnAction SetToXmlMode
			= delegate( AppLogic app )
		{
			app.MainForm.Azuki.Highlighter = Azuki.HighlighterFactory.XmlHighlighter;
			app.MainForm.Azuki.AutoIndentHook = Azuki.AutoIndentLogic.GenericHook;
		};
	}
}
