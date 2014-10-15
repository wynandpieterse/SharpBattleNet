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
using System.Collections;
using SharpBattleNet.External.Configuration.Utilities;

namespace SharpBattleNet.External.Configuration.Source.INI
{
	/// <include file='IniSection.xml' path='//Class[@name="IniSection"]/docs/*' />
	public class INISection
	{
		#region Private variables
		private OrderedList _configurationList = new OrderedList ();
		private string _name = "";
		private string _comment = null;
		private int _commentCount = 0;
		#endregion

		#region Constructors
		/// <include file='IniSection.xml' path='//Constructor[@name="ConstructorComment"]/docs/*' />
		public INISection (string name, string comment)
		{
			this._name = name;
			this._comment = comment;
		}
		
		/// <include file='IniSection.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public INISection (string name)
			: this (name, null)
		{
		}
		#endregion
		
		#region Public properties
		/// <include file='IniSection.xml' path='//Property[@name="Name"]/docs/*' />
		public string Name
		{
			get { return _name; }
		}
		
		/// <include file='IniSection.xml' path='//Property[@name="Comment"]/docs/*' />
		public string Comment
		{
			get { return _comment; }
		}
		
		/// <include file='IniSection.xml' path='//Property[@name="ItemCount"]/docs/*' />
		public int ItemCount
		{
			get { return _configurationList.Count; }
		}
		#endregion

		#region Public methods
		
		/// <include file='IniSection.xml' path='//Method[@name="GetValue"]/docs/*' />
		public string GetValue (string key)
		{
			string result = null;

			if (Contains (key)) {
				INIItem item = (INIItem)_configurationList[key];
				result = item.Value;
			}

			return result;
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="GetItem"]/docs/*' />
		public INIItem GetItem (int index)
		{
			return (INIItem)_configurationList[index];
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="GetKeys"]/docs/*' />
		public string[] GetKeys ()
		{
			ArrayList list = new ArrayList ();
			INIItem item = null;
			
			for (int i = 0; i < _configurationList.Count; i++)
			{
				item = (INIItem)_configurationList[i]; 
				if (item.Type == INIType.Key) {
					list.Add (item.Name);
				}
			}
			string[] result = new string[list.Count];
			list.CopyTo (result, 0);
			
			return result;
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="Contains"]/docs/*' />
		public bool Contains (string key)
		{
			return (_configurationList[key] != null); 
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="SetKeyComment"]/docs/*' />
		public void Set (string key, string value, string comment)
		{
			INIItem item = null;

			if (Contains (key)) {
				item = (INIItem)_configurationList[key];
				item.Value = value;
				item.Comment = comment;
			} else {
				item = new INIItem (key, value, INIType.Key, comment);
				_configurationList.Add (key, item);
			}
		}

		/// <include file='IniSection.xml' path='//Method[@name="SetKey"]/docs/*' />
		public void Set (string key, string value)
		{
			Set (key, value, null);
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="SetComment"]/docs/*' />
		public void Set (string comment)
		{
			string name = "#comment" + _commentCount;
			INIItem item = new INIItem (name, null, 
										INIType.Empty, comment);
			_configurationList.Add (name, item);
			
			_commentCount++;
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="SetNoComment"]/docs/*' />
		public void Set ()
		{
			Set (null);
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (string key)
		{
			if (Contains (key)) {
				_configurationList.Remove (key);
			}
		}
		#endregion
	}
}