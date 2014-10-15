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

namespace SharpBattleNet.External.Configuration.Source.INI
{
	#region IniWriteState enumeration
	/// <include file='IniWriter.xml' path='//Enum[@name="IniWriteState"]/docs/*' />
	public enum INIWriterState : int
	{
		/// <include file='IniWriter.xml' path='//Enum[@name="IniWriteState"]/Value[@name="Start"]/docs/*' />
		Start,
		/// <include file='IniWriter.xml' path='//Enum[@name="IniWriteState"]/Value[@name="BeforeFirstSection"]/docs/*' />
		BeforeFirstSection,
		/// <include file='IniWriter.xml' path='//Enum[@name="IniWriteState"]/Value[@name="Section"]/docs/*' />
		Section,
		/// <include file='IniWriter.xml' path='//Enum[@name="IniWriteState"]/Value[@name="Closed"]/docs/*' />
		Closed
	};
	#endregion

	/// <include file='IniWriter.xml' path='//Class[@name="IniWriter"]/docs/*' />
	public class INIWriter : IDisposable
	{
		#region Private variables
        private int _indentation = 0;
        private bool _useValueQuotes = false;
        private INIWriterState _writeState = INIWriterState.Start;
        private char _commentDelimiter = ';';
        private char _assignDelimiter = '=';
        private TextWriter _textWriter = null;
        private string _endOfLine = "\r\n";
        private StringBuilder _indentationBuilder = new StringBuilder();
        private Stream _baseStream = null;
		private bool _disposed = false;
		#endregion
		
		#region Public properties
		/// <include file='IniWriter.xml' path='//Property[@name="Indentation"]/docs/*' />
		public int Indentation
		{
			get { return _indentation; }
			set
			{
				if (value < 0)
					throw new ArgumentException ("Negative values are illegal");
				
				_indentation = value;
				_indentationBuilder.Remove(0, _indentationBuilder.Length);
				for (int i = 0; i < value; i++)
					_indentationBuilder.Append (' ');
			}
		}

		/// <include file='IniWriter.xml' path='//Property[@name="UseValueQuotes"]/docs/*' />
		public bool UseValueQuotes
		{
			get { return _useValueQuotes; }
			set { _useValueQuotes = value; }
		}

		/// <include file='IniWriter.xml' path='//Property[@name="WriteState"]/docs/*' />
		public INIWriterState WriteState
		{
			get { return _writeState; }
		}

		/// <include file='IniWriter.xml' path='//Property[@name="CommentDelimiter"]/docs/*' />
		public char CommentDelimiter
		{
			get { return _commentDelimiter; }
			set { _commentDelimiter = value; }
		}
		
		/// <include file='IniWriter.xml' path='//Property[@name="AssignDelimiter"]/docs/*' />
		public char AssignDelimiter
		{
			get { return _assignDelimiter; }
			set { _assignDelimiter = value; }
		}
		
		/// <include file='IniWriter.xml' path='//Property[@name="BaseStream"]/docs/*' />
		public Stream BaseStream
		{
			get { return _baseStream; }
		}
		#endregion
		
		#region Constructors
		/// <include file='IniWriter.xml' path='//Constructor[@name="ConstructorPath"]/docs/*' />
		public INIWriter(string filePath)
			: this (new FileStream (filePath, FileMode.Create, FileAccess.Write, FileShare.None))
		{
		}
		
		/// <include file='IniWriter.xml' path='//Constructor[@name="ConstructorTextWriter"]/docs/*' />
		public INIWriter (TextWriter writer)
		{
			_textWriter = writer;
			StreamWriter streamWriter = writer as StreamWriter;
			if (streamWriter != null) {
				_baseStream = streamWriter.BaseStream;
			}
		}
		
		/// <include file='IniWriter.xml' path='//Constructor[@name="ConstructorStream"]/docs/*' />
		public INIWriter (Stream stream)
			: this (new StreamWriter (stream))
		{
		}
		#endregion
		
