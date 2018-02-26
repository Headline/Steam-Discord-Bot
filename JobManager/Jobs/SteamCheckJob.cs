using Discord;

using SteamDiscordBot.Steam;

namespace SteamDiscordBot.Jobs
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
                connection.Connect();
            }
		}
	}
}
