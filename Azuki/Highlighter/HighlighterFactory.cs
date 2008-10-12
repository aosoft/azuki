// file: HighlighterFactory.cs
// brief: Class factory of highlighters.
// author: YAMAMOTO Suguru
// update: 2008-10-12
//=========================================================
using System;

namespace Sgry.Azuki
{
	/// <summary>
	/// Class factory to create build-in highlighter by name.
	/// </summary>
	public static class HighlighterFactory
	{
		static CppHighlighter _CppHighlighter = null;
		static XmlHighlighter _XmlHighlighter = null;
		static BasicHighlighter _BasicHighlighter = null;

		/// <summary>
		/// Gets a highlighter for C/C++/C#.
		/// </summary>
		public static CppHighlighter CppHighlighter
		{
			get
			{
				if( _CppHighlighter == null )
				{
					_CppHighlighter = new CppHighlighter();
				}
				return _CppHighlighter;
			}
		}

		/// <summary>
		/// Gets a highlighter for XML.
		/// </summary>
		public static XmlHighlighter XmlHighlighter
		{
			get
			{
				if( _XmlHighlighter == null )
				{
					_XmlHighlighter = new XmlHighlighter();
				}
				return _XmlHighlighter;
			}
		}

		/// <summary>
		/// Gets a generic keyword based highlighter.
		/// </summary>
		public static BasicHighlighter BasicHighlighter
		{
			get
			{
				if( _BasicHighlighter == null )
				{
					_BasicHighlighter = new BasicHighlighter();
				}
				return _BasicHighlighter;
			}
		}

		/// <summary>
		/// Creates a highlighter by name.
		/// </summary>
		public static IHighlighter FindByName( string typeName )
		{
			switch( typeName.ToLower() )
			{
				case "c/c++":	return CppHighlighter;
				case "c#":		return CppHighlighter;
				case "xml":		return XmlHighlighter;
				default:		return new DummyHighlighter();
			}
		}
	}
}
