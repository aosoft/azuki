// file: DummyHighlighter.cs
// brief: Dummy highlighter which executes nothing.
// author: YAMAMOTO Suguru
// update: 2008-10-13
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
		public override void Highlight( Document doc, ref int dirtyBegin, ref int dirtyEnd )
		{}
	}
}
