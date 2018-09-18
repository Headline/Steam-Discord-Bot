using Discord;
using Discord.Commands;

using System.Threading.Tasks;
using System.Collections.Generic;

using Octokit;

namespace SteamDiscordBot.Commands
{
    public class VersionCommand : ModuleBase
    {
        [Command("version"), Summary("Get the current bot version.")]
        public async Task Say()
        {
            var client = Program.Instance.ghClient;
            var releases = await client.Repository.Release.GetAll(Program.config.GitHubUpdateUsername, Program.config.GitHubUpdateRepository);

            var emb = new EmbedBuilder();
            emb.Title = "Version";
            emb.WithDescription("Current version: " + Program.VERSION + " (latest is " + GetLatestVersion(releases) + ")");
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }

        public static string GetLatestVersion(IReadOnlyList<Release> releases)
        {
            var detectionString = "-v";
            return releases[0].Name.Substring(releases[0].Name.IndexOf(detectionString) + detectionString.Length);
        }
    }
}