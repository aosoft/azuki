// file: IHighlighter.cs
// brief: Interface of highlighter object for Azuki.
// author: YAMAMOTO Suguru
// update: 2008-10-13
//=========================================================
using System;
using System.Collections.Generic;
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
		/// <param name="dirtyBegin">Index to start highlighting. On return, start index of the range to be invalidated.</param>
		/// <param name="dirtyEnd">Index to end highlighting. On return, end index of the range to be invalidated.</param>
		void Highlight( Document doc, ref int dirtyBegin, ref int dirtyEnd );
	}

	/// <summary>
	/// Adopter class for highlighter classes.
	/// </summary>
	public abstract class HighlighterBase : IHighlighter
	{
		#region Inner types
		/// <summary>
		/// Class which expresses an enclosing pair like '[' and ']'.
		/// </summary>
		protected class Enclosure
		{
			/// <summary>Token to open the enclosing pair.</summary>
			public string opener;

			/// <summary>Token to close the enclosing pair.</summary>
			public string closer;
			
			/// <summary>Char-class to be set for chars in the range of enclosing pair.</summary>
			public CharClass klass;
			
			/// <summary>Escape char used in the enclosing pair.</summary>
			public char escape;
		}
		#endregion

		/// <summary>
		/// Highlight whole document.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		public void Highlight( Document doc )
		{
			int begin = 0;
			int end = doc.Length;
			Highlight( doc, ref begin, ref end );
		}

		/// <summary>
		/// Highlight document part.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		/// <param name="dirtyBegin">Index to start highlighting. On return, start index of the range to be invalidated.</param>
		/// <param name="dirtyEnd">Index to end highlighting. On return, end index of the range to be invalidated.</param>
		public abstract void Highlight( Document doc, ref int dirtyBegin, ref int dirtyEnd );

		#region Utilities
		/// <summary>
		/// Highlight a token consisted with only digits.
		/// </summary>
		/// <returns>Index of next parse point if a pair was highlighted or 'begin' index</returns>
		protected static int TryHighlightNumberToken( Document doc, int startIndex, int endIndex )
		{
			Debug.Assert( endIndex <= doc.Length );
			int begin = startIndex;
			int end = begin;
			char postfixCh;

			if( doc.Length <= end || doc[end] < '0' || '9' < doc[end] )
				return begin;

			// check whether this token is a hex-number literal or not
			if( begin+2 < doc.Length && doc[begin] == '0' && doc[begin+1] == 'x' ) // check begin"+2" to avoid highlight token "0x" (nothing trails)
			{
				end = begin + 2;

				// seek end of this hex-number token
				while( end < endIndex && Utl.IsHexDigitChar(doc[end]) )
				{
					end++;
				}
			}
			else
			{
				// seek end of this number token
				while( end < endIndex && Utl.IsDigitOrDot(doc[end]) )
				{
					end++;
				}

				// if next char is one of the alphabets in 'f', 'i', 'j', 'l',
				// treat it as a post-fix.
				if( end < endIndex )
				{
					postfixCh = Char.ToLower( doc[end] );
					if( postfixCh == 'f' || postfixCh == 'i' || postfixCh == 'j' || postfixCh == 'l' )
					{
						end++;
					}
				}
			}
			
			// ensure this token ends with NOT an alphabet
			if( end < endIndex && Utl.IsAlphabet(doc[end]) )
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

		/// <summary>
		/// Find next token beginning position and return it's index.
		/// </summary>
		protected static int FindNextToken( Document doc, int index )
		{
			return WordLogic.NextWordStartForMove( doc, index );
		}

		/// <summary>
		/// Find previous token beginning position and return it's index.
		/// </summary>
		protected static int FindPrevToken( Document doc, int index )
		{
			return WordLogic.PrevWordStartForMove( doc, index );
		}

		/// <summary>
		/// Find token.
		/// </summary>
		protected static int Find( Document doc, string token, int startIndex, int endIndex )
		{
			Debug.Assert( doc != null && token != null );
			Debug.Assert( 0 <= startIndex && startIndex <= doc.Length );
			Debug.Assert( 0 <= endIndex && startIndex <= endIndex );

			for( int i=startIndex; i<endIndex; i++ )
			{
				int j = 0;
				for( ; j<token.Length && i+j<doc.Length; j++ )
				{
					if( doc[i+j] != token[j] )
					{
						break; // go to next position
					}
				}
				if( j == token.Length )
				{
					// found.
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Finds token backward.
		/// </summary>
		protected static int FindLast( Document doc, string token, int startIndex )
		{
			Debug.Assert( doc != null && token != null );
			Debug.Assert( 0 <= startIndex && startIndex < doc.Length );

			for( int i=startIndex; 0<=i; i-- )
			{
				int j = 0;
				for( ; j<token.Length && i+j < doc.Length; j++ )
				{
					if( doc[i+j] != token[j] )
					{
						break; // go to next position
					}
				}
				if( j == token.Length )
				{
					// found.
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// return closer pos or line-end if closer is null.
		/// </summary>
		protected static int FindCloser( Document doc, Enclosure pair, int startIndex, int endIndex )
		{
			int index;

			if( pair.closer == null )
			{
				// return line-end
				return GetLineEndIndexFromCharIndex( doc, startIndex );
			}
			else
			{
				// treat escape
				index = Find( doc, pair.closer, startIndex, endIndex );
				while( 1 <= index && doc[index-1] == pair.escape )
				{
					index++;
					index = Find( doc, pair.closer, index, endIndex );
				}
				return index;
			}
		}

		/// <summary>
		/// Gets index of the end position of the line containing given index.
		/// </summary>
		protected static int GetLineEndIndexFromCharIndex( Document doc, int index )
		{
			int lineIndex = doc.GetLineIndexFromCharIndex( index );
			if( lineIndex+1 < doc.LineCount )
				return doc.GetLineHeadIndex( lineIndex+1 );
			else
				return doc.Length;
		}

		/// <summary>
		/// Determine whether the token starts with given index in the document.
		/// </summary>
		protected static bool StartsWith( Document doc, string token, int index )
		{
			int i = 0;

			for( ; i<token.Length && index+i<doc.Length; i++ )
			{
				if( token[i] != doc[index+i] )
					return false;
			}

			if( i == token.Length )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Determine whether the enclosing pair starts with given index in the document.
		/// </summary>
		protected static Enclosure StartsWith( Document doc, List<Enclosure> pairs, int index )
		{
			foreach( Enclosure pair in pairs )
			{
				if( StartsWith(doc, pair.opener, index) )
					return pair;
			}
			return null;
		}

		static class Utl
		{
			public static bool IsAlphabet( char ch )
			{
				if( 'a' <= ch && ch <= 'z' )
					return true;
				if( 'A' <= ch && ch <= 'Z' )
					return true;

				return false;
			}

			public static bool IsDigitOrDot( char ch )
			{
				if( '0' <= ch && ch <= '9' )
					return true;
				if( ch == '.' )
					return true;

				return false;
			}

			public static bool IsHexDigitChar( char ch )
			{
				if( '0' <= ch && ch <= '9' )
					return true;
				if( 'A' <= ch && ch <= 'F' )
					return true;
				if( 'a' <= ch && ch <= 'f' )
					return true;

				return false;
			}
		}
		#endregion
	}
}
