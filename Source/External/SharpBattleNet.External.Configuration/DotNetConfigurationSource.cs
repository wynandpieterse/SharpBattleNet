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
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;

namespace SharpBattleNet.External.Configuration.Source.DotNetConfiguration
{
	/// <include file='DotNetConfigSource.xml' path='//Class[@name="DotNetConfigSource"]/docs/*' />
	public class DotNetConfigurationSource : ConfigurationSourceBase
	{
		#region Private variables
		private string[] _sections = null;
		private XmlDocument _configurationDocument = null;
		private string _savePath = null;
		#endregion

		#region Constructors
		/// <include file='DotNetConfigSource.xml' path='//Constructor[@name="ConstructorWeb"]/docs/*' />
		public DotNetConfigurationSource (string[] sections)
		{
			this._sections = sections;
			Load ();
		}

		/// <include file='DotNetConfigSource.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public DotNetConfigurationSource ()
		{
			_configurationDocument = new XmlDocument ();
			_configurationDocument.LoadXml ("<configuration><configSections/></configuration>");
			PerformLoad (_configurationDocument);
		}

		/// <include file='DotNetConfigSource.xml' path='//Constructor[@name="ConstructorPath"]/docs/*' />
		public DotNetConfigurationSource (string path)
		{
			Load (path);
		}
		
		/// <include file='DotNetConfigSource.xml' path='//Constructor[@name="ConstructorXmlReader"]/docs/*' />
		public DotNetConfigurationSource (XmlReader reader)
		{
			Load (reader);
		}
		#endregion
		
		#region Public properties
		/// <include file='DotNetConfigSource.xml' path='//Property[@name="SavePath"]/docs/*' />
		public string SavePath
		{
			get { return _savePath; }
		}
		#endregion
		
		#region Public methods
		/// <include file='DotNetConfigSource.xml' path='//Method[@name="LoadPath"]/docs/*' />
		public void Load (string path)
		{
			_savePath = path;
			_configurationDocument = new XmlDocument ();
			_configurationDocument.Load (_savePath);
			PerformLoad (_configurationDocument);
		}
		
		/// <include file='DotNetConfigSource.xml' path='//Method[@name="LoadXmlReader"]/docs/*' />
		public void Load (XmlReader reader)
		{
			_configurationDocument = new XmlDocument ();
			_configurationDocument.Load (reader);
			PerformLoad (_configurationDocument);
		}

		/// <include file='DotNetConfigSource.xml' path='//Method[@name="Save"]/docs/*' />
		public override void Save ()
		{
			if (!IsSavable ()) {
				throw new ArgumentException ("Source cannot be saved in this state");
			}
			MergeConfigurationsIntoDocument ();
		
			_configurationDocument.Save (_savePath);
			base.Save ();
		}
		
		/// <include file='DotNetConfigSource.xml' path='//Method[@name="SavePath"]/docs/*' />
		public void Save (string path)
		{
			if (!IsSavable ()) {
				throw new ArgumentException ("Source cannot be saved in this state");
			}

			_savePath = path;
			this.Save ();
		}
		
		/// <include file='DotNetConfigSource.xml' path='//Method[@name="SaveTextWriter"]/docs/*' />
		public void Save (TextWriter writer)
		{
			if (!IsSavable ()) {
				throw new ArgumentException ("Source cannot be saved in this state");
			}

			MergeConfigurationsIntoDocument ();
			_configurationDocument.Save (writer);
			_savePath = null;
			OnSaved (new EventArgs ());
		}

		/// <include file='DotNetConfigSource.xml' path='//Method[@name="SaveStream"]/docs/*' />
		public void Save (Stream stream)
		{
			if (!IsSavable ()) {
				throw new ArgumentException ("Source cannot be saved in this state");
			}

			MergeConfigurationsIntoDocument ();
			_configurationDocument.Save (stream);
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

			_configurationDocument = new XmlDocument ();
			_configurationDocument.Load (_savePath);
			MergeDocumentIntoConfigurations ();
			base.Reload ();
		}

		/// <include file='DotNetConfigSource.xml' path='//Method[@name="ToString"]/docs/*' />
		public override string ToString ()
		{
			MergeConfigurationsIntoDocument ();
			StringWriter writer = new StringWriter ();
			_configurationDocument.Save (writer);

			return writer.ToString ();
		}

#if (NET_COMPACT_1_0)
#else
		/// <include file='DotNetConfigSource.xml' path='//Method[@name="GetFullConfigPath"]/docs/*' />
		public static string GetFullConfigPath ()
		{
			return (Assembly.GetCallingAssembly().Location + ".config");
		}
#endif
		#endregion

