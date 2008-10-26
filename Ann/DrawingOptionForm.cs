// file: DrawingOptionForm.cs
// brief: dialog to setup drawing option
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-10-26
//=========================================================
using System;
using System.Windows.Forms;
using Sgry.Azuki;
using Sgry.Azuki.Windows;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace Sgry.Ann
{
	class DrawingOptionForm : Form
	{
		public DrawingOptionForm()
		{
			InitializeComponent();

			// setup azuki
			_Azuki_Preview.Font = new Font( "Courier New", 10, FontStyle.Regular );
			_Azuki_Preview.IsReadOnly = false;
			_Azuki_Preview.Text = "#include <stdio.h>\n\nint main( int argc, char* argv[] ) {\n\tprintf( \"こんにちは　世界！\\n\" );\n\treturn 0;\n}\n";
			_Azuki_Preview.IsReadOnly = true;
			_Azuki_Preview.Highlighter = Azuki.HighlighterFactory.CppHighlighter;

			// setup dialog
#			if !PocketPC
			base.AcceptButton = _Button_OK;
			base.CancelButton = _Button_Cancel;
#			endif
		}

		#region UI Event Handling
		void _Check_DrawsSpace_CheckedChanged( object sender, EventArgs e )
		{
			CheckBox checkBox = (CheckBox)sender;
			if( checkBox == _Check_DrawsEolCode )
			{
				_Azuki_Preview.DrawsEolCode = _Check_DrawsEolCode.Checked;
			}
			else if( checkBox == _Check_DrawsFullWidthSpace )
			{
				_Azuki_Preview.DrawsFullWidthSpace = _Check_DrawsFullWidthSpace.Checked;
			}
			else if( checkBox == _Check_DrawsSpace )
			{
				_Azuki_Preview.DrawsSpace = _Check_DrawsSpace.Checked;
			}
			else if( checkBox == _Check_DrawsTab )
			{
				_Azuki_Preview.DrawsTab = _Check_DrawsTab.Checked;
			}
			else if( checkBox == _Check_HighlightCurrentLine )
			{
				_Azuki_Preview.HighlightsCurrentLine = _Check_HighlightCurrentLine.Checked;
			}
			else if( checkBox == _Check_ShowsLineNumber )
			{
				_Azuki_Preview.ShowsLineNumber = _Check_ShowsLineNumber.Checked;
			}
		}

		void _Button_OK_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		void _Button_Cancel_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		#endregion

		#region UI Component Initialization
		void InitializeComponent()
		{
			this._Azuki_Preview = new Sgry.Azuki.Windows.AzukiControl();
			this._Button_OK = new System.Windows.Forms.Button();
			this._Button_Cancel = new System.Windows.Forms.Button();
			this._Edit_TabWidth = new System.Windows.Forms.TextBox();
			this._Label_TabWidth = new System.Windows.Forms.Label();
			this._Check_DrawsSpace = new System.Windows.Forms.CheckBox();
			this._Check_DrawsTab = new System.Windows.Forms.CheckBox();
			this._Check_DrawsFullWidthSpace = new System.Windows.Forms.CheckBox();
			this._Check_DrawsEolCode = new System.Windows.Forms.CheckBox();
			this._Check_HighlightCurrentLine = new System.Windows.Forms.CheckBox();
			this._Check_ShowsLineNumber = new System.Windows.Forms.CheckBox();
			this._Label_Preview = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _Azuki_Preview
			// 
			this._Azuki_Preview.AcceptsReturn = false;
			this._Azuki_Preview.AcceptsTab = false;
			this._Azuki_Preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Azuki_Preview.IsReadOnly = true;
			this._Azuki_Preview.Location = new System.Drawing.Point( 12, 212 );
			this._Azuki_Preview.Name = "_Azuki_Preview";
			this._Azuki_Preview.Size = new System.Drawing.Size( 260, 114 );
			this._Azuki_Preview.TabIndex = 9;
			this._Azuki_Preview.TabStop = false;
			this._Azuki_Preview.TabWidth = 8;
			this._Azuki_Preview.ViewWidth = 328;
			// 
			// _Button_OK
			// 
			this._Button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._Button_OK.Location = new System.Drawing.Point( 116, 332 );
			this._Button_OK.Name = "_Button_OK";
			this._Button_OK.Size = new System.Drawing.Size( 75, 23 );
			this._Button_OK.TabIndex = 10;
			this._Button_OK.Text = "OK";
			this._Button_OK.Click += new System.EventHandler( this._Button_OK_Click );
			// 
			// _Button_Cancel
			// 
			this._Button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._Button_Cancel.Location = new System.Drawing.Point( 197, 332 );
			this._Button_Cancel.Name = "_Button_Cancel";
			this._Button_Cancel.Size = new System.Drawing.Size( 75, 23 );
			this._Button_Cancel.TabIndex = 11;
			this._Button_Cancel.Text = "Cancel";
			this._Button_Cancel.Click += new System.EventHandler( this._Button_Cancel_Click );
			// 
			// _Edit_TabWidth
			// 
			this._Edit_TabWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Edit_TabWidth.Location = new System.Drawing.Point( 98, 163 );
			this._Edit_TabWidth.Name = "_Edit_TabWidth";
			this._Edit_TabWidth.Size = new System.Drawing.Size( 174, 19 );
			this._Edit_TabWidth.TabIndex = 7;
			// 
			// _Label_TabWidth
			// 
			this._Label_TabWidth.Location = new System.Drawing.Point( 12, 166 );
			this._Label_TabWidth.Name = "_Label_TabWidth";
			this._Label_TabWidth.Size = new System.Drawing.Size( 80, 16 );
			this._Label_TabWidth.TabIndex = 6;
			this._Label_TabWidth.Text = "Tab &width";
			// 
			// _Check_DrawsSpace
			// 
			this._Check_DrawsSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_DrawsSpace.Location = new System.Drawing.Point( 12, 12 );
			this._Check_DrawsSpace.Name = "_Check_DrawsSpace";
			this._Check_DrawsSpace.Size = new System.Drawing.Size( 260, 19 );
			this._Check_DrawsSpace.TabIndex = 0;
			this._Check_DrawsSpace.Text = "Draws &space";
			this._Check_DrawsSpace.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Check_DrawsTab
			// 
			this._Check_DrawsTab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_DrawsTab.Location = new System.Drawing.Point( 12, 37 );
			this._Check_DrawsTab.Name = "_Check_DrawsTab";
			this._Check_DrawsTab.Size = new System.Drawing.Size( 260, 19 );
			this._Check_DrawsTab.TabIndex = 1;
			this._Check_DrawsTab.Text = "Draws &tab";
			this._Check_DrawsTab.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Check_DrawsFullWidthSpace
			// 
			this._Check_DrawsFullWidthSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_DrawsFullWidthSpace.Location = new System.Drawing.Point( 12, 62 );
			this._Check_DrawsFullWidthSpace.Name = "_Check_DrawsFullWidthSpace";
			this._Check_DrawsFullWidthSpace.Size = new System.Drawing.Size( 260, 19 );
			this._Check_DrawsFullWidthSpace.TabIndex = 2;
			this._Check_DrawsFullWidthSpace.Text = "Draws &full width space (U+3000)";
			this._Check_DrawsFullWidthSpace.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Check_DrawsEolCode
			// 
			this._Check_DrawsEolCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_DrawsEolCode.Location = new System.Drawing.Point( 12, 87 );
			this._Check_DrawsEolCode.Name = "_Check_DrawsEolCode";
			this._Check_DrawsEolCode.Size = new System.Drawing.Size( 260, 19 );
			this._Check_DrawsEolCode.TabIndex = 3;
			this._Check_DrawsEolCode.Text = "Draws EO&L (end of line) code";
			this._Check_DrawsEolCode.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Check_HighlightCurrentLine
			// 
			this._Check_HighlightCurrentLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_HighlightCurrentLine.Location = new System.Drawing.Point( 12, 112 );
			this._Check_HighlightCurrentLine.Name = "_Check_HighlightCurrentLine";
			this._Check_HighlightCurrentLine.Size = new System.Drawing.Size( 260, 19 );
			this._Check_HighlightCurrentLine.TabIndex = 4;
			this._Check_HighlightCurrentLine.Text = "Highlights &current line";
			this._Check_HighlightCurrentLine.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Check_ShowsLineNumber
			// 
			this._Check_ShowsLineNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Check_ShowsLineNumber.Location = new System.Drawing.Point( 12, 137 );
			this._Check_ShowsLineNumber.Name = "_Check_ShowsLineNumber";
			this._Check_ShowsLineNumber.Size = new System.Drawing.Size( 260, 19 );
			this._Check_ShowsLineNumber.TabIndex = 5;
			this._Check_ShowsLineNumber.Text = "Shows line &number";
			this._Check_ShowsLineNumber.Click += new System.EventHandler( this._Check_DrawsSpace_CheckedChanged );
			// 
			// _Label_Preview
			// 
			this._Label_Preview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._Label_Preview.Location = new System.Drawing.Point( 10, 193 );
			this._Label_Preview.Name = "_Label_Preview";
			this._Label_Preview.Size = new System.Drawing.Size( 80, 16 );
			this._Label_Preview.TabIndex = 8;
			this._Label_Preview.Text = "Preview";
			// 
			// DrawingOptionForm
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size( 284, 367 );
			this.Controls.Add( this._Check_ShowsLineNumber );
			this.Controls.Add( this._Check_HighlightCurrentLine );
			this.Controls.Add( this._Check_DrawsEolCode );
			this.Controls.Add( this._Check_DrawsFullWidthSpace );
			this.Controls.Add( this._Check_DrawsTab );
			this.Controls.Add( this._Check_DrawsSpace );
			this.Controls.Add( this._Azuki_Preview );
			this.Controls.Add( this._Label_Preview );
			this.Controls.Add( this._Label_TabWidth );
			this.Controls.Add( this._Edit_TabWidth );
			this.Controls.Add( this._Button_Cancel );
			this.Controls.Add( this._Button_OK );
			this.Name = "DrawingOptionForm";
			this.Text = "Ann - Drawing Options";
			this.ResumeLayout( false );
		}
		#endregion

		#region UI Components
		AzukiControl _Azuki_Preview;
		Button _Button_OK;
		Button _Button_Cancel;
		TextBox _Edit_TabWidth;
		Label _Label_TabWidth;
		CheckBox _Check_DrawsSpace;
		CheckBox _Check_DrawsTab;
		CheckBox _Check_DrawsFullWidthSpace;
		CheckBox _Check_HighlightCurrentLine;
		CheckBox _Check_ShowsLineNumber;
		Label _Label_Preview;
		CheckBox _Check_DrawsEolCode;
		#endregion
	}
}
