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

namespace Nini.Config
{
	#region ConfigEventHandler class
	/// <include file='ConfigEventArgs.xml' path='//Delegate[@name="ConfigEventHandler"]/docs/*' />
	public delegate void ConfigEventHandler (object sender, ConfigEventArgs e);

	/// <include file='ConfigEventArgs.xml' path='//Class[@name="ConfigEventArgs"]/docs/*' />
	public class ConfigEventArgs : EventArgs
	{
		IConfiguration config = null;

		/// <include file='ConfigEventArgs.xml' path='//Constructor[@name="ConstructorIConfig"]/docs/*' />
		public ConfigEventArgs (IConfiguration config)
		{
			this.config = config;
		}

		/// <include file='ConfigEventArgs.xml' path='//Property[@name="Config"]/docs/*' />
		public IConfiguration Config
		{
			get { return config; }
		}
	}
	#endregion

	/// <include file='ConfigCollection.xml' path='//Class[@name="ConfigCollection"]/docs/*' />
	public class ConfigurationCollection : ICollection, IEnumerable, IList
	{
		#region Private variables
		ArrayList configList = new ArrayList ();
		ConfigurationSourceBase owner = null;
		#endregion

		#region Constructors
		/// <include file='ConfigCollection.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public ConfigurationCollection (ConfigurationSourceBase owner)
		{
			this.owner = owner;
		}
		#endregion
		
		#region Public properties
		/// <include file='ConfigCollection.xml' path='//Property[@name="Count"]/docs/*' />
		public int Count
		{
			get { return configList.Count; }
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
			get { return (IConfiguration)configList[index]; }
		}

		/// <include file='ConfigCollection.xml' path='//Property[@name="ItemIndex"]/docs/*' />
		object IList.this[int index]
		{
			get { return configList[index]; }
			set {  }
		}
		
		/// <include file='ConfigCollection.xml' path='//Property[@name="ItemName"]/docs/*' />
		public IConfiguration this[string configName]
		{
			get
			{
				IConfiguration result = null;

				foreach (IConfiguration config in configList)
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
			if (configList.Contains (config)) {
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
				configList.Add (config);
				OnConfigAdded (new ConfigEventArgs (config));
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
				result = new ConfigurationBase (name, owner);
				configList.Add (result);
				OnConfigAdded (new ConfigEventArgs (result));
			} else {
				throw new ArgumentException ("An IConfig of that name already exists");
			}
			
			return result;
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (IConfiguration config)
		{
			configList.Remove (config);
			OnConfigRemoved (new ConfigEventArgs (config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (object config)
		{
			configList.Remove (config);
			OnConfigRemoved (new ConfigEventArgs ((IConfiguration)config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="RemoveAt"]/docs/*' />
		public void RemoveAt (int index)
		{
			IConfiguration config = (IConfiguration)configList[index];
			configList.RemoveAt (index);
			OnConfigRemoved (new ConfigEventArgs (config));
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Clear"]/docs/*' />
		public void Clear ()
		{
			configList.Clear ();
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="GetEnumerator"]/docs/*' />
		public IEnumerator GetEnumerator ()
		{
			return configList.GetEnumerator ();
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="CopyTo"]/docs/*' />
		public void CopyTo (Array array, int index)
		{
			configList.CopyTo (array, index);
		}
		
		/// <include file='ConfigCollection.xml' path='//Method[@name="CopyToStrong"]/docs/*' />
		public void CopyTo (IConfiguration[] array, int index)
		{
			((ICollection)configList).CopyTo (array, index);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Contains"]/docs/*' />
		public bool Contains (object config)
		{
			return configList.Contains (config);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="IndexOf"]/docs/*' />
		public int IndexOf (object config)
		{
			return configList.IndexOf (config);
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="Insert"]/docs/*' />
		public void Insert (int index, object config)
		{
			configList.Insert (index, config);
		}
		#endregion

		#region Public events
		/// <include file='ConfigCollection.xml' path='//Event[@name="ConfigAdded"]/docs/*' />
		public event ConfigEventHandler ConfigAdded;

		/// <include file='ConfigCollection.xml' path='//Event[@name="ConfigRemoved"]/docs/*' />
		public event ConfigEventHandler ConfigRemoved;
		#endregion

		#region Protected methods
		/// <include file='ConfigCollection.xml' path='//Method[@name="OnConfigAdded"]/docs/*' />
		protected void OnConfigAdded (ConfigEventArgs e)
		{
			if (ConfigAdded != null) {
				ConfigAdded (this, e);
			}
		}

		/// <include file='ConfigCollection.xml' path='//Method[@name="OnConfigRemoved"]/docs/*' />
		protected void OnConfigRemoved (ConfigEventArgs e)
		{
			if (ConfigRemoved != null) {
				ConfigRemoved (this, e);
			}
		}
		#endregion
		
		#region Private methods
		#endregion
	}
}