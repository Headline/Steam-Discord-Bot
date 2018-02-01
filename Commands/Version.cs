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
            var client = new GitHubClient(new ProductHeaderValue("Steam-Discord-Bot"));
            var releases = await client.Repository.Release.GetAll("Headline", "Steam-Discord-Bot");

            await Context.Channel.SendMessageAsync("Current version: " + Program.VERSION + " (latest is " + Helpers.GetLatestVersion(releases) + ")");
        }
    }
}