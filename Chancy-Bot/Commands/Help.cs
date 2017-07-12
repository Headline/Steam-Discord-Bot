using System;
using System.Net;
using System.Threading.Tasks;

using Discord.Commands;

using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ChancyBot.Commands
{
    public class HelpCommand : ModuleBase
    {
        [Command("help"), Summary("Prints and formats all commands.")]
        public async Task Say()
        {
            await Context.Channel.SendMessageAsync("No.");
        }
    }
}