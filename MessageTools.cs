using System;
using System.Linq;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;


namespace SteamDiscordBot
{
    public static class MessageTools
    {
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

        // Using the settings.json AnnouncePrefs, it finds the first matching channel to send the message to.
        // The order of channels in sendchannels array matters.
		public static SocketTextChannel FindSendChannel(SocketGuild guild)
		{
            int i = 0;
            int index = -1;
            bool found = false;
            while (!found && i < Program.config.AnnouncePrefs.Length)
            {
                index = GuildHasChannel(guild.TextChannels, Program.config.AnnouncePrefs[i]);
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
            await Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMessageAll", "Text: " + input));

            foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
			{
                var usr = guild.CurrentUser;
                if (usr.GuildPermissions.Has(GuildPermission.SendMessages))
                {
                    SocketTextChannel channel = MessageTools.FindSendChannel(guild); // find #general

                    if (channel != null) // #general exists
                    {
                        await Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMsg", "Sending msg to: " + channel.Name));

                        try
                        {
                            var emb = new EmbedBuilder();
                            emb.WithDescription(input);
                            emb.Color = Discord.Color.Red;
                            await channel.SendMessageAsync("", false, emb);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not send to {0}: {1}", channel.Name, e.Message);
                        }
                    }
                }
			}
		}

        // Sends a message to a targeted discord guild
        public static async void SendMessageAllToTarget(string target, string input, EmbedBuilder emb = null)
        {
            await Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMessageTarget", "MSG To "+ target + ": " + input));

            foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
            {
                if (guild.Name.ToLower().Contains(target.ToLower())) // find target 
                {
                    var usr = guild.CurrentUser;
                    if (usr.GuildPermissions.Has(GuildPermission.SendMessages))
                    {
                        SocketTextChannel channel = MessageTools.FindSendChannel(guild); // find desired channel

                        if (channel != null) // target exists
                        {
                            try
                            {
                                if (emb != null)
                                {

                                    await channel.SendMessageAsync(input, false, emb);
                                }
                                else
                                {
                                    await channel.SendMessageAsync(input);
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Could not send to {0}: {1}", channel.Name, e.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
