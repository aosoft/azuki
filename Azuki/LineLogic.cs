// file: LineLogic.cs
// brief: Logics to manipulate line/column in a string.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-09-26
//=========================================================
using System;
using System.Collections;
using System.Text;

namespace Sgry.Azuki
{
	/// <summary>
	/// Logics to handle line/column in a buffer.
	/// In this logic, "line" means characters with one EOL code at tail.
	/// </summary>
	static class LineLogic
	{
		public static readonly char[] EolChars = new char[]{ '\r', '\n' };

		#region Index Conversion
		public static int GetCharIndexFromLineColumnIndex( TextBuffer text, SplitArray<int> lhi, int lineIndex, int columnIndex )
		{
			DebugUtl.Assert( text != null && lhi != null && 0 <= lineIndex && 0 <= columnIndex, "invalid arguments were given" );
			DebugUtl.Assert( lineIndex < lhi.Count, String.Format("too large line index was given (given:{0} actual line count:{1})", lineIndex, lhi.Count) );

			int lineHeadIndex = lhi[lineIndex];

#			if DEBUG
			int lineLength = GetLineLengthByCharIndex( text, lineHeadIndex );
			if( lineLength < columnIndex )
			{
				if( lineIndex == lhi.Count-1
					&& lineLength+1 == columnIndex )
				{
					// indicates EOF. this case is valid.
				}
				else
				{
					throw new ArgumentOutOfRangeException( String.Format("specified column index was too large (given:{0} actual line length:{1})", columnIndex, lineLength) );
				}
			}
#			endif

			return lineHeadIndex + columnIndex;
		}

		public static int GetLineIndexFromCharIndex( SplitArray<int> lhi, int charIndex )
		{
			DebugUtl.Assert( 0 <= charIndex, "invalid args; given charIndex was "+charIndex );

			// find the first line whose line-head index exceeds given char-index
			for( int i=1; i<lhi.Count; i++ )
			{
				int nextLineHeadIndex = lhi[i];
				if( charIndex < nextLineHeadIndex )
				{
					// found the NEXT line of the target line.
					return i-1;
				}
			}

			// if given index indicates the EOF code, return its index
			// (giving char-index indicating after EOF would be the same result but only in release build.
			// in debug build, giving such index causes assertion error.)
			return lhi.Count - 1;
		}

		public static void GetLineColumnIndexFromCharIndex( TextBuffer text, SplitArray<int> lhi, int charIndex, out int lineIndex, out int columnIndex )
		{
			DebugUtl.Assert( text != null && lhi != null );
			DebugUtl.Assert( 0 <= charIndex, "invalid args; given charIndex was "+charIndex );
			DebugUtl.Assert( charIndex <= text.Count, String.Format("given charIndex was too large (given:{0} actual text count:{1})", charIndex, text.Count) );

			// find the first line whose line-head index exceeds given char-index
			for( int i=1; i<lhi.Count; i++ )
			{
				int nextLineHeadIndex = lhi[i];
				if( charIndex < nextLineHeadIndex )
				{
					// found the NEXT line of the target line.
					lineIndex = i-1;
					columnIndex = charIndex - lhi[i-1];
					return;
				}
			}

			// if given index indicates the EOF code, return its index
			// (giving char-index indicating after EOF would be the same result but only in release build.
			// in debug build, giving such index causes assertion error.)
			lineIndex = lhi.Count - 1;
			columnIndex = charIndex - lhi[lineIndex];
		}

		public static int GetLineHeadIndexFromCharIndex( TextBuffer text, SplitArray<int> lhi, int charIndex )
		{
			DebugUtl.Assert( text != null && lhi != null );
			DebugUtl.Assert( 0 <= charIndex, "invalid arguments were given ("+charIndex+")" );
			DebugUtl.Assert( charIndex <= text.Count, String.Format("too large char-index was given (given:{0} actual text count:{1})", charIndex, text.Count) );

			// find the first line whose line-head index exceeds given char-index
			for( int i=1; i<lhi.Count; i++ )
			{
				int nextLineHeadIndex = lhi[i];
				if( charIndex < nextLineHeadIndex )
				{
					// found the NEXT line of the target line.
					return lhi[i-1];
				}
			}

			// if given index indicates the EOF code, return its index
			// (giving char-index indicating after EOF would be the same result but only in release build.
			// in debug build, giving such index causes assertion error.)
			return lhi[ lhi.Count-1 ];
		}
		#endregion

