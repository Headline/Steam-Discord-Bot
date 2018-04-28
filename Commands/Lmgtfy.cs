using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class LMGTFYCommand : ModuleBase
    {
        public static readonly string url = "http://lmgtfy.com/?q=";

        [Command("lmgtfy"), Summary("Responds with a lmgtfy link")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {

                input += args[i].Trim() + "+";
            }
            input += args[args.Length - 1];

            input = url + input;

            var emb = new EmbedBuilder();
            emb.Title = "LMGTFY";
            emb.WithDescription("URL Fetched: " + input);
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}