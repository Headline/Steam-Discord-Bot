using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class CoinCommand : ModuleBase
    {
        [Command("coin"), Summary("Flips a coin.")]
        public async Task Say()
        {
            Random rand = Program.Instance.random;
        	
        	int num = rand.Next(1, 3);

            var emb = new EmbedBuilder();
            emb.Title = "Coin Toss!";
            emb.WithDescription(string.Format("**{0}**", (num == 1) ? "Heads" : "Tails"));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
        }
    }
}