		#region Line Range
		public static void GetLineRangeWithEol( TextBuffer text, SplitArray<int> lhi, int lineIndex, out int begin, out int end )
		{
			DebugUtl.Assert( lineIndex < lhi.Count, "argument out of range; given lineIndex is "+lineIndex+" but lhi.Count is "+lhi.Count );

			// get range of the line including EOL code
			begin = lhi[lineIndex];
			if( lineIndex+1 < lhi.Count )
			{
				end = lhi[lineIndex + 1];
			}
			else
			{
				end = text.Count;
			}
		}

		public static void GetLineRange( TextBuffer text, SplitArray<int> lhi, int lineIndex, out int begin, out int end )
		{
			DebugUtl.Assert( 0 <= lineIndex && lineIndex < lhi.Count, "argument out of range; given lineIndex is "+lineIndex+" but lhi.Count is "+lhi.Count );
			int length;

			// get range of the line including EOL code
			begin = lhi[lineIndex];
			if( lineIndex+1 < lhi.Count )
			{
				end = lhi[lineIndex + 1];
			}
			else
			{
				end = text.Count;
			}

			// subtract length of the trailing EOL code
			length = end - begin;
			if( 1 <= length && text.GetAt(end-1) == '\n' )
			{
				if( 2 <= length && text.GetAt(end-2) == '\r' )
					end -= 2;
				else
					end--;
			}
			else if( 1 <= length && text.GetAt(end-1) == '\r' )
			{
				end--;
			}
		}
		#endregion

		#region Line Head Index Management
		/// <summary>
		/// Maintain line head indexes for text insertion.
		/// </summary>
		public static void LHI_Insert( SplitArray<int> lhi, TextBuffer text, string insertText, int insertIndex )
		{
			DebugUtl.Assert( 0 < lhi.Count && lhi[0] == 0, "lhi must have 0 as a first member." );
			int insL, insC;
			int lineHeadIndex;
			int lineEndIndex;
			int insLineCount;

			// at first, find the line which contains the insertion point
			GetLineColumnIndexFromCharIndex( text, lhi, insertIndex, out insL, out insC );

			// if multiple lines are inserted, insert line index entry to LHI
			insLineCount = 1;
			lineHeadIndex = 0;
			do
			{
				// get end index of this line
				lineEndIndex = NextLineHead( insertText, lineHeadIndex ) - 1;
				if( lineEndIndex == -2 ) // == "if NextLineHead returns -1"
				{
					// no more lines are followred to this line.
					// this is the final line. no need to insert new entry
					break;
				}
				lhi.Insert( insL+insLineCount, insertIndex+lineEndIndex+1 );
				insLineCount++;

				// find next line head
				lineHeadIndex = NextLineHead( insertText, lineHeadIndex );
			}
			while( lineHeadIndex != -1 );

			// shift all followings
			for( int i=insL+insLineCount; i<lhi.Count; i++ )
			{
				lhi[i] += insertText.Length;
			}
		}
		
		/// <summary>
		/// Maintain line head indexes for text deletion.
		/// THIS MUST BE CALLED BEFORE DELETING.
		/// </summary>
		public static void LHI_Delete( SplitArray<int> lhi, TextBuffer text, int delBegin, int delEnd )
		{
			DebugUtl.Assert( 0 < lhi.Count && lhi[0] == 0, "lhi must have 0 as a first member." );
			int delFromL, delFromC, delToL, delToC;
			int delLen = delEnd - delBegin;
			
			// calculate line indexes of both ends of the range
			GetLineColumnIndexFromCharIndex( text, lhi, delBegin, out delFromL, out delFromC );
			GetLineColumnIndexFromCharIndex( text, lhi, delEnd, out delToL, out delToC );

			// subtract line head indexes for lines after deletion point
			for( int i=delToL+1; i<lhi.Count; i++ )
			{
				lhi[i] -= delLen;
			}

			// if deletion decrease line count, delete entries
			if( delFromL < delToL )
			{
				lhi.Delete( delFromL+1, delToL+1 );
			}
		}
		#endregion

