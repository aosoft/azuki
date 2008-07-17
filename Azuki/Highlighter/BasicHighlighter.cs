// file: BasicHighlighter.cs
// brief: Keyword based highlighter.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-05
//=========================================================
using System;
using System.Collections.Generic;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace Sgry.Azuki
{
	/// <summary>
	/// A basic highlighter which can highlight
	/// matched keywords and parts being enclosed by specified pair.
	/// </summary>
	public class BasicHighlighter : HighlighterBase
	{
		/*class KeywordSet
		{
			public CharTreeNode root;
			public CharClass klass;
		}*/

		#region Inner Types and Fields
		class Enclosure
		{
			public string opener;
			public string closer;
			public CharClass klass;
			public char escape;
		}

		class CharTreeNode
		{
			public char ch = '\0';
			public CharTreeNode sibling = null;
			public CharTreeNode child = null;
			public int depth = 0;

#			if DEBUG
			public override string ToString()
			{
				return ch.ToString();
			}
#			endif
		}

		CharTreeNode _Root = new CharTreeNode();
		List<Enclosure> _Enclosures = new List<Enclosure>();
		List<Enclosure> _LineHighlights = new List<Enclosure>();
#		if DEBUG
		internal
#		endif
		SplitArray<int> _EPI = new SplitArray<int>( 32, 32 );
		#endregion

		#region Highlight Settings
		/// <summary>
		/// Adds a pair of strings and character-class
		/// that characters between the pair will be classified as.
		/// </summary>
		public void AddEnclosure( string openPattern, string closePattern, CharClass klass )
		{
			AddEnclosure( openPattern, closePattern, klass, '\0' );
		}

		/// <summary>
		/// Adds a pair of strings and character-class
		/// that characters between the pair will be classified as.
		/// </summary>
		public void AddEnclosure( string openPattern, string closePattern, CharClass klass, char escapeChar )
		{
			Enclosure pair = new Enclosure();
			pair.opener = openPattern;
			pair.closer = closePattern;
			pair.klass = klass;
			pair.escape = escapeChar;
			_Enclosures.Add( pair );
		}

		/// <summary>
		/// Clears all registered enclosures.
		/// </summary>
		public void ClearEnclosures()
		{
			_Enclosures.Clear();
		}

		/// <summary>
		/// Adds a line-highlight entry.
		/// </summary>
		/// <param name="openPattern">Opening pattern of the line-comment.</param>
		/// <param name="klass">Class to apply to highlighted text.</param>
		public void AddLineHighlight( string openPattern, CharClass klass )
		{
			Enclosure pair;

			pair = new Enclosure();
			pair.opener = openPattern;
			pair.closer = null;
			pair.klass = klass;

			_LineHighlights.Add( pair );
		}

		/// <summary>
		/// Clears all registered line-comment entries.
		/// </summary>
		public void ClearLineComments()
		{
			_LineHighlights.Clear();
		}

		/// <summary>
		/// Sets keywords to be highlighted.
		/// </summary>
		public void SetKeywords( string[] keywords )
		{
			Array.Sort<string>( keywords );
			for( int i=0; i<keywords.Length; i++ )
			{
				if( i+1 < keywords.Length
					&& keywords[i+1].IndexOf(keywords[i]) == 0 )
				{
					AddCharNode( keywords[i]+'\0', 0, _Root, 1 );
				}
				else
				{
					AddCharNode( keywords[i], 0, _Root, 1 );
				}
			}
		}

		void AddCharNode( string keyword, int index, CharTreeNode parent, int depth )
		{
			CharTreeNode child, node;

			if( keyword.Length <= index )
				return;

			// get child
			child = parent.child;
			if( child == null )
			{
				// no child. create
				child = new CharTreeNode();
				child.ch = keyword[index];
				child.depth = depth;
				parent.child = child;
			}

			// if the child is the char, go down
			if( child.ch == keyword[index] )
			{
				AddCharNode( keyword, index+1, child, depth+1 );
				return;
			}

			// find the char from brothers
			node = child;
			while( node.sibling != null && node.sibling.ch <= keyword[index] )
			{
				// found a node having the char?
				if( node.sibling.ch == keyword[index] )
				{
					// go down
					AddCharNode( keyword, index+1, node.sibling, depth+1 );
					return;
				}

				// get next node
				node = node.sibling;
			}

			// no node having the char exists.
			// create and go down
			CharTreeNode tmp = node.sibling;
			node.sibling = new CharTreeNode();
			node.sibling.ch = keyword[index];
			node.sibling.depth = depth;
			node.sibling.sibling = tmp;
			AddCharNode( keyword, index+1, node.sibling, depth+1 );
		}

		/// <summary>
		/// Clears registered keywords.
		/// </summary>
		public void ClearKeywords()
		{
			_Root = new CharTreeNode();
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

			int index, nextIndex;
			bool highlighted;
			int lastChangedCharIndex = 0;

			// update EPI and get index to start highlighting
			UpdateEPI( doc, begin, out begin, out end );
			invalidBegin = begin;
			invalidEnd = doc.Length;

			// seek each chars and do pattern matching
			index = begin;
			while( 0 <= index && index < end )
			{
				// highlight line-comment if this token is one
				nextIndex = TryHighlightLineComment( doc, _LineHighlights, index, end );
				if( index < nextIndex )
				{
					// successfully highlighted. skip to next.
					index = nextIndex;
					continue;
				}

				// highlight enclosing part if this token begins a part
				nextIndex = TryHighlightEnclosure( doc, _Enclosures, index, end );
				if( index < nextIndex )
				{
					// successfully highlighted. skip to next.
					index = nextIndex;
					continue;
				}

				// highlight keyword if this token is a keyword
				highlighted = TryHighlightKeyword( doc, _Root, index, end, out nextIndex );
				if( highlighted )
				{
					index = nextIndex;
					continue;
				}

				// highlight digit as number
				nextIndex = TryHighlightNumberToken( doc, index, end );
				if( index < nextIndex )
				{
					index = nextIndex;
					continue;
				}
				
				// this token is normal class; reset classes and seek to next token
				nextIndex = Utl.NextToken( doc, index );
				for( int i=index; i<nextIndex; i++ )
				{
					doc.SetCharClass( i, CharClass.Normal );
				}
				lastChangedCharIndex = nextIndex-1;
				index = nextIndex;
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

		/// <summary>
		/// Do keyword matching in [startIndex, endIndex) through keyword char-tree.
		/// </summary>
		static bool TryHighlightKeyword( Document doc, CharTreeNode root, int startIndex, int endIndex, out int nextSeekIndex )
		{
			CharTreeNode node;
			int index;

			// keyword char-tree made with "char", "if", "int", "interface", "long"
			// looks like (where * means a node with null-character):
			//
			//  *-c-h-a-r
			//    |
			//    i-f
			//    | |
			//    | n-t-*
			//    |     |
			//    |     e-r-f-a-c-e
			//    |
			//    l-o-n-g
			//
			// basic matching process:
			// - compares each chars in document to
			//   root child node, root grandchild node and so on
			// - if a node does not match, try next sibling
			//   without advancing seek point of document
			node = root.child;
			index = startIndex;
			while( node != null && index < endIndex )
			{
				// is this node matched to the char?
				if( node.ch == doc[index] )
				{
					// matched.
					if( Matched_Case1(doc, node, index) )
					{
						// next node is null; reached to the end of keyword.
						// highlight and exit
						Utl.Highlight( doc, index, node );
						nextSeekIndex = index + 1;
						return true;
					}
					else if( node.child != null && node.child.ch == '\0' )
					{
						// next node is a null-char.
						if(	index+1 == doc.Length
							|| (index+1 < doc.Length && !Char.IsLetterOrDigit(doc[index+1])) )
						{
							// keyword terminated by null-char in tree was matched.
							Utl.Highlight( doc, index, node );
							nextSeekIndex = index + 1;
							return true;
						}
						else
						{
							// there are following chars.
							// so we should continue matching process for next keyword
							node = node.child.sibling;
							index++;
						}
					}
					else
					{
						// more chars needed to be matched.
						// continue matching process
						node = node.child;
						index++;
					}
				}
				else
				{
					// not matched.
					// try next keyword.
					node = node.sibling;
				}
			}

			nextSeekIndex = index;
			return false;
		}

		/// <summary>
		/// Highlight a token consisted with only digits.
		/// </summary>
		/// <returns>Index of next parse point if a pair was highlighted or 'begin' index</returns>
		static int TryHighlightNumberToken( Document doc, int startIndex, int endIndex )
		{
			Debug.Assert( endIndex <= doc.Length );
			int begin = startIndex;
			int end = begin;

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
		#endregion

		#region Management of Enclosing Pair Indexes
		/// <summary>
		/// This method maintains enlosing pair indexes and
		/// returns range of text to be highlighted.
		/// </summary>
		void UpdateEPI( Document doc, int dirtyBegin, out int begin, out int end )
		{
			int epiIndex;
			int closePos;
			Enclosure pair;

			// calculate re-parse begin index
			epiIndex = Utl.FindLeastMaximum( _EPI, dirtyBegin );
			if( epiIndex < 0 )
			{
				epiIndex = 0;
				begin = doc.GetLineHeadIndexFromCharIndex( dirtyBegin );
			}
			else if( epiIndex % 2 == 0 )
			{
				begin = _EPI[epiIndex];
			}
			else
			{
				begin = _EPI[epiIndex];
				epiIndex++;
			}
			end = doc.Length;

			// remove deleted pair indexes in removed range
			if( epiIndex < _EPI.Count )
			{
				_EPI.Delete( epiIndex, _EPI.Count );
			}

			for( int i=begin; i<end; i++ )
			{
				// ensure a pair begins from here
				pair = Utl.StartsWith( doc, _Enclosures, i );
				if( pair == null )
				{
					continue; // no pair matched
				}

				// remember opener index
				_EPI.Insert( epiIndex, i );
				epiIndex++;

				// find closing pair
				closePos = Utl.FindCloser( doc, pair, i+pair.opener.Length, end );
				if( closePos == -1 )
				{
					break; // no matching closer
				}

				// remember closer index
				_EPI.Insert( epiIndex, closePos + pair.closer.Length );
				epiIndex++;

				// skip this pair
				i = closePos;
			}
		}
		#endregion

		#region Utilities
		/*
		EnclosingPair GetPairStartingFrom( int index )
		{
			EnclosingPair pair;
			string opener;

			pair = Utl.StartsWith( doc, _EnclosingPairs, index );
			if( pair != null )
			{
				return pair;
			}

			opener = Utl.StartsWith( doc, _LineHighlights, index );
			if( opener != null )
			{
				pair = new EnclosingPair();
				pair.opener = opener;
				pair.closer = null;
				pair.klass = CharClass.Comment;
				return pair;
			}

			if( Utl.StartsWith(doc, "\"", index) )
			{
				pair = new EnclosingPair();
				pair.opener = "\"";
				pair.closer = "\"";
				pair.klass = CharClass.String;
				return pair;
			}

			if( Utl.StartsWith(doc, "'", index) )
			{
				pair = new EnclosingPair();
				pair.opener = "'";
				pair.closer = "'";
				pair.klass = CharClass.String;
				return pair;
			}

			return null;
		}
		*/

		static bool Matched_Case1( Document doc, CharTreeNode node, int index )
		{
			if( node.child != null )
				return false;

			if( doc.Length < index+1 )
				return false;

			if( doc.Length == index+1 )
				return true;
			
			if( Char.IsLetterOrDigit(node.ch)
				&& Char.IsLetterOrDigit(doc[index+1]) )
			{
				return false;
			}

			return true;
		}

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

			public static void Highlight( Document doc, int index, CharTreeNode node )
			{
				for( int i=0; i<node.depth; i++ )
				{
					doc.SetCharClass( index-i, CharClass.Keyword );
				}
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

			static int Find( Document doc, string token, int startIndex, int endIndex )
			{
				Debug.Assert( doc != null && token != null );

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
		}

#		if DEBUG
		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			bool indent = true;
			ToString_Sub( _Root.child, buf, ref indent );

			return buf.ToString();
		}
		
		void ToString_Sub( CharTreeNode node, StringBuilder buf, ref bool indent )
		{
			while( node != null )
			{
				// write
				if( indent )
				{
					for( int i=0; i<node.depth; i++ )
						buf.Append( ' ' );
					indent = false;
				}
				buf.Append( node.ch );

				// dive into childs
				if( node.child == null )
				{
					buf.Append( "\r\n" );
					indent = true;
				}
				else
				{
					ToString_Sub( node.child, buf, ref indent );
				}

				// get sibling
				node = node.sibling;
			}
		}
#		endif
		#endregion
	}
}
