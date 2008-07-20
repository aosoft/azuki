// 2008-07-20
// encoding: UTF-8
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sgry.Azuki.Windows
{
#	if false
	class AzukiSample
	{
		static void Main_MultiDoc()
		{
			Form form = new Form();
			AzukiControl azuki = new AzukiControl();
			Document doc1 = azuki.Document;
			Document doc2 = new Document();
			MenuItem miDoc1 = new MenuItem();
			MenuItem miDoc2 = new MenuItem();
			BasicHighlighter h;
			EventHandler eh = delegate( object sender, EventArgs e ) {
				if( sender == miDoc1 )
					azuki.Document = doc1;
				else
					azuki.Document = doc2;
			};

			form.Text = "Azuki MD Test";
			azuki.KeyDown += delegate( object sender, KeyEventArgs e ) {
				if( e.KeyData == (Keys.D | Keys.Control) )
				{
					doc1.Highlighter = doc2.Highlighter;
				}
			};

			// setup documents
			h = new BasicHighlighter();
			h.AddEnclosure( "\"", "\"", CharClass.String );
			h.AddEnclosure( "/*", "*/", CharClass.Comment );
			h.AddLineHighlight( "//", CharClass.Comment );
			h.SetKeywords( new string[]{"int", "void", "char"} );
			doc1.Text = @"#include <stdio.h>
int main( int argc, char* argv[] )
{
    return 0;
}
";
			doc1.Highlighter = h;

			h = new BasicHighlighter();
			h.AddEnclosure( "\"", "\"", CharClass.String );
			h.AddEnclosure( "<!--", "-->", CharClass.Comment );
			h.SetKeywords( new string[]{"<", ">", "html", "head", "meta", "body", "title"} );
			doc2.Text = @"<html>
<head>
<meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
	<title>HOGE</title>
</head>
<body>
:)
</body>
</html>
";
			doc2.Highlighter = h;

			// setup menus
			miDoc1.Text = "C document &1";
			miDoc1.Click += eh;
			miDoc2.Text = "HTML document &2";
			miDoc2.Click += eh;
			form.Menu = new MainMenu();
			form.Menu.MenuItems.Add( miDoc1 );
			form.Menu.MenuItems.Add( miDoc2 );

			azuki.Dock = DockStyle.Fill;
			form.Controls.Add( azuki );

			Application.Run( form );
		}
	}
