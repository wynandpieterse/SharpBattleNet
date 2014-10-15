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
using System.Text;
using System.Collections;

namespace SharpBattleNet.External.Configuration
{
	/// <include file='IConfigSource.xml' path='//Interface[@name="IConfigSource"]/docs/*' />
	public abstract class ConfigurationSourceBase : IConfigurationSource
	{
		#region Private variables
		private ArrayList _sourceList = new ArrayList ();
		private ConfigurationCollection _configurationList = null;
		private bool _autoSave = false;
		private AliasText _alias = new AliasText ();
		#endregion

		#region Constructors
		/// <include file='ConfigSourceBase.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public ConfigurationSourceBase ()
		{
			_configurationList = new ConfigurationCollection (this);
		}
		#endregion
		
		#region Public properties
		/// <include file='IConfigSource.xml' path='//Property[@name="Configs"]/docs/*' />
		public ConfigurationCollection Configurations
		{
			get { return _configurationList; }
		}
		
		/// <include file='IConfigSource.xml' path='//Property[@name="AutoSave"]/docs/*' />
		public bool AutoSave
		{
			get { return _autoSave; }
			set { _autoSave = value; }
		}
		
		/// <include file='IConfigSource.xml' path='//Property[@name="Alias"]/docs/*' />
		public AliasText Alias
		{
			get { return _alias; }
		}
		#endregion
		
		#region Public methods
		/// <include file='IConfigSource.xml' path='//Method[@name="Merge"]/docs/*' />
		public void Merge (IConfigurationSource source)
		{
			if (!_sourceList.Contains (source))  {
				_sourceList.Add (source);
			}
			
			foreach (IConfiguration config in source.Configurations)
			{
				this.Configurations.Add (config);
			}
		}
		
		/// <include file='IConfigSource.xml' path='//Method[@name="AddConfig"]/docs/*' />
		public virtual IConfiguration AddConfiguration (string name)
		{
			return _configurationList.Add (name);
		}

		/// <include file='IConfigSource.xml' path='//Method[@name="GetExpanded"]/docs/*' />
		public string GetExpanded (IConfiguration config, string key)
		{
			return Expand (config, key, false);
		}
		
		/// <include file='IConfigSource.xml' path='//Method[@name="Save"]/docs/*' />
		public virtual void Save ()
		{
			OnSaved (new EventArgs ());
		}

		/// <include file='IConfigSource.xml' path='//Method[@name="Reload"]/docs/*' />
		public virtual void Reload ()
		{
			OnReloaded (new EventArgs ());
		}
		
		/// <include file='IConfigSource.xml' path='//Method[@name="ExpandKeyValues"]/docs/*' />
		public void ExpandKeyValues ()
		{
			string[] keys = null;

			foreach (IConfiguration config in _configurationList)
			{
				keys = config.GetKeys ();
				for (int i = 0; i < keys.Length; i++)
				{
					Expand (config, keys[i], true);
				}
			}
		}

		/// <include file='IConfigSource.xml' path='//Method[@name="ReplaceKeyValues"]/docs/*' />
		public void ReplaceKeyValues ()
		{
			ExpandKeyValues ();
		}
		#endregion

		#region Public events
		/// <include file='IConfigSource.xml' path='//Event[@name="Reloaded"]/docs/*' />
		public event EventHandler Reloaded;

		/// <include file='IConfigSource.xml' path='//Event[@name="Saved"]/docs/*' />
		public event EventHandler Saved;
		#endregion

		#region Protected methods
		/// <include file='ConfigSourceBase.xml' path='//Method[@name="OnReloaded"]/docs/*' />
		protected void OnReloaded (EventArgs e)
		{
			if (Reloaded != null) {
				Reloaded (this, e);
			}
		}

		/// <include file='ConfigSourceBase.xml' path='//Method[@name="OnSaved"]/docs/*' />
		protected void OnSaved (EventArgs e)
		{
			if (Saved != null) {
				Saved (this, e);
			}
		}
		#endregion

		#region Private methods	
		/// <summary>
		/// Expands key values from the given IConfig.
		/// </summary>
		private string Expand (IConfiguration config, string key, bool setValue)
		{
			string result = config.Get (key);
			if (result == null) {
				throw new ArgumentException (String.Format ("[{0}] not found in [{1}]",
										key, config.Name));
			}

			while (true)
			{
				int startIndex = result.IndexOf ("${", 0);
				if (startIndex == -1) {
					break;
				}

				int endIndex = result.IndexOf ("}", startIndex + 2);
				if (endIndex == -1) {
					break;
				}

				string search = result.Substring (startIndex + 2, 
												  endIndex - (startIndex + 2));

				if (search == key) {
					// Prevent infinite recursion
					throw new ArgumentException 
						("Key cannot have a expand value of itself: " + key);
				}

				string replace = ExpandValue (config, search);

				result = result.Replace("${" + search + "}", replace);
			}

			if (setValue) {
				config.Set(key, result);
			}

			return result;
		}
		
		/// <summary>
		/// Returns the replacement value of a config.
		/// </summary>
		private string ExpandValue (IConfiguration config, string search)
		{
			string result = null;
			
			string[] replaces = search.Split ('|');
			
			if (replaces.Length > 1) {
				IConfiguration newConfig = this.Configurations[replaces[0]];
				if (newConfig == null) {
					throw new ArgumentException ("Expand config not found: "
												 + replaces[0]);
				}
				result = newConfig.Get (replaces[1]);
				if (result == null) {
					throw new ArgumentException ("Expand key not found: "
												 + replaces[1]);
				}
			} else {
				result = config.Get (search);
				
				if (result == null) {
				    throw new ArgumentException ("Key not found: " + search);
				}
			}
			
			return result;
		}
		#endregion
	}
}