		#region Public methods
		/// <include file='IniWriter.xml' path='//Method[@name="Close"]/docs/*' />
		public void Close ()
		{
			_textWriter.Close ();
			_writeState = INIWriterState.Closed;
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="Flush"]/docs/*' />
		public void Flush ()
		{
			_textWriter.Flush ();
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="ToString"]/docs/*' />
		public override string ToString ()
		{
			return _textWriter.ToString ();
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="WriteSection"]/docs/*' />
		public void WriteSection (string section)
		{
			ValidateState ();
			_writeState = INIWriterState.Section;
			WriteLine ("[" + section + "]");
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="WriteSectionComment"]/docs/*' />
		public void WriteSection (string section, string comment)
		{
			ValidateState ();
			_writeState = INIWriterState.Section;
			WriteLine ("[" + section + "]" + Comment(comment));
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="WriteKey"]/docs/*' />
		public void WriteKey (string key, string value)
		{
			ValidateStateKey ();
			WriteLine (key + " " + _assignDelimiter + " " + GetKeyValue (value));
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="WriteKeyComment"]/docs/*' />
		public void WriteKey (string key, string value, string comment)
		{
			ValidateStateKey ();
			WriteLine (key + " " + _assignDelimiter + " " + GetKeyValue (value) + Comment (comment));
		}
	
		/// <include file='IniWriter.xml' path='//Method[@name="WriteEmpty"]/docs/*' />
		public void WriteEmpty ()
		{
			ValidateState ();
			if (_writeState == INIWriterState.Start) {
				_writeState = INIWriterState.BeforeFirstSection;
			}
			WriteLine ("");
		}

		/// <include file='IniWriter.xml' path='//Method[@name="WriteEmptyComment"]/docs/*' />
		public void WriteEmpty (string comment)
		{
			ValidateState ();
			if (_writeState == INIWriterState.Start) {
				_writeState = INIWriterState.BeforeFirstSection;
			}
			if (comment == null) {
				WriteLine ("");
			} else {
				WriteLine (_commentDelimiter + " " + comment);
			}
		}
		
		/// <include file='IniWriter.xml' path='//Method[@name="Dispose"]/docs/*' />
		public void Dispose ()
		{
			Dispose (true);
		}
		#endregion
		
		#region Protected methods
		/// <include file='IniWriter.xml' path='//Method[@name="DisposeBoolean"]/docs/*' />
		protected virtual void Dispose (bool disposing)
		{
			if (!_disposed) 
			{
				_textWriter.Close ();
				_baseStream.Close ();
				_disposed = true;

				if (disposing) 
				{
					GC.SuppressFinalize (this);
				}
			}
		}
		#endregion
		
		#region Private methods
		/// <summary>
		/// Destructor.
		/// </summary>
		~INIWriter ()
		{
			Dispose (false);
		}

		/// <summary>
		/// Returns the value of a key.
		/// </summary>
		private string GetKeyValue (string text)
		{
			string result;

			if (_useValueQuotes) {
				result = MassageValue ('"' + text + '"');
			} else {
				result = MassageValue (text);
			}
			
			return result;
		}
		
		/// <summary>
		/// Validates whether a key can be written.
		/// </summary>
		private void ValidateStateKey ()
		{
			ValidateState ();

			switch (_writeState)
			{
			case INIWriterState.BeforeFirstSection:
			case INIWriterState.Start:
				throw  new InvalidOperationException ("The WriteState is not Section");
			case INIWriterState.Closed:
				throw  new InvalidOperationException ("The writer is closed");
			}
		}
		
		/// <summary>
		/// Validates the state to determine if the item can be written.
		/// </summary>
		private void ValidateState ()
		{
			if (_writeState == INIWriterState.Closed) {
				throw  new InvalidOperationException ("The writer is closed");
			}
		}
		
		/// <summary>
		/// Returns a formatted comment.
		/// </summary>
		private string Comment (string text)
		{
			return (text == null) ? "" : (" " + _commentDelimiter + " " + text);
		}
		
		/// <summary>
		/// Writes data to the writer.
		/// </summary>
		private void Write (string value)
		{
			_textWriter.Write (_indentationBuilder.ToString () + value);
		}
		
		/// <summary>
		/// Writes a full line to the writer.
		/// </summary>
		private void WriteLine (string value)
		{
			Write (value + _endOfLine);
		}

		/// <summary>
		/// Fixes the incoming value to prevent illegal characters from 
		/// hurting the integrity of the INI file.
		/// </summary>
		private string MassageValue (string text)
		{
			return text.Replace ("\n", "");
		}
		#endregion
	}
}