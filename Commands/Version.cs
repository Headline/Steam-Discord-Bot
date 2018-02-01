using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;
using Octokit;

using System.Diagnostics;
using Octokit;
using System.IO;

namespace ChancyBot.Commands
{
    public class VersionCommand : ModuleBase
    {
        [Command("version"), Summary("Get the current bot version.")]
        public async Task Say()
        {
            var client = new GitHubClient(new ProductHeaderValue("Steam-Discord-Bot"));
            var releases = await client.Repository.Release.GetAll("Headline", "Steam-Discord-Bot");

            var detectionString = "-v";
            string version = releases[0].Name.Substring(releases[0].Name.IndexOf(detectionString)) + detectionString.Length;

            await Context.Channel.SendMessageAsync("Current version: " + Program.VERSION + " (latest is " + version + ")");
        }
    }
}