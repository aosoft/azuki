// file: Actions.Selection.cs
// brief: Actions for Azuki engine (actions to change selection).
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2008-07-05
//=========================================================
using System;
using System.Drawing;

namespace Sgry.Azuki
{
	/// <summary>
	/// Actions.
	/// </summary>
	public static partial class Actions
	{
		#region Caret Movement
		/// <summary>
		/// Move caret to right.
		/// </summary>
		public static void MoveRight( View view )
		{
			int selBegin, selEnd;

			// if there are something selected,
			// release selection and set caret at where the selection ends
			view.Document.GetSelection( out selBegin, out selEnd );
			if( selEnd != selBegin )
			{
				view.Document.SetSelection( selEnd, selEnd );
			}
			// otherwise, move caret right
			else
			{
				CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_Right, view );
			}

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to left.
		/// </summary>
		public static void MoveLeft( View view )
		{
			int selBegin, selEnd;

			// if there are something selected,
			// release selection and set caret at where the selection starts
			view.Document.GetSelection( out selBegin, out selEnd );
			if( selEnd != selBegin )
			{
				view.Document.SetSelection( selBegin, selBegin );
			}
			// otherwise, move caret left
			else
			{
				CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_Left, view );
			}

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret down.
		/// </summary>
		public static void MoveDown( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_Down, view );
		}

		/// <summary>
		/// Move caret up.
		/// </summary>
		public static void MoveUp( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_Up, view );
		}

		/// <summary>
		/// Move caret to next word.
		/// </summary>
		public static void MoveToNextWord( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_NextWord, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to previous word.
		/// </summary>
		public static void MoveToPrevWord( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_PrevWord, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to line head.
		/// </summary>
		public static void MoveToLineHead( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_LineHead, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to line end.
		/// </summary>
		public static void MoveToLineEnd( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_LineEnd, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to one page after.
		/// </summary>
		public static void MovePageDown( View view )
		{
			Document doc = view.Document;
			Point pt;
			int nextIndex;
			int diff = (view.VisibleSize.Height / view.LineSpacing);

			// get current virtual coordinate of the caret
			pt = view.GetVirPosFromIndex( doc.CaretIndex );
			
			// calc new virtual coordinate of the caret
			pt.Y += diff * view.LineSpacing;
			/*NOT_NEEDED
			if( view.VisibleSize.Height < pt.Y )
			{
				pt.Y = view.VisibleSize.Height;
			}*/

			// calc index from the coord
			nextIndex = view.GetIndexFromVirPos( pt );

			// move caret and scroll
			doc.SetSelection( nextIndex, nextIndex );
			view.Scroll( diff );
		}

		/// <summary>
		/// Move caret to one page before.
		/// </summary>
		public static void MovePageUp( View view )
		{
			Document doc = view.Document;
			Point pt;
			int nextIndex;
			int diff = (view.VisibleSize.Height / view.LineSpacing);

			// get current virtual coordinate of the caret
			pt = view.GetVirPosFromIndex( doc.CaretIndex );
			
			// calc new virtual coordinate of the caret
			pt.Y -= diff * view.LineSpacing;
			if( pt.Y < 0 )
			{
				pt.Y = 0;
			}

			// calc index from the coord
			nextIndex = view.GetIndexFromVirPos( pt );

			// move caret and scroll
			doc.SetSelection( nextIndex, nextIndex );
			view.Scroll( -diff );
		}

		/// <summary>
		/// Move caret to file head.
		/// </summary>
		public static void MoveToFileHead( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_FileHead, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Move caret to file end.
		/// </summary>
		public static void MoveToFileEnd( View view )
		{
			// move caret
			CaretMoveLogic.MoveCaret( CaretMoveLogic.Calc_FileEnd, view );

			// update desired column
			view.SetDesiredColumn();
		}
		#endregion

		#region Selection
		/// <summary>
		/// Expand selection to right.
		/// </summary>
		public static void SelectToRight( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_Right, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to left.
		/// </summary>
		public static void SelectToLeft( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_Left, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection down.
		/// </summary>
		public static void SelectToDown( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_Down, view );
		}

		/// <summary>
		/// Expand selection up.
		/// </summary>
		public static void SelectToUp( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_Up, view );
		}

		/// <summary>
		/// Expand selection to next word begin.
		/// </summary>
		public static void SelectToNextWord( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_NextWord, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to previous word begin.
		/// </summary>
		public static void SelectToPrevWord( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_PrevWord, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to line head.
		/// </summary>
		public static void SelectToLineHead( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_LineHead, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to line end.
		/// </summary>
		public static void SelectToLineEnd( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_LineEnd, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to one page after.
		/// </summary>
		public static void SelectToPageDown( View view )
		{
			Document doc = view.Document;
			Point pt;
			int nextIndex;
			int diff = (view.VisibleSize.Height / view.LineSpacing);

			// get current virtual coordinate of the caret
			pt = view.GetVirPosFromIndex( doc.CaretIndex );
			
			// calc new virtual coordinate of the caret
			pt.Y += diff * view.LineSpacing;
			/*NOT_NEEDED
			if( view.VisibleSize.Height < pt.Y )
			{
				pt.Y = view.VisibleSize.Height;
			}*/

			// calc index from the coord
			nextIndex = view.GetIndexFromVirPos( pt );

			// move caret and scroll
			doc.SetSelection( doc.AnchorIndex, nextIndex );
			view.Scroll( diff );
		}

		/// <summary>
		/// Expand selection to one page before.
		/// </summary>
		public static void SelectToPageUp( View view )
		{
			Document doc = view.Document;
			Point pt;
			int nextIndex;
			int diff = (view.VisibleSize.Height / view.LineSpacing);

			// get current virtual coordinate of the caret
			pt = view.GetVirPosFromIndex( doc.CaretIndex );
			
			// calc new virtual coordinate of the caret
			pt.Y -= diff * view.LineSpacing;
			if( pt.Y < 0 )
			{
				pt.Y = 0;
			}

			// calc index from the coord
			nextIndex = view.GetIndexFromVirPos( pt );

			// move caret and scroll
			doc.SetSelection( doc.AnchorIndex, nextIndex );
			view.Scroll( -diff );
		}

		/// <summary>
		/// Expand selection to file head.
		/// </summary>
		public static void SelectToFileHead( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_FileHead, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Expand selection to file end.
		/// </summary>
		public static void SelectToFileEnd( View view )
		{
			// change selection
			CaretMoveLogic.SelectTo( CaretMoveLogic.Calc_FileEnd, view );

			// update desired column
			view.SetDesiredColumn();
		}

		/// <summary>
		/// Select all text.
		/// </summary>
		public static void SelectAll( View view )
		{
			Document doc = view.Document;
			
			// set parameters
			doc.SetSelection( 0, doc.Length );

			// update desired column
			view.SetDesiredColumn();
			view.ScrollToCaret();

			view.Invalidate(); // this is needed because Azuki's invalidation logic only supports selection change by caret movement
		}
		#endregion
	}
}
