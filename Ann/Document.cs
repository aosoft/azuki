// 2008-10-26
using System;
using System.Text;
using Sgry.Azuki;

namespace Sgry.Ann
{
	class Document
	{
		Azuki.Document _AzukiDoc;
		string _FilePath = null;
		Encoding _Encoding = Encoding.Default;
		
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public Document( Azuki.Document azukiDoc )
		{
			_AzukiDoc = azukiDoc;
		}

		/// <summary>
		/// Gets Azuki's document object.
		/// </summary>
		public Azuki.Document AzukiDoc
		{
			get{ return _AzukiDoc; }
		}

		/// <summary>
		/// Gets or sets text content.
		/// </summary>
		public string Text
		{
			get{ return _AzukiDoc.Text; }
			set{ _AzukiDoc.Text = value; }
		}

		/// <summary>
		/// Gets or sets the file path associated with this document.
		/// </summary>
		public string FilePath
		{
			get{ return _FilePath; }
			set{ _FilePath = value; }
		}

		/// <summary>
		/// Gets or sets encoding of the document file.
		/// </summary>
		public Encoding Encoding
		{
			get{ return _Encoding; }
			set{ _Encoding = value; }
		}
	}
}
