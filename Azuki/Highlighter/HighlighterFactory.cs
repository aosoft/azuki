// file: HighlighterFactory.cs
// brief: Class factory of highlighters.
// author: YAMAMOTO Suguru
// update: 2008-10-21
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
		static JavaHighlighter _JavaHighlighter = null;
		static RubyHighlighter _RubyHighlighter = null;

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
		/// Gets a highlighter for Java.
		/// </summary>
		public static JavaHighlighter JavaHighlighter
		{
			get
			{
				if( _JavaHighlighter == null )
				{
					_JavaHighlighter = new JavaHighlighter();
				}
				return _JavaHighlighter;
			}
		}

		/// <summary>
		/// Gets a highlighter for Ruby.
		/// </summary>
		public static RubyHighlighter RubyHighlighter
		{
			get
			{
				if( _RubyHighlighter == null )
				{
					_RubyHighlighter = new RubyHighlighter();
				}
				return _RubyHighlighter;
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
				case "c":		return CppHighlighter;
				case "c++":		return CppHighlighter;
				case "c/c++":	return CppHighlighter;
				case "c#":		return CppHighlighter;
				case "java":	return JavaHighlighter;
				case "ruby":	return RubyHighlighter;
				case "xml":		return XmlHighlighter;
				default:		return new DummyHighlighter();
			}
		}
	}
}
