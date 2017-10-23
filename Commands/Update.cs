using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using System.Diagnostics;
using Octokit;
using System.IO;

namespace ChancyBot.Commands
{
    public class UpdateCommand : ModuleBase
    {
        [Command("update"), Summary("Updates and reloads the bot.")]
        public async Task Say()
        {
            int pid = Process.GetCurrentProcess().Id;

            var client = new GitHubClient(new ProductHeaderValue("my-cool-app"));
            var releases = await client.Repository.Release.GetAll("Headline22", "Steam-Discord-Bot");

            string url = "https://github.com/Headline22/Steam-Discord-Bot/releases/download/<name>/steam-discord-bot.zip";
            url = url.Replace("<name>", releases[0].Name);

            await Context.Channel.SendMessageAsync("Okay. Updating...");

            string command = string.Format("/k cd {0} & python updater.py {1} {2}",
                                                            Directory.GetCurrentDirectory(),
                                                            pid,
                                                            url);

            Process.Start("CMD.exe", command);
            Environment.Exit(0);
        }
    }
}