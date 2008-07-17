// modified a sample "Draw a line for the ascent, descent, baseline, textOrigin" at java2s.com.
// http://www.java2s.com/Code/CSharp/2D-Graphics/DrawalinefortheascentdescentbaselinetextOrigin.htm
using System;
using System.Drawing;
using System.Windows.Forms;

public class Form1 : Form
{
	static void Main()
	{
		Application.Run( new Form1() );
	}

	public Form1()
	{
		Text = "FontMetricsTest";
		Font = new Font( "Lucida Sans Unicode", 24, FontStyle.Regular );
		BackColor = Color.White;
		Paint += Form1_Paint;
	}
	
	void Form1_Paint( object sender, PaintEventArgs e )
	{
		Graphics g = e.Graphics;
		FontFamily ff = Font.FontFamily;
		float emSizeInGU = Font.Size;
		PointF pos = new PointF( 0, 10 );
		int width = ClientSize.Width;

		// Get the design unit metrics from the font family
		int emSizeInDU = ff.GetEmHeight( FontStyle.Regular );
		int ascentInDU = ff.GetCellAscent( FontStyle.Regular );
		int descentInDU = ff.GetCellDescent( FontStyle.Regular );
		int lineSpacingInDU = ff.GetLineSpacing( FontStyle.Regular );

		// Calculate the GraphicsUnit metrics from the font
		float ascentInGU = ascentInDU * (emSizeInGU / emSizeInDU);
		float descentInGU = descentInDU * (emSizeInGU / emSizeInDU);
		float lineSpacingInGU = lineSpacingInDU * (emSizeInGU / emSizeInDU);

		// Draw two lines of the text string
		g.DrawString( "www.Java2s.com", Font, Brushes.Black, pos );

		// Draw a line at the pos
		g.DrawLine( Pens.Blue, pos, new PointF( pos.X + width, pos.Y ) );

		// Draw a line at the top of the ascent
		g.DrawLine( Pens.Gold,
				pos.X,
				pos.Y + lineSpacingInGU - ascentInGU,
				pos.X + width,
				pos.Y + lineSpacingInGU - ascentInGU
			);

		// Draw a line at the baseline
		g.DrawLine( Pens.Red,
				pos.X,
				pos.Y + lineSpacingInGU,
				pos.X + width,
				pos.Y + lineSpacingInGU
			);

		// Draw a line at the bottom of the descent
		g.DrawLine( Pens.Maroon,
			pos.X,
			pos.Y + lineSpacingInGU + descentInGU,
			pos.X + width,
			pos.Y + lineSpacingInGU + descentInGU
		);
	}
}
