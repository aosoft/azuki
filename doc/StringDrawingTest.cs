using System;
using System.Drawing;
using System.Windows.Forms;

static class Program
{
	static void Main()
	{
		Application.Run( new Form1() );
	}
}

class Form1 : Form
{
	public Form1()
	{
		const string text = "HOGE";
		Font = new Font( "Meiryo", 24 );
		Timer timer = new Timer();
		int x = 0;

		Paint += delegate( object sender, PaintEventArgs e ) {
			Graphics g = e.Graphics;

			g.DrawString( text, Font, Brushes.Black, 0, 0, StringFormat.GenericTypographic );
			g.DrawString( text, Font, Brushes.Black, 0, Font.Height, StringFormat.GenericDefault );
			TextRenderer.DrawText( g, text, Font, new Point(0, Font.Height*2), Color.Black, BackColor, TextFormatFlags.NoPadding );
			TextRenderer.DrawText( g, text, Font, new Point(0, Font.Height*3), Color.Black, BackColor, TextFormatFlags.Default );

			g.DrawLine( Pens.Black, 0, Font.Height, Width, Font.Height );
			g.DrawLine( Pens.Black, 0, Font.Height*2, Width, Font.Height*2 );
			g.DrawLine( Pens.Black, 0, Font.Height*3, Width, Font.Height*3 );
			g.DrawLine( Pens.Black, 0, Font.Height*4, Width, Font.Height*4 );
		};

		MouseEventHandler onMouseDown = delegate( object sender, MouseEventArgs e ) {
			Graphics g = CreateGraphics();
			SizeF textSize;
			int lineY;

			if( e.Y < Font.Height )
			{
				textSize = g.MeasureString( text, Font, e.X, StringFormat.GenericTypographic );
				lineY = 0;
			}
			else if( e.Y < Font.Height*2 )
			{
				textSize = g.MeasureString( text, Font, e.X, StringFormat.GenericDefault );
				lineY = Font.Height;
			}
			else if( e.Y < Font.Height*3 )
			{
				Size clip = new Size( e.X, 10000 );
				textSize = TextRenderer.MeasureText( g, text, Font, clip, TextFormatFlags.NoPadding );
				lineY = Font.Height * 2;
			}
			else
			{
				Size clip = new Size( e.X, 10000 );
				textSize = TextRenderer.MeasureText( g, text, Font, clip, TextFormatFlags.Default );
				lineY = Font.Height * 3;
			}

			g.DrawLine( Pens.Red, e.X, lineY, e.X, lineY+Font.Height );
			g.DrawLine( Pens.Blue, textSize.Width, lineY, textSize.Width, lineY+Font.Height );
			
			g.Dispose();
		};
		MouseDown += onMouseDown;
		
		timer.Interval = 200;
		
		timer.Tick += delegate {
			Refresh();
			onMouseDown( this, new MouseEventArgs(MouseButtons.Left, 1, x, Font.Height*0, 0) );
			onMouseDown( this, new MouseEventArgs(MouseButtons.Left, 1, x, Font.Height*1, 0) );
			onMouseDown( this, new MouseEventArgs(MouseButtons.Left, 1, x, Font.Height*2, 0) );
			onMouseDown( this, new MouseEventArgs(MouseButtons.Left, 1, x, Font.Height*3, 0) );
			x++;
			if( ClientSize.Width < x )
				x = 0;
		};

		timer.Start();
	}
}
