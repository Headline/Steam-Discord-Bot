using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
	public class DiceCommand : ModuleBase
	{
		[Command("roll"), Summary("rolls a dice of arbitrary size. (example: \'!roll 33\' rolls a 33-sided die)")]
		public async Task Say(string argSize)
		{
            Random rand = Program.Instance.random;
            uint num = 0;
			num = (uint) rand.Next(1, (int) Convert.ToUInt32(argSize, 10));

            var emb = new EmbedBuilder();
            emb.Title = "Dice Roll";
            emb.WithDescription(string.Format("You rolled a **{0}**", num));
            emb.Color = Color.Red;

            await Context.Channel.SendMessageAsync("", false, emb);
		}
	}
}