		#region Utilities
		public static int CountLine( string text )
		{
			int count = 0;
			int lineHead = 0;

			lineHead = NextLineHead( text, lineHead );
			while( lineHead != -1 )
			{
				count++;
				lineHead = NextLineHead( text, lineHead );
			}

			return count + 1;
		}

		public static bool IsEolChar( char ch )
		{
			return (ch == '\r' || ch == '\n');
		}

		public static bool IsEolChar( string str, int index )
		{
			if( 0 <= index && index < str.Length )
			{
				char ch = str[index];
				return (ch == '\r' || ch == '\n');
			}
			else
			{
				return false;
			}
		}

		public static int NextLineHead( TextBuffer str, int searchFromIndex )
		{
			for( int i=searchFromIndex; i<str.Count; i++ )
			{
				// found EOL code?
				if( str[i] == '\r' )
				{
					if( i+1 < str.Count
						&& str[i+1] == '\n' )
					{
						return i+2;
					}

					return i+1;
				}
				else if( str[i] == '\n' )
				{
					return i+1;
				}
			}

			return -1; // not found
		}

		public static int NextLineHead( string str, int searchFromIndex )
		{
			for( int i=searchFromIndex; i<str.Length; i++ )
			{
				// found EOL code?
				if( str[i] == '\r' )
				{
					if( i+1 < str.Length
						&& str[i+1] == '\n' )
					{
						return i+2;
					}

					return i+1;
				}
				else if( str[i] == '\n' )
				{
					return i+1;
				}
			}

			return -1; // not found
		}

		public static int PrevLineHead( TextBuffer str, int searchFromIndex )
		{
			DebugUtl.Assert( searchFromIndex <= str.Count, "invalid argument; searchFromIndex is too large ("+searchFromIndex+" but str.Count is "+str.Count+")" );

			if( str.Count <= searchFromIndex )
			{
				searchFromIndex = str.Count - 1;
			}

			for( int i=searchFromIndex-1; 0<=i; i-- )
			{
				// found EOL code?
				if( str[i] == '\n' )
				{
					return i+1;
				}
				else if( str[i] == '\r' )
				{
					if( i+1 < str.Count
						&& str[i+1] == '\n' )
					{
						continue;
					}
					return i+1;
				}
			}

			return 0;
		}

		public static int PrevLineHead( string str, int searchFromIndex )
		{
			DebugUtl.Assert( searchFromIndex <= str.Length, "invalid argument; searchFromIndex is too large ("+searchFromIndex+" but str.Length is "+str.Length+")" );

			if( str.Length <= searchFromIndex )
			{
				searchFromIndex = str.Length - 1;
			}

			for( int i=searchFromIndex-1; 0<=i; i-- )
			{
				// found EOL code?
				if( str[i] == '\n' )
				{
					return i+1;
				}
				else if( str[i] == '\r' )
				{
					if( i+1 < str.Length
						&& str[i+1] == '\n' )
					{
						continue;
					}
					return i+1;
				}
			}

			return 0;
		}

		/// <summary>
		/// Find non-EOL char from specified index.
		/// Note that the char at specified index always be skipped.
		/// </summary>
		public static int PrevNonEolChar( TextBuffer str, int searchFromIndex )
		{
			for( int i=searchFromIndex-1; 0<=i; i-- )
			{
				if( IsEolChar(str[i]) != true )
				{
					// found non-EOL code
					return i;
				}
			}

			return -1; // not found
		}

		public static int GetLineLengthByCharIndex( TextBuffer text, int charIndex )
		{
			int prevLH = PrevLineHead( text, charIndex );
			int nextLH = NextLineHead( text, charIndex );
			if( nextLH == -1 )
			{
				nextLH = text.Count - 1;
			}

			return (nextLH - prevLH);
		}

		public static int GetLineLengthByCharIndex( string text, int charIndex )
		{
			int prevLH = PrevLineHead( text, charIndex );
			int nextLH = NextLineHead( text, charIndex );
			if( nextLH == -1 )
			{
				nextLH = text.Length - 1;
			}

			return (nextLH - prevLH);
		}
		#endregion
	}
}
