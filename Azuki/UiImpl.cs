// file: UiImpl.cs
// brief: Implementation of user interface logic
// author: YAMAMOTO Suguru
// update: 2008-07-27
//=========================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Sgry.Azuki
{
	class UiImpl : IDisposable
	{
		#region Fields
		IUserInterface _UI;
		View _View = null;
		ViewType _ViewType = ViewType.Propotional;

		IDictionary< int, ActionProc > _KeyMap = new Dictionary< int, ActionProc >( 32 );
		AutoIndentHook _AutoIndentHook = null;
		bool _IsOverwriteMode = false;
		bool _ConvertsTabToSpaces = false;
		bool _ConvertsFullWidthSpaceToSpace = false;

		Point _MouseDownPos = new Point( -1, 0 ); // this X coordinate also be used as a flag to determine whether the mouse button is down or not
		bool _MouseDragging = false;

		Thread _HighlighterThread;
		bool _ShouldBeHighlighted = false;
		int _DirtyRangeBegin = -1;
		int _DirtyRangeEnd = -1;
		#endregion

		#region Init / Dispose
		public UiImpl( IUserInterface ui )
		{
			_UI = ui;
			_View = new PropView( ui );

			_HighlighterThread = new Thread( HighlighterThreadProc );
			_HighlighterThread.Start();
		}

		public void Dispose()
		{
			_HighlighterThread.Abort();

			// uninstall document event handlers
			Document.SelectionChanged -= Doc_SelectionChanged;
			Document.ContentChanged -= Doc_ContentChanged;

			// dispose view
			View.Dispose();
		}
		#endregion

		#region View and Document
		public Document Document
		{
			get{ return View.Document; }
			set
			{
				if( value == null )
					throw new ArgumentNullException();

				// uninstall event handlers
				if( View.Document != null
					&& View.Document != value )
				{
					View.Document.SelectionChanged -= Doc_SelectionChanged;
					View.Document.ContentChanged -= Doc_ContentChanged;
				}

				// replace document
				View.Document = value;

				// install event handlers
				View.Document.SelectionChanged += Doc_SelectionChanged;
				View.Document.ContentChanged += Doc_ContentChanged;

				// redraw graphic
				_UI.Invalidate();
				_UI.UpdateCaretGraphic();
			}
		}

		public View View
		{
			get{ return _View; }
			set{ _View = value; }
		}
		
		/// <summary>
		/// Gets or sets type of the view.
		/// View type determine how to render text content.
		/// </summary>
		public ViewType ViewType
		{
			get{ return _ViewType; }
			set
			{
				View oldView = View;

				// switch to new view object
				switch( value )
				{
					case ViewType.WrappedPropotional:
						View = new PropWrapView( View );
						break;
					//case ViewType.Propotional:
					default:
						View = new PropView( View );
						break;
				}
				_ViewType = value;

				// dispose using view object
				oldView.Dispose();

				// re-install event handlers
				// (AzukiControl's event handler MUST be called AFTER view's one)
				if( Document != null )
				{
					Document.ContentChanged -= Doc_ContentChanged;
					Document.ContentChanged += Doc_ContentChanged;
					Document.SelectionChanged -= Doc_SelectionChanged;
					Document.SelectionChanged += Doc_SelectionChanged;
				}

				// refresh GUI
				_UI.Invalidate();
				if( Document != null )
				{
					_UI.UpdateCaretGraphic();
					_UI.UpdateScrollBarRange();
				}
			}
		}
		#endregion

		#region Behavior
		/// <summary>
		/// Gets or sets whether the input character overwrites the character at where the caret is on.
		/// </summary>
		public bool IsOverwriteMode
		{
			get{ return _IsOverwriteMode; }
			set
			{
				_IsOverwriteMode = value;
				_UI.UpdateCaretGraphic();
			}
		}

		/// <summary>
		/// Gets or sets whether to automatically convert
		/// an input tab character to equivalent amount of spaces.
		/// </summary>
		public bool ConvertsTabToSpaces
		{
			get{ return _ConvertsTabToSpaces; }
			set{ _ConvertsTabToSpaces = value; }
		}

		/// <summary>
		/// Gets or sets whether to automatically convert
		/// an input full-width space to a space.
		/// </summary>
		public bool ConvertsFullWidthSpaceToSpace
		{
			get{ return _ConvertsFullWidthSpaceToSpace; }
			set{ _ConvertsFullWidthSpaceToSpace = value; }
		}

		/// <summary>
		/// Gets or sets hook delegate to execute auto-indentation.
		/// If null, auto-indentation will not be performed.
		/// </summary>
		/// <seealso cref="AutoIndentLogic"/>
		public AutoIndentHook AutoIndentHook
		{
			get{ return _AutoIndentHook; }
			set{ _AutoIndentHook = value; }
		}
		#endregion

		#region Key Handling
		public void SetKeyBind( int keyCode, ActionProc action )
		{
			// remove specified key code from dictionary anyway
			_KeyMap.Remove( keyCode );

			// if it's not null, regist the action
			if( action != null )
			{
				_KeyMap.Add( keyCode, action );
			}
		}

		internal bool IsKeyBindDefined( int keyCode )
		{
			return _KeyMap.ContainsKey( keyCode );
		}

		public void ClearKeyBind()
		{
			_KeyMap.Clear();
		}

		/// <summary>
		/// Handles translated character input event.
		/// </summary>
		internal void HandleKeyPress( char ch )
		{
			string str = null;
			int newCaretIndex;
			Document doc = Document;
			int selBegin, selEnd;

			// just notify and return if in read only mode
			if( Document.IsReadOnly )
			{
				Plat.Inst.MessageBeep();
				return;
			}

			// try to use hook delegate
			if( _AutoIndentHook != null
				&& _AutoIndentHook(Document, ch) == true )
			{
				goto update;
			}

			// make string to be inserted
			doc.GetSelection( out selBegin, out selEnd );
			if( LineLogic.IsEolChar(ch) )
			{
				str = doc.EolCode;
			}
			else if( ch == '\t' && _ConvertsTabToSpaces )
			{
				int spaceCount = NextTabStop( selBegin ) - selBegin;
				str = String.Empty;
				for( int i=0; i<spaceCount; i++ )
				{
					str += ' ';
				}
			}
			else if( ch == '\x3000' && _ConvertsFullWidthSpaceToSpace )
			{
				str = "\x0020";
			}
			else
			{
				str = ch.ToString();
			}
			newCaretIndex = selBegin + str.Length;

			// calc replacement target range
			if( IsOverwriteMode
				&& selBegin == selEnd && selEnd+1 < doc.Length
				&& LineLogic.IsEolChar(doc[selBegin]) != true )
			{
				selEnd++;
			}

			// replace selection to input char
			doc.Replace( str, selBegin, selEnd );
			doc.SetSelection( newCaretIndex, newCaretIndex );

		update:
			// set desired column
			_View.SetDesiredColumn();

			// update graphic
			_View.ScrollToCaret();
			//NO_NEED//_View.Invalidate( xxx ); // Doc_ContentChanged will do invalidation well.
		}
		#endregion

		#region Highlight Thread
		void HighlighterThreadProc()
		{
			int invalidBegin, invalidEnd;
			int dirtyBegin, dirtyEnd;
			Document doc;

			while( true )
			{
				// wait until the flag was set down
				while( _ShouldBeHighlighted == false )
				{
					Thread.Sleep( 500 );
				}
				_ShouldBeHighlighted = false;

				// wait a moment and begin highlight
				Thread.Sleep( 500 );
				if( _ShouldBeHighlighted != false || _UI.Document == null )
				{
					continue; // flag was set up while this thread are sleeping... skip this time.
				}

				doc = _UI.Document;

				// determine where to start highlighting
				dirtyBegin = Math.Max( 0, _DirtyRangeBegin );
				dirtyEnd = Math.Max( doc.Length, _DirtyRangeEnd );

				// highlight and refresh view
				doc.Highlighter.Highlight( doc, dirtyBegin, doc.Length, out invalidBegin, out invalidEnd );
				_UI.Invalidate();

				// prepare for next loop
				_DirtyRangeBegin = -1;
				_DirtyRangeEnd = -1;
			}
		}
		#endregion

		#region UI Event
		public void HandleKeyDown( int keyData )
		{
			try
			{
				ActionProc action = _KeyMap[ keyData ];
				action( _UI );
			}
			catch( KeyNotFoundException )
			{}
		}

		public void HandlePaint( Rectangle clipRect )
		{
			View.OnPaint( clipRect );
		}

		internal void HandleMouseDown( int buttonIndex, Point pos, bool shift, bool ctrl, bool alt, bool win )
		{
			_MouseDownPos = pos;
			View.ScreenToVirtual( ref pos );

			if( buttonIndex == 0 ) // left click
			{
				if( shift )
				{
					int index = View.GetIndexFromVirPos( pos );
					Document.SetSelection( Document.AnchorIndex, index );
				}
				else
				{
					int index = View.GetIndexFromVirPos( pos );
					Document.SetSelection( index, index );
				}
				View.SetDesiredColumn();
				View.ScrollToCaret();
			}
		}
		
		internal void HandleMouseUp( int buttonIndex, Point pos, bool shift, bool ctrl, bool alt, bool win )
		{
			_MouseDownPos.X = -1;
			_MouseDragging = false;
		}

		internal void HandleDoubleClick( int buttonIndex, Point pos, bool shift, bool ctrl, bool alt, bool win )
		{
			int index;
			int begin, end;

			View.ScreenToVirtual( ref pos );

			// get range of a word at clicked location
			index = View.GetIndexFromVirPos( pos );
			WordLogic.GetWordAt( Document.InternalBuffer, index, out begin, out end );
			if( end <= begin )
			{
				return;
			}
			
			// select the word.
			// (because Azuki's invalidation logic only supports
			// selection change by keyboard commands,
			// emulate as if this selection was done by keyboard.
			Document.SetSelection( begin, begin ); // select caret to the head of the word
			Document.SetSelection( begin, end ); // then, expand selection to the end of it
		}

		internal void HandleMouseMove( int buttonIndex, Point pos, bool shift, bool ctrl, bool alt, bool win )
		{
			// if mouse button was not down, ignore
			if( _MouseDownPos.X < 0 )
				return;

			// if the movement is very slightly, ignore
			if( _MouseDragging == false )
			{
				int xOffset = Math.Abs( pos.X - _MouseDownPos.X );
				int yOffset = Math.Abs( pos.Y - _MouseDownPos.Y );
				if( View.DragThresh < xOffset || View.DragThresh < yOffset )
				{
					_MouseDragging = true;
				}
				else
				{
					return;
				}
			}

			// dragging with left button?
			if( buttonIndex == 0 )
			{
				View.ScreenToVirtual( ref pos );

				// calc index of where the mouse pointer is on
				int index = View.GetIndexFromVirPos( pos );
				if( index == -1 || index == Document.CaretIndex )
				{
					return; // failed to get index or same as previous index
				}

				// expand selection to there
				Document.SetSelection( Document.AnchorIndex, index );
				View.SetDesiredColumn();
				View.ScrollToCaret();
			}
		}
		#endregion

		#region Event Handlers
		void Doc_SelectionChanged( object sender, EventArgs e )
		{
			// update caret graphic
			_UI.UpdateCaretGraphic();

			// send event to component users
			((Windows.AzukiControl)_UI).InvokeCaretMoved();
		}

		public void Doc_ContentChanged( object sender, ContentChangedEventArgs e )
		{
			// redraw caret graphic
			_UI.UpdateCaretGraphic();
			
			// update range of scroll bars
			_UI.UpdateScrollBarRange();

			// set flag to start highlighting
			if( _DirtyRangeBegin == -1 || e.Index < _DirtyRangeBegin )
			{
				_DirtyRangeBegin = e.Index;
			}
			if( _DirtyRangeEnd == -1 || _DirtyRangeEnd < e.Index + e.NewText.Length )
			{
				_DirtyRangeEnd = e.Index + e.NewText.Length;
			}
			_ShouldBeHighlighted = true;
		}
		#endregion

		int NextTabStop( int index )
		{
			return ((index / _View.TabWidth) + 1) * _View.TabWidth;
		}
	}
}
