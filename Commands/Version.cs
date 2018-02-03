using System.Threading.Tasks;

using Discord.Commands;
using Octokit;


namespace ChancyBot.Commands
{
    public class VersionCommand : ModuleBase
    {
        [Command("version"), Summary("Get the current bot version.")]
        public async Task Say()
        {
            var client = Program.Instance.ghClient;
            var releases = Program.Instance.ghReleases;

            await Context.Channel.SendMessageAsync("Current version: " + Program.VERSION + " (latest is " + Helpers.GetLatestVersion(releases) + ")");
        }
    }
}