		#region Private methods
		/// <summary>
		/// Merges all of the configs from the config collection into the 
		/// XmlDocument.
		/// </summary>
		private void MergeConfigurationsIntoDocument ()
		{
			RemoveSections ();
			foreach (IConfiguration config in this.Configurations)
			{
				string[] keys = config.GetKeys ();

				RemoveKeys (config.Name);
				XmlNode node = GetChildElement (config.Name);
				if (node == null) {
					node = SectionNode (config.Name);
				}
				
				for (int i = 0; i < keys.Length; i++)
				{
					SetKey (node, keys[i], config.Get (keys[i]));
				}
			}
		}

		/// <summary>
		/// Loads all collection classes.
		/// </summary>
		private void Load ()
		{
#if (NET_COMPACT_1_0)
			throw new NotSupportedException ("This loading method is not supported");
#else
			this.Merge (this); // required for SaveAll
			for (int i = 0; i < _sections.Length; i++)
			{
				LoadCollection (_sections[i], (NameValueCollection)ConfigurationSettings
								.GetConfig (_sections[i]));
			}
#endif
		}
		
		/// <summary>
		/// Loads all sections and keys.
		/// </summary>
		private void PerformLoad (XmlDocument document)
		{
			this.Configurations.Clear ();

			this.Merge (this); // required for SaveAll

			if (document.DocumentElement.Name != "configuration") {
				throw new ArgumentException ("Did not find configuration node");
			}

			LoadSections (document.DocumentElement);
		}
		
		/// <summary>
		/// Loads all configuration sections.
		/// </summary>
		private void LoadSections (XmlNode rootNode)
		{
			LoadOtherSection (rootNode, "appSettings");

			XmlNode sections = GetChildElement (rootNode, "configSections");

			if (sections == null) {
				// There is no configSections node so exit
				return;
			}

			ConfigurationBase config = null;
			foreach (XmlNode node in sections.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "section") {
					config = new ConfigurationBase 
							(node.Attributes["name"].Value, this);
				
					this.Configurations.Add (config);
					LoadKeys (rootNode, config);
				}
			}
		}
		
		/// <summary>
		/// Loads special sections that are not loaded in the configSections
		/// node.  This includes such sections such as appSettings.
		/// </summary>
		private void LoadOtherSection (XmlNode rootNode, string nodeName)
		{
			XmlNode section = GetChildElement (rootNode, nodeName);
			ConfigurationBase config = null;
			
			if (section != null) {
				config = new ConfigurationBase (section.Name, this);
				
				this.Configurations.Add (config);
				LoadKeys (rootNode, config);
			}
		}
		
