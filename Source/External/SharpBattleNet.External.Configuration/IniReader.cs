#region Copyright
//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 
#endregion

using System;
using System.IO;
using System.Text;
using System.Collections;

namespace SharpBattleNet.External.Configuration.Source.INI
{
	#region IniReadState enumeration
	/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/docs/*' />
	public enum INIReadState : int
	{
		/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/Value[@name="Closed"]/docs/*' />
		Closed,
		/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/Value[@name="EndOfFile"]/docs/*' />
		EndOfFile,
		/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/Value[@name="Error"]/docs/*' />
		Error,
		/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/Value[@name="Initial"]/docs/*' />
		Initial,
		/// <include file='IniReader.xml' path='//Enum[@name="IniReadState"]/Value[@name="Interactive"]/docs/*' />
		Interactive
	};
	#endregion

	#region IniType enumeration
	/// <include file='IniReader.xml' path='//Enum[@name="IniType"]/docs/*' />
	public enum INIType : int
	{
		/// <include file='IniReader.xml' path='//Enum[@name="IniType"]/Value[@name="Section"]/docs/*' />
		Section,
		/// <include file='IniReader.xml' path='//Enum[@name="IniType"]/Value[@name="Key"]/docs/*' />
		Key,
		/// <include file='IniReader.xml' path='//Enum[@name="IniType"]/Value[@name="Empty"]/docs/*' />
		Empty
	}
	#endregion

	/// <include file='IniReader.xml' path='//Class[@name="IniReader"]/docs/*' />
	public class INIReader : IDisposable
	{
		#region Private variables
        private int _lineNumber = 1;
        private int _column = 1;
        private INIType _iniType = INIType.Empty;
        private TextReader _textReader = null;
        private bool _ignoreComments = false;
        private StringBuilder _name = new StringBuilder();
        private StringBuilder _value = new StringBuilder();
        private StringBuilder _comment = new StringBuilder();
        private INIReadState _readState = INIReadState.Initial;
        private bool _hasComment = false;
        private bool _disposed = false;
        private bool _lineContinuation = false;
        private bool _acceptCommentAfterKey = true;
        private bool _acceptNoAssignmentOperator = false;
        private bool _consumeAllKeyText = false;
        private char[] _commentDelimiters = new char[] { ';' };
		private char[] _assignDelimeters = new char[] { '=' };
		#endregion

		#region Public properties
		/// <include file='IniReader.xml' path='//Property[@name="Name"]/docs/*' />
		public string Name
		{
			get { return this._name.ToString (); }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="Value"]/docs/*' />
		public string Value
		{
			get { return this._value.ToString (); }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="Type"]/docs/*' />
		public INIType Type
		{
			get { return _iniType; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="Comment"]/docs/*' />
		public string Comment
		{
			get { return (_hasComment) ? this._comment.ToString () : null; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="LineNumber"]/docs/*' />
		public int LineNumber
		{
			get { return _lineNumber; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="LinePosition"]/docs/*' />
		public int LinePosition
		{
			get { return _column; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="IgnoreComments"]/docs/*' />
		public bool IgnoreComments
		{
			get { return _ignoreComments; }
			set { _ignoreComments = value; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="ReadState"]/docs/*' />
		public INIReadState ReadState
		{
			get { return _readState; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="LineContinuation"]/docs/*' />
		public bool LineContinuation
		{
			get { return _lineContinuation; }
			set { _lineContinuation = value; }
		}
		
		/// <include file='IniReader.xml' path='//Property[@name="AcceptCommentAfterKey"]/docs/*' />
		public bool AcceptCommentAfterKey
		{
			get { return _acceptCommentAfterKey; }
			set { _acceptCommentAfterKey = value; }
		}

		/// <include file='IniReader.xml' path='//Property[@name="AcceptNoAssignmentOperator"]/docs/*' />
		public bool AcceptNoAssignmentOperator
		{
			get { return _acceptNoAssignmentOperator; }
			set { _acceptNoAssignmentOperator = value; }
		}

		/// <include file='IniReader.xml' path='//Property[@name="ConsumeAllKeyText"]/docs/*' />
		public bool ConsumeAllKeyText
		{
			get { return _consumeAllKeyText; }
			set { _consumeAllKeyText = value; }
		}
		#endregion
		
		#region Constructors
		/// <include file='IniReader.xml' path='//Constructor[@name="ConstructorPath"]/docs/*' />
		public INIReader (string filePath)
		{
			_textReader = new StreamReader (filePath);
		}
		
		/// <include file='IniReader.xml' path='//Constructor[@name="ConstructorTextReader"]/docs/*' />
		public INIReader (TextReader reader)
		{
			_textReader = reader;
		}
		
		/// <include file='IniReader.xml' path='//Constructor[@name="ConstructorStream"]/docs/*' />
		public INIReader (Stream stream)
			: this (new StreamReader (stream))
		{
		}
		#endregion
		
		#region Public methods
		/// <include file='IniReader.xml' path='//Method[@name="Read"]/docs/*' />
		public bool Read ()
		{
			bool result = false;
			
			if (_readState != INIReadState.EndOfFile 
				|| _readState != INIReadState.Closed) {
				_readState = INIReadState.Interactive;
				result = ReadNext ();
			}
			
			return result;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="MoveToNextSection"]/docs/*' />
		public bool MoveToNextSection ()
		{
			bool result = false;
			
			while (true)
			{
				result = Read ();

				if (_iniType == INIType.Section || !result) {
					break;
				}
			}
			
			return result;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="MoveToNextKey"]/docs/*' />
		public bool MoveToNextKey ()
		{
			bool result = false;
			
			while (true)
			{
				result = Read ();

				if (_iniType == INIType.Section) {
					result = false;
					break;
				}
				if (_iniType == INIType.Key || !result) {
					break;
				}
			}
			
			return result;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="Close"]/docs/*' />
		public void Close ()
		{
			Reset ();
			_readState = INIReadState.Closed;
			
			if (_textReader != null) {
				_textReader.Close ();
			}
		}

		/// <include file='IniReader.xml' path='//Method[@name="Dispose"]/docs/*' />
		public void Dispose ()
		{
			Dispose (true);
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="GetCommentDelimiters"]/docs/*' />
		public char[] GetCommentDelimiters ()
		{
			char[] result = new char[_commentDelimiters.Length];
			Array.Copy (_commentDelimiters, 0, result, 0, _commentDelimiters.Length);

			return result;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="SetCommentDelimiters"]/docs/*' />
		public void SetCommentDelimiters (char[] delimiters)
		{
			if (delimiters.Length < 1) {
				throw new ArgumentException ("Must supply at least one delimiter");
			}
			
			_commentDelimiters = delimiters;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="GetAssignDelimiters"]/docs/*' />
		public char[] GetAssignDelimiters ()
		{
			char[] result = new char[_assignDelimeters.Length];
			Array.Copy (_assignDelimeters, 0, result, 0, _assignDelimeters.Length);

			return result;
		}
		
		/// <include file='IniReader.xml' path='//Method[@name="SetAssignDelimiters"]/docs/*' />
		public void SetAssignDelimiters (char[] delimiters)
		{
			if (delimiters.Length < 1) {
				throw new ArgumentException ("Must supply at least one delimiter");
			}
			
			_assignDelimeters = delimiters;
		}
		#endregion
		
		#region Protected methods
		/// <include file='IniReader.xml' path='//Method[@name="DisposeBoolean"]/docs/*' />
		protected virtual void Dispose (bool disposing)
		{
			if (!_disposed) {
				_textReader.Close ();
				_disposed = true;

				if (disposing) {
					GC.SuppressFinalize (this);
				}
			}
		}
		#endregion
		
		#region Private methods
		/// <summary>
		/// Destructor.
		/// </summary>
		~INIReader ()
		{
			Dispose (false);
		}

		/// <summary>
		/// Resets all of the current INI line data.
		/// </summary>
		private void Reset ()
		{
			this._name.Remove (0, this._name.Length);
			this._value.Remove (0, this._value.Length);
			this._comment.Remove (0, this._comment.Length);
			_iniType = INIType.Empty;
			_hasComment = false;
		}
		
		/// <summary>
		/// Reads the next INI line item.
		/// </summary>
		private bool ReadNext ()
		{
			bool result = true;
			int ch = PeekCharacter ();
			Reset ();
			
			if (IsComment (ch)) {
				_iniType = INIType.Empty;
				ReadCharacter (); // consume comment character
				ReadComment ();

				return result;
			}

			switch (ch)
			{
				case ' ':
				case '\t':
				case '\r':
					SkipWhitespace ();
					ReadNext ();
					break;
				case '\n':
					ReadCharacter ();
					break;
				case '[':
					ReadSection ();
					break;
				case -1:
					_readState = INIReadState.EndOfFile;
					result = false;
					break;
				default:
					ReadKey ();
					break;
			}
			
			return result;
		}
		
		/// <summary>
		/// Reads a comment. Must start after the comment delimiter.
		/// </summary>
		private void ReadComment  ()
		{
			int ch = -1;
			SkipWhitespace ();
			_hasComment = true;

			do
			{
				ch = ReadCharacter ();
				this._comment.Append ((char)ch);
			} while (!EndOfLine (ch));
			
			RemoveTrailingWhitespace (this._comment);
		}
		
		/// <summary>
		/// Removes trailing whitespace from a StringBuilder.
		/// </summary>
		private void RemoveTrailingWhitespace (StringBuilder builder)
		{
			string temp = builder.ToString ();
		
			builder.Remove (0, builder.Length);
			builder.Append (temp.TrimEnd (null));
		}
		
		/// <summary>
		/// Reads a key.
		/// </summary>
		private void ReadKey ()
		{
			int ch = -1;
			_iniType = INIType.Key;
			
			while (true)
			{
				ch = PeekCharacter ();

				if (IsAssign (ch)) {
					ReadCharacter ();
					break;
				}
				
				if (EndOfLine (ch)) {
					if (_acceptNoAssignmentOperator) {
						break;
					}
					throw new INIException (this, 
						String.Format ("Expected assignment operator ({0})", 
										_assignDelimeters[0]));
				}

				this._name.Append ((char)ReadCharacter ());
			}
			
			ReadKeyValue ();
			SearchForComment ();
			RemoveTrailingWhitespace (this._name);
		}
		
		/// <summary>
		/// Reads the value of a key.
		/// </summary>
		private void ReadKeyValue ()
		{
			int ch = -1;
			bool foundQuote = false;
			int characters = 0;
			SkipWhitespace ();

			while (true)
			{
				ch = PeekCharacter ();

				if (!IsWhitespace (ch)) {
					characters++;
				}
				
				if (!this.ConsumeAllKeyText && ch == '"') {
					ReadCharacter ();

					if (!foundQuote && characters == 1) {				
						foundQuote = true;
						continue;
					} else {
						break;
					}
				}
				
				if (foundQuote && EndOfLine (ch)) {
					throw new INIException (this, "Expected closing quote (\")");
				}
				
				// Handle line continuation
				if (_lineContinuation && ch == '\\') 
				{
					StringBuilder buffer = new StringBuilder ();
					buffer.Append ((char)ReadCharacter ()); // append '\'
					
					while (PeekCharacter () != '\n' && IsWhitespace (PeekCharacter ()))
					{
						if (PeekCharacter () != '\r') {
							buffer.Append ((char)ReadCharacter ());
						} else {
							ReadCharacter (); // consume '\r'
						}
					}
					
					if (PeekCharacter () == '\n') {
						// continue reading key value on next line
						ReadCharacter ();
						continue;
					} else {
						// Replace consumed characters
						this._value.Append (buffer.ToString ());
					}
				}

				if (!this.ConsumeAllKeyText) {
					// If accepting comments then don't consume as key value
					if (_acceptCommentAfterKey && IsComment (ch) && !foundQuote) {
						break;
					}
				}

				// Always break at end of line
				if (EndOfLine (ch)) {
					break;
				}

				this._value.Append ((char)ReadCharacter ());
			}
			
			if (!foundQuote) {
				RemoveTrailingWhitespace (this._value);
			}
		}
		
		/// <summary>
		/// Reads an INI section.
		/// </summary>
		private void ReadSection ()
		{
			int ch = -1;
			_iniType = INIType.Section;
			ch = ReadCharacter (); // consume "["

			while (true)
			{
				ch = PeekCharacter ();
				if (ch == ']') {
					break;
				}
				if (EndOfLine (ch)) {
					throw new INIException (this, "Expected section end (])");
				}

				this._name.Append ((char)ReadCharacter ());
			}

			ConsumeToEnd (); // all after '[' is garbage			
			RemoveTrailingWhitespace (this._name);
		}
		
		/// <summary>
		/// Looks for a comment.
		/// </summary>
		private void SearchForComment ()
		{
			int ch = ReadCharacter ();
			
			while (!EndOfLine (ch))
			{
				if (IsComment (ch)) {
					if (_ignoreComments) {
						ConsumeToEnd ();
					} else {
						ReadComment ();
					}
					break;
				}
				ch = ReadCharacter ();
			}
		}

		/// <summary>
		/// Consumes all data until the end of a line. 
		/// </summary>		
		private void ConsumeToEnd ()
		{
			int ch = -1;

			do
			{
				ch = ReadCharacter ();
			} while (!EndOfLine (ch));
		}
		
		/// <summary>
		/// Returns and consumes the next character from the stream.
		/// </summary>
		private int ReadCharacter ()
		{
			int result = _textReader.Read ();
			
			if (result == '\n') {
				_lineNumber++;
				_column = 1;
			} else {
				_column++;
			}
			
			return result;
		}
		
		/// <summary>
		/// Returns the next upcoming character from the stream.
		/// </summary>
		private int PeekCharacter ()
		{
			return _textReader.Peek ();
		}
		
		/// <summary>
		/// Returns true if a comment character is found.
		/// </summary>
		private bool IsComment (int ch)
		{
			return HasCharacter (_commentDelimiters, ch);
		}
		
		/// <summary>
		/// Returns true if character is an assign character.
		/// </summary>
		private bool IsAssign (int ch)
		{
			return HasCharacter (_assignDelimeters, ch);
		}

		/// <summary>
		/// Returns true if the character is found in the given array.
		/// </summary>
		private bool HasCharacter (char[] characters, int ch)
		{
			bool result = false;
			
			for (int i = 0; i < characters.Length; i++)
			{
				if (ch == characters[i]) 
				{
					result = true;
					break;
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// Returns true if a value is whitespace.
		/// </summary>
		private bool IsWhitespace (int ch)
		{
			return ch == 0x20 || ch == 0x9 || ch == 0xD || ch == 0xA;
		}
		
		/// <summary>
		/// Skips all whitespace.
		/// </summary>
		private void SkipWhitespace ()
		{
			while (IsWhitespace (PeekCharacter ()))
			{
				if (EndOfLine (PeekCharacter ())) {
					break;
				}

				ReadCharacter ();
			}
		}

		/// <summary>
		/// Returns true if an end of line is found.  End of line
		/// includes both an end of line or end of file.
		/// </summary>
		private bool EndOfLine (int ch)
		{
			return (ch == '\n' || ch == -1);
		}
		#endregion
	}
}