#	else
	class AzukiSample
	{
		const string Text0 = "        b\na\tbc\tdef\nghi\tj\tk";
		const string Text1 = "\"Keep\r\nit	as　simple	as possible but\nnot simpler.\"\rAlbert Einstein said.";
		const string Text2 = @"Windows アプリケーション開発の方策
x64 版 Windows が現れてから少しは時間が経過しているものの、まだ x64 版でのみ動作するアプリケーションを開発するのは現実的ではありません。今後、当分の間は x86 版および x64 版 Windows の両方で動作するアプリケーションの開発が主流になるでしょう。 すると、考えられる開発モデルは次のうちどちらかになります。
・x86 バイナリと x64 バイナリの両方を作成し、アプリケーションの x86 版と x64 版を提供する
・x86 バイナリのみを作成して x86 版アプリケーションを提供するが、x64 版 Windows では WOW64 上での動作をサポートする
前者の開発モデルでは x86、x64 両環境でクロスコンパイルするための知識が必要です。逆に後者の開発モデルでは WOW64 環境とネイティブな x86 環境との違いを理解する必要があります。新規の開発であれば最初からクロスコンパイルできるようにするべきでしょう。なぜなら今後は徐々に x64 環境へと移行していくからです。逆に、クロスコンパイルをまったく考えていないような既存の x86 アプリケーションをメンテナンスするのであれば WOW64 上での動作を完全なものにするのが良い選択肢かもしれません。いずれにしても開発するアプリケーションによって、どちらの方策を採用するかは自然と決まってくるでしょう。
";
		// U+26951 - 0x10000
		// --> 0x16951
		// --> 0001 0110 1001 0101 0001
		// --> 0001011010 0101010001
		//
		//            00 0101 1010 +        01 0101 0001
		// OR) 1101 1000 0000 0000 + 1101 1100 0000 0000 (0xD800 ~ 0xDC00)
		// ---------------------------------------------
		//     1101 1000 0101 1010 + 1101 1101 0101 0001
		//        d    8    5    a      d    d    5    1
		const string Text3 = "臼と似た形をした文字「\xd85a\xdd51」は、Unicode の第 2 面に位置する";
		const string Text4 = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiu


smod tempor incididunt ut labore
et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";
		const string Text5 = @"#include <stdio.h>

//-------------------------------------
// function: main
// brief: application entry point
//-------------------------------------
int
main( int argc, char* argv[] )
{
	/* public private class struct interface { //
	 * can this parser ignore "" in comment?
	 * or // or /* in comment?
	 */
	while( 0 < argc )
		printf( ""can ignore // or /* in a string %s\n"", argv[--argc] );

#	if NDEBUG
	printf( ""and also recognize*/ escapement like \"" and \''.\n"" );
#	endif
	printf( @""escape in C#'s new string format like """" can also be recognized \n"" );

	return 0;
}
";

#		if !PocketPC
		[STAThread]
#		endif
		static void Main()
		{
//Console.Error.WriteLine( GC.GetTotalMemory(true) );
//Console.Error.WriteLine( "IsLittleEndian:{0}", BitConverter.IsLittleEndian );
			Form form = new Form();
			TextBox statusLabel = new TextBox();
			Button okButton = new Button();
			AzukiControl azuki = new AzukiControl();
			double dppY; // dot per point

			// 1pt = 1/72" = 96/72px (@ 96dpi)
			using( Graphics g = form.CreateGraphics() )
			{
				dppY = g.DpiY / 72.0;
			}

			// setup azuki
			azuki.Text = Text5;
			azuki.Font = new Font( "Tahoma", 10, FontStyle.Regular );
			//azuki.Font = new Font( "Meiryo", 16, FontStyle.Regular );
			azuki.ClearHistory();
			azuki.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			azuki.Top = (int)(8 * dppY);
			azuki.Width = form.ClientSize.Width;
			azuki.Height = form.ClientSize.Height - azuki.Top;
//			azuki.ShowLineNumber = false;
			azuki.ViewType = ViewType.WrappedPropotional;
//			azuki.AcceptsReturn = false;
//			azuki.AcceptsTab = false;
//			azuki.HighlightCurrentLine = false;
//			azuki.SetSelection( 24, 24 );
			azuki.ScrollToCaret();
//			azuki.IsReadOnly = true;
			azuki.DrawsSpace = true;
//			azuki.DrawsFullWidthSpace = false;
//			azuki.DrawsTab = false;
//			azuki.DrawsEolCode = false;
			azuki.Document.EolCode = "\r\n";
			azuki.AutoIndentHook = AutoIndentLogic.CHook;
			azuki.Document.Highlighter = CreateCHighlighter( azuki.Document );
			azuki.Resize += delegate {
				if( WrapTextAtWindowBorder )
				{
					azuki.ViewWidth = azuki.ClientSize.Width;
				}
			};
			form.Controls.Add( azuki as Control );

			// setup form
#			if !PocketPC
			form.Text = "Azuki Sample";
#			else
			form.Text = "Azuki Compact Sample";
			form.MinimizeBox = false;
#			endif
			form.Menu = new MainMenu();
			form.Width = 400;
			SetupMenus( form.Menu, azuki );

			// setup status bar
			statusLabel.Font = new Font( "MS Gothic", 8, FontStyle.Regular );
			statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;
			statusLabel.ForeColor = Color.White;
			statusLabel.BackColor = Color.Green;
			statusLabel.Width = (int)( form.ClientSize.Width * (4.0/5.0) );
			statusLabel.Height = (int)(8 * dppY);
			statusLabel.Top = 0;
			statusLabel.Left = 0;
			statusLabel.BorderStyle = BorderStyle.None;
			statusLabel.ReadOnly = true;
			statusLabel.AcceptsTab = false;
			form.Controls.Add( statusLabel );

			// setup button
			okButton.Text = "";
			okButton.Font = new Font( "MS Gothic", 8, FontStyle.Regular );
			okButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;
			okButton.Width = (int)( form.ClientSize.Width * (1.0/5.0) );
			okButton.Height = (int)(8 * dppY);
			okButton.Top = 0;
			okButton.Left = (int)( form.ClientSize.Width * (4.0/5.0) );
			form.Controls.Add( okButton );
#			if !PocketPC
			form.AcceptButton = okButton;
#			endif

			// setup event handler
			azuki.CaretMoved += delegate( object sender, EventArgs e ) {
				string ch;
				int pLine, pColumn;
				int lLine, lColumn;
				string klass = "Normal";
				int caret = azuki.CaretIndex;
				azuki.GetLineColumnIndexFromCharIndex( caret, out pLine, out pColumn );
				azuki.Document.GetLineColumnIndexFromCharIndex( caret, out lLine, out lColumn );
				if( caret == azuki.TextLength )
				{
					ch = "EOF";
				}
				else
				{
					klass = azuki.Document.GetCharClass( caret ).ToString();
					ch = azuki.GetTextInRange( caret, caret+1 );
					if( ch == "\r" ) ch = "\x2190";
					else if( ch == "\n" ) ch = "\x2193";
					else if( ch == "\r\n" ) ch = "\x21b2"; // or \x21b2 or \x23ce
					else if( ch == "\0" ) ch = "\\0";
					else if( ch == "\t" ) ch = "\\t";
					else if( ch == " " ) ch = "SPC";
				}
				
				statusLabel.Text = String.Format( "[{0}|{1}]@{2}, lPos:({3}, {4}), pPos:({5}, {6})", ch, klass, caret, lLine+1, lColumn+1, pLine+1, pColumn+1 );
			};
			okButton.Click += delegate {
				MessageBox.Show( "Hello :)", form.Text );
			};

			// setup keybinds for debug
			azuki.SetKeyBind( Keys.F12,
				delegate( View view ){ azuki.ShowLineNumber = !azuki.ShowLineNumber; }
			);
			azuki.SetKeyBind( Keys.H | Keys.Control,
				delegate( View view ){ azuki.ShowHScrollBar = !azuki.ShowHScrollBar; }
			);
			azuki.SetKeyBind( Keys.R | Keys.Control,
				delegate( View view ){ azuki.Invalidate(); }
			);
			azuki.SetKeyBind( Keys.Scroll,
				delegate( View view ){ System.GC.Collect(); Plat.Inst.MessageBeep(); }
			);

			// run test application
			Application.Run( form );
		}

		static void SetupMenus( Menu menu, AzukiControl azuki )
		{
			MenuItem mi;
			MenuItem miFile = new MenuItem();
			MenuItem miView = new MenuItem();
			MenuItem miMode = new MenuItem();

			// File
			miFile.Text = "&File";
			menu.MenuItems.Add( miFile );

			// File - Open
			mi = new MenuItem();
			mi.Text = "&Open";
			mi.Click += delegate {
				OpenFileDialog ofd = new OpenFileDialog();
				if( ofd.ShowDialog() != DialogResult.Cancel )
				{
					using( StreamReader r = new StreamReader(ofd.FileName, Encoding.Default) )
						azuki.Text = r.ReadToEnd();
					azuki.ClearHistory();
				}
			};
			miFile.MenuItems.Add( mi );

			// File - SaveAs
			mi = new MenuItem();
			mi.Text = "Save &as";
			mi.Click += delegate {
				SaveFileDialog sfd = new SaveFileDialog();
				if( sfd.ShowDialog() != DialogResult.Cancel )
				{
					using( FileStream f = File.OpenWrite(sfd.FileName) )
					{
						byte[] bytes = Encoding.Default.GetBytes( azuki.Text );
						f.Write( bytes, 0, bytes.Length );
					}
					azuki.ClearHistory();
				}
			};
			miFile.MenuItems.Add( mi );

			// View
			miView.Text = "&View";
			menu.MenuItems.Add( miView );

			MenuItem miLN = new MenuItem();
			MenuItem miPV = new MenuItem();
			MenuItem miPWV = new MenuItem();
			MenuItem miWWB = new MenuItem();

			// Tool - ShowLineNumber
			miLN.Text = "Show line &number";
			miLN.Click += delegate {
				miLN.Checked = !miLN.Checked;
				azuki.ShowLineNumber = miLN.Checked;
			};
			miLN.Checked = true;
			miView.MenuItems.Add( miLN );

			// Tool - PropView
			miPV.Text = "Use &propotional view";
			miPV.Click += delegate {
				miPV.Checked = true;
				miPWV.Checked = false;
				azuki.ViewType = ViewType.Propotional;
			};
			miView.MenuItems.Add( miPV );

			// Tool - PropWrapView
			miPWV.Text = "Use &wrapped propotional view";
			miPWV.Checked = true;
			miPWV.Click += delegate {
				miPWV.Checked = true;
				miPV.Checked = false;
				azuki.ViewType = ViewType.WrappedPropotional;
			};
			miView.MenuItems.Add( miPWV );

			// Tool - WrapAtWindowBorder
			miWWB.Text = "Wrap lines at window &border";
			miWWB.Click += delegate {
				miWWB.Checked = !miWWB.Checked;
				WrapTextAtWindowBorder = miWWB.Checked;
			};
			miView.MenuItems.Add( miWWB );

			// Mode
			miMode.Text = "&Mode";
			menu.MenuItems.Add( miMode );

			// Mode - Auto indent
			MenuItem miAI = new MenuItem();
			miAI.Text = "Smart &indent";
			miMode.MenuItems.Add( miAI );

			MenuItem miAI_None = new MenuItem();
			MenuItem miAI_Generic = new MenuItem();
			MenuItem miAI_C = new MenuItem();

			miAI_None.Text = "&None";
			miAI_None.Checked = true;
			miAI_None.Click += delegate {
				miAI_None.Checked = true;
				miAI_Generic.Checked = false;
				miAI_C.Checked = false;
				azuki.AutoIndentHook = null;
			};
			miAI.MenuItems.Add( miAI_None );

			miAI_Generic.Text = "&Basic";
			miAI_Generic.Click += delegate {
				miAI_None.Checked = false;
				miAI_Generic.Checked = true;
				miAI_C.Checked = false;
				azuki.AutoIndentHook = AutoIndentLogic.GenericHook;
			};
			miAI.MenuItems.Add( miAI_Generic );

			miAI_C.Text = "&C";
			miAI_C.Click += delegate {
				miAI_None.Checked = false;
				miAI_Generic.Checked = false;
				miAI_C.Checked = true;
				azuki.AutoIndentHook = AutoIndentLogic.CHook;
			};
			miAI.MenuItems.Add( miAI_C );
		}

		static bool WrapTextAtWindowBorder = false;

		static IHighlighter CreateCHighlighter( Document doc )
		{
			BasicHighlighter h = new BasicHighlighter();

			CharClass ppKlass = new CharClass( 10, "Preprocessor Macro" );
			doc.RegisterCharClass( ppKlass, Color.Purple );

			h.SetKeywords( new string[] {
				"__FILE__", "__declspec",
				"asm", "auto",
				"bool", "break", "case", "catch", "char", "class", "const", "const_cast",
				"continue", "default", "delete", "do", "double", "dynamic_cast", "else",
				"enum", "explicit", "extern", "false", "float",
				"for", "friend", "goto", "if", "inline", "int", "long", "namespace",
				"new", "operator", "private", "protected", "public",
				"reinterpret_cast", "return", "short",
				"signed", "sizeof", "static",
				"interface", "goto", "while", "long", "new", "return"
			}, CharClass.Keyword );
			
			h.SetKeywords( new string[] {
				"#define", "#elif", "#elif", "#endif", "#error", "#if", "#include", "#line", "#pragma", "#undef",
				"__FILE__", "__declspec",
			}, ppKlass );

			h.AddEnclosure( "'", "'", CharClass.String, '\\' );
			h.AddEnclosure( "@\"", "\"", CharClass.String, '\"' );
			h.AddEnclosure( "\"", "\"", CharClass.String, '\\' );
			h.AddEnclosure( "/**", "*/", CharClass.Keyword );
			h.AddEnclosure( "/*", "*/", CharClass.Comment );
			h.AddLineHighlight( "//", CharClass.Comment );

			return h;
		}
	}
#	endif
}
