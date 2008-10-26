// 2008-10-26
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Sgry.Azuki;
using Sgry.Azuki.Windows;
using Debug = System.Diagnostics.Debug;
using AzukiDocument = Sgry.Azuki.Document;

namespace Sgry.Ann
{
	class AppLogic
	{
		AnnForm _MainForm = null;
		List<Document> _DAD_Documents = new List<Document>(); // Dont Access Directly
		Document _DAD_ActiveDocument = null; // Dont Access Directly

		#region Properties
		/// <summary>
		/// Gets or sets application's main form.
		/// </summary>
		public AnnForm MainForm
		{
			get{ return _MainForm; }
			set
			{
				_MainForm = value;
				Document doc = new Document( value.Azuki.Document );
				AddDocument( doc );
				ActiveDocument = doc;
MainForm.Azuki.Font = AppConfig.Font;
			}
		}

		/// <summary>
		/// Gets list of documents currently loaded.
		/// </summary>
		public List<Document> Documents
		{
			get{ return _DAD_Documents; }
		}

		/// <summary>
		/// Gets currently active document.
		/// </summary>
		public Document ActiveDocument
		{
			get
			{
				Debug.Assert( _DAD_ActiveDocument == null || _DAD_Documents.Contains(_DAD_ActiveDocument) );
				return _DAD_ActiveDocument;
			}
			set
			{
				Debug.Assert( _DAD_Documents.Contains(value) );
				if( _DAD_ActiveDocument == value )
					return;

				// activate document
				_DAD_ActiveDocument = value;
				_MainForm.Azuki.Document = ActiveDocument.AzukiDoc;
				MainForm.Azuki.ScrollToCaret();

				// update UI
				MainForm.Text = Utl.ToWindowText( _DAD_ActiveDocument );
			}
		}
		#endregion

		#region Document Management
		/// <summary>
		/// Add a document to document list.
		/// </summary>
		public void AddDocument( Document doc )
		{
			Debug.Assert( _DAD_Documents.Contains(doc) == false );
			_DAD_Documents.Add( doc );
		}

		/// <summary>
		/// Removes a document from document list.
		/// </summary>
		public void RemoveDocument( Document doc )
		{
			Debug.Assert( _DAD_Documents.Contains(doc) );

			int index = _DAD_Documents.IndexOf( doc );
			_DAD_Documents.RemoveAt( index );
			if( _DAD_ActiveDocument == doc )
			{
				if( index < _DAD_Documents.Count )
					ActiveDocument = _DAD_Documents[ index ];
				else if( 0 < _DAD_Documents.Count )
					ActiveDocument = _DAD_Documents[ 0 ];
				else
					_DAD_ActiveDocument = null;
			}
		}

		/// <summary>
		/// Switch to next document.
		/// </summary>
		public void ActivateNextDocument()
		{
			if( ActiveDocument == null )
				return;

			int index = _DAD_Documents.IndexOf( _DAD_ActiveDocument );
			if( index+1 < _DAD_Documents.Count )
			{
				ActiveDocument = _DAD_Documents[ index+1 ];
			}
			else
			{
				ActiveDocument = _DAD_Documents[ 0 ];
			}
		}

		/// <summary>
		/// Switch to previous document.
		/// </summary>
		public void ActivatePrevDocument()
		{
			if( ActiveDocument == null )
				return;

			int index = _DAD_Documents.IndexOf( _DAD_ActiveDocument );
			if( 0 <= index-1 )
			{
				ActiveDocument = _DAD_Documents[ index-1 ];
			}
			else
			{
				ActiveDocument = _DAD_Documents[ _DAD_Documents.Count-1 ];
			}
		}
		#endregion

		#region I/O
		/// <summary>
		/// Opens a file with specified encoding.
		/// Specify null to encoding parameter estimates encoding automatically.
		/// </summary>
		public Document OpenFile( string filePath, Encoding encoding, bool withBom )
		{
			Document doc = new Document( new AzukiDocument() );
			StreamReader reader = null;
			char[] buf = new char[ 4 ];
			int readCount = 0;

			// analyze encoding
			if( encoding == null )
			{
				Utl.AnalyzeEncoding( filePath, out encoding, out withBom );
			}
			doc.Encoding = encoding;

			// load file content
			using( reader = new StreamReader(filePath, encoding) )
			{
				while( !reader.EndOfStream )
				{
					readCount += reader.Read( buf, 0, buf.Length );
					doc.AzukiDoc.Replace( new String(buf), doc.AzukiDoc.Length, doc.AzukiDoc.Length );
				}
			}
			doc.AzukiDoc.ClearHistory();
			doc.FilePath = filePath;

			return doc;
		}

		/// <summary>
		/// Save file.
		/// </summary>
		public void SaveFile( Document doc )
		{
			Debug.Assert( doc.FilePath != null, "associate file path to the document before calling SaveFile." );
			StreamWriter writer = null;

			using( writer = new StreamWriter(doc.FilePath, false, doc.Encoding) )
			{
				writer.Write( doc.Text );
			}
			doc.AzukiDoc.IsDirty = false;
		}

		/// <summary>
		/// Save file with another file name.
		/// </summary>
		public void SaveAsFile( Document doc, string filePath )
		{
			StreamWriter writer = null;

			using( writer = new StreamWriter(filePath, false, doc.Encoding) )
			{
				writer.Write( doc.Text );
			}
			doc.AzukiDoc.IsDirty = false;
			doc.FilePath = filePath;
		}

		/// <summary>
		/// Closes a document.
		/// </summary>
		public void CloseDocument( Document doc )
		{
			DialogResult result;

			// confirm to discard modification
			if( doc.AzukiDoc.IsDirty )
			{
				result = Utl.AlertWarning(
					Path.GetFileName(doc.FilePath) + " is modified but not saved. Close anyway?",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button2
				);
				if( result != DialogResult.OK )
				{
					return;
				}
			}

			// close
			RemoveDocument( doc );
		}
		#endregion

		#region Utilities
		static class Utl
		{
			public static string ToWindowText( Document doc )
			{
				return String.Format( "{0} [{1}]", Path.GetFileName(doc.FilePath), doc.Encoding.WebName );
			}

			public static void AnalyzeEncoding( string filePath, out Encoding encoding, out bool withBom )
			{
				const int OneMega = 1024 * 1024;
				Stream file = null;
				byte[] data;
				int dataSize;

				try
				{
					using( file = File.OpenRead(filePath) )
					{
						// prepare buffer
						if( OneMega < file.Length )
							dataSize = OneMega;
						else
							dataSize = (int)file.Length;
						data = new byte[ dataSize ];

						// read data at maximum 1MB
						file.Read( data, 0, dataSize );
						encoding = EncodingAnalyzer.Analyze( data, out withBom );
						if( encoding == null )
						{
							encoding = Encoding.Default;
							withBom = false;
						}
					}
				}
				catch( IOException )
				{
					encoding = Encoding.Default;
					withBom = false;
				}
			}

			public static DialogResult AlertWarning( string message, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
			{
				return MessageBox.Show( message, "Ann", buttons, icon, defaultButton );
			}
		}
		#endregion
	}
}
