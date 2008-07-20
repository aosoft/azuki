// file: CharClass.cs
// brief: Indicator for class of characters.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-20
//=========================================================

namespace Sgry.Azuki
{
	/// <summary>
	/// Class of characters mainly for syntax highlighting.
	/// </summary>
	public struct CharClass
	{
		#region Fields
		static CharClass _Normal = new CharClass( 0, "Normal" );
		static CharClass _Number = new CharClass( 1, "Number" );
		static CharClass _String = new CharClass( 2, "String" );
		static CharClass _Comment = new CharClass( 3, "Comment" );
		static CharClass _DocComment = new CharClass( 4, "DocComment" );
		static CharClass _Keyword = new CharClass( 5, "Keyword 1" );
		static CharClass _Selection = new CharClass( 0xff, "Normal" );

		byte _Id;
		string _Name;
		#endregion

		#region Init / Dispose
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public CharClass( byte id, string name )
		{
			_Id = id;
			_Name = name;
		}
		#endregion

		#region Properties
		/// <summary>
		/// ID of this char-class.
		/// </summary>
		public byte Id
		{
			get{ return _Id; }
			set{ _Id = value; }
		}
		#endregion

		#region Properties of an object in programming
		/// <summary>
		/// Operator ==.
		/// </summary>
		public static bool operator ==( CharClass x, CharClass y )
		{
			return x._Id == y._Id;
		}

		/// <summary>
		/// Operator !=.
		/// </summary>
		public static bool operator !=( CharClass x, CharClass y )
		{
			return !( x == y );
		}

		/// <summary>
		/// Tests whether two objects are equal or not.
		/// </summary>
		public override bool Equals( object obj )
		{
			if( obj is CharClass )
				return this == (CharClass)obj;
			return false;
		}

		/// <summary>
		/// Gets hash code of this object.
		/// </summary>
		public override int GetHashCode()
		{
			return _Id;
		}

		/// <summary>
		/// Gets name of this char-class.
		/// </summary>
		public override string ToString()
		{
			return _Name;
		}
		#endregion

		#region Pre-defined classes
		/// <summary>
		/// Indicates normal text.
		/// </summary>
		public static CharClass Normal
		{
			get{ return _Normal; }
		}

		/// <summary>
		/// Indicates number.
		/// </summary>
		public static CharClass Number
		{
			get{ return _Number; }
		}
		
		/// <summary>
		/// Indicates string.
		/// </summary>
		public static CharClass String
		{
			get{ return _String; }
		}
		
		/// <summary>
		/// Indicates comment.
		/// </summary>
		public static CharClass Comment
		{
			get{ return _Comment; }
		}
		
		/// <summary>
		/// Indicates documentation comment.
		/// </summary>
		public static CharClass DocComment
		{
			get{ return _DocComment; }
		}

		/// <summary>
		/// Indicates keyword.
		/// </summary>
		public static CharClass Keyword
		{
			get{ return _Keyword; }
		}
		
		/// <summary>
		/// This is invalid char-class.
		/// Used internally in painting logic.
		/// </summary>
		public static CharClass Selection
		{
			get{ return CharClass._Selection; }
		}
		#endregion
	}
}
