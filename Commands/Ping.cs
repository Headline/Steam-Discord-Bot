using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class PingCommand : ModuleBase
    {
        [Command("ping"), Summary("Test command.")]
        public async Task Say()
        {
            var emb = new EmbedBuilder();
            emb.Title = "Pong!";
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}
