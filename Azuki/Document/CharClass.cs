// file: CharClass.cs
// brief: Enumeration to indicate class of characters.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-03
//=========================================================

namespace Sgry.Azuki
{
	/// <summary>
	/// Class of characters mainly for syntax highlighting.
	/// </summary>
	public enum CharClass : ushort
	{
		/// <summary>Indicates normal text.</summary>
		Normal = 0,

		/// <summary>Indicates number.</summary>
		Number = 1,
		
		/// <summary>Indicates string.</summary>
		String = 2,
		
		/// <summary>Indicates comment.</summary>
		Comment = 3,
		
		/// <summary>Indicates documentation comment.</summary>
		DocComment = 4,

		/// <summary>Indicates keyword.</summary>
		Keyword = 5,
		
		/// <summary>
		/// This is invalid char-class.
		/// Used internally in painting logic.
		/// </summary>
		Selection = 0xff
	}
}
