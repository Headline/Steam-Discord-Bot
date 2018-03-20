using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class LMGTFYCommand : ModuleBase
    {
        public static readonly string url = "http://lmgtfy.com/?q=";

        [Command("coin"), Summary("Flips a coin.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {

                input += args[i].Trim() + "+";
            }
            input += args[args.Length - 1];

            input = url + input;
            await Context.Channel.SendMessageAsync("URL Fetched: " + input);
        }
    }
}