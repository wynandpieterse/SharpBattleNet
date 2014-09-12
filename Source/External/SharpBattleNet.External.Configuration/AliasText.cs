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

namespace SharpBattleNet.External.Configuration
{
	/// <include file='AliasText.xml' path='//Class[@name="AliasText"]/docs/*' />
	public class AliasText
	{
		#region Private variables
		private Hashtable _integerAlias = null;
		private Hashtable _booleanAlias = null;
		#endregion

		#region Constructors
		/// <include file='AliasText.xml' path='//Constructor[@name="AliasText"]/docs/*' />
		public AliasText ()
		{
			_integerAlias = InsensitiveHashtable ();
			_booleanAlias = InsensitiveHashtable ();
			DefaultAliasLoad ();
		}
		#endregion
		
		#region Public methods
		/// <include file='AliasText.xml' path='//Method[@name="AddAliasInt"]/docs/*' />
		public void AddAlias (string key, string alias, int value)
		{
			if (_integerAlias.Contains (key)) {
				Hashtable keys = (Hashtable)_integerAlias[key];
				
				keys[alias] = value;
			} else {
				Hashtable keys = InsensitiveHashtable ();
				keys[alias] = value;
				_integerAlias.Add (key, keys);
			}
		}
		
		/// <include file='AliasText.xml' path='//Method[@name="AddAliasBoolean"]/docs/*' />
		public void AddAlias (string alias, bool value)
		{
			_booleanAlias[alias] = value;
		}
		
#if (NET_COMPACT_1_0)
#else
		/// <include file='AliasText.xml' path='//Method[@name="AddAliasEnum"]/docs/*' />
		public void AddAlias (string key, Enum enumAlias)
		{
			SetAliasTypes (key, enumAlias);
		}
#endif
		
		/// <include file='AliasText.xml' path='//Method[@name="ContainsBoolean"]/docs/*' />
		public bool ContainsBoolean (string key)
		{
			return _booleanAlias.Contains (key);
		}
		
		/// <include file='AliasText.xml' path='//Method[@name="ContainsInt"]/docs/*' />
		public bool ContainsInteger (string key, string alias)
		{
			bool result = false;

			if (_integerAlias.Contains (key)) {
				Hashtable keys = (Hashtable)_integerAlias[key];
				result = (keys.Contains (alias));
			}
			
			return result;
		}
		
		/// <include file='AliasText.xml' path='//Method[@name="GetBoolean"]/docs/*' />
		public bool GetBoolean (string key)
		{
			if (!_booleanAlias.Contains (key)) {
				throw new ArgumentException ("Alias does not exist for text");
			}
			
			return (bool)_booleanAlias[key];
		}
		
		/// <include file='AliasText.xml' path='//Method[@name="GetInt"]/docs/*' />
		public int GetInteger (string key, string alias)
		{
			if (!_integerAlias.Contains (key)) {
				throw new ArgumentException ("Alias does not exist for key");
			}

			Hashtable keys = (Hashtable)_integerAlias[key];

			if (!keys.Contains (alias)) {
				throw new ArgumentException ("Config value does not match a " +
											 "supplied alias");
			}
			
			return (int)keys[alias];
		}
		#endregion
		
		#region Private methods
		/// <summary>
		/// Loads the default alias values.
		/// </summary>
		private void DefaultAliasLoad ()
		{
			AddAlias("true", true);
			AddAlias("false", false);
		}

#if (NET_COMPACT_1_0)
#else
		/// <summary>
		/// Extracts and sets the alias types from an enumeration.
		/// </summary>
		private void SetAliasTypes (string key, Enum enumAlias)
		{
			string[] names = Enum.GetNames (enumAlias.GetType ());
			int[] values = (int[])Enum.GetValues (enumAlias.GetType ());
			
			for (int i = 0; i < names.Length; i++)
			{
				AddAlias (key, names[i], values[i]);
			}
		}
#endif
		
		/// <summary>
		/// Returns a case insensitive hashtable.
		/// </summary>
		private Hashtable InsensitiveHashtable ()
		{
			return new Hashtable (CaseInsensitiveHashCodeProvider.Default, 
								  CaseInsensitiveComparer.Default);
		}
		#endregion
	}
}