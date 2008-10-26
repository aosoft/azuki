// 2008-10-25
using System;
using System.Windows.Forms;

namespace Sgry.Ann
{
	delegate void AnnAction( AppLogic app );

	static class Actions
	{
		#region Document
		/// <summary>
		/// Shows a dialog and opens a file.
		/// </summary>
		public static void OpenFile( AppLogic app )
		{
			OpenFileDialog dialog = null;
			DialogResult result;
			
			using( dialog = new OpenFileDialog() )
			{
				// show dialog
				result = dialog.ShowDialog();
				if( result != DialogResult.OK )
				{
					return;
				}

				// load the file
				app.OpenFile( dialog.FileName, null );
			}
		}

		/// <summary>
		/// Save file.
		/// </summary>
		public static void SaveFile( AppLogic app )
		{
			//app.SaveFile(  );
		}

		/// <summary>
		/// Close all documents and exit application.
		/// </summary>
		public static void Exit( AppLogic app )
		{
			app.MainForm.Close();
		}
		#endregion
	}
}
