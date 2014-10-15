#region Copyright
//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 
#endregion

using SharpBattleNet.External.Configuration.Utilities;
using System;
using System.Collections;

namespace SharpBattleNet.External.Configuration.Source.INI
{
	/// <include file='IniSectionCollection.xml' path='//Class[@name="IniSectionCollection"]/docs/*' />
	public class INISectionCollection : ICollection, IEnumerable
	{
		#region Private variables
		private OrderedList _list = new OrderedList ();
		#endregion

		#region Public properties	
		/// <include file='IniSectionCollection.xml' path='//Property[@name="ItemIndex"]/docs/*' />
		public INISection this[int index]
		{
			get { return (INISection)_list[index]; }
		}
		
		/// <include file='IniSectionCollection.xml' path='//Property[@name="ItemName"]/docs/*' />
		public INISection this[string configName]
		{
			get { return (INISection)_list[configName]; }
		}

		/// <include file='IniSectionCollection.xml' path='//Property[@name="Count"]/docs/*' />
		public int Count
		{
			get { return _list.Count; }
		}
		
		/// <include file='IniSectionCollection.xml' path='//Property[@name="SyncRoot"]/docs/*' />
		public object SyncRoot
		{
			get { return _list.SyncRoot; }
		}
		
		/// <include file='IniSectionCollection.xml' path='//Property[@name="IsSynchronized"]/docs/*' />
		public bool IsSynchronized
		{
			get { return _list.IsSynchronized; }
		}
		#endregion

		#region Public methods
		/// <include file='IniSectionCollection.xml' path='//Method[@name="Add"]/docs/*' />
		public void Add (INISection section)
		{
			if (_list.Contains (section)) {
				throw new ArgumentException ("IniSection already exists");
			}
			
			_list.Add (section.Name, section);
		}
		
		/// <include file='IniSectionCollection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (string config)
		{
			_list.Remove (config);
		}
		
		/// <include file='IniSectionCollection.xml' path='//Method[@name="CopyTo"]/docs/*' />
		public void CopyTo (Array array, int index) 
		{
			_list.CopyTo (array, index);
		}
		
		/// <include file='IniSectionCollection.xml' path='//Method[@name="CopyToStrong"]/docs/*' />
		public void CopyTo (INISection[] array, int index)
		{
			((ICollection)_list).CopyTo (array, index);
		}

		/// <include file='IniSectionCollection.xml' path='//Method[@name="GetEnumerator"]/docs/*' />
		public IEnumerator GetEnumerator () 
		{
			return _list.GetEnumerator ();
		}
		#endregion
		
		#region Private methods
		#endregion
	}
}