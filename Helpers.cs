using System.Net;
using System.Linq;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ChancyBot
{
    public class Helpers
    {
        public static readonly string[] sendchannels = { "announc", "general" };

        public static int GuildHasChannel(IReadOnlyCollection<SocketTextChannel> channels, string channelName)
        {
            bool found = false;
            int index = 0;
            
            while (!found && index < channels.Count)
            {
                if (channels.ElementAt(index).Name.Contains(channelName))
                {
                    found = true;
                }
                else
                {
                    index++;
                }
            }

            return (found)?index:-1;
        }

        // Using the static channels array, it finds the first matching channel to send the message to.
        // The order of channels in sendchannels array matters.
		public static SocketTextChannel FindSendChannel(SocketGuild guild)
		{
            int i = 0;
            int index = -1;
            bool found = false;
            while (!found && i < sendchannels.Length)
            {
                index = GuildHasChannel(guild.TextChannels, sendchannels[i]);
                if (index != -1)
                {
                    found = true;
                }
                else
                {
                    i++;
                }
            }

            return (found && index != -1) ? guild.TextChannels.ElementAt(index) : null;
        }
        
        // Sends mass message to all discord guilds the bot is connected to.
        public async static void SendMessageAllToGenerals(string input)
		{
            Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMessageAll", "Text: " + input));

            foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
			{
				SocketTextChannel channel = Helpers.FindSendChannel(guild); // find #general

				await Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMsg", "Sending msg to: " + channel.Name));

				if (channel != null) // #general exists
				{
					await channel.SendMessageAsync(input);
				}
			}
		}

        // Sends a message to a targeted discord guild
        public static async void SendMessageAllToTarget(string target, string input)
        {
            Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMessageTarget", "MSG To "+ target + ": " + input));

            foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
            {
                if (guild.Name.ToLower().Contains(target.ToLower())) // find target 
                {
                    SocketTextChannel channel = Helpers.FindSendChannel(guild); // find desired channel

                    if (channel != null) // target exists
                    {
                        await channel.SendMessageAsync(input);
                    }
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
