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
using Nini.Util;

namespace Nini.Ini
{
	/// <include file='IniSection.xml' path='//Class[@name="IniSection"]/docs/*' />
	public class INISection
	{
		#region Private variables
		OrderedList configList = new OrderedList ();
		string name = "";
		string comment = null;
		int commentCount = 0;
		#endregion

		#region Constructors
		/// <include file='IniSection.xml' path='//Constructor[@name="ConstructorComment"]/docs/*' />
		public INISection (string name, string comment)
		{
			this.name = name;
			this.comment = comment;
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
			get { return name; }
		}
		
		/// <include file='IniSection.xml' path='//Property[@name="Comment"]/docs/*' />
		public string Comment
		{
			get { return comment; }
		}
		
		/// <include file='IniSection.xml' path='//Property[@name="ItemCount"]/docs/*' />
		public int ItemCount
		{
			get { return configList.Count; }
		}
		#endregion

		#region Public methods
		
		/// <include file='IniSection.xml' path='//Method[@name="GetValue"]/docs/*' />
		public string GetValue (string key)
		{
			string result = null;

			if (Contains (key)) {
				INIItem item = (INIItem)configList[key];
				result = item.Value;
			}

			return result;
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="GetItem"]/docs/*' />
		public INIItem GetItem (int index)
		{
			return (INIItem)configList[index];
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="GetKeys"]/docs/*' />
		public string[] GetKeys ()
		{
			ArrayList list = new ArrayList ();
			INIItem item = null;
			
			for (int i = 0; i < configList.Count; i++)
			{
				item = (INIItem)configList[i]; 
				if (item.Type == IniType.Key) {
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
			return (configList[key] != null); 
		}
		
		/// <include file='IniSection.xml' path='//Method[@name="SetKeyComment"]/docs/*' />
		public void Set (string key, string value, string comment)
		{
			INIItem item = null;

			if (Contains (key)) {
				item = (INIItem)configList[key];
				item.Value = value;
				item.Comment = comment;
			} else {
				item = new INIItem (key, value, IniType.Key, comment);
				configList.Add (key, item);
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
			string name = "#comment" + commentCount;
			INIItem item = new INIItem (name, null, 
										IniType.Empty, comment);
			configList.Add (name, item);
			
			commentCount++;
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
				configList.Remove (key);
			}
		}
		#endregion
	}
}