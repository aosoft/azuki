// file: IHighlighter.cs
// brief: Interface of highlighter object for Azuki.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-05
//=========================================================
using System;

namespace Sgry.Azuki
{
	/// <summary>
	/// Interface of highlighter object for Azuki.
	/// </summary>
	public interface IHighlighter
	{
		/// <summary>
		/// Highlight whole document.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		void Highlight( Document doc );

		/// <summary>
		/// Highlight document part.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		/// <param name="begin">Index to start highlighting.</param>
		/// <param name="end">Index to end highlighting.</param>
		/// <param name="invalidBegin">Begin index of the range to be invalidated.</param>
		/// <param name="invalidEnd">End index of the range to be invalidated.</param>
		void Highlight( Document doc, int begin, int end, out int invalidBegin, out int invalidEnd );
	}

	/// <summary>
	/// Adopter class for highlighter classes.
	/// </summary>
	public abstract class HighlighterBase : IHighlighter
	{
		/// <summary>
		/// Highlight whole document.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		public void Highlight( Document doc )
		{
			int b, e;
			Highlight( doc, 0, doc.Length, out b, out e );
		}

		/// <summary>
		/// Highlight document part.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		/// <param name="begin">Index to start highlighting.</param>
		/// <param name="end">Index to end highlighting.</param>
		/// <param name="invalidBegin">Begin index of the range to be invalidated.</param>
		/// <param name="invalidEnd">End index of the range to be invalidated.</param>
		public abstract void Highlight( Document doc, int begin, int end, out int invalidBegin, out int invalidEnd );
	}
}
