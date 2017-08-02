using System;
using System.Net;

using ChancyBot;
using SteamKit2;
using Discord;

using ChancyBot.Steam;
using System.Threading;

namespace ChancyBot.Jobs
{
	public class SteamCheckJob : Job
	{
        SteamConnection connection;

        public SteamCheckJob(SteamConnection connection)
		{
            this.connection = connection;
		}

		public override void OnRun()
		{
            if (!this.connection.isRunning)
            {
				Program.Instance.Log(new LogMessage(LogSeverity.Info, "SteamCheck", "Steam connection was disconnected. Rebooting..."));

                // Connect to steam and pump callbacks 
                connection = new SteamConnection(Config.Instance.SteamUsername, Config.Instance.SteamPassword);
                new Thread(new ThreadStart(() =>
                {
                    connection.Connect();
                })).Start();
            }
		}
	}
}
