// 2008-11-03
using System;
using System.IO;
using System.Windows.Forms;

namespace Sgry.Ann
{
	delegate void AnnAction( AppLogic app );

	static partial class Actions
	{
		#region Fields and Constants
		const string OpenFileFilter = "All files(*.*)|*.*|Text files(*.txt, *.c, ...)|*.txt;*.tex;*.java;*.rb;*.pl;*.py;*.c;*.cpp;*.cxx;*.cs;*.h;*.hpp;*.hxx;*.vbs;*.bat;*.log;*.ini;*.inf;*.js;*.htm;*.html;*.xml";
		const string SaveFileFilter = 
			"Text file(*.txt, *.log, *.ini, ...)|*.txt;*.log;*.ini;*.inf;*.tex"
			+ "|HTML file(*.htm, *.html)|*.htm;*.html"
			+ "|CSS file(*.css)|*.css"
			+ "|Javascript file(*.js)|*.js"
			+ "|XML file(*.xml)|*.xml"
			+ "|C/C++ source(*.c, *.h, ...)|*.c;*.cpp;*.cxx;*.h;*.hpp;*.hxx"
			+ "|C# source(*.cs)|*.cs"
			+ "|Java source(*.java)|*.java"
			+ "|Python script(*.py)|*.py"
			+ "|Ruby script(*.rb)|*.rb"
			+ "|Perl script(*.pl)|*.pl"
			+ "|VB script(*.vbs)|*.vbs"
			+ "|Batch file(*.bat)|*.bat";
		#endregion

		#region Document
		/// <summary>
		/// Shows a dialog and opens a file.
		/// </summary>
		public static AnnAction OpenDocument
			= delegate( AppLogic app )
		{
			OpenFileDialog dialog = null;
			DialogResult result;
			Document doc;
			
			using( dialog = new OpenFileDialog() )
			{
				// setup dialog
				if( app.ActiveDocument.FilePath != null )
				{
					// set initial directory to directory containing currently active file if exists
					string dirPath = Path.GetDirectoryName( app.ActiveDocument.FilePath );
					if( Directory.Exists(dirPath) )
					{
						dialog.InitialDirectory = dirPath;
					}
				}
				dialog.Filter = OpenFileFilter;

				// show dialog
				result = dialog.ShowDialog();
				if( result != DialogResult.OK )
				{
					return;
				}

				// load the file
				doc = app.OpenFile( dialog.FileName, null, false );
				app.AddDocument( doc );

				// activate it
				app.ActiveDocument = doc;
				app.MainForm.Azuki.SetSelection( 0, 0 );
				app.MainForm.Azuki.ScrollToCaret();
			}
		};

		/// <summary>
		/// Save document with another file name.
		/// </summary>
		public static AnnAction SaveDocumentAs
			= delegate( AppLogic app )
		{
			SaveFileDialog dialog = null;
			DialogResult result;
			
			if( app.ActiveDocument == null )
				return;
			
			using( dialog = new SaveFileDialog() )
			{
				// setup dialog
				if( app.ActiveDocument.FilePath != null )
				{
					// set initial directory to directory containing currently active file if exists
					string dirPath = Path.GetDirectoryName( app.ActiveDocument.FilePath );
					if( Directory.Exists(dirPath) )
					{
						dialog.InitialDirectory = dirPath;
					}
				}
				dialog.Filter = SaveFileFilter;

				// show dialog
				result = dialog.ShowDialog();
				if( result != DialogResult.OK )
				{
					return;
				}

				// associate the file path
				app.ActiveDocument.FilePath = dialog.FileName;

				// delegate to overwrite logic
				SaveDocument( app );
			}
		};

		/// <summary>
		/// Save file.
		/// </summary>
		public static AnnAction SaveDocument
			= delegate( AppLogic app )
		{
			if( app.ActiveDocument.FilePath != null )
			{
				app.SaveDocument( app.ActiveDocument );
			}
			else
			{
				SaveDocumentAs( app );
			}
		};

		/// <summary>
		/// Close active document.
		/// </summary>
		public static AnnAction CloseDocument
			= delegate( AppLogic app )
		{
			app.CloseDocument( app.ActiveDocument );
		};

		/// <summary>
		/// Close all documents and exit application.
		/// </summary>
		public static AnnAction Exit
			= delegate( AppLogic app )
		{
			app.MainForm.Close();
		};
		#endregion
	}
}
