// file: CppHighlighter.cs
// brief: C/C++/C# highlighter.
// author: YAMAMOTO Suguru
// update: 2008-09-10
//=========================================================
using System;
using Color = System.Drawing.Color;

namespace Sgry.Azuki
{
	/// <summary>
	/// Highlighter for C/C++/C# language based on keyword matching.
	/// </summary>
	public class CppHighlighter : BasicHighlighter
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public CppHighlighter()
		{
			SetKeywords( new string[] {
				"__FILE__", "__LINE__",
				"as", "asm", "auto", "base",
				"bool", "break", "byte", "case", "catch", "char", "checked",
				"class", "const", "const_cast", "continue", "decimal", "default",
				"delegate", "delete", "do", "double", "dynamic_cast", "else",
				"enum", "event", "explicit", "export", "extern", "false", "finally", "fixed",
				"float", "for", "foreach", "friend", "goto", "if", "in", "inline",
				"int", "interface", "internal", "is", "lock", "long", "mutable",
				"namespace", "null", "new", "object",
				"operator", "out", "override", "params", "private", "protected",
				"public", "readonly", "ref", "register",
				"reinterpret_cast", "return", "sbyte", "sealed", "short",
				"signed", "sizeof", "stackalloc", "static", "static_cast", "string",
				"struct", "switch", "template", "this", "throw", "true", "try",
				"typedef", "typeid", "typename", "typeof", "uint", "ulong",
				"unchecked", "union", "unsafe", "unsigned", "using", "ushort",
				"virtual", "void", "volatile", "while"
			}, CharClass.Keyword );

			SetKeywords( new string[] {
				"size_t", "wchar_t"
			}, CharClass.Keyword2 );

			SetKeywords( new string[] {
				"BOOL", "BYTE", "DWORD", "DWORD_PTR", "FALSE", "HANDLE", "HRESULT", "HWND",
				"INT_PTR", "LONG_PTR", "LPARAM", "LRESULT", "NULL", "TCHAR", "TRUE",
				"WORD", "WPARAM"
			}, CharClass.Keyword3 );

			SetKeywords( new string[] {
				"#define", "#elif", "#else", "#endif", "#endregion", "#error",
				"#if", "#ifdef", "#ifndef", "#include", "#import",
				"#line", "#pragma", "#region", "#undef"
			}, CharClass.PreProcessor );

			AddEnclosure( "'", "'", CharClass.String, '\\' );
			AddEnclosure( "@\"", "\"", CharClass.String, '\"' );
			AddEnclosure( "\"", "\"", CharClass.String, '\\' );
			AddEnclosure( "/**", "*/", CharClass.DocComment );
			AddEnclosure( "/*", "*/", CharClass.Comment );
			AddLineHighlight( "//", CharClass.Comment );
		}
	}
}
