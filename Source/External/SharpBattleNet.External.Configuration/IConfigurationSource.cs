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

namespace SharpBattleNet.External.Configuration
{
	/// <include file='IConfigSource.xml' path='//Interface[@name="IConfigSource"]/docs/*' />
	public interface IConfigurationSource
	{
		/// <include file='IConfigSource.xml' path='//Property[@name="Configs"]/docs/*' />
		ConfigurationCollection Configurations { get; }
		
		/// <include file='IConfigSource.xml' path='//Property[@name="AutoSave"]/docs/*' />
		bool AutoSave { get; set; }
		
		/// <include file='IConfigSource.xml' path='//Property[@name="Alias"]/docs/*' />
		AliasText Alias { get; }
		
		/// <include file='IConfigSource.xml' path='//Method[@name="Merge"]/docs/*' />
		void Merge (IConfigurationSource source);
		
		/// <include file='IConfigSource.xml' path='//Method[@name="Save"]/docs/*' />
		void Save ();

		/// <include file='IConfigSource.xml' path='//Method[@name="Reload"]/docs/*' />
		void Reload ();
		
		/// <include file='IConfigSource.xml' path='//Method[@name="AddConfig"]/docs/*' />
		IConfiguration AddConfiguration (string name);

		/// <include file='IConfigSource.xml' path='//Method[@name="GetExpanded"]/docs/*' />
		string GetExpanded (IConfiguration config, string key);

		/// <include file='IConfigSource.xml' path='//Method[@name="ExpandKeyValues"]/docs/*' />
		void ExpandKeyValues ();
		
		/// <include file='IConfigSource.xml' path='//Method[@name="ReplaceKeyValues"]/docs/*' />
		void ReplaceKeyValues ();

		/// <include file='IConfigSource.xml' path='//Event[@name="Reloaded"]/docs/*' />
		event EventHandler Reloaded;

		/// <include file='IConfigSource.xml' path='//Event[@name="Saved"]/docs/*' />
		event EventHandler Saved;
	}
}