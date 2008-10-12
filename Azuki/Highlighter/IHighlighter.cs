// file: IHighlighter.cs
// brief: Interface of highlighter object for Azuki.
// author: YAMAMOTO Suguru
// update: 2008-10-12
//=========================================================
using System;
using Debug = System.Diagnostics.Debug;

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

		/// <summary>
		/// Highlight a token consisted with only digits.
		/// </summary>
		/// <returns>Index of next parse point if a pair was highlighted or 'begin' index</returns>
		protected static int TryHighlightNumberToken( Document doc, int startIndex, int endIndex )
		{
			Debug.Assert( endIndex <= doc.Length );
			int begin = startIndex;
			int end = begin;
int ＨＥＸリテラルに対応;
			if( doc.Length <= end || doc[end] < '0' || '9' < doc[end] )
				return begin;

			// seek end of this number token
			while( end < endIndex && '0' <= doc[end] && doc[end] <= '9' )
			{
				end++;
			}

			// ensure this token ends with NOT an alphabet
			if( end < endIndex && Char.IsLetter(doc[end]) )
			{
				return begin; // not a number token
			}

			// highlight this token
			for( int i=begin; i<end; i++ )
			{
				doc.SetCharClass( i, CharClass.Number );
			}

			return end;
		}
	}
}
