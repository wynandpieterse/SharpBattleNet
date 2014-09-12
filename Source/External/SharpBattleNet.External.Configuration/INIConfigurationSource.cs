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
	/// <include file='IniConfigSource.xml' path='//Class[@name="IniConfigSource"]/docs/*' />
	public class INIConfigurationSource : ConfigurationSourceBase
	{
		#region Private variables
		private INIDocument _iniDocument = null;
		private string _savePath = null;
		private bool _caseSensitive = true;
		#endregion
		
		#region Public properties
		#endregion

		#region Constructors
		/// <include file='IniConfigSource.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public INIConfigurationSource ()
		{
			_iniDocument = new INIDocument ();
		}

		/// <include file='IniConfigSource.xml' path='//Constructor[@name="ConstructorPath"]/docs/*' />
		public INIConfigurationSource (string filePath)
		{
			Load (filePath);
		}
		
		/// <include file='IniConfigSource.xml' path='//Constructor[@name="ConstructorTextReader"]/docs/*' />
		public INIConfigurationSource (TextReader reader)
		{
			Load (reader);
		}

		/// <include file='IniConfigSource.xml' path='//Constructor[@name="ConstructorIniDocument"]/docs/*' />
		public INIConfigurationSource (INIDocument document)
		{
			Load (document);
		}
		
		/// <include file='IniConfigSource.xml' path='//Constructor[@name="ConstructorStream"]/docs/*' />
		public INIConfigurationSource (Stream stream)
		{
			Load (stream);
		}
		#endregion
		
		#region Public properties
		/// <include file='IniConfigSource.xml' path='//Property[@name="CaseSensitive"]/docs/*' />
		public bool CaseSensitive
		{
			get { return _caseSensitive; }
			set { _caseSensitive = value; }
		}

		/// <include file='IniConfigSource.xml' path='//Property[@name="SavePath"]/docs/*' />
		public string SavePath
		{
			get { return _savePath; }
		}
		#endregion
		
		#region Public methods
		/// <include file='IniConfigSource.xml' path='//Method[@name="LoadPath"]/docs/*' />
		public void Load (string filePath)
		{
			Load (new StreamReader (filePath));
			this._savePath = filePath;
		}
		
		/// <include file='IniConfigSource.xml' path='//Method[@name="LoadTextReader"]/docs/*' />
		public void Load (TextReader reader)
		{
			Load (new INIDocument (reader));
		}

		/// <include file='IniConfigSource.xml' path='//Method[@name="LoadIniDocument"]/docs/*' />
		public void Load (INIDocument document)
		{
			this.Configurations.Clear ();

			this.Merge (this); // required for SaveAll
			_iniDocument = document;
			Load ();
		}
		
		/// <include file='IniConfigSource.xml' path='//Method[@name="LoadStream"]/docs/*' />
		public void Load (Stream stream)
		{
			Load (new StreamReader (stream));
		}

		/// <include file='IniConfigSource.xml' path='//Method[@name="Save"]/docs/*' />
		public override void Save ()
		{
			if (!IsSavable ()) {
				throw new ArgumentException ("Source cannot be saved in this state");
			}

			MergeConfigurationsIntoDocument ();
			
			_iniDocument.Save (this._savePath);
			base.Save ();
		}
		
		/// <include file='IniConfigSource.xml' path='//Method[@name="SavePath"]/docs/*' />
		public void Save (string path)
		{
			this._savePath = path;
			this.Save ();
		}
		
		/// <include file='IniConfigSource.xml' path='//Method[@name="SaveTextWriter"]/docs/*' />
		public void Save (TextWriter writer)
		{
			MergeConfigurationsIntoDocument ();
			_iniDocument.Save (writer);
			_savePath = null;
			OnSaved (new EventArgs ());
		}

		/// <include file='IniConfigSource.xml' path='//Method[@name="SaveStream"]/docs/*' />
		public void Save (Stream stream)
		{
			MergeConfigurationsIntoDocument ();
			_iniDocument.Save (stream);
			_savePath = null;
			OnSaved (new EventArgs ());
		}

		/// <include file='IConfigSource.xml' path='//Method[@name="Reload"]/docs/*' />
		public override void Reload ()
		{
			if (_savePath == null) {
				throw new ArgumentException ("Error reloading: You must have "
							+ "the loaded the source from a file");
			}

			_iniDocument = new INIDocument (_savePath);
			MergeDocumentIntoConfigurations ();
			base.Reload ();
		}

		/// <include file='IniConfigSource.xml' path='//Method[@name="ToString"]/docs/*' />
		public override string ToString ()
		{
			MergeConfigurationsIntoDocument ();
			StringWriter writer = new StringWriter ();
			_iniDocument.Save (writer);

			return writer.ToString ();
		}
		#endregion
		
		#region Private methods
		/// <summary>
		/// Merges all of the configs from the config collection into the 
		/// IniDocument before it is saved.  
		/// </summary>
		private void MergeConfigurationsIntoDocument ()
		{
			RemoveSections ();
			foreach (IConfiguration config in this.Configurations)
			{
				string[] keys = config.GetKeys ();

				// Create a new section if one doesn't exist
				if (_iniDocument.Sections[config.Name] == null) {
					INISection section = new INISection (config.Name);
					_iniDocument.Sections.Add (section);
				}
				RemoveKeys (config.Name);

				for (int i = 0; i < keys.Length; i++)
				{
					_iniDocument.Sections[config.Name].Set (keys[i], config.Get (keys[i]));
				}
			}
		}

		/// <summary>
		/// Removes all INI sections that were removed as configs.
		/// </summary>
		private void RemoveSections ()
		{
			INISection section = null;
			for (int i = 0; i < _iniDocument.Sections.Count; i++)
			{
				section = _iniDocument.Sections[i];
				if (this.Configurations[section.Name] == null) {
					_iniDocument.Sections.Remove (section.Name);
				}
			}
		}

		/// <summary>
		/// Removes all INI keys that were removed as config keys.
		/// </summary>
		private void RemoveKeys (string sectionName)
		{
			INISection section = _iniDocument.Sections[sectionName];

			if (section != null) {
				foreach (string key in section.GetKeys ())
				{
					if (this.Configurations[sectionName].Get (key) == null) {
						section.Remove (key);
					}
				}
			}
		}

		/// <summary>
		/// Loads the configuration file.
		/// </summary>
		private void Load ()
		{
			INIConfiguration config = null;
			INISection section = null;
			INIItem item = null;

			for (int j = 0; j < _iniDocument.Sections.Count; j++)
			{
				section = _iniDocument.Sections[j];
				config = new INIConfiguration (section.Name, this);

				for (int i = 0; i < section.ItemCount; i++)
				{
					item = section.GetItem (i);
					
					if  (item.Type == INIType.Key) {
						config.Add (item.Name, item.Value);
					}
				}
				
				this.Configurations.Add (config);
			}
		}

		/// <summary>
		/// Merges the IniDocument into the Configs when the document is 
		/// reloaded.  
		/// </summary>
		private void MergeDocumentIntoConfigurations ()
		{
			// Remove all missing configs first
			RemoveConfigurations ();

			INISection section = null;
			for (int i = 0; i < _iniDocument.Sections.Count; i++)
			{
				section = _iniDocument.Sections[i];

				IConfiguration config = this.Configurations[section.Name];
				if (config == null) {
					// The section is new so add it
					config = new ConfigurationBase (section.Name, this);
					this.Configurations.Add (config);
				}				
				RemoveConfigurationKeys (config);
			}
		}

		/// <summary>
		/// Removes all configs that are not in the newly loaded INI doc.  
		/// </summary>
		private void RemoveConfigurations ()
		{
			IConfiguration config = null;
			for (int i = this.Configurations.Count - 1; i > -1; i--)
			{
				config = this.Configurations[i];
				// If the section is not present in the INI doc
				if (_iniDocument.Sections[config.Name] == null) {
					this.Configurations.Remove (config);
				}
			}
		}

		/// <summary>
		/// Removes all INI keys that were removed as config keys.
		/// </summary>
		private void RemoveConfigurationKeys (IConfiguration config)
		{
			INISection section = _iniDocument.Sections[config.Name];

			// Remove old keys
			string[] configKeys = config.GetKeys ();
			foreach (string configKey in configKeys)
			{
				if (!section.Contains (configKey)) {
					// Key doesn't exist, remove
					config.Remove (configKey);
				}
			}

			// Add or set all new keys
			string[] keys = section.GetKeys ();
			for (int i = 0; i < keys.Length; i++)
			{
				string key = keys[i];
				config.Set (key, section.GetItem (i).Value);
			}
		}

		/// <summary>
		/// Returns true if this instance is savable.
		/// </summary>
		private bool IsSavable ()
		{
			return (this._savePath != null);
		}
		#endregion
	}
}