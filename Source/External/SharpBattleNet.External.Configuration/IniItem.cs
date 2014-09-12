using System;

namespace SharpBattleNet.External.Configuration.Source.INI
{
		/// <include file='IniItem.xml' path='//Class[@name="IniItem"]/docs/*' />
		public class INIItem
		{
			#region Private variables
			private INIType _iniType = INIType.Empty;
			private string _iniName = "";
			private string _iniValue = "";
			private string _iniComment = null;
			#endregion
			
			#region Public properties
			/// <include file='IniItem.xml' path='//Property[@name="Type"]/docs/*' />
			public INIType Type
			{
				get { return _iniType; }
				set { _iniType = value; }
			}
			
			/// <include file='IniItem.xml' path='//Property[@name="Value"]/docs/*' />
			public string Value
			{
				get { return _iniValue; }
				set { _iniValue = value; }
			}
			
			/// <include file='IniItem.xml' path='//Property[@name="Name"]/docs/*' />
			public string Name
			{
				get { return _iniName; }
			}
			
			/// <include file='IniItem.xml' path='//Property[@name="Comment"]/docs/*' />
			public string Comment
			{
				get { return _iniComment; }
				set { _iniComment = value; }
			}
			#endregion
			
			/// <include file='IniItem.xml' path='//Constructor[@name="Constructor"]/docs/*' />
			internal protected INIItem (string name, string value, INIType type, string comment)
			{
				_iniName = name;
				_iniValue = value;
				_iniType = type;
				_iniComment = comment;
			}
		}
}

