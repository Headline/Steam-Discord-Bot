using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;
using Octokit;

namespace SteamDiscordBot
{
    public static class Helpers
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
                    SocketTextChannel channel = Helpers.FindSendChannel(guild); // find #general

                    if (channel != null) // #general exists
                    {
                        await Program.Instance.Log(new LogMessage(LogSeverity.Info, "SendMsg", "Sending msg to: " + channel.Name));

                        try
                        {
                            var emb = new EmbedBuilder();
                            emb.WithDescription(input);
                            await channel.SendMessageAsync("", false, emb.Build());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not send to {0}: {1}", channel.Name, e.Message);
                        }
                    }
                }
			}
		}

        public static string[] GetGuilds()
        {
            string[] guilds = new string[Program.Instance.client.Guilds.Count];
            int count = 0;
            foreach (SocketGuild guild in Program.Instance.client.Guilds) // loop through each discord guild
            {
                guilds[count++] = guild.Name;
            }
            return guilds;
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
                        SocketTextChannel channel = Helpers.FindSendChannel(guild); // find desired channel

                        if (channel != null) // target exists
                        {
                            try
                            {
                                if (emb != null)
                                {

                                    await channel.SendMessageAsync(input, false, emb.Build());
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

        public async static void Update()
        {
            if (Program.config.GitHubAuthToken.Length == 0)
            {
                return;
            }

            try
            {
                /*
                 * We give the python script our pid, that way
                 * it can loop and wait for the application to completely
                 * exit before replacing our files
                 */
                int pid = Process.GetCurrentProcess().Id;

                var client = Program.Instance.ghClient;
                var releases = await client.Repository.Release.GetAll(Program.config.GitHubUsername, Program.config.GitHubRepository);

                string url = "https://github.com/"
                    + Program.config.GitHubUsername
                    + "/"
                    + Program.config.GitHubRepository
                    + "/releases/download/<name>/"
                    + Program.config.GitHubReleaseFilename;
                url = url.Replace("<name>", releases[0].Name);

                string command = string.Format("/k cd {0} & python updater.py {1} {2}",
                                                                Directory.GetCurrentDirectory(),
                                                                pid,
                                                                url);
                
                Process.Start("CMD.exe", command);
                Environment.Exit(0);
            }
            catch { } // ignore errors. if it failed: oh well.
        }

        public static bool IsCommandDisabled(string cmd)
        {
            foreach (string var in Program.config.DisabledCommands)
            {
                if (var.Equals(cmd))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetLatestVersion(IReadOnlyList<Release> releases)
        {
            var detectionString = "-v";
            return releases[0].Name.Substring(releases[0].Name.IndexOf(detectionString) + detectionString.Length);
        }

        public static string GetAppName(uint appid)
		{
			var json = new WebClient().DownloadString("http://store.steampowered.com/api/appdetails?appids=" + appid);
			JObject o = JObject.Parse(json);
			string name = (string)o["" + appid]["data"]["name"];
			return name;
		}

        public static string[] BuildHelpLines()
        {
            List<string> arrayList = new List<string>();

            Assembly asm = Assembly.GetExecutingAssembly(); // Get assembly

            var results = from type in asm.GetTypes()
                          where typeof(ModuleBase).IsAssignableFrom(type)
                          select type; // Grab all types that inherit ModuleBase

            foreach (Type t in results) // For each type in results
            {
                /* Grab MethodInfo of the type where the method has the attribute SummaryAttribute */
               MethodInfo info = t.GetMethods().Where(x => x.GetCustomAttributes(typeof(SummaryAttribute), false).Length > 0).First();

                /* Grab summary attribute */
                SummaryAttribute summary = info.GetCustomAttribute(typeof(SummaryAttribute)) as SummaryAttribute;

                /* Grab command attribute */
                CommandAttribute command = info.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;

                /* Both objects are non null, valid, so lets grab the attribute text */
                if (summary != null && command != null)
                {
                    if (!Helpers.IsCommandDisabled(command.Text))
                        arrayList.Add("!" + command.Text + " - " + summary.Text);
                }
            }

            return arrayList.ToArray(); // return string[] array
        }
    }
}
