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
using System.Collections;

namespace SharpBattleNet.External.Configuration.Source.INI
{
	#region IniFileType enumeration
	/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/docs/*' />
	public enum INIFileType
	{
		/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/Value[@name="Standard"]/docs/*' />
		Standard,
		/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/Value[@name="PythonStyle"]/docs/*' />
		PythonStyle,
		/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/Value[@name="SambaStyle"]/docs/*' />
		SambaStyle,
		/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/Value[@name="MysqlStyle"]/docs/*' />
		MySQLStyle,
		/// <include file='IniDocument.xml' path='//Enum[@name="IniFileType"]/Value[@name="WindowsStyle"]/docs/*' />
		WindowsStyle
	}
	#endregion

	/// <include file='IniDocument.xml' path='//Class[@name="IniDocument"]/docs/*' />
	public class INIDocument
	{
		#region Private variables
		private INISectionCollection _sections = new INISectionCollection ();
		private ArrayList _initialComment = new ArrayList ();
		private INIFileType _fileType = INIFileType.Standard;
		#endregion
		
		#region Public properties
		/// <include file='IniDocument.xml' path='//Property[@name="FileType"]/docs/*' />
		public INIFileType FileType
		{
			get { return _fileType; }
			set { _fileType = value; }
		}
		#endregion

		#region Constructors
		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorPath"]/docs/*' />
		public INIDocument (string filePath)
		{
			_fileType = INIFileType.Standard;
			Load (filePath);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorPathType"]/docs/*' />
		public INIDocument (string filePath, INIFileType type)
		{
			_fileType = type;
			Load (filePath);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorTextReader"]/docs/*' />
		public INIDocument (TextReader reader)
		{
			_fileType = INIFileType.Standard;
			Load (reader);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorTextReaderType"]/docs/*' />
		public INIDocument (TextReader reader, INIFileType type)
		{
			_fileType = type;
			Load (reader);
		}
		
		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorStream"]/docs/*' />
		public INIDocument (Stream stream)
		{
			_fileType = INIFileType.Standard;
			Load (stream);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorStreamType"]/docs/*' />
		public INIDocument (Stream stream, INIFileType type)
		{
			_fileType = type;
			Load (stream);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="ConstructorIniReader"]/docs/*' />
		public INIDocument (INIReader reader)
		{
			_fileType = INIFileType.Standard;
			Load (reader);
		}

		/// <include file='IniDocument.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public INIDocument ()
		{
		}
		#endregion
		
		#region Public methods
		/// <include file='IniDocument.xml' path='//Method[@name="LoadPath"]/docs/*' />
		public void Load (string filePath)
		{
			Load (new StreamReader (filePath));
		}

		/// <include file='IniDocument.xml' path='//Method[@name="LoadTextReader"]/docs/*' />
		public void Load (TextReader reader)
		{
			Load (GetINIReader (reader, _fileType));
		}

		/// <include file='IniDocument.xml' path='//Method[@name="LoadStream"]/docs/*' />
		public void Load (Stream stream)
		{
			Load (new StreamReader (stream));
		}

		/// <include file='IniDocument.xml' path='//Method[@name="LoadIniReader"]/docs/*' />
		public void Load (INIReader reader)
		{
			LoadReader (reader);
		}

		/// <include file='IniSection.xml' path='//Property[@name="Comment"]/docs/*' />
		public INISectionCollection Sections
		{
			get { return _sections; }
		}

		/// <include file='IniDocument.xml' path='//Method[@name="SaveTextWriter"]/docs/*' />
		public void Save (TextWriter textWriter)
		{
			INIWriter writer = GetINIWriter (textWriter, _fileType);
			INIItem item = null;
			INISection section = null;
			
			foreach (string comment in _initialComment)
			{
				writer.WriteEmpty  (comment);
			}

			for (int j = 0; j < _sections.Count; j++)
			{
				section = _sections[j];
				writer.WriteSection (section.Name, section.Comment);
				for (int i = 0; i < section.ItemCount; i++)
				{
					item = section.GetItem (i);
					switch (item.Type)
					{
					case INIType.Key:
						writer.WriteKey (item.Name, item.Value, item.Comment);
						break;
					case INIType.Empty:
						writer.WriteEmpty (item.Comment);
						break;
					}
				}
			}

			writer.Close ();
		}
		
		/// <include file='IniDocument.xml' path='//Method[@name="SavePath"]/docs/*' />
		public void Save (string filePath)
		{
			StreamWriter writer = new StreamWriter (filePath);
			Save (writer);
			writer.Close ();
		}

		/// <include file='IniDocument.xml' path='//Method[@name="SaveStream"]/docs/*' />
		public void Save (Stream stream)
		{
			Save (new StreamWriter (stream));
		}
		#endregion
		
		#region Private methods
		/// <summary>
		/// Loads the file not saving comments.
		/// </summary>
		private void LoadReader (INIReader reader)
		{
			reader.IgnoreComments = false;
			bool sectionFound = false;
			INISection section = null;
			
			try {
				while (reader.Read ())
				{
					switch (reader.Type)
					{
					case INIType.Empty:
						if (!sectionFound) {
							_initialComment.Add (reader.Comment);
						} else {
							section.Set (reader.Comment);
						}

						break;
					case INIType.Section:
						sectionFound = true;
						// If section already exists then overwrite it
						if (_sections[reader.Name] != null) {
							_sections.Remove (reader.Name);
						}
						section = new INISection (reader.Name, reader.Comment);
						_sections.Add (section);

						break;
					case INIType.Key:
						if (section.GetValue (reader.Name) == null) { 
							section.Set (reader.Name, reader.Value, reader.Comment); 
						} 
						break;
					}
				}
			} catch (Exception ex) {
				throw ex;
			} finally {
				// Always close the file
				reader.Close ();
			}
		}

		/// <summary>
		/// Returns a proper INI reader depending upon the type parameter.
		/// </summary>
		private INIReader GetINIReader (TextReader reader, INIFileType type)
		{
			INIReader result = new INIReader (reader);

			switch (type)
			{
			case INIFileType.Standard:
				// do nothing
				break;
			case INIFileType.PythonStyle:
				result.AcceptCommentAfterKey = false;
				result.SetCommentDelimiters (new char[] { ';', '#' });
				result.SetAssignDelimiters (new char[] { ':' });
				break;
			case INIFileType.SambaStyle:
				result.AcceptCommentAfterKey = false;
				result.SetCommentDelimiters (new char[] { ';', '#' });
				result.LineContinuation = true;
				break;
			case INIFileType.MySQLStyle:
				result.AcceptCommentAfterKey = false;
				result.AcceptNoAssignmentOperator = true;
				result.SetCommentDelimiters (new char[] { '#' });
				result.SetAssignDelimiters (new char[] { ':', '=' });
				break;
			case INIFileType.WindowsStyle:
				result.ConsumeAllKeyText = true;
				break;
			}

			return result;
		}

		/// <summary>
		/// Returns a proper IniWriter depending upon the type parameter.
		/// </summary>
		private INIWriter GetINIWriter (TextWriter reader, INIFileType type)
		{
			INIWriter result = new INIWriter (reader);

			switch (type)
			{
			case INIFileType.Standard:
			case INIFileType.WindowsStyle:
				// do nothing
				break;
			case INIFileType.PythonStyle:
				result.AssignDelimiter = ':';
				result.CommentDelimiter = '#';
				break;
			case INIFileType.SambaStyle:
			case INIFileType.MySQLStyle:
				result.AssignDelimiter = '=';
				result.CommentDelimiter = '#';
				break;
			}

			return result;
		}
		#endregion
	}
}

