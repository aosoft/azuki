// 2008-10-26
using System;
using System.Windows.Forms;
using Sgry.Azuki;
using Sgry.Azuki.Windows;

namespace Sgry.Ann
{
	static partial class Actions
	{
		/// <summary>
		/// Shows a dialog to select visibility of each special chars.
		/// </summary>
		public static AnnAction SelectSpecialCharVisibility
			= delegate( AppLogic app )
		{
			Form dialog;
			DialogResult result;

			using( dialog = new DrawingOptionForm() )
			{
				result = dialog.ShowDialog();
				if( result != DialogResult.OK )
				{
					return;
				}
			}
		};

		/// <summary>
		/// Toggles whether lines should be drawn wrapped or not.
		/// </summary>
		public static AnnAction ToggleWrapLines
			= delegate( AppLogic app )
		{
			AzukiControl azuki = app.MainForm.Azuki;
			if( azuki.ViewType == ViewType.Propotional )
			{
				azuki.ViewType = ViewType.WrappedPropotional;
			}
			else
			{
				azuki.ViewType = ViewType.Propotional;
			}
		};
	}
}
