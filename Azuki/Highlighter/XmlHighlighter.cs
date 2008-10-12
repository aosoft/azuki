// file: XmlHighlighter.cs
// brief: Highlighter for XML.
// author: YAMAMOTO Suguru
// update: 2008-10-12
//=========================================================
using System;
using System.Collections.Generic;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace Sgry.Azuki
{
	/// <summary>
	/// A highlighter to highlight XML.
	/// </summary>
	public class XmlHighlighter : HighlighterBase
	{
		#region Inner Types and Fields
		class Enclosure
		{
			public string opener;
			public string closer;
			public CharClass klass;
			public char escape;
		}

		List<Enclosure> _Enclosures = new List<Enclosure>();
		#endregion

		#region Init / Dispose
		public XmlHighlighter()
		{
			Enclosure doubleQuote = new Enclosure();
			doubleQuote.opener = doubleQuote.closer = "\"";
			doubleQuote.escape = '\\';
			doubleQuote.klass = CharClass.String;
			_Enclosures.Add( doubleQuote );

			Enclosure singleQuote = new Enclosure();
			singleQuote.opener = singleQuote.closer = "'";
			singleQuote.escape = '\\';
			singleQuote.klass = CharClass.String;
			_Enclosures.Add( singleQuote );

			Enclosure comment = new Enclosure();
			comment.opener = "<!--";
			comment.closer = "-->";
			comment.klass = CharClass.Comment;
			_Enclosures.Add( comment );
		}
		#endregion

		#region Highlighting Logic
		/// <summary>
		/// Parse and highlight keywords.
		/// </summary>
		/// <param name="doc">Document to highlight.</param>
		/// <param name="begin">Index to start highlighting.</param>
		/// <param name="end">Index to end highlighting.</param>
		/// <param name="invalidBegin">Begin index of the range to be invalidated.</param>
		/// <param name="invalidEnd">End index of the range to be invalidated.</param>
		public override void Highlight( Document doc, int begin, int end, out int invalidBegin, out int invalidEnd )
		{
			if( begin < 0 || doc.Length < begin )
				throw new ArgumentOutOfRangeException( "begin" );
			if( end < 0 || doc.Length < end )
				throw new ArgumentOutOfRangeException( "end" );

			char nextCh;
			int index, nextIndex;

			// get index to start highlighting
			begin = Utl.FindLast( doc, "<", begin );
			if( begin == -1 )
			{
				begin = 0;
			}
			end = Utl.Find( doc, ">", end, doc.Length );
			if( end == -1 )
			{
				end = doc.Length;
			}
			invalidBegin = begin;
			invalidEnd = end;

			// seek each tags
			index = Utl.Find( doc, "<", begin, doc.Length );
			while( 0 <= index && index < end )
			{
				if( Utl.StartsWith(doc, "<!--", index) )
				{
					// highlight enclosing part if this token begins a part
					nextIndex = TryHighlightEnclosure( doc, _Enclosures, index, end );
					if( index < nextIndex )
					{
						// successfully highlighted. skip to next.
						index = nextIndex;
					}
				}
				else
				{
					// set class for '<'
					doc.SetCharClass( index, CharClass.Keyword );
					index++;
					if( doc.Length <= index )
					{
						return; // reached to the end
					}

					// if next char is '?' or '/', highlight it too
					nextCh = doc[ index ];
					if( nextCh == '?' || nextCh == '/' || nextCh == '!' )
					{
						doc.SetCharClass( index, CharClass.Keyword );
						index++;
						if( doc.Length <= index )
							return; // reached to the end
					}

					// skip whitespaces
					while( Char.IsWhiteSpace(doc[index]) )
					{
						index++;
						if( doc.Length <= index )
							return; // reached to the end
					}

					// highlight element name
					nextIndex = Utl.NextToken( doc, index );
					for( int i=index; i<nextIndex; i++ )
					{
						doc.SetCharClass( i, CharClass.Keyword2 );
					}
					index = nextIndex;

					// highlight attributes
					while( index < doc.Length && doc[index] != '>' )
					{
						// highlight enclosing part if this token begins a part
						nextIndex = TryHighlightEnclosure( doc, _Enclosures, index, end );
						if( index < nextIndex )
						{
							// successfully highlighted. skip to next.
							index = nextIndex;
							continue;
						}

						// this token is normal class; reset classes and seek to next token
						nextIndex = Utl.NextToken( doc, index );
						for( int i=index; i<nextIndex; i++ )
						{
							doc.SetCharClass( i, CharClass.Keyword3 );
						}
						index = nextIndex;
					}

					// highlight '>'
					if( index < doc.Length )
					{
						doc.SetCharClass( index, CharClass.Keyword );
						if( 1 <= index && doc[index-1] == '/' )
							doc.SetCharClass( index-1, CharClass.Keyword );
					}
				}

				// seek to next tag
				index = Utl.Find( doc, "<", index, doc.Length );
			}

			//
			if( 0 <= index && index < doc.Length )
			{
				invalidEnd = Utl.Find( doc, ">", index, doc.Length );
			}
		}

		/// <summary>
		/// Highlight part between a enclosing pair registered.
		/// </summary>
		/// <returns>Index of next parse point if a pair was highlighted or startIndex</returns>
		static int TryHighlightEnclosure( Document doc, List<Enclosure> pairs, int startIndex, int endIndex )
		{
			Enclosure pair;
			int closePos;

			// get pair which begins from this position
			pair = Utl.StartsWith( doc, pairs, startIndex );
			if( pair == null )
			{
				return startIndex; // no pair begins from here.
			}

			// find closing pair
			closePos = Utl.FindCloser( doc, pair, startIndex+pair.opener.Length, endIndex );
			if( closePos == -1 )
			{
				// not found.
				// if this is an opener without closer, highlight
				if( endIndex == doc.Length )
				{
					for( int i=startIndex; i<doc.Length; i++ )
						doc.SetCharClass( i, pair.klass );
					return doc.Length;
				}
				else
				{
					return startIndex;
				}
			}

			// highlight enclosed part
			for( int i = 0; i < closePos + pair.closer.Length - startIndex; i++ )
			{
				doc.SetCharClass( startIndex+i, pair.klass );
			}
			return closePos + pair.closer.Length;
		}

		/// <summary>
		/// Highlight line comment.
		/// </summary>
		/// <returns>Index of next parse point if highlight succeeded or startIndex</returns>
		static int TryHighlightLineComment( Document doc, List<Enclosure> pairs, int startIndex, int endIndex )
		{
			int closePos;
			Enclosure pair;

			// get line comment opener
			pair = Utl.StartsWith( doc, pairs, startIndex );
			if( pair == null )
			{
				return startIndex; // no line-comment begins from here.
			}

			// get line-end pos
			closePos = Utl.GetLineEndIndexFromCharIndex( doc, startIndex );

			// highlight the line
			for( int i=startIndex; i<closePos; i++ )
			{
				doc.SetCharClass( i, pair.klass );
			}
			return closePos;
		}
		#endregion

		#region Utilities
		static class Utl
		{
			public static int GetLineEndIndexFromCharIndex( Document doc, int index )
			{
				int lineIndex = doc.GetLineIndexFromCharIndex( index );
				if( lineIndex+1 < doc.LineCount )
					return doc.GetLineHeadIndex( lineIndex+1 );
				else
					return doc.Length;
			}

			public static void EPI_RemoveBetween( SplitArray<int> epi, int min, int max )
			{
				int i;
				
				if( epi.Count == 0 )
					return;

				// skip first value which overs the 'min'
				i = 0;
				while( i < epi.Count && epi[i] < min )
				{
					if( epi.Count <= i )
						return;
					
					i++;
				}

				// delete values
				while( i < epi.Count && epi[i] < max )
				{
					if( epi.Count <= i )
						return;

					epi.Delete( i, i+1 );
				}
			}

			public static int FindLeastMaximum( SplitArray<int> numbers, int value )
			{
				if( numbers.Count == 0 )
				{
					return -1;
				}

				for( int i=0; i<numbers.Count; i++ )
				{
					if( value <= numbers[i] )
					{
						return i - 1; // this may return -1 but it's okay.
					}
				}

				return numbers.Count - 1;
			}

			public static int NextToken( Document doc, int index )
			{
				if( doc.Length <= index+1 )
					return doc.Length;

				if( Char.IsLetterOrDigit(doc[index]) )
				{
					do
					{
						index++;
						if( doc.Length <= index )
							return doc.Length;
					}
					while( Char.IsLetterOrDigit(doc[index]) );
				}
				else
				{
					index++;
				}
				
				while( Char.IsWhiteSpace(doc[index]) )
				{
					index++;
					if( doc.Length <= index )
						return doc.Length;
				}
				
				return index;
			}

			public static int PrevToken( Document doc, int index )
			{
				return WordLogic.PrevWordStartForMove( doc, index );
			}

			public static bool StartsWith( Document doc, string token, int index )
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

			public static Enclosure StartsWith( Document doc, List<Enclosure> pairs, int index )
			{
				foreach( Enclosure pair in pairs )
				{
					if( StartsWith(doc, pair.opener, index) )
						return pair;
				}
				return null;
			}

			public static string StartsWith( Document doc, List<string> strings, int index )
			{
				foreach( string str in strings )
				{
					if( StartsWith(doc, str, index) )
						return str;
				}
				return null;
			}

			/// <summary>
			/// return closer pos or line-end if closer is null.
			/// </summary>
			public static int FindCloser( Document doc, Enclosure pair, int startIndex, int endIndex )
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

			public static int Find( Document doc, string token, int startIndex, int endIndex )
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

			public static int FindLast( Document doc, string token, int startIndex )
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
		}
		#endregion
	}
}
