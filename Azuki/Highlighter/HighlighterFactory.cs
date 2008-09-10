// file: HighlighterFactory.cs
// brief: Class factory of highlighters.
// author: YAMAMOTO Suguru
// update: 2008-09-10
//=========================================================
using System;

namespace Sgry.Azuki
{
	/// <summary>
	/// Class factory to create build-in highlighter by name.
	/// </summary>
	public static class HighlighterFactory
	{
		/// <summary>
		/// Creates a highlighter by name.
		/// </summary>
		public static IHighlighter Create( string typeName )
		{
			if( String.Compare(typeName, "C/C++", true) == 0 )
			{
				return new CppHighlighter();
			}
			else if( String.Compare(typeName, "C#", true) == 0 )
			{
				return new CppHighlighter();
			}

			return new DummyHighlighter();
		}
	}
}
