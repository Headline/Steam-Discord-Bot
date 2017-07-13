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
		public List<string> RepoList;

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
            this.RepoList = new List<string>();
		}

		// Used to load the default configuration if Load() fails.
		public static void Default()
        {
			Config.Instance = new Config();

            Config.Instance.SteamUsername = "username";
            Config.Instance.SteamPassword = "password";
            Config.Instance.DiscordBotToken = "token";

            Config.Instance.AppIDList.Add(730); // default add csgo
            Config.Instance.RepoList.Add("alliedmodders/sourcemod");
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