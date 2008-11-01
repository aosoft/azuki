// 2008-11-01
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Sgry.Ann
{
	class DocumentListForm : Form
	{
		List<Document> _Documents = null;
		int _SelectedIndex = 0;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public DocumentListForm()
		{
			InitializeComponent();
#			if !PocketPC
			StartPosition = FormStartPosition.CenterParent;
			AutoScaleMode = AutoScaleMode.Font;
			Font = SystemInformation.MenuFont;
#			endif
		}

		/// <summary>
		/// Sets documents to be shown in this dialog.
		/// </summary>
		public List<Document> Documents
		{
			set
			{
				_Documents = value;
				_ListView.Items.Clear();
				foreach( Document doc in _Documents )
				{
					ListViewItem lvItem = new ListViewItem();
					lvItem.Text = Path.GetFileName( doc.FilePath );
					_ListView.Items.Add( lvItem );
				}
				_CH_FileName.Width = -1; // expand minimum width to show all lines
				_CH_Directory.Width = -2; // expand for maximum width available
			}
		}

		/// <summary>
		/// Gets the document selected by user;
		/// </summary>
		public Document SelectedDocument
		{
			get
			{
				if( _SelectedIndex < _Documents.Count )
					return _Documents[ _SelectedIndex ];
				else
					return null;
			}
		}

		void _ListView_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Enter )
			{
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		protected override void OnClosed( EventArgs e )
		{
			if( DialogResult == DialogResult.OK )
			{
				_SelectedIndex = _ListView.FocusedItem.Index;
			}

			base.OnClosed( e );
		}

		#region UI Component Initialization
		void InitializeComponent()
		{
			_ListView = new ListView();
			_CH_FileName = new ColumnHeader();
			_CH_Directory = new ColumnHeader();
			SuspendLayout();
			// 
			// _ListView
			// 
			_ListView.Columns.Add( _CH_FileName );
			_ListView.Columns.Add( _CH_Directory );
			_ListView.Dock = DockStyle.Fill;
			_ListView.FullRowSelect = true;
			_ListView.Name = "_ListView";
			_ListView.TabIndex = 0;
			_ListView.View = View.Details;
			_ListView.KeyDown += _ListView_KeyDown;
			// 
			// _CH_FileName
			// 
			_CH_FileName.Text = "File name";
			_CH_FileName.Width = 160;
			// 
			// _CH_Directory
			// 
			_CH_Directory.Text = "Directory";
			_CH_Directory.Width = 230;
			// 
			// DocumentListForm
			// 
			Controls.Add( _ListView );
			Name = "DocumentListForm";
			Text = "DocumentListForm";
			ResumeLayout( false );
		}
		#endregion

		#region UI Components
		ListView _ListView;
		ColumnHeader _CH_FileName;
		ColumnHeader _CH_Directory;
		#endregion
	}
}