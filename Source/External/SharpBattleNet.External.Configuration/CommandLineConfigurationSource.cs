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
using System.Text;
using System.Collections;

namespace SharpBattleNet.External.Configuration.Source.CommandLine
{
	/// <include file='ArgvConfigSource.xml' path='//Class[@name="ArgvConfigSource"]/docs/*' />
	public class CommandLineConfigurationSource : ConfigurationSourceBase
	{
		#region Private variables
		private CommandLineArgumentParser _parser = null;
		private string[] _arguments = null;
		#endregion

		#region Constructors
		/// <include file='ArgvConfigSource.xml' path='//Constructor[@name="Constructor"]/docs/*' />
		public CommandLineConfigurationSource (string[] arguments)
		{
			_parser = new CommandLineArgumentParser (arguments);
			this._arguments = arguments;
		}
		#endregion
		
		#region Public properties
		#endregion
		
		#region Public methods
		/// <include file='ArgvConfigSource.xml' path='//Method[@name="Save"]/docs/*' />
		public override void Save ()
		{
			throw new ArgumentException ("Source is read only");
		}

		/// <include file='ArgvConfigSource.xml' path='//Method[@name="Reload"]/docs/*' />
		public override void Reload ()
		{
			throw new ArgumentException ("Source cannot be reloaded");
		}
		
		/// <include file='ArgvConfigSource.xml' path='//Method[@name="AddSwitch"]/docs/*' />
		public void AddSwitch (string configName, string longName)
		{
			AddSwitch (configName, longName, null);
		}
		
		/// <include file='ArgvConfigSource.xml' path='//Method[@name="AddSwitchShort"]/docs/*' />
		public void AddSwitch (string configName, string longName, 
								string shortName)
		{
			IConfiguration config = GetConfiguration (configName);
			
			if (shortName != null && 
				(shortName.Length < 1 || shortName.Length > 2)) {
				throw new ArgumentException ("Short name may only be 1 or 2 characters");
			}

			// Look for the long name first
			if (_parser[longName] != null) {
				config.Set (longName, _parser[longName]);
			} else if (shortName != null && _parser[shortName] != null) {
				config.Set (longName, _parser[shortName]);
			}
		}
		
		/// <include file='ArgvConfigSource.xml' path='//Method[@name="GetArguments"]/docs/*' />
		public string[] GetArguments ()
		{
			string[] result = new string[this._arguments.Length];
			Array.Copy (this._arguments, 0, result, 0, this._arguments.Length);

			return result;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Returns an IConfig.  If it does not exist then it is added.
		/// </summary>
		private IConfiguration GetConfiguration (string name)
		{
			IConfiguration result = null;
			
			if (this.Configurations[name] == null) {
				result = new ConfigurationBase (name, this);
				this.Configurations.Add (result);
			} else {
				result = this.Configurations[name];
			}
			
			return result;
		}
		#endregion
	}
}