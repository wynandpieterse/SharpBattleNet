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
using System.Security;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace SharpBattleNet.External.Configuration.Source.INI
{
	/// <include file='IniException.xml' path='//Class[@name="IniException"]/docs/*' />
#if (NET_COMPACT_1_0)
#else
	[Serializable]
#endif
	public class INIException : SystemException /*, ISerializable */
	{
		#region Private variables
		private INIReader _iniReader = null;
		private string _message = "";
		#endregion

		#region Public properties
		/// <include file='IniException.xml' path='//Property[@name="LinePosition"]/docs/*' />
		public int LinePosition
		{
			get	{
				return (_iniReader == null) ? 0 : _iniReader.LinePosition;
			}
		}
		
		/// <include file='IniException.xml' path='//Property[@name="LineNumber"]/docs/*' />
		public int LineNumber
		{
			get {
				return (_iniReader == null) ? 0 : _iniReader.LineNumber;
			}
		}
		
		/// <include file='IniException.xml' path='//Property[@name="Message"]/docs/*' />
		public override string Message
		{
			get {
				if (_iniReader == null) {
					return base.Message;
				}

				return String.Format (CultureInfo.InvariantCulture, "{0} - Line: {1}, Position: {2}.",
										_message, this.LineNumber, this.LinePosition);
			}
		}
		#endregion

		#region Constructors
		/// <include file='IniException.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public INIException ()
			: base ()
		{
			this._message  = "An error has occurred";
		}
		
		/// <include file='IniException.xml' path='//Constructor[@name="ConstructorException"]/docs/*' />
		public INIException (string message, Exception exception)
			: base (message, exception)
		{
		}

		/// <include file='IniException.xml' path='//Constructor[@name="ConstructorMessage"]/docs/*' />
		public INIException (string message)
			: base (message)
		{
			this._message  = message;
		}
		
		/// <include file='IniException.xml' path='//Constructor[@name="ConstructorTextReader"]/docs/*' />
		internal INIException (INIReader reader, string message)
			: this (message)
		{
			_iniReader = reader;
			this._message = message;
		}

#if (NET_COMPACT_1_0)
#else
		/// <include file='IniException.xml' path='//Constructor[@name="ConstructorSerialize"]/docs/*' />
		protected INIException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
#endif
		#endregion
		
		#region Public methods
#if (NET_COMPACT_1_0)
#else
		/// <include file='IniException.xml' path='//Method[@name="GetObjectData"]/docs/*' />
		[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
		public override void GetObjectData (SerializationInfo info, 
											StreamingContext context)
		{
			base.GetObjectData (info, context);
			if (_iniReader != null) {
				info.AddValue ("lineNumber", _iniReader.LineNumber);

				info.AddValue ("linePosition", _iniReader.LinePosition);
			}
		}
#endif
		#endregion
	}
}