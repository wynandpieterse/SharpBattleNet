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
	#region ConfigEventHandler class
	/// <include file='ConfigEventArgs.xml' path='//Delegate[@name="ConfigEventHandler"]/docs/*' />
	public delegate void ConfigurationEventHandler (object sender, ConfigurationEventArguments e);

	/// <include file='ConfigEventArgs.xml' path='//Class[@name="ConfigEventArgs"]/docs/*' />
	public class ConfigurationEventArguments : EventArgs
	{
		private IConfiguration _configuration = null;

		/// <include file='ConfigEventArgs.xml' path='//Constructor[@name="ConstructorIConfig"]/docs/*' />
		public ConfigurationEventArguments (IConfiguration config)
		{
			this._configuration = config;
		}

		/// <include file='ConfigEventArgs.xml' path='//Property[@name="Config"]/docs/*' />
		public IConfiguration Configuration
		{
			get { return _configuration; }
		}
	}
	#endregion

	/// <include file='ConfigCollection.xml' path='//Class[@name="ConfigCollection"]/docs/*' />
	public class ConfigurationCollection : ICollection, IEnumerable, IList
	{
		#region Private variables
		private ArrayList _configurationList = new ArrayList ();
		private ConfigurationSourceBase _owner = null;
		#endregion

		#region Constructors
		/// <include file='ConfigCollection.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public ConfigurationCollection (ConfigurationSourceBase owner)
		{
			this._owner = owner;
		}
		#endregion
		
		#region Public properties
		/// <include file='ConfigCollection.xml' path='//Property[@name="Count"]/docs/*' />
		public int Count
		{
			get { return _configurationList.Count; }
		}
		
		/// <include file='ConfigCollection.xml' path='//Property[@name="IsSynchronized"]/docs/*' />
		public bool IsSynchronized
		{
			get { return false; }
		}
		
		/// <include file='ConfigCollection.xml' path='//Property[@name="SyncRoot"]/docs/*' />
		public object SyncRoot
		{
			get { return this; }
		}
		
		/// <include file='ConfigCollection.xml' path='//Property[@name="ItemIndex"]/docs/*' />
		public IConfiguration this[int index]
		{
			get { return (IConfiguration)_configurationList[index]; }
		}

		/// <include file='ConfigCollection.xml' path='//Property[@name="ItemIndex"]/docs/*' />
		object IList.this[int index]
		{
			get { return _configurationList[index]; }
			set {  }
		}
		
		/// <include file='ConfigCollection.xml' path='//Property[@name="ItemName"]/docs/*' />
		public IConfiguration this[string configName]
		{
			get
			{
				IConfiguration result = null;

				foreach (IConfiguration config in _configurationList)
				{
					if (config.Name == configName) {
						result = config;
						break;
					}
				}
				
				return result;
			}
		}

		/// <include file='ConfigCollection.xml' path='//Property[@name="IsFixedSize"]/docs/*' />
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <include file='ConfigCollection.xml' path='//Property[@name="IsReadOnly"]/docs/*' />
		public bool IsReadOnly
		{
			get { return false; }
		}
		#endregion
		
		#region Public methods
		/// <include file='ConfigCollection.xml' path='//Method[@name="Add"]/docs/*' />
		public void Add (IConfiguration config)
		{
			if (_configurationList.Contains (config)) {
				throw new ArgumentException ("IConfig already exists");
			}
			IConfiguration existingConfig = this[config.Name];

			if (existingConfig != null) {
				// Set all new keys
				string[] keys = config.GetKeys ();
				for (int i = 0; i < keys.Length; i++)
				{
					existingConfig.Set (keys[i], config.Get (keys[i]));
				}
			} else {
				_configurationList.Add (config);
				OnConfigurationAdded (new ConfigurationEventArguments (config));
			}
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Add"]/docs/*' />
		int IList.Add (object config)
		{
			IConfiguration newConfig = config as IConfiguration;

			if (newConfig == null) {
				throw new Exception ("Must be an IConfig");
			} else {
				this.Add (newConfig);
				return IndexOf (newConfig);
			}
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="AddName"]/docs/*' />
		public IConfiguration Add (string name)
		{
			ConfigurationBase result = null;

			if (this[name] == null) {
				result = new ConfigurationBase (name, _owner);
				_configurationList.Add (result);
				OnConfigurationAdded (new ConfigurationEventArguments (result));
			} else {
				throw new ArgumentException ("An IConfig of that name already exists");
			}
			
			return result;
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (IConfiguration config)
		{
			_configurationList.Remove (config);
			OnConfigurationRemoved (new ConfigurationEventArguments (config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (object config)
		{
			_configurationList.Remove (config);
			OnConfigurationRemoved (new ConfigurationEventArguments ((IConfiguration)config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="RemoveAt"]/docs/*' />
		public void RemoveAt (int index)
		{
			IConfiguration config = (IConfiguration)_configurationList[index];
			_configurationList.RemoveAt (index);
			OnConfigurationRemoved (new ConfigurationEventArguments (config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Clear"]/docs/*' />
		public void Clear ()
		{
			_configurationList.Clear ();
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="GetEnumerator"]/docs/*' />
		public IEnumerator GetEnumerator ()
		{
			return _configurationList.GetEnumerator ();
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="CopyTo"]/docs/*' />
		public void CopyTo (Array array, int index)
		{
			_configurationList.CopyTo (array, index);
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="CopyToStrong"]/docs/*' />
		public void CopyTo (IConfiguration[] array, int index)
		{
			((ICollection)_configurationList).CopyTo (array, index);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Contains"]/docs/*' />
		public bool Contains (object config)
		{
			return _configurationList.Contains (config);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="IndexOf"]/docs/*' />
		public int IndexOf (object config)
		{
			return _configurationList.IndexOf (config);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Insert"]/docs/*' />
		public void Insert (int index, object config)
		{
			_configurationList.Insert (index, config);
		}
		#endregion

		#region Public events
		/// <include file='ConfigCollection.xml' path='//Event[@name="ConfigAdded"]/docs/*' />
		public event ConfigurationEventHandler ConfigurationAdded;

		/// <include file='ConfigCollection.xml' path='//Event[@name="ConfigRemoved"]/docs/*' />
		public event ConfigurationEventHandler ConfigurationRemoved;
		#endregion

		#region Protected methods
		/// <include file='ConfigCollection.xml' path='//Method[@name="OnConfigAdded"]/docs/*' />
		protected void OnConfigurationAdded (ConfigurationEventArguments e)
		{
			if (ConfigurationAdded != null) {
				ConfigurationAdded (this, e);
			}
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="OnConfigRemoved"]/docs/*' />
		protected void OnConfigurationRemoved (ConfigurationEventArguments e)
		{
			if (ConfigurationRemoved != null) {
				ConfigurationRemoved (this, e);
			}
		}
		#endregion
		
		#region Private methods
		#endregion
	}
}