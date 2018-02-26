using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class PingCommand : ModuleBase
    {
        [Command("ping"), Summary("Test command.")]
        public async Task Say()
        {
            await Context.Channel.SendMessageAsync("Pong.");
        }
    }
}
