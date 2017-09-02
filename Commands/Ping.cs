using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;

namespace ChancyBot.Commands
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
