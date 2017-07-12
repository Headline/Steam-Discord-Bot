using System;
using System.Net;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace ChancyBot
{
    public class Helpers
    {
		public static SocketTextChannel FindGeneralChannel(SocketGuild guild)
		{
			Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMsg", "Trying to find #general in: " + guild.Name));

			foreach (SocketTextChannel channel in guild.TextChannels)
			{
				if (channel.Name.Equals("general") || channel.Name.Equals("#general"))
				{
					return channel;
				}
			}

			return null;
		}

		public static void SendMessageAllToGenerals(string input)
		{
			foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
			{
				SocketTextChannel channel = Helpers.FindGeneralChannel(guild); // find #general

				Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMsg", "Sending msg to: " + channel.Name));

				if (channel != null) // #general exists
				{
					channel.SendMessageAsync(input);
				}
			}
		}

		public static string GetAppName(uint appid)
		{
			var json = new WebClient().DownloadString("http://store.steampowered.com/api/appdetails?appids=" + appid);
			JObject o = JObject.Parse(json);
			string name = (string)o["" + appid]["data"]["name"];
			return name;
		}
	}
}
