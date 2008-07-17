// file: DummyHighlighter.cs
// brief: Dummy highlighter which executes nothing.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-12
//=========================================================
using System;

namespace Sgry.Azuki
{
	/// <summary>
	/// Dummy highlighter which does nothing.
	/// </summary>
	public class DummyHighlighter : HighlighterBase
	{
		/// <summary>
		/// Does nothing.
		/// </summary>
		public override void Highlight( Document doc, int begin, int end, out int invalidBegin, out int invalidEnd )
		{
			invalidBegin = invalidEnd = 0;
		}
	}
}