		/// <summary>
		/// Loads all keys for a config.
		/// </summary>
		private void LoadKeys (XmlNode rootNode, ConfigurationBase config)
		{
			XmlNode section = GetChildElement (rootNode, config.Name);

			foreach (XmlNode node in section.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "add") {
					config.Add (node.Attributes["key"].Value,
								node.Attributes["value"].Value);
				}
			}
		}
		
		/// <summary>
		/// Removes all XML sections that were removed as configs.
		/// </summary>
		private void RemoveSections ()
		{
			XmlAttribute attr = null;
			XmlNode sections = GetChildElement ("configSections");

			if (sections == null) {
				// There is no configSections node so exit
				return;
			}
			
			foreach (XmlNode node in sections.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "section") {
					attr = node.Attributes["name"];
					if (attr != null) {
						if (this.Configurations[attr.Value] == null) {
							// Removes the configSections section
							node.ParentNode.RemoveChild (node);

							// Removes the <SectionName> section
							XmlNode dataNode = GetChildElement (attr.Value);
							if (dataNode != null) {
								_configurationDocument.DocumentElement.RemoveChild (dataNode);
							}
						}
					} else {
						throw new ArgumentException ("Section name attribute not found");
					}
				}
			}
		}
		
		/// <summary>
		/// Removes all XML keys that were removed as config keys.
		/// </summary>
		private void RemoveKeys (string sectionName)
		{
			XmlNode node = GetChildElement (sectionName);
			XmlAttribute keyName = null;
			
			if (node != null) {
				foreach (XmlNode key in node.ChildNodes)
				{
					if (key.NodeType == XmlNodeType.Element
						&& key.Name == "add") {
						keyName = key.Attributes["key"];
						if (keyName != null) {
							if (this.Configurations[sectionName].Get (keyName.Value) == null) {
								node.RemoveChild (key);
							}
						} else {
							throw new ArgumentException ("Key attribute not found in node");
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Sets an XML key.  If it does not exist then it is created.
		/// </summary>
		private void SetKey (XmlNode sectionNode, string key, string value)
		{
			XmlNode keyNode = GetKey (sectionNode, key);
			
			if (keyNode == null) {
				CreateKey (sectionNode, key, value);
			} else {
				keyNode.Attributes["value"].Value = value;
			}
		}

		/// <summary>
		/// Gets an XML key by it's name. Returns null if it does not exist.
		/// </summary>
		private XmlNode GetKey (XmlNode sectionNode, string keyName)
		{
			XmlNode result = null;

			foreach (XmlNode node in sectionNode.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "add"
					&& node.Attributes["key"].Value == keyName) {
					result = node;
					break;
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// Creates a key node and adds it to the collection at the end.
		/// </summary>
		private void CreateKey (XmlNode sectionNode, string key, string value)
		{
			XmlNode node = _configurationDocument.CreateElement ("add");
			XmlAttribute keyAttr = _configurationDocument.CreateAttribute ("key");
			XmlAttribute valueAttr = _configurationDocument.CreateAttribute ("value");
			keyAttr.Value = key;
			valueAttr.Value = value;

			node.Attributes.Append (keyAttr);
			node.Attributes.Append (valueAttr);

			sectionNode.AppendChild (node);
		}

		/// <summary>
		/// Loads a collection class.
		/// </summary>
		private void LoadCollection (string name, NameValueCollection collection)
		{
			ConfigurationBase config = new ConfigurationBase (name, this);

			if (collection == null) {
				throw new ArgumentException ("Section was not found");
			}

			if (collection != null) {
				for (int i = 0; i < collection.Count; i++)
				{
					config.Add (collection.Keys[i], collection[i]);
				}
				
				this.Configurations.Add (config);
			}
		}
		
		/// <summary>
		/// Returns a new section node.
		/// </summary>
		private XmlNode SectionNode (string name)
		{
			// Add node for configSections node
			XmlNode node = _configurationDocument.CreateElement ("section");
			XmlAttribute attr = _configurationDocument.CreateAttribute ("name");
			attr.Value = name;
			node.Attributes.Append (attr);
			
			attr = _configurationDocument.CreateAttribute ("type");
			attr.Value = "System.Configuration.NameValueSectionHandler";
			node.Attributes.Append (attr);

			XmlNode section = GetChildElement ("configSections");
			section.AppendChild (node);
		
			// Add node for configuration node
			XmlNode result = _configurationDocument.CreateElement (name);
			_configurationDocument.DocumentElement.AppendChild (result);
			
			return result;
		}
		
		/// <summary>
		/// Returns true if this instance is savable.
		/// </summary>
		private bool IsSavable ()
		{
			return (this._savePath != null
					|| _configurationDocument != null);
		}

		/// <summary>
		/// Returns the single named child element.
		/// </summary>
		private XmlNode GetChildElement (XmlNode parentNode, string name)
		{
			XmlNode result = null;

			foreach (XmlNode node in parentNode.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == name) {
					result = node;
					break;
				}
			}

			return result;
		}
		
		/// <summary>
		/// Returns a child element from the XmlDocument.DocumentElement.
		/// </summary>
		private XmlNode GetChildElement (string name)
		{
			return GetChildElement (_configurationDocument.DocumentElement, name);
		}
		
		/// <summary>
		/// Merges the XmlDocument into the Configs when the document is 
		/// reloaded.  
		/// </summary>
		private void MergeDocumentIntoConfigurations ()
		{
			// Remove all missing configs first
			RemoveConfigurations ();
			
			XmlNode sections = GetChildElement ("configSections");

			if (sections == null) {
				// There is no configSections node so exit
				return;
			}
			
			foreach (XmlNode node in sections.ChildNodes)
			{
				// Find all section nodes
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "section") {
					
					string sectionName = node.Attributes["name"].Value;
					IConfiguration config = this.Configurations[sectionName];
					if (config == null) {
						// The section is new so add it
						config = new ConfigurationBase (sectionName, this);
						this.Configurations.Add (config);
					}				
					RemoveConfigurationKey (config);
				}
			}
		}

		/// <summary>
		/// Removes all configs that are not in the newly loaded XmlDocument.  
		/// </summary>
		private void RemoveConfigurations ()
		{
			IConfiguration config = null;
			for (int i = this.Configurations.Count - 1; i > -1; i--)
			{
				config = this.Configurations[i];
				// If the section is not present in the XmlDocument
				if (GetChildElement (config.Name) == null) {
					this.Configurations.Remove (config);
				}
			}
		}

		/// <summary>
		/// Removes all XML keys that were removed as config keys.
		/// </summary>
		private void RemoveConfigurationKey (IConfiguration config)
		{
			XmlNode section = GetChildElement (config.Name);
			
			// Remove old keys
			string[] configKeys = config.GetKeys ();
			foreach (string configKey in configKeys)
			{
				if (GetKey (section, configKey) == null) {
					// Key doesn't exist, remove
					config.Remove (configKey);
				}
			}

			// Add or set all new keys
			foreach (XmlNode node in section.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.Element
					&& node.Name == "add") {
					config.Set (node.Attributes["key"].Value,
								node.Attributes["value"].Value);
				}
			}
		}
		#endregion
	}
}