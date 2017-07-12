using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ChancyBot
{
	public class Config
	{
		// ---------------------------------------------------------------------------------------
		// Insert configurable values below:
		public string SteamUsername;
		public string SteamPassword;
        public string DiscordBotToken;
        public List<uint> AppIDList;

		// ---------------------------------------------------------------------------------------

		// Name of configuration file.
		[XmlIgnore]
		public const string FileName = "Settings.xml";

		// Globally accessable instance of loaded configuration.
		[XmlIgnore]
		public static Config Instance { get; private set; }

		// Empty constructor for XmlSerializer.
		public Config()
		{
            this.AppIDList = new List<uint>();
		}

		// Used to load the default configuration if Load() fails.
		public static void Default()
        {
			Config.Instance = new Config();

            Config.Instance.SteamUsername = "username";
            Config.Instance.SteamPassword = "password";
            Config.Instance.DiscordBotToken = "token";

            Config.Instance.AppIDList.Add(730);
		}

		// Loads the configuration from file.
		public static void Load()
		{
			var serializer = new XmlSerializer(typeof(Config));

			using (var fStream = new FileStream(Config.FileName, FileMode.Open))
				Config.Instance = (Config)serializer.Deserialize(fStream);
		}

		// Saves the configuration to file.
		public void Save()
		{
			var serializer = new XmlSerializer(typeof(Config));

			using (var fStream = new FileStream(Config.FileName, FileMode.Create))
				serializer.Serialize(fStream, this);
		}
	}
}


/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using SteamKit2;
using ChancyBot;

namespace ChancyBot
{
	static class Settings
	{
		const string SETTINGS_FILE = "Settings.xml";

		public static SettingsXml Current { get; private set; }


		static Settings()
		{
			Current = new SettingsXml();
		}


		public static void Load()
		{
			string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILE);
            Console.WriteLine("Reading file: " + settingsPath);
			Current = SettingsXml.Load(settingsPath);
		}

		public static void Save()
		{
			string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILE);

			Current.Save(settingsPath);
		}
   	}

	// the backing store of settings
	public class SettingsXml : XmlSerializable<SettingsXml>
	{

		[XmlAttribute]
		public string SteamUsername;
		[XmlAttribute]
		public string SteamPassword;

        [XmlAttribute]
		public string DiscordBotToken;


		[XmlArrayItem("App")]
		public List<ImportantApp> ImportantApps;


		public SettingsXml()
		{
			ImportantApps = new List<ImportantApp>();
   		}

		public class ImportantApp
		{
			[XmlAttribute]
			public uint AppID;
		}

	}
}*/

/*﻿using System;

namespace ChancyBot
{
    public class Settings
    {
		public static readonly string user = "hockey9044";
		public static readonly string pass = "tiptiptip123TIP";

		public static string botToken = "MzMxMjY5MzYzNzU3ODc1MjAx.DD7hCA.HkC-rEUj2gzfZfABKjF8y_TuCZ8";

        public static readonly uint[] update = { 730, 440, 240, 10 };
	}
}*/
