// 2008-10-26
using System;
using System.Windows.Forms;

namespace Sgry.Ann
{
	delegate void AnnAction( AppLogic app );

	static partial class Actions
	{
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
		/// Save file with another file name.
		/// </summary>
		public static AnnAction SaveFileAs
			= delegate( AppLogic app )
		{
			SaveFileDialog dialog = null;
			DialogResult result;
			
			if( app.ActiveDocument == null )
				return;
			
			using( dialog = new SaveFileDialog() )
			{
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
				app.SaveFile( app.ActiveDocument );
			}
			else
			{
				SaveFileAs( app );
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
