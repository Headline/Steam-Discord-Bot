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
    public class CoinCommand : ModuleBase
    {
        [Command("coin"), Summary("Flips a coin.")]
        public async Task Say()
        {
        	Random rand = new Random();
        	
        	int num = rand.Next(1, 3);

        	if (num == 1)
	            await Context.Channel.SendMessageAsync("Heads");
            else
	            await Context.Channel.SendMessageAsync("Tails");
        }
    }
}