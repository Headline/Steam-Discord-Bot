using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
            emb.WithDescription("Current version: " + Program.VERSION + " (latest is " + Helpers.GetLatestVersion(releases) + ")");
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}